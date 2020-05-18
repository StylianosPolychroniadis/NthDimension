# Microsoft Developer Studio Project File - Name="EngineCore" - Package Owner=<4>
# Microsoft Developer Studio Generated Build File, Format Version 6.00
# ** DO NOT EDIT **

# TARGTYPE "Win32 (x86) Static Library" 0x0104

CFG=EngineCore - Win32 Debug
!MESSAGE This is not a valid makefile. To build this project using NMAKE,
!MESSAGE use the Export Makefile command and run
!MESSAGE 
!MESSAGE NMAKE /f "EngineCore.mak".
!MESSAGE 
!MESSAGE You can specify a configuration when running NMAKE
!MESSAGE by defining the macro CFG on the command line. For example:
!MESSAGE 
!MESSAGE NMAKE /f "EngineCore.mak" CFG="EngineCore - Win32 Debug"
!MESSAGE 
!MESSAGE Possible choices for configuration are:
!MESSAGE 
!MESSAGE "EngineCore - Win32 Release" (based on "Win32 (x86) Static Library")
!MESSAGE "EngineCore - Win32 Debug" (based on "Win32 (x86) Static Library")
!MESSAGE 

# Begin Project
# PROP AllowPerConfigDependencies 0
# PROP Scc_ProjName ""
# PROP Scc_LocalPath ""
CPP=cl.exe
RSC=rc.exe

!IF  "$(CFG)" == "EngineCore - Win32 Release"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "Release"
# PROP BASE Intermediate_Dir "Release"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "..\Release"
# PROP Intermediate_Dir "Release"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /W3 /GX /O2 /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /YX /FD /c
# ADD CPP /nologo /G5 /Zp4 /MD /W3 /GX /Zi /O2 /I "..\include" /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /FR /YX /FD /c
# ADD BASE RSC /l 0x409 /d "NDEBUG"
# ADD RSC /l 0x409 /d "NDEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=link.exe -lib
# ADD BASE LIB32 /nologo
# ADD LIB32 /nologo

!ELSEIF  "$(CFG)" == "EngineCore - Win32 Debug"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 1
# PROP BASE Output_Dir "Debug"
# PROP BASE Intermediate_Dir "Debug"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 1
# PROP Output_Dir "..\Debug"
# PROP Intermediate_Dir "Debug"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /W3 /Gm /GX /ZI /Od /D "WIN32" /D "_DEBUG" /D "_MBCS" /D "_LIB" /YX /FD /GZ /c
# ADD CPP /nologo /G5 /Zp4 /MDd /W3 /Gm /GX /ZI /Od /I "..\include" /D "WIN32" /D "_DEBUG" /D "_MBCS" /D "_LIB" /FR /YX /FD /GZ /c
# ADD BASE RSC /l 0x409 /d "_DEBUG"
# ADD RSC /l 0x409 /d "_DEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=link.exe -lib
# ADD BASE LIB32 /nologo
# ADD LIB32 /nologo

!ENDIF 

# Begin Target

# Name "EngineCore - Win32 Release"
# Name "EngineCore - Win32 Debug"
# Begin Group "Source Files"

# PROP Default_Filter "cpp;c;cxx;rc;def;r;odl;idl;hpj;bat"
# Begin Source File

SOURCE=.\Noise.cpp
# End Source File
# Begin Source File

SOURCE=.\PixelBuffer.cpp
# End Source File
# Begin Source File

SOURCE=.\Texture.cpp
# End Source File
# End Group
# Begin Group "Header Files"

# PROP Default_Filter "h;hpp;hxx;hm;inl"
# Begin Source File

SOURCE=.\CommonDefines.h
# End Source File
# Begin Source File

SOURCE=.\CommonIncludes.h
# End Source File
# Begin Source File

SOURCE=.\EngineCore.h
# End Source File
# Begin Source File

SOURCE=.\Font.h
# End Source File
# Begin Source File

SOURCE=.\Geometry.h
# End Source File
# Begin Source File

SOURCE=.\GLBufferObject.h
# End Source File
# Begin Source File

SOURCE=.\GLFrameBufferObject.h
# End Source File
# Begin Source File

SOURCE=.\GLShaderObject.h
# End Source File
# Begin Source File

SOURCE=.\Matrix.h
# End Source File
# Begin Source File

SOURCE=.\Noise.h
# End Source File
# Begin Source File

SOURCE=.\PixelBuffer.h
# End Source File
# Begin Source File

SOURCE=.\Texture.h
# End Source File
# End Group
# Begin Group "Utility Files"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\Allocator.h
# End Source File
# Begin Source File

SOURCE=.\DateTime.h
# End Source File
# Begin Source File

SOURCE=.\Factory.h
# End Source File
# Begin Source File

SOURCE=.\Functor.h
# End Source File
# Begin Source File

SOURCE=.\Log.h
# End Source File
# Begin Source File

SOURCE=.\Object.h
# End Source File
# Begin Source File

SOURCE=.\Profiler.h
# End Source File
# Begin Source File

SOURCE=.\PropertySet.h
# End Source File
# Begin Source File

SOURCE=.\Reference.h
# End Source File
# Begin Source File

SOURCE=.\Singleton.h
# End Source File
# End Group
# Begin Group "Kernel Files"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\CameraTask.h
# End Source File
# Begin Source File

SOURCE=.\CaptureTask.h
# End Source File
# Begin Source File

SOURCE=.\ConsoleTask.h
# End Source File
# Begin Source File

SOURCE=.\InputTask.h
# End Source File
# Begin Source File

SOURCE=.\Interpolator.h
# End Source File
# Begin Source File

SOURCE=.\Kernel.h
# End Source File
# Begin Source File

SOURCE=.\TimerTask.h
# End Source File
# Begin Source File

SOURCE=.\Trigger.h
# End Source File
# Begin Source File

SOURCE=.\VideoTask.h
# End Source File
# End Group
# End Target
# End Project
