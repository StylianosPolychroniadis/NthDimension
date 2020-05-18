using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;

namespace NthStudio.Game.Dungeon.Data
{
    internal class RoomDef : IChoosable
    {
        CaveFeature[] _tiles;
        string _name;
        Size _size;
        List<Point> _goodySpots = new List<Point>();
        int _frequency;

        private RoomDef()
        {
        }

        public string Name { get { return _name; } }
        public int ChooseFrequency { get { return _frequency; } }
        public IEnumerable<Point> GoodySpots { get { return _goodySpots; } }
        public CaveFeature[] Tiles { get { return _tiles; } }
        public Size Size { get { return _size; } }

        public static List<RoomDef> LoadFromFile(string fileName)
        {
            List<RoomDef> definitions = new List<RoomDef>();
            using (StreamReader reader = File.OpenText(fileName))
            {
                string line;
                RoomDef definition = null;
                List<string> tileDataLines = new List<string>();
                int lineNumber = 0;
                while (null != (line = reader.ReadLine()))
                {
                    // Track line number for improved error reporting
                    lineNumber++;

                    // Remove any whitespace at the end
                    line = line.TrimEnd(' ', '\t');

                    // Skip comments
                    if (line.StartsWith("#"))
                        continue;

                    // Finish the current record if the line is blank
                    if (line.Length == 0)
                    {
                        if (definition != null)
                        {
                            FinishRoomDefinition(definition, tileDataLines);
                            tileDataLines.Clear();
                            definitions.Add(definition);
                            definition = null;
                        }
                        continue;
                    }

                    if (definition == null)
                        definition = new RoomDef();

                    // Read name
                    if (line.StartsWith("N:"))
                        definition._name = line.Substring(2);
                    else if (line.StartsWith("F:"))
                        definition._frequency = int.Parse(line.Substring(2));
                    else if (line.StartsWith("D:"))
                        tileDataLines.Add(line.Substring(2));
                    else throw new ApplicationException(string.Format("Failed to load room definitions.  Error on line {0}", lineNumber));
                }
            }

            return definitions;
        }

        static void FinishRoomDefinition(RoomDef definition, List<string> tileDataLines)
        {
            // Determine the size of the room
            int height = tileDataLines.Count;
            int width = 0;
            for (int i = 0; i < height; i++)
                width = Math.Max(width, tileDataLines[i].Length);
            definition._size = new Size(width, height);

            // Translate into cave features
            definition._tiles = new CaveFeature[width * height];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    int offset = y * width + x;
                    char c = ' ';
                    if (x < tileDataLines[y].Length)
                        c = tileDataLines[y][x];
                    CaveFeature feature = CaveFeature.None;
                    switch (c)
                    {
                        case ' ':
                            feature = CaveFeature.None;
                            break;
                        case 'X':
                            feature = CaveFeature.PermanentWallOuter;
                            break;
                        case '#':
                            feature = CaveFeature.GraniteWallInner;
                            break;
                        case 'S':
                            feature = CaveFeature.GraniteWallSolid;
                            break;
                        case '%':
                            feature = CaveFeature.GraniteWallOuter;
                            break;
                        case '.':
                            feature = CaveFeature.CaveFloor;
                            break;
                        case '+':
                            feature = CaveFeature.DoorClosed1;
                            break;
                        case 'o':
                            feature = CaveFeature.CaveFloor;
                            definition._goodySpots.Add(new Point(x, y));
                            break;
                        default:
                            throw new ApplicationException(string.Format("Unexpected character in tile data for {0} room definition", definition._name));
                    }
                    definition._tiles[offset] = feature;
                }
        }
    }
}
