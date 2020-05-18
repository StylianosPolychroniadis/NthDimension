// ProjectedTetrahedra.h
//

#ifndef __ProjectedTetrahedra_h__
#define __ProjectedTetrahedra_h__


/*
class CProjectedTetrahedra : public C3DObject
{
public:
	CVector m_vCorner[4];
	CPlane m_plane[4];

	// The first three vertices define one face counter-clockwise, the fourth is the opposite vertex
	CProjectedTetrahedra(const CVector &v1, const CVector &v2, const CVector &v3, const CVector &v4)
	{
		m_vCorner[0] = v1;
		m_vCorner[1] = v2;
		m_vCorner[2] = v3;
		m_vCorner[3] = v4;
		m_plane[0].Init(m_vCorner[2], m_vCorner[1], m_vCorner[3]);	// Opposite vertex 0
		m_plane[1].Init(m_vCorner[0], m_vCorner[2], m_vCorner[3]);	// Opposite vertex 1
		m_plane[2].Init(m_vCorner[1], m_vCorner[0], m_vCorner[3]);	// Opposite vertex 2
		m_plane[3].Init(m_vCorner[0], m_vCorner[1], m_vCorner[2]);	// Opposite vertex 3
	}

	float DistanceCheck(C3DObject *pCamera)
	{
		CVector vCamera = pCamera->GetPosition();
		CVector v = (m_vCorner[0] + m_vCorner[1] + m_vCorner[2] + m_vCorner[3]) * 0.25f;
		return vCamera.Distance(v);
	}

	void DrawVolume(C3DObject *pCamera)
	{
		CVector vPos = pCamera->GetPosition() - GetPosition();
		float fFacing[4];
		int nFacing[4];
		int nPos = 0, nNeg = 0;
		for(int i=0; i<4; i++)
		{
			fFacing[i] = m_plane[i].Distance(vPos);
			if(fFacing[i] > DELTA)
			{
				nPos++;
				nFacing[i] = 1;
			}
			else if(fFacing[i] < -DELTA)
			{
				nNeg++;
				nFacing[i] = -1;
			}
			else
			{
				nFacing[i] = 0;
			}
		}

		if(nNeg == 1 && nPos < 3)
		{
			for(int i=0; i<4; i++)
			{
				if(nFacing[i] == 0)
				{
					nFacing[i] = 1;
					nPos++;
				}
			}
		}
		if(nPos == 1 && nNeg < 3)
		{
			for(int i=0; i<4; i++)
			{
				if(nFacing[i] == 0)
				{
					nFacing[i] = -1;
					nNeg++;
				}
			}
		}

		glColor4ub(255, 255, 255, 255);
		if(nPos == 3 || nNeg == 3)
		{
			float fTex[4];
			CColor color[4];
			for(int i=0; i<4; i++)
			{
				fTex[i] = 0;
				if((nPos == 3 && nFacing[i] == -1) || (nNeg == 3 && nFacing[i] == 1))
				{
					CVector v = pCamera->GetPosition();
					m_plane[i].Intersection(v, m_vCorner[i]-v);
					float fAlpha = v.Distance(m_vCorner[i]) * 0.5f;
					fTex[i] = fAlpha;
				}
			}

			if(nNeg == 3)
				glCullFace(GL_FRONT);
			glPushMatrix();
			glMultMatrixf(GetModelMatrix(pCamera));
			glBegin(GL_TRIANGLES);
			glTexCoord1f(fTex[0]);
			glVertex3f(m_vCorner[0].x, m_vCorner[0].y, m_vCorner[0].z);
			glTexCoord1f(fTex[1]);
			glVertex3f(m_vCorner[1].x, m_vCorner[1].y, m_vCorner[1].z);
			glTexCoord1f(fTex[2]);
			glVertex3f(m_vCorner[2].x, m_vCorner[2].y, m_vCorner[2].z);

			glTexCoord1f(fTex[1]);
			glVertex3f(m_vCorner[1].x, m_vCorner[1].y, m_vCorner[1].z);
			glTexCoord1f(fTex[0]);
			glVertex3f(m_vCorner[0].x, m_vCorner[0].y, m_vCorner[0].z);
			glTexCoord1f(fTex[3]);
			glVertex3f(m_vCorner[3].x, m_vCorner[3].y, m_vCorner[3].z);

			glTexCoord1f(fTex[2]);
			glVertex3f(m_vCorner[2].x, m_vCorner[2].y, m_vCorner[2].z);
			glTexCoord1f(fTex[1]);
			glVertex3f(m_vCorner[1].x, m_vCorner[1].y, m_vCorner[1].z);
			glTexCoord1f(fTex[3]);
			glVertex3f(m_vCorner[3].x, m_vCorner[3].y, m_vCorner[3].z);

			glTexCoord1f(fTex[0]);
			glVertex3f(m_vCorner[0].x, m_vCorner[0].y, m_vCorner[0].z);
			glTexCoord1f(fTex[2]);
			glVertex3f(m_vCorner[2].x, m_vCorner[2].y, m_vCorner[2].z);
			glTexCoord1f(fTex[3]);
			glVertex3f(m_vCorner[3].x, m_vCorner[3].y, m_vCorner[3].z);
			glEnd();
			if(nNeg == 3)
				glCullFace(GL_BACK);
			glPopMatrix();
		}
		else if(nPos == 2 && nNeg == 2)
		{
			int nLine1[2], nLine2[2];
			if(nFacing[0] == 1 && nFacing[1] == 1)
			{
				nLine1[0] = 2;
				nLine1[1] = 3;
				nLine2[0] = 0;
				nLine2[1] = 1;
			}
			else if(nFacing[0] == 1 && nFacing[2] == 1)
			{
				nLine1[0] = 1;
				nLine1[1] = 3;
				nLine2[0] = 2;
				nLine2[1] = 0;
			}
			else if(nFacing[0] == 1 && nFacing[3] == 1)
			{
				nLine1[0] = 1;
				nLine1[1] = 2;
				nLine2[0] = 3;
				nLine2[1] = 0;
			}
			else if(nFacing[1] == 1 && nFacing[2] == 1)
			{
				nLine1[0] = 0;
				nLine1[1] = 3;
				nLine2[0] = 1;
				nLine2[1] = 2;
			}
			else if(nFacing[1] == 1 && nFacing[3] == 1)
			{
				nLine1[0] = 0;
				nLine1[1] = 2;
				nLine2[0] = 1;
				nLine2[1] = 3;
			}
			else if(nFacing[2] == 1 && nFacing[3] == 1)
			{
				nLine1[0] = 0;
				nLine1[1] = 1;
				nLine2[0] = 2;
				nLine2[1] = 3;
			}

			CVector v1, v2;
			v1 = m_vCorner[nLine1[0]];
			v2 = m_vCorner[nLine2[0]];
			CPlane plane1, plane2;
			CVector vPos = pCamera->GetPosition();
			plane1.Init(vPos, m_vCorner[nLine1[0]], m_vCorner[nLine1[1]]);
			plane2.Init(vPos, m_vCorner[nLine2[0]], m_vCorner[nLine2[1]]);
			plane1.Intersection(v2, m_vCorner[nLine2[1]]-m_vCorner[nLine2[0]]);
			plane2.Intersection(v1, m_vCorner[nLine1[1]]-m_vCorner[nLine1[0]]);
			float fAlpha = v1.Distance(v2) * 0.5f;

			glPushMatrix();
			glMultMatrixf(GetModelMatrix(pCamera));
			glDisable(GL_CULL_FACE);
			glBegin(GL_TRIANGLES);

			glTexCoord1f(fAlpha);
			glVertex3f(v1.x, v1.y, v1.z);
			glTexCoord1f(0.0f);
			glVertex3f(m_vCorner[nLine1[0]].x, m_vCorner[nLine1[0]].y, m_vCorner[nLine1[0]].z);
			glVertex3f(m_vCorner[nLine2[0]].x, m_vCorner[nLine2[0]].y, m_vCorner[nLine2[0]].z);

			glTexCoord1f(fAlpha);
			glVertex3f(v1.x, v1.y, v1.z);
			glTexCoord1f(0.0f);
			glVertex3f(m_vCorner[nLine2[0]].x, m_vCorner[nLine2[0]].y, m_vCorner[nLine2[0]].z);
			glVertex3f(m_vCorner[nLine1[1]].x, m_vCorner[nLine1[1]].y, m_vCorner[nLine1[1]].z);

			glTexCoord1f(fAlpha);
			glVertex3f(v1.x, v1.y, v1.z);
			glTexCoord1f(0.0f);
			glVertex3f(m_vCorner[nLine1[1]].x, m_vCorner[nLine1[1]].y, m_vCorner[nLine1[1]].z);
			glVertex3f(m_vCorner[nLine2[1]].x, m_vCorner[nLine2[1]].y, m_vCorner[nLine2[1]].z);

			glTexCoord1f(fAlpha);
			glVertex3f(v1.x, v1.y, v1.z);
			glTexCoord1f(0.0f);
			glVertex3f(m_vCorner[nLine2[1]].x, m_vCorner[nLine2[1]].y, m_vCorner[nLine2[1]].z);
			glVertex3f(m_vCorner[nLine1[0]].x, m_vCorner[nLine1[0]].y, m_vCorner[nLine1[0]].z);

			glEnd();
			glEnable(GL_CULL_FACE);
			glPopMatrix();
		}
	}

	void DrawOpaque(C3DObject *pCamera)
	{
		glPushMatrix();
		glMultMatrixf(GetModelMatrix(pCamera));
		glBegin(GL_TRIANGLES);

		glColor4d(1.0, 0.0, 0.0, 1.0);
		glVertex3f(m_vCorner[0].x, m_vCorner[0].y, m_vCorner[0].z);
		glColor4d(0.0, 1.0, 0.0, 1.0);
		glVertex3f(m_vCorner[1].x, m_vCorner[1].y, m_vCorner[1].z);
		glColor4d(0.0, 0.0, 1.0, 1.0);
		glVertex3f(m_vCorner[2].x, m_vCorner[2].y, m_vCorner[2].z);
		
		glColor4d(0.0, 1.0, 0.0, 1.0);
		glVertex3f(m_vCorner[1].x, m_vCorner[1].y, m_vCorner[1].z);
		glColor4d(1.0, 0.0, 0.0, 1.0);
		glVertex3f(m_vCorner[0].x, m_vCorner[0].y, m_vCorner[0].z);
		glColor4d(1.0, 1.0, 1.0, 1.0);
		glVertex3f(m_vCorner[3].x, m_vCorner[3].y, m_vCorner[3].z);

		glColor4d(0.0, 0.0, 1.0, 1.0);
		glVertex3f(m_vCorner[2].x, m_vCorner[2].y, m_vCorner[2].z);
		glColor4d(0.0, 1.0, 0.0, 1.0);
		glVertex3f(m_vCorner[1].x, m_vCorner[1].y, m_vCorner[1].z);
		glColor4d(1.0, 1.0, 1.0, 1.0);
		glVertex3f(m_vCorner[3].x, m_vCorner[3].y, m_vCorner[3].z);

		glColor4d(1.0, 0.0, 0.0, 1.0);
		glVertex3f(m_vCorner[0].x, m_vCorner[0].y, m_vCorner[0].z);
		glColor4d(0.0, 0.0, 1.0, 1.0);
		glVertex3f(m_vCorner[2].x, m_vCorner[2].y, m_vCorner[2].z);
		glColor4d(1.0, 1.0, 1.0, 1.0);
		glVertex3f(m_vCorner[3].x, m_vCorner[3].y, m_vCorner[3].z);

		glEnd();
		glPopMatrix();
	}
};
*/

enum
{	// Enumerated values indicating the order of the cube faces
	RightFace = 0,
	LeftFace = 1,
	TopFace = 2,
	BottomFace = 3,
	FrontFace = 4,
	BackFace = 5
};

class CProjectedCube : public CSRTTransform
{
protected:
	bool IsInFace(int nFace, int nVertex)
	{
		return m_nIndices[nFace][0] == nVertex || m_nIndices[nFace][1] == nVertex || m_nIndices[nFace][2] == nVertex || m_nIndices[nFace][3] == nVertex;
	}

	static const int m_nOpposite[6];
	static const int m_nIndices[6][4];


public:
	CVector m_vCorner[8];
	CPlane m_plane[6];

	// The first four vertices define the front face counter-clockwise starting with the top-left (looking at the front face)
	// The second four define the rear face in the same fashion (again looking at the front face)
	CProjectedCube(const CVector *pCorner)
	{
		for(int i=0; i<8; i++)
			m_vCorner[i] = pCorner[i];

		m_plane[FrontFace].Init(m_vCorner[0], m_vCorner[1], m_vCorner[2]);
		m_plane[BackFace].Init(m_vCorner[7], m_vCorner[6], m_vCorner[5]);
		m_plane[RightFace].Init(m_vCorner[3], m_vCorner[6], m_vCorner[7]);
		m_plane[LeftFace].Init(m_vCorner[1], m_vCorner[0], m_vCorner[4]);
		m_plane[TopFace].Init(m_vCorner[0], m_vCorner[3], m_vCorner[4]);
		m_plane[BottomFace].Init(m_vCorner[1], m_vCorner[5], m_vCorner[2]);
	}
	// The first four vertices define the front face counter-clockwise starting with the top-left (looking at the front face)
	// The second four define the rear face in the same fashion (again looking at the front face)
	CProjectedCube()
	{
		m_vCorner[0] = CVector(-0.5f, 0.5f, 0.5f);
		m_vCorner[1] = CVector(-0.5f, -0.5f, 0.5f);
		m_vCorner[2] = CVector(0.5f, -0.5f, 0.5f);
		m_vCorner[3] = CVector(0.5f, 0.5f, 0.5f);
		m_vCorner[4] = CVector(-0.5f, 0.5f, -0.5f);
		m_vCorner[5] = CVector(-0.5f, -0.5f, -0.5f);
		m_vCorner[6] = CVector(0.5f, -0.5f, -0.5f);
		m_vCorner[7] = CVector(0.5f, 0.5f, -0.5f);

		m_plane[FrontFace].Init(m_vCorner[0], m_vCorner[1], m_vCorner[2]);
		m_plane[BackFace].Init(m_vCorner[7], m_vCorner[6], m_vCorner[5]);
		m_plane[RightFace].Init(m_vCorner[3], m_vCorner[6], m_vCorner[7]);
		m_plane[LeftFace].Init(m_vCorner[1], m_vCorner[0], m_vCorner[4]);
		m_plane[TopFace].Init(m_vCorner[0], m_vCorner[3], m_vCorner[4]);
		m_plane[BottomFace].Init(m_vCorner[1], m_vCorner[5], m_vCorner[2]);
	}

	void DrawVolume(const CSRTTransform &camera)
	{
		int i, j, nVertex;
		CVector vTemp;
		glPushMatrix();
		glMultMatrixf(BuildModelMatrix());

		CVector vCamera = camera.GetPosition() - GetPosition();	// Get the camera position relative to this one
		float fFacing[6];
		int nFrontFace[6];
		int nCount = 0;
		CVector vCenter = (m_vCorner[0] + m_vCorner[6]) * 0.5f;
		float fDistance = vCamera.Distance(vCenter);
		for(i=0; i<6; i++)
		{
			fFacing[i] = m_plane[i].Distance(vCamera);
			if(fFacing[i] / fDistance > DELTA)
			{
				CVector v = vCamera;
				m_plane[i].Intersection(v, v);
				fFacing[i] = (v-vCenter).MagnitudeSquared();

				nFrontFace[nCount++] = i;
				for(j=nCount-1; j>0; j--)
				{
					if(fFacing[nFrontFace[j]] < fFacing[nFrontFace[j-1]])
					{
						int n = nFrontFace[j];
						nFrontFace[j] = nFrontFace[j-1];
						nFrontFace[j-1] = n;
					}
				}
			}
		}

		// nFrontFace[0] now has the most front-facing plane, and 1 and 2 have the next two front-facing planes (if they exist)

		if(nCount == 0)		// Camera is inside cube
		{
			// Determine the alpha value of each rear vertex by passing a ray from the camera
			// to the vertex, then cutting the ray off at the front plane and determining its length
			float fAlpha[8] = {0, 0, 0, 0, 0, 0, 0, 0};
			for(i=0; i<8; i++)
				fAlpha[i] = vCamera.Distance(m_vCorner[i]) * 0.25f;

			// Now draw the inside of all the faces using the alpha values calculated above
			glBegin(GL_QUADS);
			for(int nFace=0; nFace<6; nFace++)
			{
				for(i=3; i>=0; i--)
				{
					int nVertex = m_nIndices[nFace][i];
					//glColor4f(1, 1, 1, fAlpha[nVertex]);
					glTexCoord1f(fAlpha[nVertex]);
					glVertex3f(m_vCorner[nVertex].x, m_vCorner[nVertex].y, m_vCorner[nVertex].z);
				}
			}
			glEnd();
		}
		else if(nCount == 1)
		{
			// Determine the alpha value of each rear vertex by passing a ray from the camera
			// to the vertex, then cutting the ray off at the front plane and determining its length
			float fAlpha[8] = {0, 0, 0, 0, 0, 0, 0, 0};
			for(i=0; i<4; i++)
			{
				nVertex = m_nIndices[m_nOpposite[nFrontFace[0]]][i];
				vTemp = m_vCorner[nVertex]-vCamera;
				m_plane[nFrontFace[0]].Intersection(vCamera, vTemp);
				fAlpha[nVertex] = vCamera.Distance(m_vCorner[nVertex]) * 0.25f;
			}

			// Now draw the inside of all the back faces using the alpha values calculated above
			glBegin(GL_QUADS);
			for(int nFace=0; nFace<6; nFace++)
			{
				if(nFace == nFrontFace[0])
					continue;
				for(i=3; i>=0; i--)
				{
					int nVertex = m_nIndices[nFace][i];
					//glColor4f(1, 1, 1, fAlpha[nVertex]);
					glTexCoord1f(fAlpha[nVertex]);
					glVertex3f(m_vCorner[nVertex].x, m_vCorner[nVertex].y, m_vCorner[nVertex].z);
				}
			}
			glEnd();
		}
		else if(nCount == 2)
		{
			// Find the two vertices shared between the two front faces along with their adjacent vertices on the secondary front face
			// (Have to keep them in their normal counter-clockwise order for the triangle strip below)
			int nEdge[2] = {-1, -1};
			int nAdjacent[2] = {-1, -1};
			if(IsInFace(nFrontFace[0], m_nIndices[nFrontFace[1]][0]))
			{
				if(IsInFace(nFrontFace[0], m_nIndices[nFrontFace[1]][1]))
				{
					nEdge[0] = m_nIndices[nFrontFace[1]][0];
					nEdge[1] = m_nIndices[nFrontFace[1]][1];
					nAdjacent[0] = m_nIndices[nFrontFace[1]][3];
					nAdjacent[1] = m_nIndices[nFrontFace[1]][2];
				}
				else
				{
					nEdge[0] = m_nIndices[nFrontFace[1]][3];
					nEdge[1] = m_nIndices[nFrontFace[1]][0];
					nAdjacent[0] = m_nIndices[nFrontFace[1]][2];
					nAdjacent[1] = m_nIndices[nFrontFace[1]][1];
				}
			}
			else if(IsInFace(nFrontFace[0], m_nIndices[nFrontFace[1]][2]))
			{
				if(IsInFace(nFrontFace[0], m_nIndices[nFrontFace[1]][3]))
				{
					nEdge[0] = m_nIndices[nFrontFace[1]][2];
					nEdge[1] = m_nIndices[nFrontFace[1]][3];
					nAdjacent[0] = m_nIndices[nFrontFace[1]][1];
					nAdjacent[1] = m_nIndices[nFrontFace[1]][0];
				}
				else
				{
					nEdge[0] = m_nIndices[nFrontFace[1]][1];
					nEdge[1] = m_nIndices[nFrontFace[1]][2];
					nAdjacent[0] = m_nIndices[nFrontFace[1]][0];
					nAdjacent[1] = m_nIndices[nFrontFace[1]][3];
				}
			}

			// Find the adjacent rear-facing planes containing each edge/adjacent pair
			int nPlane[2] = {-1, -1};
			for(i=0; i<6; i++)
			{
				if(i == nFrontFace[0] || i == m_nOpposite[nFrontFace[0]] || i == nFrontFace[1] || i == m_nOpposite[nFrontFace[1]])
					continue;
				if(IsInFace(i, nEdge[0]))
					nPlane[0] = i;
				if(IsInFace(i, nEdge[1]))
					nPlane[1] = i;
			}

			// Create two new vertices by projecting the two shared vertices to the back-most facing plane
			// and truncating the resulting segment to fit between the adjacent planes
			CVector vNew[2];
			vNew[0] = m_vCorner[nEdge[0]];
			vNew[1] = m_vCorner[nEdge[1]];
			vTemp = vNew[0]-vCamera;
			m_plane[m_nOpposite[nFrontFace[0]]].Intersection(vNew[0], vTemp);
			vTemp = vNew[1]-vCamera;
			m_plane[m_nOpposite[nFrontFace[0]]].Intersection(vNew[1], vTemp);
			vTemp = vNew[0]-vNew[1];
			m_plane[nPlane[0]].Intersection(vNew[0], vTemp);
			m_plane[nPlane[1]].Intersection(vNew[1], vTemp);

			// Replace the adjacent vertices with the new ones (in a temporary array) and draw the mesh just like we did with 1 front face
			CVector vCorner[8] = {m_vCorner[0], m_vCorner[1], m_vCorner[2], m_vCorner[3], m_vCorner[4], m_vCorner[5], m_vCorner[6], m_vCorner[7]};
			vCorner[nAdjacent[0]] = vNew[0];
			vCorner[nAdjacent[1]] = vNew[1];
			
			// Calculate alpha like we did with 1 front face
			float fAlpha[8] = {0, 0, 0, 0, 0, 0, 0, 0};
			for(i=0; i<4; i++)
			{
				nVertex = m_nIndices[m_nOpposite[nFrontFace[0]]][i];
				vTemp = vCorner[nVertex]-vCamera;
				m_plane[nFrontFace[0]].Intersection(vCamera, vTemp);
				fAlpha[nVertex] = vCamera.Distance(vCorner[nVertex]) * 0.25f;
			}

			// Draw the inside of the rear faces like we did with 1 front face
			glBegin(GL_QUADS);
			for(int nFace=0; nFace<6; nFace++)
			{
				if(nFace == nFrontFace[0] || nFace == nFrontFace[1])
					continue;
				for(int i=3; i>=0; i--)
				{
					nVertex = m_nIndices[nFace][i];
					//glColor4f(1, 1, 1, fAlpha[nVertex]);
					glTexCoord1f(fAlpha[nVertex]);
					glVertex3f(vCorner[nVertex].x, vCorner[nVertex].y, vCorner[nVertex].z);
				}
			}
			glEnd();

			// Now draw the extra triangles required by the creation of the new vertices
			glBegin(GL_TRIANGLE_STRIP);
			//glColor4f(1, 1, 1, fAlpha[nEdge[0]]);
			glTexCoord1f(fAlpha[nEdge[0]]);
			glVertex3f(vCorner[nEdge[0]].x, vCorner[nEdge[0]].y, vCorner[nEdge[0]].z);
			//glColor4f(1, 1, 1, fAlpha[nAdjacent[0]]);
			glTexCoord1f(fAlpha[nAdjacent[0]]);
			glVertex3f(vCorner[nAdjacent[0]].x, vCorner[nAdjacent[0]].y, vCorner[nAdjacent[0]].z);
			//glColor4f(1, 1, 1, 0);
			glTexCoord1f(0);
			glVertex3f(m_vCorner[nAdjacent[0]].x, m_vCorner[nAdjacent[0]].y, m_vCorner[nAdjacent[0]].z);
			//glColor4f(1, 1, 1, fAlpha[nAdjacent[1]]);
			glTexCoord1f(fAlpha[nAdjacent[1]]);
			glVertex3f(vCorner[nAdjacent[1]].x, vCorner[nAdjacent[1]].y, vCorner[nAdjacent[1]].z);
			//glColor4f(1, 1, 1, 0);
			glTexCoord1f(0);
			glVertex3f(m_vCorner[nAdjacent[1]].x, m_vCorner[nAdjacent[1]].y, m_vCorner[nAdjacent[1]].z);
			//glColor4f(1, 1, 1, fAlpha[nEdge[1]]);
			glTexCoord1f(fAlpha[nEdge[1]]);
			glVertex3f(vCorner[nEdge[1]].x, vCorner[nEdge[1]].y, vCorner[nEdge[1]].z);
			glEnd();
		}
		else if(nCount == 3)
		{
			// First find the front and back corner vertices.
			// Also find the adjacent edge vertices of the front vertex for finding the inner rectangle (in counter-clockwise order)
			// Also find the fan vertices for the first and second triangle fans (in counter-clockwise order)
			int nFront, nBack;
			int nEdge[2];
			int nFan1[3];
			int nFan2[3];
			for(i=0; i<4; i++)
			{
				nVertex = m_nIndices[nFrontFace[0]][i];
				if(IsInFace(nFrontFace[1], nVertex) && IsInFace(nFrontFace[2], nVertex))
				{
					nFront = nVertex;
					nEdge[0] = m_nIndices[nFrontFace[0]][(i+1)&0x3];
					nEdge[1] = m_nIndices[nFrontFace[0]][(i+3)&0x3];
					nFan1[0] = m_nIndices[nFrontFace[0]][(i+1)&0x3];
					nFan1[1] = m_nIndices[nFrontFace[0]][(i+2)&0x3];
					nFan1[2] = m_nIndices[nFrontFace[0]][(i+3)&0x3];
				}

				nVertex = m_nIndices[m_nOpposite[nFrontFace[0]]][i];
				if(IsInFace(m_nOpposite[nFrontFace[1]], nVertex) && IsInFace(m_nOpposite[nFrontFace[2]], nVertex))
				{
					nBack = nVertex;
					nFan2[0] = m_nIndices[m_nOpposite[nFrontFace[0]]][(i+4-1)&0x3];
					nFan2[1] = m_nIndices[m_nOpposite[nFrontFace[0]]][(i+4-2)&0x3];
					nFan2[2] = m_nIndices[m_nOpposite[nFrontFace[0]]][(i+4-3)&0x3];
				}
			}

			// Create the inner rectangle
			// It consists of the front and back vertices and the two new vertices made by edge intersections projected onto the back face
			CVector vInner[4];
			vInner[0] = m_vCorner[nFront];
			vTemp = vInner[0]-vCamera;
			m_plane[m_nOpposite[nFrontFace[0]]].Intersection(vInner[0], vTemp);
			vInner[1] = m_vCorner[nEdge[0]];
			vTemp = vInner[1]-vCamera;
			m_plane[m_nOpposite[nFrontFace[0]]].Intersection(vInner[1], vTemp);
			vTemp = vInner[1]-vInner[0];
			if(!IsInFace(nFrontFace[1], nEdge[0]))
				m_plane[m_nOpposite[nFrontFace[1]]].Intersection(vInner[1], vTemp);
			if(!IsInFace(nFrontFace[2], nEdge[0]))
				m_plane[m_nOpposite[nFrontFace[2]]].Intersection(vInner[1], vTemp);
			vInner[2] = m_vCorner[nBack];
			vInner[3] = m_vCorner[nEdge[1]];
			vTemp = vInner[3]-vCamera;
			m_plane[m_nOpposite[nFrontFace[0]]].Intersection(vInner[3], vTemp);
			vTemp = vInner[3]-vInner[0];
			if(!IsInFace(nFrontFace[1], nEdge[1]))
				m_plane[m_nOpposite[nFrontFace[1]]].Intersection(vInner[3], vTemp);
			if(!IsInFace(nFrontFace[2], nEdge[1]))
				m_plane[m_nOpposite[nFrontFace[2]]].Intersection(vInner[3], vTemp);

			// Determine the alpha value of each inner vertex by passing a ray from the camera
			// to the vertex, then cutting the ray off at the front plane and determining its length
			float fAlpha[4];
			for(i=0; i<4; i++)
			{
				CVector v = vCamera;
				vTemp = vInner[i]-vCamera;
				m_plane[nFrontFace[0]].Intersection(vCamera, vTemp);
				fAlpha[i] = vCamera.Distance(vInner[i]) * 0.25f;
			}

			// Draw the first fan
			glBegin(GL_TRIANGLE_FAN);
			//glColor4f(1, 1, 1, fAlpha[0]);
			glTexCoord1f(fAlpha[0]);
			glVertex3f(vInner[0].x, vInner[0].y, vInner[0].z);
			//glColor4f(1, 1, 1, fAlpha[1]);
			glTexCoord1f(fAlpha[1]);
			glVertex3f(vInner[1].x, vInner[1].y, vInner[1].z);
			//glColor4f(1, 1, 1, fAlpha[3]);
			glTexCoord1f(fAlpha[3]);
			glVertex3f(vInner[3].x, vInner[3].y, vInner[3].z);
			//glColor4f(1, 1, 1, 0);
			glTexCoord1f(0);
			glVertex3f(m_vCorner[nFan2[0]].x, m_vCorner[nFan2[0]].y, m_vCorner[nFan2[0]].z);
			glVertex3f(m_vCorner[nFan2[1]].x, m_vCorner[nFan2[1]].y, m_vCorner[nFan2[1]].z);
			glVertex3f(m_vCorner[nFan2[2]].x, m_vCorner[nFan2[2]].y, m_vCorner[nFan2[2]].z);
			//glColor4f(1, 1, 1, fAlpha[1]);
			glTexCoord1f(fAlpha[1]);
			glVertex3f(vInner[1].x, vInner[1].y, vInner[1].z);
			glEnd();

			// Draw the second fan
			glBegin(GL_TRIANGLE_FAN);
			//glColor4f(1, 1, 1, fAlpha[2]);
			glTexCoord1f(fAlpha[2]);
			glVertex3f(vInner[2].x, vInner[2].y, vInner[2].z);
			//glColor4f(1, 1, 1, fAlpha[3]);
			glTexCoord1f(fAlpha[3]);
			glVertex3f(vInner[3].x, vInner[3].y, vInner[3].z);
			//glColor4f(1, 1, 1, fAlpha[1]);
			glTexCoord1f(fAlpha[1]);
			glVertex3f(vInner[1].x, vInner[1].y, vInner[1].z);
			//glColor4f(1, 1, 1, 0);
			glTexCoord1f(0);
			glVertex3f(m_vCorner[nFan1[0]].x, m_vCorner[nFan1[0]].y, m_vCorner[nFan1[0]].z);
			glVertex3f(m_vCorner[nFan1[1]].x, m_vCorner[nFan1[1]].y, m_vCorner[nFan1[1]].z);
			glVertex3f(m_vCorner[nFan1[2]].x, m_vCorner[nFan1[2]].y, m_vCorner[nFan1[2]].z);
			//glColor4f(1, 1, 1, fAlpha[3]);
			glTexCoord1f(fAlpha[3]);
			glVertex3f(vInner[3].x, vInner[3].y, vInner[3].z);
			glEnd();

			// Draw two corner triangles that don't fit into fans
			glBegin(GL_TRIANGLES);
			//glColor4f(1, 1, 1, fAlpha[1]);
			glTexCoord1f(fAlpha[1]);
			glVertex3f(vInner[1].x, vInner[1].y, vInner[1].z);
			//glColor4f(1, 1, 1, 0);
			glTexCoord1f(0);
			glVertex3f(m_vCorner[nFan2[2]].x, m_vCorner[nFan2[2]].y, m_vCorner[nFan2[2]].z);
			glVertex3f(m_vCorner[nFan1[0]].x, m_vCorner[nFan1[0]].y, m_vCorner[nFan1[0]].z);
			//glColor4f(1, 1, 1, fAlpha[3]);
			glTexCoord1f(fAlpha[3]);
			glVertex3f(vInner[3].x, vInner[3].y, vInner[3].z);
			//glColor4f(1, 1, 1, 0);
			glTexCoord1f(0);
			glVertex3f(m_vCorner[nFan1[2]].x, m_vCorner[nFan1[2]].y, m_vCorner[nFan1[2]].z);
			glVertex3f(m_vCorner[nFan2[0]].x, m_vCorner[nFan2[0]].y, m_vCorner[nFan2[0]].z);
			glEnd();
		}

		glPopMatrix();
	}
};

const int CProjectedCube::m_nOpposite[6] = {1, 0, 3, 2, 5, 4};
const int CProjectedCube::m_nIndices[6][4] =
{
	{3, 2, 6, 7},
	{4, 5, 1, 0},
	{4, 0, 3, 7},
	{1, 5, 6, 2},
	{0, 1, 2, 3},
	{7, 6, 5, 4}
};
#endif // __ProjectedTetrahedra_h__
