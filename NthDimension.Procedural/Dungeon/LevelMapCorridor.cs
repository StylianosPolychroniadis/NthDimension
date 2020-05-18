using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Dungeon
{
    public class LevelMapCorridor
    {
        protected BitmapRasterizer<LevelTile> Rasterizer { get; private set; }
        protected LevelGraph Graph { get; private set; }
        protected Random Rand { get; private set; }


        public Rect CorridorRectangle
        {
            get { return _rect; }
        }

        private Rect _rect;

        internal void Init(BitmapRasterizer<LevelTile> rasterizer, LevelGraph graph, Random rand)
        {
            //throw new NotImplementedException("Rasterizer call");
            Rasterizer = rasterizer;
            Graph = graph;
            Rand = rand;
        }
        public void Init(LevelGraph graph, Random rand)
        {
            Graph = graph;
            Rand = rand;
        }

        public virtual void Rasterize(LevelRoom src, LevelRoom dst, Point srcPos, Point dstPos)
        {
        }

        protected void Default(Point srcPos, Point dstPos, LevelTile tile)
        {
            if (srcPos.X == dstPos.X)
            {
                if (srcPos.Y > dstPos.Y)
                    NthDimension.Utilities.Utils.Swap(ref srcPos, ref dstPos);

                //throw new NotImplementedException("Rasterizer call");
                _rect = new Rect(srcPos.X, srcPos.Y, srcPos.X + Graph.Template.CorridorWidth, dstPos.Y);    //  //Rasterizer.FillRect(new Rect(srcPos.X, srcPos.Y, srcPos.X + Graph.Template.CorridorWidth, dstPos.Y), tile);                 //Rasterizer.FillRect(new Rect(srcPos.X, srcPos.Y, srcPos.X + Graph.Template.CorridorWidth, dstPos.Y), tile);
            }
            else if (srcPos.Y == dstPos.Y)
            {
                if (srcPos.X > dstPos.X)
                    NthDimension.Utilities.Utils.Swap(ref srcPos, ref dstPos);

                //throw new NotImplementedException("Rasterizer call");
                _rect = new Rect(srcPos.X, srcPos.Y, dstPos.X, srcPos.Y + Graph.Template.CorridorWidth);    // //Rasterizer.FillRect(new Rect(srcPos.X, srcPos.Y, dstPos.X, srcPos.Y + Graph.Template.CorridorWidth), tile);
            }

            Rasterizer.FillRect(_rect, tile);            

        }
    }
}
