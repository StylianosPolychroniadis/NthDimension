// ScatterGPU.cpp : Defines the entry point for the application.
//

#include "StdAfx.h"

DECLARE_CORE_GLOBALS;


class CAppTask : public TSingleton<CAppTask>, public CKernelTask, public IInputEventListener
{
protected:
	CSRTTransform m_srtModel;
	int m_nPolygonMode;
	bool m_bHDR;

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
	CTexture m_tOpticalDepth;

	CGLShaderObject m_shSkyFromSpace;
	CGLShaderObject m_shSkyFromAtmosphere;
	CGLShaderObject m_shGroundFromSpace;
	CGLShaderObject m_shGroundFromAtmosphere;

	// HDR variables
	CGLFrameBufferObject m_fb;
	CGLShaderObject m_shHDR;
	float m_fExposure;

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
			case SDLK_h:
				m_bHDR = !m_bHDR;
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
				if(m_bHDR)
				{
					if(m_fExposure < 10.0f)
						m_fExposure += 0.1f;
				}
				else
					m_nSamples = CMath::Min(m_nSamples+1, 25);
				break;
			case SDLK_MINUS:
			case SDLK_KP_MINUS:
				if(m_bHDR)
				{
					if(m_fExposure > 0.1f)
						m_fExposure -= 0.1f;
				}
				else
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

		m_nSamples = 2;		// Number of sample rays to use in integral equation
		m_Kr = 0.0025f;		// Rayleigh scattering constant
		m_Kr4PI = m_Kr*4.0f*PI;
		m_Km = 0.0015f;		// Mie scattering constant
		m_Km4PI = m_Km*4.0f*PI;
		m_ESun = 15.0f;		// Sun brightness constant
		m_g = -0.95f;		// The Mie phase asymmetry factor

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
		m_tOpticalDepth.Init(&m_pbOpticalDepth);

		m_shSkyFromSpace.Init("shaders/SkyFromSpaceVert.glsl", "shaders/SkyFromSpaceFrag.glsl");
		m_shSkyFromAtmosphere.Init("shaders/SkyFromAtmosphereVert.glsl", "shaders/SkyFromAtmosphereFrag.glsl");
		m_shGroundFromSpace.Init("shaders/GroundFromSpaceVert.glsl", "shaders/GroundFromSpaceFrag.glsl");
		m_shGroundFromAtmosphere.Init("shaders/GroundFromAtmosphereVert.glsl", "shaders/GroundFromAtmosphereFrag.glsl");

		m_fExposure = 2.0f;
		m_fb.Init(800, 600, GL_RGBA16F_ARB);
		//m_fb.Init(800, 600, GL_RGBA8);
		m_shHDR.Init("shaders/HDRVert.glsl", "shaders/HDRFrag.glsl");
		m_bHDR = true;
		return true;
	}

	virtual void Update()
	{
		PROFILE("CAppTask::Update()", 1);
		if(m_bHDR && m_fb.IsValid())
			m_fb.EnableFrameBuffer();

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
		CVector vUnitCamera = vCamera / vCamera.Magnitude();

		glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
		glPolygonMode(GL_FRONT, m_nPolygonMode);

		// Draw the groud sphere
		CGLShaderObject *pGroundShader;
		if(vCamera.Magnitude() >= m_fOuterRadius)
			pGroundShader = &m_shGroundFromSpace;
		else
			pGroundShader = &m_shGroundFromAtmosphere;
		pGroundShader->Enable();
		pGroundShader->SetUniformParameter3f("v3CameraPos", vCamera.x, vCamera.y, vCamera.z);
		pGroundShader->SetUniformParameter3f("v3LightPos", m_vLightDirection.x, m_vLightDirection.y, m_vLightDirection.z);
		pGroundShader->SetUniformParameter3f("v3InvWavelength", 1/m_fWavelength4[0], 1/m_fWavelength4[1], 1/m_fWavelength4[2]);
		pGroundShader->SetUniformParameter1f("fCameraHeight", vCamera.Magnitude());
		pGroundShader->SetUniformParameter1f("fCameraHeight2", vCamera.MagnitudeSquared());
		pGroundShader->SetUniformParameter1f("fInnerRadius", m_fInnerRadius);
		pGroundShader->SetUniformParameter1f("fInnerRadius2", m_fInnerRadius*m_fInnerRadius);
		pGroundShader->SetUniformParameter1f("fOuterRadius", m_fOuterRadius);
		pGroundShader->SetUniformParameter1f("fOuterRadius2", m_fOuterRadius*m_fOuterRadius);
		pGroundShader->SetUniformParameter1f("fKrESun", m_Kr*m_ESun);
		pGroundShader->SetUniformParameter1f("fKmESun", m_Km*m_ESun);
		pGroundShader->SetUniformParameter1f("fKr4PI", m_Kr4PI);
		pGroundShader->SetUniformParameter1f("fKm4PI", m_Km4PI);
		pGroundShader->SetUniformParameter1f("fScale", 1.0f / (m_fOuterRadius - m_fInnerRadius));
		pGroundShader->SetUniformParameter1f("fScaleDepth", m_fRayleighScaleDepth);
		pGroundShader->SetUniformParameter1f("fScaleOverScaleDepth", (1.0f / (m_fOuterRadius - m_fInnerRadius)) / m_fRayleighScaleDepth);
		pGroundShader->SetUniformParameter1f("g", m_g);
		pGroundShader->SetUniformParameter1f("g2", m_g*m_g);
		pGroundShader->SetUniformParameter1i("nSamples", m_nSamples);
		pGroundShader->SetUniformParameter1f("fSamples", m_nSamples);
		pGroundShader->SetUniformParameter1i("s2Test", 0);
		GLUquadricObj *pSphere = gluNewQuadric();
		//m_tEarth.Enable();
		gluSphere(pSphere, m_fInnerRadius, 100, 50);
		//m_tEarth.Disable();
		gluDeleteQuadric(pSphere);
		pGroundShader->Disable();

		// Draw the sky sphere
		CGLShaderObject *pSkyShader;
		if(vCamera.Magnitude() >= m_fOuterRadius)
			pSkyShader = &m_shSkyFromSpace;
		else
			pSkyShader = &m_shSkyFromAtmosphere;
		pSkyShader->Enable();
		pSkyShader->SetUniformParameter3f("v3CameraPos", vCamera.x, vCamera.y, vCamera.z);
		pSkyShader->SetUniformParameter3f("v3LightPos", m_vLightDirection.x, m_vLightDirection.y, m_vLightDirection.z);
		pSkyShader->SetUniformParameter3f("v3InvWavelength", 1/m_fWavelength4[0], 1/m_fWavelength4[1], 1/m_fWavelength4[2]);
		pSkyShader->SetUniformParameter1f("fCameraHeight", vCamera.Magnitude());
		pSkyShader->SetUniformParameter1f("fCameraHeight2", vCamera.MagnitudeSquared());
		pSkyShader->SetUniformParameter1f("fInnerRadius", m_fInnerRadius);
		pSkyShader->SetUniformParameter1f("fInnerRadius2", m_fInnerRadius*m_fInnerRadius);
		pSkyShader->SetUniformParameter1f("fOuterRadius", m_fOuterRadius);
		pSkyShader->SetUniformParameter1f("fOuterRadius2", m_fOuterRadius*m_fOuterRadius);
		pSkyShader->SetUniformParameter1f("fKrESun", m_Kr*m_ESun);
		pSkyShader->SetUniformParameter1f("fKmESun", m_Km*m_ESun);
		pSkyShader->SetUniformParameter1f("fKr4PI", m_Kr4PI);
		pSkyShader->SetUniformParameter1f("fKm4PI", m_Km4PI);
		pSkyShader->SetUniformParameter1f("fScale", 1.0f / (m_fOuterRadius - m_fInnerRadius));
		pSkyShader->SetUniformParameter1f("fScaleDepth", m_fRayleighScaleDepth);
		pSkyShader->SetUniformParameter1f("fScaleOverScaleDepth", (1.0f / (m_fOuterRadius - m_fInnerRadius)) / m_fRayleighScaleDepth);
		pSkyShader->SetUniformParameter1f("g", m_g);
		pSkyShader->SetUniformParameter1f("g2", m_g*m_g);
		pSkyShader->SetUniformParameter1i("nSamples", m_nSamples);
		pSkyShader->SetUniformParameter1f("fSamples", m_nSamples);
		m_tOpticalDepth.Enable();
		pSkyShader->SetUniformParameter1f("tex", 0);
		glFrontFace(GL_CW);
		glEnable(GL_BLEND);
		glBlendFunc(GL_ONE, GL_ONE);
		pSphere = gluNewQuadric();
		gluSphere(pSphere, m_fOuterRadius, 100, 100);
		gluDeleteQuadric(pSphere);
		glDisable(GL_BLEND);
		glFrontFace(GL_CCW);
		m_tOpticalDepth.Disable();
		pSkyShader->Disable();

		glPolygonMode(GL_FRONT, GL_FILL);

		if(m_bHDR && m_fb.IsValid())
		{
			m_fb.DisableFrameBuffer();
			CVideoTask::GetPtr()->GetFont().Begin();
			m_fb.EnableTexture();
			m_shHDR.Enable();
			m_shHDR.SetUniformParameter1f("fExposure", m_fExposure);
			m_shHDR.SetUniformParameter1i("s2Test", 0);
			glClearColor(0.0f, 0.0f, 0.0f, 0.0f);
			glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
			glColor4f(1.0f, 1.0f, 1.0f, 1.0f);
			glBegin(GL_QUADS);
			{
				glTexCoord2f(0, 0);     glVertex2i(0, 600);
				glTexCoord2f(800, 0);   glVertex2i(800, 600);
				glTexCoord2f(800, 600); glVertex2i(800, 0);
				glTexCoord2f(0, 600);   glVertex2i(0,  0);
			}
			glEnd();
			m_shHDR.Disable();
			m_fb.DisableTexture();
			CVideoTask::GetPtr()->GetFont().End();
		}

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
		m_shSkyFromSpace.Cleanup();
		m_shSkyFromAtmosphere.Cleanup();
		m_shGroundFromSpace.Cleanup();
		m_shGroundFromAtmosphere.Cleanup();
		CInputTask::GetPtr()->RemoveInputEventListener(this);
	}
};

int main(int argc, char *argv[])
{
	CLog log;
	log.Init(Debug, "ScatterGPU");
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
