using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace System.IO
{
    public static class DirectoryAlternative
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        struct WIN32_FIND_DATA
        {
            public uint dwFileAttributes;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
            public uint nFileSizeHigh;
            public uint nFileSizeLow;
            public uint dwReserved0;
            public uint dwReserved1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string cFileName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            public string cAlternateFileName;
            public uint dwFileType;
            public uint dwCreatorType;
            public ushort wFinderFlags;
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool FindClose(IntPtr hFindFile);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr FindFirstFile(string lpFileName, out WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool FindNextFile(IntPtr hFindFile, out WIN32_FIND_DATA lpFindFileData);

        private enum EntryType { All = 0, Directories = 1, Files = 2 };

        public static IEnumerable<string> EnumerateDirectories(string path)
        {
            return EnumerateDirectories(path, "*", SearchOption.TopDirectoryOnly);
        }

        public static IEnumerable<string> EnumerateDirectories(string path, string searchPattern)
        {
            return EnumerateDirectories(path, searchPattern, SearchOption.TopDirectoryOnly);
        }

        public static IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption)
        {
            CheckErrors(path, searchPattern);
            List<string> retValue = new List<string>();
            Enumerate(path, searchPattern, searchOption, ref retValue, EntryType.Directories);
            return retValue;
        }

        public static IEnumerable<string> EnumerateFiles(string path)
        {
            return EnumerateFiles(path, "*", SearchOption.TopDirectoryOnly);
        }

        public static IEnumerable<string> EnumerateFiles(string path, string searchPattern)
        {
            return EnumerateFiles(path, searchPattern, SearchOption.TopDirectoryOnly);
        }

        public static IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption)
        {
            CheckErrors(path, searchPattern);
            List<string> retValue = new List<string>();
            Enumerate(path, searchPattern, searchOption, ref retValue, EntryType.Files);
            return retValue;
        }

        public static IEnumerable<string> EnumerateFileSystemEntries(string path)
        {
            return EnumerateFileSystemEntries(path, "*", SearchOption.TopDirectoryOnly);
        }

        public static IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern)
        {
            return EnumerateFileSystemEntries(path, searchPattern, SearchOption.TopDirectoryOnly);
        }

        public static IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern, SearchOption searchOption)
        {
            CheckErrors(path, searchPattern);
            List<string> retValue = new List<string>();
            Enumerate(path, searchPattern, searchOption, ref retValue, EntryType.All);
            return retValue;
        }

        public static string[] GetFiles(string path)
        {
            return EnumerateFiles(path).ToArray<string>();
        }

        public static string[] GetFiles(string path, string searchPattern)
        {
            return EnumerateFiles(path, searchPattern).ToArray<string>();
        }

        public static string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
        {
            return EnumerateFiles(path, searchPattern, searchOption).ToArray<string>();
        }

        public static string[] GetDirectories(string path)
        {
            return EnumerateDirectories(path).ToArray<string>();
        }

        public static string[] GetDirectories(string path, string searchPattern)
        {
            return EnumerateDirectories(path, searchPattern).ToArray<string>();
        }

        public static string[] GetDirectories(string path, string searchPattern, SearchOption searchOption)
        {
            return EnumerateDirectories(path, searchPattern, searchOption).ToArray<string>();
        }

        public static string[] GetFileSystemEntries(string path)
        {
            return EnumerateFileSystemEntries(path).ToArray<string>();
        }

        public static string[] GetFileSystemEntries(string path, string searchPattern)
        {
            return EnumerateFileSystemEntries(path, searchPattern).ToArray<string>();
        }

        public static string[] GetFileSystemEntries(string path, string searchPattern, SearchOption searchOption)
        {
            return EnumerateFileSystemEntries(path, searchPattern, searchOption).ToArray<string>();
        }

        private static void CheckErrors(string path, string searchPattern)
        {
            if (path.Trim() == "") throw new ArgumentException();
            new DirectoryInfo(path);
            if (searchPattern == null) throw new ArgumentNullException();
            if (!System.IO.Directory.Exists(path)) throw new DirectoryNotFoundException();
            try
            {
                FileInfo fi = new FileInfo(path);
                if (fi.Length > 0) throw new IOException();
            }
            catch
            {
            }
        }

        private static void Enumerate(string path, string searchPattern, SearchOption searchOption, ref List<string> retValue, EntryType entryType)
        {
            WIN32_FIND_DATA findData;
            if (path.Last<char>() != '\\') path += "\\";
            AdjustSearchPattern(ref path, ref searchPattern);
            searchPattern = searchPattern.Replace("*.*", "*");
            Text.RegularExpressions.Regex rx = new Text.RegularExpressions.Regex(
                "^" +
                Text.RegularExpressions.Regex.Escape(path) +
                Text.RegularExpressions.Regex.Escape(searchPattern)
                .Replace("\\*", ".*")
                .Replace("\\?", ".")
                + "$"
                , Text.RegularExpressions.RegexOptions.IgnoreCase);
            IntPtr hFile = FindFirstFile(path + "*", out findData);
            //IntPtr hFileFirst = hFile;
            List<string> subDirs = new List<string>();
            
            if (hFile.ToInt64() != -1)  // BUG in original codeproject article ToInt32() changed to ToInt64()   // https://www.codeproject.com/Articles/1383832/System-IO-Directory-Alternative-using-WinAPI
            {
                do
                {
                    if (findData.cFileName == "." || findData.cFileName == "..") continue;
                    if ((findData.dwFileAttributes & (uint)FileAttributes.Directory) == (uint)FileAttributes.Directory)
                    {
                        subDirs.Add(path + findData.cFileName);
                        if ((entryType == EntryType.Directories || entryType == EntryType.All) && rx.IsMatch(path + findData.cFileName)) retValue.Add(path + findData.cFileName);
                    }
                    else
                    {
                        if ((entryType == EntryType.Files || entryType == EntryType.All) && rx.IsMatch(path + findData.cFileName)) retValue.Add(path + findData.cFileName);
                    }

                    //if (hFileFirst == hFile) break;

                } while (FindNextFile(hFile, out findData));
                if (searchOption == SearchOption.AllDirectories)
                    foreach (string subdir in subDirs)
                        Enumerate(subdir, searchPattern, searchOption, ref retValue, entryType);
            }
            FindClose(hFile);
        }

        private static void AdjustSearchPattern(ref string path, ref string searchPattern)
        {
            if (path.Last<char>() != '\\') path += "\\";
            if (searchPattern.Contains("\\"))
            {
                path = (path + searchPattern).Substring(0, (path + searchPattern).LastIndexOf("\\") + 1);
                searchPattern = searchPattern.Substring(searchPattern.IndexOf("\\") + 1);
            }
            if (searchPattern == "*.*")
                searchPattern = "*";
        }
    }
}
