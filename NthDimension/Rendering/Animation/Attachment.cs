


namespace NthDimension.Rendering.Animation
{
   
    using System.Collections.Concurrent;
    using NthDimension.Algebra;
    using NthDimension.Rendering.Drawables.Models;

    public class Attachment
    {
        private string                      m_name;
        private AnimatedModel               m_parent;
        private int                         m_matrixIndex;
        private Vector3                     m_vertex;
        private Vector3                     m_offset;
        private Quaternion                  m_orientation;
        private bool                        m_enabled               = true;
        private string                      m_meshFile;
        private string                      m_meshMaterialFile;

        #region Properties

        public AnimatedModel                Parent
        {
            get {  return m_parent; }
            private set { m_parent = value; }
        }

        public string                       Name
        {
            get {  return m_name; }
            private set { m_name = value; }
        }

        public int                          AnimationMatrix
        {
            get {  return m_matrixIndex; }
            private set { m_matrixIndex = value; }
        }

        public Vector3                      Vertex
        {
            get {  return m_vertex; }
            private set { m_vertex = value; }
        }

        public Vector3                      Offset
        {
            get { return m_offset; }
            set { m_offset = value; }

        }

        public Quaternion                   Orientation
        {
            get
            {
                if(null == Orientation)
                    m_orientation = Quaternion.Identity;

                return m_orientation;
            }
            set { m_orientation = value; }
        }

        public bool                         Enabled
        {
            get {  return m_enabled; }
            set { m_enabled = value; }
        }

        public string                       MeshFile
        {
            get { return m_meshFile; }
        }

        public string                       MeshMaterialFile
        {
            get {  return m_meshMaterialFile; }
        }
        #endregion

        /// <summary>
        /// Defines an attachment point for an Animated Model (namely avatar character models). The attachment points
        /// are used to attach another model that follows the parent model animation transformations
        /// </summary>
        /// <param name="parent">The parent animated model whose animation must be followed</param>
        /// <param name="attachmentName">An identifier for the object that is being attached</param>
        /// <param name="animationMatrix">The index of the Active Animation matrix that transforms the attached model</param>
        /// <param name="vertex">The vertex position of the parent model that is used a reference</param>
        /// <param name="offset">The offset to the vertex position for correct placement</param>
        /// <param name="orientation">The orientation to be applied to the attached model</param>
        /// <param name="meshFile">The mesh file of the attached model</param>
        /// <param name="meshMaterialFile">The material for the mesh to be attached</param>
        public Attachment(AnimatedModel parent, string attachmentName, int animationMatrix, Vector3 vertex, Vector3 offset, Quaternion orientation, string meshFile, string meshMaterialFile)
        {
            this.m_parent           = parent;
            this.m_name             = attachmentName;
            this.m_matrixIndex      = animationMatrix;
            this.m_vertex           = vertex;
            this.m_offset           = offset;
            this.m_orientation      = orientation;
            this.m_meshFile         = meshFile;
            this.m_meshMaterialFile = meshMaterialFile;
        }
    }

    public class KnownVertices : ConcurrentDictionary<string, Vector3>
    {
        public KnownVertices()
        {
            this.GetOrAdd("head", new Vector3());
            this.GetOrAdd("lhand", new Vector3());
            this.GetOrAdd("rhand", new Vector3());
            this.GetOrAdd("lfoot", new Vector3());
            this.GetOrAdd("rfoot", new Vector3());
            this.GetOrAdd("top", new Vector3());
            this.GetOrAdd("bottom", new Vector3());
        }
    }
}
