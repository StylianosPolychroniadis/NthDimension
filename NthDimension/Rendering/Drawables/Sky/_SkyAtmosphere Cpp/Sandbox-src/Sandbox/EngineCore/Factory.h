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

template<class BaseClass, class ObjectClass>
BaseClass *CreateObject()
{
   return new ObjectClass();
}


template <class BaseClass, class IDType>
class TClassFactory
{
protected:
	typedef BaseClass *(*CreateObjectFunc)();
	std::map<IDType, CreateObjectFunc> m_mapObjectClasses;

public:
	template<class ObjectClass>
	bool Register(IDType id)
	{
		if(m_mapObjectClasses.find(id) != m_mapObjectClasses.end())
			return false;
		m_mapObjectClasses[id] = &CreateObject<BaseClass, ObjectClass>;
		return true;
	}

	bool Unregister(IDType id)
	{
		return (m_mapObjectClasses.erase(id) == 1);
	}

	BaseClass *Create(IDType id)
	{
		if(m_mapObjectClasses.find(id) == m_mapObjectClasses.end())
			return NULL;
		return (m_mapObjectClasses[id])();
	}
};
