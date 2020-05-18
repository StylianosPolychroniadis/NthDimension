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

#ifndef __Dator_h__
#define __Dator_h__

template <class T>
T ToVal(std::string &str)
{
	std::stringstream s;
	s.unsetf(std::ios::skipws);
	s << str;
	T res;
	s >> res;
	return res;
}

template <class T>
std::string ToString(T &val)
{
	std::stringstream s;
	s.unsetf(std::ios::skipws);
	s << val;
	std::string res;
	s >> res;
	return res;
}


class CDator : public CAutoRefCounter
{
protected:
	CDator() {}
	CDator(CDator &d){ *this = d; }

public:
	typedef TReference<CDator> Ref;
	virtual CDator &operator=(std::string &str)=0;
	virtual CDator &operator+=(std::string &str)=0;
	virtual CDator &operator-=(std::string &str)=0;
	virtual bool operator==(std::string &str)=0;
	virtual bool operator!=(std::string &str)=0;

	virtual bool HasMultipleValues() = 0;
	virtual operator std::string()=0;
};

template<class T>
class TDator : public CDator
{
protected:
	T &m_data;

public:
	TDator(T &t) : m_data(t) {}
	virtual CDator &operator=(std::string &str)		{ m_data = ToVal<T>(str); return *this; }
	virtual CDator &operator+=(std::string &str)	{ m_data += ToVal<T>(str); return *this; }
	virtual CDator &operator-=(std::string &str)	{ m_data -= ToVal<T>(str); return *this; }
	virtual bool operator==(std::string &str)		{ return (str == (std::string)(*this)); }
	virtual bool operator!=(std::string &str)		{ return !(*this == str); }
	virtual operator std::string()					{ return ToString<T>(m_data); }
	virtual bool HasMultipleValues()				{ return false; }

	operator T&()									{ return m_data; }
};

template<class T>
class TListDator : public CDator
{
protected:
	std::list<T> &m_data;

public:
	TListDator(std::list<T> &l) : m_data(l) {}
	virtual CDator &operator=(std::string &str)
	{
		m_data.clear();
		size_t nStart = 0;
		size_t nEnd = str.find(';', nStart);
		while(nEnd != (size_t)-1)
		{
			m_data.push_back(ToVal<T>(str.substr(nStart, nEnd-nStart)));
			nStart = nEnd + 1;
			nEnd = str.find(';', nStart);
		}
		m_data.push_back(ToVal<T>(str.substr(nStart)));
		return *this;
	}
	virtual CDator &operator+=(std::string &str)	{ m_data.push_back(ToVal<T>(str)); return *this; }
	virtual CDator &operator-=(std::string &str)	{ m_data.remove(ToVal<T>(str)); return *this; }
	virtual bool operator==(std::string &str)		{ return find(m_data.begin(), m_data.end(), ToVal<T>(str)) != m_data.end(); }
	virtual bool operator!=(std::string &str)		{ return !(*this == str); }
	virtual operator std::string()
	{
		std::stringstream s;
		std::list<T>::iterator it = m_data.begin();
		for(it=m_data.begin(); it!=m_data.end(); it++)
		{
			if(it != m_data.begin())
				s << ';';
			s << *it;
		}
		s << std::ends;
		return s.str();
	}
	virtual bool HasMultipleValues()		{ return true; }

	operator std::list<T>&()				{ return m_data; }
};

#endif // __Dator_h__
