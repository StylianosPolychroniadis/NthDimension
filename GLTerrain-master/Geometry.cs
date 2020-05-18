using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace GLTerrain {
    public class Geometry : IDisposable {
        public Vector3 Position { get; set; }
        public Vector3 Scale { get; set; }
        public Quaternion Orientation { get; set; }
        public BeginMode DrawMode { get; set; }

        private VertexBuffer buffer = null;
        private int displayList = 0;

        public Geometry(BeginMode drawMode, VertexBuffer buffer) {
            this.buffer = buffer;
            DrawMode = drawMode;
            Position = new Vector3();
            Scale = new Vector3(1, 1, 1);
            Orientation = Quaternion.Identity;
        }

        public void Draw() {
            if (buffer.IsVBO) {
                GL.EnableClientState(ArrayCap.VertexArray);
                GL.EnableClientState(ArrayCap.NormalArray);
                GL.EnableClientState(ArrayCap.TextureCoordArray);

                if (buffer.ColorsID > 0) {
                    GL.EnableClientState(ArrayCap.ColorArray);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, buffer.ColorsID);
                    GL.ColorPointer(4, ColorPointerType.Float, 0, 0);
                }

                GL.BindBuffer(BufferTarget.ArrayBuffer, buffer.ID);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, buffer.IndicesID);

                int stride = BlittableValueType.StrideOf(buffer.Vertices);
                GL.TexCoordPointer(2, TexCoordPointerType.Float, stride, 12);
                GL.NormalPointer(NormalPointerType.Float, stride, 20);
                GL.VertexPointer(3, VertexPointerType.Float, stride, 0);

                Matrix4 transform = GetTransform();
                GL.PushMatrix();
                {
                    GL.MultMatrix(ref transform);

                    if (buffer.IndicesSize == 2) {
                        GL.DrawElements(DrawMode, buffer.NumIndices, DrawElementsType.UnsignedShort, 0);
                    } else {
                        GL.DrawElements(DrawMode, buffer.NumIndices, DrawElementsType.UnsignedInt, 0);
                    }
                }
                GL.PopMatrix();

                GL.DisableClientState(ArrayCap.VertexArray);
                GL.DisableClientState(ArrayCap.NormalArray);
                GL.DisableClientState(ArrayCap.TextureCoordArray);

                if (buffer.ColorsID > 0) {
                    GL.DisableClientState(ArrayCap.ColorArray);
                }

            } else {
                if (displayList == 0) {
                    CreateDisplayList(DrawMode);
                }

                Matrix4 transform = GetTransform();
                GL.PushAttrib(AttribMask.CurrentBit);
                GL.PushMatrix();
                {
                    GL.Color4(1.0f, 1.0f, 1.0f, 1.0f);
                    GL.MultMatrix(ref transform);

                    GL.CallList(displayList);
                }
                GL.PopMatrix();
                GL.PopAttrib();
            }
        }

        private void CreateDisplayList(BeginMode mode) {
            if (!buffer.IsValid) {
                return;
            }

            if (displayList > 0) {
                GL.DeleteLists(displayList, 1);
            }

            displayList = GL.GenLists(1);
            GL.NewList(displayList, ListMode.Compile);

            GL.Begin(mode);
            for (int index = 0; index < buffer.Vertices.Length; index++) {
                
                if (buffer.TextureCoords != null) {
                    GL.TexCoord2(buffer.TextureCoords[index].U, buffer.TextureCoords[index].V);
                }

                if (buffer.Colors != null) {
                    GL.Color4(buffer.Colors[index]);
                }

                GL.Vertex3(buffer.Vertices[index].X, buffer.Vertices[index].Y, buffer.Vertices[index].Z);
            }
            GL.End();

            GL.EndList();
        }

        public Matrix4 GetTransform() {
            return Matrix4.Scale(Scale) * Matrix4.CreateTranslation(Position) * Matrix4.Rotate(Orientation);
        }

        public void Dispose() {
            if (displayList > 0) {
                GL.DeleteLists(displayList, 1);
            }
            buffer.Dispose();
        }
    }
}
