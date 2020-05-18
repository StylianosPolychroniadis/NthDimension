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

#define CONSOLE

namespace NthDimension.Rendering.Utilities
{
#if _DEVELOPER_
    using MySociNet.Developer.Console;
#endif

    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;

    public class ConsoleUtil
    {
#if _WINDOWS_
        #region WinApi P/Invokes

        static IntPtr __consoleHandle;
        const int STD_OUTPUT_HANDLE = -11;
        const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);       

        /// <summary>
        /// Enables ANSI Console features (color, underline, bold, etc)
        /// </summary>
        public static void ANSIEffects()
        {
            __consoleHandle = GetStdHandle(STD_OUTPUT_HANDLE);
            uint mode;
            GetConsoleMode(__consoleHandle, out mode);
            mode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;
            SetConsoleMode(__consoleHandle, mode);
        }

        #endregion WinApi P/Invokes
#endif

        /// <summary>
        /// The character 
        /// </summary>
        public const string     tick            = "\u221A";
        /// <summary>
        /// The character • (Note: Also produces a System.Beep)
        /// </summary>
        public const string     bullet          = "\u2022";              // Causes System.Beep(s)
        /// <summary>
        /// The character ◦ 
        /// </summary>
        public const string     bulletWhite     = "\u263a";             // U+263A Simley
        /// <summary>
        /// The character ©
        /// </summary>
        public const string     copyright       = "\u00A9";
        /// <summary>
        /// The character ®
        /// </summary>
        public const string     registered      = "\u00AE";
        /// <summary>
        /// The character ™
        /// </summary>
        public const string     trademark       = "\u2122";
        /// <summary>
        /// The character ℠
        /// </summary>
        public const string     servicemark     = "\u2120";
        public const char       square            = (char)255;
        public const char       diamond           = (char)4;
        public const char       smileyWhite       = (char)1;
        public const char       smileyBlack       = (char)2;
        public const char       leftTriangle      = (char)17;
        public const char       rightTriangle     = (char)16;
        public const char       upTriangle        = (char)30;
        public const char       downTriangle      = (char)31;


        public static void log(string text, bool newLine = true)
        {
            try
            {
                // Debugger Friendly

                if (null != ApplicationBase.Instance && !newLine)
                    ApplicationBase.Instance.WriteToLogFile(text);
                //GameBase.Instance.WriteToLogFile(string.Format("{0}{1}", text, newLine ? Environment.NewLine : string.Empty));

#if CONSOLE
                consoleLog(text, newLine);
#endif

            }
            catch { }
        }

#if CONSOLE
        private static void consoleLog(string text, bool newLine)
        {
#if _WINDOWS_
            ANSIEffects();
#endif
            if (newLine)
            {


#if _DEVELOPER_
                    ConsoleExt.PrependLine(text);
#else
                Console.WriteLine(text);
#endif
            }
            else
            {
                int bufferWidth = Console.BufferWidth;
                int left = Console.CursorLeft;
                int top = Console.CursorTop;
                if (Console.CursorTop > 0)
                    Console.SetCursorPosition(0, Console.CursorTop - 1);

#if _DEVELOPER_
                    string t = string.Format(text, bufferWidth - left);
                    ConsoleExt.PrependLine(t);
#else
                Console.WriteLine(text, bufferWidth - left);
#endif
            }
        }
#endif

        public static void logFileSize(string message, string filesize)
        {
            try
            {
                // Debugger Friendly
                if (null != ApplicationBase.Instance)
                    ApplicationBase.Instance.WriteToLogFile(string.Format("{0} {1}", message, filesize));
                System.ConsoleColor console = Console.ForegroundColor;

#if _DEVELOPER_
                ConsoleExt.PrependLine(message);
#else
            Console.Write(message);
#endif
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(filesize);
                Console.ForegroundColor = console;
            }
            catch { }
        }

        public static void errorlog(string op, string error)
        {
            try
            {
                // Debugger Friendly
                if (null != ApplicationBase.Instance)
                    ApplicationBase.Instance.WriteToLogFile(string.Format("{0} {1}", op, error));

#if CONSOLE
                System.ConsoleColor console = Console.ForegroundColor;

#if _DEVELOPER_
                ConsoleExt.PrependLine(op);
#else
                Console.Write(op);
#endif
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(error);
                Console.ForegroundColor = console;
#endif
            }
            catch { }

        }


        //static ConsoleProgressBar pbar = new ConsoleProgressBar();
        //public static void relogProgress(string text, int progress)
        //{
        //    //log(text);
        //    //return;
        //    int bufferWidth = Console.BufferWidth;
        //    int left = Console.CursorLeft;
        //    int top = Console.CursorTop;
        //    if (Console.CursorTop > 0)
        //        Console.SetCursorPosition(0, Console.CursorTop - 1);

        //    text = string.Format(" {0}", text);


        //    Console.Write(text, bufferWidth - left);
        //    pbar.Report(progress);
        //    ConsoleUtil.log();
        //}

        public static void progress(int progress)
        {
            //pbar.Report(progress);
        }
    }

    /// <summary>
    /// An ASCII progress bar
    /// </summary>
    public class ConsoleProgressBar : IDisposable, IProgress<double>
    {
        private const int blockCount = 10;
        private readonly TimeSpan animationInterval = TimeSpan.FromSeconds(1.0 / 8);
        private const string animation = @"|/-\";

        private readonly Timer timer;

        private double currentProgress = 0;
        private string currentText = string.Empty;
        private bool disposed = false;
        private int animationIndex = 0;

        public ConsoleProgressBar()
        {
            timer = new Timer(TimerHandler);

            // A progress bar is only for temporary display in a console window.
            // If the console output is redirected to a file, draw nothing.
            // Otherwise, we'll end up with a lot of garbage in the target file.
            if (!Console.IsOutputRedirected)
            {
                ResetTimer();
            }
        }

        public void Report(double value)
        {
            // Make sure value is in [0..1] range
            value = Math.Max(0, Math.Min(1, value));
            Interlocked.Exchange(ref currentProgress, value);
        }

        private void TimerHandler(object state)
        {
            lock (timer)
            {
                if (disposed) return;

                int progressBlockCount = (int)(currentProgress * blockCount);
                int percent = (int)(currentProgress * 100);
                string text = string.Format("[{0}{1}] {2,3}% {3}",
                    new string('#', progressBlockCount), new string('-', blockCount - progressBlockCount),
                    percent,
                    animation[animationIndex++ % animation.Length]);
                UpdateText(text);

                ResetTimer();
            }
        }

        private void UpdateText(string text)
        {
            // Get length of common portion
            int commonPrefixLength = 0;
            int commonLength = Math.Min(currentText.Length, text.Length);
            while (commonPrefixLength < commonLength && text[commonPrefixLength] == currentText[commonPrefixLength])
            {
                commonPrefixLength++;
            }

            // Backtrack to the first differing character
            StringBuilder outputBuilder = new StringBuilder();
            outputBuilder.Append('\b', currentText.Length - commonPrefixLength);

            // Output new suffix
            outputBuilder.Append(text.Substring(commonPrefixLength));

            // If the new text is shorter than the old one: delete overlapping characters
            int overlapCount = currentText.Length - text.Length;
            if (overlapCount > 0)
            {
                outputBuilder.Append(' ', overlapCount);
                outputBuilder.Append('\b', overlapCount);
            }

            Console.Write(outputBuilder);
            currentText = text;
        }

        private void ResetTimer()
        {
            timer.Change(animationInterval, TimeSpan.FromMilliseconds(-1));
        }

        public void Dispose()
        {
            lock (timer)
            {
                disposed = true;
                UpdateText(string.Empty);
            }
        }

    }
}
