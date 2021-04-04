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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Rendering.Utilities
{
    /// <summary>
    /// Color Channels
    /// </summary>
    public enum enuColorChannel : int
    {
        /// <summary>
        /// Red Channel
        /// </summary>
        R = 0x01,

        /// <summary>
        /// Green Channel
        /// </summary>
        G = 0x02,

        /// <summary>
        /// Blue Channel
        /// </summary>
        B = 0x03,

        /// <summary>
        /// Alpha Channel (Transparency)
        /// </summary>
        A = 0x04,
    }

    /// <summary>
    /// Normal Map Converter
    /// </summary>
    public class NormalGenerator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public NormalGenerator()
        {
            /* DO_NOTHING */
        }

        /// <summary>
        /// Height Data
        /// </summary>
        private double[,] HeightData { get; set; }

        /// <summary>
        /// Input Image
        /// </summary>
        private Bitmap Image { get; set; }

        /// <summary>
        /// Normal Output Image 
        /// </summary>
        private Bitmap NormalMap { get; set; }

        /// <summary>
        /// Load Image
        /// </summary>
        /// <param name="filename">Input filename</param>
        /// <param name="inputChannel">Input color channel</param>
        public void Load(string filename, int inputChannel)
        {
            Image = new Bitmap(filename);
            HeightData = new double[Image.Width, Image.Height];
            Color color = new Color();

            for (int x = 0; x < Image.Width; x++)
            {
                for (int y = 0; y < Image.Height; y++)
                {
                    color = Image.GetPixel(x, y);

                    switch (inputChannel)
                    {
                        case (int)enuColorChannel.R: { HeightData[x, y] = (double)color.R / 127.5; } break;
                        case (int)enuColorChannel.G: { HeightData[x, y] = (double)color.G / 127.5; } break;
                        case (int)enuColorChannel.B: { HeightData[x, y] = (double)color.B / 127.5; } break;
                        case (int)enuColorChannel.A: { HeightData[x, y] = (double)color.A / 127.5; } break;
                        default: { /* DO_NOTHING */ } break;
                    }
                }
            }
        }

        /// <summary>
        /// Create a Normal Map
        /// </summary>
        /// <param name="scale"></param>
        /// <returns></returns>
        public Bitmap CreateNormalMap(double scale)
        {
            NormalMap = (Bitmap)Image.Clone();
            Color color = new Color();

            for (int x = 0; x < Image.Width; x++)
            {
                for (int y = 0; y < Image.Height; y++)
                {
                    int i = 1, j = 1;
                    if (x == 0 || x == Image.Width - 1)
                        i = 0;
                    if (y == 0 || y == Image.Height - 1)
                        j = 0;

                    //Vector creation
                    double[] vector = new double[3];
                    vector[0] = (-HeightData[x + i, y + 0] + HeightData[x - i, y + 0]) * scale;
                    vector[1] = (+HeightData[x + 0, y + j] - HeightData[x + 0, y - j]) * scale;
                    vector[2] = 1.0;

                    //Normalization
                    double invlength = 1.0 / System.Math.Sqrt(vector[0] * vector[0] + vector[1] * vector[1] + vector[2] * vector[2]);
                    vector[0] *= invlength;
                    vector[1] *= invlength;
                    vector[2] *= invlength;

                    //Change range
                    vector[0] = (vector[0] + 1.0) * 127.5;
                    vector[1] = (vector[1] + 1.0) * 127.5;
                    vector[2] = (vector[2] + 1.0) * 127.5;

                    if (vector[0] < 0.0) vector[0] = 0;
                    if (vector[1] < 0.0) vector[1] = 0;
                    if (vector[2] < 0.0) vector[2] = 0;

                    if (vector[0] > 255.0) vector[0] = 255.0;
                    if (vector[1] > 255.0) vector[1] = 255.0;
                    if (vector[2] > 255.0) vector[2] = 255.0;

                    //Normal data storage
                    color = Color.FromArgb(
                        (byte)(HeightData[x, y] * 127.5),
                        (byte)vector[0],
                        (byte)vector[1],
                        (byte)vector[2]);
                    NormalMap.SetPixel(x, y, color);
                }
            }

            return NormalMap;
        }

       


    }
}
