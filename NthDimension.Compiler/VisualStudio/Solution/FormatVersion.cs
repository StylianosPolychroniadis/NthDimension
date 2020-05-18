namespace NthStudio.Compiler.VisualStudio.Solution
{
    using System;
    public struct FormatVersion
    {
        public int                  Major           { get; set; }
        public int                  Minor           { get; set; }
        public const char           SeparatorChar   = '.';
        public FormatVersion        Parse(string str)
        {
            FormatVersion ret = new FormatVersion();

            // TODO:: Regex Check for non {0}.{1} structure

            string[] str2numerals = str.Split(FormatVersion.SeparatorChar);
            if (str2numerals.Length != 2)
                throw new Exception(string.Format("FormatVersion wrong number of arguments {0}", str));

            try
            {
                ret.Major = int.Parse(str2numerals[0]);
                ret.Minor = int.Parse(str2numerals[1]);
            }
            catch (Exception e)
            {
                throw e;                // TODO:: Improve error handling
            }
            return ret;
        }
        public override string      ToString()
        {
            return string.Format("{0}{1}{2}", Major, SeparatorChar, Minor);
        }
    }
}
