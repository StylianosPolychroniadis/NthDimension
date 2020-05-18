using System;

namespace NthStudio.Utilities
{
    public static class PathUtil
    {
        public static string AdaptPathSeparator(string path)
        {
            if (System.Environment.OSVersion.Platform != PlatformID.Unix)
                return path.Replace('/', '\\');
            else
                return path;
        }

        public static string MergePaths(string leftPath, string rightPath)
        {
            char sep = System.IO.Path.DirectorySeparatorChar;
            string sepStr = sep.ToString();

            leftPath = leftPath.TrimEnd(new char[] { ' ' });
            rightPath = rightPath.TrimEnd(new char[] { ' ' });

            if (!leftPath.EndsWith(sepStr, StringComparison.InvariantCulture))
                leftPath = leftPath + sepStr;

            rightPath = rightPath.TrimStart(new char[] { ' ', sep, '.' });

            return leftPath + rightPath;
        }
    }
}
