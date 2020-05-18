// ScatterCPU.cpp : Defines the entry point for the application.
//

#include "StdAfx.h"

DECLARE_CORE_GLOBALS;


#define SAMPLE_SIZE		5

struct SVertex
{
	CVector vPos;
	CColor cColor;
};

class CSphere
{
protected:
	float m_fRadius;
	int m_nSlices;
	int m_nSections;

	SVertex *m_pVertex;
	unsigned short m_nVertices;

public:
	CSphere()	{}

	int GetVertexCount()		{ return m_nVertices; }
	SVertex *GetVertexBuffer()	{ return m_pVertex; }
	
	void Init(float fRadius, int nSlices, int nSections)
	{
		int i;
		m_fRadius = fRadius;
		m_nSlices = nSlices;
		m_nSections = nSections;

		m_nVertices = nSlices * (nSections-1) + 2;
		m_pVertex = new SVertex[m_nVertices];

		float fSliceArc = 2*PI / nSlices;
		float fSectionArc = PI / nSections;
		float *fRingz = new float[nSections+1];
		float *fRingSize = new float[nSections+1];
		float *fRingx = new float[nSlices+1];
		float *fRingy = new float[nSlices+1];
		for(i=0; i<=nSections; i++)
		{
			fRingz[i] = cosf(fSectionArc * i);
			fRingSize[i] = sinf(fSectionArc * i);
		}
		for(i=0; i<=nSlices; i++)
		{
			fRingx[i] = cosf(fSliceArc * i);
			fRingy[i] = sinf(fSliceArc * i);
		}

		int nIndex = 0;
		m_pVertex[nIndex++].vPos = CVector(0, 0, fRadius);
		for(int j=1; j<nSections; j++)
		{
			for(int i=0; i<nSlices; i++)
			{
				CVector v;
				v.x = fRingx[i] * fRingSize[j];
				v.y = fRingy[i] * fRingSize[j];
				v.z = fRingz[j];
				v *= fRadius / v.Magnitude();
				m_pVertex[nIndex++].vPos = v;
			}
		}

		m_pVertex[nIndex++].vPos = CVector(0, 0, -fRadius);
	}

	void Draw()
	{
		int i;
		glBegin(GL_TRIANGLE_FAN);
		glColor4ubv(m_pVertex[0].cColor);
		glVertex3fv(m_pVertex[0].vPos);
		for(i=0; i<m_nSlices; i++)
		{
			glColor4ubv(m_pVertex[i+1].cColor);
			glVertex3fv(m_pVertex[i+1].vPos);
		}
		glColor4ubv(m_pVertex[1].cColor);
		glVertex3fv(m_pVertex[1].vPos);
		glEnd();

		int nIndex1 = 1;
		int nIndex2 = 1 + m_nSlices;
		for(int j=1; j<m_nSections-1; j++)
		{
			glBegin(GL_TRIANGLE_STRIP);
			for(int i=0; i<m_nSlices; i+=2)
			{
				glColor4ubv(m_pVertex[nIndex1+i].cColor);
				glVertex3fv(m_pVertex[nIndex1+i].vPos);
				glColor4ubv(m_pVertex[nIndex2+i].cColor);
				glVertex3fv(m_pVertex[nIndex2+i].vPos);
				glColor4ubv(m_pVertex[nIndex1+1+i].cColor);
				glVertex3fv(m_pVertex[nIndex1+1+i].vPos);
				glColor4ubv(m_pVertex[nIndex2+1+i].cColor);
				glVertex3fv(m_pVertex[nIndex2+1+i].vPos);
			}
			glColor4ubv(m_pVertex[nIndex1].cColor);
			glVertex3fv(m_pVertex[nIndex1].vPos);
			glColor4ubv(m_pVertex[nIndex2].cColor);
			glVertex3fv(m_pVertex[nIndex2].vPos);
			glEnd();
			nIndex1 += m_nSlices;
			nIndex2 += m_nSlices;
		}


		glBegin(GL_TRIANGLE_FAN);
		glColor4ubv(m_pVertex[m_nVertices-1].cColor);
		glVertex3fv(m_pVertex[m_nVertices-1].vPos);
		for(i=0; i<m_nSlices; i++)
		{
			glColor4ubv(m_pVertex[m_nVertices-2-i].cColor);
			glVertex3fv(m_pVertex[m_nVertices-2-i].vPos);
		}
		glColor4ubv(m_pVertex[m_nVertices-2].cColor);
		glVertex3fv(m_pVertex[m_nVertices-2].vPos);
		glEnd();
	}
};

class CAppTask : public TSingleton<CAppTask>, public CKernelTask, public IInputEventListener
{
protected:
	CSRTTransform m_srtModel;
	int m_nPolygonMode;

	CVector m_vLight;
	CVector m_vLightDirection;
	
	// Variables that can be tweaked with keypresses
	bool m_bShowTexture;
	int m_nSamples;
	float m_Kr, m_Kr4PI;
	float m_Km, m_Km4PI;
	float m_ESun;
	float m_g;

	float m_fInnerRadius;
	float m_fOuterRadius;
	float m_fScale;
	float m_fWavelength[3];
	float m_fWavelength4[3];
	float m_fRayleighScaleDepth;
	float m_fMieScaleDepth;
	CPixelBuffer m_pbOpticalDepth;

	CSphere m_sphereInner;
	CSphere m_sphereOuter;

public:
	DEFAULT_TASK_CREATOR(CAppTask);

	virtual void OnQuit()
	{
		CKernel::GetPtr()->KillAllTasks();
	}

	virtual void OnKeyDown(int nKey, int nMod)
	{
		switch(nKey)
		{
			case SDLK_ESCAPE:
				OnQuit();
				break;
			case SDLK_p:
				m_nPolygonMode = (m_nPolygonMode == GL_FILL) ? GL_LINE : GL_FILL;
				break;
			case SDLK_F5:
				if(CInputTask::GetPtr()->IsKeyDown(SDLK_LSHIFT) || CInputTask::GetPtr()->IsKeyDown(SDLK_RSHIFT))
					m_Kr = CMath::Max(0.0f, m_Kr - 0.0001f);
				else
					m_Kr += 0.0001f;
				m_Kr4PI = m_Kr*4.0f*PI;
				break;
			case SDLK_F6:
				if(CInputTask::GetPtr()->IsKeyDown(SDLK_LSHIFT) || CInputTask::GetPtr()->IsKeyDown(SDLK_RSHIFT))
					m_Km = CMath::Max(0.0f, m_Km - 0.0001f);
				else
					m_Km += 0.0001f;
				m_Km4PI = m_Km*4.0f*PI;
				break;
			case SDLK_F7:
				if(CInputTask::GetPtr()->IsKeyDown(SDLK_LSHIFT) || CInputTask::GetPtr()->IsKeyDown(SDLK_RSHIFT))
					m_g = CMath::Max(-1.0f, m_g-0.01f);
				else
					m_g = CMath::Min(1.0f, m_g+0.01f);
				break;
			case SDLK_F8:
				if(CInputTask::GetPtr()->IsKeyDown(SDLK_LSHIFT) || CInputTask::GetPtr()->IsKeyDown(SDLK_RSHIFT))
					m_ESun = CMath::Max(0.0f, m_ESun - 0.1f);
				else
					m_ESun += 0.1f;
				break;
			case SDLK_F9:
				if(CInputTask::GetPtr()->IsKeyDown(SDLK_LSHIFT) || CInputTask::GetPtr()->IsKeyDown(SDLK_RSHIFT))
					m_fWavelength[0] = CMath::Max(0.001f, m_fWavelength[0] -= 0.001f);
				else
					m_fWavelength[0] += 0.001f;
				m_fWavelength4[0] = powf(m_fWavelength[0], 4.0f);
				break;
			case SDLK_F10:
				if(CInputTask::GetPtr()->IsKeyDown(SDLK_LSHIFT) || CInputTask::GetPtr()->IsKeyDown(SDLK_RSHIFT))
					m_fWavelength[1] = CMath::Max(0.001f, m_fWavelength[1] -= 0.001f);
				else
					m_fWavelength[1] += 0.001f;
				m_fWavelength4[1] = powf(m_fWavelength[1], 4.0f);
				break;
			case SDLK_F11:
				if(CInputTask::GetPtr()->IsKeyDown(SDLK_LSHIFT) || CInputTask::GetPtr()->IsKeyDown(SDLK_RSHIFT))
					m_fWavelength[2] = CMath::Max(0.001f, m_fWavelength[2] -= 0.001f);
				else
					m_fWavelength[2] += 0.001f;
				m_fWavelength4[2] = powf(m_fWavelength[2], 4.0f);
				break;
			case SDLK_PLUS:
			case SDLK_KP_PLUS:
				m_nSamples = CMath::Min(m_nSamples+1, 25);
				break;
			case SDLK_MINUS:
			case SDLK_KP_MINUS:
				m_nSamples = CMath::Max(m_nSamples-1, 1);
				break;
		}
	}

	virtual bool Start()
	{
		CInputTask::GetPtr()->AddInputEventListener(this);
		CCameraTask::GetPtr()->SetPosition(CVector(0, 0, 25));
		CCameraTask::GetPtr()->SetThrust(1.0f);

		m_nPolygonMode = GL_FILL;
		m_vLight = CVector(1000, 1000, 1000);
		m_vLightDirection = m_vLight / m_vLight.Magnitude();

		m_nSamples = 4;		// Number of sample rays to use in integral equation
		m_Kr = 0.0025f;		// Rayleigh scattering constant
		m_Kr4PI = m_Kr*4.0f*PI;
		m_Km = 0.0015f;		// Mie scattering constant
		m_Km4PI = m_Km*4.0f*PI;
		m_ESun = 15.0f;		// Sun brightness constant
		m_g = -0.85f;		// The Mie phase asymmetry factor

		m_fInnerRadius = 10.0f;
		m_fOuterRadius = 10.25f;
		m_fScale = 1 / (m_fOuterRadius - m_fInnerRadius);

		m_fWavelength[0] = 0.650f;		// 650 nm for red
		m_fWavelength[1] = 0.570f;		// 570 nm for green
		m_fWavelength[2] = 0.475f;		// 475 nm for blue
		m_fWavelength4[0] = powf(m_fWavelength[0], 4.0f);
		m_fWavelength4[1] = powf(m_fWavelength[1], 4.0f);
		m_fWavelength4[2] = powf(m_fWavelength[2], 4.0f);

		m_fRayleighScaleDepth = 0.25f;
		m_fMieScaleDepth = 0.1f;
		m_pbOpticalDepth.MakeOpticalDepthBuffer(m_fInnerRadius, m_fOuterRadius, m_fRayleighScaleDepth, m_fMieScaleDepth);

		m_sphereInner.Init(m_fInnerRadius, 50, 50);
		m_sphereOuter.Init(m_fOuterRadius, 100, 100);
		return true;
	}

	virtual void Update()
	{
		int i;
		PROFILE("CAppTask::Update()", 1);

		// Cheap collision detection/response
		CVector vCamera = CCameraTask::GetPtr()->GetPosition();
		if(vCamera.Magnitude() < m_fInnerRadius + 0.01f)
		{
			CVector N = vCamera / vCamera.Magnitude();
			CVector I = CCameraTask::GetPtr()->GetVelocity();
			float fSpeed = I.Magnitude();
			I /= fSpeed;
			CVector R = N * (2.0*(-I | N)) + I;
			CCameraTask::GetPtr()->SetVelocity(R * fSpeed);

			vCamera = N * (m_fInnerRadius + 0.01f);
			CCameraTask::GetPtr()->SetPosition(vCamera);
		}

		glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
		glPolygonMode(GL_FRONT, m_nPolygonMode);

		// Update the color for the vertices of each sphere
		SVertex *pBuffer = m_sphereInner.GetVertexBuffer();
		for(i=0; i<m_sphereInner.GetVertexCount(); i++)
		{
			if((vCamera | pBuffer[i].vPos) > 0)		// Cheap optimization: Don't update vertices on the back half of the sphere
				SetColor(&pBuffer[i]);
		}
		pBuffer = m_sphereOuter.GetVertexBuffer();
		for(i=0; i<m_sphereOuter.GetVertexCount(); i++)
		{
			if((vCamera | pBuffer[i].vPos) > 0)		// Cheap optimization: Don't update vertices on the back half of the sphere
				SetColor(&pBuffer[i]);
		}

		// Then draw the two spheres
		m_sphereInner.Draw();
		glFrontFace(GL_CW);
		m_sphereOuter.Draw();
		glFrontFace(GL_CCW);
		glPolygonMode(GL_FRONT, GL_FILL);

		glColor4ub(255, 255, 255, 255);
		char szBuffer[256];
		CFont &fFont = CVideoTask::GetPtr()->GetFont();
		fFont.Begin();
		fFont.SetPosition(0, 15);
		sprintf(szBuffer, "Samples (+/-): %d", m_nSamples);
		fFont.Print(szBuffer);
		fFont.SetPosition(0, 30);
		sprintf(szBuffer, "Kr (F5/Sh+F5): %-4.4f", m_Kr);
		fFont.Print(szBuffer);
		fFont.SetPosition(0, 45);
		sprintf(szBuffer, "Km (F6/Sh+F6): %-4.4f", m_Km);
		fFont.Print(szBuffer);
		fFont.SetPosition(0, 60);
		sprintf(szBuffer, "g (F7/Sh+F7): %-2.2f", m_g);
		fFont.Print(szBuffer);
		fFont.SetPosition(0, 75);
		sprintf(szBuffer, "ESun (F8/Sh+F8): %-1.1f", m_ESun);
		fFont.Print(szBuffer);
		fFont.SetPosition(0, 90);
		sprintf(szBuffer, "Red (F9/Sh+F9): %-3.3f", m_fWavelength[0]);
		fFont.Print(szBuffer);
		fFont.SetPosition(0, 105);
		sprintf(szBuffer, "Green (F10/Sh+F10): %-3.3f", m_fWavelength[1]);
		fFont.Print(szBuffer);
		fFont.SetPosition(0, 120);
		sprintf(szBuffer, "Blue (F11/Sh+F11): %-3.3f", m_fWavelength[2]);
		fFont.Print(szBuffer);
		fFont.End();
	}

	virtual void Stop()
	{
		CInputTask::GetPtr()->RemoveInputEventListener(this);
	}

	void SetColor(SVertex *pVertex)
	{
		CVector vPos = pVertex->vPos;

		// Get the ray from the camera to the vertex, and its length (which is the far point of the ray passing through the atmosphere)
		CVector vCamera = CCameraTask::GetPtr()->GetPosition();
		CVector vRay = vPos - vCamera;
		float fFar = vRay.Magnitude();
		vRay /= fFar;

		// Calculate the closest intersection of the ray with the outer atmosphere (which is the near point of the ray passing through the atmosphere)
		float B = 2.0f * (vCamera | vRay);
		float C = (vCamera | vCamera) - m_fOuterRadius*m_fOuterRadius;
		float fDet = CMath::Max(0.0f, B*B - 4.0f * C);
		float fNear = 0.5f * (-B - sqrtf(fDet));

		bool bCameraInAtmosphere = false;
		bool bCameraAbove = true;
		float fCameraDepth[4] = {0, 0, 0, 0};
		float fLightDepth[4];
		float fSampleDepth[4];
		if(fNear <= 0)
		{
			// If the near point is behind the camera, it means the camera is inside the atmosphere
			bCameraInAtmosphere = true;
			fNear = 0;
			float fCameraHeight = vCamera.Magnitude();
			float fCameraAltitude = (fCameraHeight - m_fInnerRadius) * m_fScale;
			bCameraAbove = fCameraHeight >= vPos.Magnitude();
			float fCameraAngle = ((bCameraAbove ? -vRay : vRay) | vCamera) / fCameraHeight;
			m_pbOpticalDepth.Interpolate(fCameraDepth, fCameraAltitude, 0.5f - fCameraAngle * 0.5f);
		}
		else
		{
			// Otherwise, move the camera up to the near intersection point
			vCamera += vRay * fNear;
			fFar -= fNear;
			fNear = 0;
		}

		// If the distance between the points on the ray is negligible, don't bother to calculate anything
		if(fFar <= DELTA)
		{
			glColor4f(0, 0, 0, 1);
			return;
		}

		// Initialize a few variables to use inside the loop
		float fRayleighSum[3] = {0, 0, 0};
		float fMieSum[3] = {0, 0, 0};
		float fSampleLength = fFar / m_nSamples;
		float fScaledLength = fSampleLength * m_fScale;
		CVector vSampleRay = vRay * fSampleLength;

		// Start at the center of the first sample ray, and loop through each of the others
		vPos = vCamera + vSampleRay * 0.5f;
		for(int i=0; i<m_nSamples; i++)
		{
			float fHeight = vPos.Magnitude();

			// Start by looking up the optical depth coming from the light source to this point
			float fLightAngle = (m_vLightDirection | vPos) / fHeight;
			float fAltitude = (fHeight - m_fInnerRadius) * m_fScale;
			m_pbOpticalDepth.Interpolate(fLightDepth, fAltitude, 0.5f - fLightAngle * 0.5f);

			// If no light light reaches this part of the atmosphere, no light is scattered in at this point
			if(fLightDepth[0] < DELTA)
				continue;

			// Get the density at this point, along with the optical depth from the light source to this point
			float fRayleighDensity = fScaledLength * fLightDepth[0];
			float fRayleighDepth = fLightDepth[1];
			float fMieDensity = fScaledLength * fLightDepth[2];
			float fMieDepth = fLightDepth[3];

			// If the camera is above the point we're shading, we calculate the optical depth from the sample point to the camera
			// Otherwise, we calculate the optical depth from the camera to the sample point
			if(bCameraAbove)
			{
				float fSampleAngle = (-vRay | vPos) / fHeight;
				m_pbOpticalDepth.Interpolate(fSampleDepth, fAltitude, 0.5f - fSampleAngle * 0.5f);
				fRayleighDepth += fSampleDepth[1] - fCameraDepth[1];
				fMieDepth += fSampleDepth[3] - fCameraDepth[3];
			}
			else
			{
				float fSampleAngle = (vRay | vPos) / fHeight;
				m_pbOpticalDepth.Interpolate(fSampleDepth, fAltitude, 0.5f - fSampleAngle * 0.5f);
				fRayleighDepth += fCameraDepth[1] - fSampleDepth[1];
				fMieDepth += fCameraDepth[3] - fSampleDepth[3];
			}

			// Now multiply the optical depth by the attenuation factor for the sample ray
			fRayleighDepth *= m_Kr4PI;
			fMieDepth *= m_Km4PI;

			// Calculate the attenuation factor for the sample ray
			float fAttenuation[3];
			fAttenuation[0] = expf(-fRayleighDepth / m_fWavelength4[0] - fMieDepth);
			fAttenuation[1] = expf(-fRayleighDepth / m_fWavelength4[1] - fMieDepth);
			fAttenuation[2] = expf(-fRayleighDepth / m_fWavelength4[2] - fMieDepth);

			fRayleighSum[0] += fRayleighDensity * fAttenuation[0];
			fRayleighSum[1] += fRayleighDensity * fAttenuation[1];
			fRayleighSum[2] += fRayleighDensity * fAttenuation[2];

			fMieSum[0] += fMieDensity * fAttenuation[0];
			fMieSum[1] += fMieDensity * fAttenuation[1];
			fMieSum[2] += fMieDensity * fAttenuation[2];

			// Move the position to the center of the next sample ray
			vPos += vSampleRay;
		}

		// Calculate the angle and phase values (this block of code could be handled by a small 1D lookup table, or a 1D texture lookup in a pixel shader)
		float fAngle = -vRay | m_vLightDirection;
		float fPhase[2];
		float fAngle2 = fAngle*fAngle;
		float g2 = m_g*m_g;
		fPhase[0] = 0.75f * (1.0f + fAngle2);
		fPhase[1] = 1.5f * ((1 - g2) / (2 + g2)) * (1.0f + fAngle2) / powf(1 + g2 - 2*m_g*fAngle, 1.5f);
		fPhase[0] *= m_Kr * m_ESun;
		fPhase[1] *= m_Km * m_ESun;

		// Calculate the in-scattering color and clamp it to the max color value
		float fColor[3] = {0, 0, 0};
		fColor[0] = fRayleighSum[0] * fPhase[0] / m_fWavelength4[0] + fMieSum[0] * fPhase[1];
		fColor[1] = fRayleighSum[1] * fPhase[0] / m_fWavelength4[1] + fMieSum[1] * fPhase[1];
		fColor[2] = fRayleighSum[2] * fPhase[0] / m_fWavelength4[2] + fMieSum[2] * fPhase[1];
		fColor[0] = CMath::Min(fColor[0], 1.0f);
		fColor[1] = CMath::Min(fColor[1], 1.0f);
		fColor[2] = CMath::Min(fColor[2], 1.0f);

		// Last but not least, set the color
		pVertex->cColor = CColor(fColor[0], fColor[1], fColor[2]);
	}
};

int main(int argc, char *argv[])
{
	CLog log;
	log.Init(Debug, "ScatterCPU");
	LogInfo("Starting app (%s)", g_strBuildStamp.c_str());
	CProfiler profile("main", 3, Info);

	CKernel *pKernel = CKernel::Create();
	pKernel->AddTask(CTimerTask::Create(10));
	pKernel->AddTask(CInputTask::Create(20));
	pKernel->AddTask(CInterpolatorTask::Create(30));
	pKernel->AddTask(CTriggerTask::Create(40));
	pKernel->AddTask(CCameraTask::Create(50));
	pKernel->AddTask(CVideoTask::Create(10000));

	pKernel->AddTask(CAppTask::Create(100));
	pKernel->Execute();
	pKernel->Destroy();

	LogInfo("Closing app");
	return 0;
}
