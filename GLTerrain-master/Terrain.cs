using System;
using System.Drawing;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace GLTerrain {
    public class Terrain : IDisposable {
        public Vector3 CenterPoint { 
            get {
                return geometry.Position;
            }
            set {
                geometry.Position = value;
                normals.Position = value;
            }
        }

        public Quaternion Orientation {
            get {
                return geometry.Orientation;
            }
            set {
                geometry.Orientation = value;
                normals.Orientation = value;
            }
        }

        public ShaderProgram Shader { get; set; }

        public float Width { get; private set; }
        public float Height { get; private set; }
        public float TextureScale { get; private set; }
        public float HeightScale { get; private set; }

        public int NumTrisWide { get; private set; }
        public int NumTrisTall { get; private set; }
        public int NumVertices { get; private set; }
        public int NumTriangles { get; private set; }

        public bool DrawNormals { get; set; }

        private Texture texture = null;
        private Geometry geometry = null;
        private Geometry normals = null;

        public delegate float HeightFunc(float x, float z);

        public Terrain(
            float width,
            float height,
            int subdivisionsWide,
            int subdivisionsTall,
            string heightMapFile,
            float heightScale,
            string textureFile,
            float textureScale) {

            Width = width;
            Height = height;
            NumTrisWide = subdivisionsWide;
            NumTrisTall = subdivisionsTall;
            TextureScale = textureScale;
            HeightScale = heightScale;

            texture = new Texture(textureFile, false);

            Vertex[] verts = null;

            if (String.IsNullOrEmpty(heightMapFile)) {
                verts = CreatePlane(width, height, subdivisionsWide, subdivisionsTall, (x,y) => 0.0f);
            } else {

                // load the bitmap data
                int wide = 0, tall = 0;
                byte[] imagePixels = null;

                using (Bitmap bitmap = new Bitmap(heightMapFile)) {
                    wide = bitmap.Width;
                    tall = bitmap.Height;

                    System.Drawing.Imaging.BitmapData data =
                        bitmap.LockBits(
                            new Rectangle(0, 0, wide, tall),
                            System.Drawing.Imaging.ImageLockMode.ReadOnly,
                            System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    int imageSize = data.Stride * data.Height;
                    imagePixels = new byte[imageSize];

                    Marshal.Copy(data.Scan0, imagePixels, 0, imageSize);

                    bitmap.UnlockBits(data);
                }

                verts = CreatePlane(width, height, subdivisionsWide, subdivisionsTall, (x, y) => {
                    float bx = x * wide;
                    float by = y * tall;

                    if (bx >= wide) {
                        bx = wide - 1;
                    }
                    if (by >= tall) {
                        by = tall - 1;
                    }

                    return HeightScale * GetHeightMapValue(imagePixels, bx, by, wide, tall);
                });
            }

            uint[] indices = CreateIndices(subdivisionsWide, subdivisionsTall);
            GenerateNormals(verts, indices);

            geometry = new Geometry(BeginMode.TriangleStrip, new VertexBuffer(verts, indices));
            NumVertices = verts.Length;
            NumTriangles = (NumTrisWide * 2) * NumTrisTall;
        }

        public void Draw() {
            texture.Bind();
            if (Shader != null) {
                Shader.Attach();
            }

            geometry.Draw();

            if (Shader != null) {
                Shader.Detach();
            }
            texture.Unbind();

            if (DrawNormals) {
                GL.PushAttrib(AttribMask.LightingBit | AttribMask.CurrentBit);
                GL.Disable(EnableCap.Lighting);
                normals.Draw();
                GL.PopAttrib();
                GL.Color4(Color4.White);
            }
        }

        private float GetHeightMapValue(byte[] heightMap, float x, float y, int width, int height) {
            int rowSize = width * 4;

            int centerPixelX = (int)x;
            int centerPixelY = (int)y;
            int centerOffset = centerPixelX * 4 + (centerPixelY * rowSize);

            float pixelSum = heightMap[centerOffset];
            int numPixels = 1;

            if (centerPixelX > 0) {
                pixelSum += heightMap[centerOffset - 4];
                numPixels++;
            }

            if (centerPixelX < width - 1) {
                pixelSum += heightMap[centerOffset + 4];
                numPixels++;
            }

            if (centerPixelY > 0) {
                pixelSum += heightMap[centerOffset - rowSize];
                numPixels++;
            }

            if (centerPixelY < height - 1) {
                pixelSum += heightMap[centerOffset + rowSize];
                numPixels++;
            }

            return (pixelSum / numPixels) / 255.0f;
        }

        private void GenerateNormals(Vertex[] verts, uint[] indices) {
            for (int i = 2; i < indices.Length; i++) {
                uint index1 = indices[i - 2],
                     index2 = indices[i - 1],
                     index3 = indices[i];

                Vector3 normal = CalcNormal(verts[index1], verts[index2], verts[index3]);
                if (normal.Y < 0) {
                    normal *= -1.0f;
                }

                verts[index1].Normal += normal;
                verts[index2].Normal += normal;
                verts[index3].Normal += normal;
            }

            Vertex[] normVerts = new Vertex[verts.Length * 2];
            Color4[] colors = new Color4[verts.Length * 2];

            for (int i = 0; i < verts.Length; i++) {
                verts[i].Normal = Vector3.NormalizeFast(verts[i].Normal);
                normVerts[i * 2] = verts[i];
                normVerts[i * 2 + 1] = new Vertex() { 
                    X = verts[i].X + (verts[i].Normal.X * 2), 
                    Y = verts[i].Y + (verts[i].Normal.Y * 2), 
                    Z = verts[i].Z + (verts[i].Normal.Z * 2)
                };
                colors[i * 2] = Color4.Yellow;
                colors[i * 2 + 1] = Color4.Yellow;
            }

            normals = new Geometry(BeginMode.Lines, new VertexBuffer(normVerts, colors));
        }

        private Vector3 CalcNormal(Vertex v1, Vertex v2, Vertex v3) {
            Vector3 first = new Vector3(v2.X - v1.X, v2.Y - v1.Y, v2.Z - v1.Z);
            Vector3 second = new Vector3(v3.X - v1.X, v3.Y - v1.Y, v3.Z - v1.Z);
            return Vector3.NormalizeFast(Vector3.Cross(first, second));
        }

        private Vertex[] CreatePlane(float width, float height, int subX, int subZ, HeightFunc func) {
            int numVerts = (subX + 1) * (subZ + 1);
            Vertex[] verts = new Vertex[numVerts];

            float xSubSize = width / subX;
            float zSubSize = height / subZ;
            float sInc = 1.0f / subX;
            float tInc = 1.0f / subZ;

            float curX = -width / 2.0f;
            float curZ = -height / 2.0f;
            float curS = 0.0f;
            float curT = 0.0f;

            int curVert = 0;

            for (int z = 0; z <= subZ; z++) {
                for (int x = 0; x <= subX; x++) {
                    verts[curVert++] = new Vertex() { 
                        X = curX, 
                        Y = func(curS, curT), 
                        Z = curZ, 
                        S = curS * TextureScale, 
                        T = curT * TextureScale 
                    };
                    curS += sInc;
                    curX += xSubSize;
                }
                curS = 0.0f;
                curT += tInc;
                curX = -width / 2.0f;
                curZ += zSubSize;
            }

            return verts;
        }

        private uint[] CreateIndices(int subX, int subZ) {
            int numIndices = (subX * 2 * (subZ - 1)) + (subZ - 2);
            uint[] ind = new uint[numIndices];

            int vertsWide = subX + 1;
            int curIndex = 0;

            for (int z = 0; z < subZ - 1; z++) {
                // even rows move ->
                // odd rows move <-
                if (z % 2 == 0) {
                    int x = 0;
                    for (; x < subX; x++) {
                        ind[curIndex++] = (uint)(x + (z * vertsWide));
                        ind[curIndex++] = (uint)(x + (z * vertsWide) + vertsWide);
                    }

                    if (z != subZ - 2) {
                        ind[curIndex++] = (uint)(--x + (z * vertsWide));
                    }
                } else {
                    int x = subX - 1;
                    for (; x >= 0; --x) {
                        ind[curIndex++] = (uint)(x + (z * vertsWide));
                        ind[curIndex++] = (uint)(x + (z * vertsWide) + vertsWide);
                    }

                    if (z != subZ - 2) {
                        ind[curIndex++] = (uint)(++x + (z * vertsWide));
                    }
                }
            }

            return ind;
        }

        public void Dispose() {
            geometry.Dispose();
            texture.Dispose();
        }
    }
}
