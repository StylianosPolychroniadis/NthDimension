/*
 Copyright (c) 2004 DigiTec Web Consultants, LLC.  All rights reserved.

 The use of this software is for test and performance purposes only.
 You may not use this software in any commercial applications without
 the express permission of the copyright holder.  You may add to or
 modify the code contained here-in to cause it to run slower without
 contacting the copyright holder, however, any attempts to make this
 code run faster should be documented on:

 http://weblogs.asp.net/justin_rogers/articles/131704.aspx

 I reserve the right to change or modify the publicly available version
 of this code at any time.  I will not provide version protection, so
 if you have reliance on a particular build of this software, then make
 your own back-ups.

 You must laugh, at least a little, when reading this licensing agreement,
 unless of course you don't have a sense of humor.  In all seriousness,
 excluding the laughter, laughter in itself does not void this license
 agreement, nor compromise it's ability to legally bind you.

 You must not remove this notice, or any other, from this software.
*/

/* LICENSE
 * Copyright (C) 2008 - 2018 SYSCON Technologies, Hellas - All Rights Reserved
 * Written by Stylianos N. Polychroniadis (info@polytronic.gr) http://www.polytronic.gr
 * 
 * This file is part of nthDimension Platform
 * 
 * WARNING! Commercial Software, All Use Must Be Licensed
 * This software is protected by Hellenic Copyright Law and International Treaties. 
 * Unauthorized use, duplication, reverse engineering, any form of redistribution, or 
 * use in part or in whole other than by prior, express, printed and signed license 
 * for use is subject to civil and criminal prosecution. 
*/

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace NthDimension.Rendering.Imaging
{


    /*
        Need some notes on use.  I previously published a blog entry describing
        this guy, but the end result was that unless you had already done some
        GDI+ work, my library would fail.  The reason it works in Windows Forms
        applications is because WinForms uses and initializes the library
        correctly before I get there.  However, try to load ImageFast stand-alone
        and all you get is a NullReferenceException.

        I'm past all of that now.  We correctly call GdiplusStartup and
        GdiplusShutdown (hopefully).  In addition, I've added more thorough image
        loading support by providing metafile loaders.

        To compile a release library:
            csc /t:library /optimize+ ImageFast.cs

        To compile a debug library with some console output:
            csc /t:library /debug+ /d:DEBUG,TRACE ImageFast.cs

        NOTE: This is now ready for ASP .NET users.  I've verified all of the
        different startup paths and shutdown paths to make sure there isn't
        anything strange with the libraries that would prevent them from
        running under ASP .NET without special tuning or instrumentation.
    */



    public class ImageFast
    {
        [DllImport("gdiplus.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern int GdipLoadImageFromFile(string filename, out IntPtr image);

        [DllImport("gdiplus.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern int GdiplusStartup(out IntPtr token, ref StartupInput input, out StartupOutput output);

        [DllImport("gdiplus.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern int GdiplusShutdown(IntPtr token);

        [DllImport("gdiplus.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern int GdipGetImageType(IntPtr image, out GdipImageTypeEnum type);

        private static IntPtr gdipToken = IntPtr.Zero;

        static ImageFast()
        {
#if DEBUG
            Console.WriteLine("Initializing GDI+");
#endif
            if (gdipToken == IntPtr.Zero)
            {
                StartupInput input = StartupInput.GetDefaultStartupInput();
                StartupOutput output;

                int status = GdiplusStartup(out gdipToken, ref input, out output);
#if DEBUG
                if (status == 0)
                    Console.WriteLine("Initializing GDI+ completed successfully");
#endif
                if (status == 0)
                    AppDomain.CurrentDomain.ProcessExit += new EventHandler(Cleanup_Gdiplus);
            }
        }

        private static void Cleanup_Gdiplus(object sender, EventArgs e)
        {
#if DEBUG
            Console.WriteLine("GDI+ shutdown entered through ProcessExit event");
#endif
            if (gdipToken != IntPtr.Zero)
                GdiplusShutdown(gdipToken);

#if DEBUG
            Console.WriteLine("GDI+ shutdown completed");
#endif
        }

        private static Type bmpType = typeof(System.Drawing.Bitmap);
        private static Type emfType = typeof(System.Drawing.Imaging.Metafile);

        public static Image FromFile(string filename)
        {
            filename = Path.GetFullPath(filename);
            IntPtr loadingImage = IntPtr.Zero;

            // We are not using ICM at all, fudge that, this should be FAAAAAST!
            if (GdipLoadImageFromFile(filename, out loadingImage) != 0)
            {
                throw new Exception("GDI+ threw a status error code.");
            }

            GdipImageTypeEnum imageType;
            if (GdipGetImageType(loadingImage, out imageType) != 0)
            {
                throw new Exception("GDI+ couldn't get the image type");
            }

            switch (imageType)
            {
                case GdipImageTypeEnum.Bitmap:
                    return (Bitmap)bmpType.InvokeMember("FromGDIplus", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, new object[] { loadingImage });
                case GdipImageTypeEnum.Metafile:
                    return (Metafile)emfType.InvokeMember("FromGDIplus", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, new object[] { loadingImage });
            }

            throw new Exception("Couldn't convert underlying GDI+ object to managed object");
        }

        private ImageFast() { }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct StartupInput
    {
        public int GdiplusVersion;
        public IntPtr DebugEventCallback;
        public bool SuppressBackgroundThread;
        public bool SuppressExternalCodecs;

        public static StartupInput GetDefaultStartupInput()
        {
            StartupInput result = new StartupInput();
            result.GdiplusVersion = 1;
            result.SuppressBackgroundThread = false;
            result.SuppressExternalCodecs = false;
            return result;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct StartupOutput
    {
        public IntPtr Hook;
        public IntPtr Unhook;
    }

    internal enum GdipImageTypeEnum
    {
        Unknown = 0,
        Bitmap = 1,
        Metafile = 2
    }
}




