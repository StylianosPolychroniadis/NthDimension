namespace NthDimension.Rasterizer.GL1
{
    using Algebra;
    using Algebra.Raytracing;
    using Buffers;
    using Geometry;
    using System;

    public partial class RendererBase
    {
        #region Ray
        public abstract Ray CreateRay(int x, int y, Vector3 origin);
        #endregion

        #region DisplayList
        public abstract DisplayList				CreateDisplayList();

        public abstract void ListBase(int listId);
        public abstract void ListBase(uint listId);

        public abstract void CallList(int listId);
        public abstract void CallList(uint listId);

        public abstract void CallLists(int n, ListNameType type, IntPtr lists);
        public abstract void CallLists(int n, uint[] lists);
        public abstract void CallLists(int n, byte[] lists);
        #endregion

        #region VBO

        [Obsolete("Use RendererGL.Buffers instead")]
        public abstract VboBuffer				CreateVBO_VAO(Mesh mesh);                     // TODO:: Temporary, remove-replace
        //public abstract VboBuffer CreateVertexBuffer(List<Vector3> positions, List<uint> indexes, List<Vector3> normals, List<Vector2> uvs);
                                                                                     //[Obsolete("Use RendererGL.Buffers instead")]
                                                                                     //public abstract VertexBuffer            CreateVertexBuffer(Vector3[] vertices, Vector2[] texcoords, Vector3[] normals);
                                                                                     //[Obsolete("Use RendererGL.Buffers instead")]
                                                                                     //public abstract VertexBuffer            CreateVertexBuffer<T>(SceneElement element) where T : SceneElement;
        #endregion

        #region VAO

        #endregion
    }
}
