// SpriteTest.cpp : Defines the entry point for the application.
//

#include "StdAfx.h"

DECLARE_CORE_GLOBALS;


class CAppTask : public TSingleton<CAppTask>, public CKernelTask, public IInputEventListener
{
protected:
#define XORDER 96
#define YORDER 96
#define ZORDER 16
	SBufferShape m_shape;
	CGLVertexBufferObject m_buffer;
	CGLShaderObject m_shader;
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
		CCameraTask::GetPtr()->SetPosition(CVector(0.0f, 0.0f, 20.0f));

		m_nPolygonMode = GL_FILL;

		m_shape.Init(true, true, true, NULL);
		SPNCVertex *vertex = new SPNCVertex[XORDER*YORDER*ZORDER*4];
		SPNCVertex *ptr = vertex;
		for(int x=0; x<XORDER; x++)
		{
			for(int y=0; y<YORDER; y++)
			{
				for(int z=0; z<ZORDER; z++)
				{
					ptr[0].p = CVector(x, y, z);
					ptr[1].p = ptr[0].p;
					ptr[2].p = ptr[0].p;
					ptr[3].p = ptr[0].p;
					ptr[0].n = CVector(-0.5f, 0.5f, 0.0f);
					ptr[1].n = CVector(-0.5f, -0.5f, 0.0f);
					ptr[2].n = CVector(0.5f, -0.5f, 0.0f);
					ptr[3].n = CVector(0.5f, 0.5f, 0.0f);
					ptr[0].c[0] = 255; ptr[0].c[1] = 255; ptr[0].c[2] = 255; ptr[0].c[3] = 255;
					ptr[1].c[0] = 255; ptr[1].c[1] = 255; ptr[1].c[2] = 255; ptr[1].c[3] = 255;
					ptr[2].c[0] = 255; ptr[2].c[1] = 255; ptr[2].c[2] = 255; ptr[2].c[3] = 255;
					ptr[3].c[0] = 255; ptr[3].c[1] = 255; ptr[3].c[2] = 255; ptr[3].c[3] = 255;
					ptr += 4;
				}
			}
		}
		m_buffer.Init(&m_shape, XORDER*YORDER*ZORDER*4, vertex);
		delete vertex;

		m_shader.Init("shaders/BillboardVert.glsl", "shaders/BillboardFrag.glsl");

		CPixelBuffer pb;
		pb.Init(256, 256, 1, 2, GL_LUMINANCE_ALPHA);
		pb.MakeGlow2(4.0f, 0.0f);
		m_tex.Init(&pb);
		return true;
	}

	virtual void Update()
	{
		PROFILE("CAppTask::Update()", 1);
		glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

		glPolygonMode(GL_FRONT, m_nPolygonMode);
		if(m_nPolygonMode == GL_FILL)
		{
			//glEnable(GL_BLEND);
			//glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
			static int nTest = 0;
			nTest = (nTest+1) % 256;
			glEnable(GL_ALPHA_TEST);
			glAlphaFunc(GL_GREATER, nTest / 255.0);
		}
		CVector vPos = CCameraTask::GetPtr()->GetCamera().GetPosition();
		CVector vView = CCameraTask::GetPtr()->GetCamera().GetViewAxis();
		CVector vUp = CCameraTask::GetPtr()->GetCamera().GetUpAxis();
		m_shader.Enable();
		m_shader.SetUniformParameter3f("v3CameraPos", vPos.x, vPos.y, vPos.z);
		m_shader.SetUniformParameter3f("v3CameraView", vView.x, vView.y, vView.z);
		m_shader.SetUniformParameter3f("v3CameraUp", vUp.x, vUp.y, vUp.z);
		m_shader.SetUniformParameter1f("fScale", 3.0f);
		m_shader.SetUniformParameter1i("s2Tex1", 0);
		m_tex.Enable();

		m_buffer.Enable(&m_shape);

		// Draw buffer without indices
		glDrawArrays(GL_QUADS, 0, XORDER*YORDER*ZORDER*4);

		// Draw buffer with indices
		//int index[4] = {0, 1, 2, 3};
		//glDrawElements(GL_QUADS, 4, GL_UNSIGNED_SHORT, index);

		m_buffer.Disable(&m_shape);

		m_tex.Disable();
		m_shader.Disable();
		glDisable(GL_ALPHA_TEST);
		glPolygonMode(GL_FRONT, GL_FILL);
	}

	virtual void Stop()
	{
		m_buffer.Cleanup();
		m_shader.Cleanup();
		CInputTask::GetPtr()->RemoveInputEventListener(this);
	}
};


int main(int argc, char *argv[])
{
	CLog log;
	log.Init(Debug, "SpriteTest");
	LogInfo("Starting app (%s)", g_strBuildStamp.c_str());
	CProfiler profile("main", 3, Info);

	CKernel *pKernel = CKernel::Create();
	pKernel->AddTask(CTimerTask::Create(10));
	pKernel->AddTask(CInputTask::Create(20));
	pKernel->AddTask(CInterpolatorTask::Create(30));
	pKernel->AddTask(CTriggerTask::Create(40));
	pKernel->AddTask(CCameraTask::Create(50));
	pKernel->AddTask(CVideoTask::Create(10000));

	// The video task must start before any tasks that may use OpenGL in their Start() methods
	// (Even though the video task's Update() method should be called afterward)
	pKernel->AddTask(CAppTask::Create(100));
	pKernel->Execute();
	pKernel->Destroy();

	LogInfo("Closing app");
	return 0;
}
