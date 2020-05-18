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

namespace NthStudio.Utilities
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;

    public class HardwareMonitor
    {
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetPhysicallyInstalledSystemMemory(out long TotalMemoryInKilobytes);


        private static PerformanceCounter   cpuCounter      = new PerformanceCounter();
        
        private static Process              currentProcess  = System.Diagnostics.Process.GetCurrentProcess();

        public static float CurrentCPUusage
        {
            get
            {
                try
                {
                    cpuCounter.CategoryName = "Processor";
                    cpuCounter.CounterName = "% Processor Time";
                    cpuCounter.InstanceName = "_Total";
                    return cpuCounter.NextValue();
                }
                catch
                {
                }
                return 0f;
            }
        }

        public static float CurrentMemoryUsage
        {
            get
            {
                try
                {
                    //long privateWorkingSetBytes = GC.GetTotalMemory(false) / 1024l / 1024l;
                    currentProcess.Refresh();
                    long privateWorkingSetBytes = currentProcess.WorkingSet64 / 1024l / 1024l;
                    return ((int)privateWorkingSetBytes); // MBytes


                }
                catch { }
                return 0f;
            }
        }

        public static long GetSystemRAM
        {
            get
            {
                ////int bytesPerMebibyte = (1 << 20);  // http://physics.nist.gov/cuu/Units/binary.html
                ////Microsoft.VisualBasic.Devices.ComputerInfo myCompInfo = new Microsoft.VisualBasic.Devices.ComputerInfo();

                ////return Math.Abs(((int)myCompInfo.TotalPhysicalMemory / bytesPerMebibyte) * 1024); // MBytes

                ////return 0;



                long memKb;
                GetPhysicallyInstalledSystemMemory(out memKb);

                return memKb;
            }
        }

        public static long GetTotalFreeSpace(string driveName)
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady && drive.Name == driveName)
                {
                    return drive.AvailableFreeSpace;
                }
            }
            return -1;
        }
    }

    internal static class CpuId
    {

        [DllImport("user32", EntryPoint = "CallWindowProcW", CharSet = CharSet.Unicode, SetLastError = true,
            ExactSpelling = true)]
        private static extern IntPtr CallWindowProcW([In] byte[] bytes, IntPtr hWnd, int msg, [In, Out] byte[] wParam,
            IntPtr lParam);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool VirtualProtect([In] byte[] bytes, IntPtr size, int newProtect, out int oldProtect);

        const int PAGE_EXECUTE_READWRITE = 0x40;

        public static string GetCpuId()
        {
            var sn = new byte[8];

            return !ExecuteCode(ref sn) ? "ND" : string.Format("{0:X8}{1:X8}", BitConverter.ToUInt32(sn, 4), BitConverter.ToUInt32(sn, 0));
        }

        private static bool ExecuteCode(ref byte[] result)
        {
            /* The opcodes below implement a C function with the signature:
             * __stdcall CpuIdWindowProc(hWnd, Msg, wParam, lParam);
             * with wParam interpreted as an 8 byte unsigned character buffer.
             * */

            var isX64Process = IntPtr.Size == 8;
            byte[] code;

            if (isX64Process)
            {
                code = new byte[]
                {
                    0x53, /* push rbx */
                    0x48, 0xc7, 0xc0, 0x01, 0x00, 0x00, 0x00, /* mov rax, 0x1 */
                    0x0f, 0xa2, /* cpuid */
                    0x41, 0x89, 0x00, /* mov [r8], eax */
                    0x41, 0x89, 0x50, 0x04, /* mov [r8+0x4], edx */
                    0x5b, /* pop rbx */
                    0xc3, /* ret */
                };
            }
            else
            {
                code = new byte[]
                {
                    0x55, /* push ebp */
                    0x89, 0xe5, /* mov  ebp, esp */
                    0x57, /* push edi */
                    0x8b, 0x7d, 0x10, /* mov  edi, [ebp+0x10] */
                    0x6a, 0x01, /* push 0x1 */
                    0x58, /* pop  eax */
                    0x53, /* push ebx */
                    0x0f, 0xa2, /* cpuid    */
                    0x89, 0x07, /* mov  [edi], eax */
                    0x89, 0x57, 0x04, /* mov  [edi+0x4], edx */
                    0x5b, /* pop  ebx */
                    0x5f, /* pop  edi */
                    0x89, 0xec, /* mov  esp, ebp */
                    0x5d, /* pop  ebp */
                    0xc2, 0x10, 0x00, /* ret  0x10 */
                };
            }

            var ptr = new IntPtr(code.Length);

            int i0 = 0;

            if (!VirtualProtect(code, ptr, PAGE_EXECUTE_READWRITE, out i0))
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());

            ptr = new IntPtr(result.Length);
            return CallWindowProcW(code, IntPtr.Zero, 0, result, ptr) != IntPtr.Zero;

        }
    }


}
