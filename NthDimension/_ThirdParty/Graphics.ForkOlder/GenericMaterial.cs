using NthDimension.Algebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Graphics
{
    // Very hard to abstract material definitions from different formats.
    // basically we just have a generic top-level class and then completely different subclasses...

    public abstract class GenericMaterial
    {
        public static readonly float Invalidf = float.MinValue;
        public static readonly Vector3 Invalid = new Vector3(-1, -1, -1);

        public string name;
        public int id;


        abstract public Vector3 DiffuseColor { get; set; }
        abstract public float Alpha { get; set; }


        public enum KnownMaterialTypes
        {
            OBJ_MTL_Format
        }
        public KnownMaterialTypes Type { get; set; }
    }
}
