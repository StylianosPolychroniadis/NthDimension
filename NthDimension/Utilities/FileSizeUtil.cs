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
using System.Net;
using System.Reflection;

namespace NthDimension.Utilities
{
    public class FileSizeUtil
    {
        static string[] sizeAbbrv = { "B", "KB", "MB", "GB", "TB" };
        public static string BytesToSizeReadable(long bytes)
        {
            int order = 0;
            while (bytes >= 1024 && order < sizeAbbrv.Length - 1)
            {
                order++;
                bytes = bytes / 1024;
            }

            // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
            // show a single decimal place, and no space.
            return String.Format("{0:0.##} {1}", bytes, sizeAbbrv[order]);

        }
        public static string GetFileSizeReadable(string filename)
        {

            long len = new FileInfo(filename).Length;
            int order = 0;
            while (len >= 1024 && order < sizeAbbrv.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
            // show a single decimal place, and no space.
            return String.Format("{0:0.##} {1}", len, sizeAbbrv[order]);
        }
        public static long GetFileSize(string filename)
        {
            return new FileInfo(filename).Length;
        }
        public static long GetFileSize(Uri uriPath)
        {
            long ret = -1L;
            int retryAttempts = 0;
            ServicePointManager.ServerCertificateValidationCallback = (s, cert, chain, ssl) => true;                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            retry:
            if (retryAttempts > 3) throw new Exception(string.Format("GetFileSize {0} exceeded maxc retry attempts", uriPath.AbsolutePath));
            HttpWebResponse resp;
            var webRequest = (HttpWebRequest)WebRequest.Create(uriPath);
            webRequest.Method = "HEAD";
            //webRequest.Timeout = 6000; // 100000 mSec originally
            webRequest.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
            webRequest.MaximumAutomaticRedirections = 10;
            webRequest.AllowAutoRedirect = true;

            try
            {

                using (var webResponse = webRequest.GetResponse())
                {
                   
                    long.TryParse(webResponse.Headers.Get("Content-Length"), out ret);
                    return ret;
                }
            }
            catch(WebException e)
            {
                resp = (HttpWebResponse)e.Response;
                uriPath = new Uri(resp.GetResponseHeader("Location"));       // Assuming 308 or 301 Redirect Errors

                retryAttempts++;
              
                goto retry;
            }
#pragma warning disable CS0162
            return ret;
        }
        

        // TODO:: Remove duplicate function
        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

    }
}
