﻿using System.Drawing;


namespace NthStudio.Utilities
{
    public class ColorUtil
    {

        /// <summary>
		/// Creates color with corrected brightness.
		/// </summary>
		/// <param name="color">TextColor to correct.</param>
		/// <param name="correctionFactor">The brightness correction factor. Must be between -1 and 1. 
		/// Negative values produce darker colors.</param>
		/// <returns>
		/// Corrected <see cref="Color"/> structure.
		/// </returns>
		public static Color ChangeColorBrightness(Color color, float correctionFactor)
        {
            float red = (float)color.R;
            float green = (float)color.G;
            float blue = (float)color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }

            return Color.FromArgb(color.A, (int)red, (int)green, (int)blue);
        }

        // TODO:: Fix for Color4f
        public static Color FColorToColor(float r, float g, float b, float a)
        {
            var c = Color.FromArgb((int)a * 255,
                                   (int)r * 255,
                                   (int)g * 255,
                                   (int)b * 255);
            return c;
        }
    }
}
