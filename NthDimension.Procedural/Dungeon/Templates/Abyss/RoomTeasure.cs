using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NthDimension.Procedural.Dungeon;

namespace NthDimension.Procedural.Dungeon.Templates.Abyss
{
    internal class TreasureRoom : LevelRootRoom
    {
        public override LevelRoomType Type { get { return LevelRoomType.Special; } }

        public override int Width { get { return 15; } }

        public override int Length { get { return 21; } }

        static readonly Tuple<Direction, int>[] connections = {
            Tuple.Create(Direction.South, 6)
        };

        public override Tuple<Direction, int>[] ConnectionPoints { get { return connections; } }

        public override void Rasterize(BitmapRasterizer<LevelTile> rasterizer, Random rand)
        {
            rasterizer.Copy(AbyssTemplate.MapTemplate, new Rect(70, 10, 85, 31), Pos, tile => tile.TileType.Name == "Space");

            var bounds = Bounds;
            var buf = rasterizer.Bitmap;
            for (int x = bounds.X; x < bounds.MaxX; x++)
                for (int y = bounds.Y; y < bounds.MaxY; y++)
                {
                    if (buf[x, y].TileType != AbyssTemplate.Space)
                        buf[x, y].Region = "Treasure";
                }
        }

        public override void Rasterize(Rendering.Scenegraph.SceneGame scene, Random rand)
        {
            throw new NotImplementedException();
        }
    }
}
