namespace NthDimension.Rendering.Scenegraph
{
    using NthDimension.Algebra;
    using NthDimension.Rendering.Culling;
    using NthDimension.Rendering.Partition;


    public class SceneOctree : OctreeWorld
    {
        public bool Finalized = false;
        private float size;

        public SceneOctree(float groundSize)
        {
            this.size = groundSize;
            
        }

        public void FinalizeOctree()
        {
            this.BuildTree(new BoundingAABB(new Vector3(-size, -size, -size),
                                      new Vector3(size, size, size)));
            this.Finalized = true;
        }

        public float GetOctreeSize
        {
            get { return size; }
        }
    }
}
