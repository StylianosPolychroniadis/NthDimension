using NthDimension.Algebra;

namespace NthDimension.Graphics.Mesh
{
    public interface IMeshDeformable : IMesh
    {
        void SetVertex(int vID, Vector3d vNewPos);
        void SetVertexNormal(int vid, Vector3 vNewNormal);
    }
}
