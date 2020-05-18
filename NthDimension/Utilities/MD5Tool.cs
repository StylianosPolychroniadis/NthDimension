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
using System.Security.Cryptography;

namespace NthDimension.Utilities
{
    public class MD5Tool
    {
        public static string GetChecksum(string file)
        {
            //using (FileStream stream = File.OpenRead(file))
            using (var stream = new BufferedStream(File.OpenRead(file), 32 * 1024 * 1024))
            {
                var sha = new SHA256Managed();
                byte[] checksum = sha.ComputeHash(stream);
                return BitConverter.ToString(checksum).Replace("-", String.Empty);
            }
        }

        public static string GetChecksumBuffered(Stream stream)
        {
            using (var bufferedStream = new BufferedStream(stream, 1024 * 32))
            {
                var sha = new SHA256Managed();
                byte[] checksum = sha.ComputeHash(bufferedStream);
                return BitConverter.ToString(checksum).Replace("-", String.Empty);
            }
        }

        public static DateTime GetFileTime(string filename)
        {
            DateTime now = DateTime.Now;
            TimeSpan localOffset = now - now.ToUniversalTime();
            return File.GetLastWriteTimeUtc(filename) + localOffset;
        }

        public static DateTime GetFileTime(FileInfo fi)
        {
            TimeSpan localOffset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
            return fi.LastWriteTimeUtc + localOffset;
        }

        public static DateTime GetLastFileTime(string fileFullPath)
        {
            TimeSpan localOffset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
            return (new FileInfo(fileFullPath)).LastWriteTimeUtc + localOffset;
        }
    }
}
