using NthDimension.Procedural.Dungeon;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Dungeon.Templates.Abyss
{
    internal class Corridor : LevelMapCorridor
    {
        public override void Rasterize(LevelRoom src, LevelRoom dst, Point srcPos, Point dstPos)
        {
            Default(srcPos, dstPos, new LevelTile
            {
                TileType = AbyssTemplate.RedSmallChecks
            });
        }
    }
}
