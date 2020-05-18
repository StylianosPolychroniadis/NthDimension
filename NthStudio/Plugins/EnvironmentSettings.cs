using System;
using System.IO;

namespace NthStudio.Plugins
{
    public static class EnvironmentSettings
    {
        public static string GetFullPath(string path)
        {
            //return AppDomain.CurrentDomain.BaseDirectory + path;
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
        }
    }
}
