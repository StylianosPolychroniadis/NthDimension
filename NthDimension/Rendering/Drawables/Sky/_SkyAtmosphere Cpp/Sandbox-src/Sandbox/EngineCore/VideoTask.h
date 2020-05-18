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

#ifndef __VideoTask_h__
#define __VideoTask_h__

#include <SDL_syswm.h>


class CVideoTask : public CKernelTask, public TSingleton<CVideoTask>
{
protected:
	CFont m_fFont;
	char m_szFrameCount[20];
	int m_nTime;
	int m_nFrames;

	std::string m_strVendor;
	std::string m_strRenderer;
	std::string m_strVersion;
	std::string m_strExtensions;

	SDL_SysWMinfo m_sdlInfo;
	int m_nWidth, m_nHeight;

public:
	DEFAULT_TASK_CREATOR(CVideoTask);

	bool IsNVIDIA()						{ return memcmp(m_strVendor.c_str(), "NVIDIA", 6) == 0; }
	bool IsATI()						{ return memcmp(m_strVendor.c_str(), "ATI", 3) == 0; }
	bool IsGeForceFX()					{ return memcmp(m_strRenderer.c_str(), "GeForce FX", 10) == 0; }
	bool IsGeForce6()					{ return memcmp(m_strRenderer.c_str(), "GeForce 6", 9) == 0; }
	bool IsRadeon9()					{ return memcmp(m_strRenderer.c_str(), "RADEON 9", 8) == 0; }
	bool HasExtension(const char *psz)	{ return strstr(m_strExtensions.c_str(), psz) != NULL; }
	CFont &GetFont()					{ return m_fFont; }

	int GetWidth()						{ return m_nWidth; }
	int GetHeight()						{ return m_nHeight; }

#ifdef _WIN32
	HWND GetHWnd()						{ return m_sdlInfo.window; }
	HGLRC GetHGLRC()					{ return m_sdlInfo.hglrc; }
#endif

	virtual bool Start()
	{
		m_nWidth = 800;
		m_nHeight = 600;

		if(SDL_InitSubSystem(SDL_INIT_VIDEO) == -1)
		{
			LogCritical(SDL_GetError());
			LogAssert(false);
			return false;
		}
		SDL_GL_SetAttribute(SDL_GL_RED_SIZE, 8);
		SDL_GL_SetAttribute(SDL_GL_GREEN_SIZE, 8);
		SDL_GL_SetAttribute(SDL_GL_BLUE_SIZE, 8);
		SDL_GL_SetAttribute(SDL_GL_ALPHA_SIZE, 8);
		//SDL_GL_SetAttribute(SDL_GL_DEPTH_SIZE, 32);
		SDL_GL_SetAttribute(SDL_GL_DOUBLEBUFFER, 1);

#if 1
		int nFlags = SDL_OPENGL | SDL_ANYFORMAT;
#else
		int nFlags = SDL_OPENGL | SDL_ANYFORMAT | SDL_FULLSCREEN;
#endif

		/*if(!::init("libGL.so.1"))
		{
			LogCritical(SDL_GetError());
			LogAssert(false);
			return false;
		}*/

		if(!SDL_SetVideoMode(m_nWidth, m_nHeight, 32, nFlags))
		{
			LogCritical(SDL_GetError());
			LogAssert(false);
			return false;
		}

		glViewport(0, 0, m_nWidth, m_nHeight);
		glMatrixMode(GL_PROJECTION);
		glLoadIdentity();
		gluPerspective(45.0, (double)m_nWidth / (double)m_nHeight, 0.001, 100.0);
		glMatrixMode(GL_MODELVIEW);
		glLoadIdentity();

		glEnable(GL_DEPTH_TEST);
		glDepthFunc(GL_LEQUAL);
		glEnable(GL_CULL_FACE);

		//hide the mouse cursor
		SDL_ShowCursor(SDL_DISABLE);

		*m_szFrameCount = 0;
		m_nTime = 0;
		m_nFrames = 0;
		m_fFont.Init();

		m_strVendor = (const char *)glGetString(GL_VENDOR);
		m_strRenderer = (const char *)glGetString(GL_RENDERER);
		m_strVersion = (const char *)glGetString(GL_VERSION);
		m_strExtensions = (const char *)glGetString(GL_EXTENSIONS);
		LogInfo("GL_VENDOR = %s", m_strVendor.c_str());
		LogInfo("GL_RENDERER = %s", m_strRenderer.c_str());
		LogInfo("GL_VERSION = %s", m_strVersion.c_str());
		LogInfo("GL_EXTENSIONS = %s", m_strExtensions.c_str());

		GLenum err = glewInit();
		if(err != GLEW_OK)
		{
			LogCritical((const char *)glewGetErrorString(err));
			LogAssert(false);
			return false;
		}

		SDL_VERSION(&m_sdlInfo.version);
		SDL_GetWMInfo(&m_sdlInfo);
		LOG_GLERROR();
		return true;
	}

	virtual void Update()
	{
		PROFILE("CVideoTask::Update()", 1);

		// Determine the FPS
		m_nTime += CTimerTask::GetPtr()->GetFrameTicks();
		if(m_nTime >= 1000)
		{
			float fFPS = (float)(m_nFrames * 1000) / (float)m_nTime;
			sprintf(m_szFrameCount, "%2.2f FPS", fFPS);
			m_nTime = m_nFrames = 0;
		}
		m_nFrames++;

		if(!CInputTask::GetPtr()->IsConsoleActive())
		{
			// Draw the FPS in the top-left corner
			glDisable(GL_LIGHTING);
			glColor4d(1.0, 1.0, 1.0, 1.0);
			m_fFont.Begin();
			m_fFont.SetPosition(0, 0);
			m_fFont.Print(m_szFrameCount);
			m_fFont.End();
		}

		// Swap the back buffer to the front buffer
		SDL_GL_SwapBuffers();
		LOG_GLERROR();
	}

	virtual void Stop()
	{
		m_fFont.Cleanup();
		SDL_QuitSubSystem(SDL_INIT_VIDEO);
	}
};


#endif // __VideoTask_h__
