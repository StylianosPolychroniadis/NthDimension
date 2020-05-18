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

#ifndef __Functor_h__
#define __Functor_h__


class CFunctor : public CAutoRefCounter
{
public:
	typedef TReference<CFunctor> Ref;
	virtual void operator ()()=0;
	virtual bool operator==(void *p)=0;
};

template<class T>
class TFunctor : public CFunctor
{
protected:
	TReference<T> m_refObject;
	typedef void (T::*FunctionType)();
	FunctionType m_pFunction;

public:
	TFunctor(T *pObject, FunctionType pFunction)
	{
		m_refObject = pObject;
		m_pFunction = pFunction;
	}
	virtual void operator ()()			{ (m_refObject->*m_pFunction)(); }
	virtual bool operator==(void *p)	{ return p == (void *)((T *)m_refObject); }
};

template<class T>
class TPtrFunctor : public CFunctor
{
protected:
	T *m_pObject;
	typedef void (T::*FunctionType)();
	FunctionType m_pFunction;

public:
	TPtrFunctor(T *pObject, FunctionType pFunction)
	{
		m_pObject = pObject;
		m_pFunction = pFunction;
	}
	virtual void operator ()()			{ (m_pObject->*m_pFunction)(); }
	virtual bool operator==(void *p)	{ return p == (void *)m_pObject; }
};

#endif // __Functor_h__
