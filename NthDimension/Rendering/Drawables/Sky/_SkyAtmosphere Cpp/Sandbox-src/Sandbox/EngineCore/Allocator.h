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

#ifndef __Allocator_h__
#define __Allocator_h__


// A simple buffer class, similar to a vector
template <class T> class TBufferObject : public CAutoRefCounter
{
protected:
	T *m_pBuffer;
	int m_nElements;

	TBufferObject(int nElements)
	{
		m_nElements = nElements;
		m_pBuffer = new T[m_nElements];
	}
	virtual ~TBufferObject()
	{
		delete []m_pBuffer;
	}

public:
	typedef TReference< TBufferObject<T> > Ref;
	static Ref Create(int nElements)	{ return new TBufferObject<T>(nElements); }

	int GetSize()				{ return m_nElements; }
	operator T*()				{ return m_pBuffer; }
	T& operator[](int nIndex)	{ LogAssert(nIndex >= 0 && nIndex < m_nElements); return m_pBuffer[nIndex]; }
};

template <class NODE, class INDEX> class TArray
{
protected:
	INDEX m_nElements;			// The number of elements in the array
	INDEX m_nLockedElements;	// The number of locked elements in the array
	unsigned char *m_pFlags;	// An array of element status flags
	NODE *m_pBuffer;			// The array of elements

public:
	enum { Locked = 0x80, Dirty = 0x40 };

	// Constructor/destructor methods
	TArray(INDEX nElements=0)
	{
		m_nElements = 0;
		if(nElements)
			Init(nElements);
	}
	~TArray()		{ Cleanup(); }

	// Init and Cleanup methods
	void Init(INDEX nElements)
	{
		Cleanup();
		m_nElements = nElements;
		m_nLockedElements = 0;
		m_pFlags = new unsigned char[m_nElements];
		memset(m_pFlags, 0, m_nElements);
		m_pBuffer = new NODE[m_nElements];
	}
	void Cleanup()
	{
		if(m_nElements)
		{
			delete []m_pBuffer;
			delete m_pFlags;
			m_nElements = 0;
		}
	}

	// Info methods
	INDEX GetElementCount()				{ return m_nElements; }
	INDEX GetElementSize()				{ return sizeof(NODE); }
	INDEX GetArraySize()				{ return GetElementCount() * GetElementSize(); }
	INDEX GetLockedElementCount()		{ return m_nLockedElements; }
	INDEX GetFreeElementCount()			{ return GetElementCount() - GetLockedElementCount(); }

	// Status flag methods
	unsigned char GetFlags(INDEX n)					{ return m_pFlags[n]; }
	void SetFlags(INDEX n, unsigned char nFlags)	{ m_pFlags[n] |= nFlags; }
	void ClearFlags(INDEX n, unsigned char nFlags)	{ m_pFlags[n] &= ~nFlags; }

	// Array manipulation methods
	NODE *GetBuffer()					{ return m_pBuffer; }
	NODE *operator[](INDEX n)			{ return &m_pBuffer[n]; }
};


template <class NODE, class INDEX> class TStackedArray : public TArray<NODE, INDEX>
{
protected:
	INDEX *m_pStack;			// A stack of free element indices

public:
	// Constructor/destructor methods
	TStackedArray(INDEX nElements=0)
	{
		TArray<NODE, INDEX>::m_nElements = 0;
		if(nElements)
			Init(nElements);
	}
	~TStackedArray()		{ Cleanup(); }

	// Init and Cleanup methods
	void Init(INDEX nElements)
	{
		Cleanup();
		TArray<NODE, INDEX>::Init(nElements);
		m_pStack = new INDEX[TArray<NODE, INDEX>::m_nElements+1];
		for(INDEX i=0; i<TArray<NODE, INDEX>::m_nElements; i++)
			m_pStack[i] = i;
	}
	void Cleanup()
	{
		if(TArray<NODE, INDEX>::m_nElements)
			delete m_pStack;
		TArray<NODE, INDEX>::Cleanup();
	}

	// Array manipulation methods
	INDEX LockElement()
	{
		INDEX nElement = (INDEX)-1;
		if(TArray<NODE, INDEX>::m_nLockedElements < TArray<NODE, INDEX>::m_nElements)
		{
			nElement = m_pStack[TArray<NODE, INDEX>::m_nLockedElements++];
			TArray<NODE, INDEX>::m_pFlags[nElement] = TArray<NODE, INDEX>::Locked;
		}
		return nElement;
	}
	void UnlockElement(INDEX n)
	{
		TArray<NODE, INDEX>::m_pFlags[n] = 0;
		m_pStack[--TArray<NODE, INDEX>::m_nLockedElements] = n;
	}

	INDEX GetStackIndex(INDEX n)		{ return m_pStack[n]; }
};


#endif // __Allocator_h__
