using OpenTK.Graphics.OpenGL;

namespace NthDimension.Rasterizer.GL1
{
    using Geometry;
    using Buffers;
    using Algebra;
    using Algebra.Geometry;
    using Renderer;

    using System;
    using System.Collections.Generic;
    using System.Drawing;

    public partial class RendererGL1x 
    {

        //CreateDevice() located in RendererGL.cs

        #region Ray
        public override Algebra.Raytracing.Ray CreateRay(int x, int y, Algebra.Vector3 origin)
        {


            OpenTK.Matrix4 modelViewMatrix;
            OpenTK.Graphics.OpenGL.GL.GetFloat(OpenTK.Graphics.OpenGL.GetPName.ModelviewMatrix, out modelViewMatrix); GetError(true);
            OpenTK.Matrix4 modelViewMatrix_inv = modelViewMatrix.Inverted();
            


            Vector3 rayOrigin = new Vector3((float)modelViewMatrix_inv.ExtractTranslation().X,
                                            (float)modelViewMatrix_inv.ExtractTranslation().Y,
                                            (float)modelViewMatrix_inv.ExtractTranslation().Z);



            OpenTK.Matrix4d projMatrix;
            OpenTK.Graphics.OpenGL.GL.GetDouble(OpenTK.Graphics.OpenGL.GetPName.ProjectionMatrix, out projMatrix); GetError(true);

            int[] view = new int[4];
            OpenTK.Graphics.OpenGL.GL.GetInteger(OpenTK.Graphics.OpenGL.GetPName.Viewport, view); GetError(true);

            Vector3 screen_space;

            // device space to normalized screen space (NDC)
            screen_space.X = (((2.0f * (float)x) / view[2] - view[0]) - 1) / (float)projMatrix.M11; //.right.X;
            screen_space.Y = -(((2.0f * (float)y) / view[3] - view[1]) - 1) / (float)projMatrix.M22;
            screen_space.Z = -1.0f;

            Matrix4 m4 = new Matrix4((float)modelViewMatrix_inv.M11,
                                    (float)modelViewMatrix_inv.M12,
                                    (float)modelViewMatrix_inv.M13,
                                    (float)modelViewMatrix_inv.M14,
                                    (float)modelViewMatrix_inv.M21,
                                    (float)modelViewMatrix_inv.M22,
                                    (float)modelViewMatrix_inv.M23,
                                    (float)modelViewMatrix_inv.M24,
                                    (float)modelViewMatrix_inv.M31,
                                    (float)modelViewMatrix_inv.M32,
                                    (float)modelViewMatrix_inv.M33,
                                    (float)modelViewMatrix_inv.M34,
                                    (float)modelViewMatrix_inv.M41,
                                    (float)modelViewMatrix_inv.M42,
                                    (float)modelViewMatrix_inv.M43,
                                    (float)modelViewMatrix_inv.M44);

            Vector3 rayDir = Vector3.TransformVector(screen_space, m4);

            rayDir.Normalize();

            return new Algebra.Raytracing.Ray(rayOrigin, rayDir);

            //Vector4 dir4 = UnProject(projMatrix.ToSyscon(),
            //            modelViewMatrix.ToSyscon(),
            //            new Size(view[2] - view[0],
            //                     view[3] - view[1]),
            //            new Vector2(x, view[3] - y));

            //Algebra.Vector3 dir = new Algebra.Vector3(dir4.X, dir4.Y, dir4.Z); 

            ////dir.Normalize();
            //return new Algebra.Raytracing.Ray(origin, new Vector3((float)dir.X, (float)dir.Y, (float)dir.Z));
            ////return new Algebra.Raytracing.Ray(new Vector3((float)dir.X, (float)dir.Y, (float)dir.Z));

        }
        #endregion

        #region DisplayList
        public override Renderer.DisplayList CreateDisplayList()
        {
            DisplayList ret = new DisplayListGL();
            GetError(true);
            return ret;//new DisplayListGL();
        }

        public override int GenLists(int n)
        {
            int ret = GL.GenLists(n);
            GetError(true);
            return ret;// OpenTK.Graphics.OpenGL.GL.GenLists(n);
        }

        public override void ListBase(int listId)
        {
            OpenTK.Graphics.OpenGL.GL.ListBase(listId); GetError(true);
        }
        public override void ListBase(uint listId)
        {
            OpenTK.Graphics.OpenGL.GL.ListBase(listId); GetError(true);
        }

        public override void CallList(int listId)
        {
            OpenTK.Graphics.OpenGL.GL.CallList(listId); GetError(true);
        }
        public override void CallList(uint listId)
        {
            OpenTK.Graphics.OpenGL.GL.CallList(listId); GetError(true);
        }
        
        public override void CallLists(int n, ListNameType type, IntPtr lists)
        {
            OpenTK.Graphics.OpenGL.GL.CallLists(n, type.ToOpenGL(), lists); GetError(true);
        }
        /// <summary>
        /// Execute	a list of display lists. Automatically uses the GL_UNSIGNED_INT version of the function.
        /// </summary>
        /// <param name="n">The number of lists.</param>
        /// <param name="lists">The lists.</param>
        public override void CallLists(int n, uint[] lists)
        {
            OpenTK.Graphics.OpenGL.GL.CallLists(n, ListNameType.UnsignedInt.ToOpenGL(), lists); GetError(true);
        }
        /// <summary>
        /// Execute	a list of display lists. Automatically uses the GL_UNSIGNED_BYTE version of the function.
        /// </summary>
        /// <param name="n">The number of lists.</param>
        /// <param name="lists">The lists.</param>
        public override void CallLists(int n, byte[] lists)
        {
            OpenTK.Graphics.OpenGL.GL.CallLists(n, ListNameType.UnsignedByte.ToOpenGL(), lists); GetError(true);
        }

        #endregion

        #region VBO
        //[Obsolete("Use RendererGL.Buffers instead")]
        public override VboBuffer CreateVBO_VAO(Mesh mesh)
        {
#if _LOCK
            lock (m_lock)
            {
#endif

            List<Vector3> tmpv = new List<Vector3>();          // Positions
            List<uint> tmpi = new List<uint>();             // Indices for triangles (2 per Face which is a Quad)
            List<Vector3> tmpn = new List<Vector3>();          // Normals
            List<Vector2> tmpuv = new List<Vector2>();          // Texture Coords

            for (int p = 0; p < mesh.Positions.Count; p++)
                tmpv.Add(mesh.Positions[p].Position);

            for (int f = 0; f < mesh.Faces.Count; f++)
            {
                Face face = mesh.Faces[f];

                //tmpn.Add(face.Normal);
                //tmpuv.Add(new Vector2(mesh.UVs[f].U, mesh.UVs[f].V)); 

                tmpi.Add(face.VertexIndices[0].PositionIndex);
                tmpi.Add(face.VertexIndices[1].PositionIndex);
                tmpi.Add(face.VertexIndices[2].PositionIndex);

                if (face.VertexIndices.Count == 4)
                {
                    tmpi.Add(face.VertexIndices[2].PositionIndex);
                    tmpi.Add(face.VertexIndices[3].PositionIndex);
                    tmpi.Add(face.VertexIndices[0].PositionIndex);
                }
            }


            VboBuffer handle = new VboBuffer();
            handle.NumElements = tmpi.Count;

            #region Create VBOs
            OpenTK.Graphics.OpenGL.GL.GenBuffers(1, out handle.positionsHandle); GetError(true);
            
            OpenTK.Graphics.OpenGL.GL.BindBuffer(BufferTarget.ArrayBuffer.ToOpenGL(), handle.positionsHandle); GetError(true);
            OpenTK.Graphics.OpenGL.GL.BufferData<Vector3>(BufferTarget.ArrayBuffer.ToOpenGL(),
                                   new IntPtr(tmpv.ToArray().Length * Vector3.SizeInBytes),
                                   tmpv.ToArray(),
                                   BufferUsageHint.StaticDraw.ToOpenGL()); GetError(true);



            OpenTK.Graphics.OpenGL.GL.GenBuffers(1, out handle.normalHandle); GetError(true);
            OpenTK.Graphics.OpenGL.GL.BindBuffer(BufferTarget.ArrayBuffer.ToOpenGL(), handle.normalHandle); GetError(true);
            OpenTK.Graphics.OpenGL.GL.BufferData<Vector3>(BufferTarget.ArrayBuffer.ToOpenGL(),
                                   new IntPtr(tmpn.ToArray().Length * Vector3.SizeInBytes),
                                   tmpn.ToArray(),
                                   BufferUsageHint.StaticDraw.ToOpenGL()); GetError(true);

            OpenTK.Graphics.OpenGL.GL.GenBuffers(1, out handle.indicesHandle); GetError(true);
            OpenTK.Graphics.OpenGL.GL.BindBuffer(BufferTarget.ElementArrayBuffer.ToOpenGL(), handle.indicesHandle); GetError(true);
            OpenTK.Graphics.OpenGL.GL.BufferData(BufferTarget.ElementArrayBuffer.ToOpenGL(),
                          new IntPtr(sizeof(uint) * tmpi.ToArray().Length),
                          tmpi.ToArray(),
                          BufferUsageHint.StaticDraw.ToOpenGL()); GetError(true);

            OpenTK.Graphics.OpenGL.GL.BindBuffer(BufferTarget.ArrayBuffer.ToOpenGL(), 0); GetError(true);
            OpenTK.Graphics.OpenGL.GL.BindBuffer(BufferTarget.ElementArrayBuffer.ToOpenGL(), 0); GetError(true);
            #endregion

            #region Create VAOs
            // GL3 allows us to store the vertex layout in a "vertex array object" (VAO).
            // This means we do not have to re-issue VertexAttribPointer calls
            // every time we try to use a different vertex layout - these calls are
            // stored in the VAO so we simply need to bind the correct VAO.
            OpenTK.Graphics.OpenGL.GL.GenVertexArrays(1, out handle.vaoHandle); GetError(true);
            OpenTK.Graphics.OpenGL.GL.BindVertexArray(handle.vaoHandle); GetError(true);

            OpenTK.Graphics.OpenGL.GL.EnableVertexAttribArray(0); GetError(true);
            OpenTK.Graphics.OpenGL.GL.BindBuffer(BufferTarget.ArrayBuffer.ToOpenGL(), handle.positionsHandle); GetError(true);
            OpenTK.Graphics.OpenGL.GL.VertexAttribPointer(0, 3, OpenTK.Graphics.OpenGL.VertexAttribPointerType.Float, true, Vector3.SizeInBytes, 0); GetError(true);

            OpenTK.Graphics.OpenGL.GL.EnableVertexAttribArray(1); GetError(true);
            OpenTK.Graphics.OpenGL.GL.BindBuffer(BufferTarget.ArrayBuffer.ToOpenGL(), handle.normalHandle); GetError(true);
            OpenTK.Graphics.OpenGL.GL.VertexAttribPointer(1, 3, OpenTK.Graphics.OpenGL.VertexAttribPointerType.Float, true, Vector3.SizeInBytes, 0); GetError(true);

            OpenTK.Graphics.OpenGL.GL.BindBuffer(BufferTarget.ElementArrayBuffer.ToOpenGL(), handle.indicesHandle); GetError(true);

            OpenTK.Graphics.OpenGL.GL.BindVertexArray(0); GetError(true);
            #endregion

            handle.generated = true;

#if _LOCK
            }
#endif

            return handle;
        }
        #endregion

        #region VAO

        #endregion

        #region TrueTypeFont2D
        public int CreateTrueTypeFont2D(Font font, bool antializing)
        {
            TrueTypeFont ttf = new TrueTypeFont(font, antializing);

            foreach (KeyValuePair<int, TrueTypeFont> kvp in m_trueTypeFonts)
            {
                if (kvp.Value == ttf)
                    return -1;
            }

            int fontKey = m_trueTypeFonts.Count;
            m_trueTypeFonts.Add(fontKey, ttf);
            return fontKey;
        }
        #endregion

    }
}
