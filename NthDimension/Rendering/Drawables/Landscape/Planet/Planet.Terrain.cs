/* LICENSE
 * Copyright (C) 2008 - 2018 SYSCON Technologies, Hellas - All Rights Reserved
 * Written by Stylianos N. Polychroniadis (info@polytronic.gr) http://www.polytronic.gr
 * 
 * This file is part of nthDimension Platform
 * 
 * WARNING! Commercial Software, All Use Must Be Licensed
 * This software is protected by Hellenic Copyright Law and International Treaties. 
 * Unauthorized use, duplication, reverse engineering, any form of redistribution, or 
 * use in part or in whole other than by prior, express, printed and signed license 
 * for use is subject to civil and criminal prosecution. 
*/

namespace NthDimension.Rendering.Drawables.Models
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Drawing;

    using NthDimension.Algebra;
    using NthDimension.Rasterizer;
    using NthDimension.Rendering.Utilities;
    using NthDimension.Rendering.Serialization;
    using NthDimension.Rendering.Geometry;

    public partial class PlanetModel
    {
        // TODO:: Refactor (eliminate System.Drawing interface and System.Drawing.Bitmap. Texture flow through static TextureLoader)
        public partial class Terrain
        {
            private HeightMesh heighMesh;

            private float                   heightMult                      = 7.8f;     ////9.2f; //0.012f;  // TDB

            //private int[]                   indexBuffer;                    // TBC Index Buffers for all TerrainPatches
            //private int[]                   indicesCount;                   // TBC Total index count of each TerrainPatch
            private int[,]                  bridgeIndexBuffer;              // TBD Index Buffer for Terrain Bridges
            private int[,]                  bridgeIndicesCount;             // TBD Total index counts for Terrain Bridges

            private int                     texID;                          // TBD
            private int                     detailtexID;

            //private List<Vector3>           vertices;                       // TBD ???All vertices of all Terrain Patches (as a whole)???
            // private List<Vector2>           uvCoor;                         // TBD ???All texture UVs of all Terrain Patches (as a whole)???

            //private int                     lodLevels;
            private int                     tSize;                          // terrain size
            private int                     pSize;                          // patches size
            private int                     patchesPerRow;                            // number of patches per row
            private float                   radius;                         // TBD ???terrain sphere radius???
            private Vector3                 position;                       // TBD ???world space position???

            #region Properties
            public int LODLEVELS
            { get { return heighMesh.LevelsOfDetail; } }
            public int Width
            { get { return tSize; } }
            public int Height
            { get { return tSize; } }
            public int PatchSize
            { get { return pSize; } }
            public int PatchesPerRow
            { get { return patchesPerRow; } }

            public Vector3 Position;
            public float Radius { get { return radius; } set { radius = value; } }
            public int[] IndexBuffer
            { get { return heighMesh.EboHandle;  /*indexBuffer;*/ } }
            public int[] IndicesCount
            { get { return heighMesh.IndexCount;  /*indicesCount;*/ } }
            public int[,] BridgeIndexBuffer
            { get { return bridgeIndexBuffer; } }
            public int[,] BridgeIndicesCount
            { get { return bridgeIndicesCount; } }

            protected List<Vector3> _verticesFast;

            public List<Vector3> Vertices
            { get {
                    if (null == _verticesFast)
                        _verticesFast = new List<Vector3>(heighMesh.Vertices);
                    return _verticesFast; } }
            protected List<Vector2> _uvsFast;
            public List<Vector2> uvCoordinates
            { get {
                    if (null == _uvsFast)
                        _uvsFast = new List<Vector2>(heighMesh.UV);
                    return _uvsFast; } }
            public int TextureID
            { get { return texID; } }
            #endregion

            public int DetailTextureID
            { get { return detailtexID; } }



            public Terrain(Vector3 centerPos, Vector3 axisX, Vector3 axisZ, int LODLevels, int terrainSize, int patchSize, float planetRadius, int textureID, string heightmapURL, string detailURL)
            {
                if ((!Math2.IsPowerOfTwo(terrainSize - 1)) || (!Math2.IsPowerOfTwo(patchSize - 1)))
                    ConsoleUtil.errorlog("Terrain.Ctor() ", "Patch size must be 2^x+1 and terrain size must be 2^x+1");

                if (!File.Exists(heightmapURL))
                    ConsoleUtil.errorlog("Terrain.Ctor() ", "Heightmap not found!");
                
                texID           = textureID;
                tSize           = terrainSize;
                pSize           = patchSize;
                Radius          = planetRadius;
                
                patchesPerRow     = (terrainSize - 1) / patchSize;                          // Number of patches per row

                detailtexID     = ApplicationBase.Instance.TextureLoader.getTextureId(detailURL.Replace(Configuration.GameSettings.TextureFolder, string.Empty));
               
                heighMesh       = new HeightMesh(terrainSize, LODLevels);

                #region create HeightMesh
                float[,] heights    = new float[terrainSize, terrainSize];

                using (Bitmap heightMap = new Bitmap(heightmapURL))
                {
                    lock (heightMap)
                    {
                        for (int z = 0; z < terrainSize; z++)
                            for (int x = 0; x < terrainSize; x++)
                                heights[z, x] = heightMap.GetPixel(x, z).R * heightMult; // TODO:: Convert to GetPixelFast() // Consider using single channel color image intead of 32R 32G 32B 32A
                    }
                    heightMap.Dispose();
                }

                int index = 0;

                for (int z = 0; z < terrainSize; z++)
                    for(int x = 0; x < terrainSize; x++)
                    {
                        index                       = (z * terrainSize) + x;
                        heighMesh.Vertices[index]   = new Vector3(centerPos + (axisX) * ((x)) + (axisZ) * ((z)));
                        heighMesh.Vertices[index]   = GenerateCubeToSphereCoordinates(heighMesh.Vertices[index], heights[z,x]);
                        heighMesh.UV[index]         = new Vector2((float)(1.0 / terrainSize * x), (float)(1.0 / terrainSize * z));
                    }

                

                #endregion create HeightMesh

                int lodSqr = (int)Math.Pow(2, heighMesh.LevelsOfDetail - 1) + 1;

                
                bridgeIndicesCount = new int[2,lodSqr];                                 // this array stores the indice count for the bridges - for the case Resolution-To-Resolution and the case Resolution-To-(Resolution-1)
                bridgeIndexBuffer = new int[lodSqr, 8];                                 // this array stores all bridges buffers. 4 bridges to the same resolution, 4 bridges to a lower resolution.

                for (int i = 1; i <= heighMesh.LevelsOfDetail; i++)
                {
                    int resolution = (int)Math.Pow(2, i - 1);
                    // calculate indices count for every lod level
                    int currLevel = 0, lastLevel = 0;
                    if (i == 1)
                    {
                        // there are 3 indices per triangle and 2 triangle to make a polygon
                        heighMesh.IndexCount[i] = ((patchSize) * (patchSize) * 3 * 2);
                    }
                    else
                    {
                        currLevel = resolution;
                        lastLevel = resolution - (int)Math.Pow(2, i - 2);
                        heighMesh.IndexCount[currLevel] = heighMesh.IndexCount[lastLevel] / 4;
                    }

                    // assign the index buffer ids and then calculate and create the index buffers
                    BuildIBO(resolution);

                    // calculate the 8 bridges
                    bridgeIndicesCount[0, resolution] = ((pSize - 1) / resolution) * 3 * 2;
                    // top bridge (to same level)
                    BuildDefaultHoriBridge(resolution, true);
                    // right bridge (to same level)
                    BuildDefaultVertBridge(resolution, false);
                    // bottom bridge (to same level)
                    BuildDefaultHoriBridge(resolution, false);
                    // left bridge (to same level)
                    BuildDefaultVertBridge(resolution, true);

                    bridgeIndicesCount[1, resolution] = ((((pSize - 1) / (resolution * 2)) * 3) - 2) * 3;
                    // top bridge (to lower level)
                    BuildLowerHoriBridge(resolution, true);
                    // right bridge (to lower level)
                    BuildLowerVertBridge(resolution, false);
                    // bottom bridge (to lower level)
                    BuildLowerHoriBridge(resolution, false);
                    // left bridge (to lower level)
                    BuildLowerVertBridge(resolution, true);


                }

                Console.WriteLine("(Class: Terrain) Terrain created");
            }

            MeshVboData terrainMesh;


            private void BuildIBO(int resolution)
            {
                int[] indices = new int[heighMesh.IndexCount[resolution]];                              // indices dem Crackfix anpassen?
                int index = 0;
                for (int z = resolution; z < pSize - resolution - 1; z++)                               // -1 oder -2 (Im alten Code habe ich pSize-2 stehen)
                {
                    for (int x = resolution; x < pSize - resolution - 1; x++)
                    {
                        indices[index++] = x + z * pSize;
                        indices[index++] = (x + resolution) + z * pSize;
                        indices[index++] = x + (z + resolution) * pSize;
                        indices[index++] = x + (z + resolution) * pSize;
                        indices[index++] = (x + resolution) + z * pSize;
                        indices[index++] = (x + resolution) + (z + resolution) * pSize;
                        x += resolution - 1;                                                            // -1? => da oben x++!
                    }
                    z += resolution - 1;
                }


                int size;
               
                //// Create Index buffer 
                ApplicationBase.Instance.Renderer.GenBuffers(1, out heighMesh.EboHandle[resolution]);
                ApplicationBase.Instance.Renderer.BindBuffer(BufferTarget.ElementArrayBuffer, heighMesh.EboHandle[resolution]);
                ApplicationBase.Instance.Renderer.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(heighMesh.IndexCount[resolution] * OpenTK.BlittableValueType.StrideOf(indices[resolution])), indices, BufferUsageHint.StaticDraw);
                ApplicationBase.Instance.Renderer.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);

                if (heighMesh.IndexCount[resolution] * OpenTK.BlittableValueType.StrideOf(indices[resolution]) != size) {
                    throw new ApplicationException("Error: (Class: Terrain) indices data not uploaded correctly");
                }
               
            }

            // Calculates a horizontal bridge to a patch of the same level
            private void BuildDefaultHoriBridge(int resolution, bool isUpperBridge)
            {
                int[] indices = new int[bridgeIndicesCount[0, resolution]];
                int z = isUpperBridge ? 0 : pSize - resolution - 1;
                int index = 0;

                for (int x = 0; x < pSize - resolution; x++)
                {
                    // first triangle
                    if (x == 0)
                    {
                        int modPoint = isUpperBridge ? z : (z + resolution);
                        indices[index++] = (x + modPoint * pSize);
                        indices[index++] = ((x + resolution) + z * pSize);
                        indices[index++] = ((x + resolution) + (z + resolution) * pSize);
                    }
                    // last triangle
                    else if (x == pSize - resolution - 1)
                    {
                        int modPoint = isUpperBridge ? z : (z + resolution);
                        indices[index++] = (x + z * pSize);
                        indices[index++] = (x + (z + resolution) * pSize);
                        indices[index++] = ((x + resolution) + modPoint * pSize);
                    }
                    else
                    {
                        // middle polygons
                        indices[index++] = (x + z * pSize);
                        indices[index++] = ((x + resolution) + z * pSize);
                        indices[index++] = (x + (z + resolution) * pSize);
                        indices[index++] = (x + (z + resolution) * pSize);
                        indices[index++] = ((x + resolution) + z * pSize);
                        indices[index++] = ((x + resolution) + (z + resolution) * pSize);
                    }

                    x += resolution - 1;
                }

                
                int size = 0; // For debug

                // Create Index buffer
                int bridgeIndex = isUpperBridge ? 0 : 2;
                //throw new NotImplementedException("Call to GL");
                //GL.GenBuffers(1, out bridgeIndexBuffer[resolution, bridgeIndex]);
                //GL.BindBuffer(BufferTarget.ElementArrayBuffer, bridgeIndexBuffer[resolution, bridgeIndex]);
                //GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * sizeof(int)), indices, BufferUsageHint.StaticDraw);
                //GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);

                ApplicationBase.Instance.Renderer.GenBuffers(1, out bridgeIndexBuffer[resolution, bridgeIndex]);
                ApplicationBase.Instance.Renderer.BindBuffer(BufferTarget.ElementArrayBuffer, bridgeIndexBuffer[resolution, bridgeIndex]);
                ApplicationBase.Instance.Renderer.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * sizeof(int)), indices, BufferUsageHint.StaticDraw);
                ApplicationBase.Instance.Renderer.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);


                if (indices.Length * sizeof(int) != size) { throw new ApplicationException("Error: (Class: Terrain) indices data not uploaded correctly"); }
            }

            // Calculates a vertical bridge to a patch of the same level
            private void BuildDefaultVertBridge(int resolution, bool isLeftBridge)
            {
                int[] indices = new int[bridgeIndicesCount[0, resolution]];
                int x = isLeftBridge ? 0 : pSize - resolution - 1;
                int index = 0;

                for (int z = 0; z < pSize - resolution; z++)
                {
                    // first triangle
                    if (z == 0)
                    {
                        int modPoint = isLeftBridge ? x : (x + resolution);
                        indices[index++] = (modPoint + z * pSize);
                        indices[index++] = (x + (z + resolution) * pSize);
                        indices[index++] = ((x + resolution) + (z + resolution) * pSize);
                    }
                    // last triangle
                    else if (z == pSize - resolution - 1)
                    {
                        int modPoint = isLeftBridge ? x : (x + resolution);
                        indices[index++] = (x + z * pSize);
                        indices[index++] = ((x + resolution) + z * pSize);
                        indices[index++] = (modPoint + (z + resolution) * pSize);
                    }
                    else
                    {
                        // middle polygons
                        indices[index++] = (x + z * pSize);
                        indices[index++] = ((x + resolution) + z * pSize);
                        indices[index++] = (x + (z + resolution) * pSize);
                        indices[index++] = (x + (z + resolution) * pSize);
                        indices[index++] = ((x + resolution) + z * pSize);
                        indices[index++] = ((x + resolution) + (z + resolution) * pSize);
                    }

                    z += resolution - 1;
                }

                // For debug
                int size;

                // Create Index buffer
                int bridgeIndex = isLeftBridge ? 3 : 1;
                //throw new NotImplementedException("Call to GL");
                //GL.GenBuffers(1, out bridgeIndexBuffer[resolution, bridgeIndex]);
                //GL.BindBuffer(BufferTarget.ElementArrayBuffer, bridgeIndexBuffer[resolution, bridgeIndex]);
                //GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * sizeof(int)), indices, BufferUsageHint.StaticDraw);
                //GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);

                ApplicationBase.Instance.Renderer.GenBuffers(1, out bridgeIndexBuffer[resolution, bridgeIndex]);
                ApplicationBase.Instance.Renderer.BindBuffer(BufferTarget.ElementArrayBuffer, bridgeIndexBuffer[resolution, bridgeIndex]);
                ApplicationBase.Instance.Renderer.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * sizeof(int)), indices, BufferUsageHint.StaticDraw);
                ApplicationBase.Instance.Renderer.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);


                if (indices.Length * sizeof(int) != size) { throw new ApplicationException("Error: (Class: Terrain) indices data not uploaded correctly"); }
            }

            // Calculates a horizontal bridge to a patch of a lower level
            private void BuildLowerHoriBridge(int resolution, bool isUpperBridge)
            {
                int[] indices = new int[bridgeIndicesCount[1, resolution]];
                int z = isUpperBridge ? 0 : pSize - resolution - 1;
                int doubleResolution = resolution * 2;
                int index = 0;

                for (int x = 0; x < pSize - doubleResolution; x++)
                {
                    // left triangle
                    if (x != 0)
                    {
                        int modPoint = isUpperBridge ? (z + resolution) : z;
                        indices[index++] = x + z * pSize;
                        indices[index++] = x + (z + resolution) * pSize;
                        indices[index++] = (x + resolution) + modPoint * pSize;
                    }
                    // right triangle
                    if (x != pSize - doubleResolution - 1)
                    {
                        int modPoint = isUpperBridge ? (z + resolution) : z;
                        indices[index++] = (x + doubleResolution) + z * pSize;
                        indices[index++] = (x + resolution) + modPoint * pSize;
                        indices[index++] = (x + doubleResolution) + (z + resolution) * pSize;
                    }
                    // middle triangle
                    if (isUpperBridge)
                    {
                        indices[index++] = x + z * pSize;
                        indices[index++] = (x + resolution) + (z + resolution) * pSize;
                        indices[index++] = (x + doubleResolution) + z * pSize;
                    }
                    else
                    {
                        indices[index++] = x + (z + resolution) * pSize;
                        indices[index++] = (x + resolution) + z * pSize;
                        indices[index++] = (x + doubleResolution) + (z + resolution) * pSize;
                    }

                    x += doubleResolution - 1;
                }

                // For debug
                int size;

                // Create Index buffer
                int bridgeIndex = isUpperBridge ? 4 : 6;
                //throw new NotImplementedException("Call to GL");
                //GL.GenBuffers(1, out bridgeIndexBuffer[resolution, bridgeIndex]);
                //GL.BindBuffer(BufferTarget.ElementArrayBuffer, bridgeIndexBuffer[resolution, bridgeIndex]);
                //GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * sizeof(int)), indices, BufferUsageHint.StaticDraw);
                //GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);

                ApplicationBase.Instance.Renderer.GenBuffers(1, out bridgeIndexBuffer[resolution, bridgeIndex]);
                ApplicationBase.Instance.Renderer.BindBuffer(BufferTarget.ElementArrayBuffer, bridgeIndexBuffer[resolution, bridgeIndex]);
                ApplicationBase.Instance.Renderer.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * sizeof(int)), indices, BufferUsageHint.StaticDraw);
                ApplicationBase.Instance.Renderer.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);


                if (indices.Length * sizeof(int) != size) { throw new ApplicationException("Error: (Class: Terrain) indices data not uploaded correctly"); }
            }

            // Calculates a vertical bridge to a patch of a lower level
            private void BuildLowerVertBridge(int resolution, bool isLeftBridge)
            {
                int[] indices = new int[bridgeIndicesCount[1, resolution]];
                int x = isLeftBridge ? 0 : pSize - resolution - 1;
                int doubleResolution = resolution * 2;
                int index = 0;

                for (int z = 0; z < pSize - doubleResolution; z++)
                {
                    // left triangle
                    if (z != 0)
                    {
                        int modPoint = isLeftBridge ? (x + resolution) : x;
                        indices[index++] = x + z * pSize;
                        indices[index++] = (x + resolution) + z * pSize;
                        indices[index++] = modPoint + (z + resolution) * pSize;
                    }
                    // right triangle
                    if (z != pSize - doubleResolution - 1)
                    {
                        int modPoint = isLeftBridge ? (x + resolution) : x;
                        indices[index++] = modPoint + (z + resolution) * pSize;
                        indices[index++] = x + (z + doubleResolution) * pSize;
                        indices[index++] = (x + resolution) + (z + doubleResolution) * pSize;
                    }
                    // middle triangle
                    if (isLeftBridge)
                    {
                        indices[index++] = x + z * pSize;
                        indices[index++] = (x + resolution) + (z + resolution) * pSize;
                        indices[index++] = x + (z + doubleResolution) * pSize;
                    }
                    else
                    {
                        indices[index++] = (x + resolution) + z * pSize;
                        indices[index++] = x + (z + resolution) * pSize;
                        indices[index++] = (x + resolution) + (z + doubleResolution) * pSize;
                    }

                    z += doubleResolution - 1;
                }

                // For debug
                int size;

                // Create Index buffer
                int bridgeIndex = isLeftBridge ? 7 : 5;
                //throw new NotImplementedException("Call to GL");
                //GL.GenBuffers(1, out bridgeIndexBuffer[resolution, bridgeIndex]);
                //GL.BindBuffer(BufferTarget.ElementArrayBuffer, bridgeIndexBuffer[resolution, bridgeIndex]);
                //GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * sizeof(int)), indices, BufferUsageHint.StaticDraw);
                //GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);

                ApplicationBase.Instance.Renderer.GenBuffers(1, out bridgeIndexBuffer[resolution, bridgeIndex]);
                ApplicationBase.Instance.Renderer.BindBuffer(BufferTarget.ElementArrayBuffer, bridgeIndexBuffer[resolution, bridgeIndex]);
                ApplicationBase.Instance.Renderer.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * sizeof(int)), indices, BufferUsageHint.StaticDraw);
                ApplicationBase.Instance.Renderer.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);

                if (indices.Length * sizeof(int) != size) { throw new ApplicationException("Error: (Class: Terrain) indices data not uploaded correctly"); }

            }

            private Vector3 GenerateCubeToSphereCoordinates(Vector3 cubeV, float height)
            {
                cubeV.Normalize();
                Vector3 result = new Vector3(cubeV);
                // + Parameter überschrieben?
                result.X *= Radius + height;
                result.Y *= Radius + height;
                result.Z *= Radius + height;
                return result;
            }

            public void Destroy()
            {
                /*for (int i = 0; i < indexBuffer.Length; i++)
                {
                    if (indexBuffer[i] != 0) { GL.DeleteBuffers(1, ref indexBuffer[i]); }
                }*/
            }

            //public struct Vertex3
            //{
            //    public float X;
            //    public float Y;
            //    public float Z;
            //}

            // STELIOS INTERFACE FUNCTIONS
            private MeshVbo meshTerrain;
            private MeshVbo meshHorizontalRightBridge;
            private MeshVbo meshHorizontalLeftBridge;
            private MeshVbo meshVerticalTopBridge;
            private MeshVbo meshVerticalBottomBridge;

            private MeshVbo meshLowerHorizontalRightBridge;
            private MeshVbo meshLowerHorizontalLeftBridge;
            private MeshVbo meshLowerVerticalTopBridge;
            private MeshVbo meshLowerVerticalBottomBridge;

            
          
            sealed class HeightMesh
            {
                protected Vector3[]     vertArray;
                protected Vector2[]     uvArray;
                protected Int32[]       indexBuffer;
                protected Int32[]       indexCount;
                protected Int32[]       indices;

                private int             m_terrainSize;
                private int             m_lodDepth;

                public HeightMesh(int terrainSize, int lodLevels)
                {
                    m_terrainSize   = terrainSize;
                    m_lodDepth      = lodLevels;

                    int iSize = (int)Math.Pow(2, lodLevels - 1) + 1;

                    indexCount      = new int[iSize];
                    indexBuffer     = new int[iSize];

                    int tSize       = terrainSize * terrainSize;

                    vertArray       = new Vector3[tSize];
                    uvArray         = new Vector2[tSize];
                }

                public Vector3[] Vertices
                {
                    get { return vertArray; }
                }
                public Vector2[] UV
                {
                    get { return uvArray; }
                }
                public Int32[] EboHandle
                {
                    get { return indexBuffer; }
                }
                public Int32[] IndexCount
                {
                    get { return indexCount; }
                }
                public int Size
                {
                    get { return m_terrainSize; }
                }
                public int LevelsOfDetail
                {
                    get { return m_lodDepth; }
                }
                public Int32[] Indices
                {
                    get { return indices; }
                }
            }

        }
    }
}
