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

#ifndef __CGShaderObject_h__
#define __CGShaderObject_h__

#include <Cg\cg.h>
#include <Cg\cgGL.h>


class CCGShaderObject
{
protected:
	static CGcontext m_cgContext;
	static CGprofile m_cgVertexProfile, m_cgFragmentProfile;

	CGprogram m_cgVertexProgram;
	CGprogram m_cgFragmentProgram;

public:
	static void HandleCgError()
	{
		LogError("Cg error: %s", cgGetErrorString(cgGetError()));
	}
	static void InitContext()
	{
		cgSetErrorCallback(HandleCgError);
		m_cgContext = cgCreateContext();

		if(cgGLIsProfileSupported(CG_PROFILE_VP30))
		{
			m_cgVertexProfile = CG_PROFILE_VP30;
			LogInfo("Cg vertex profile: vp30");
		}
		else if(cgGLIsProfileSupported(CG_PROFILE_ARBVP1))
		{
			m_cgVertexProfile = CG_PROFILE_ARBVP1;
			LogInfo("Cg vertex profile: arbvp1");
		}
		else if (cgGLIsProfileSupported(CG_PROFILE_VP20))
		{
			m_cgVertexProfile = CG_PROFILE_VP20;
			LogInfo("Cg vertex profile: vp20");
		}
		else
			LogError("No Cg vertex shader profiles are supported on this system");

		if (cgGLIsProfileSupported(CG_PROFILE_FP30))
		{
			m_cgFragmentProfile = CG_PROFILE_FP30;
			LogInfo("Cg fragment profile: fp30");
		}
		else if(cgGLIsProfileSupported(CG_PROFILE_ARBFP1))
		{
			m_cgFragmentProfile = CG_PROFILE_ARBFP1;
			LogInfo("Cg fragment profile: arbfp1");
		}
		else if (cgGLIsProfileSupported(CG_PROFILE_FP20))
		{
			m_cgFragmentProfile = CG_PROFILE_FP20;
			LogInfo("Cg fragment profile: fp20");
		}
		else
			LogError("No Cg fragment shader profiles are supported on this system");
	}

	static void ReleaseContext()
	{
		cgDestroyContext(m_cgContext);
	}

	CCGShaderObject()
	{
		m_cgVertexProgram = NULL;
		m_cgFragmentProgram = NULL;
	}
	CCGShaderObject(const char *pszPath)
	{
		Load(pszPath);
	}
	~CCGShaderObject()
	{
		Unload();
	}

	void CheckError()
	{
		CGerror err = cgGetError();
		if(err != CG_NO_ERROR)
			LogError("CG error: %s", cgGetErrorString(err));
	}

	void Load(const char *pszPath, const char *pszPath2=NULL)
	{
		char szPath[_MAX_PATH];

		sprintf(szPath, "%sCg.vert", pszPath);
		m_cgVertexProgram = cgCreateProgramFromFile(m_cgContext, CG_SOURCE, szPath, m_cgVertexProfile, NULL, NULL);
		if(cgIsProgram(m_cgVertexProgram))
		{
			if(!cgIsProgramCompiled(m_cgVertexProgram)) 
				cgCompileProgram(m_cgVertexProgram);
			LogInfo(cgGetProgramString(m_cgVertexProgram, CG_COMPILED_PROGRAM));
			cgGLEnableProfile(m_cgVertexProfile);
			cgGLLoadProgram(m_cgVertexProgram);
			cgGLDisableProfile(m_cgVertexProfile);
		}

		sprintf(szPath, "%sCg.frag", pszPath2 ? pszPath2 : pszPath);
		m_cgFragmentProgram = cgCreateProgramFromFile(m_cgContext, CG_SOURCE, szPath, m_cgFragmentProfile, NULL, NULL);
		if(cgIsProgram(m_cgFragmentProgram))
		{
			if(!cgIsProgramCompiled(m_cgFragmentProgram)) 
				cgCompileProgram(m_cgFragmentProgram);
			LogInfo(cgGetProgramString(m_cgFragmentProgram, CG_COMPILED_PROGRAM));
			cgGLEnableProfile(m_cgFragmentProfile);
			cgGLLoadProgram(m_cgFragmentProgram);
			cgGLDisableProfile(m_cgFragmentProfile);
		}
	}

	void Unload()
	{
		if(cgIsProgram(m_cgVertexProgram))
		{
			cgDestroyProgram(m_cgVertexProgram);
			m_cgVertexProgram = NULL;
		}
		if(cgIsProgram(m_cgFragmentProgram))
		{
			cgDestroyProgram(m_cgFragmentProgram);
			m_cgFragmentProgram = NULL;
		}
	}

	void Enable()
	{
		if(cgIsProgram(m_cgVertexProgram))
		{
			cgGLEnableProfile(m_cgVertexProfile);
			cgGLBindProgram(m_cgVertexProgram);
			CGparameter pMatrix = cgGetNamedParameter(m_cgVertexProgram, "gl_ModelViewProjectionMatrix");
			cgGLSetStateMatrixParameter(pMatrix, CG_GL_MODELVIEW_PROJECTION_MATRIX, CG_GL_MATRIX_IDENTITY);
		}
		if(cgIsProgram(m_cgFragmentProgram))
		{
			cgGLEnableProfile(m_cgFragmentProfile);
			cgGLBindProgram(m_cgFragmentProgram);
		}
	}

	void Disable()
	{
		if(cgIsProgram(m_cgVertexProgram))
			cgGLDisableProfile(m_cgVertexProfile);
		if(cgIsProgram(m_cgFragmentProgram))
			cgGLDisableProfile(m_cgFragmentProfile);
	}

	void SetTextureParameter(const char *pszParameter, int p1)
	{
		CGparameter pVert = cgGetNamedParameter(m_cgVertexProgram, pszParameter);
		if(pVert)
			cgGLSetTextureParameter(pVert, p1);
		CGparameter pFrag = cgGetNamedParameter(m_cgFragmentProgram, pszParameter);
		if(pFrag)
			cgGLSetTextureParameter(pFrag, p1);
	}
	void SetUniformParameter1i(const char *pszParameter, int n1)
	{
	}
	void SetUniformParameter1f(const char *pszParameter, float p1)
	{
		CGparameter pVert = cgGetNamedParameter(m_cgVertexProgram, pszParameter);
		if(pVert)
			cgGLSetParameter1f(pVert, p1);
		CGparameter pFrag = cgGetNamedParameter(m_cgFragmentProgram, pszParameter);
		if(pFrag)
			cgGLSetParameter1f(pFrag, p1);
	}
	void SetUniformParameter3f(const char *pszParameter, float p1, float p2, float p3)
	{
		CGparameter pVert = cgGetNamedParameter(m_cgVertexProgram, pszParameter);
		if(pVert)
			cgGLSetParameter3f(pVert, p1, p2, p3);
		CGparameter pFrag = cgGetNamedParameter(m_cgFragmentProgram, pszParameter);
		if(pFrag)
			cgGLSetParameter3f(pFrag, p1, p2, p3);
	}
};


#endif // __CGShaderObject_h__

