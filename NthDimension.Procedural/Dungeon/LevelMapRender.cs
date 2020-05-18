using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Dungeon
{
    public class LevelMapRender
    {
        protected BitmapRasterizer<LevelTile> Rasterizer { get; private set; }
        protected LevelGraph Graph { get; private set; }
        protected Random Rand { get; private set; }

        internal void Init(BitmapRasterizer<LevelTile> rasterizer, LevelGraph graph, Random rand)
        {
            Rasterizer = rasterizer;


            Graph = graph;
            Rand = rand;
        }

        public virtual void Rasterize()
        {
        }
    }
}
