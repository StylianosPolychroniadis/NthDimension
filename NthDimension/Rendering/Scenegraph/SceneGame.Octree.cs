namespace NthDimension.Rendering.Scenegraph
{
    using NthDimension.Rendering.Partition;

    public partial class SceneGame
    {
        public OctreeWorld WorldOctree;
        public void CreateOctree()
        {
            WorldOctree = new OctreeWorld();
        }
    }
}
