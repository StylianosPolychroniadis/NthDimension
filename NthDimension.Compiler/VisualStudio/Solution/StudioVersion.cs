using System;

namespace NthStudio.Compiler.VisualStudio.Solution
{
    public struct StudioVersion
    {
        public int          Major { get; set; }
        public int          Minor { get; set; }
        public int          Build { get; set; }
        public int          BuildRevision { get; set; }

        public const char   SeparatorChar = '.';
        public StudioVersion Parse(string str)
        {
            StudioVersion ret = new StudioVersion();

            // TODO:: Regex Check for non {0}.{1} structure

            string[] str4numerals = str.Split(FormatVersion.SeparatorChar);
            if (str4numerals.Length != 2)
                throw new Exception(string.Format("FormatVersion wrong number of arguments {0}", str));

            try
            {
                ret.Major           = int.Parse(str4numerals[0]);
                ret.Minor           = int.Parse(str4numerals[1]);
                ret.Build           = int.Parse(str4numerals[2]);
                ret.BuildRevision   = int.Parse(str4numerals[3]);
            }
            catch (Exception e)
            {
                throw e;    // TODO:: Improve error handling
            }
            return ret;
        }
        public override string ToString()
        {
            return string.Format("{0}{1}{2}", Major, SeparatorChar, Minor);
        }
    }
}
