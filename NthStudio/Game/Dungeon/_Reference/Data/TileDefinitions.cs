using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

using NthDimension.Rendering;
using NthDimension.Rendering.Drawables.Models;

namespace NthStudio.Game.Dungeon.Data
{
    internal class TileDefs
    {
        int                             _tileSize       = 32;
        int                             _columns;
        int                             _rows;
        Dictionary<string, int>         _tileNames      = new Dictionary<string, int>();

        // Drop
        Bitmap                          _tileImage;
        string                          _imageFile;

        // Use
        Material                        _material;
        Model                           _model;

        TileDefs()
        {
        }

        public string                   ImageFile       { get { return _imageFile; } }
        public Bitmap                   TileImage       { get { return _tileImage; } }
        public Dictionary<string, int>  TileNames       { get { return _tileNames; } }
        public int                      TileSize        { get { return _tileSize; } }
        public int                      Columns         { get { return _columns; } }
        public int                      Rows            { get { return _rows; } }

        public static TileDefs LoadFromFile(string fileName)
        {
            TileDefs defs = new TileDefs();

            using (PropertyStream stream = new PropertyStream(File.OpenText(fileName)))
            {
                // The current tile index which is being read from the file
                int tileIndex = -1;

                // Read the tile definition file line-by-line
                string property, value;
                while (stream.ReadNext(out property, out value))
                {
                    // Which property are we loading from this line?
                    if (0 == string.Compare("Index", property, true))
                    {
                        // Set the current color index
                        if (!int.TryParse(value, out tileIndex))
                            throw new ApplicationException(string.Format(
                                "Failed to load tile definitions.  Invalid tile index on line {0}.",
                                stream.LineNumber));
                    }
                    else if (0 == string.Compare("Name", property, true))
                    {
                        if (string.IsNullOrEmpty(value))
                            throw new ApplicationException(string.Format(
                                "Failed to load tile definitions.  Invalid tile name on line {0}.",
                                stream.LineNumber));
                        defs._tileNames[value] = tileIndex;
                    }
                    else if (0 == string.Compare("Image File", property, true))
                    {
                        // Try to load the image
                        defs._imageFile = value;
                        try
                        {
                            string imageFile = Path.Combine(NthDimension.Utilities.DirectoryUtil.AssemblyDirectory, value);
                            if (!File.Exists(imageFile))
                                throw new ApplicationException("The file does not exist: " + value);
                            Image fromFile = Image.FromFile(imageFile);
                            ((Bitmap)fromFile).MakeTransparent(Color.FromArgb(255, 0, 255));

                            // Convert into 32bpp PArgb for speed reasons
                            defs._tileImage = new Bitmap(fromFile.Width, fromFile.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                            Graphics g = Graphics.FromImage(defs._tileImage);
                            g.Clear(Color.Transparent);
                            g.DrawImage(fromFile, 0, 0, fromFile.Width, fromFile.Height);
                            g.Dispose();
                            fromFile.Dispose();
                            //defs._tileImage.MakeTransparent(Color.FromArgb(255, 0, 255));
                        }
                        catch (Exception ex)
                        {
                            throw new ApplicationException("Failed to load tiles image", ex);
                        }
                    }
                    else if (0 == string.Compare("Tile Size", property, true))
                    {
                        // Did we load a bad size?
                        int size = 0;
                        if (!int.TryParse(value, out size))
                            throw new ApplicationException(string.Format(
                                "Failed to load tile definitions.  Invalid tile size on line {0}.", stream.LineNumber));
                        defs._tileSize = size;
                    }
                }
            }

            // Validate the definitions before finishing
            if (defs._tileImage == null)
                throw new ApplicationException("Failed to load tile definitions.  The tiles image is undefined.");

            defs._columns = defs._tileImage.Width / defs._tileSize;
            defs._rows = defs._tileImage.Height / defs._tileSize;
            if (defs._tileImage.Width != defs._tileSize * defs._columns)
                throw new ApplicationException("Failed to load tile definitions.  The tiles image width is not a multiple of the tile size.");
            if (defs._tileImage.Height != defs._tileSize * defs._rows)
                throw new ApplicationException("Failed to load tile definitions.  The tiles image height is not a multiple of the tile size.");

            int tileCount = defs._columns * defs._rows;
            foreach (KeyValuePair<string, int> pair in defs._tileNames)
            {
                if (pair.Value < 0 || pair.Value >= tileCount)
                    throw new ApplicationException("Failed to load tile definitions.  Referenced invalid tile index: " + pair.Value.ToString());
            }

            // Finished
            return defs;
        }
    }
}
