/* LICENSE
 * Copyright (C) 2017 - 2018 Rafa Software, Hellas - All Rights Reserved
 * Written by Stylianos N. Polychroniadis (info@rafasoftware.com) http://www.rafasoftware.com
 * 
 * This file is part of MySoci.Net Social Network
 * 
 * WARNING! Commercial Software, All Use Must Be Licensed
 * This software is protected by Hellenic Copyright Law and International Treaties. 
 * Unauthorized use, duplication, reverse engineering, any form of redistribution, or 
 * use in part or in whole other than by prior, express, printed and signed license 
 * for use is subject to civil and criminal prosecution. 
*/

#if _WINFORMS_

using System;
using System.Runtime.InteropServices;

namespace NthDimension.Utilities
{
    public static class NativeMethods
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);
        public const int WM_VSCROLL = 277; // Vertical scroll
        public const int SB_BOTTOM = 7; // Scroll to bottom 

        [StructLayout(LayoutKind.Sequential)]
        public struct PeekMsg
        {
            public IntPtr hWnd;
            public Message msg;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public System.Drawing.Point p;
        }

        [System.Security.SuppressUnmanagedCodeSecurity] // We won't use this maliciously
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool PeekMessage(out PeekMsg msg, IntPtr hWnd, uint messageFilterMin, uint messageFilterMax, uint flags);

        public static bool AppStillIdle
        {
            get
            {
                PeekMsg msg;
                return !PeekMessage(out msg, IntPtr.Zero, 0, 0, 0);
            }
        }

        public static void AppendText(RichTextBox box, string line)
        {
            if (box == null || box.IsDisposed)
                return;
            try
            {
                box.AppendText(line + Environment.NewLine);
                ScrollRichTextBox(box);
            }
            catch
            {
            }
        }

        public static void ScrollRichTextBox(RichTextBox box)
        {
            if (box == null || box.IsDisposed || box.Disposing)
                return;
            SendMessage(box.Handle, WM_VSCROLL, (IntPtr)SB_BOTTOM, IntPtr.Zero);
        }

        [DllImport("WinInet.dll", PreserveSig = true, SetLastError = true)]
        public static extern void DeleteUrlCacheEntry(string url);
    }
}

#endif