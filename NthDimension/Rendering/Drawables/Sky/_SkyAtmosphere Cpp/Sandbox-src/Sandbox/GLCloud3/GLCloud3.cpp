// GLCloud3.cpp : Defines the entry point for the application.
//

#include "StdAfx.h"
#include "ProjectedTetrahedra.h"

DECLARE_CORE_GLOBALS;


class CAppTask : public TSingleton<CAppTask>, public CKernelTask, public IInputEventListener
{
protected:
	CSRTTransform m_srtModel;
	CTexture m_tex;
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
		CCameraTask::GetPtr()->SetPosition(CVector(5, 5, 20));

		m_nPolygonMode = GL_FILL;

		CPixelBuffer pb;
		pb.Init(64, 1, 1, 2, GL_LUMINANCE_ALPHA);
		pb.MakeGlow1D();
		m_tex.Init(&pb);
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
			glEnable(GL_BLEND);
			glDepthFunc(GL_ALWAYS);
			glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
			glColor4f(1, 1, 1, 1);
			m_tex.Enable();
		}
		
		CSRTTransform camera = CCameraTask::GetPtr()->GetCamera();
		int i, j;
#define LEVEL	10
		const int nCubes = LEVEL*LEVEL*LEVEL;
		CProjectedCube cube[nCubes];
		float fDistance[nCubes];
		for(i=0; i<nCubes; i++)
		{
			cube[i].SetPosition(CVector(i/(LEVEL*LEVEL)*2, ((i/LEVEL)%LEVEL)*2, (i%LEVEL)*2));
			fDistance[i] = cube[i].GetPosition().Distance(camera.GetPosition());
		}
		int nOrder[nCubes];
		for(i=0; i<nCubes; i++)
			nOrder[i] = i;
		for(i=0; i<nCubes; i++)
		{
			for(j=0; j<nCubes-i-1; j++)
			{
				if(fDistance[nOrder[j]] > fDistance[nOrder[j+1]])
				{
					int nTemp = nOrder[j];
					nOrder[j] = nOrder[j+1];
					nOrder[j+1] = nTemp;
				}
			}
		}
		for(i=0; i<nCubes; i++)
			cube[nOrder[nCubes-1-i]].DrawVolume(camera);
		
		m_tex.Disable();
		glDisable(GL_BLEND);
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
	log.Init(Debug, "GLCloud3");
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
