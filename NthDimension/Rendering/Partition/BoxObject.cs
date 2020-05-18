using System;
using NthDimension.Algebra;
using NthDimension.Rendering.Culling;

namespace NthDimension.Rendering.Partition
{
    public class BoxObject : OctreeObject
    {
        public Vector3 postion;
        public Vector3 size;
        public float rotation;
        public string name;


        public BoxObject(BoundingAABB box, string meshName)
            : base()
        {
            this.name = meshName;
            size = box.Max - box.Min;
            size *= 0.5f;
            postion = box.Min + size;
            rotation = 0;
            OctreeBounds = new BoundingAABB(box.Min, box.Max);
        }


    }
}
