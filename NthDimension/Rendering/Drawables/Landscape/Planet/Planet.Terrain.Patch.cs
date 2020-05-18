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
    using System.Collections.Generic;
    using NthDimension.Algebra;

    public partial class PlanetModel
    {
        public partial class Terrain
        {
            public enum rasterizer
            {
                VBO,
                DisplayList
            }

            public class TerrainPatch
            {
                #region Fields

                //private DisplayList m_displayList;
                private Terrain.rasterizer mraster = Terrain.rasterizer.VBO;


                protected float distanceFactor = 150.0f; // TBD was 50

                private int id; // TerrainPatch Id used for indexing/referencing
                private int patchSize; // TBD
                private int vertexBuffer; // The Vertex Buffer Id Probably not to be used here after refactoring to <Model>

                private int textureBuffer
                    ; // The Texture Buffer Id, Probably not to be used here after refactoring to <Model>

                private Vector3 highestV; // TBD
                private Vector3 lowestV; // TBD
                private Vector3 centerV; // TBD

                public int verticesCount; // Total Vertex Count
                private List<Vector3> vertices; // The Vertices array
                private List<Vector2> uvCoor; // The Texture UV Coordinates array (for UV-Mapping)

                private Vector3[] bbV; // Vertices describing the bounding box (if there is any)



                #endregion

                #region Properties
                public int ID
                {
                    get { return id; }
                }
                public int PatchSize
                {
                    get { return patchSize; }
                }
                public int VertexBuffer
                {
                    get { return vertexBuffer; }
                }
                public int TextureBuffer
                {
                    get { return textureBuffer; }
                }
                public List<Vector3> Vertices
                {
                    get { return vertices; }
                    set { vertices = value; }
                }
                public Vector3 CenterVertex
                {
                    get { return centerV; }
                }
                public Vector3[] BoundingBoxV
                {
                    get { return bbV; }
                }

                public int UpdateToResolution = -1;   // This is used to ensure the max. difference of 1 lod between two patches

                #endregion


                // just for testing purpose
                public TerrainPatch(int startX, int startZ, int patchSize, float distFactor)
                {
                    //Debug.Print("Obsolete");

                    this.distanceFactor = distFactor;

                    Vertices = new List<Vector3>();
                    uvCoor = new List<Vector2>();

                    this.patchSize = patchSize;

                    for (int x = 0; x < patchSize; x++)
                    {
                        for (int z = 0; z < patchSize; z++)
                        {
                            Vertices.Add(new Vector3(x, 0, z));
                            uvCoor.Add(new Vector2(1.0f, 1.0f));
                        }
                    }

                    int size;

                    int uvCount = uvCoor.ToArray().Length;

                    float[] uvCoorf = new float[uvCoor.Count * 2];
                    int index = 0;
                    foreach (Vector2 uv in uvCoor)
                    {
                        uvCoorf[index++] = uv.X;
                        uvCoorf[index++] = uv.Y;
                    }

                    // Create array with vertices coordinates for buffer data
                    float[] verticesf = new float[Vertices.Count * 3];
                    index = 0;
                    foreach (Vector3 vec in Vertices)
                    {
                        verticesf[index++] = vec.X;
                        verticesf[index++] = vec.Y;
                        verticesf[index++] = vec.Z;
                    }

                    verticesCount = verticesf.Length;

                    //if (mraster == rasterizer.VBO)
                    //{
                    //    // Create Texture buffer
                    //    GL.GenBuffers(1, out textureBuffer);
                    //    GL.BindBuffer(BufferTarget.ArrayBuffer, textureBuffer);
                    //    GL.BufferData(BufferTarget.ArrayBuffer,
                    //        (IntPtr) (uvCoorf.Length * BlittableValueType.StrideOf(uvCoorf)),
                    //        uvCoorf, BufferUsageHint.StaticDraw);
                    //    GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);
                    //    if (uvCoorf.Length * BlittableValueType.StrideOf(uvCoorf) != size)
                    //    {
                    //        throw new ApplicationException("Error: (Class: Terrain) texture data not uploaded correctly");
                    //    }

                    //    // Create Vertex buffer
                    //    GL.GenBuffers(1, out vertexBuffer);
                    //    GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
                    //    GL.BufferData(BufferTarget.ArrayBuffer,
                    //        (IntPtr) (verticesCount * BlittableValueType.StrideOf(verticesf)),
                    //        verticesf, BufferUsageHint.StaticDraw);
                    //    GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);
                    //    if (verticesCount * BlittableValueType.StrideOf(verticesf) != size)
                    //    {
                    //        throw new ApplicationException("Error: (Class: Terrain) vertex data not uploaded correctly");
                    //    }
                    //}
                }
                // ************************

                public TerrainPatch(int id, Vector3[] boundingBox, List<Vector3> vertices, List<Vector2> uvCoordinates,
                    int patchSize, float distFactor)
                {
                    this.distanceFactor = distFactor;
                    this.id = id;
                    Vertices = vertices;
                    uvCoor = uvCoordinates;

                    this.patchSize = patchSize;

                    centerV = vertices[vertices.Count / 2];

                    // Calculate the highest point of the terrain patch
                    // First use a dummy so we make sure that we get the highest point.
                    highestV = new Vector3(vertices[0].X - 999, vertices[0].Y - 999, vertices[0].Z - 999);
                    for (int i = 0; i < vertices.Count; i++)
                    {
                        if (vertices[i].X > highestV.X)
                        {
                            highestV.X = vertices[i].X;
                        }
                        if (vertices[i].Y > highestV.Y)
                        {
                            highestV.Y = vertices[i].Y;
                        }
                        if (vertices[i].Z > highestV.Z)
                        {
                            highestV.Z = vertices[i].Z;
                        }
                    }
                    // This prevents a bounding box with a height, width and length of 0
                    if (highestV.X == 0.0f)
                    {
                        highestV.X = 1.0f;
                    }
                    if (highestV.Y == 0.0f)
                    {
                        highestV.Y = 1.0f;
                    }
                    if (highestV.Z == 0.0f)
                    {
                        highestV.Z = 1.0f;
                    }

                    lowestV = new Vector3(highestV);
                    // Calculate the lowest point. Used to calculate the boundingbox later on
                    for (int i = 0; i < vertices.Count; i++)
                    {
                        if (vertices[i].X < lowestV.X)
                        {
                            lowestV.X = vertices[i].X;
                        }
                        if (vertices[i].Y < lowestV.Y)
                        {
                            lowestV.Y = vertices[i].Y;
                        }
                        if (vertices[i].Z < lowestV.Z)
                        {
                            lowestV.Z = vertices[i].Z;
                        }
                    }

                    // X und Z müssen durch die spherische Verschiebungen AUCH neuberechnet werden!!!
                    // !!!!!!!!!!!!!!!!!
                    // Nicht vergessen, dies dann auch in der BA zu ändern.


                    //highestV.Y = lowestV.Y = 0;

                    // Calculate the 3d bounding box
                    bbV = new Vector3[8];
                    // corner: bottom half(Y), back(X), left(Z)
                    bbV[0] = new Vector3(lowestV.X, lowestV.Y, lowestV.Z);
                    // corner: bottom half, back, right
                    bbV[1] = new Vector3(lowestV.X, lowestV.Y, highestV.Z);
                    // corner: bottom half, front, left
                    bbV[2] = new Vector3(highestV.X, lowestV.Y, lowestV.Z);
                    // corner: bottom half, front, right
                    bbV[3] = new Vector3(highestV.X, lowestV.Y, highestV.Z);
                    // corner: top half, back, left
                    bbV[4] = new Vector3(lowestV.X, highestV.Y, lowestV.Z);
                    // corner: top half, back, right
                    bbV[5] = new Vector3(lowestV.X, highestV.Y, highestV.Z);
                    // corner: top half, front, left
                    bbV[6] = new Vector3(highestV.X, highestV.Y, lowestV.Z);
                    // corner: top half, front, right
                    bbV[7] = new Vector3(highestV.X, highestV.Y, highestV.Z);

                    // For debug
                    int size;

                    int uvCount = uvCoor.ToArray().Length;

                    // Create array with uv coordinates for buffer data
                    float[] uvCoorf = new float[uvCoor.Count * 2];
                    int index = 0;
                    foreach (Vector2 uv in uvCoor)
                    {
                        uvCoorf[index++] = uv.X;
                        uvCoorf[index++] = uv.Y;
                    }

                    // Create array with vertices coordinates for buffer data
                    float[] verticesf = new float[vertices.Count * 3];
                    index = 0;
                    foreach (Vector3 vec in vertices)
                    {
                        verticesf[index++] = vec.X;
                        verticesf[index++] = vec.Y;
                        verticesf[index++] = vec.Z;
                    }

                    verticesCount = verticesf.Length;

                    if (mraster == Terrain.rasterizer.VBO)
                    {
                        //// Create Texture buffer
                        //GL.GenBuffers(1, out textureBuffer);
                        //GL.BindBuffer(BufferTarget.ArrayBuffer, textureBuffer);
                        //GL.BufferData(BufferTarget.ArrayBuffer,
                        //    (IntPtr) (uvCoorf.Length * BlittableValueType.StrideOf(uvCoorf)),
                        //    uvCoorf, BufferUsageHint.StaticDraw);
                        //GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);
                        //if (uvCoorf.Length * BlittableValueType.StrideOf(uvCoorf) != size)
                        //{
                        //    throw new ApplicationException("Error: (Class: Terrain) texture data not uploaded correctly");
                        //}

                        //// Create Vertex buffer
                        //GL.GenBuffers(1, out vertexBuffer);
                        //GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
                        //GL.BufferData(BufferTarget.ArrayBuffer,
                        //    (IntPtr) (verticesCount * BlittableValueType.StrideOf(verticesf)),
                        //    verticesf, BufferUsageHint.StaticDraw);
                        //GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);
                        //if (verticesCount * BlittableValueType.StrideOf(verticesf) != size)
                        //{
                        //    throw new ApplicationException("Error: (Class: Terrain) vertex data not uploaded correctly");
                        //}
                    }
                }

                public int GetResolution(int maxLodLevel)
                {
                    int patchResolution = (int)Math.Pow(2, maxLodLevel - 1);
                    ;
                    float distance =
                        (ApplicationBase.Instance.Player.Position - centerV).LengthFast; //.camera.calcDistanceToVertexPoint(centerV);

                    //Console.WriteLine(distance);
                    for (int i = maxLodLevel; i > 0; i--)
                    {
                        if (distance < distanceFactor * i)
                        {
                            patchResolution = (int)Math.Pow(2, i - 1);
                        }
                    }
                    // for debug only!
                    //Console.WriteLine(patchResolution);
                    //patchResolution = 1;
                    return patchResolution;
                }

                public void Render(int mainPatchIndexBuffer, int[] defaultBridgeIndexBuffer, int[] lowerBridgeIndexBuffer,
                    int[] indicesCount, int textureID, int detailtextureID)
                {
                    //// for debug
                    ////Debug.BoundingBox.Render(bbV);

                    //this.ApplyFixedFunctionMaterial(true, true);

                    //if (mraster == rasterizer.VBO)
                    //{
                    //    #region VBO
                    //    GL.ClientActiveTexture(TextureUnit.Texture0);
                    //    GL.Enable(EnableCap.Texture2D);
                    //    Texture.Bind(textureID);

                    //    GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)TextureEnvMode.Replace);
                    //    GL.BindBuffer(BufferTarget.ArrayBuffer, this.textureBuffer);
                    //    GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, new IntPtr(0));

                    //    GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexBuffer);
                    //    GL.VertexPointer(3, VertexPointerType.Float, 0, new IntPtr(0));

                    //    // render the main block
                    //    GL.BindBuffer(BufferTarget.ElementArrayBuffer, mainPatchIndexBuffer);


                    //    GL.DrawElements(BeginMode.Triangles, indicesCount[0], DrawElementsType.UnsignedInt, IntPtr.Zero);
                    //    // Only working if the main class = Game class
                    //    Game.polyCount += indicesCount[0];

                    //    GL.Disable(EnableCap.CullFace);

                    //    // if there is any default bridges, render them
                    //    if (defaultBridgeIndexBuffer.Length > 0)
                    //    {
                    //        for (int i = 0; i < defaultBridgeIndexBuffer.Length; i++)
                    //        {
                    //            GL.BindBuffer(BufferTarget.ElementArrayBuffer, defaultBridgeIndexBuffer[i]);
                    //            GL.DrawElements(BeginMode.Triangles, indicesCount[1], DrawElementsType.UnsignedInt, IntPtr.Zero);
                    //            Game.polyCount += indicesCount[1];
                    //        }
                    //    }

                    //    // if there is any lower bridges, render them
                    //    if (lowerBridgeIndexBuffer.Length > 0)
                    //    {
                    //        for (int i = 0; i < lowerBridgeIndexBuffer.Length; i++)
                    //        {
                    //            GL.BindBuffer(BufferTarget.ElementArrayBuffer, lowerBridgeIndexBuffer[i]);
                    //            GL.DrawElements(BeginMode.Triangles, indicesCount[2], DrawElementsType.UnsignedInt, IntPtr.Zero);
                    //            Game.polyCount += indicesCount[2];
                    //        }
                    //    }

                    //    GL.Enable(EnableCap.CullFace);

                    //    GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float)TextureEnvMode.Modulate); // Added and fixed explosion and bars
                    //    #endregion
                    //}
                    //else if (mraster == rasterizer.DisplayList)
                    //{
                    //    #region DisplayList
                    //    if (m_displayList == null || !m_displayList.Valid())
                    //    {
                    //        m_displayList = new DisplayListGL();
                    //        m_displayList.Invalidate();
                    //        m_displayList.Generate();
                    //        m_displayList.Start(false);
                    //        // ======================== Start Draw =================================================
                    //        GL.ClientActiveTexture(TextureUnit.Texture0);
                    //        GL.Enable(EnableCap.Texture2D);
                    //        Texture.Bind(textureID);

                    //        GL.DrawElements(BeginMode.Triangles, indicesCount[0], DrawElementsType.UnsignedInt, IntPtr.Zero);
                    //        // Only working if the main class = Game class
                    //        Game.polyCount += indicesCount[0];

                    //        GL.Disable(EnableCap.CullFace);

                    //        // if there is any default bridges, render them
                    //        if (defaultBridgeIndexBuffer.Length > 0)
                    //        {
                    //            for (int i = 0; i < defaultBridgeIndexBuffer.Length; i++)
                    //            {
                    //                GL.DrawElements(BeginMode.Triangles, indicesCount[1], DrawElementsType.UnsignedInt, IntPtr.Zero);
                    //                Game.polyCount += indicesCount[1];
                    //            }
                    //        }

                    //        // if there is any lower bridges, render them
                    //        if (lowerBridgeIndexBuffer.Length > 0)
                    //        {
                    //            for (int i = 0; i < lowerBridgeIndexBuffer.Length; i++)
                    //            {
                    //                GL.DrawElements(BeginMode.Triangles, indicesCount[2], DrawElementsType.UnsignedInt, IntPtr.Zero);
                    //                Game.polyCount += indicesCount[2];
                    //            }
                    //        }

                    //        GL.Enable(EnableCap.CullFace);

                    //        // =========================== End Draw ========================================================
                    //        m_displayList.End();
                    //    }
                    //    else
                    //        if (m_displayList.Valid() && !m_displayList.Recording)
                    //        m_displayList.Call();
                    //    #endregion
                    //}
                }

                public void Destroy()
                {
                    //GL.DeleteBuffers(1, ref vertexBuffer);
                    //GL.DeleteBuffers(1, ref textureBuffer);
                }

                private void ApplyFixedFunctionMaterial( /*Mesh mesh, Material mat,*/ bool textured, bool shaded)
                {

                    //    GL.Enable(EnableCap.Lighting);
                    //    GL.Disable(EnableCap.ColorMaterial);
                    //    GL.Enable(EnableCap.Normalize);

                    //    var alpha = 1.0f;

                    //    var color = new Color4(.8f, .8f, .8f, 1.0f);

                    //    color.A *= alpha;

                    //    GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, color);

                    //    color = new Color4(0, 0, 0, 1.0f);

                    //    GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, color);

                    //    color = new Color4(.2f, .2f, .2f, 1.0f);

                    //    GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, color);

                    //    color = new Color4(.4f, .4f, .4f, 1.0f);

                    //    GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, color);

                    //    float shininess = 1;
                    //    float strength = 1;

                    //    var exp = shininess * strength;
                    //    if (exp >= 128.0f) // 128 is the maximum exponent as per the Gl spec
                    //    {
                    //        exp = 128.0f;
                    //    }

                    //    GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, exp);


                    //    GL.Color3(color.R, color.G, color.B);

                    //    {
                    //        GL.Disable(EnableCap.Blend);
                    //        GL.DepthMask(true);
                    //    }
                    //}
                }
            }
        }
    }

}
