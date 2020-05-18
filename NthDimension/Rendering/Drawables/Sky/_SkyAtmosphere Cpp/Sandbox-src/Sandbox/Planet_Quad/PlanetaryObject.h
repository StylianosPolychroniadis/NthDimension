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

#ifndef __PlanetaryObect_h__
#define __PlanetaryObect_h__


class CPlanetaryMapNode; // Forward declaration


class CPlanetaryObject : public CObjectBase
{
public:
	typedef TReference<CPlanetaryObject> Ref;
	CPlanetaryObject(CObjectType *pType) : CObjectBase(pType) {}

	virtual void Init(CPropertySet &prop) {}
	virtual void Update() {}
	virtual void Draw() {}
};
DECLARE_GENERIC_TYPE_CLASS(CPlanetaryObject, CObjectType);

class CPlanetaryFactory : public CPlanetaryObject
{
public:
	typedef TReference<CPlanetaryFactory> Ref;
	CPlanetaryFactory(CObjectType *pType) : CPlanetaryObject(pType) {}
	virtual bool AffectsNode(CPlanetaryMapNode *pNode) { return true; }
	virtual void BuildNode(CPlanetaryMapNode *pNode) {}
	virtual void DestroyNode(CPlanetaryMapNode *pNode) {}
};
DECLARE_GENERIC_TYPE_CLASS(CPlanetaryFactory, CPlanetaryObjectType);

class CSimpleHeightMapFactory : public CPlanetaryFactory, public CFractal
{
protected:
	// Members for fractal routine
	int m_nFractalSeed;
	float m_fOctaves;
	float m_fRoughness;

public:
	typedef TReference<CSimpleHeightMapFactory> Ref;
	CSimpleHeightMapFactory(CObjectType *pType) : CPlanetaryFactory(pType)
	{
		m_nFractalSeed = 0;
	}

	virtual void Init(CPropertySet &prop)
	{
		m_nFractalSeed = prop.GetIntProperty("seed");
		m_fOctaves = prop.GetFloatProperty("octaves");
		m_fRoughness = prop.GetFloatProperty("roughness");
		CFractal::Init(3, m_nFractalSeed, 1.0f - m_fRoughness, 2.0f);
	}
	virtual void BuildNode(CPlanetaryMapNode *pNode);
};
DECLARE_GENERIC_TYPE_CLASS(CSimpleHeightMapFactory, CPlanetaryFactoryType);

class CSimpleCraterFactory : public CPlanetaryFactory, public CFractal
{
protected:
	int m_nSeed;
	int m_nTopLevel;
	int m_nBottomLevel;
	int m_nCratersPerLevel;
	float m_fCraterScale[50];

public:
	typedef TReference<CSimpleCraterFactory> Ref;
	CSimpleCraterFactory(CObjectType *pType) : CPlanetaryFactory(pType)
	{
		m_nSeed = 0;
	}

	virtual void Init(CPropertySet &prop)
	{
		m_nSeed = prop.GetIntProperty("seed");
		m_nTopLevel = prop.GetIntProperty("level.top");
		m_nBottomLevel = prop.GetIntProperty("level.bottom");
		m_nCratersPerLevel = prop.GetIntProperty("craters.per.level");
		m_fCraterScale[0] = 0.0f;
	}
	virtual void BuildNode(CPlanetaryMapNode *pNode);
};
DECLARE_GENERIC_TYPE_CLASS(CSimpleCraterFactory, CPlanetaryFactoryType);

class CSimpleColorMapFactory : public CPlanetaryFactory
{
protected:
	std::vector<CColor> m_vColors;

public:
	typedef TReference<CSimpleColorMapFactory> Ref;
	CSimpleColorMapFactory(CObjectType *pType) : CPlanetaryFactory(pType)
	{
	}

	virtual void Init(CPropertySet &prop)
	{
		for(int i=1; ; i++)
		{
			char szProperty[256];
			sprintf(szProperty, "color.%d", i);
			const char *pszColor = prop.GetProperty(szProperty);
			if(!pszColor)
				break;
			int n[4];
			sscanf(pszColor, "%d, %d, %d, %d", &n[0], &n[1], &n[2], &n[3]);
			m_vColors.push_back(CColor(n[0], n[1], n[2], n[3]));
		}
	}
	virtual void BuildNode(CPlanetaryMapNode *pNode);
};
DECLARE_GENERIC_TYPE_CLASS(CSimpleColorMapFactory, CPlanetaryFactoryType);


class CCrater
{
public:
	float x, y, m_fRadius;

	CCrater()
	{
	}
	void Init()
	{
		x = ((float)rand()/(float)RAND_MAX)*0.5f + 0.25f;
		y = ((float)rand()/(float)RAND_MAX)*0.5f + 0.25f;
		m_fRadius = ((float)rand()/(float)RAND_MAX)*0.125f + 0.125f;
	}
};

class CCraterMap
{
protected:
	int m_nSeed;
	int m_nCraters;
	CCrater m_pCrater[20];

public:
	// To go to each child, add a different prime number to minimize the chances of the same seed coming up
	enum {TopLeft=11, TopRight=101, BottomLeft=1009, BottomRight=10007};
	CCraterMap()							{}
	~CCraterMap()							{}

	void Init(int nSeed, int nCraters, bool bCreate=true)
	{
		int i;
		m_nSeed = nSeed;
		srand(m_nSeed);
		m_nCraters = nCraters;
		for(i=0; i<nCraters; i++)
		{
			if((float)rand()/(float)RAND_MAX < 0.5f)
				m_nCraters--;
		}
		for(i=0; i<m_nCraters; i++)
			m_pCrater[i].Init();
	}

	static int GetChildSeed(int nSeed, float &x, float &y)
	{
		nSeed = nSeed * 100;
		if(x < 0.5f)
		{
			if(y < 0.5f)
				nSeed += TopLeft;
			else
			{
				y -= 0.5f;
				nSeed += BottomLeft;
			}
		}
		else
		{
			if(y < 0.5f)
			{
				x -= 0.5f;
				nSeed += TopRight;
			}
			else
			{
				x -= 0.5f;
				y -= 0.5f;
				nSeed += BottomRight;
			}
		}
		x *= 2.0f;
		y *= 2.0f;
		return nSeed;
	}
	
	float GetDistance(int i, float x, float y)
	{
		float dx = x-m_pCrater[i].x;
		float dy = y-m_pCrater[i].y;
		dx *= dx;
		dy *= dy;
		return dx + dy;
	}
	float GetOffset(float x, float y, float fScale)
	{
		float fOffset = 0;
		for(int i=0; i<m_nCraters; i++)
		{
			float fDist = GetDistance(i, x, y);
			float fRadius = m_pCrater[i].m_fRadius * m_pCrater[i].m_fRadius;
			if(fDist < fRadius)
			{
				fDist /= fRadius;
				float fDist2 = fDist*fDist;
				fOffset += ((fDist2 - 0.25f) * 2.0f * m_pCrater[i].m_fRadius) * (1-fDist);
			}
		}
		return fOffset * fScale;
	}
};


#endif // __PlanetaryObect_h__
