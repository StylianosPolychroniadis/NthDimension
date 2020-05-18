// ConsoleTask.h
//

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

#ifndef __ConsoleTask_h__
#define __ConsoleTask_h__

#define CONSOLE_ROW_HEIGHT	15
#define CONSOLE_ROW_HISTORY	15
#define CONSOLE_PROMPT		"> "


class CConsoleTask : public CKernelTask, public IInputEventListener, public TSingleton<CConsoleTask>
{
protected:
	std::list<std::string> m_listRows;
	char m_szCommand[256];
	int m_nCommandLength;

	void AddRow(std::string str)
	{
		m_listRows.push_back(str);
		while(m_listRows.size() > CONSOLE_ROW_HISTORY)
			m_listRows.pop_front();
	}

public:
	DEFAULT_TASK_CREATOR(CConsoleTask);

	virtual bool Start()
	{
		CInputTask::GetPtr()->SetConsole(this);
		m_nCommandLength = 0;
		return true;
	}

	virtual void Update()
	{
		PROFILE("CConsoleTask::Update()", 1);
		if(!CInputTask::GetPtr()->IsConsoleActive())
			return;

		glDisable(GL_LIGHTING);
		glEnable(GL_BLEND);
		glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);

		CFont &font = CVideoTask::GetPtr()->GetFont();
		font.Begin();
		glColor4d(0.25f, 0.25f, 0.25f, 0.5f);
		glBegin(GL_QUADS);
		glVertex2f(0, 0);
		glVertex2f(0, CONSOLE_ROW_HEIGHT*(m_listRows.size() + 1));
		glVertex2f(800, CONSOLE_ROW_HEIGHT*(m_listRows.size() + 1));
		glVertex2f(800, 0);
		glEnd();
		glColor4d(1.0, 1.0, 1.0, 1.0);
		int nHeight = 0;
		for(std::list<std::string>::iterator it = m_listRows.begin(); it != m_listRows.end(); it++)
		{
			font.SetPosition(0, nHeight);
			font.Print(it->c_str());
			nHeight += CONSOLE_ROW_HEIGHT;
		}
		font.SetPosition(0, nHeight);
		m_szCommand[m_nCommandLength] = 0;
		std::string str = std::string(CONSOLE_PROMPT) + std::string(m_szCommand) + std::string("_");
		font.Print(str.c_str());
		font.End();

		glDisable(GL_BLEND);
	}

	virtual void Stop()
	{
	}

	virtual void OnKeyDown(int nKey, int nMod)
	{
		switch(nKey)
		{
			case SDLK_ESCAPE:		// Clear the current command, or close console if no command
				m_nCommandLength = 0;
				break;
			case SDLK_BACKSPACE:	// Back up one character
				if(m_nCommandLength > 0)
					m_nCommandLength--;
				break;
			case SDLK_RETURN:		// Execute command
				AddRow(std::string(CONSOLE_PROMPT) + std::string(m_szCommand));
				m_nCommandLength = 0;
				break;
			default:
				if(nKey >= SDLK_SPACE && nKey <= SDLK_z)
				{
					if(nMod & KMOD_SHIFT)
						m_szCommand[m_nCommandLength++] = nKey;
					else
						m_szCommand[m_nCommandLength++] = nKey;
				}
				break;
		}
	}
};

#endif // __ConsoleTask_h__
