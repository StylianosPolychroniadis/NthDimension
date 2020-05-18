using System;
using OpenTK.Graphics.OpenGL;
using NthDimension.Graphics.Renderer;


namespace NthDimension.Rasterizer.GL1
{
    /// <summary>
    /// The display list class basicly wraps an OpenGL display list, making them easier
    /// to manage. Remember this class is completely OpenGL dependant. In time this class
    /// will derive from the IOpenGLDependant interface.
    /// </summary>
    //[SecuritySafeCritical]
    public class DisplayListGL : DisplayList
    {
        protected int m_listId = -1;
        protected int m_listSize = 1;
        protected object m_tag = null;
        protected bool m_recording = false;

        public DisplayListGL()
        {

        }

        /// <summary>
        /// This function generates the display list. You must call it before you call
        /// anything else!
        /// </summary>
        /// <param name="gl">OpenGL</param>
        public override void Generate(int size = 1)
        {
            //	Generate one list.
            m_listSize = size;
            m_listId = GL.GenLists(size);
        }
        /// <summary>
        /// This function makes the display list.
        /// </summary>
        /// <param name="gl">OpenGL</param>
        /// <param name="mode">The mode, compile or compile and execute.</param>
        public override void New(NthDimension.Graphics.Renderer.ListMode mode)
        {
            //	Start the list.
            GL.NewList(m_listId, mode.ToOpenGL());
        }

        /// <summary>
        /// This function ends the compilation of a list.
        /// </summary>
        /// <param name="gl"></param>
        public override void End()
        {
            //	This function ends the display list
            if (m_recording)
                GL.EndList();

            m_recording = false;
        }
        public override bool IsList()
        {
            //	Is the list a proper display list?
            return GL.IsList(m_listId);
        }
        public static bool IsList(DisplayListGL displayList)
        {
            //	Is the specified list a proper display list?
            return GL.IsList(displayList.ListId);
        }
        public override bool Call()
        {
            bool ret = false;

            if (!Valid() || m_recording)
                return ret;

            try
            {
                GL.CallList(m_listId);
                return true;
            }
            catch (Exception)   // OpenGL Driver  error. TODO:: Log
            {
                return false;
            }
            return false;
        }
        public override void Delete()
        {
            GL.DeleteLists(m_listId, m_listSize);
            m_listId = -1;
        }

        public override bool Valid()
        {
            return m_listId != -1;
        }
        public override void Invalidate()
        {
            if (m_recording)
                End();

            if (Valid())
                GL.DeleteLists(m_listId, m_listSize);

            m_listId = -1;
        }
        public override void Start()
        {
            Start(false);
        }
        public override void Start(bool execute)
        {
            //this.Invalidate();
            //this.Generate();

            if (execute)
                New(NthDimension.Graphics.Renderer.ListMode.CompileAndExecute);
            else
                New(NthDimension.Graphics.Renderer.ListMode.Compile);

            m_recording = true;
        }
        public override int ListId
        {
            get { return m_listId; }
        }

        public override bool Recording
        {
            get { return m_recording; }
        }
    }
}
