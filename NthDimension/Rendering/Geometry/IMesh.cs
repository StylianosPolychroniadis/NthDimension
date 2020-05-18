namespace NthDimension.Rendering
{
    using NthDimension.Rendering.Geometry;
    using NthDimension.Rendering.Serialization;


    public interface IMesh
    {
        MeshVbo createMesh(ListVector3 vertices, int[] indices, ListVector2 uvs);
        MeshVbo createMesh(ListVector3 vertices, int[] indices, ListVector2 uvs, ListVector3 normals);
        MeshVbo createMesh(ListVector3 vertices, int[] indices, ListVector2 uvs, ListVector3 normals, ListVector3 tangents);
        MeshVbo createMesh(ListVector3 vertices, int[] indices, ListVector2 uvs, ListVector3 normals, ListVector3 binormals, ListVector3 tangents, ListVector3 bitangents);
    }
}
