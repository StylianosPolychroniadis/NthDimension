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
#include "PlanetaryObject.h"
#include "PlanetaryMapNode.h"


void CSimpleHeightMapFactory::BuildNode(CPlanetaryMapNode *pNode)
{
	PROFILE("CSimpleHeightMapFactory::BuildNode", 3);
	for(int nIndex=0; nIndex<BORDER_MAP_COUNT; nIndex++)
	{
		CVector vNormalized = builder.coord[nIndex].GetDirectionVector();
		float fHeight = builder.coord[nIndex].GetHeight();
		if(m_nFractalSeed)
		{
			CVector v = vNormalized;
			fHeight += fBmTest3(v, m_fOctaves, 1.0f, -0.1f) * pNode->GetPlanetaryMap()->GetMaxHeight();
		}
		builder.coord[nIndex].SetHeight(fHeight);
	}
}

void CSimpleCraterFactory::BuildNode(CPlanetaryMapNode *pNode)
{
	PROFILE("CSimpleCraterFactory::BuildNode", 3);

	if(m_fCraterScale[0] == 0.0f)
	{
		float fScale = pNode->GetPlanetaryMap()->GetRadius() * 0.5f;
		for(int i=0; i<50; i++)
		{
			m_fCraterScale[i] = fScale / (2.0f * powf(fScale, 0.25f));
			fScale *= 0.5f;
		}
	}

	for(int nIndex=0; nIndex<BORDER_MAP_COUNT; nIndex++)
	{
		float fHeight = builder.coord[nIndex].GetHeight();
		float x = builder.coord[nIndex].x;
		float y = builder.coord[nIndex].y;

		// Skip over the top levels of the crater tree, generating the appropriate seeds on the way
		int i, nSeed = m_nSeed + (pNode->GetFace()<<4);
		for(i=0; i<m_nTopLevel; i++)
			nSeed = CCraterMap::GetChildSeed(nSeed, x, y);

		// Now add all the applicable crater offsets down to the bottom of the tree
		CCraterMap node;
		while(i <= m_nBottomLevel)
		{
			node.Init(nSeed, m_nCratersPerLevel);
			fHeight += node.GetOffset(x, y, m_fCraterScale[i]);
			nSeed = CCraterMap::GetChildSeed(nSeed, x, y);
			i++;
		}

		builder.coord[nIndex].SetHeight(fHeight);
	}
}

void CSimpleColorMapFactory::BuildNode(CPlanetaryMapNode *pNode)
{
	PROFILE("CSimpleColorMapFactory::BuildNode", 3);
	// Initialize the color map
	builder.pb.Init(HEIGHT_MAP_WIDTH, HEIGHT_MAP_WIDTH, 1);
	unsigned char *pBuffer = (unsigned char *)builder.pb.GetBuffer();
	for(int y=0; y<HEIGHT_MAP_WIDTH; y++)
	{
		int nCoord = (y+1)*BORDER_MAP_WIDTH + 1;
		for(int x=0; x<HEIGHT_MAP_WIDTH; x++)
		{
			float fAltitude = builder.coord[nCoord++].GetHeight() - pNode->GetPlanetaryMap()->GetRadius();
			float fHeight = fAltitude / pNode->GetPlanetaryMap()->GetMaxHeight();
			fHeight = (m_vColors.size()-1) * CMath::Clamp(0.001f, 0.999f, (fHeight+1.0f) * 0.5f);
			int nHeight = (int)fHeight;
			float fRatio = fHeight - nHeight;
			CColor c = m_vColors[nHeight] * (1-fRatio) + m_vColors[nHeight+1] * fRatio;

			*pBuffer++ = c.r;
			*pBuffer++ = c.g;
			*pBuffer++ = c.b;
		}
	}
}
