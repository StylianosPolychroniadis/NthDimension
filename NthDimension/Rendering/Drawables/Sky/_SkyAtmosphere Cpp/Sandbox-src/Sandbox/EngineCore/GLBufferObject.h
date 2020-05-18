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

#ifndef __GLBufferObject_h__
#define __GLBufferObject_h__

struct SPNCVertex
{
	CVector p;
	CVector n;
	unsigned char c[4];
};

struct SBufferShape
{
	short m_nPositionOffset;
	short m_nNormalOffset;
	short m_nColorOffset;
	short m_nTexCoord[4];
	short m_nTexCoordOffset[4];
	short m_nVertexSize;

	void Init(bool bHasPosition, bool bHasNormal, bool bHasColor, int *nTexCoord)
	{
		int nOffset = 0;
		if(bHasPosition)
		{
			m_nPositionOffset = nOffset;
			nOffset += sizeof(CVector);
		}
		else
			m_nPositionOffset = -1;

		if(bHasNormal)
		{
			m_nNormalOffset = nOffset;
			nOffset += sizeof(CVector);
		}
		else
			m_nNormalOffset = -1;

		if(bHasColor)
		{
			m_nColorOffset = nOffset;
			nOffset += 4 * sizeof(unsigned char);
		}
		else
			m_nColorOffset = -1;

		for(int i=0; i<4; i++)
		{
			if(nTexCoord && nTexCoord[i])
			{
				m_nTexCoord[i] = nTexCoord[i];
				m_nTexCoordOffset[i] = nOffset;
				nOffset += m_nTexCoord[i] * sizeof(float);
			}
			else
			{
				m_nTexCoord[i] = 0;
				m_nTexCoordOffset[i] = -1;
			}
		}

		m_nVertexSize = nOffset;
	}
};


class CGLVertexBufferObject
{
protected:
	unsigned int m_nBufferObject;
	void *m_pBuffer;

public:
	CGLVertexBufferObject()
	{
		m_nBufferObject = (unsigned int)-1;
		m_pBuffer = NULL;
	}
	~CGLVertexBufferObject()
	{
		Cleanup();
	}

	void Init(SBufferShape *pObject, int nElements, void *pBuffer=NULL, GLenum nUsage=GL_STATIC_DRAW_ARB)	// Or GL_DYNAMIC_DRAW_ARB
	{
		Cleanup();
		if(!CVideoTask::GetPtr()->HasExtension("GL_ARB_vertex_buffer_object"))
		{
			int nSize = pObject->m_nVertexSize * nElements;
			m_pBuffer = new char[nSize];
			if(pBuffer)
				memcpy(m_pBuffer, pBuffer, nSize);
		}
		else
		{
			glGenBuffersARB(1, &m_nBufferObject);
			glBindBufferARB(GL_ARRAY_BUFFER_ARB, m_nBufferObject);
			glBufferDataARB(GL_ARRAY_BUFFER_ARB, nElements * pObject->m_nVertexSize, pBuffer, nUsage);
		}
		LOG_GLERROR();
	}
	void Cleanup()
	{
		if(m_pBuffer != NULL)
		{
			delete (char *)m_pBuffer;
			m_pBuffer = NULL;
		}
		if(m_nBufferObject != (unsigned int)-1)
		{
			glDeleteBuffersARB(1, &m_nBufferObject);
			m_nBufferObject = (unsigned int)-1;
		}
	}

	void *Lock()
	{
		if(m_pBuffer != NULL)
			return m_pBuffer;
		return glMapBufferARB(GL_ARRAY_BUFFER_ARB, GL_WRITE_ONLY_ARB);
	}

	void Unlock()
	{
		if(m_pBuffer != NULL)
			return;
		glUnmapBufferARB(GL_ARRAY_BUFFER_ARB);
	}

	void Enable(SBufferShape *pObject)
	{
		if(m_pBuffer == NULL)
			glBindBufferARB(GL_ARRAY_BUFFER_ARB, m_nBufferObject);
		if(pObject->m_nPositionOffset != -1)
		{
			glEnableClientState(GL_VERTEX_ARRAY);
			glVertexPointer(3, GL_FLOAT, pObject->m_nVertexSize, (void *)((unsigned long)m_pBuffer + pObject->m_nPositionOffset));
		}
		if(pObject->m_nNormalOffset != -1)
		{
			glEnableClientState(GL_NORMAL_ARRAY);
			glNormalPointer(GL_FLOAT, pObject->m_nVertexSize, (void *)((unsigned long)m_pBuffer + pObject->m_nNormalOffset));
		}
		if(pObject->m_nColorOffset != -1)
		{
			glEnableClientState(GL_COLOR_ARRAY);
			glColorPointer(4, GL_UNSIGNED_BYTE, pObject->m_nVertexSize, (void *)((unsigned long)m_pBuffer + pObject->m_nColorOffset));
		}
		for(int i=3; i>=0; i--)
		{
			if(pObject->m_nTexCoordOffset[i] != -1)
			{
				glClientActiveTexture(GL_TEXTURE0 + i);
				glEnableClientState(GL_TEXTURE_COORD_ARRAY);
				glTexCoordPointer(pObject->m_nTexCoord[i], GL_FLOAT, pObject->m_nVertexSize, (void *)((unsigned long)m_pBuffer + pObject->m_nTexCoordOffset[i]));
			}
		}
		LOG_GLERROR();
	}

	void Disable(SBufferShape *pObject)
	{
		if(pObject->m_nPositionOffset != -1)
			glDisableClientState(GL_VERTEX_ARRAY);
		if(pObject->m_nNormalOffset != -1)
			glDisableClientState(GL_NORMAL_ARRAY);
		if(pObject->m_nColorOffset != -1)
			glDisableClientState(GL_COLOR_ARRAY);
		for(int i=3; i>=0; i--)
		{
			if(pObject->m_nTexCoordOffset[i] != -1)
			{
				glClientActiveTexture(GL_TEXTURE0 + i);
				glDisableClientState(GL_TEXTURE_COORD_ARRAY);
			}
		}
		LOG_GLERROR();
	}
};

class CGLIndexBufferObject
{
protected:
	unsigned int m_nBufferObject;
	int m_nElements;

public:
	CGLIndexBufferObject()
	{
	}
	~CGLIndexBufferObject()
	{
	}

	void Init(int nElements, void *pBuffer=NULL, GLenum nUsage=GL_STATIC_DRAW_ARB)	// Or GL_DYNAMIC_DRAW_ARB
	{
		m_nElements = nElements;
		glGenBuffersARB(1, &m_nBufferObject);
		glBindBufferARB(GL_ELEMENT_ARRAY_BUFFER_ARB, m_nBufferObject);
		glBufferDataARB(GL_ELEMENT_ARRAY_BUFFER_ARB, nElements * sizeof(unsigned short), pBuffer, nUsage);
		LOG_GLERROR();
	}

	void *Lock()
	{
		return glMapBufferARB(GL_ELEMENT_ARRAY_BUFFER_ARB, GL_WRITE_ONLY_ARB);
	}

	void Unlock()
	{
		glUnmapBufferARB(GL_ELEMENT_ARRAY_BUFFER_ARB);
	}

	void Cleanup()
	{
		glDeleteBuffersARB(1, &m_nBufferObject);
	}

	void Draw(GLenum nMode=GL_TRIANGLES)
	{
		glBindBufferARB(GL_ELEMENT_ARRAY_BUFFER_ARB, m_nBufferObject);
		glDrawElements(nMode, m_nElements, GL_UNSIGNED_SHORT, 0);
		LOG_GLERROR();
	}
};

#endif // __GLBufferObject_h__
