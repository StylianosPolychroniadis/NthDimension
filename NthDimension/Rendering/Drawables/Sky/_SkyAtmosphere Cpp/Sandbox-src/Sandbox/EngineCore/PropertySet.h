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


#ifndef __PropertySet_h__
#define __PropertySet_h__


class CPropertySet
{
protected:
	std::string m_strPrefix;
	std::map<std::string, std::string> m_mapProperties;

public:
	CPropertySet(const char *pszPrefix="")
	{
		m_strPrefix = pszPrefix;
	}
	CPropertySet(const CPropertySet &prop, const char *pszPrefix="")
	{
		m_strPrefix = pszPrefix;
		*this = prop;
	}

	void SetPrefix(const char *pszPrefix="")	{ m_strPrefix = pszPrefix; }
	const char *GetPrefix()const				{ return m_strPrefix.c_str(); }

	const CPropertySet &operator=(const CPropertySet &prop)
	{
		std::map<std::string, std::string>::const_iterator i;
		for(i = prop.m_mapProperties.begin(); i != prop.m_mapProperties.end(); i++)
		{
			// If a prefix string is set, only copy the properties that have a matching prefix
			if(m_strPrefix.empty() || i->first.find(m_strPrefix) == 0)
				SetProperty(i->first.c_str(), i->second.c_str());
		}
		return *this;
	}

	const char *GetRootProperty(const char *pszKey, const char *pszDefault=NULL) const
	{
		// This method is a quick and dirty way to get a property without using the prefix
		std::string strKey = pszKey;
		std::map<std::string, std::string>::const_iterator it = m_mapProperties.find(strKey);
		if(it != m_mapProperties.end())
			return it->second.c_str();
		return pszDefault;
	}

	const char *GetProperty(const char *pszKey, const char *pszDefault=NULL) const
	{
		// If a prefix string is set, force it to be at the front of any properties retrieved
		std::string strKey = pszKey;
		std::map<std::string, std::string>::const_iterator it = m_mapProperties.find((m_strPrefix.empty() || strKey.find(m_strPrefix) == 0) ? strKey : m_strPrefix + strKey);
		if(it != m_mapProperties.end())
			return it->second.c_str();
		return pszDefault;
	}

	int GetIntProperty(const char *pszKey, int nDefault=0) const
	{
		const char *psz = GetProperty(pszKey);
		if(psz != NULL)
			nDefault = atoi(psz);
		return nDefault;
	}

	float GetFloatProperty(const char *pszKey, float fDefault=0.0f) const
	{
		const char *psz = GetProperty(pszKey);
		if(psz != NULL)
			fDefault = (float)atof(psz);
		return fDefault;
	}

	int GetInt(const char *key, int def=0) { return GetIntProperty(key, def); }

	int GetRootInt(const char *pszKey, int nDefault=0) const
	{
		const char *psz = GetRootProperty(pszKey);
		if(psz != NULL)
			nDefault = atoi(psz);
		return nDefault;
	}

	void SetProperty(const char *pszKey, const char *pszValue)
	{
		// If a prefix string is set, force it to be at the front of any properties set
		std::string strKey = pszKey;
		m_mapProperties[(m_strPrefix.empty() || strKey.find(m_strPrefix) == 0) ? strKey : m_strPrefix + strKey] = pszValue;
	}

	void SetProperty(const char *pszKey, int nValue)
	{
		char szValue[20];
		itoa(nValue, szValue, 10);
		SetProperty(pszKey, szValue);
	}

	void SetProperty(const char *pszKey, float fValue)
	{
		char szValue[20];
		sprintf(szValue, "%f", fValue);
		SetProperty(pszKey, szValue);
	}

	bool DeleteProperty(const char *pszKey)
	{
		std::string strKey = pszKey;
		std::map<std::string, std::string>::iterator it = m_mapProperties.find((m_strPrefix.empty() || strKey.find(m_strPrefix) == 0) ? strKey : m_strPrefix + strKey);
		if(it != m_mapProperties.end())
		{
			m_mapProperties.erase(it);
			return true;
		}
		return false;
	}

	int DeletePrefix(const char *pszPrefix)
	{
		int nErased = 0;
		int nLength = (int)strlen(pszPrefix);
		std::map<std::string, std::string>::iterator it = m_mapProperties.begin();
		while(it != m_mapProperties.end())
		{
			if(memcmp(pszPrefix, it->first.c_str(), nLength) == 0)
			{
				std::map<std::string, std::string>::iterator itDead = it;
				it++;
				m_mapProperties.erase(itDead);
				nErased++;
			}
			else
				it++;
		}
		return nErased;
	}

	void Clear()
	{
		m_mapProperties.clear();
	}

	void Load(std::istream &in)
	{
		char szBuffer[8192];
		in.getline(szBuffer, 8192);
		while(in)
		{
			int i=0;
			while(szBuffer[i] != 0 && szBuffer[i] != '#' && szBuffer[i] != '=')
				i++;
			if(szBuffer[i] == '=')
			{
				szBuffer[i] = 0;
				int j = i+1;
				while(szBuffer[j] != 0 && szBuffer[j] != '#')
					j++;
				while(szBuffer[j-1] <= 0x20)
					j--;
				szBuffer[j] = 0;
				if(j > i)
				{
					// If a prefix string is set, only load the properties that have a matching prefix
					if(m_strPrefix.empty() || std::string(&szBuffer[i+1]).find(m_strPrefix) == 0)
						SetProperty(szBuffer, &szBuffer[i+1]);
				}
			}
			in.getline(szBuffer, 8192);
		}
	}

	void Save(std::ostream &out)
	{
		std::map<std::string, std::string>::iterator i;
		for(i = m_mapProperties.begin(); i != m_mapProperties.end(); i++)
		{
			// If a prefix string is set, only save the properties that have a matching prefix
			if(m_strPrefix.empty() || i->first.find(m_strPrefix) == 0)
				out << i->first << "=" << i->second << std::endl;
		}
	}

	void LoadFile(const char *pszPath)
	{
		std::ifstream in(pszPath);
		if(in)
			Load(in);
		in.close();
	}

	void SaveFile(const char *pszPath)
	{
		std::ofstream out(pszPath);
		if(out)
			Save(out);
		out.close();
	}
};

#endif // __PropertySet_h__
