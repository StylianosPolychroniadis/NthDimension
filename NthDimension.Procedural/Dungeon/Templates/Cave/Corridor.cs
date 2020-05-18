using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NthDimension.Procedural.Dungeon;

namespace NthDimension.Procedural.Dungeon.Templates.Cave
{
    internal class Corridor : LevelMapCorridor
    {
        public override void Rasterize(LevelRoom src, LevelRoom dst, Point srcPos, Point dstPos)
        {
            Default(srcPos, dstPos, new LevelTile
            {
                TileType = CaveTemplate.BrownLines
            });
        }
    }
}
