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

#ifndef __GLShaderObject_h__
#define __GLShaderObject_h__

inline void LogGLInfoLog(GLhandleARB hObj)
{
	int nBytes;
	glGetObjectParameterivARB(hObj, GL_OBJECT_INFO_LOG_LENGTH_ARB, &nBytes);
	if(nBytes)
	{
		char *pInfo = new char[nBytes];
		glGetInfoLogARB(hObj, nBytes, &nBytes, pInfo);
		LogInfo(pInfo);
		delete pInfo;
	}
}

class CGLShaderObject
{
protected:
	GLhandleARB m_hProgram;
	GLhandleARB m_hVertexShader;
	GLhandleARB m_hFragmentShader;
	std::map<std::string, GLint> m_mapParameters;

public:
	CGLShaderObject()
	{
		m_hProgram = NULL;
		m_hVertexShader = NULL;
		m_hFragmentShader = NULL;
	}
	~CGLShaderObject()
	{
		Cleanup();
	}

	bool IsValid()	{ return m_hProgram != NULL; }

	void Cleanup()
	{
		if(m_hFragmentShader)
		{
			glDeleteObjectARB(m_hFragmentShader);
			m_hFragmentShader = NULL;
		}
		if(m_hVertexShader)
		{
			glDeleteObjectARB(m_hVertexShader);
			m_hVertexShader = NULL;
		}
		if(m_hProgram)
		{
			glDeleteObjectARB(m_hProgram);
			m_hProgram = NULL;
		}
		m_mapParameters.clear();
	}

	bool Init(const char *pszVertexShader, const char *pszFragmentShader)
	{
		Cleanup();
		if(!CVideoTask::GetPtr()->HasExtension("GL_ARB_shader_objects"))
		{
			LogWarning("Shader objects not supported, unable to load %s", pszVertexShader);
			return false;
		}

		m_hProgram = glCreateProgramObjectARB();
		m_hVertexShader = glCreateShaderObjectARB(GL_VERTEX_SHADER_ARB);
		m_hFragmentShader = glCreateShaderObjectARB(GL_FRAGMENT_SHADER_ARB);

		char *psz;
		int nBytes, bSuccess;

		LogInfo("Compiling GLSL vertex shader %s", pszVertexShader);
		std::ifstream ifVertexShader(pszVertexShader, std::ios::binary);
		if(!ifVertexShader)
		{
			LogError("Failed to open %s", pszVertexShader);
			return false;
		}
		ifVertexShader.seekg(0, std::ios::end);
		nBytes = ifVertexShader.tellg();
		ifVertexShader.seekg(0, std::ios::beg);
		psz = new char[nBytes+1];
		ifVertexShader.read(psz, nBytes);
		psz[nBytes] = 0;
		ifVertexShader.close();
		glShaderSourceARB(m_hVertexShader, 1, (const char **)&psz, &nBytes);
		glCompileShaderARB(m_hVertexShader);
		glGetObjectParameterivARB(m_hVertexShader, GL_OBJECT_COMPILE_STATUS_ARB, &bSuccess);
		delete psz;
		if(!bSuccess)
		{
			LogError("Failed to compile vertex shader %s", pszVertexShader);
			LOG_GLERROR();
			LogGLInfoLog(m_hVertexShader);
			return false;
		}

		LogInfo("Compiling GLSL fragment shader %s", pszFragmentShader);
		std::ifstream ifFragmentShader(pszFragmentShader, std::ios::binary);
		if(!ifFragmentShader)
		{
			LogError("Failed to open %s", pszFragmentShader);
			return false;
		}
		ifFragmentShader.seekg(0, std::ios::end);
		nBytes = ifFragmentShader.tellg();
		ifFragmentShader.seekg(0, std::ios::beg);
		psz = new char[nBytes];
		ifFragmentShader.read(psz, nBytes);
		ifFragmentShader.close();
		glShaderSourceARB(m_hFragmentShader, 1, (const char **)&psz, &nBytes);
		glCompileShaderARB(m_hFragmentShader);
		glGetObjectParameterivARB(m_hFragmentShader, GL_OBJECT_COMPILE_STATUS_ARB, &bSuccess);
		delete psz;
		if(!bSuccess)
		{
			LogError("Failed to compile fragment shader %s", pszFragmentShader);
			LOG_GLERROR();
			LogGLInfoLog(m_hFragmentShader);
			return false;
		}

		glAttachObjectARB(m_hProgram, m_hVertexShader);
		glAttachObjectARB(m_hProgram, m_hFragmentShader);
		glLinkProgramARB(m_hProgram);

		glGetObjectParameterivARB(m_hProgram, GL_OBJECT_LINK_STATUS_ARB, &bSuccess);
		if(!bSuccess)
		{
			LogError("Failed to link shaders %s and %s", pszVertexShader, pszFragmentShader);
			LOG_GLERROR();
			LogGLInfoLog(m_hProgram);
			return false;
		}

		LogGLInfoLog(m_hProgram);
		return true;
	}

	void Enable()
	{
		glUseProgramObjectARB(m_hProgram);
	}
	void Disable()
	{
		glUseProgramObjectARB(NULL);
	}

	GLint GetUniformParameterID(const char *pszParameter)
	{
		std::map<std::string, GLint>::iterator it = m_mapParameters.find(pszParameter);
		if(it == m_mapParameters.end())
		{
			GLint nLoc = glGetUniformLocationARB(m_hProgram, pszParameter);
			it = m_mapParameters.insert(std::pair<std::string, GLint>(pszParameter, nLoc)).first;
		}
		return it->second;
	}

	/*
	void BindTexture(const char *pszParameter, unsigned int nID)
	{
		GLint n = GetUniformParameterID(pszParameter);
		glBindTexture(GL_TEXTURE_2D, nID);
		glUniform1iARB(n, nID);
	}
	*/
	void SetUniformParameter1i(const char *pszParameter, int n1)
	{
		glUniform1iARB(GetUniformParameterID(pszParameter), n1);
	}
	void SetUniformParameter1f(const char *pszParameter, float p1)
	{
		glUniform1fARB(GetUniformParameterID(pszParameter), p1);
	}
	void SetUniformParameter3f(const char *pszParameter, float p1, float p2, float p3)
	{
		glUniform3fARB(GetUniformParameterID(pszParameter), p1, p2, p3);
	}
};

#endif // __GLShaderObject_h__

