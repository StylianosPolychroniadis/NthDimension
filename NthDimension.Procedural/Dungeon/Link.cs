namespace NthDimension.Procedural.Dungeon
{
    public struct Link
    {
        public readonly Direction Direction;
        public readonly int Offset;

        public Link(Direction direction, int offset)
        {
            Direction = direction;
            Offset = offset;
        }

        public override string ToString()
        {
            return string.Format("[{0}, {1}]", Direction, Offset);
        }
    }
}
