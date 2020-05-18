using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Game.Dungeon.Data
{
    internal struct FeatureTile
    {
        public byte CharIndex;
        public byte ColorIndex;
        public byte MapColorIndex;
        public int TileIndex;
        public int TileDarkIndex;
        public string Name;
        public FeatureFlag Flags;
    }

    internal class FeatureTiles
    {
        const int MaxTiles = 512;
        FeatureTile[] _entries = new FeatureTile[MaxTiles];

        FeatureTiles()
        {
        }

        public FeatureTile[] Entries { get { return _entries; } }

        public static FeatureTiles LoadFromFile(string fileName)
        {
            FeatureTiles tiles = new FeatureTiles();

            using (PropertyStream stream = new PropertyStream(System.IO.File.OpenText(fileName)))
            {
                // The current feature which is being read from the file
                CaveFeature feature = CaveFeature.Undefined;

                // Read the display file line-by-line
                string property, value;
                while (stream.ReadNext(out property, out value))
                {
                    // Which property are we loading from this line?
                    if (0 == string.Compare(property, "Feature", true))
                    {
                        try
                        {
                            feature = (CaveFeature)Enum.Parse(typeof(CaveFeature), value);
                        }
                        catch (Exception ex)
                        {
                            throw new ApplicationException(
                                string.Format("Failed to load feature definitions.  Unrecognized feature name on line {0}.",
                                stream.LineNumber), ex);
                        }
                    }
                    else if (0 == string.Compare(property, "Char", true))
                    {
                        // Make sure that we know which feature the name belongs to
                        if (feature == CaveFeature.Undefined)
                            throw new ApplicationException(string.Format(
                                "Failed to load feature definitions.  Cave feature must be specified before the char index on line {0}.",
                                stream.LineNumber));

                        // Try to parse the tile index
                        byte tileIndex = 0;
                        if (byte.TryParse(value, out tileIndex))

                            // Assign the tile index to the corresponding feature
                            tiles._entries[(int)feature].CharIndex = tileIndex;
                        else
                            throw new ApplicationException(string.Format(
                                "Failed to load feature definitions.  Invalid char index on line {0}.",
                                stream.LineNumber));
                    }
                    else if (0 == string.Compare(property, "Tile", true))
                    {
                        // Make sure that we know which feature the name belongs to
                        if (feature == CaveFeature.Undefined)
                            throw new ApplicationException(string.Format(
                                "Failed to load feature definitions.  Cave feature must be specified before the tile name on line {0}.",
                                stream.LineNumber));

                        // Try to parse the tile index
                        if (!Globals.TileDefinitions.TileNames.ContainsKey(value))
                            throw new ApplicationException(string.Format(
                                "Failed to load feature definitions.  Invalid tile name on line {0}.",
                                stream.LineNumber));

                        // Assign the tile index to the corresponding feature
                        tiles._entries[(int)feature].TileIndex = Globals.TileDefinitions.TileNames[value];

                        string darkName = value + " Dark";
                        if (Globals.TileDefinitions.TileNames.ContainsKey(darkName))
                            tiles._entries[(int)feature].TileDarkIndex = Globals.TileDefinitions.TileNames[darkName];
                    }
                    else if (0 == string.Compare(property, "Name", true))
                    {
                        // Make sure that we know which feature the name belongs to
                        if (feature == CaveFeature.Undefined)
                            throw new ApplicationException(string.Format(
                                "Failed to load feature definitions.  Cave feature must be specified before the name on line {0}.",
                                stream.LineNumber));
                        tiles._entries[(int)feature].Name = value;
                    }
                    else if (0 == string.Compare(property, "Color", true))
                    {
                        // Make sure that we know which feature the name belongs to
                        if (feature == CaveFeature.Undefined)
                            throw new ApplicationException(string.Format(
                                "Failed to load feature definitions.  Cave feature must be specified before the color name on line {0}.",
                                stream.LineNumber));

                        // Do we recognize the color name?
                        if (!Globals.ColorDefinitions.ColorNames.ContainsKey(value))
                            throw new ApplicationException(string.Format(
                                "Failed to load feature definitions.  Unrecognized color name on line {0}.",
                                stream.LineNumber));

                        // Assign the color value to the corresponding feature
                        tiles._entries[(int)feature].ColorIndex = (byte)Globals.ColorDefinitions.ColorNames[value];
                    }
                    else if (0 == string.Compare(property, "Map Color", true))
                    {
                        // Make sure that we know which feature the name belongs to
                        if (feature == CaveFeature.Undefined)
                            throw new ApplicationException(string.Format(
                                "Failed to load feature definitions.  Cave feature must be specified before the map color name on line {0}.",
                                stream.LineNumber));

                        // Do we recognize the color name?
                        if (!Globals.ColorDefinitions.ColorNames.ContainsKey(value))
                            throw new ApplicationException(string.Format(
                                "Failed to load feature definitions.  Unrecognized color name on line {0}.",
                                stream.LineNumber));

                        // Assign the color value to the corresponding feature
                        tiles._entries[(int)feature].MapColorIndex = (byte)Globals.ColorDefinitions.ColorNames[value];
                    }
                    else if (0 == string.Compare(property, "Flags", true))
                    {
                        // Make sure feature is defined
                        if (feature == CaveFeature.Undefined)
                            throw new ApplicationException(string.Format(
                                "Failed to load feature definitions.  Cave feature must be specified before the flags line {0}.",
                                stream.LineNumber));
                        try
                        {
                            tiles._entries[(int)feature].Flags = (FeatureFlag)Enum.Parse(typeof(FeatureFlag), value);
                        }
                        catch (ArgumentException ex)
                        {
                            throw new ApplicationException(
                                string.Format("Failed to load feature definitions.  Unrecognized feature flags on line {0}.",
                                stream.LineNumber), ex);
                        }
                    }
                }
            }

            return tiles;
        }
    }
}
