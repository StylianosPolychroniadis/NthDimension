using NthDimension.Algebra;

namespace NthDimension.Procedural.Tree
{
    public class Leaf
    {
        public Vector3 Position { get; set; }
        public Branch ClosestBranch { get; set; }

        public float Weight { get; set; }
        public const int MaxWeight = 1;

        public Leaf(Vector3 position)
        {
            Position = position;
        }
    }
}
