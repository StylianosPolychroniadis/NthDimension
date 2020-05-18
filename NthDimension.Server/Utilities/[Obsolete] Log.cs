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
using System.IO;

namespace NthDimension.Server.Utilities
{ 
    /// <summary>
    /// Console Logger
    /// </summary>
    public abstract class Log
    {
        private static StreamWriter _logFileStream;

        private static string _serviceName = string.Empty;

        public static void Init(string serviceName)
        {

            _serviceName = serviceName;

            if (Environment.UserInteractive)
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.Clear();

                if (!Directory.Exists("logs"))
                    Directory.CreateDirectory("logs");

                _logFileStream = new StreamWriter(File.Open("logs/" + DateTime.Now.ToFileTime() + ".log", FileMode.Create,
                        FileAccess.Write))
                { AutoFlush = true };
            }


        }

        public static void Info(string message, params object[] arg)
        {
            LogMessage(ConsoleColor.Blue, " [Info] ", message, arg);
        }

        public static void Warn(string message, params object[] arg)
        {
            LogMessage(ConsoleColor.Yellow, " [Warning] ", message, arg);
        }

        public static void Error(string message, params object[] arg)
        {
            LogMessage(ConsoleColor.Red, " [Error] ", message, arg);
        }

        public static void Severe(string message, params object[] arg)
        {
            LogMessage(ConsoleColor.DarkRed, " [Severe] ", message, arg);
        }

        public static void LogMessage(ConsoleColor color, string prefix, string message, params object[] arg)
        {


            if (message == null)
                return;

            if (arg == null)
                return;

            if (Environment.UserInteractive)
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write(DateTime.Now);
                Console.ForegroundColor = color;
                Console.Write(prefix);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                try { Console.WriteLine(message, arg); } catch { }
                Console.ResetColor();
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.DarkGray;

                try { _logFileStream.WriteLine(DateTime.Now + prefix + String.Format(message, arg)); } catch { }
            }
            else
            {
                // NOTE:: Writting to event log is very slow on Windows Server 2016 causing the entire application to perform extremelly slow when running as a Windows Service

                //System.Diagnostics.EventLog tempLog = new System.Diagnostics.EventLog("Application", ".", _serviceName);
                //tempLog.WriteEntry(string.Format(message, arg));
                //tempLog.Close();
            }



        }
    }
}
