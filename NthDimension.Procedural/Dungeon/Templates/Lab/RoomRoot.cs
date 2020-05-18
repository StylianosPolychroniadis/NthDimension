using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Dungeon.Templates.Lab
{
    using NthDimension.Procedural.Dungeon;
    internal class RoomRoot : LevelRootRoom
    {
        static readonly Rect template = new Rect(0, 96, 26, 128);

        public override LevelRoomType Type { get { return LevelRoomType.Start; } }

        public override int Width { get { return template.MaxX - template.X; } }

        public override int Length { get { return template.MaxY - template.Y; } }

        static readonly Tuple<Direction, int>[] connections = {
            Tuple.Create(Direction.North, 11)
        };

        public override Tuple<Direction, int>[] ConnectionPoints { get { return connections; } }

        public override void Rasterize(BitmapRasterizer<LevelTile> rasterizer, Random rand)
        {
            rasterizer.Copy(LabTemplate.MapTemplate, template, Pos);
            LabTemplate.DrawSpiderWeb(rasterizer, Bounds, rand);
        }

        public override void Rasterize(Rendering.Scenegraph.SceneGame scene, Random rand)
        {
            throw new NotImplementedException();
        }
    }
}
