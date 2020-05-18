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
#include "GrassFactory.h"


void CGrassFactory::BuildNode(CPlanetaryMapNode *pNode)
{
	PROFILE("CGrassFactory::BuildNode", 3);
	float fRadius = pNode->GetPlanetaryMap()->GetRadius();
	float fMaxHeight = pNode->GetPlanetaryMap()->GetMaxHeight();
	float fMin = fRadius + fMaxHeight * 0.1f;
	float fMax = fRadius + fMaxHeight * 0.6f;
	if(pNode->GetLevel() == 9)
	{
		for(int y=0; y<HEIGHT_MAP_WIDTH; y++)
		{
			int nCoord = (y+1)*BORDER_MAP_WIDTH + 1;
			for(int x=0; x<HEIGHT_MAP_WIDTH; x++)
			{
				if(builder.coord[nCoord].GetHeight() >= fMin && builder.coord[nCoord].GetHeight() <= fMax)
					m_mapGrass[pNode].push_back(builder.coord[nCoord]);
				nCoord++;
			}
		}
	}
}

void CGrassFactory::DestroyNode(CPlanetaryMapNode *pNode)
{
	m_mapGrass.erase(pNode);
}

void CGrassFactory::Update()
{
}

void CGrassFactory::Draw()
{
	PROFILE("CGrassFactory::Draw", 3);
	CVector vPos = CCameraTask::GetPtr()->GetCamera().GetPosition();
	CVector vView = CCameraTask::GetPtr()->GetCamera().GetViewAxis();
	CVector vUp = CCameraTask::GetPtr()->GetCamera().GetUpAxis();

	if(m_shBillboard.IsValid())
	{
		m_shBillboard.Enable();
		m_shBillboard.SetUniformParameter3f("v3CameraPos", vPos.x, vPos.y, vPos.z);
		m_shBillboard.SetUniformParameter3f("v3CameraView", vView.x, vView.y, vView.z);
		m_shBillboard.SetUniformParameter3f("v3CameraUp", vUp.x, vUp.y, vUp.z);
		m_shBillboard.SetUniformParameter1f("fScale", 0.001f);
		m_shBillboard.SetUniformParameter1i("s2Tex1", 0);

		m_tGrass.Enable();
		glEnable(GL_ALPHA_TEST);
		glAlphaFunc(GL_GREATER, 0.25f);
		//glEnable(GL_BLEND);
		//glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);

		glBegin(GL_QUADS);
		std::map<CPlanetaryMapNode *, std::list<CPlanetaryMapCoord> >::iterator it;
		for(it = m_mapGrass.begin(); it != m_mapGrass.end(); it++)
		{
			std::list<CPlanetaryMapCoord>::iterator itList;
			for(itList = it->second.begin(); itList != it->second.end(); itList++)
			{
				CVector vPos = itList->GetPositionVector(itList->GetHeight() + 0.0004f);
				glNormal3f(-0.5f, 0.5f, 0.0f);
				glVertex3f(vPos.x, vPos.y, vPos.z);
				glNormal3f(-0.5f, -0.5f, 0.0f);
				glVertex3f(vPos.x, vPos.y, vPos.z);
				glNormal3f(0.5f, -0.5f, 0.0f);
				glVertex3f(vPos.x, vPos.y, vPos.z);
				glNormal3f(0.5f, 0.5f, 0.0f);
				glVertex3f(vPos.x, vPos.y, vPos.z);
			}
		}
		glEnd();

		//glDisable(GL_BLEND);
		glDisable(GL_ALPHA_TEST);
		m_tGrass.Disable();
		m_shBillboard.Disable();
	}
}
