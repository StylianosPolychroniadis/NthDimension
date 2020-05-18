namespace NthDimension.Procedural.City
{
    using System.Collections.Generic;
    using NthDimension.Algebra;
    using NthDimension.Procedural.MockTBD;            // Warning Ref

    public class BlockSet
    {
        public struct Vertex
        {
            public Vector3 Position;
            public Vector3 Normal;
        }

        public struct Patch                         // WARNING Ref
        {
            public VertexBuffer<Vertex> vb;
            public IndexBuffer<ushort> ib;

            public List<Building> buildings;
            public List<Building> newBuildings;

            public ushort nbVertices;
            public ushort nbIndices;
        }

        public class Shader { }                     // WARNING Ref
        public class Renderer { }                   // WARNING Ref

        public Shader shader;                       // WARNING Ref

        public void addBlock(Block block) { }
        public void commitChange() { }
        public void render(Renderer renderer) { }
        public void reset() { }
        protected void precomputeBuffer() { }
    }
}
