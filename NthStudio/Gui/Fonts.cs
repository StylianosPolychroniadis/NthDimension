using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NthDimension.Rasterizer.NanoVG;

namespace NthStudio.Gui
{
    public static class Fonts
    {
        private static readonly string m_resourcesPath = "data/fonts/";
        private static Dictionary<string, int> m_fontMap = new Dictionary<string, int>();


        public static void Load(NVGcontext ctx, string fontName, string fileName)
        {
            string filePath = m_resourcesPath + fileName;
            int fontHandle = NanoVG.nvgCreateFont(ctx, fontName, filePath);
            m_fontMap[fontName] = fontHandle;
        }

        public static int Get(string name)
        {
            int ret = -1;
            if (!m_fontMap.TryGetValue(name, out ret))
                return ret;
            return ret;
        }

        private static Dictionary<int, byte[]> s_IconMap = new Dictionary<int, byte[]>();
        static readonly byte[] icon = new byte[8];

        public static byte[] GetIconUTF8(int iconId)
        {
            byte[] ret = null;
            if (!s_IconMap.TryGetValue(iconId, out ret))
            {
                ret = UnicodeToUTF8(iconId);
                s_IconMap.Add(iconId, ret);
            }
            return ret;
        }

        /// <summary>
        /// Unicode code point to UTF8. (mysterious code)
        /// </summary>
        /// <returns>UTF8 string of the unicode.</returns>
        /// <param name="cp">code point.</param>
        private static byte[] UnicodeToUTF8(int cp)
        {
            int n = 0;
            if (cp < 0x80)
                n = 1;
            else if (cp < 0x800)
                n = 2;
            else if (cp < 0x10000)
                n = 3;
            else if (cp < 0x200000)
                n = 4;
            else if (cp < 0x4000000)
                n = 5;
            else if (cp <= 0x7fffffff)
                n = 6;
            icon[n] = (byte)'\0';
            switch (n)
            {
                case 6:
                    goto case_6;
                case 5:
                    goto case_5;
                case 4:
                    goto case_4;
                case 3:
                    goto case_3;
                case 2:
                    goto case_2;
                case 1:
                    goto case_1;
            }
            goto end;

        case_6:
            icon[5] = (byte)(0x80 | (cp & 0x3f));
            cp = cp >> 6;
            cp |= 0x4000000;
        case_5:
            icon[4] = (byte)(0x80 | (cp & 0x3f));
            cp = cp >> 6;
            cp |= 0x200000;
        case_4:
            icon[3] = (byte)(0x80 | (cp & 0x3f));
            cp = cp >> 6;
            cp |= 0x10000;
        case_3:
            icon[2] = (byte)(0x80 | (cp & 0x3f));
            cp = cp >> 6;
            cp |= 0x800;
        case_2:
            icon[1] = (byte)(0x80 | (cp & 0x3f));
            cp = cp >> 6;
            cp |= 0xc0;
        case_1:
            icon[0] = (byte)cp;

        end:

            byte[] ret = new byte[n];
            Array.Copy(icon, ret, n);
            return ret;
        }
    }
}
