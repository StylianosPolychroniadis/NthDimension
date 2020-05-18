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
using System.Drawing;
using System.Reflection;
using System.Collections.Generic;

using WHUD = NthDimension.Forms.Widget.WHUD;
using NthDimension.Rasterizer.NanoVG;

namespace NthDimension.Forms
{
    public class NanoFont
    {
        const float testFontSize = 12f;

        public NanoFont(NanoFont font, float emSize)
        {
            InternalName = font.InternalName;
            this.FileName = font.FileName;
            Id = -1;

            float h = GetFontHeight(emSize);
            FontParameters fp = WHUD.LibContext.GetFontParemeters(font, font.Id, h);
            SetFontParams(fp);
        }

        public NanoFont(string internalFontName, string fileName)
            : this(internalFontName, fileName, testFontSize)
        {
        }

        public NanoFont(string internalFontName, string fileName, float emSize = testFontSize)
        {
            InternalName = internalFontName;
            this.FileName = fileName;
            Height = GetFontHeight(emSize);
            Id = -1;

            FontParameters fp = WHUD.LibContext.CreateFont(this);
            SetFontParams(fp);
        }

        public NanoFont(string internalFontName, Stream strm, float emSize = testFontSize)
        {
            InternalName = internalFontName;
            Height = GetFontHeight(emSize);
            Id = -1;
            FontData = new byte[strm.Length];
            strm.Read(FontData, 0, FontData.Length);
            strm.Dispose();

            FontParameters fp = WHUD.LibContext.CreateFont(this);
            SetFontParams(fp);
        }

        void SetFontParams(FontParameters fp)
        {
            Id = fp.ID;
            Height = fp.FontHeight;
            LineHeight = fp.LineHeight;
            Ascender = fp.Ascender;
            Descender = fp.Descender;
        }

        public int Id
        {
            get;
            internal set;
        }

        public string InternalName
        {
            get;
            private set;
        }

        public string FileName
        {
            get;
            private set;
        }

        public byte[] FontData
        {
            get;
            private set;
        }

        /// <summary>
        /// Size in Points (em size)
        /// </summary>

        private float m_fsize;
        public float FontSize
        {
            get { return m_fsize; }
            //private set;
            set { m_fsize = value; }
        }

        private float GetFontHeight(float emSize)
        {
            if (System.Math.Abs(emSize - FontSize) < float.Epsilon)
                return Height;

            FontSize = emSize;

            // There are 72 points per inch.
            // default 96 pixels per inch. (default 96 dpi)
            // points = pixels * 72 / 96
            // 
            // float point = (16 * 72) / 96;
            // float pixels = (point * 96) / 72;

            if (Environment.OSVersion.Platform == PlatformID.Unix)
                emSize += 2.49f;
            else
                emSize += 1.5f;

            return (emSize * 96) / 72;
        }

        /*
		public float FontSizePixels
		{
			get;
			private set;
		}*/

        /// <summary>
        /// Vertical font height in pixels
        /// </summary>
        /// <returns>The height.</returns>
        public float GetHeight()
        {
            return Height;
        }

        public float Ascender
        {
            get;
            private set;
        }

        public float Descender
        {
            get;
            private set;
        }

        /// <summary>
        /// Vertical font height in pixels
        /// </summary>
        /// <value>The height.</value>
        public float Height
        {
            get;
            private set;
        }

        /// <summary>
        /// Line Height in Pixels
        /// </summary>
        /// <value>The height of the line.</value>
        public float LineHeight
        {
            get;
            private set;
        }

        #region Constants-Fonts

        //static NanoFont TerminalRegular_;
        //static NanoFont TerminalRegularBold_;
        //static NanoFont TerminalItalic_;
        //static NanoFont TerminalItalicBold_;
        //static NanoFont DefaultLight_;
        static NanoFont DefaultRegular_;
        static NanoFont DefaultRegularBold_;
        static NanoFont DefaultItalic_;
        static NanoFont DefaultItalicBold_;

      


        static NanoFont DefaultIcons_;

        //public static NanoFont DefaultLight
        //{
        //    get
        //    {
        //        if (DefaultLight_ == null)
        //        {
        //            try
        //            {
        //                Assembly _assembly = Assembly.GetExecutingAssembly();
        //                Stream ttfStream = _assembly.GetManifestResourceStream("NthDimension.Forms.Resources.Roboto-Light.ttf");

        //                DefaultLight_ = new NanoFont("sans-mono-light", ttfStream);
        //            }
        //            catch (Exception ex)
        //            {
        //                throw new Exception("Can't create Default Font.", ex);
        //            }
        //        }
        //        return DefaultRegular_;
        //    }
        //}

        //public static NanoFont TerminalRegular
        //{
        //    get
        //    {
        //        if (TerminalRegular_ == null)
        //        {
        //            try
        //            {
        //                Assembly _assembly = Assembly.GetExecutingAssembly();
        //                Stream ttfStream = _assembly.GetManifestResourceStream("NthDimension.Forms.Resources.DejaVuSansMono.ttf");

        //                TerminalRegular_ = new NanoFont("sans-terminal-regular", ttfStream);
        //            }
        //            catch (Exception ex)
        //            {
        //                throw new Exception("Can't create Default Font.", ex);
        //            }
        //        }
        //        return TerminalRegular_;
        //    }
        //}
        //public static NanoFont TerminalRegularBold
        //{
        //    get
        //    {
        //        if (TerminalRegularBold_ == null)
        //        {
        //            try
        //            {
        //                Assembly _assembly = Assembly.GetExecutingAssembly();
        //                Stream ttfStream = _assembly.GetManifestResourceStream("NthDimension.Forms.Resources.DejaVuSansMono-Bold.ttf");

        //                TerminalRegularBold_ = new NanoFont("sans-terminal-regular-bold", ttfStream);
        //            }
        //            catch (Exception ex)
        //            {
        //                throw new Exception("Can't create Default Font.", ex);
        //            }
        //        }
        //        return TerminalRegularBold_;
        //    }
        //}
        //public static NanoFont TerminalItalic
        //{
        //    get
        //    {
        //        if (TerminalItalic_ == null)
        //        {
        //            try
        //            {
        //                Assembly _assembly = Assembly.GetExecutingAssembly();
        //                Stream ttfStream = _assembly.GetManifestResourceStream("NthDimension.Forms.Resources.DejaVuSansMono-Oblique.ttf");

        //                TerminalItalic_ = new NanoFont("sans-terminal-italic", ttfStream);
        //            }
        //            catch (Exception ex)
        //            {
        //                throw new Exception("Can't create Default Font.", ex);
        //            }
        //        }
        //        return TerminalItalic_;
        //    }
        //}
        //public static NanoFont TerminalItalicBold
        //{
        //    get
        //    {
        //        if (TerminalItalicBold_ == null)
        //        {
        //            try
        //            {
        //                Assembly _assembly = Assembly.GetExecutingAssembly();
        //                Stream ttfStream = _assembly.GetManifestResourceStream("NthDimension.Forms.Resources.DejaVuSansMono-ObliqueBold.ttf");

        //                TerminalItalicBold_ = new NanoFont("sans-terminal-italic-bold", ttfStream);
        //            }
        //            catch (Exception ex)
        //            {
        //                throw new Exception("Can't create Default Font.", ex);
        //            }
        //        }
        //        return TerminalItalicBold_;
        //    }
        //}

        public static NanoFont DefaultRegular
        {
            get
            {
                if (DefaultRegular_ == null)
                {
                    try
                    {
                        Assembly _assembly = Assembly.GetExecutingAssembly();
                        Stream ttfStream = _assembly.GetManifestResourceStream("NthDimension.Forms.Resources.Roboto-Regular.ttf");

                        DefaultRegular_ = new NanoFont("default-regular", ttfStream);
                        //DefaultRegular.FontSize = 14f;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Can't create Default Font.", ex);
                    }
                }
                return DefaultRegular_;
            }
        }
        public static NanoFont DefaultRegularBold
        {
            get
            {
                if (DefaultRegularBold_ == null)
                {
                    try
                    {
                        Assembly _assembly = Assembly.GetExecutingAssembly();
                        Stream ttfStream = _assembly.GetManifestResourceStream("NthDimension.Forms.Resources.Roboto-Bold.ttf");
                        //Stream ttfStream = _assembly.GetManifestResourceStream("NthDimension.Forms.Resources.SpaceAge-Regular.ttf");

                        DefaultRegularBold_ = new NanoFont("default-bold-regular", ttfStream);
                        //DefaultRegularBold.FontSize = 14f;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Can't create Default Regular Bold Font.", ex);
                    }
                }
                return DefaultRegularBold_;
            }
        }

        //public static NanoFont CalibriRegular
        //{
        //    get
        //    {
        //        if (CalibriRegular_ == null)
        //        {
        //            try
        //            {
        //                Assembly _assembly = Assembly.GetExecutingAssembly();
        //                Stream ttfStream = _assembly.GetManifestResourceStream("NthDimension.Forms.Resources.calibri.ttf");

        //                CalibriRegular_ = new NanoFont("calibri", ttfStream);
        //            }
        //            catch (Exception ex)
        //            {
        //                throw new Exception("Can't create Default Font.", ex);
        //            }
        //        }
        //        return CalibriRegular_;
        //    }
        //}

        //public static NanoFont PrimeRegular
        //{
        //    get
        //    {
        //        if (PrimeRegular_ == null)
        //        {
        //            try
        //            {
        //                Assembly _assembly = Assembly.GetExecutingAssembly();
        //                Stream ttfStream = _assembly.GetManifestResourceStream("NthDimension.Forms.Resources..PrimeRegular.ttf");

        //                PrimeRegular_ = new NanoFont("PrimeRegular", ttfStream);
        //            }
        //            catch (Exception ex)
        //            {
        //                throw new Exception("Can't create Prome Regular Font.", ex);
        //            }
        //        }
        //        return PrimeRegular_;
        //    }
        //}

     
        public static NanoFont DefaultItalic
        {
            get
            {
                if (DefaultItalic_ == null)
                {
                    try
                    {
                        Assembly _assembly = Assembly.GetExecutingAssembly();
                        Stream ttfStream = _assembly.GetManifestResourceStream("NthDimension.Forms.Resources.DroidSerif-Italic.ttf");

                        DefaultItalic_ = new NanoFont("sans-mono-italic", ttfStream);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Can't create Default Italic Font.", ex);
                    }
                }
                return DefaultItalic_;
            }
        }
        public static NanoFont DefaultItalicBold
        {
            get
            {                
                if (DefaultItalicBold_ == null)
                {
                    try
                    {
                        Assembly _assembly = Assembly.GetExecutingAssembly();
                        Stream ttfStream = _assembly.GetManifestResourceStream("NthDimension.Forms.Resources.DroidSerif-Italic.ttf");

                        DefaultItalicBold_ = new NanoFont("sans-mono-italic-bold", ttfStream);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Can't create Default Font.", ex);
                    }
                }
                return DefaultItalicBold_;
            }
        }

        public static NanoFont DefaultIcons
        {
            get
            {
                if (DefaultIcons_ == null)
                {
                    try
                    {
                        Assembly _assembly = Assembly.GetExecutingAssembly();
                        Stream ttfStream = _assembly.GetManifestResourceStream("NthDimension.Forms.Resources.entypo.ttf");

                        DefaultIcons_ = new NanoFont("entypo", ttfStream);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Can't create Default Icons Font.", ex);
                    }
                }
                return DefaultIcons_;
            }
        }

        #endregion Constants-Fonts

        #region Glyphs
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
        public static byte[] UnicodeToUTF8(int cp)
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
        #endregion

        #region Measurements
        public static Size MeasureText(NVGcontext vg, string text, NanoFont font)
        {
            if (!NanoVG.FontCreated(vg, font.Id))
                throw new Exception(string.Format("Font '{0}', not created", font));

            var bounds = new float[4];


            NanoVG.nvgSave(vg);

            NanoVG.nvgFontSize(vg, font.Height);
            NanoVG.nvgFontFace(vg, font.Id);

            NanoVG.nvgTextBounds(vg, 0, 0, text, bounds);

            NanoVG.nvgRestore(vg);

            var s = new Size((int)(bounds[2] - bounds[0]), (int)(bounds[3] - bounds[1]));

            return s;
        }
        public static Size MeasureGlyph(NVGcontext vg, byte[] text, NanoFont font)
        {
            return new Size(10, 32); // OpenGL State gets corrupted. We are using a fixed value for now

            if (!NanoVG.FontCreated(vg, font.Id))
                throw new Exception(string.Format("Font '{0}', not created", font));

            var bounds = new float[4];


            NanoVG.nvgSave(vg);

            NanoVG.nvgFontSize(vg, font.FontSize);
            NanoVG.nvgFontFace(vg, font.Id);

            NanoVG.nvgTextBounds(vg, 0, 0, text, bounds);

            NanoVG.nvgRestore(vg);

            var s = new Size((int)(bounds[2] - bounds[0]), (int)(bounds[3] - bounds[1]));

            return s;
        }

        public static float MeasureTextWidth(NVGcontext vg, string text, NanoFont font)
        {
            if (!NanoVG.FontCreated(vg, font.Id))
                throw new Exception(String.Format("Font '{0}', not created", font.ToString()));

            NanoVG.nvgSave(vg);

            NanoVG.nvgFontSize(vg, font.Height);
            NanoVG.nvgFontFace(vg, font.Id);

            float textWidth = NanoVG.nvgTextBounds(vg, 0, 0, text);

            NanoVG.nvgRestore(vg);

            return textWidth;
        }
        #endregion
    }

    public struct FontParameters
    {
        public int ID;
        public float FontHeight;
        public float LineHeight;
        public float Ascender;
        public float Descender;
    }
}
