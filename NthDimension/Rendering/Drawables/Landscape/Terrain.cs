using NthDimension.Algebra;
using NthDimension.Rendering.Geometry;
using NthDimension.Rendering.Serialization;
using NthDimension.Rendering.Shaders;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Rendering.Drawables.Models
{
    /// <summary>
    /// Simple Terrain class. Makes use of 4 different textures (see material definition)
    /// Class is MeshData LOD excluded. Using dynamic terrain LOD
    /// </summary>
    public class Terrain : Model
    {
        new public static string nodename = "tmodel";

        public struct Vertex
        {
            public float X;
            public float Y;
            public float Z;
            public float U;
            public float V;
            public Vector3 Normal;

            public static Vector3 PositionToVec3(Vertex toConvert)
            {
                return new Vector3(toConvert.X, toConvert.Y, toConvert.Z);
            }
        }

        public delegate float HeightFunc(float x, float z);

        const float meshUv              = 1f;

        uint[]      indices;
        ListVector3 positions           = new ListVector3();
        ListVector2 uvs                 = new ListVector2();
        ListVector3 normals             = new ListVector3();
        ListFace    faces               = new ListFace();

        byte[]      imagePixels         = null;      // Hacked to acquire height by x,z
        int         wide = 0, tall      = 0;         // Hacked to acquire height by x,z

        public Vector3      Min                 { get; private set; }   = Vector3.Zero;
        public Vector3      Max                 { get; private set; }   = Vector3.Zero;
        public float        HeightScale         { get; private set; }
        public float        Width               { get; private set; }
        public float        Length              { get; private set; }
        public int          NumVertices         { get; private set; }
        public int          NumTriangles        { get; private set; }

        public int          SubdivisionsWide    { get; private set; }
        public int          SubdivisionsTall    { get; private set; }

        public float[,]     Heights             { get; private set; }

        public float        HeightMin           { get; private set; } 
        public float        HeightMax           { get; private set; } 

        public ListVector3  Points {  get { return positions; } }
        public uint[]       Indices {  get { return indices; } }

        public Vector2      UVScale { get; set; } = new Vector2(1.0f, 1.0f);

        public Terrain(string heightMapFile,
                       string material,
                        float width, float length,
                        int subdivisionsWide, int subdivisionsLong,                         
                        float heightScale,
                        float heightMin,
                        float heightMax)
        {
            PrimitiveType       = Rasterizer.PrimitiveType.TriangleStrip;
            IgnoreLod           = true;

            SubdivisionsWide    = subdivisionsWide;
            SubdivisionsTall    = subdivisionsLong;
         
            HeightScale         = heightScale;
            HeightMin           = heightMin;
            HeightMax           = heightMax;

            this.setMaterial(material);
            
            Vertex[] verts      = null;

            if (string.IsNullOrEmpty(heightMapFile))
            {
                verts = this.createPlane(width, length,
                                         subdivisionsWide, subdivisionsLong,
                                         (x, y) => 0.0f);
            } 
            else 
            {
                using (Bitmap bitmap = new Bitmap(heightMapFile))
                {
                    wide = bitmap.Width;
                    tall = bitmap.Height;

                    Width = wide;
                    Length = tall;

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

                verts = createPlane(width, length, 
                                    subdivisionsWide, subdivisionsLong, 
                                    (x, y) => {
                                        float bx = x * wide;
                                        float by = y * tall;

                                        if (bx >= wide)
                                            bx = wide - 1;

                                        if (by >= tall)
                                            by = tall - 1;

                                        return HeightScale * getHeightmapValueFunction(imagePixels, bx, by, wide, tall);
                                    });
            }

            indices = createIndices(subdivisionsWide, subdivisionsLong);

            this.generateNormals(verts, indices);

            foreach(Vertex v in verts)
            {
                positions.Add(new Vector3(v.X, v.Y, v.Z));
                uvs.Add(new Vector2(v.U, v.V));
            }

            for (int f = 0; f < indices.Length; f += 3)
            {
                try
                {
                    VertexIndices a = new VertexIndices((int)indices[f + 0], (int)indices[f + 0], (int)indices[f + 0]);
                    VertexIndices b = new VertexIndices((int)indices[f + 1], (int)indices[f + 1], (int)indices[f + 1]);
                    VertexIndices c = new VertexIndices((int)indices[f + 2], (int)indices[f + 2], (int)indices[f + 2]);
                    faces.Add(new Face(a, b, c));
                }
                catch { }
            }

            MeshVbo mterrain = ApplicationBase.Instance.MeshLoader.FromMesh(positions, normals, uvs, faces, 
                                                                            "Terrain v1.0",
                                                                            false, false, false, false);

            this.meshes = new MeshVbo[1]
            {
                mterrain
            };

            this.CreateVAO();
        }

        [Obsolete("Old fixed-pipeline function. REMOVE ")]
        private int loadTexture(string textureFile)
        {
            string texture = textureFile.Replace(Configuration.GameSettings.TextureFolder, string.Empty);

            if (System.IO.Path.GetExtension(texture).ToLower().Contains(".png"))
                ApplicationBase.Instance.TextureLoader.fromPng(texture);
            if (System.IO.Path.GetExtension(texture).ToLower().Contains(".dds"))
                ApplicationBase.Instance.TextureLoader.fromDds(texture);

            ApplicationBase.Instance.TextureLoader.LoadTextures(null, true);

            return ApplicationBase.Instance.TextureLoader.getTextureId(texture);
        }

        private Vertex[] createPlane(float width, float length, int subX, int subZ, HeightFunc func)
        {
            int numVerts = (subX + 1) * (subZ + 1);
            Vertex[] verts = new Vertex[numVerts];

            float xSubSize = width / subX;
            float zSubSize = length / subZ;
            float sInc = 1.0f / subX;
            float tInc = 1.0f / subZ;

            float curX = -width / 2.0f;
            float curZ = -length / 2.0f;                       
            float curU = 0.0f;
            float curV = 0.0f;
            int curVert = 0;

            Heights = new float[subX+1, subZ+1];


            for (int z = 0; z <= subZ; z++)
            {
                for (int x = 0; x <= subX; x++)
                {
                   

                    verts[curVert++] = new Vertex()
                    {
                        X = curX,
                        Y = func(curU, curV),                        
                        Z = curZ,
                        U = curU * meshUv,
                        V = curV * meshUv
                    };
                    curU += sInc;
                    curX += xSubSize;

                    Heights[x, z] = verts[curVert-1].Y/* * HeightScale*/;
                }
                curU = 0.0f;
                curV += tInc;
                curX = -width / 2.0f;
                curZ += zSubSize;
            }

            return verts;
        }

        private uint[] createIndices(int subX, int subZ)
        {
            int numIndices = (subX * 2 * (subZ - 1)) + (subZ - 2);
            uint[] ind = new uint[numIndices];

            int vertsWide = subX + 1;
            int curIndex = 0;

            for (int z = 0; z < subZ - 1; z++)
            {
                // even rows move ->
                // odd rows move <-
                if (z % 2 == 0)
                {
                    int x = 0;
                    for (; x < subX; x++)
                    {
                        ind[curIndex++] = (uint)(x + (z * vertsWide));
                        ind[curIndex++] = (uint)(x + (z * vertsWide) + vertsWide);
                    }

                    if (z != subZ - 2)
                    {
                        ind[curIndex++] = (uint)(--x + (z * vertsWide));
                    }
                }
                else
                {
                    int x = subX - 1;
                    for (; x >= 0; --x)
                    {
                        ind[curIndex++] = (uint)(x + (z * vertsWide));
                        ind[curIndex++] = (uint)(x + (z * vertsWide) + vertsWide);
                    }

                    if (z != subZ - 2)
                    {
                        ind[curIndex++] = (uint)(++x + (z * vertsWide));
                    }
                }
            }

            return ind;
        }

        private float getHeightmapValueFunction(byte[] heightMap, float x, float y, int width, int height)
        {
            int rowSize = width * 4;

            int centerPixelX = (int)x;
            int centerPixelY = (int)y;
            int centerOffset = centerPixelX * 4 + (centerPixelY * rowSize);

            float pixelSum = heightMap[centerOffset];
            int numPixels = 1;

            if (centerPixelX > 0)
            {
                pixelSum += heightMap[centerOffset - 4];
                numPixels++;
            }

            if (centerPixelX < width - 1)
            {
                pixelSum += heightMap[centerOffset + 4];
                numPixels++;
            }

            if (centerPixelY > 0)
            {
                pixelSum += heightMap[centerOffset - rowSize];
                numPixels++;
            }

            if (centerPixelY < height - 1)
            {
                pixelSum += heightMap[centerOffset + rowSize];
                numPixels++;
            }

            return (pixelSum / numPixels) / 255.0f;
        }

        private void generateNormals(Vertex[] verts, uint[] indices)
        {
            for (int i = 2; i < indices.Length; i++)
            {
                uint index1 = indices[i - 2],
                     index2 = indices[i - 1],
                     index3 = indices[i];

                Vector3 normal = Vector3.Zero; 
                normal = calcNormal(verts[index1], verts[index2], verts[index3]);

                if (normal.Y < 0)
                    normal *= -1.0f;                

                normal.NormalizeFast();

                verts[index1].Normal = normal; 
                verts[index2].Normal = normal; 
                verts[index3].Normal = normal; 

                normals.Add(normal);
            }

            #region for visualization, do not use
            //Vertex[] normVerts = new Vertex[verts.Length * 2];
            //Color4[] colors = new Color4[verts.Length * 2];

            //for (int i = 0; i < verts.Length; i++)
            //{
            //    verts[i].Normal = Vector3.NormalizeFast(verts[i].Normal);
            //    normVerts[i * 2] = verts[i];
            //    normVerts[i * 2 + 1] = new Vertex()
            //    {
            //        X = verts[i].X + (verts[i].Normal.X * 2),
            //        Y = verts[i].Y + (verts[i].Normal.Y * 2),
            //        Z = verts[i].Z + (verts[i].Normal.Z * 2)
            //    };
            //    colors[i * 2] = Color4.Yellow;
            //    colors[i * 2 + 1] = Color4.Yellow;
            //}

            //normals = new Geometry(BeginMode.Lines, new VertexBuffer(normVerts, colors));
            #endregion
        }

        private Vector3 calcNormal(Vertex v1, Vertex v2, Vertex v3)
        {
            Vector3 first = new Vector3(v2.X - v1.X, v2.Y - v1.Y, v2.Z - v1.Z);
            Vector3 second = new Vector3(v3.X - v1.X, v3.Y - v1.Y, v3.Z - v1.Z);
            return Vector3.Normalize(Vector3.Cross(first, second));
        }

        public float GetHeightAt(float x, float z)
        {
            float bx = x * wide;
            float by = z * tall;

            if (bx >= wide)
                bx = wide - 1;

            if (by >= tall)
                by = tall - 1;

            return HeightScale * getHeightmapValueFunction(imagePixels, bx, by, wide, tall);
        }


        protected override void setSpecialUniforms(ref Shader curShader, ref MeshVbo CurMesh)  // Added Mar-12-18
        {
            base.setSpecialUniforms(ref curShader, ref CurMesh);

            if (curShader.Loaded)
            {
                float heightMin = HeightMin;
                float heightMax = HeightMax;                
                Vector2 uvScale = UVScale;

                curShader.InsertUniform(Uniform.terrain_minHeight,  ref heightMin);
                curShader.InsertUniform(Uniform.terrain_maxHeight,  ref heightMax);
                curShader.InsertUniform(Uniform.terrain_uvScale,    ref uvScale);
                
                if (ApplicationBase.Instance.Scene.DirectionalLights.Count > 0)
                {   
                    // NOTE:: Assumes that sun in always directional light [0]
                    // NOTE:: Works only on DirectionalLight[0]
                    Vector3 sun = ApplicationBase.Instance.Scene.DirectionalLights[0].PointingDirection.Normalized();
                    curShader.InsertUniform(Uniform.terrain_lightDir, ref sun);
                }
                else
                {
                    Vector3 sun = new Vector3(0f, -1f, 0.05f);
                    curShader.InsertUniform(Uniform.terrain_lightDir, ref sun);
                }
            }
        }

        

    }
}
