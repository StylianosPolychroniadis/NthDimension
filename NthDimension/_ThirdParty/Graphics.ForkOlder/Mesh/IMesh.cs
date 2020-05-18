using NthDimension.Algebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Graphics.Mesh
{
    public interface IMesh : IPointSet
    {
        int TriangleCount { get; }
        int MaxTriangleID { get; }

        bool HasVertexUVs { get; }
        Vector2 GetVertexUV(int i);

        NewVertexInfo GetVertexAll(int i);

        bool HasTriangleGroups { get; }

        Index3i GetTriangle(int i);
        int GetTriangleGroup(int i);

        bool IsTriangle(int tID);

        // iterators allow us to work with gaps in index space
        System.Collections.Generic.IEnumerable<int> TriangleIndices();
    }
}
