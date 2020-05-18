using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NthDimension.Procedural.Dungeon;

namespace NthDimension.Procedural.Dungeon.Templates.Cave
{

    internal class Background : LevelMapRender
    {
        public override void Rasterize()
        {
            var tile = new LevelTile
            {
                TileType = CaveTemplate.ShallowWater
            };

            Rasterizer.Clear(tile);
        }
    }
}
