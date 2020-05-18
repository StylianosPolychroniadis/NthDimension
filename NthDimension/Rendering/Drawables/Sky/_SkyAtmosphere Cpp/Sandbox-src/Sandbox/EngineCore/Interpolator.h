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

#ifndef __Interpolator_h__
#define __Interpolator_h__


class CInterpolator : public CAutoRefCounter
{
protected:
	float *m_pTarget;	// Points to the value that needs to be updated
	bool m_bPaused;

public:
	typedef TReference<CInterpolator> Ref;
	CInterpolator(float *pTarget=NULL)
	{
		m_pTarget = pTarget;
		m_bPaused = false;
	}
	void Pause()				{ m_bPaused = true; }
	void Resume()				{ m_bPaused = false; }
	virtual bool Update(float dt) = 0;
};

class CInterpolatorTask : public CKernelTask, public TSingleton<CInterpolatorTask>
{
protected:
	std::list<CInterpolator::Ref> m_list;

public:
	DEFAULT_TASK_CREATOR(CInterpolatorTask);

	virtual bool Start()
	{
		return true;
	}

	virtual void Update()
	{
		PROFILE("CInterpolatorTask::Update()", 1);
		float fSeconds = CTimerTask::GetPtr()->GetFrameSeconds();
		for(std::list<CInterpolator::Ref>::iterator i = m_list.begin(); i != m_list.end();)
		{
			if((*i)->Update(fSeconds))
				i++;
			else
				i = m_list.erase(i);
		}
	}

	virtual void Stop()
	{
		m_list.clear();
	}

	void Add(CInterpolator::Ref ref)
	{
		m_list.push_back(ref);
	}

	void Kill(CInterpolator::Ref ref)
	{
		std::list<CInterpolator::Ref>::iterator i = std::find(m_list.begin(), m_list.end(), ref);
		if(i != m_list.end())
			m_list.erase(i);
	}
};

template <class T>
class TTimeInterpolator : public CInterpolator
{
protected:
	float m_fStart, m_fEnd;
	float m_fElapsedTime, m_fTotalTime;
	virtual void Calculate() = 0;

public:
	void Reset()
	{
		m_fElapsedTime = 0;
	}
	virtual bool Update(float dt)
	{
		if(!m_bPaused)
		{
			m_fElapsedTime += dt;
			Calculate();
			if(m_fElapsedTime >= m_fTotalTime)
				return false;
		}
		return true;
	}

	static CInterpolator *Create(float *pTarget, float fTime, float fStart, float fEnd)
	{
		TTimeInterpolator *p = new T();
		p->m_pTarget = pTarget;
		p->m_fElapsedTime = 0;
		p->m_fTotalTime = fTime;
		p->m_fStart = fStart;
		p->m_fEnd = fEnd;
		CInterpolatorTask::GetPtr()->Add(p);
		return p;
	}
};

class CLinearInterpolator : public TTimeInterpolator<CLinearInterpolator>
{
protected:
	virtual void Calculate()
	{
		float fRatio = CMath::Clamp(0.0f, 1.0f, m_fElapsedTime / m_fTotalTime);
		*m_pTarget = m_fStart * (1-fRatio) + m_fEnd * fRatio;
	}
};

class CSqrInterpolator : public TTimeInterpolator<CSqrInterpolator>
{
protected:
	virtual void Calculate()
	{
		float fRatio = CMath::Clamp(0.0f, 1.0f, m_fElapsedTime / m_fTotalTime);
		fRatio *= fRatio;
		*m_pTarget = m_fStart * (1-fRatio) + m_fEnd * fRatio;
	}
};

class CSqrtInterpolator : public TTimeInterpolator<CSqrtInterpolator>
{
protected:
	virtual void Calculate()
	{
		float fRatio = CMath::Clamp(0.0f, 1.0f, m_fElapsedTime / m_fTotalTime);
		fRatio = sqrtf(fRatio);
		*m_pTarget = m_fStart * (1-fRatio) + m_fEnd * fRatio;
	}
};

class CSmoothInterpolator : public TTimeInterpolator<CSqrtInterpolator>
{
protected:
	virtual void Calculate()
	{
		float fRatio = CMath::Clamp(0.0f, 1.0f, m_fElapsedTime / m_fTotalTime);
		fRatio = CMath::Smoothstep(0.0f, 1.0f, fRatio);
		*m_pTarget = m_fStart * (1-fRatio) + m_fEnd * fRatio;
	}
};

#endif // __Interpolator_h__
