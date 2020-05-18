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

#ifdef _WIN32
#ifndef __CaptureTask_h__
#define __CaptureTask_h__

#include <vfw.h>


class CCaptureTask : public CKernelTask, public TSingleton<CCaptureTask>
{
protected:
	std::string m_strPath;
	PAVIFILE m_pAVIFile;
	PAVISTREAM m_pAVIStream;
	PAVISTREAM m_pAVICompressed;
	int m_nFrames;

public:
	DEFAULT_TASK_CREATOR(CCaptureTask);

	void SetPath(const char *pszPath)	{ m_strPath = pszPath; }

	virtual bool Start()
	{
		m_pAVIStream = NULL;
		m_strPath = "capture.avi";
		m_pAVICompressed = NULL;
		m_pAVIFile = NULL;
		m_nFrames = 0;
		AVIFileInit();
		return true;
	}

	virtual void Update()
	{
		PROFILE("CCaptureTask::Update()", 1);

		if(m_nFrames == 0)
			OpenAVI();

		HBITMAP hBitmap = CaptureScreen();

		// Get the bitmap bits
		int nSize = CVideoTask::GetPtr()->GetWidth() * CVideoTask::GetPtr()->GetHeight() * 4;
		unsigned char *pBuffer = new unsigned char[nSize];
		if(!GetBitmapBits(hBitmap, nSize, pBuffer))
		{
			char szBuffer[8192];
			LogError(FormatLastError(GetLastError(), szBuffer));
		}

		// Drop the alpha channel and turn the picture upright (it's upside-down)
		int nSize2 = CVideoTask::GetPtr()->GetWidth() * CVideoTask::GetPtr()->GetHeight() * 3;
		unsigned char *pBuffer2 = new unsigned char[nSize2];
		int n = 0;
		for(int y=CVideoTask::GetPtr()->GetHeight()-1; y>=0; y--)
		{
			unsigned char *pSrc = pBuffer + y * CVideoTask::GetPtr()->GetWidth() * 4;
			for(int x=0; x<CVideoTask::GetPtr()->GetWidth(); x++)
			{
				pBuffer2[n++] = *pSrc++;
				pBuffer2[n++] = *pSrc++;
				pBuffer2[n++] = *pSrc++;
				pSrc++;
			}
		}

		HRESULT hResult = AVIStreamWrite(m_pAVICompressed, m_nFrames, 1, pBuffer2, nSize2, AVIIF_KEYFRAME, NULL, NULL);
		delete pBuffer;
		DeleteObject(hBitmap);
		m_nFrames++;
	}

	virtual void Stop()
	{
		CloseAVI();
		AVIFileExit();
	}

	static HBITMAP CaptureScreen()
	{
		int nWidth = CVideoTask::GetPtr()->GetWidth();
		int nHeight = CVideoTask::GetPtr()->GetHeight();
		HDC hdcScreen = wglGetCurrentDC();
		HDC hdcCompatible = CreateCompatibleDC(hdcScreen); 
		HBITMAP hbmScreen = CreateCompatibleBitmap(hdcScreen, nWidth, nHeight);
		HGDIOBJ hOld = SelectObject(hdcCompatible, hbmScreen);
		BitBlt(hdcCompatible, 0, 0, nWidth, nHeight, hdcScreen, 0, 0, SRCCOPY);
		SelectObject(hdcCompatible, hOld);
		DeleteDC(hdcCompatible);
		return hbmScreen;
	}

	void OpenAVI()
	{
		BITMAPINFOHEADER bih;
		memset(&bih, 0, sizeof(BITMAPINFOHEADER));
		bih.biSize = sizeof(BITMAPINFOHEADER);
		bih.biWidth = CVideoTask::GetPtr()->GetWidth();
		bih.biHeight = CVideoTask::GetPtr()->GetHeight();
		bih.biPlanes = 1;
		bih.biBitCount = 24;
		bih.biCompression = BI_RGB;
		bih.biSizeImage = CVideoTask::GetPtr()->GetWidth() * CVideoTask::GetPtr()->GetHeight() * 3;

		AVISTREAMINFO strhdr;
		memset(&strhdr, 0, sizeof(strhdr));
		strhdr.fccType = streamtypeVIDEO;
		strhdr.dwScale = 1;
		strhdr.dwRate = 30;
		strhdr.rcFrame.right = bih.biWidth;
		strhdr.rcFrame.bottom = bih.biHeight;
		strhdr.dwQuality = -1;
		CTimerTask::GetPtr()->LockFrameRate((int)(1000.0f / 30.0f + 0.5f));

		AVICOMPRESSOPTIONS opts;
		AVICOMPRESSOPTIONS *pOpts = &opts;
		memset(&opts, 0, sizeof(AVICOMPRESSOPTIONS));

		DeleteFile(m_strPath.c_str());
		HRESULT hResult = AVIFileOpen(&m_pAVIFile, m_strPath.c_str(), OF_WRITE | OF_CREATE,	NULL);
		hResult = AVIFileCreateStream(m_pAVIFile, &m_pAVIStream, &strhdr);
		BOOL b = AVISaveOptions(NULL, ICMF_CHOOSE_KEYFRAME, 1, &m_pAVIStream, &pOpts);
		hResult = AVIMakeCompressedStream(&m_pAVICompressed, m_pAVIStream, &opts, NULL);
		hResult = AVIStreamSetFormat(m_pAVICompressed, 0, &bih, sizeof(bih));
	}

	void CloseAVI()
	{
		if(m_pAVIStream)
			AVIStreamRelease(m_pAVIStream);
		m_pAVIStream = NULL;
		if(m_pAVICompressed)
			AVIStreamRelease(m_pAVICompressed);
		m_pAVICompressed = NULL;
		if(m_pAVIFile)
			AVIFileRelease(m_pAVIFile);
		m_pAVIFile = NULL;
		CTimerTask::GetPtr()->UnlockFrameRate();
	}

};

#endif // __CaptureTask_h__
#endif // _WIN32
