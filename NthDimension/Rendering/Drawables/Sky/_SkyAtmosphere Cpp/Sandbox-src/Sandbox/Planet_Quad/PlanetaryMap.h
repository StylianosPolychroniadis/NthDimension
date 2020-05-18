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

#ifndef __PlanetaryMap_h__
#define __PlanetaryMap_h__

#include "PlanetaryObject.h"
#include "PlanetaryMapCoord.h"


/******************************************************************************
* Class: CPlanetaryMap
*******************************************************************************
* This class encapsulates an entire planetary map, which consists of six
* quad-trees. The top level of each quad-tree represents a face on a cube.
******************************************************************************/
class CPlanetaryMap : public CSRTTransform
{
protected:
	std::list<CPlanetaryFactory::Ref> m_llFactories;	// A list of factories that have global effect
	CGLShaderObject m_shNoAtmosphere;
	CGLShaderObject m_shGroundFromSpace;
	CGLShaderObject m_shGroundFromAtmosphere;
	CGLShaderObject m_shSkyFromSpace;
	CGLShaderObject m_shSkyFromAtmosphere;

	float m_fRadius;					// The radius of this planetary map
	float m_fFrustumRadius;				// The radius used for frustum culling checks (also the height at which priority checks are made)
	float m_fMaxHeight;					// The max height offset from that radius
	float m_fAtmosphereRadius;
	float m_fWavelength[3];
	float m_fEsun;
	float m_fKr;
	float m_fKm;
	float m_fG;
	
	CPlanetaryMapNode *m_pRoot[6];		// Pointers to the 6 top-level quad-tree nodes (one for each cube face)
	unsigned char m_nOrder[6];			// Temporary space used to order the top-level quad-tree nodes by distance
	CTextureArray m_texArray;

	// Members to control and throttle the split/merge operations
	float m_fSplitPower;				// Adjust this value to affect split priorities (exponential effect)
	float m_fSplitFactor;				// Adjust this value to affect split priorities (linear effect)
	int m_nSplits;						// The number of splits so far this frame
	int m_nMaxSplits;					// The maximum number of splits allowed per frame

	// Temporary members used by the Update() and Draw() methods
	CPlanetaryMapCoord m_coordCamera;	// A planetary map coord indicating where the camera is over the planet
	CVector m_vLightPos;
	CVector m_vCamera;					// The position vector of the camera relative to the planet
	CQuaternion m_qCamera;				// The orientation of the camera relative to the planet
	float m_fHorizon;					// The current distance from the camera to the horizon
	CFrustum m_frustum;					// The current view frustum (for view frustum culling)

	// Statistical members to indicate the current state of the quad-tree
	int m_nMaxDepth;					// The current maximum depth of the tree
	int m_nNodes;						// The number of visible nodes in the tree
	int m_nTriangles;					// The number of triangles rendered so far this frame
	int m_nObjects;						// The number of independent objects rendered so far this frame

public:
	CPlanetaryMap()
	{
		m_nMaxSplits = 50;
		for(int nFace=0; nFace<6; nFace++)
			m_pRoot[nFace] = NULL;
	}
	CPlanetaryMap(const CPlanetaryMap &)
	{
		m_nMaxSplits = 50;
		for(int nFace=0; nFace<6; nFace++)
			m_pRoot[nFace] = NULL;
	}
	~CPlanetaryMap()
	{
		Cleanup();
	}

	void AddObject(CPlanetaryObject *pObj, bool bGlobal=true);
	void Init(CPropertySet &prop);
	void Cleanup();
	void UpdateSurface(CVector vLightPos, CVector vCamera, CQuaternion qCamera, float fSplitPower, float fSplitFactor);
	void DrawSurface();

	float GetRadius()				{ return m_fRadius; }
	float GetFrustumRadius()		{ return m_fFrustumRadius; }
	float GetMaxHeight()			{ return m_fMaxHeight; }
	float GetSplitPower()			{ return m_fSplitPower; }
	float GetSplitFactor()			{ return m_fSplitFactor; }
	int GetMaxSplits()				{ return m_nMaxSplits; }

	CTextureArray &GetTextureArray()			{ return m_texArray; }
	const CPlanetaryMapCoord &GetCameraCoord()	{ return m_coordCamera; }
	const CVector &GetCameraPosition()			{ return m_vCamera; }
	const CQuaternion &GetCameraOrientation()	{ return m_qCamera; }
	float GetHorizonDistance()					{ return m_fHorizon; }
	const CFrustum &GetViewFrustum()			{ return m_frustum; }

	int GetTriangleCount()	{ return m_nTriangles; }
	int GetObjectCount()	{ return m_nObjects; }
	int GetNodeCount()		{ return m_nNodes; }
	int GetMaxDepth()		{ return m_nMaxDepth; }
	int GetSplitCount()		{ return m_nSplits; }

	void UpdateMaxDepth(int n)	{ m_nMaxDepth = CMath::Max(m_nMaxDepth, n); }
	void IncrementNodeCount()	{ m_nNodes++; }
	bool CanSplit()
	{
		if(m_nSplits >= m_nMaxSplits)
			return false;
		m_nSplits++;
		return true;
	}

	bool HasAtmosphere()					{ return m_fAtmosphereRadius > 0.0f; }
	bool IsInAtmosphere()					{ return m_coordCamera.GetHeight() < m_fAtmosphereRadius; }
	float GetHeightAboveGround(const CVector &v);

	typedef std::list<CPlanetaryFactory::Ref>::iterator iterator;
	iterator begin()	{ return m_llFactories.begin(); }
	iterator end()		{ return m_llFactories.end(); }

	void InitScatteringShader(CGLShaderObject *pShader)
	{
		CVector vLightDirection = m_vLightPos;
		vLightDirection.Normalize();

		float fRayleighScaleDepth = 0.25f;
		pShader->SetUniformParameter3f("v3CameraPos", m_vCamera.x, m_vCamera.y, m_vCamera.z);
		pShader->SetUniformParameter3f("v3LightPos", vLightDirection.x, vLightDirection.y, vLightDirection.z);
		pShader->SetUniformParameter3f("v3InvWavelength", 1/powf(m_fWavelength[0], 4.0f), 1/powf(m_fWavelength[1], 4.0f), 1/powf(m_fWavelength[2], 4.0f));
		pShader->SetUniformParameter1f("fCameraHeight", m_vCamera.Magnitude());
		pShader->SetUniformParameter1f("fCameraHeight2", m_vCamera.MagnitudeSquared());
		pShader->SetUniformParameter1f("fInnerRadius", m_fRadius);
		pShader->SetUniformParameter1f("fInnerRadius2", m_fRadius*m_fRadius);
		pShader->SetUniformParameter1f("fOuterRadius", m_fAtmosphereRadius);
		pShader->SetUniformParameter1f("fOuterRadius2", m_fAtmosphereRadius*m_fAtmosphereRadius);
		pShader->SetUniformParameter1f("fKrESun", m_fKr*m_fEsun);
		pShader->SetUniformParameter1f("fKmESun", m_fKm*m_fEsun);
		pShader->SetUniformParameter1f("fKr4PI", m_fKr*4.0f*PI);
		pShader->SetUniformParameter1f("fKm4PI", m_fKm*4.0f*PI);
		pShader->SetUniformParameter1f("fScale", 1.0f / (m_fAtmosphereRadius - m_fRadius));
		pShader->SetUniformParameter1f("fScaleDepth", fRayleighScaleDepth);
		pShader->SetUniformParameter1f("fScaleOverScaleDepth", (1.0f / (m_fAtmosphereRadius - m_fRadius)) / fRayleighScaleDepth);
		pShader->SetUniformParameter1f("g", m_fG);
		pShader->SetUniformParameter1f("g2", m_fG*m_fG);
		pShader->SetUniformParameter1i("nSamples", 2);
		pShader->SetUniformParameter1f("fSamples", 2);
		pShader->SetUniformParameter1i("s2Tex", 0);
	}
};

#endif
