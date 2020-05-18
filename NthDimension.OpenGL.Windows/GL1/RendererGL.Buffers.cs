using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NthDimension.Algebra;
using NthDimension.Algebra.Geometry;
using NthDimension.Graphics.Buffers;
using NthDimension.Graphics.Modelling;
using NthDimension.Graphics.Renderer;

using OpenTK.Graphics.OpenGL;
using NthDimension.Graphics.Geometry;

namespace NthDimension.Rasterizer.GL1
{
    public partial class RendererGL1x : RendererBase
    {
        public override void DeleteBuffer(int buffers)
        {
            GL.DeleteBuffers(1, ref buffers); GetError(true);
        }
        public override void DeleteBuffers(int n, ref int buffers)
        {
            int id = 0;
            GL.DeleteBuffers(n, ref id); GetError(true);

        }
        public override void DrawArrays(NthDimension.Graphics.Geometry.enuPolygonMode mode, int first, int count)
        {
            GL.DrawArrays(mode.ToOpenGL(), first, count); GetError(true);
        }
        public override void DrawArraysInstanced(NthDimension.Graphics.Geometry.enuPolygonMode mode, int first, int count, int instancecount)
        {
            GL.DrawArraysInstanced(mode.ToOpenGL(), first, count, instancecount); GetError(true);
        }
        public override void BindBuffer(BufferTarget target, int buffer)
        {
            GL.BindBuffer(target.ToOpenGL(), buffer); GetError(true);
        }
        //public override int GenBuffer()
        //{
        //    return GL.GenBuffer();
        //}
        public override void GenBuffers(int n, out int buffers)
        {
            GL.GenBuffers(n, out buffers); GetError(true);
        }
        public override void BufferData<T2>(BufferTarget target, IntPtr size, T2[] data, BufferUsageHint usage)
        {
             GL.BufferData<T2>(target.ToOpenGL(), size, data, usage.ToOpenGL()); GetError(true);
        }
        public override void GetBufferParameter(BufferTarget target, BufferParameterName name, out int result)
        {
            GL.GetBufferParameter(target.ToOpenGL(), name.ToOpenGL(), out result); GetError(true);
        }
        public override int GenVertexArray()
        {
            int res = -1;
            GL.GenVertexArrays(1, out res); GetError(true);
            return res;
        }
        public override void BindVertexArray(int vao)
        {
            GL.BindVertexArray(vao); GetError(true);
        }
        public override void PushClientAttrib(ClientAttribMask mask)
        {
            GL.PushClientAttrib(mask.ToOpenGL()); GetError(true);
        }
        public override void PopClientAttrib()
        {
            GL.PopClientAttrib(); GetError(true);
        }

        /////////////////////////////////////////

        public override void drawVertexBuffer(Model model)
        {
            //// To draw a VBO:
            //// 1) Ensure that the VertexArray client state is enabled.
            //// 2) Bind the vertex and element buffer handles.
            //// 3) Set up the data pointers (vertex, normal, color) according to your vertex format.
            //// 4) Call DrawElements. (Note: the last parameter is an offset into the element buffer
            ////    and will usually be IntPtr.Zero).

            //GL.EnableClientState(ArrayCap.ColorArray);
            //GL.EnableClientState(ArrayCap.VertexArray);

            //GL.BindBuffer(BufferTarget.ArrayBuffer.ToOpenGL(),
            //              model.VBO.positionsHandle);

            //GL.BindBuffer(BufferTarget.ElementArrayBuffer.ToOpenGL(),
            //              model.VBO.indicesHandle);

            //GL.VertexPointer(3, VertexPointerType.Float,
            //                 OpenTK.BlittableValueType.StrideOf(Vector3),
            //                 new IntPtr(0));
            ////GL.ColorPointer(4, ColorPointerType.UnsignedByte, 
            ////                BlittableValueType.StrideOf(CubeVertices), 
            ////                new IntPtr(12));

            //GL.DrawElements(PrimitiveType.Triangles.ToOpenGL(),
            //                model.VBO.NumElements,
            //                OpenTK.Graphics.OpenGL.DrawElementsType.UnsignedShort,
            //                IntPtr.Zero);
        }
        public override void drawVertexBuffer(NthDimension.Graphics.Geometry.enuPolygonMode primitive, VboBuffer vbo, Mesh mesh)
        {
            GL.EnableClientState(OpenTK.Graphics.OpenGL.EnableCap.ColorArray); GetError(true);
            //GL.EnableClientState(EnableCap.VertexArray);

            
            OpenTK.Graphics.OpenGL.GL.BindBuffer(BufferTarget.ArrayBuffer.ToOpenGL(), vbo.positionsHandle); GetError(true);
            OpenTK.Graphics.OpenGL.GL.BindBuffer(BufferTarget.ArrayBuffer.ToOpenGL(), vbo.normalHandle); GetError(true);
            OpenTK.Graphics.OpenGL.GL.BindBuffer(BufferTarget.ElementArrayBuffer.ToOpenGL(), vbo.indicesHandle); GetError(true);

            GL.VertexPointer(3, VertexPointerType.Float.ToOpenGL(), mesh.Positions.Count * Vector3.SizeInBytes, new IntPtr(0)); GetError(true);
            //GL.ColorPointer(4, ColorPointerType.UnsignedByte, BlittableValueType.StrideOf(mesh.Colors), new IntPtr(12));

            GL.DrawElements(primitive.ToOpenGL(), vbo.NumElements, OpenTK.Graphics.OpenGL.DrawElementsType.UnsignedShort, IntPtr.Zero); GetError(true);
        }
        public override void drawVertexArray(Model model)   // Works OK on VBO Rednering
        {
            //throw new NotImplementedException();
            //GL.BindVertexArray(0);
            GL.BindVertexArray(model.VBO.vaoHandle); GetError(true);
            GL.DrawElements(model.Mesh.PolygonMode.ToOpenGL(), model.VBO.NumElements, DrawElementsType.UnsignedInt.ToOpenGL(), 0); GetError(true);
            GL.BindVertexArray(0); GetError(true);
        }
        public override void drawVertexArray(NthDimension.Graphics.Geometry.enuPolygonMode primitive, VboBuffer vbo)
        {
            GL.BindVertexArray(vbo.vaoHandle); GetError(true);
            GL.DrawElements(primitive.ToOpenGL(), vbo.NumElements, DrawElementsType.UnsignedInt.ToOpenGL(), 0); GetError(true);
            GL.BindVertexArray(0); GetError(true);
        }


        // WORKSPACE
        //        #region [ OpenGL 3.x ] IVbo Members
        //        // Multiple layers of Vertex Buffer Objects

        //        // WARNING:: Do NOT MODIFY! functions are OK
        //        public override Drawables.VertexBuffer CreateVertexBuffer(Algebra.Vector3[] vertices, Algebra.Vector2[] texcoords, Algebra.Vector3[] normals)
        //        {
        //            throw new NotImplementedException();
        //        }

        //        public override Drawables.VertexBuffer CreateVertexBuffer<T>(Scenegraph.SceneElement element)
        //        {
        //            throw new NotImplementedException();
        //        }


        //        public override void CreateVertexBuffer(Model model)
        //        {
        //#if _LOCK
        //            lock (m_lock)
        //            {
        //#endif
        //            model.VBO = LoadVBO(model.VertexArray, model.IndexArray);



        //            List<VertexT2dN3dV3d> tmpv = new List<VertexT2dN3dV3d>();
        //            List<uint> tmpe = new List<uint>();

        //            for (int f = 0; f < model.Mesh.QuadFaces.Count; f++)
        //            {
        //                Face face = model.Mesh.QuadFaces[f];

        //                for (int v = 0; v < face.VertexIndices.Count; v++)
        //                {
        //                    VertexIndex vi = face.VertexIndices[v];

        //                    Vector2d uv = new Vector2d(1f, 1f);
        //                    //Vector2d uv = new Vector2d(modelMesh.UVs[(int)vi.UVCoordIndex].U,
        //                    //                           modelMesh.UVs[(int)vi.UVCoordIndex].V);

        //                    Vector3d normal = new Vector3d(model.Mesh.Normals[(int)vi.NormalIndex].n1.X,
        //                                                   model.Mesh.Normals[(int)vi.NormalIndex].n1.Y,
        //                                                   model.Mesh.Normals[(int)vi.NormalIndex].n1.Z);

        //                    Vector3d position = new Vector3d(model.Mesh.Positions[(int)vi.PositionIndex].Position.X,
        //                                                     model.Mesh.Positions[(int)vi.PositionIndex].Position.Y,
        //                                                     model.Mesh.Positions[(int)vi.PositionIndex].Position.Z);

        //                    tmpv.Add(new VertexT2dN3dV3d(uv,
        //                                                normal,
        //                                                position));

        //                    tmpe.Add(vi.PositionIndex);
        //                }


        //            }
        //            model.VertexArray = tmpv.ToArray();
        //            model.IndexArray = tmpe.ToArray();

        //            model.VBO = LoadVBO(model.VertexArray, model.IndexArray);
        //#if _LOCK
        //            }
        //#endif
        //            //return model.VBO;
        //        }
        public VboBuffer LoadVBO<TVertex>(TVertex[] vertices, uint[] elements) where TVertex : struct
        {
            // TODO:: See MeshLoader.generateVBO from ultrasandbox
            // TODO:: Move to Renderer.OpenGL

            VboBuffer handle = new VboBuffer();
            int size;

            // To create a VBO:
            // 1) Generate the buffer handles for the vertex and element buffers.
            // 2) Bind the vertex buffer handle and upload your vertex data. Check that the buffer was uploaded correctly.
            // 3) Bind the element buffer handle and upload your element data. Check that the buffer was uploaded correctly.

            GL.GenBuffers(1, out handle.positionsHandle); GetError(true);
            GL.GenBuffers(1, out handle.normalHandle); GetError(true);
            GL.GenBuffers(1, out handle.textureCoordsHandle); GetError(true);
            GL.GenBuffers(1, out handle.tangentHandle); GetError(true);
            GL.GenBuffers(1, out handle.indicesHandle); GetError(true);

            //if (affectingBonesCount > 0)
            //{
            //    _renderer.GenBuffers(affectingBonesCount, target.boneWeightVboHandles);
            //    _renderer.GenBuffers(affectingBonesCount, target.boneIdVboHandles);
            //}

            #region Bind Positions
            GL.BindBuffer(BufferTarget.ArrayBuffer.ToOpenGL(), handle.positionsHandle); GetError(true);
            GL.BufferData(BufferTarget.ArrayBuffer.ToOpenGL(), (IntPtr)(vertices.Length * OpenTK.BlittableValueType.StrideOf(vertices)), vertices,
                          BufferUsageHint.StaticDraw.ToOpenGL()); GetError(true);


            GL.GetBufferParameter(BufferTarget.ArrayBuffer.ToOpenGL(), BufferParameterName.BufferSize.ToOpenGL(), out size); GetError(true);
            if (vertices.Length * OpenTK.BlittableValueType.StrideOf(vertices) != size)
                throw new ApplicationException("Vertex data not uploaded correctly");
            #endregion

            #region Bind Normals

            #endregion

            #region Bind Texture Coords

            #endregion

            #region Bind Tangents

            #endregion

            #region Bind Bones

            #endregion

            #region Bind Elements
            GL.BindBuffer(BufferTarget.ElementArrayBuffer.ToOpenGL(), handle.indicesHandle); GetError(true);
            GL.BufferData(BufferTarget.ElementArrayBuffer.ToOpenGL(), (IntPtr)(elements.Length * sizeof(short)), elements,
                          BufferUsageHint.StaticDraw.ToOpenGL()); GetError(true);
            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer.ToOpenGL(), BufferParameterName.BufferSize.ToOpenGL(), out size); GetError(true);
            if (elements.Length * sizeof(short) != size)
                throw new ApplicationException("Element data not uploaded correctly");

            handle.NumElements = elements.Length;
            #endregion

            handle.generated = true;

            return handle;
        }
        //        public override void drawVertexBuffer(Model model)
        //        {
        //            // To draw a VBO:
        //            // 1) Ensure that the VertexArray client state is enabled.
        //            // 2) Bind the vertex and element buffer handles.
        //            // 3) Set up the data pointers (vertex, normal, color) according to your vertex format.
        //            // 4) Call DrawElements. (Note: the last parameter is an offset into the element buffer
        //            //    and will usually be IntPtr.Zero).

        //            //_renderer.EnableClientState(EnableCap.ColorArray);
        //            GL.EnableClientState(EnableCap.TextureCoordArray);
        //            GL.EnableClientState(EnableCap.NormalArray);
        //            GL.EnableClientState(EnableCap.VertexArray);

        //            GL.BindBuffer(BufferTarget.ArrayBuffer.ToOpenGL(), model.VBO.positionsHandle);
        //            GL.BindBuffer(BufferTarget.ElementArrayBuffer.ToOpenGL(), model.VBO.indicesHandle);

        //            GL.TexCoordPointer(2, TexCoordPointerType.Float, OpenTK.BlittableValueType.StrideOf(model.VertexArray), new IntPtr(0)); // TODO:: Vertex arrays to all 3 calls are you sure???
        //            GL.NormalPointer(NormalPointerType.Float, OpenTK.BlittableValueType.StrideOf(model.VertexArray), new IntPtr(0));
        //            GL.VertexPointer(3, VertexPointerType.Float, OpenTK.BlittableValueType.StrideOf(model.VertexArray), new IntPtr(0));

        //            //_renderer.ColorPointer(4, ColorPointerType.UnsignedByte, BlittableValueType.StrideOf(VertexArray), new IntPtr(12));


        //            GL.DrawElements(model.PrimitiveMode.ToOpenGL(), // TODO:: Refactor to OpenGL10 & implement OpenGL 4.0
        //                                model.VBO.NumElements,
        //                                DrawElementsType.UnsignedInt,
        //                                IntPtr.Zero);
        //        }
        //        #endregion

        #region [ OpenGL 4.x ] VAO Members

        #endregion

        /////////////////////////////////////// FrameBuffer /////////////////////////////////////////////////

        public override void GenFramebuffers(int n, out int framebuffers)
        {
            GL.GenFramebuffers(n, out framebuffers); GetError(true);
        }
        public override void GenRenderbuffers(int n, out int renderbuffers)
        {
            GL.GenRenderbuffers(n, out renderbuffers); GetError(true);
        }
        public override void BindFramebuffer(FramebufferTarget target, int framebuffer)
        {
            GL.BindFramebuffer(target.ToOpenGL(), framebuffer); GetError(true);
        }
        public override void BindRenderbuffer(RenderbufferTarget target, int renderbuffer)
        {
            GL.BindRenderbuffer(target.ToOpenGL(), renderbuffer); GetError(true);
        }
        public override void DeleteFramebuffers(int n, ref int framebuffers)
        {
            GL.DeleteFramebuffers(n, ref framebuffers); GetError(true);
        }
        public override void DeleteRenderbuffers(int n, ref int renderbuffers)
        {
            GL.DeleteRenderbuffers(n, ref renderbuffers); GetError(true);
        }
        public override void FramebufferTexture2D(FramebufferTarget target, FramebufferAttachment attachment, TextureTarget textarget, int texture, int level)
        {
            GL.FramebufferTexture2D(target.ToOpenGL(),
                                    attachment.ToOpenGL(),
                                    textarget.ToOpenGL(),
                                    texture, 
                                    level); GetError(true);
        }
        public override void FramebufferRenderbuffer(FramebufferTarget target, FramebufferAttachment attachment, RenderbufferTarget renderbuffertarget, int renderbuffer)
        {
            GL.FramebufferRenderbuffer(target.ToOpenGL(),
                                       attachment.ToOpenGL(), 
                                       renderbuffertarget.ToOpenGL(), 
                                       renderbuffer); GetError(true);
        }
        public override void RenderbufferStorage(RenderbufferTarget target, RenderbufferStorage internalformat, int width, int height)
        {
            GL.RenderbufferStorage(target.ToOpenGL(),
                                    internalformat.ToOpenGL(), 
                                    width, 
                                    height); GetError(true);
        }
        public override FramebufferErrorCode CheckFramebufferStatus(FramebufferTarget target)
        {
            return GL.CheckFramebufferStatus(target.ToOpenGL()).ToSyscon(); GetError(true);
        }

    }
}
