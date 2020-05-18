using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NthDimension.Procedural.Dungeon;

namespace NthDimension.Procedural.Dungeon.Templates.Abyss
{
    internal class RoomRoot : LevelRoom
    {
        readonly int len;
        internal Point portalPos;

        public RoomRoot(int len)
        {
            this.len = len;
        }

        public override LevelRoomType Type { get { return LevelRoomType.Start; } }

        public override int Width { get { return len; } }

        public override int Length { get { return len; } }

        public override void Rasterize(BitmapRasterizer<LevelTile> rasterizer, Random rand)
        {
            rasterizer.FillRect(Bounds, new LevelTile
            {
                TileType = AbyssTemplate.RedSmallChecks
            });

            var buf = rasterizer.Bitmap;
            var bounds = Bounds;

            bool portalPlaced = false;
            while (!portalPlaced)
            {
                int x = rand.Next(bounds.X + 2, bounds.MaxX - 4);
                int y = rand.Next(bounds.Y + 2, bounds.MaxY - 4);
                if (buf[x, y].Object != null)
                    continue;

                buf[x, y].Region = "Spawn";
                buf[x, y].Object = new LevelObject
                {
                    ObjectType = AbyssTemplate.CowardicePortal
                };
                portalPos = new Point(x, y);
                portalPlaced = true;
            }
        }

        public override void Rasterize(Rendering.Scenegraph.SceneGame scene, Random rand)
        {
            throw new NotImplementedException();
        }
    }
}
