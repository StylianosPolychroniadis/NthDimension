// GLCloud1.cpp : Defines the entry point for the application.
//

#include "StdAfx.h"
#include "CloudBlock.h"

DECLARE_CORE_GLOBALS;


class CAppTask : public TSingleton<CAppTask>, public CKernelTask, public IInputEventListener
{
protected:
	int m_nSeed;
	CCloudBlock m_cBlock;
	CVector m_vLight;
	CTexture m_tCloudCell;
	bool m_bLightRotating;
	bool m_bCloudsRotating;
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
			case SDLK_m:
				m_bLightRotating = !m_bLightRotating;
				break;
			case SDLK_r:
				m_bCloudsRotating = !m_bCloudsRotating;
				break;
			case SDLK_u:
				m_cBlock.NoiseFill(m_nSeed++); m_cBlock.Light(m_vLight);
				break;
		}
	}

	virtual bool Start()
	{
		CInputTask::GetPtr()->AddInputEventListener(this);
		CCameraTask::GetPtr()->SetPosition(CVector(0, 0, 65));

		m_nSeed = 0;
		m_bLightRotating = false;
		m_bCloudsRotating = false;
		m_nPolygonMode = GL_FILL;
		m_vLight = CVector(0, -1, 0);
		m_cBlock.Init(96, 96, 16);
		m_cBlock.NoiseFill(m_nSeed++);
		m_cBlock.Rotate(CQuaternion(CVector(1, 0, 0), CMath::ToRadians(90)));
		m_cBlock.Light(m_vLight);

		CPixelBuffer pb;
		pb.Init(32, 32, 1, 2, GL_LUMINANCE_ALPHA);
		pb.MakeGlow2(2.0f, 0.0f);
		m_tCloudCell.Init(&pb);
		return true;
	}

	virtual void Update()
	{
		PROFILE("CAppTask::Update()", 1);

		int nMilliseconds = CTimerTask::GetPtr()->GetFrameTicks();
		if(m_bCloudsRotating)
			m_cBlock.Rotate(CQuaternion(CVector(1, 0, 0), 0.2f * nMilliseconds * 0.001f));
		if(m_bLightRotating)
			m_vLight = CQuaternion(CVector(0, 0, 1), 0.2f * nMilliseconds * 0.001f).TransformVector(m_vLight);

		glPolygonMode(GL_FRONT, m_nPolygonMode);
		glClearColor(0.253f, 0.47f, 0.683f, 1);
		glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
		m_cBlock.Light(m_vLight);
		if(m_nPolygonMode == GL_FILL)
		{
			glDisable(GL_DEPTH_TEST);
			m_tCloudCell.Enable();
		}
		m_cBlock.Draw(CCameraTask::GetPtr()->GetCamera(), 1.5f);
		m_tCloudCell.Disable();
		glEnable(GL_DEPTH_TEST);
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
	log.Init(Debug, "GLCloud1");
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
