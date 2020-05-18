using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Game.Dungeon.Data
{
    internal class ColorDefs
    {
        public const byte FixedColorBlack = 0;
        public const byte FixedColorWhite = 1;
        public const byte FixedColorSlate = 2;
        public const byte FixedColorOrange = 3;
        public const byte FixedColorRed = 4;
        public const byte FixedColorGreen = 5;
        public const byte FixedColorBlue = 6;
        public const byte FixedColorBrown = 7;
        public const byte FixedColorDarkGrey = 8;
        public const byte FixedColorLightGrey = 9;
        public const byte FixedColorViolet = 10;
        public const byte FixedColorYellow = 11;
        public const byte FixedColorLightRed = 12;
        public const byte FixedColorLightGreen = 13;
        public const byte FixedColorCyan = 14;
        public const byte FixedColorLightBrown = 15;
        public const byte FixedColorDarkViolet = 16;
        public const byte FixedColorDarkGreen = 17;
        public const byte FixedColorDarkCyan = 18;

        public const int MaxColors = 32;

        Color[] _entries = new Color[MaxColors];
        Dictionary<string, int> _colorNames = new Dictionary<string, int>();

        ColorDefs()
        {
            // Initialize all colors as empty, signifying that they are unused
            for (int i = 0; i < _entries.Length; i++)
                _entries[i] = Color.Empty;
        }

        public Color[] Entries { get { return _entries; } }

        public Dictionary<string, int> ColorNames { get { return _colorNames; } }

        public static ColorDefs LoadFromFile(string fileName)
        {
            ColorDefs definitions = new ColorDefs();

            using (PropertyStream stream = new PropertyStream(File.OpenText(fileName)))
            {
                // The current color index which is being read from the file
                int colorIndex = -1;

                // Read the color definition file line-by-line
                string property, value;
                while (stream.ReadNext(out property, out value))
                {
                    // Which property are we loading from this line?
                    if (0 == string.Compare(property, "index", true))
                    {
                        // Did we load a bad color index?
                        bool invalidIndex = false;

                        // Set the current color index
                        if (!int.TryParse(value, out colorIndex))
                            invalidIndex = true;

                        // Is the color index out of bounds?
                        if (colorIndex < 0 || colorIndex >= MaxColors)
                            invalidIndex = true;

                        // Report errors for bad color index
                        if (invalidIndex)
                            throw new ApplicationException(string.Format(
                                "Failed to load color definitions.  Invalid color index on line {0}.",
                                stream.LineNumber));
                    }
                    else if (0 == string.Compare(property, "color name", true))
                    {
                        // Make sure that we know which index the name belongs to
                        if (colorIndex < 0)
                            throw new ApplicationException(string.Format(
                                "Failed to load color definitions.  Color index must be specified before the color name on line {0}.",
                                stream.LineNumber));

                        // Multiple definitions may be separated by commas
                        string[] parts = value.Split(',');

                        // Store the name(s) for the current color index
                        foreach (string part in parts)
                            definitions._colorNames[part.Trim()] = colorIndex;
                    }
                    else if (0 == string.Compare(property, "rgb", true))
                    {
                        // Make sure that we know which index the RGB values belong to
                        if (colorIndex < 0)
                            throw new ApplicationException(string.Format(
                                "Failed to load color definitions.  Color index must be specified before the RGB values on line {0}.",
                                stream.LineNumber));

                        // We're looking for a comma-delimited set of 3 integers
                        string[] parts = value.Split(',');
                        if (parts.Length != 3)
                            throw new ApplicationException(string.Format(
                                "Failed to load color definitions.  Unrecognized format for RGB values on line {0}.",
                                stream.LineNumber));

                        // Remove any excess whitespace before trying to parse the numbers
                        for (int i = 0; i < 3; i++)
                            parts[i] = parts[i].Trim();

                        // Parse the RGB values
                        int r = 0, g = 0, b = 0;
                        bool loadedRGB =
                            int.TryParse(parts[0], out r) &&
                            int.TryParse(parts[1], out g) &&
                            int.TryParse(parts[2], out b);

                        // Were all of the values loaded OK?
                        if (!loadedRGB)
                            throw new ApplicationException(string.Format(
                                "Failed to load color definitions.  Unrecognized format for RGB values on line {0}.",
                                stream.LineNumber));

                        // Store the color definition at the current index
                        definitions._entries[colorIndex] = Color.FromArgb(r, g, b);
                    }
                }
            }

            // Finished loading color definitions
            return definitions;
        }
    }
}
