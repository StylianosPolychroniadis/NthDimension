using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Dungeon
{
    public abstract class LevelRoom
    {
        protected LevelRoom()
        {
            Edges = new List<Edge>(4);
        }

        public IList<Edge> Edges { get; private set; }
        public int Depth { get; internal set; }

        public abstract LevelRoomType Type { get; }
        public abstract int Width { get; }
        public abstract int Length { get; }     // TODO:: Refactor this to Depth

        public Point Pos { get; set; }

        public Rect Bounds { get { return new Rect(Pos.X, Pos.Y, Pos.X + Width, Pos.Y + Length); } }

        public virtual Range NumBranches { get { return new Range(1, 4); } }

        public abstract void Rasterize(BitmapRasterizer<LevelTile> rasterizer, Random rand);
        public abstract void Rasterize(Rendering.Scenegraph.SceneGame scene, Random rand);

        public string Mesh;         // @"scenes\empty\uvbox.xmf"
        public string Material;     // @"scenes\empty\unitbox_1mm.obj"
    }
}
