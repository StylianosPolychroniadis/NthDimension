// TestApp.cpp : Defines the entry point for the application.
//

#include "StdAfx.h"

DECLARE_CORE_GLOBALS;


class CAppTask : public TSingleton<CAppTask>, public CKernelTask, public IInputEventListener
{
protected:
	CSRTTransform m_srtModel;
	int m_nPolygonMode;
	CGLFrameBufferObject m_fb;
	CGLShaderObject m_shHDR;
	bool m_bHDR;
	bool m_bHDRSquare;
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
			case SDLK_MINUS:
			case SDLK_KP_MINUS:
				if(m_fExposure > 0.1f)
					m_fExposure -= 0.1f;
				break;
			case SDLK_PLUS:
			case SDLK_KP_PLUS:
				if(m_fExposure < 10.0f)
					m_fExposure += 0.1f;
				break;
			case SDLK_p:
				m_nPolygonMode = (m_nPolygonMode == GL_FILL) ? GL_LINE : GL_FILL;
				break;
			case SDLK_h:
				m_bHDR = !m_bHDR;
				break;
		}
	}

	virtual bool Start()
	{
		CInputTask::GetPtr()->AddInputEventListener(this);
		CCameraTask::GetPtr()->SetPosition(CVector(0, 0, 2));
		m_nPolygonMode = GL_FILL;
		m_bHDR = true;
		m_fExposure = 1.0f;

		// ATI doesn't support rectangular textures in GLSL
		m_bHDRSquare = !CVideoTask::GetPtr()->IsNVIDIA();
		if(CVideoTask::GetPtr()->IsNVIDIA())
		{
			m_fb.Init(CVideoTask::GetPtr()->GetWidth(), CVideoTask::GetPtr()->GetHeight(), GL_FLOAT_RGBA16_NV);
			if(m_fb.IsValid())
				m_shHDR.Init("shaders/HDRVert.glsl", "shaders/HDRRectFrag.glsl");
		}
		else if(CVideoTask::GetPtr()->IsATI())
		{
			m_fb.Init(1024, 1024, GL_RGBA_FLOAT16_ATI);
			if(m_fb.IsValid())
				m_shHDR.Init("shaders/HDRVert.glsl", "shaders/HDRSquareFrag.glsl");
		}
		else
		{
			m_fb.Init(1024, 1024, GL_RGBA16F_ARB);
			if(m_fb.IsValid())
				m_shHDR.Init("shaders/HDRVert.glsl", "shaders/HDRSquareFrag.glsl");
		}

		return true;
	}

	virtual void Update()
	{
		PROFILE("CAppTask::Update()", 1);

		if(m_bHDR && m_fb.IsValid())
		{
			m_fb.EnableFrameBuffer();
			if(m_bHDRSquare)
				glViewport(0, 0, 1024, 1024);
		}

		glClearColor(0.25f, 0.25f, 0.25f, 0.25f);
		glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

		glPolygonMode(GL_FRONT, m_nPolygonMode);
		CMatrix4 mModel = m_srtModel.BuildModelMatrix();
		glPushMatrix();
		glMultMatrixf(mModel);
		glBegin(GL_TRIANGLES);
		glColor3f(1.0f,0.0f,0.0f);
		glVertex3f(0.0f, 1.0f, 0.0f);
		glColor3f(0.0f,1.0f,0.0f);
		glVertex3f(-1.0f,-1.0f, 0.0f);
		glColor3f(0.0f,0.0f,1.0f);
		glVertex3f(1.0f,-1.0f, 0.0f);
		glEnd();
		glPopMatrix();
		glPolygonMode(GL_FRONT, GL_FILL);

		if(m_bHDR && m_fb.IsValid())
		{
			m_fb.DisableFrameBuffer();
			if(m_bHDRSquare)
				glViewport(0, 0, CVideoTask::GetPtr()->GetWidth(), CVideoTask::GetPtr()->GetHeight());
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
				if(m_bHDRSquare)
				{
					glTexCoord2f(0, 0); glVertex2i(10, CVideoTask::GetPtr()->GetHeight()-10);
					glTexCoord2f(1, 0); glVertex2i(CVideoTask::GetPtr()->GetWidth()-10, CVideoTask::GetPtr()->GetHeight()-10);
					glTexCoord2f(1, 1); glVertex2i(CVideoTask::GetPtr()->GetWidth()-10, 10);
					glTexCoord2f(0, 1); glVertex2i(10,  10);
				}
				else
				{
					glTexCoord2f(0, 0); glVertex2i(10, CVideoTask::GetPtr()->GetHeight()-10);
					glTexCoord2f(CVideoTask::GetPtr()->GetWidth(), 0); glVertex2i(CVideoTask::GetPtr()->GetWidth()-10, CVideoTask::GetPtr()->GetHeight()-10);
					glTexCoord2f(CVideoTask::GetPtr()->GetWidth(), CVideoTask::GetPtr()->GetHeight()); glVertex2i(CVideoTask::GetPtr()->GetWidth()-10, 10);
					glTexCoord2f(0, CVideoTask::GetPtr()->GetHeight()); glVertex2i(10,  10);
				}
			}
			glEnd();
			m_shHDR.Disable();
			m_fb.DisableTexture();

			char szBuffer[256];
			CVideoTask::GetPtr()->GetFont().SetPosition(0, 15);
			sprintf(szBuffer, "Exposure (+/-): %-2.2f", m_fExposure);
			CVideoTask::GetPtr()->GetFont().Print(szBuffer);
			CVideoTask::GetPtr()->GetFont().End();
		}
	}

	virtual void Stop()
	{
		m_shHDR.Cleanup();
		m_fb.Cleanup();
		CInputTask::GetPtr()->RemoveInputEventListener(this);
	}
};


int main(int argc, char *argv[])
{
	CLog log;
	log.Init(Debug, "TestApp");
	LogInfo("Starting app (%s)", g_strBuildStamp.c_str());
	CProfiler profile("main", 3, Info);

	CKernel *pKernel = CKernel::Create();
	pKernel->AddTask(CTimerTask::Create(10));
	pKernel->AddTask(CInputTask::Create(20));
	pKernel->AddTask(CInterpolatorTask::Create(30));
	pKernel->AddTask(CTriggerTask::Create(40));
	pKernel->AddTask(CCameraTask::Create(50));
	pKernel->AddTask(CConsoleTask::Create(9000));
	pKernel->AddTask(CVideoTask::Create(10000));

	pKernel->AddTask(CAppTask::Create(100));
	pKernel->Execute();
	pKernel->Destroy();

	LogInfo("Closing app");
	return 0;
}
