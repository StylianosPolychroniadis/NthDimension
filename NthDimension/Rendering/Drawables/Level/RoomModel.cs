using NthDimension.Rendering.Drawables.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Rendering.Drawables.Level
{
    public class RoomModel : StaticModel
    {
        new public static string nodename = "room";

        //private Procedural.DungeonGenerator     generator;

        //Template                template;

        //FloorModel              floor;
        //CeilingModel            ceiling;
        public int Width { get; private set; }
        public int Length { get; private set; }
        public int Height { get; private set; }

        public RoomModel(ApplicationObject parent, int width, int length, int height) : base(parent)
        {
            Width = width;
            Length = length;
            Height = height;
        }

    }
}
