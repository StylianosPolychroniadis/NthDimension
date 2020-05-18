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

#ifndef __EngineCore_h__
#define __EngineCore_h__


// Fixed includes (third-party, almost never change)
#include "CommonIncludes.h"

// Some common constants and inline functions
#include "CommonDefines.h"

// Utility headers
#include "DateTime.h"
#include "Log.h"
#include "Singleton.h"
#include "Reference.h"
#include "PropertySet.h"
#include "Functor.h"
#include "Profiler.h"
#include "Factory.h"
#include "Object.h"
#include "Allocator.h"


// Math headers
#include "Noise.h"
#include "Matrix.h"
#include "Geometry.h"

// Miscellaneous headers
#include "PixelBuffer.h"
#include "Texture.h"
#include "Font.h"

// Kernel headers
#include "Kernel.h"
#include "TimerTask.h"
#include "Trigger.h"
#include "Interpolator.h"
#include "InputTask.h"
#include "VideoTask.h"
#include "CameraTask.h"
#include "ConsoleTask.h"
#include "CaptureTask.h"

// Miscellaneous headers
#include "GLBufferObject.h"
#include "GLShaderObject.h"
#include "GLFrameBufferObject.h"


extern const std::string g_strBuildStamp;

#define DECLARE_CORE_GLOBALS\
	const std::string g_strBuildStamp = std::string(__DATE__) + std::string(" ") + std::string(__TIME__);\
	CLog *CLog::m_pSingleton = NULL;\
	CObjectType CObjectType::m_root


#endif // __EngineCore_h__
