// GLCloud2.cpp : Defines the entry point for the application.
//

#include "StdAfx.h"
#include "CloudSphere.h"

DECLARE_CORE_GLOBALS;


class CAppTask : public TSingleton<CAppTask>, public CKernelTask, public IInputEventListener
{
protected:
	CSRTTransform m_srtModel;
	CCloudSphere m_cSphere, m_cSphere2;
	CTexture m_tex;
	CGLShaderObject m_shader;
	int m_nPolygonMode;

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
		}
	}

	virtual bool Start()
	{
		CInputTask::GetPtr()->AddInputEventListener(this);
		CCameraTask::GetPtr()->SetPosition(CVector(0, 0, 2));

		m_nPolygonMode = GL_FILL;

		m_cSphere.SetPosition(CVector(-15, 0, -50));
		m_cSphere.SetBoundingRadius(15.0f);
		m_cSphere2.SetPosition(CVector(10, 0, -50));
		m_cSphere2.SetBoundingRadius(20.0f);

		CPixelBuffer pb;
		pb.Init(128, 128, 128, 2, GL_LUMINANCE_ALPHA);
		pb.Make3DNoise(238653);
		m_tex.Init(&pb, false, false);
		m_shader.Init("shaders\\SwellVert.glsl", "shaders\\SwellFrag.glsl");
		return true;
	}

	virtual void Update()
	{
		PROFILE("CAppTask::Update()", 1);
		glClearColor(0.253f, 0.47f, 0.683f, 1);
		glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

		glPolygonMode(GL_FRONT, m_nPolygonMode);
		if(m_nPolygonMode == GL_FILL)
		{
			glDisable(GL_DEPTH_TEST);
			m_tex.Enable();
			m_shader.Enable();
			m_shader.SetUniformParameter1i("s3Tex0", 0);
		}

		CSRTTransform camera = CCameraTask::GetPtr()->GetCamera();
		float fDistance[2];
		fDistance[0] = camera.GetPosition().Distance(m_cSphere.GetPosition());
		fDistance[1] = camera.GetPosition().Distance(m_cSphere2.GetPosition());
		if(fDistance[0] > fDistance[1])
		{
			m_cSphere.Draw(camera, m_tex);
			m_cSphere2.Draw(camera, m_tex);
		}
		else
		{
			m_cSphere2.Draw(camera, m_tex);
			m_cSphere.Draw(camera, m_tex);
		}
		glEnable(GL_DEPTH_TEST);
		m_shader.Disable();
		m_tex.Disable();
		glPolygonMode(GL_FRONT, GL_FILL);
	}

	virtual void Stop()
	{
		CInputTask::GetPtr()->RemoveInputEventListener(this);
	}
};

int main(int argc, char *argv[])
{
	CLog log;
	log.Init(Debug, "GLCloud2");
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
