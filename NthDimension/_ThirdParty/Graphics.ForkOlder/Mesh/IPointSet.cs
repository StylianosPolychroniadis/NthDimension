using NthDimension.Algebra;

namespace NthDimension.Graphics.Mesh
{
    public interface IPointSet
    {
        int VertexCount { get; }
        int MaxVertexID { get; }

        bool HasVertexNormals { get; }
        bool HasVertexColors { get; }

        Vector3d GetVertex(int i);
        Vector3 GetVertexNormal(int i);
        Vector3 GetVertexColor(int i);

        bool IsVertex(int vID);

        // iterators allow us to work with gaps in index space
        System.Collections.Generic.IEnumerable<int> VertexIndices();

        int Timestamp { get; }
    }
}
