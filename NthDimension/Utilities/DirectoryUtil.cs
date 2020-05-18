using NthDimension.Utilities;
using System;

namespace NthDimension.Utilities
{


    public static class DirectoryUtil
    {
        static string _readonlyAssemblyDir;
        public static string AssemblyDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(_readonlyAssemblyDir))
                {
                    string codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                    UriBuilder uri = new UriBuilder(codeBase);
                    string path = Uri.UnescapeDataString(uri.Path);
                    return _readonlyAssemblyDir = System.IO.Path.GetDirectoryName(path);
                }

                return _readonlyAssemblyDir;
            }
        }

        static string initialFolder = KnownFoldersUtil.GetPath(KnownFolder.LocalAppData);

        public static string AppData
        {
            get { return initialFolder; }
            set
            {
                initialFolder = value;

                if (!System.IO.Directory.Exists(initialFolder))
                    System.IO.Directory.CreateDirectory(initialFolder);
            }
        }

        public static string AppData_Profiles
        {
            // TODO:: Create Dir and use accordingly
            get
            {
                string pathChat = System.IO.Path.Combine(AppData, @"MySoci\Profiles");

                if (!System.IO.Directory.Exists(pathChat))
                    System.IO.Directory.CreateDirectory(pathChat);

                return pathChat;
            }
        }

        public static string AppData_Temporary
        {
            get
            {
                string pathTemp = System.IO.Path.Combine(AppData, @"MySoci\tmp");

                if (!System.IO.Directory.Exists(pathTemp))
                    System.IO.Directory.CreateDirectory(pathTemp);

                return pathTemp;
            }
        }

        public static string AppData_User
        {
            // TODO:: Create Dir and use accordingly
            get
            {
                string pathUser = System.IO.Path.Combine(AppData, @"MySoci\User");

                if (!System.IO.Directory.Exists(pathUser))
                    System.IO.Directory.CreateDirectory(pathUser);

                return pathUser;
            }
        }

        public static string Documents
        {
            get
            {
                string path = System.IO.Path.Combine(KnownFoldersUtil.GetPath(KnownFolder.Documents), System.Reflection.Assembly.GetExecutingAssembly().GetName(false).Name);

                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);

                return path;
            }
        }

        public static string Documents_Temporary
        {
            get
            {
                string path = System.IO.Path.Combine(Documents, "_temp");

                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);

                return path;
            }
        }

        public static string Documents_Cache
        {
            get
            {
                string path = System.IO.Path.Combine(Documents, "cache");

                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);

                return path;
            }
        }

        public static string Documents_Extracted
        {
            get
            {
                string path = System.IO.Path.Combine(Documents, "extracted");

                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);

                return path;
            }
        }
    }
}
