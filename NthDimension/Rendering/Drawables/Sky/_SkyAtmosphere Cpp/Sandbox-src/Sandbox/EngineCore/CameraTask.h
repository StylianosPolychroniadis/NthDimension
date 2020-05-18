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

#ifndef __CameraTask_h__
#define __CameraTask_h__


class CCameraTask : public CKernelTask, public TSingleton<CCameraTask>
{
protected:
	float m_fThrust;
	CVector m_vOldPos;
	CVector m_vVelocity;
	CSRTTransform m_srtCamera;

public:
	DEFAULT_TASK_CREATOR(CCameraTask);

	float GetThrust() const					{ return m_fThrust; }
	const CSRTTransform &GetCamera() const	{ return m_srtCamera; }
	CVector GetVelocity() const				{ return m_vVelocity; }
	CVector GetPosition() const				{ return m_srtCamera.GetPosition(); }

	void SetThrust(const float f)		{ m_fThrust = f; }
	void SetVelocity(const CVector &v)	{ m_vVelocity = v; }
	void SetPosition(const CVector &v)	{ m_srtCamera.SetPosition(v); }

	virtual bool Start()
	{
		m_fThrust = 2.0f;
		m_vVelocity = CVector(0.0f);
		return true;
	}
	virtual void Update()
	{
		PROFILE("CCameraTask::Update()", 1);
		float fSeconds = (float)CTimerTask::GetPtr()->GetFrameSeconds();

		if(CInputTask::GetPtr()->IsKeyDown(SDLK_KP2) || CInputTask::GetPtr()->IsKeyDown(SDLK_DOWN))
			m_srtCamera.Rotate(CQuaternion(m_srtCamera.GetRightAxis(), fSeconds*0.5f));
		if(CInputTask::GetPtr()->IsKeyDown(SDLK_KP8) || CInputTask::GetPtr()->IsKeyDown(SDLK_UP))
			m_srtCamera.Rotate(CQuaternion(m_srtCamera.GetRightAxis(), -fSeconds*0.5f));
		if(CInputTask::GetPtr()->IsKeyDown(SDLK_KP4) || CInputTask::GetPtr()->IsKeyDown(SDLK_LEFT))
			m_srtCamera.Rotate(CQuaternion(m_srtCamera.GetUpAxis(), fSeconds*0.5f));
		if(CInputTask::GetPtr()->IsKeyDown(SDLK_KP6) || CInputTask::GetPtr()->IsKeyDown(SDLK_RIGHT))
			m_srtCamera.Rotate(CQuaternion(m_srtCamera.GetUpAxis(), -fSeconds*0.5f));
		if(CInputTask::GetPtr()->IsKeyDown(SDLK_KP7) || CInputTask::GetPtr()->IsKeyDown(SDLK_q))
			m_srtCamera.Rotate(CQuaternion(m_srtCamera.GetViewAxis(), -fSeconds*0.5f));
		if(CInputTask::GetPtr()->IsKeyDown(SDLK_KP9) || CInputTask::GetPtr()->IsKeyDown(SDLK_e))
			m_srtCamera.Rotate(CQuaternion(m_srtCamera.GetViewAxis(), fSeconds*0.5f));

		CVector vAccel(0.0f);
		if(CInputTask::GetPtr()->IsKeyDown(SDLK_SPACE))
			m_vVelocity = CVector(0.0f);
		else
		{
			if(CInputTask::GetPtr()->IsKeyDown(SDLK_w))
				vAccel += m_srtCamera.GetViewAxis();
			if(CInputTask::GetPtr()->IsKeyDown(SDLK_s))
				vAccel -= m_srtCamera.GetViewAxis();
			if(CInputTask::GetPtr()->IsKeyDown(SDLK_d))
				vAccel += m_srtCamera.GetRightAxis();
			if(CInputTask::GetPtr()->IsKeyDown(SDLK_a))
				vAccel -= m_srtCamera.GetRightAxis();

			if(CInputTask::GetPtr()->IsKeyDown(SDLK_RCTRL) || CInputTask::GetPtr()->IsKeyDown(SDLK_LCTRL))
				vAccel *= m_fThrust * 10.0f;
			else
				vAccel *= m_fThrust;

			m_vOldPos = m_srtCamera.m_vTranslate;
			m_srtCamera.Translate(m_vVelocity * fSeconds + vAccel * (0.5f * fSeconds * fSeconds));
			m_vVelocity += vAccel * fSeconds;
		}

		CMatrix4 mView = m_srtCamera.BuildViewMatrix();
		glLoadMatrixf(mView);
	}

	virtual void Stop()		{}

	void Bounce()
	{
		m_vVelocity = -m_vVelocity;
		m_srtCamera.m_vTranslate = m_vOldPos;
	}
};

#endif // __CameraTask_h__
