/*
Copyright (c) 2000, Sean O'Neil (s_p_oneil@hotmail.com)
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions
are met:

* Redistributions of source code must retain the above copyright notice,
  this list of conditions and the following disclaimer.
* Redistributions in binary form must reproduce the above copyright notice,
  this list of conditions and the following disclaimer in the documentation
  and/or other materials provided with the distribution.
* Neither the name of the project nor the names of its contributors may be
  used to endorse or promote products derived from this software without
  specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

#include "StdAfx.h"
#include "PlanetaryMap.h"
#include "PlanetaryMapNode.h"


void CPlanetaryMap::Init(CPropertySet &prop)
{
	Cleanup();

	m_texArray.Init(1024, HEIGHT_MAP_WIDTH, 3, GL_RGB, GL_UNSIGNED_BYTE);
	m_shNoAtmosphere.Init("shaders/BumpVert.glsl", "shaders/BumpFrag.glsl");
	m_shGroundFromSpace.Init("shaders/BumpGroundFromSpaceVert.glsl", "shaders/BumpGroundFromSpaceFrag.glsl");
	m_shGroundFromAtmosphere.Init("shaders/BumpGroundFromAtmoVert.glsl", "shaders/BumpGroundFromAtmoFrag.glsl");
	m_shSkyFromSpace.Init("shaders/SkyFromSpaceVert.glsl", "shaders/SkyFromSpaceFrag.glsl");
	m_shSkyFromAtmosphere.Init("shaders/SkyFromAtmosphereVert.glsl", "shaders/SkyFromAtmosphereFrag.glsl");

	// Get all the simple properties for this planet
	sscanf(prop.GetProperty("position"), "%f, %f, %f", &m_vTranslate.x, &m_vTranslate.y, &m_vTranslate.z);
	m_fRadius = prop.GetFloatProperty("radius");
	m_fFrustumRadius = prop.GetFloatProperty("frustum.radius");
	m_fMaxHeight = prop.GetFloatProperty("max.height");
	m_fAtmosphereRadius = prop.GetFloatProperty("atmosphere.radius", 0.0f);
	if(m_fAtmosphereRadius > 0.0f)
	{
		sscanf(prop.GetProperty("atmosphere.wavelength"), "%f, %f, %f", &m_fWavelength[0], &m_fWavelength[1], &m_fWavelength[2]);
		m_fKr = prop.GetFloatProperty("atmosphere.Kr");
		m_fKm = prop.GetFloatProperty("atmosphere.Km");
		m_fEsun = prop.GetFloatProperty("atmosphere.Esun");
		m_fG = prop.GetFloatProperty("atmosphere.g");
	}

	// Now build the factories for this planet
	std::string strPrefix = prop.GetPrefix();
	int i;
	for(i=1; ; i++)
	{
		char szPrefix[256];
		sprintf(szPrefix, "%sfactory.%d.", strPrefix.c_str(), i);
		prop.SetPrefix(szPrefix);
		const char *pszType = prop.GetProperty("type");
		if(!pszType)
			break;

		CPlanetaryFactory *pFactory = (CPlanetaryFactory *)CObjectType::CreateObject(pszType);
		m_llFactories.push_back(pFactory);
		pFactory->Init(prop);
	}
	prop.SetPrefix(strPrefix.c_str());

	int nFace;
	for(nFace=0; nFace<6; nFace++)
		m_pRoot[nFace] = new CPlanetaryMapNode(this, nFace);
	m_pRoot[RightFace]->SetNeighbors(m_pRoot[TopFace], m_pRoot[BackFace], m_pRoot[BottomFace], m_pRoot[FrontFace]);
	m_pRoot[LeftFace]->SetNeighbors(m_pRoot[TopFace], m_pRoot[FrontFace], m_pRoot[BottomFace], m_pRoot[BackFace]);
	m_pRoot[TopFace]->SetNeighbors(m_pRoot[BackFace], m_pRoot[RightFace], m_pRoot[FrontFace], m_pRoot[LeftFace]);
	m_pRoot[BottomFace]->SetNeighbors(m_pRoot[FrontFace], m_pRoot[RightFace], m_pRoot[BackFace], m_pRoot[LeftFace]);
	m_pRoot[FrontFace]->SetNeighbors(m_pRoot[TopFace], m_pRoot[RightFace], m_pRoot[BottomFace], m_pRoot[LeftFace]);
	m_pRoot[BackFace]->SetNeighbors(m_pRoot[TopFace], m_pRoot[LeftFace], m_pRoot[BottomFace], m_pRoot[RightFace]);
}

void CPlanetaryMap::Cleanup()
{
	int nFace;
	for(nFace=0; nFace<6; nFace++)
	{
		if(m_pRoot[nFace])
			delete m_pRoot[nFace];
		m_pRoot[nFace] = NULL;
	}
	m_llFactories.clear();
}

void CPlanetaryMap::UpdateSurface(CVector vLightPos, CVector vCamera, CQuaternion qCamera, float fSplitPower, float fSplitFactor)
{
	PROFILE("CPlanetaryMap::UpdateSurface", 2);
	CMatrix4 mView = BuildModelMatrix();
	glPushMatrix();
	glMultMatrixf(mView);
	m_frustum.Init();
	glPopMatrix();

	// Clear some counters for this update
	m_nSplits = 0;

	// Initialize the members used by the update
	m_vLightPos = m_qRotate.UnitInverse().TransformVector(vLightPos - m_vTranslate);
	m_vCamera = m_qRotate.UnitInverse().TransformVector(vCamera - m_vTranslate);
	m_qCamera = m_qRotate.UnitInverse() * qCamera;
	float fAltitude = m_vCamera.Magnitude();
	float fHorizonAltitude = CMath::Max(fAltitude-m_fRadius, m_fMaxHeight);
	m_fHorizon = sqrtf(fHorizonAltitude*fHorizonAltitude + 2.0f*fHorizonAltitude*m_fRadius);
	
	m_coordCamera.Init(m_vCamera, fAltitude);
	m_fSplitPower = fSplitPower;
	m_fSplitFactor = fSplitFactor;

	// Build an array of faces sorted by distance
	float fDistance[6];
	int nFace;
	for(nFace=0; nFace<6; nFace++)
		fDistance[nFace] = (m_pRoot[nFace]->HitTest(m_vCamera)) ? 0 : m_vCamera.DistanceSquared(m_pRoot[nFace]->GetVertex(40)->m_vPosition);
	m_nOrder[0] = 0; m_nOrder[1] = 1; m_nOrder[2] = 2; m_nOrder[3] = 3; m_nOrder[4] = 4; m_nOrder[5] = 5;
	for(int i=0; i<5; i++)
	{
		for(int j=0; j<5-i; j++)
		{
			if(fDistance[m_nOrder[j]] > fDistance[m_nOrder[j+1]])
				Swap(m_nOrder[j], m_nOrder[j+1]);
		}
	}

	// Now update the quad-tree (in order from nearest to farthest)
	for(nFace=0; nFace<6; nFace++)
		m_pRoot[m_nOrder[nFace]]->UpdateSurface();

	// Now update the factories (which may be handling surface objects)
	for(std::list<CPlanetaryFactory::Ref>::iterator it = m_llFactories.begin(); it != m_llFactories.end(); it++)
		(*it)->Update();
}

void CPlanetaryMap::DrawSurface()
{
	PROFILE("CPlanetaryMap::DrawSurface", 2);
	// Clear some counters for this render
	m_nObjects = 0;
	m_nTriangles = 0;
	m_nMaxDepth = 0;
	m_nNodes = 0;

	CMatrix4 mView = BuildModelMatrix();
	glPushMatrix();
	glMultMatrixf(mView);

	bool bUseTexture = m_pRoot[0]->HasTexture();
	if(bUseTexture)
	{
		m_texArray.Flush();
		glMatrixMode(GL_TEXTURE);
		m_texArray.Enable();
	}

	// Draw the planet
	CGLShaderObject *pShader;
	if(IsInAtmosphere())
		pShader = &m_shGroundFromAtmosphere;
	else if(HasAtmosphere())
		pShader = &m_shGroundFromSpace;
	else
		pShader = &m_shNoAtmosphere;
	if(pShader->IsValid())
	{
		pShader->Enable();
		InitScatteringShader(pShader);
	}
	else
	{
		glEnable(GL_LIGHTING);
		glEnable(GL_LIGHT0);
		glLightfv(GL_LIGHT0, GL_DIFFUSE, CQuaternion(1, 1, 1, 1));
		glLightfv(GL_LIGHT0, GL_POSITION, CQuaternion(m_vLightPos.x, m_vLightPos.y, m_vLightPos.z, 1));
	}

	// Then draw the objects in forward-Z order to minimize pixel overdraw
	for(int i=0; i<6; i++)
		m_nTriangles += m_pRoot[m_nOrder[i]]->DrawSurface();

	if(pShader->IsValid())
		pShader->Disable();
	else
		glDisable(GL_LIGHTING);

	// Draw the sky dome
	if(HasAtmosphere())
	{
		if(IsInAtmosphere())
			pShader = &m_shSkyFromAtmosphere;
		else
			pShader = &m_shSkyFromSpace;
		if(pShader->IsValid())
		{
			pShader->Enable();
			InitScatteringShader(pShader);
			glFrontFace(GL_CW);
			glEnable(GL_BLEND);
			glBlendFunc(GL_ONE, GL_ONE);
			GLUquadricObj *pSphere = gluNewQuadric();
			gluSphere(pSphere, m_fAtmosphereRadius, 100, 100);
			gluDeleteQuadric(pSphere);
			glDisable(GL_BLEND);
			glFrontFace(GL_CCW);
			pShader->Disable();
		}
	}

	if(bUseTexture)
	{
		glMatrixMode(GL_MODELVIEW);
		m_texArray.Disable();
	}

	// Now draw the factories (which may be handling surface objects)
	for(std::list<CPlanetaryFactory::Ref>::iterator it = m_llFactories.begin(); it != m_llFactories.end(); it++)
		(*it)->Draw();

	glPopMatrix();
}

float CPlanetaryMap::GetHeightAboveGround(const CVector &v)
{
	CVector vPos = m_qRotate.UnitInverse().TransformVector(v - m_vTranslate);
	float fAltitude = vPos.Magnitude();
	CPlanetaryMapCoord coord(vPos);
	for(int i=0; i<6; i++)
	{
		if(m_pRoot[i]->HitTest(coord))
			return fAltitude - m_pRoot[i]->GetHeight(coord);
	}
	return fAltitude - m_fRadius;
}
