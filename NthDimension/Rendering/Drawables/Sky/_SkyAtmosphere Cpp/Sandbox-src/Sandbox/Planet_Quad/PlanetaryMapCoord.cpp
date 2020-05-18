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
#include "PlanetaryMapCoord.h"


unsigned char GetFace(const CVector &v)
{
	unsigned char nFace;
	float x = CMath::Abs(v.x), y = CMath::Abs(v.y), z = CMath::Abs(v.z);
	if(x > y && x > z)
		nFace = (v.x > 0) ? RightFace : LeftFace;
	else if(y > x && y > z)
		nFace = (v.y > 0) ? TopFace : BottomFace;
	else
		nFace = (v.z > 0) ? FrontFace : BackFace;
	return nFace;
}

void GetFaceCoordinates(unsigned char nFace, const CVector &v, float &x, float &y)
{
	// The vector passed in may not be in the specified face.
	// If not, the coordinates within nFace closest to v are returned.
	// (This helps find the shortest distance from a point to a node in the quad-tree)
	float xABS = CMath::Abs(v.x), yABS = CMath::Abs(v.y), zABS = CMath::Abs(v.z);
	switch(nFace)
	{
		case RightFace:
			x = xABS <= zABS ? (v.z > 0 ? -1 : 1) : (-v.z / xABS);
			y = xABS <= yABS ? (v.y > 0 ? -1 : 1) : (-v.y / xABS);
			// If v is on the opposite face, we need to force the coordinates to the edge of this face.
			if(v.x < 0.0f && xABS > zABS && xABS > yABS)
			{
				if(CMath::Abs(x) > CMath::Abs(y))
					x = x > 0 ? 1 : -1;
				else
					y = y > 0 ? 1 : -1;
			}
			break;
		case LeftFace:
			x = xABS <= zABS ? (v.z > 0.0f ? 1.0f : -1.0f) : (v.z / xABS);
			y = xABS <= yABS ? (v.y > 0.0f ? -1.0f : 1.0f) : (-v.y / xABS);
			// If v is on the opposite face, we need to force the coordinates to the edge of this face.
			if(v.x > 0.0f && xABS > zABS && xABS > yABS)
			{
				if(CMath::Abs(x) > CMath::Abs(y))
					x = x > 0 ? 1 : -1;
				else
					y = y > 0 ? 1 : -1;
			}
			break;
		case TopFace:
			x = yABS <= xABS ? (v.x > 0.0f ? 1.0f : -1.0f) : (v.x / yABS);
			y = yABS <= zABS ? (v.z > 0.0f ? 1.0f : -1.0f) : (v.z / yABS);
			// If v is on the opposite face, we need to force the coordinates to the edge of this face.
			if(v.y < 0.0f && yABS > xABS && yABS > zABS)
			{
				if(CMath::Abs(x) > CMath::Abs(y))
					x = x > 0 ? 1 : -1;
				else
					y = y > 0 ? 1 : -1;
			}
			break;
		case BottomFace:
			x = yABS <= xABS ? (v.x > 0.0f ? 1.0f : -1.0f) : (v.x / yABS);
			y = yABS <= zABS ? (v.z > 0.0f ? -1.0f : 1.0f) : (-v.z / yABS);
			// If v is on the opposite face, we need to force the coordinates to the edge of this face.
			if(v.y > 0.0f && yABS > xABS && yABS > zABS)
			{
				if(CMath::Abs(x) > CMath::Abs(y))
					x = x > 0 ? 1 : -1;
				else
					y = y > 0 ? 1 : -1;
			}
			break;
		case FrontFace:
			x = zABS <= xABS ? (v.x > 0.0f ? 1.0f : -1.0f) : (v.x / zABS);
			y = zABS <= yABS ? (v.y > 0.0f ? -1.0f : 1.0f) : (-v.y / zABS);
			// If v is on the opposite face, we need to force the coordinates to the edge of this face.
			if(v.z < 0.0f && zABS > xABS && zABS > yABS)
			{
				if(CMath::Abs(x) > CMath::Abs(y))
					x = x > 0 ? 1 : -1;
				else
					y = y > 0 ? 1 : -1;
			}
			break;
		case BackFace:
			x = zABS <= xABS ? (v.x > 0.0f ? -1.0f : 1.0f) : (-v.x / zABS);
			y = zABS <= yABS ? (v.y > 0.0f ? -1.0f : 1.0f) : (-v.y / zABS);
			// If v is on the opposite face, we need to force the coordinates to the edge of this face.
			if(v.z > 0.0f && zABS > xABS && zABS > yABS)
			{
				if(CMath::Abs(x) > CMath::Abs(y))
					x = x > 0 ? 1 : -1;
				else
					y = y > 0 ? 1 : -1;
			}
			break;
	}

	// x and y should be approximately from -1 to 1, scale and clamp to range from 0 to 1
	x = CMath::Max(0.0f, CMath::Min(1.0f, (x + 1.0f) * 0.5f));
	y = CMath::Max(0.0f, CMath::Min(1.0f, (y + 1.0f) * 0.5f));
}

CVector GetPlanetaryVector(unsigned char nFace, float x, float y, float fLength)
{
	CVector v;
	float fx = (x * 2.0f) - 1.0f;
	float fy = (y * 2.0f) - 1.0f;
	switch(nFace)
	{
		case RightFace:
			v.x = 1;
			v.y = -fy;
			v.z = -fx;
			break;
		case LeftFace:
			v.x = -1;
			v.y = -fy;
			v.z = fx;
			break;
		case TopFace:
			v.x = fx;
			v.y = 1;
			v.z = fy;
			break;
		case BottomFace:
			v.x = fx;
			v.y = -1;
			v.z = -fy;
			break;
		case FrontFace:
			v.x = fx;
			v.y = -fy;
			v.z = 1;
			break;
		case BackFace:
			v.x = -fx;
			v.y = -fy;
			v.z = -1;
			break;
	}

	float fScale = fLength / v.Magnitude();
	return v * fScale;
}
