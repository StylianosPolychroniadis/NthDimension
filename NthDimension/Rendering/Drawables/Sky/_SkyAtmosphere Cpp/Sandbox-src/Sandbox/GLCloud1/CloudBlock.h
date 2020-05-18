/*
s_p_oneil@hotmail.com
Copyright (c) 2000, Sean O'Neil
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice,
  this list of conditions and the following disclaimer.
* Redistributions in binary form must reproduce the above copyright notice,
  this list of conditions and the following disclaimer in the documentation
  and/or other materials provided with the distribution.
* Neither the name of this project nor the names of its contributors
  may be used to endorse or promote products derived from this software
  without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
POSSIBILITY OF SUCH DAMAGE.
*/

#ifndef __CloudBlock_h__
#define __CloudBlock_h__

class CCloudCell
{
public:
	unsigned char m_cDensity, m_cBrightness;
};

class CCloudBlock : public CSRTTransform
{
protected:
	unsigned short nx, ny, nz;
	CCloudCell *m_pGrid;

public:
	CCloudBlock()
	{
		m_pGrid = NULL;
	}
	void Cleanup()
	{
		if(m_pGrid)
		{
			delete m_pGrid;
			m_pGrid = NULL;
		}
	}
	void Init(unsigned short x, unsigned short y, unsigned short z)
	{
		Cleanup();
		nx = x;
		ny = y;
		nz = z;
		m_pGrid = new CCloudCell[nx*ny*nz];
	}

	int GetCellPos(unsigned short x, unsigned short y, unsigned short z)
	{
		if(x >= nx || y >= ny || z >= nz)
			return -1;
		return ((z * ny + y) * nx) + x;
	}
	CCloudCell *GetCloudCell(unsigned short x, unsigned short y, unsigned short z)
	{
		int nPos = GetCellPos(x, y, z);
		if(nPos < 0)
			return NULL;
		return &m_pGrid[nPos];
	}

	void NoiseFill(int nSeed);
	void Light(CVector &vLight);
	void Draw(const CSRTTransform &camera, float fHalfSize);
};

#endif // __CloudBlock_h__
