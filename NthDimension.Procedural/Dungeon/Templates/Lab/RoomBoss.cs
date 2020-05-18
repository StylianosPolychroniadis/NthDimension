using System;

namespace NthDimension.Procedural.Dungeon.Templates.Lab
{
    using NthDimension.Procedural.Dungeon;
    internal class RoomBoss : LevelRootRoom
    {
        static readonly Rect template = new Rect(0, 0, 24, 50);

        public override LevelRoomType Type { get { return LevelRoomType.Target; } }

        public override int Width { get { return template.MaxX - template.X; } }

        public override int Length { get { return template.MaxY - template.Y; } }

        static readonly Tuple<Direction, int>[] connections = {
            Tuple.Create(Direction.South, 10)
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
