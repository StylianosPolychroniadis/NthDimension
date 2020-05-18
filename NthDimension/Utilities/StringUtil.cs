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

using System.Text.RegularExpressions;

namespace NthDimension.Utilities
{
    using System.Collections.Generic;
    using System.Linq;

    public static class StringUtil
    {
        public static string CharEvent(string input, ref int characterPosition, uint key)
        {
            string caption = string.IsNullOrEmpty(input) ? string.Empty : input;

            try
            {
                string addChar = ((char)key).ToString();

                caption = caption.Insert(characterPosition++, addChar);
            }
            catch/* (System.ArgumentOutOfRangeException e)*/
            {

            }

            return caption;
        }
        public static string KeyEvent(string input, ref int characterPosition, int key, int scan = 0, int action = 0, int modifiers = 0)
        {
            string caption = input;



            //if (key == 53 && characterPosition - 1 >= 0 && caption.Length > 0)                              // backspace
            //    caption = caption.Remove(--characterPosition, 1);
            //if (key == 55 && (characterPosition) >= 0 && characterPosition < caption.Length)                // delete
            //    caption = caption.Remove(characterPosition, 1);
            //if (key == 47 && characterPosition - 1 >= 0)                                                    // left arrow
            //    characterPosition--;
            //if (key == 48 && characterPosition < caption.Length)                                            // right arrow
            //    characterPosition++;
            //if (key == 59)                                                                                  // End
            //    characterPosition = caption.Length;
            //if (key == 58)                                                                                  // Home
            //    characterPosition = 0;

            if (key == 8 && characterPosition - 1 >= 0 && caption.Length > 0)                              // backspace
                caption = caption.Remove(--characterPosition, 1);
            if (key == 46 && (characterPosition) >= 0 && characterPosition < caption.Length)                // delete
                caption = caption.Remove(characterPosition, 1);
            if (key == 37 && characterPosition - 1 >= 0)                                                    // left arrow
                characterPosition--;
            if (key == 39 && characterPosition < caption.Length)                                            // right arrow
                characterPosition++;
            if (key == 35)                                                                                  // End
                characterPosition = caption.Length;
            if (key == 36)                                                                                  // Home
                characterPosition = 0;

            return caption;
        }
        public static IEnumerable<string> Split(string str, int chunkSize)
        {
            return Enumerable.Range(0, str.Length / chunkSize)
                .Select(i => str.Substring(i * chunkSize, chunkSize));
        }
        public static bool StringIsUrl(string url)
        {
            //return Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute);
            ////string pattern = @"^(http|https|ftp|)\://|[a-zA-Z0-9\-\.]+\.[a-zA-Z](:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]$";
            string pattern = @"((([A-Za-z]{3,9}:(?:\/\/)?)(?:[\-;:&=\+\$,\w]+@)?[A-Za-z0-9\.\-]+|(?:www\.|[\-;:&=\+\$,\w]+@)[A-Za-z0-9\.\-]+)((?:\/[\+~%\/\.\w\-_]*)?\??(?:[\-\+=&;%@\.\w_]*)#?(?:[\.\!\/\\\w]*))?)";
            Regex reg = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return reg.IsMatch(url) && url.Count(f => f == '.') >= 2;




        }
    }

    
}
