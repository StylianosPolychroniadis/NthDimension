using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Text
{
    public static class StringExtensions
    {
        //http://stackoverflow.com/questions/4135317/make-first-letter-of-a-string-upper-case-for-maximum-performance
        /// <summary>
        /// Capitalize the first character of a string
        /// </summary>
        /// <param name="string"></param>
        /// <returns></returns>
        public static string FirstCharToUpper(this string @string)
        {
            if (String.IsNullOrEmpty(@string))
                throw new ArgumentException("There is no first letter");
            char[] chars = @string.ToCharArray();
            chars[0] = char.ToUpper(chars[0]);
            return new string(chars);
        }
    }
}
