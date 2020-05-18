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

#ifndef __PlanetaryMapNode_h__
#define __PlanetaryMapNode_h__

#include "PlanetaryMap.h"

// The surface map will have 16 squares per node (64 triangles, and (8+1)x(8+1) set of vertices)
#define NODE_WIDTH			8
#define HEIGHT_MAP_SCALE	2

#define SURFACE_MAP_WIDTH	(NODE_WIDTH+1)
#define SURFACE_MAP_COUNT	(SURFACE_MAP_WIDTH*SURFACE_MAP_WIDTH)

// The height map resolution will be twice the resolution of the surface map (16+1)x(16+1)
#define HEIGHT_MAP_WIDTH	(NODE_WIDTH*HEIGHT_MAP_SCALE+1)
#define HEIGHT_MAP_COUNT	(HEIGHT_MAP_WIDTH*HEIGHT_MAP_WIDTH)

// The height map resolution plus a border surrounding it for normal calculations
#define BORDER_MAP_WIDTH	(HEIGHT_MAP_WIDTH+2)
#define BORDER_MAP_COUNT	(BORDER_MAP_WIDTH*BORDER_MAP_WIDTH)


enum
{	// Enumerated values indicating the order of the 4 quadrants
	TopLeft=0,
	TopRight=1,
	BottomLeft=2,
	BottomRight=3
};

enum
{	// Enumerated values indicating the order of the neighbors
	TopEdge=0,
	RightEdge=1,
	BottomEdge=2,
	LeftEdge=3
};

enum
{	// Enumerated bit masks for the m_nNodeFlags member below
	FaceMask=0x0007,			// These 3 bits indicate which top-level face the node is in
	QuadrantMask=0x0018,		// These 2 bits indicate which of the parent's quadrants was split to create this node
	LevelMask=0x03E0,			// These 5 bits indicate the level of the tree the node is in (0-31)
	CameraInMask=0x0C00,		// These 2 bits indicate which quadrant the camera is in (if it's in this node)
	NodeDirty=0x1000,			// This bit indicates whether the node is dirty (i.e. needs geo-morphing)

	BeyondHorizonMask=0x000F0000,	// These bits indicate whether each quadrant is beyond the horizon distance
	OutsideFrustumMask=0x00F00000,	// These bits indicate whether each quadrant is outside the view frustum
};

class CNodeBuilder
{
public:
	CPixelBuffer pb;
	CPlanetaryMapCoord coord[BORDER_MAP_COUNT];
	CVector vNormal[HEIGHT_MAP_COUNT];

	void ComputeNormals()
	{
		int nIndex = 0;
		for(int y=0; y<HEIGHT_MAP_WIDTH; y++)
		{
			int nCoord = (y+1)*BORDER_MAP_WIDTH + 1;
			for(int x=0; x<HEIGHT_MAP_WIDTH; x++)
			{
				CVector vCenter = coord[nCoord].GetPositionVector();
				CVector vNorth = coord[nCoord-BORDER_MAP_WIDTH].GetPositionVector() - vCenter;
				CVector vWest = coord[nCoord-1].GetPositionVector() - vCenter;
				CVector vSouth = coord[nCoord+BORDER_MAP_WIDTH].GetPositionVector() - vCenter;
				CVector vEast = coord[nCoord+1].GetPositionVector() - vCenter;
				vNormal[nIndex] = (vNorth ^ vWest) + (vSouth ^ vEast);
				vNormal[nIndex].Normalize();
				nCoord++;
				nIndex++;
			}
		}
	}
};

extern CNodeBuilder builder;


class CPlanetaryVertex
{
// Attributes
public:
	CVector m_vPosition;		// The position of the vertex
	CVector m_vNormal;			// The normal of the vertex (used for smooth shading)
	float m_fTexCoord[2];		// The texture coordinates of the vertex

// Operations
public:
	CPlanetaryVertex() {}
	CPlanetaryVertex(const CPlanetaryVertex &v) { *this = v; }
	void operator=(const CPlanetaryVertex &v)
	{
		m_vPosition = v.m_vPosition;
		m_vNormal = v.m_vNormal;
		m_fTexCoord[0] = v.m_fTexCoord[0];
		m_fTexCoord[1] = v.m_fTexCoord[1];
	}
	void Init(CVector &v)
	{
		m_vPosition = v;
		m_vNormal = v;
		m_fTexCoord[0] = m_fTexCoord[1] = 0;
	}
};

/******************************************************************************
* Class: CPlanetaryMapNode
*******************************************************************************
* This class encapsulates a single quad-tree node. Being a recursive object,
* this is the heart of the quad-tree.
******************************************************************************/
class CPlanetaryMapNode
{
protected:
	// Lookup tables
	static unsigned short m_nQuadrantOffset[4];
	static unsigned short m_nQuadrantHeightOffset[4];
	static unsigned short m_nFaceNeighbor[6][4];
	static SBufferShape m_shape;

	CPlanetaryMapNode *m_pParent;
	CPlanetaryMapNode *m_pChild[4];
	CPlanetaryMapNode *GetParent()				{ return m_pParent; }
	void SetParent(CPlanetaryMapNode *p)		{ m_pParent = p; }
	CPlanetaryMapNode *GetChild(int n)			{ return m_pChild[n]; }
	void SetChild(int n, CPlanetaryMapNode *p)	{ m_pChild[n] = p; }

	CGLVertexBufferObject m_boVertex;
	CPlanetaryVertex m_vNode[SURFACE_MAP_COUNT];
	CPixelBuffer m_pbHeightMap;

	// Node-specific members
	unsigned int m_nNodeFlags;				// A set of bit flags tracking various node states
	CPlanetaryMap *m_pMap;					// A pointer to the parent map (contains useful state information)
	CPlanetaryMapNode *m_pNeighbor[4];		// Pointers to the 4 neighbors (NULL if the neighbor is not split down to the current node's level)
	unsigned char m_nSort[4];				// Indexes to the 4 quadrants sorted by distance from the camera (used by Update() and Render())
	float m_fCorner[4];						// The corners of this node (x min, y min, x max, y max)

	unsigned short m_nTextureID;			// The texture map for the current node
	unsigned short m_nBumpID;				// The bump map for the current node

public:

	static void InitTables();

	// Initialization and cleanup methods
	CPlanetaryMapNode(CPlanetaryMap *pMap, int nFace);
	CPlanetaryMapNode(CPlanetaryMapNode *pParent, int nQuadrant);
	~CPlanetaryMapNode();
	void InitNode(int nFace, int nQuadrant);
	void SetNeighbors(CPlanetaryMapNode *pTop, CPlanetaryMapNode *pRight, CPlanetaryMapNode *pBottom, CPlanetaryMapNode *pLeft)
	{
		m_pNeighbor[TopEdge] = pTop;
		m_pNeighbor[RightEdge] = pRight;
		m_pNeighbor[BottomEdge] = pBottom;
		m_pNeighbor[LeftEdge] = pLeft;
	}


	// Some common Get methods
	CPlanetaryMap *GetPlanetaryMap()				{ return m_pMap; }
	short GetFace()									{ return m_nNodeFlags & FaceMask; }
	int GetQuadrant()								{ return (m_nNodeFlags & QuadrantMask) >> 3; }
	int GetLevel()									{ return (m_nNodeFlags & LevelMask) >> 5; }
	bool IsDirty()									{ return (m_nNodeFlags & NodeDirty) != 0; }
	bool IsCameraIn(int n)							{ return (m_nNodeFlags & (1 << (10+n))) != 0; }
	bool IsBeyondHorizon(int n)						{ return (m_nNodeFlags & (1 << (16+n))) != 0; }
	bool IsOutsideFrustum(int n)					{ return (m_nNodeFlags & (1 << (20+n))) != 0; }
	bool HasTexture()								{ return m_nTextureID != (unsigned short)-1; }

	float GetMinX()									{ return m_fCorner[0]; }
	float GetMinY()									{ return m_fCorner[1]; }
	float GetMaxX()									{ return m_fCorner[2]; }
	float GetMaxY()									{ return m_fCorner[3]; }
	float GetMidX()									{ return (m_fCorner[2]+m_fCorner[0]) * 0.5f; }
	float GetMidY()									{ return (m_fCorner[3]+m_fCorner[1]) * 0.5f; }
	float GetWidth()								{ return m_fCorner[2] - m_fCorner[0]; }
	float GetHeight()								{ return m_fCorner[3] - m_fCorner[1]; }

	unsigned short GetIndex(int x, int y)			{ return y * SURFACE_MAP_WIDTH + x; }
	unsigned short GetHeightIndex(int x, int y)		{ return y * HEIGHT_MAP_WIDTH + x; }
	CPlanetaryVertex *GetVertex(int nIndex)			{ return &m_vNode[nIndex]; }
	CVector GetDirectionVector(float x, float y)	{ return ::GetPlanetaryVector(GetFace(), x, y, 1.0f); }
	bool IsLeaf()									{ return (m_pChild[0] == NULL && m_pChild[1] == NULL && m_pChild[2] == NULL && m_pChild[3] == NULL); }

	bool AffectsNode(const CPlanetaryMapCoord &qtc, float fRadius)
	{
		if(qtc.GetFace() == GetFace())
		{
			if(qtc.x + fRadius > m_fCorner[0] && qtc.x - fRadius < m_fCorner[2] && qtc.y + fRadius > m_fCorner[1] && qtc.y - fRadius < m_fCorner[3])
				return true;
		}
		return false;
	}

	bool HitTest(const CPlanetaryMapCoord &qtc, unsigned char nQuadrant=-1)
	{
		if(qtc.GetFace() != GetFace())
			return false;
		switch(nQuadrant)
		{
			case TopLeft:		// Is coordinate in top-left quadrant?
				return (qtc.x >= m_fCorner[0] && qtc.x <= CMath::Avg(m_fCorner[0], m_fCorner[2]) &&
						qtc.y >= m_fCorner[1] && qtc.y <= CMath::Avg(m_fCorner[1], m_fCorner[3]));
			case TopRight:		// Is coordinate in top-right quadrant?
				return (qtc.x >= CMath::Avg(m_fCorner[0], m_fCorner[2]) && qtc.x <= m_fCorner[2] &&
						qtc.y >= m_fCorner[1] && qtc.y <= CMath::Avg(m_fCorner[1], m_fCorner[3]));
			case BottomLeft:	// Is coordinate in bottom-left quadrant?
				return (qtc.x >= m_fCorner[0] && qtc.x <= CMath::Avg(m_fCorner[0], m_fCorner[2]) &&
						qtc.y >= CMath::Avg(m_fCorner[1], m_fCorner[3]) && qtc.y <= m_fCorner[3]);
			case BottomRight:	// Is coordinate in bottom-right quadrant?
				return (qtc.x >= CMath::Avg(m_fCorner[0], m_fCorner[2]) && qtc.x <= m_fCorner[2] &&
						qtc.y >= CMath::Avg(m_fCorner[1], m_fCorner[3]) && qtc.y <= m_fCorner[3]);
		}
		// Is coordinate in node?
		return (qtc.x >= m_fCorner[0] && qtc.x <= m_fCorner[2] &&
				qtc.y >= m_fCorner[1] && qtc.y <= m_fCorner[3]);
	}

	// Recurses through the tree to find the current height at a specific map coordinate
	float GetHeight(const CPlanetaryMapCoord &qtc)
	{
		int nQuadrant = -1;
		float fMidX = CMath::Avg(m_fCorner[0], m_fCorner[2]);
		float fMidY = CMath::Avg(m_fCorner[1], m_fCorner[3]);
		if(qtc.x <= fMidX && qtc.y <= fMidY)
			nQuadrant = TopLeft;
		else if(qtc.x >= fMidX && qtc.y <= fMidY)
			nQuadrant = TopRight;
		else if(qtc.x <= fMidX && qtc.y >= fMidY)
			nQuadrant = BottomLeft;
		else if(qtc.x >= fMidX && qtc.y >= fMidY)
			nQuadrant = BottomRight;

		if(m_pChild[nQuadrant])
			return m_pChild[nQuadrant]->GetHeight(qtc);

		return GetHeightFromMap(qtc);
	}

	float GetHeightFromMap(const CPlanetaryMapCoord &qtc)
	{	// Only call this method when you're certain x and y are in this node
		return GetHeightFromMap(qtc.x, qtc.y);
	}
	float GetHeightFromMap(float x, float y)
	{	// Only call this method when you're certain x and y are in this node
		float fHeight;
		m_pbHeightMap.Interpolate(&fHeight, (x - m_fCorner[0]) / (m_fCorner[2] - m_fCorner[0]), (y - m_fCorner[1]) / (m_fCorner[3] - m_fCorner[1]));
		return fHeight;
	}

	CPlanetaryMapCoord GetNearestCoord(const CPlanetaryMapCoord &qtc, unsigned char nQuadrant=-1)
	{
		float x, y;
		short nFace = GetFace();
		CPlanetaryMapCoord qtcTemp(nFace, qtc.GetDirectionVector());
		switch(nQuadrant)
		{
			case TopLeft:		// Get nearest coordinate for top-left quadrant
				x = CMath::Max(m_fCorner[0], CMath::Min(CMath::Avg(m_fCorner[0], m_fCorner[2]), qtcTemp.x));
				y = CMath::Max(m_fCorner[1], CMath::Min(CMath::Avg(m_fCorner[1], m_fCorner[3]), qtcTemp.y));
				break;
			case TopRight:		// Get nearest coordinate to top-right quadrant
				x = CMath::Max(CMath::Avg(m_fCorner[0], m_fCorner[2]), CMath::Min(m_fCorner[2], qtcTemp.x));
				y = CMath::Max(m_fCorner[1], CMath::Min(CMath::Avg(m_fCorner[1], m_fCorner[3]), qtcTemp.y));
				break;
			case BottomLeft:	// Get nearest coordinate to bottom-left quadrant
				x = CMath::Max(m_fCorner[0], CMath::Min(CMath::Avg(m_fCorner[0], m_fCorner[2]), qtcTemp.x));
				y = CMath::Max(CMath::Avg(m_fCorner[1], m_fCorner[3]), CMath::Min(m_fCorner[3], qtcTemp.y));
				break;
			case BottomRight:	// Get nearest coordinate to bottom-right quadrant
				x = CMath::Max(CMath::Avg(m_fCorner[0], m_fCorner[2]), CMath::Min(m_fCorner[2], qtcTemp.x));
				y = CMath::Max(CMath::Avg(m_fCorner[1], m_fCorner[3]), CMath::Min(m_fCorner[3], qtcTemp.y));
				break;
			default:			// Get nearest coordinate to entire node
				x = CMath::Max(m_fCorner[0], CMath::Min(m_fCorner[2], qtcTemp.x));
				y = CMath::Max(m_fCorner[1], CMath::Min(m_fCorner[3], qtcTemp.y));
				break;
		}
		return CPlanetaryMapCoord(nFace, x, y);
	}

	// Methods for finding neighboring nodes, edges, and vertices
	CPlanetaryMapNode *FindQuadrantNeighbor(int nQuadrant, int nEdge, bool bForceSplit=false);
	CPlanetaryMapNode *FindNeighbor(int nEdge, bool bForceSplit=false)
	{
		if(!m_pParent)
			return m_pNeighbor[nEdge];
		return m_pParent->FindQuadrantNeighbor(GetQuadrant(), nEdge, bForceSplit);
	}
	void UpdateNeighbors()
	{
		for(int i=0; i<4; i++)
			m_pNeighbor[i] = FindNeighbor(i);
	}
	int FindMatchingEdge(int nEdge)
	{
		for(int i=0; i<4; i++)
		{
			if(m_pNeighbor[nEdge] && m_pNeighbor[nEdge]->m_pNeighbor[i] == this)
				return i;
		}
		return -1;
	}


	// Recursive methods for updating and rendering the quad-tree
	void Split(int nQuadrant);
	bool CanMerge();
	bool Merge(int nQuadrant);
	void UpdateSurface();
	int DrawSurface();
};


#endif // __PlanetaryMapNode_h__
