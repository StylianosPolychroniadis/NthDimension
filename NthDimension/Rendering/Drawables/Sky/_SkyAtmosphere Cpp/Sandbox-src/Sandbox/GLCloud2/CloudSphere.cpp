// CloudSphere.cpp
//

#include "StdAfx.h"
#include "CloudSphere.h"


void CCloudSphere::Draw(const CSRTTransform &camera, CTexture &tex)
{
	CMatrix4 m = BuildModelMatrix();
	glPushMatrix();
	glMultMatrixf(m);
	glMatrixMode(GL_TEXTURE);

	CVector vTo = -camera.GetViewAxis();
	//CVector vTo(0, 0, 1);
	//CVector vTo = pCamera->GetPosition() - this->GetPosition();
	vTo.Normalize();
	CVector vUp(0, 1, 0);
	CVector vRight = vUp ^ vTo;
	vRight.Normalize();
	vUp = vTo ^ vRight;
	vUp.Normalize();

	glActiveTexture(GL_TEXTURE0);
	glLoadMatrixf(m_mTex1);
	//tex.Enable();

	glActiveTexture(GL_TEXTURE1);
	glLoadMatrixf(m_mTex2);
	//tex.Enable();
	
	glActiveTexture(GL_TEXTURE2);
	glLoadMatrixf(m_mTex3);
	//tex.Enable();
	
	glActiveTexture(GL_TEXTURE3);
	glLoadMatrixf(m_mTex3);
	//tex.Enable();

	m_mTex1.Translate(CVector(0.0001f, -0.0001f, 0));
	//m_mTex1.RotateY(-0.0002f);
	m_mTex2.Translate(CVector(0, -0.0001f, 0));
	//m_mTex2.RotateX(0.0005f);
	m_mTex3.Translate(CVector(0, 0, 0.0001f));
	//m_mTex3.RotateZ(0.0005f);
	m_mTex4.Translate(CVector(-0.0001f, 0, 0));
	
	glDisable(GL_LIGHTING);
	glEnable(GL_BLEND);
	glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);

	int nSlices = 10;
	int nHalf = nSlices >> 1;
	//glEnable(GL_ALPHA_TEST);
	//glAlphaFunc(GL_GEQUAL, 0.25f / nSlices);
	for(int z=0; z<nSlices; z++)
	{
		CVector v = CVector(0, 0, 0) - vTo * (m_fBoundingRadius * (nHalf-z)) / (float)nHalf;
		// Use the smallest size square we can get away with for each slice to minimize pixel overdraw
		float fSize = sinf(acosf(v.Magnitude() / m_fBoundingRadius)) * m_fBoundingRadius;
		for(int y=0; y<nSlices; y++)
		{
			glBegin(GL_TRIANGLE_STRIP);
			for(int x=0; x<=nSlices; x++)
			{
				CVector vPos = v - vRight * ((fSize * (nHalf-x)) / (float)nHalf) + vUp * ((fSize * (nHalf-y)) / (float)nHalf);
				float fMagnitude = vPos.Magnitude();
				CVector vCoord = vPos / m_fBoundingRadius;
				float fColor = sqrtf((vPos.y+m_fBoundingRadius) * 0.5f / m_fBoundingRadius);
				float fAlpha = (m_fBoundingRadius - fMagnitude) / nSlices;
				glColor4f(fColor, fColor, fColor, fAlpha);
				glMultiTexCoord3fv(GL_TEXTURE0_ARB, vCoord*0.125f);
				glMultiTexCoord3fv(GL_TEXTURE1_ARB, vCoord*0.25f);
				glMultiTexCoord3fv(GL_TEXTURE2_ARB, vCoord*0.5f);
				glMultiTexCoord3fv(GL_TEXTURE3_ARB, vCoord);
				glVertex3fv(vPos);
				vPos -= vUp * fSize / (float)nHalf;
				fMagnitude = vPos.Magnitude();
				vCoord = vPos / m_fBoundingRadius;
				fColor = sqrtf((vPos.y+m_fBoundingRadius) * 0.5f / m_fBoundingRadius);
				fAlpha = (m_fBoundingRadius - fMagnitude) / nSlices;
				glColor4f(fColor, fColor, fColor, fAlpha);
				glMultiTexCoord3fv(GL_TEXTURE0_ARB, vCoord*0.125f);
				glMultiTexCoord3fv(GL_TEXTURE1_ARB, vCoord*0.25f);
				glMultiTexCoord3fv(GL_TEXTURE2_ARB, vCoord*0.5f);
				glMultiTexCoord3fv(GL_TEXTURE3_ARB, vCoord);
				glVertex3fv(vPos);
			}
			glEnd();
		}
	}
	glDisable(GL_ALPHA_TEST);

	glDisable(GL_BLEND);
	glEnable(GL_LIGHTING);

	glActiveTexture(GL_TEXTURE3);
	tex.Disable();
	glActiveTexture(GL_TEXTURE2);
	tex.Disable();
	glActiveTexture(GL_TEXTURE1);
	tex.Disable();
	glActiveTexture(GL_TEXTURE0);
	tex.Disable();

	glMatrixMode(GL_MODELVIEW);
	glPopMatrix();
}

