using NthDimension.Algebra;

namespace NthDimension.Procedural.Tree
{
    public class Branch
    {
        public Branch Parent { get; private set; }
        public Vector3 GrowDirection { get; set; }
        public Vector3 OriginalGrowDirection { get; set; }
        public int GrowCount { get; set; }
        public float Age { get; set; }
        public Vector3 Position { get; private set; }

        public Branch(Branch parent, Vector3 position, Vector3 growDirection)
        {
            Parent = parent;
            Position = position;
            GrowDirection = growDirection;
            OriginalGrowDirection = growDirection;
        }

        public void Reset()
        {
            GrowCount = 0;
            GrowDirection = OriginalGrowDirection;
        }
    }
}
