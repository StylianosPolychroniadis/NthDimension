
using NthDimension.Algebra;

namespace NthDimension.Procedural.Tree
{
    internal interface ITreeShape
    {
        int X { get; }
        int Y { get; }
        int Z { get; }
        int Length { get; }
        int Width { get; }
        int Height { get; }
        bool Contains(Vector3 point);
    }
}
