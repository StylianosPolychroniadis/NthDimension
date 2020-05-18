using System;
using System.Drawing;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace GLTerrain {
    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct Vertex {
        public float X;
        public float Y;
        public float Z;
        public float S;
        public float T;
        public Vector3 Normal;

        public static Vector3 PositionToVec3(Vertex toConvert) {
            return new Vector3(toConvert.X, toConvert.Y, toConvert.Z);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct TexCoord {
        public float U;
        public float V;
    }

    public class VertexBuffer : IDisposable {
        public int ID { get; private set; }
        public int IndicesID { get; private set; }
        public int ColorsID { get; private set; }

        public Vertex[] Vertices { get; private set;}
        public TexCoord[] TextureCoords { get; private set; }
        public Color4[] Colors { get; private set; }
        public bool IsVBO { get; private set; }
        public int IndicesSize { get; private set; }
        public int NumIndices { get; private set; }

        public bool IsValid {
            get {
                if (Vertices != null) {
                    if (TextureCoords != null) {
                        if (Colors != null) {
                            return Vertices.Length == TextureCoords.Length &&
                                   Vertices.Length == Colors.Length;
                        } else {
                            return Vertices.Length == TextureCoords.Length;
                        }
                    }
                    return true;
                }
                return false;
            }
        }

        public VertexBuffer(Vertex[] vertices) {
            Vertices = vertices;
            ID = 0;
            IndicesID = 0;
            IndicesSize = 0;
            NumIndices = 0;
            IsVBO = false;
        }

        public VertexBuffer(Vertex[] vertices, ushort[] indices) {
            Vertices = vertices;
            NumIndices = indices.Length;

            int id = -1;
            GL.GenBuffers(1, out id);
            ID = id;

            GL.BindBuffer(BufferTarget.ArrayBuffer, id);
            GL.BufferData(
                BufferTarget.ArrayBuffer, 
                (IntPtr)(vertices.Length * BlittableValueType.StrideOf(vertices)), 
                vertices, 
                BufferUsageHint.StaticDraw);

            GL.GenBuffers(1, out id);
            IndicesID = id;

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndicesID);
            GL.BufferData(
                BufferTarget.ElementArrayBuffer, 
                (IntPtr) (indices.Length * sizeof(ushort)), 
                indices, 
                BufferUsageHint.StaticDraw);

            IsVBO = true;
            IndicesSize = 2;
        }

        public VertexBuffer(Vertex[] vertices, uint[] indices) {
            Vertices = vertices;
            NumIndices = indices.Length;

            int id = -1;
            GL.GenBuffers(1, out id);
            ID = id;

            GL.BindBuffer(BufferTarget.ArrayBuffer, id);
            GL.BufferData(
                BufferTarget.ArrayBuffer,
                (IntPtr)(vertices.Length * BlittableValueType.StrideOf(vertices)),
                vertices,
                BufferUsageHint.StaticDraw);

            GL.GenBuffers(1, out id);
            IndicesID = id;

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndicesID);
            GL.BufferData(
                BufferTarget.ElementArrayBuffer,
                (IntPtr)(indices.Length * sizeof(uint)),
                indices,
                BufferUsageHint.StaticDraw);

            IsVBO = true;
            IndicesSize = 4;
        }

        public VertexBuffer(Vertex[] vertices, TexCoord[] texCoords) 
            : this(vertices) {
            TextureCoords = texCoords;
        }

        public VertexBuffer(Vertex[] vertices, Color4[] colors) 
            : this(vertices) {
            Colors = colors;
        }

        public VertexBuffer(Vertex[] vertices, uint[] indices, Color4[] colors)
            : this(vertices, indices) {

            Colors = colors;
            int id = -1;

            GL.GenBuffers(1, out id);
            ColorsID = id;

            GL.BindBuffer(BufferTarget.ArrayBuffer, id);
            GL.BufferData(
                BufferTarget.ArrayBuffer,
                (IntPtr)(colors.Length * BlittableValueType.StrideOf(colors)),
                colors,
                BufferUsageHint.StaticDraw);
        }

        public VertexBuffer(Vertex[] vertices, ushort[] indices, Color4[] colors)
            : this(vertices, indices) {

            Colors = colors;
            int id = -1;

            GL.GenBuffers(1, out id);
            ColorsID = id;

            GL.BindBuffer(BufferTarget.ArrayBuffer, id);
            GL.BufferData(
                BufferTarget.ArrayBuffer,
                (IntPtr)(colors.Length * BlittableValueType.StrideOf(colors)),
                colors,
                BufferUsageHint.StaticDraw);
        }

        public void Dispose() {
            int id = ID;
            GL.DeleteBuffers(1, ref id);

            id = IndicesID;
            GL.DeleteBuffers(1, ref id);
        }
    }
}
