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

#ifndef __PlanetaryMapCoord_h__
#define __PlanetaryMapCoord_h__

enum
{	// Enumerated values indicating the order of the cube faces
	RightFace = 0,
	LeftFace = 1,
	TopFace = 2,
	BottomFace = 3,
	FrontFace = 4,
	BackFace = 5
};

enum
{	// Enumerated values for the m_nState member below
	NoState = 0,
	Borrowed = 1
};


// Global methods
extern unsigned char GetFace(const CVector &v);
extern void GetFaceCoordinates(unsigned char nFace, const CVector &v, float &x, float &y);
inline unsigned char GetFaceCoordinates(const CVector &v, float &x, float &y)
{
	unsigned char nFace = GetFace(v);
	GetFaceCoordinates(nFace, v, x, y);
	return nFace;
}
extern CVector GetPlanetaryVector(unsigned char nFace, float x, float y, float fLength=1.0f);


class CFaceCoord
{
public:
	// Use fixed-point unsigned integers for x and y? (they should always be between 0 and 1)
	// Unless we limited ourselves to 16 bits, multiplication would require retrieval of the high DWORD
	// It would give better precision, and it may improve performance
	// Conversion between floating point and integer would lose that precision again
	float x, y;		// The X and Y coordinates within a map face (from 0 to 1)

public:
	CFaceCoord() {}
	CFaceCoord(const CFaceCoord &c)
	{
		*this = c;
	}
	CFaceCoord(unsigned char nFace, const CVector &v)
	{
		Init(nFace, v);
	}
	CFaceCoord(float x, float y)
	{
		Init(x, y);
	}

	void Init(unsigned char nFace, const CVector &v)
	{
		::GetFaceCoordinates(nFace, v, x, y);
	}
	void Init(float x, float y)
	{
		this->x = x;
		this->y = y;
	}

	const CFaceCoord &operator=(const CFaceCoord &c)
	{
		x = c.x;
		y = c.y;
		return *this;
	}

	CVector GetVector(int nFace, float fSize=1.0f) const
	{
		return ::GetPlanetaryVector(nFace, x, y, fSize);
	}
};


// Utility class for handling conversions between 3D and quad-tree coordinates
class CPlanetaryMapCoord : public CFaceCoord
{
protected:
	float m_fHeight;		// Height offset from the radius of the planet
	unsigned char m_nFace;	// The cube face this coordinate lies in
	unsigned char m_nState;	// Storage for the state of this planetary coord

public:
	CPlanetaryMapCoord() {}
	CPlanetaryMapCoord(const CPlanetaryMapCoord &c)
	{
		*this = c;
	}
	CPlanetaryMapCoord(unsigned char nFace, const CFaceCoord &c, float fHeight=0)
	{
		Init(nFace, c, fHeight);
	}
	CPlanetaryMapCoord(const CVector &v, float fHeight=0)	// Don't use this method when the face is known (rounding errors may occur near edge boundaries)
	{
		Init(v, fHeight);
	}
	CPlanetaryMapCoord(unsigned char nFace, const CVector &v, float fHeight=0)
	{
		Init(nFace, v, fHeight);
	}
	CPlanetaryMapCoord(unsigned char nFace, float x, float y, float fHeight=0)
	{
		Init(nFace, x, y, fHeight);
	}

	void Init(unsigned char nFace, const CFaceCoord &c, float fHeight=0)
	{
		m_nFace = nFace;
		m_nState = NoState;
		m_fHeight = fHeight;
		x = c.x;
		y = c.y;
	}
	void Init(const CVector &v, float fHeight=0)	// Don't use this method when the face is known (rounding errors may occur near edge boundaries)
	{
		m_nFace = ::GetFace(v);
		m_nState = NoState;
		m_fHeight = fHeight;
		CFaceCoord::Init(m_nFace, v);
	}
	void Init(unsigned char nFace, const CVector &v, float fHeight=0)
	{
		m_nFace = nFace;
		m_nState = NoState;
		m_fHeight = fHeight;
		CFaceCoord::Init(m_nFace, v);
	}
	void Init(unsigned char nFace, float x, float y, float fHeight=0)
	{
		m_nFace = nFace;
		m_nState = NoState;
		m_fHeight = fHeight;
		CFaceCoord::Init(x, y);
	}

	const CPlanetaryMapCoord &operator=(const CPlanetaryMapCoord &coord)
	{
		m_nFace = coord.m_nFace;
		m_nState = coord.m_nState;
		m_fHeight = coord.m_fHeight;
		x = coord.x;
		y = coord.y;
		return *this;
	}

	void SetState(unsigned char n)			{ m_nState = n; }
	void SetHeight(float f)					{ m_fHeight = f; }
	unsigned char GetFace() const			{ return m_nFace; }
	unsigned char GetState() const			{ return m_nState; }
	float GetHeight() const					{ return m_fHeight; }

	CVector GetDirectionVector() const		{ return CFaceCoord::GetVector(m_nFace); }
	CVector GetPositionVector() const		{ return GetPositionVector(m_fHeight); }
	CVector GetPositionVector(float fHeight) const	{ return CFaceCoord::GetVector(m_nFace, fHeight); }
};

#endif // __PlanetaryMapCoord_h__
