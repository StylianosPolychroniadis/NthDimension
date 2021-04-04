using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace NthDimension.Rendering.Utilities
{
    public enum RGB
    {
        R,
        G,
        B
    }

    /// <summary>
    /// Different useful functions.
    /// </summary>
    public class BitmapUtils
    {
        /// <summary>
        /// Return mean value for image.
        /// </summary>
        /// <param name="bmp">Source image.</param>
        /// <param name="channel">Color channel.</param>
        /// <returns>Mean value.</returns>
        public static double GetMeanValue(Bitmap bmp, RGB channel)
        {
            double e = 0;
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    e += GetColorChannel(bmp.GetPixel(i, j), channel);
                }
            }

            return e / (bmp.Width * bmp.Height);
        }

        /// <summary>
        /// Return mean value for image.
        /// </summary>
        /// <param name="bmp">Source image.</param>
        /// <returns>Mean value of color.</returns>
        public static Color GetMeanValue(Bitmap bmp)
        {
            return Color.FromArgb((int)(GetMeanValue(bmp, RGB.R)),
                                (int)(GetMeanValue(bmp, RGB.G)),
                                (int)(GetMeanValue(bmp, RGB.B)));
        }

        /// <summary>
        /// Get dispersion of image.
        /// </summary>
        /// <param name="bmp">Source image.</param>
        /// <param name="channel">Color channel.</param>
        /// <returns>Dispersion of image.</returns>
        public static double GetDispersion(Bitmap bmp, RGB channel)
        {
            double d = 0;
            double e = GetMeanValue(bmp, channel);
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    double difference = GetColorChannel(bmp.GetPixel(i, j), channel) - e;
                    d += difference * difference;
                }
            }
            d /= (bmp.Width * bmp.Height);
            return System.Math.Sqrt(d);
        }

        /// <summary>
        /// Get dispersion of image.
        /// </summary>
        /// <param name="bmp">Source image.</param>
        /// <returns>Dispersion of color.</returns>
        public static Color GetDispersion(Bitmap bmp)
        {
            return Color.FromArgb((int)(GetDispersion(bmp, RGB.R)),
                                (int)(GetDispersion(bmp, RGB.G)),
                                (int)(GetDispersion(bmp, RGB.B)));
        }

        /// <summary>
        /// Get color channel.
        /// </summary>
        /// <param name="color">RGB color.</param>
        /// <param name="channel">Color channel.</param>
        /// <returns></returns>
        public static int GetColorChannel(Color color, RGB channel)
        {
            switch (channel)
            {
                case RGB.R: return color.R;
                case RGB.G: return color.G;
                default: return color.B;
            }
        }

        /// <summary>
        /// Get correct value of RGB color channel.
        /// </summary>
        /// <param name="value">Channel value.</param>
        /// <returns>Corrected value.</returns>
        public static int GetCorrectChannel(int value)
        {
            if ((value < 256) & (value >= 0))
            {
                return value;
            }
            if (value > 255)
            {
                return 255;
            }
            return 0;
        }

        /// <summary>
        /// Get correct color;
        /// </summary>
        /// <param name="r">R channel value.</param>
        /// <param name="g">G channel value.</param>
        /// <param name="b">B channel value.</param>
        /// <returns>Correct RGB color.</returns>
        public static Color GetCorrectColor(int r, int g, int b)
        {
            return Color.FromArgb(GetCorrectChannel(r),
                                  GetCorrectChannel(g),
                                  GetCorrectChannel(b));
        }

        /// <summary>
        /// Return max value of color channel.
        /// </summary>
        /// <param name="color">Color.</param>
        /// <returns>Max channel value.</returns>
        public static int MaxChannel(Color color)
        {
            return System.Math.Max(System.Math.Max(color.R, color.G), color.B);
        }

        /// <summary>
        /// Return min value of color channel.
        /// </summary>
        /// <param name="color">Color.</param>
        /// <returns>min channel value.</returns>
        public static int MinChannel(Color color)
        {
            return System.Math.Min(System.Math.Min(color.R, color.G), color.B);
        }

        /// <summary>
        /// Return sum of color channels.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static int ChannelSum(Color color)
        {
            return color.R + color.G + color.B;
        }

        /// <summary>
        /// Are colors equal of not?
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public static bool IsEqualColors(Color c1, Color c2)
        {
            return ((c1.R == c2.R) & (c1.B == c2.B) & (c1.G == c2.G));
        }

        /// <summary>
        /// Get grayscale color.
        /// Gray = Green * 0.59 + Blue * 0.30 + Red * 0.11
        /// </summary>
        /// <param name="color">RGB Color.</param>
        /// <returns></returns>
        public static int GetGray(Color color)
        {
            return (int)(color.R * 0.11 + color.G * 0.59 + color.B * 0.3);
        }

        public static Color GetGrayColor(Color color)
        {
            return GetCorrectColor(GetGray(color), GetGray(color), GetGray(color));
        }

        /// <summary>
        /// Get distance between colors.
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public static int GetColorDistance(Color c1, Color c2)
        {
            return (int)System.Math.Sqrt((c1.R - c2.R) * (c1.R - c2.R) +
                            (c1.G - c2.G) * (c1.G - c2.G) +
                            (c1.B - c2.B) * (c1.B - c2.B));
        }

        /// <summary>
        /// Get color superposition.
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public static Color SuperpositionColor(Color c1, Color c2, double k)
        {
            return GetCorrectColor((int)(c1.R * k + c2.R * (1 - k)),
                                    (int)(c1.G * k + c2.G * (1 - k)),
                                    (int)(c1.B * k + c2.B * (1 - k)));
        }

        public static Bitmap MakeGrayscale3(Bitmap original)
        {
            //create a blank bitmap the same size as original
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            //get a graphics object from the new image
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(newBitmap);

            //create the grayscale ColorMatrix
            ColorMatrix colorMatrix = new ColorMatrix(
                new float[][]
                {
                    new float[] {.3f, .3f, .3f, 0, 0},
                    new float[] {.59f, .59f, .59f, 0, 0},
                    new float[] {.11f, .11f, .11f, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {0, 0, 0, 0, 1}
                });

            //create some image attributes
            ImageAttributes attributes = new ImageAttributes();

            //set the color matrix attribute
            attributes.SetColorMatrix(colorMatrix);

            //draw the original image on the new image
            //using the grayscale color matrix
            g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
                0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);

            //dispose the Graphics object
            g.Dispose();
            return newBitmap;
        }
    }
}
