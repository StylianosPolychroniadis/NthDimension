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
#include "PlanetaryMapNode.h"

CNodeBuilder builder;


SBufferShape CPlanetaryMapNode::m_shape;

// Offsets to the start of each quadrant (useful for splitting, merging, and rendering)
unsigned short CPlanetaryMapNode::m_nQuadrantOffset[4] = {0, 4, 36, 40};
unsigned short CPlanetaryMapNode::m_nQuadrantHeightOffset[4] = {0, 8, 136, 144};

// A list of each face's neighbors
unsigned short CPlanetaryMapNode::m_nFaceNeighbor[6][4];


class CInitPlanetaryMapNode
{
public:
	CInitPlanetaryMapNode()
	{
		CPlanetaryMapNode::InitTables();
	}
};
CInitPlanetaryMapNode g_cInitPlanetaryMapNode;

void CPlanetaryMapNode::InitTables()
{
	int nTexCoord[4] = {2, 0, 0, 0};
	m_shape.Init(true, true, false, nTexCoord);

	// Calculate the offset to each quadrant in the vertex table
	m_nQuadrantOffset[TopLeft] = 0;
	m_nQuadrantOffset[TopRight] = (SURFACE_MAP_WIDTH-1) / 2;
	m_nQuadrantOffset[BottomRight] = (SURFACE_MAP_COUNT-1) / 2;
	m_nQuadrantOffset[BottomLeft] = m_nQuadrantOffset[BottomRight] - m_nQuadrantOffset[TopRight];

	// Calculate the offset to each quadrant in the height table
	m_nQuadrantHeightOffset[TopLeft] = 0;
	m_nQuadrantHeightOffset[TopRight] = (HEIGHT_MAP_WIDTH-1) / 2;
	m_nQuadrantHeightOffset[BottomRight] = (HEIGHT_MAP_COUNT-1) / 2;
	m_nQuadrantHeightOffset[BottomLeft] = m_nQuadrantHeightOffset[BottomRight] - m_nQuadrantHeightOffset[TopRight];

	// Set up the N/E/S/W neighbors of the front face of the cube
	m_nFaceNeighbor[FrontFace][TopEdge] = TopFace;
	m_nFaceNeighbor[FrontFace][RightEdge] = RightFace;
	m_nFaceNeighbor[FrontFace][BottomEdge] = BottomFace;
	m_nFaceNeighbor[FrontFace][LeftEdge] = LeftFace;

	// Set up the N/E/S/W neighbors of the back face of the cube
	m_nFaceNeighbor[BackFace][TopEdge] = TopFace;
	m_nFaceNeighbor[BackFace][RightEdge] = LeftFace;
	m_nFaceNeighbor[BackFace][BottomEdge] = BackFace;
	m_nFaceNeighbor[BackFace][LeftEdge] = RightFace;

	// Set up the N/E/S/W neighbors of the right face of the cube
	m_nFaceNeighbor[RightFace][TopEdge] = TopFace;
	m_nFaceNeighbor[RightFace][RightEdge] = BackFace;
	m_nFaceNeighbor[RightFace][BottomEdge] = BottomFace;
	m_nFaceNeighbor[RightFace][LeftEdge] = FrontFace;
	
	// Set up the N/E/S/W neighbors of the left face of the cube
	m_nFaceNeighbor[LeftFace][TopEdge] = TopFace;
	m_nFaceNeighbor[LeftFace][RightEdge] = FrontFace;
	m_nFaceNeighbor[LeftFace][BottomEdge] = BottomFace;
	m_nFaceNeighbor[LeftFace][LeftEdge] = BackFace;

	// Set up the N/E/S/W neighbors of the top face of the cube
	m_nFaceNeighbor[TopFace][TopEdge] = BackFace;
	m_nFaceNeighbor[TopFace][RightEdge] = RightFace;
	m_nFaceNeighbor[TopFace][BottomEdge] = FrontFace;
	m_nFaceNeighbor[TopFace][LeftEdge] = LeftFace;

	// Set up the N/E/S/W neighbors of the bottom face of the cube
	m_nFaceNeighbor[BottomFace][TopEdge] = FrontFace;
	m_nFaceNeighbor[BottomFace][RightEdge] = RightFace;
	m_nFaceNeighbor[BottomFace][BottomEdge] = BackFace;
	m_nFaceNeighbor[BottomFace][LeftEdge] = LeftFace;
}


CPlanetaryMapNode::CPlanetaryMapNode(CPlanetaryMap *pMap, int nFace)
{
	// Initialize flags and corners, then call the map builder to build this node
	m_pParent = NULL;
	m_pChild[0] = m_pChild[1] = m_pChild[2] = m_pChild[3] = NULL;
	m_pMap = pMap;
	InitNode(nFace, -1);
}

CPlanetaryMapNode::CPlanetaryMapNode(CPlanetaryMapNode *pParent, int nQuadrant)
{
	// Initialize flags, corners, and neighbor pointers, then call the map builder to build this node
	m_pParent = pParent;
	m_pChild[0] = m_pChild[1] = m_pChild[2] = m_pChild[3] = NULL;
	m_pMap = pParent->m_pMap;
	InitNode(pParent->GetFace(), nQuadrant);
}

CPlanetaryMapNode::~CPlanetaryMapNode()
{
	if(m_nBumpID != (unsigned short)-1)
		m_pMap->GetTextureArray().ReleaseTexture(m_nBumpID);
	if(HasTexture())
		m_pMap->GetTextureArray().ReleaseTexture(m_nTextureID);

	m_boVertex.Cleanup();

	// Clear the parent and neighbor pointers that point to this node
	if(m_pParent)
	{
		m_pParent->m_pChild[GetQuadrant()] = NULL;
		for(int i=0; i<4; i++)
		{
			if(m_pNeighbor[i])
			{
				for(int j=0; j<4; j++)
					if(m_pNeighbor[i]->m_pNeighbor[j] == this)
						m_pNeighbor[i]->m_pNeighbor[j] = NULL;
			}
		}
	}

	// Now pass this node to all factories and objects that might affect it
	for(CPlanetaryMap::iterator it = m_pMap->begin(); it != m_pMap->end(); it++)
	{
		if((*it)->AffectsNode(this))
			(*it)->DestroyNode(this);
	}
}

void CPlanetaryMapNode::InitNode(int nFace, int nQuadrant)
{
	m_nTextureID = (unsigned short)-1;
	m_nBumpID = (unsigned short)-1;

	// Set up node flags
	int nLevel = m_pParent ? m_pParent->GetLevel() + 1 : 0;
	m_nNodeFlags = NodeDirty | ((nLevel << 5) & LevelMask) | ((nQuadrant << 3) & QuadrantMask) | (nFace & FaceMask);
	m_nSort[0] = 0; m_nSort[1] = 1; m_nSort[2] = 2; m_nSort[3] = 3;

	if(m_pParent)
	{
		// Update neighbor pointers (and our neighbors' neighbor pointers)
		m_pParent->m_pChild[nQuadrant] = this;
		UpdateNeighbors();
		for(int i=0; i<4; i++)
		{
			if(m_pNeighbor[i])
				m_pNeighbor[i]->UpdateNeighbors();
		}
	}

	// Set up the corner boundaries for this node
	switch(nQuadrant)
	{
		case TopLeft:
			m_fCorner[0] = m_pParent->m_fCorner[0];
			m_fCorner[1] = m_pParent->m_fCorner[1];
			m_fCorner[2] = m_pParent->m_fCorner[0] + (m_pParent->m_fCorner[2] - m_pParent->m_fCorner[0]) * 0.5f;
			m_fCorner[3] = m_pParent->m_fCorner[1] + (m_pParent->m_fCorner[3] - m_pParent->m_fCorner[1]) * 0.5f;
			break;
		case TopRight:
			m_fCorner[0] = m_pParent->m_fCorner[0] + (m_pParent->m_fCorner[2] - m_pParent->m_fCorner[0]) * 0.5f;
			m_fCorner[1] = m_pParent->m_fCorner[1];
			m_fCorner[2] = m_pParent->m_fCorner[2];
			m_fCorner[3] = m_pParent->m_fCorner[1] + (m_pParent->m_fCorner[3] - m_pParent->m_fCorner[1]) * 0.5f;
			break;
		case BottomLeft:
			m_fCorner[0] = m_pParent->m_fCorner[0];
			m_fCorner[1] = m_pParent->m_fCorner[1] + (m_pParent->m_fCorner[3] - m_pParent->m_fCorner[1]) * 0.5f;
			m_fCorner[2] = m_pParent->m_fCorner[0] + (m_pParent->m_fCorner[2] - m_pParent->m_fCorner[0]) * 0.5f;
			m_fCorner[3] = m_pParent->m_fCorner[3];
			break;
		case BottomRight:
			m_fCorner[0] = m_pParent->m_fCorner[0] + (m_pParent->m_fCorner[2] - m_pParent->m_fCorner[0]) * 0.5f;
			m_fCorner[1] = m_pParent->m_fCorner[1] + (m_pParent->m_fCorner[3] - m_pParent->m_fCorner[1]) * 0.5f;
			m_fCorner[2] = m_pParent->m_fCorner[2];
			m_fCorner[3] = m_pParent->m_fCorner[3];
			break;
		default:	// Top-level node, no parent exists
			m_fCorner[0] = 0.0f;
			m_fCorner[1] = 0.0f;
			m_fCorner[2] = 1.0f;
			m_fCorner[3] = 1.0f;
			break;
	}

	// Initialize the node builder structure for this node
	int x, y, nIndex = 0;
	float fXOffset = (m_fCorner[2] - m_fCorner[0]) / (HEIGHT_MAP_WIDTH-1);
	float fYOffset = (m_fCorner[3] - m_fCorner[1]) / (HEIGHT_MAP_WIDTH-1);
	float fY = m_fCorner[1] - fYOffset;
	for(y=0; y<BORDER_MAP_WIDTH; y++)
	{
		float fX = m_fCorner[0] - fXOffset;
		for(x=0; x<BORDER_MAP_WIDTH; x++)
		{
			builder.coord[nIndex++].Init(nFace, fX, fY, m_pMap->GetRadius());
			fX += fXOffset;
		}
		fY += fYOffset;
	}

	// Now pass this node to all factories and objects that might affect it
	for(CPlanetaryMap::iterator it = m_pMap->begin(); it != m_pMap->end(); it++)
	{
		if((*it)->AffectsNode(this))
			(*it)->BuildNode(this);
	}

	if(builder.pb.GetBuffer() != NULL)
	{
		m_nTextureID = m_pMap->GetTextureArray().LockTexture();
		m_pMap->GetTextureArray().Update(m_nTextureID, &builder.pb);

	}

	builder.ComputeNormals();

	m_nBumpID = m_pMap->GetTextureArray().LockTexture();
	unsigned char *pBuffer = (unsigned char *)builder.pb.GetBuffer();
	nIndex = 0;
	for(y=0; y<HEIGHT_MAP_WIDTH; y++)
	{
		for(x=0; x<HEIGHT_MAP_WIDTH; x++)
		{
			CVector vNormal = builder.vNormal[nIndex];
			vNormal *= 128.0f / vNormal.Magnitude();
			*pBuffer++ = (unsigned char)(vNormal.x + 128.0f);
			*pBuffer++ = (unsigned char)(vNormal.y + 128.0f);
			*pBuffer++ = (unsigned char)(vNormal.z + 128.0f);
			nIndex++;
		}
	}
	m_pMap->GetTextureArray().Update(m_nBumpID, &builder.pb);


	// Initialize the vertex map
	m_pbHeightMap.Init(SURFACE_MAP_WIDTH, SURFACE_MAP_WIDTH, 1, 1, GL_ALPHA, GL_FLOAT);
	float *pfHeightMap = (float *)m_pbHeightMap.GetBuffer();
	nIndex = 0;
	for(y=0; y<SURFACE_MAP_WIDTH; y++)
	{
		int nCoord = (y*HEIGHT_MAP_SCALE+1) * BORDER_MAP_WIDTH + 1;
		int nNormal = y*HEIGHT_MAP_SCALE * HEIGHT_MAP_WIDTH;
		for(x=0; x<SURFACE_MAP_WIDTH; x++)
		{
			*pfHeightMap++ = builder.coord[nCoord].GetHeight();
			CPlanetaryVertex *pVertex = &m_vNode[nIndex++];
			pVertex->m_vPosition = builder.coord[nCoord].GetPositionVector();
			pVertex->m_vNormal = builder.vNormal[nNormal];
			pVertex->m_fTexCoord[0] = builder.coord[nCoord].x;
			pVertex->m_fTexCoord[1] = builder.coord[nCoord].y;
			nCoord += HEIGHT_MAP_SCALE;
			nNormal += HEIGHT_MAP_SCALE;
		}
	}

	m_boVertex.Init(&m_shape, SURFACE_MAP_COUNT, m_vNode);
}

CPlanetaryMapNode *CPlanetaryMapNode::FindQuadrantNeighbor(int nQuadrant, int nEdge, bool bForceSplit)
{
	// When neighbors cross from one face of the cube to another, a mapping is needed to find the correct quadrant
	static const unsigned char nTopLeftTopEdge[6] = { BottomRight, TopLeft, TopRight, BottomLeft, BottomLeft, TopRight };
	static const unsigned char nTopLeftLeftEdge[6] = { TopRight, TopRight, TopLeft, BottomRight, TopRight, TopRight };
	static const unsigned char nTopRightTopEdge[6] = { TopRight, BottomLeft, TopLeft, BottomRight, BottomRight, TopLeft };
	static const unsigned char nTopRightRightEdge[6] = { TopLeft, TopLeft, TopRight, BottomLeft, TopLeft, TopLeft };
	static const unsigned char nBottomLeftBottomEdge[6] = { TopRight, BottomLeft, TopLeft, BottomRight, TopLeft, BottomRight };
	static const unsigned char nBottomLeftLeftEdge[6] = { BottomRight, BottomRight, TopRight, BottomLeft, BottomRight, BottomRight };
	static const unsigned char nBottomRightBottomEdge[6] = { BottomRight, TopLeft, TopRight, BottomLeft, TopRight, BottomLeft };
	static const unsigned char nBottomRightRightEdge[6] = { BottomLeft, BottomLeft, TopLeft, BottomRight, BottomLeft, BottomLeft };

	CPlanetaryMapNode *pNeighbor = NULL;
	int nChild;
	switch(nQuadrant)
	{
		case TopLeft:
			switch(nEdge)
			{
				case TopEdge:
					if(m_pNeighbor[nEdge])
					{
						nChild = (m_pNeighbor[nEdge]->GetFace() == GetFace()) ? BottomLeft : nTopLeftTopEdge[GetFace()];
						if(bForceSplit)
							m_pNeighbor[nEdge]->Split(nChild);
						pNeighbor = m_pNeighbor[nEdge]->m_pChild[nChild];
					}
					break;
				case RightEdge:
					pNeighbor = m_pChild[TopRight];
					break;
				case BottomEdge:
					pNeighbor = m_pChild[BottomLeft];
					break;
				case LeftEdge:
					if(m_pNeighbor[nEdge])
					{
						nChild = (m_pNeighbor[nEdge]->GetFace() == GetFace()) ? TopRight : nTopLeftLeftEdge[GetFace()];
						if(bForceSplit)
							m_pNeighbor[nEdge]->Split(nChild);
						pNeighbor = m_pNeighbor[nEdge]->m_pChild[nChild];
					}
					break;
			}
			break;
		case TopRight:
			switch(nEdge)
			{
				case TopEdge:
					if(m_pNeighbor[nEdge])
					{
						nChild = (m_pNeighbor[nEdge]->GetFace() == GetFace()) ? BottomRight : nTopRightTopEdge[GetFace()];
						if(bForceSplit)
							m_pNeighbor[nEdge]->Split(nChild);
						pNeighbor = m_pNeighbor[nEdge]->m_pChild[nChild];
					}
					break;
				case RightEdge:
					if(m_pNeighbor[nEdge])
					{
						nChild = (m_pNeighbor[nEdge]->GetFace() == GetFace()) ? TopLeft : nTopRightRightEdge[GetFace()];
						if(bForceSplit)
							m_pNeighbor[nEdge]->Split(nChild);
						pNeighbor = m_pNeighbor[nEdge]->m_pChild[nChild];
					}
					break;
				case BottomEdge:
					pNeighbor = m_pChild[BottomRight];
					break;
				case LeftEdge:
					pNeighbor = m_pChild[TopLeft];
					break;
			}
			break;
		case BottomLeft:
			switch(nEdge)
			{
				case TopEdge:
					pNeighbor = m_pChild[TopLeft];
					break;
				case RightEdge:
					pNeighbor = m_pChild[BottomRight];
					break;
				case BottomEdge:
					if(m_pNeighbor[nEdge])
					{
						nChild = (m_pNeighbor[nEdge]->GetFace() == GetFace()) ? TopLeft : nBottomLeftBottomEdge[GetFace()];
						if(bForceSplit)
							m_pNeighbor[nEdge]->Split(nChild);
						pNeighbor = m_pNeighbor[nEdge]->m_pChild[nChild];
					}
					break;
				case LeftEdge:
					if(m_pNeighbor[nEdge])
					{
						nChild = (m_pNeighbor[nEdge]->GetFace() == GetFace()) ? BottomRight : nBottomLeftLeftEdge[GetFace()];
						if(bForceSplit)
							m_pNeighbor[nEdge]->Split(nChild);
						pNeighbor = m_pNeighbor[nEdge]->m_pChild[nChild];
					}
					break;
			}
			break;
		case BottomRight:
			switch(nEdge)
			{
				case TopEdge:
					pNeighbor = m_pChild[TopRight];
					break;
				case RightEdge:
					if(m_pNeighbor[nEdge])
					{
						nChild = (m_pNeighbor[nEdge]->GetFace() == GetFace()) ? BottomLeft : nBottomRightRightEdge[GetFace()];
						if(bForceSplit)
							m_pNeighbor[nEdge]->Split(nChild);
						pNeighbor = m_pNeighbor[nEdge]->m_pChild[nChild];
					}
					break;
				case BottomEdge:
					if(m_pNeighbor[nEdge])
					{
						nChild = (m_pNeighbor[nEdge]->GetFace() == GetFace()) ? TopRight : nBottomRightBottomEdge[GetFace()];
						if(bForceSplit)
							m_pNeighbor[nEdge]->Split(nChild);
						pNeighbor = m_pNeighbor[nEdge]->m_pChild[nChild];
					}
					break;
				case LeftEdge:
					pNeighbor = m_pChild[BottomLeft];
					break;
			}
			break;
	}
	return pNeighbor;
}

void CPlanetaryMapNode::Split(int nQuadrant)
{
	// If the requested quadrant is already split, return
	if(m_pChild[nQuadrant])
		return;

	// Make sure the affected edges have neighbors by calling the force-splitting version of FindNeighbor()
	switch(nQuadrant)
	{
		case TopLeft:
			if(!FindNeighbor(TopEdge, true) || !FindNeighbor(LeftEdge, true))
				return;
			break;
		case TopRight:
			if(!FindNeighbor(TopEdge, true) || !FindNeighbor(RightEdge, true))
				return;
			break;
		case BottomLeft:
			if(!FindNeighbor(BottomEdge, true) || !FindNeighbor(LeftEdge, true))
				return;
			break;
		case BottomRight:
			if(!FindNeighbor(BottomEdge, true) || !FindNeighbor(RightEdge, true))
				return;
			break;
	}

	// Finally, check the map to see if we've already split too many times during this frame
	if(m_pMap->CanSplit())
		m_pChild[nQuadrant] = new CPlanetaryMapNode(this, nQuadrant);
}

bool CPlanetaryMapNode::CanMerge()
{
	// If this node has children of its own, it can't be merged
	if(!IsLeaf())
		return false;

	// If this node has any neighbors with children bordering this node, it can't be merged
	if(m_pNeighbor[TopEdge] && (FindQuadrantNeighbor(TopLeft, TopEdge) || FindQuadrantNeighbor(TopRight, TopEdge)))
		return false;
	if(m_pNeighbor[RightEdge] && (FindQuadrantNeighbor(TopRight, RightEdge) || FindQuadrantNeighbor(BottomRight, RightEdge)))
		return false;
	if(m_pNeighbor[BottomEdge] && (FindQuadrantNeighbor(BottomRight, BottomEdge) || FindQuadrantNeighbor(BottomLeft, BottomEdge)))
		return false;
	if(m_pNeighbor[LeftEdge] && (FindQuadrantNeighbor(BottomLeft, LeftEdge) || FindQuadrantNeighbor(TopLeft, LeftEdge)))
		return false;

	// If none of the above restrictions apply, this node can be merged
	return true;
}

bool CPlanetaryMapNode::Merge(int nQuadrant)
{
	// If the requested quadrant does not exist, return success (nothing to do)
	CPlanetaryMapNode *pNode = m_pChild[nQuadrant];
	if(!pNode)
		return true;

	// If any of its children fail to merge, or if it fails the merge test, return failure
	bool bSuccess = true;
	if(!pNode->Merge(0))
		bSuccess = false;
	if(!pNode->Merge(1))
		bSuccess = false;
	if(!pNode->Merge(2))
		bSuccess = false;
	if(!pNode->Merge(3))
		bSuccess = false;
	if(!bSuccess || !pNode->CanMerge())
		return false;

	// Remove the quadrant
	delete m_pChild[nQuadrant];
	return true;
}

void CPlanetaryMapNode::UpdateSurface()
{
	// Get the camera position and clear the node flags set by this function
	CVector vCamera = m_pMap->GetCameraPosition();
	m_nNodeFlags &= ~(CameraInMask | BeyondHorizonMask | OutsideFrustumMask);

	// Calculate the map coordinates for the center of each quadrant
	short nFace = GetFace();
	float fMid[2] = {CMath::Avg(m_fCorner[0], m_fCorner[2]), CMath::Avg(m_fCorner[1], m_fCorner[3])};
	float x[2] = {CMath::Avg(m_fCorner[0], fMid[0]), CMath::Avg(m_fCorner[2], fMid[0])};
	float y[2] = {CMath::Avg(m_fCorner[1], fMid[1]), CMath::Avg(m_fCorner[3], fMid[1])};
	CPlanetaryMapCoord cQuadrant[4] =
	{
		CPlanetaryMapCoord(nFace, x[0], y[0]),
		CPlanetaryMapCoord(nFace, x[1], y[0]),
		CPlanetaryMapCoord(nFace, x[0], y[1]),
		CPlanetaryMapCoord(nFace, x[1], y[1])
	};

	// Calculate the 3D position for the center of each quadrant
	CVector vQuadrant[4] =
	{
		cQuadrant[0].GetPositionVector(m_pMap->GetFrustumRadius()),
		cQuadrant[1].GetPositionVector(m_pMap->GetFrustumRadius()),
		cQuadrant[2].GetPositionVector(m_pMap->GetFrustumRadius()),
		cQuadrant[3].GetPositionVector(m_pMap->GetFrustumRadius()),
	};

	// Calculate the distance to the camera for the center of each quadrant
	float fDistance[4] =
	{
		vCamera.Distance(vQuadrant[0]),
		vCamera.Distance(vQuadrant[1]),
		vCamera.Distance(vQuadrant[2]),
		vCamera.Distance(vQuadrant[3]),
	};

	// Now sort the quadrants by distance (used for updating and rendering order)
	// Don't use the actual distances because there will be precision problems
	float xCoord, yCoord;
	GetFaceCoordinates(nFace, vCamera, xCoord, yCoord);
	if(xCoord < fMid[0])
	{
		if(yCoord < fMid[1])
		{
			m_nSort[0] = 0;	// Closest
			m_nSort[1] = 1;	// Doesn't matter
			m_nSort[2] = 2;	// Doesn't matter
			m_nSort[3] = 3;	// Farthest
		}
		else
		{
			m_nSort[0] = 2;	// Closest
			m_nSort[1] = 0;	// Doesn't matter
			m_nSort[2] = 3;	// Doesn't matter
			m_nSort[3] = 1;	// Farthest
		}
	}
	else
	{
		if(yCoord < fMid[1])
		{
			m_nSort[0] = 1;	// Closest
			m_nSort[1] = 0;	// Doesn't matter
			m_nSort[2] = 3;	// Doesn't matter
			m_nSort[3] = 2;	// Farthest
		}
		else
		{
			m_nSort[0] = 3;	// Closest
			m_nSort[1] = 1;	// Doesn't matter
			m_nSort[2] = 2;	// Doesn't matter
			m_nSort[3] = 0;	// Farthest
		}
	}

	// Calculate the quadrant size (the length of the diagonal), and use that to calculate the radius for view frustum culling
	float fQuadrantSize = vQuadrant[TopLeft].Distance(vQuadrant[BottomRight] * (m_pMap->GetRadius()/m_pMap->GetFrustumRadius()));
	float fRadius = CMath::Max(fQuadrantSize, m_pMap->GetMaxHeight());

	// For split priority, we can't use the true quadrant size because quadrants near the corners of cube faces are smaller than quadrants mear the center
	// This caused an odd-looking artifact at the corners where nodes nearest the camera were at a lower detail level than nodes farther away
	fQuadrantSize = (m_fCorner[2]-m_fCorner[0]) * m_pMap->GetRadius();
	float fPriority = (fQuadrantSize <= m_pMap->GetMaxHeight()*0.001f) ? 0.0f : m_pMap->GetSplitFactor() * powf(fQuadrantSize, m_pMap->GetSplitPower());

	// Update each quadrant in forward Z order (to split nodes closer to the camera first)
	for(int i=0; i<4; i++)
	{
		int nQuadrant = m_nSort[i];

		// Check to see if the camera's in this quadrant
		if(HitTest(m_pMap->GetCameraCoord(), nQuadrant))
			m_nNodeFlags |= (nQuadrant << 10) & CameraInMask;
		else
		{
			// Check the quadrant against the horizon distance
			if(m_pMap->GetHorizonDistance() > 0.0f && fDistance[nQuadrant]-fRadius > m_pMap->GetHorizonDistance())
			{
				m_nNodeFlags |= 1 << (16+nQuadrant);
				Merge(nQuadrant);
				continue;
			}

			// Check the quadrant against the view frustum
			if(!m_pMap->GetViewFrustum().IsInFrustum(vQuadrant[nQuadrant], fRadius))
			{
				m_nNodeFlags |= 1 << (20+nQuadrant);
				Merge(nQuadrant);
				continue;
			}
		}

		// If the quadrant wasn't culled, check its distance against the priority to see if it should be merged
		if(fDistance[nQuadrant] > fPriority)
		{
			Merge(nQuadrant);
			continue;
		}

		// If we're not trying to merge it, we should split it (if it doesn't already exist) and update its children
		Split(nQuadrant);
		if(m_pChild[nQuadrant])
			m_pChild[nQuadrant]->UpdateSurface();
	}
}

int CPlanetaryMapNode::DrawSurface()
{
	m_pMap->UpdateMaxDepth(GetLevel());

	static const unsigned short nFan[10] = {20, 22, 12, 2, 10, 18, 28, 38, 30, 22};
	static const unsigned short nOther[4][6] = {{0, 10, 2, 2, 12, 4}, {4, 12, 22, 22, 30, 40}, {40, 30, 38, 38, 28, 36}, {36, 28, 18, 18, 10, 0}};
	static const unsigned short nEdge[4][2] = {{1, 3}, {13, 31}, {39, 37}, {27, 9}};
	static unsigned short nMesh[48];
	static unsigned short nMeshIndex;
	int i, n, nQuadrant;
	int nTriangles = 0;

	// If we need it, set up the texture state for this node
	bool bUseTexture = HasTexture() && (!m_pChild[0] || !m_pChild[1] || !m_pChild[2] || !m_pChild[3]);
	if(bUseTexture)
	{
		CMatrix4 mTexture;
		glActiveTexture(GL_TEXTURE1);
		m_pMap->GetTextureArray().MapCorners(m_nBumpID, mTexture, m_fCorner[0], m_fCorner[1], m_fCorner[2], m_fCorner[3]);
		glPushMatrix();
		glLoadMatrixf(mTexture);

		glActiveTexture(GL_TEXTURE0);
		m_pMap->GetTextureArray().MapCorners(m_nTextureID, mTexture, m_fCorner[0], m_fCorner[1], m_fCorner[2], m_fCorner[3]);
		glPushMatrix();
		glLoadMatrixf(mTexture);
	}

	// Then draw the quadrants in forward-Z order (to avoid pixel overdraw)
	for(i=0; i<4; i++)
	{
		nQuadrant = m_nSort[i];
		if(IsBeyondHorizon(nQuadrant) || IsOutsideFrustum(nQuadrant))
			continue;

		if(m_pChild[nQuadrant])
			nTriangles += m_pChild[nQuadrant]->DrawSurface();
		else
		{
			m_pMap->IncrementNodeCount();

			m_boVertex.Enable(&m_shape);

			// Regardless of whether any of the neighbors are split, each quadrant has a fan of 8 triangles in its center
			for(n=0; n<10; n++)
				nMesh[n] = nFan[n] + m_nQuadrantOffset[nQuadrant];
			glDrawElements(GL_TRIANGLE_FAN, 10, GL_UNSIGNED_SHORT, nMesh);
			nTriangles += 8;

			// Each quadrant has 2 triangles on each edge, which are split into 4 if the quadrant's neighbor is split.
			nMeshIndex = 0;
			for(n=0; n<4; n++)
			{
				bool bNeighborSplit = FindQuadrantNeighbor(nQuadrant, n) != NULL;

				nMesh[nMeshIndex++] = nOther[n][0] + m_nQuadrantOffset[nQuadrant];
				nMesh[nMeshIndex++] = nOther[n][1] + m_nQuadrantOffset[nQuadrant];
				if(bNeighborSplit)
				{
					nMesh[nMeshIndex++] = nEdge[n][0] + m_nQuadrantOffset[nQuadrant];
					nMesh[nMeshIndex++] = nEdge[n][0] + m_nQuadrantOffset[nQuadrant];
					nMesh[nMeshIndex++] = nOther[n][1] + m_nQuadrantOffset[nQuadrant];
				}
				nMesh[nMeshIndex++] = nOther[n][2] + m_nQuadrantOffset[nQuadrant];

				nMesh[nMeshIndex++] = nOther[n][3] + m_nQuadrantOffset[nQuadrant];
				nMesh[nMeshIndex++] = nOther[n][4] + m_nQuadrantOffset[nQuadrant];
				if(bNeighborSplit)
				{
					nMesh[nMeshIndex++] = nEdge[n][1] + m_nQuadrantOffset[nQuadrant];
					nMesh[nMeshIndex++] = nEdge[n][1] + m_nQuadrantOffset[nQuadrant];
					nMesh[nMeshIndex++] = nOther[n][4] + m_nQuadrantOffset[nQuadrant];
				}
				nMesh[nMeshIndex++] = nOther[n][5] + m_nQuadrantOffset[nQuadrant];
			}

			// Draw the mesh for this quadrant
			glDrawElements(GL_TRIANGLES, nMeshIndex, GL_UNSIGNED_SHORT, nMesh);
			nTriangles += nMeshIndex/3;
		}
	}

	// Clean up the texture states for this node
	if(bUseTexture)
	{
		glClientActiveTexture(GL_TEXTURE1);
		glActiveTexture(GL_TEXTURE1);
		glPopMatrix();
		glClientActiveTexture(GL_TEXTURE0);
		glActiveTexture(GL_TEXTURE0);
		glPopMatrix();
	}

	return nTriangles;
}
