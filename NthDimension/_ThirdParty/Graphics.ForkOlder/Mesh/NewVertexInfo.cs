using NthDimension.Algebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Graphics.Mesh
{
    public struct NewVertexInfo
    {
        public Vector3d v;
        public Vector3 n, c;
        public Vector2 uv;
        public bool bHaveN, bHaveUV, bHaveC;

        public NewVertexInfo(Vector3d v)
        {
            this.v = v; n = c = Vector3.Zero; uv = Vector2.Zero;
            bHaveN = bHaveC = bHaveUV = false;
        }
        public NewVertexInfo(Vector3d v, Vector3 n)
        {
            this.v = v; this.n = n; c = Vector3.Zero; uv = Vector2.Zero;
            bHaveN = true; bHaveC = bHaveUV = false;
        }
        public NewVertexInfo(Vector3d v, Vector3 n, Vector3 c)
        {
            this.v = v; this.n = n; this.c = c; uv = Vector2.Zero;
            bHaveN = bHaveC = true; bHaveUV = false;
        }
        public NewVertexInfo(Vector3d v, Vector3 n, Vector3 c, Vector2 uv)
        {
            this.v = v; this.n = n; this.c = c; this.uv = uv;
            bHaveN = bHaveC = bHaveUV = true;
        }
    }
}
