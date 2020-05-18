using NthDimension.Algebra;

namespace NthDimension.Procedural.Tree
{
    internal class Cube : ITreeShape
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public int Length { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Cube(int x, int y, int z, int width, int height, int length)
        {
            X = x;
            Y = y;
            Z = z;
            Length = length;
            Width = width;
            Height = height;
        }

        public bool Contains(Vector3 point)
        {
            return point.X >= X && point.X <= X + Width && point.Y >= Y && point.Y <= Y + Height && point.Z >= Z &&
                   point.Z <= Z + Length;
        }
    }
    internal class Sphere : ITreeShape
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public int Radius { get; }
        public int Length { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Sphere(int x, int y, int z, int radius)
        {
            X = x;
            Y = y;
            Z = z;
            Radius = radius;
            Length = Width = Height = radius * 2;
        }

        public bool Contains(Vector3 point)
        {
            return (new Vector3(X + Length / 2f, Y + Width / 2f, Z + Height / 2f) - point).Length <= Radius;
        }
    }
}
