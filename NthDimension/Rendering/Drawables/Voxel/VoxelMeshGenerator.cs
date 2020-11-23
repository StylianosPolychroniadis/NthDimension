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

//#define USE_QUADS // Missing triangles (after triangulation?)

using NthDimension.Rendering.Geometry;
using NthDimension.Rendering.Serialization;



namespace NthDimension.Rendering.Drawables.Voxel
{
    using System;
    using System.Collections.Generic;

    using NthDimension.Algebra;

    using Rendering.Geometry;

    class VoxelMeshGenerator : VoxelManager
    {
        public ListVector3 positions;
        public ListVector2 textureCoords;
        public ListVector3 normals;
        public ListFace Faces;

        public Vector3 resolution;
        private MeshVbo surfaceMesh;

        public VoxelMeshGenerator(VoxelManager parent, VoxelDataGenerator vdata) : base(parent)
        {
            resolution = vdata.resolution + Vector3.One; //add 3 (10 Faces mean 11 Vertecies)

            surfaceMesh = new MeshVbo();
        }

        public MeshVbo generateMesh(VoxelDataGenerator vdata)
        {
            ApplicationBase.Instance.VAR_StopWatch.Start();

            vdata.generateVoxelData();
            prepareData();

            Vector3 up          = new Vector3(0, 1, 0);
            Vector3 down        = new Vector3(0, -1, 0);
            Vector3 right       = new Vector3(1, 0, 0);
            Vector3 left        = new Vector3(-1, 0, 0);
            Vector3 front       = new Vector3(0, 0, 1);
            Vector3 back        = new Vector3(0, 0, -1);                       

            for (int i = 0; i < vdata.voxelAmnt; i++)
            {
                Vector3 postion = vdata.idToNumber(i);

                if (vdata.getData(i) == 1)
                {
                    bool isBorder = vdata.isBorder(i);

                    if (vdata.getRelativeData(i, up) == 0)
                    {
                        generateTopFace(i, isBorder, postion);
                    }

                    if (vdata.getRelativeData(i, right) == 0)
                    {
                        generateRightFace(i, isBorder, postion);
                    }

                    if (vdata.getRelativeData(i, front) == 0)
                    {
                        generateFrontFace(i, isBorder, postion);
                    }

                    if (vdata.getRelativeData(i, back) == 0)
                    {
                        generateBackFace(i, isBorder, postion);
                    }

                    if (vdata.getRelativeData(i, left) == 0)
                    {
                        generateLeftFace(i, isBorder, postion);
                    }

                    if (vdata.getRelativeData(i, down) == 0)
                    {
                        generateDownFace(i, isBorder, postion);
                    }
                }
            }

            vdata.deleteCage();

            performSmoothing();
            performCubicMapping();

            surfaceMesh.MeshData.PositionCache  = positions;
            surfaceMesh.MeshData.TextureCache   = textureCoords;
            surfaceMesh.MeshData.NormalCache    = normals;

            surfaceMesh.MeshData.Positions      = positions.ToArray();
            surfaceMesh.MeshData.Textures       = textureCoords.ToArray();
            surfaceMesh.MeshData.Normals        = normals.ToArray();

            surfaceMesh.MeshData.Faces          = Faces;

#if USE_QUADS
            ApplicationBase.Instance.MeshLoader.ParseFaceList(ref surfaceMesh, false, MeshVbo.MeshLod.Level0, true, false, false);
#else
            ApplicationBase.Instance.MeshLoader.ParseFaceList(ref surfaceMesh, false, MeshVbo.MeshLod.Level0, false, false, false);
#endif
            ApplicationBase.Instance.MeshLoader.GenerateVBO(ref surfaceMesh, MeshVbo.MeshLod.Level0);

            surfaceMesh.CurrentLod = MeshVbo.MeshLod.Level0;
            surfaceMesh.MeshData.LodEnabled = false;

            ApplicationBase.Instance.VAR_StopWatch.Stop();
            
            if(NthDimension.Settings.Instance.game.diagnostics)
                Utilities.ConsoleUtil.log("Generated Voxel Chunk:" + ApplicationBase.Instance.VAR_StopWatch.Elapsed);

            return surfaceMesh;
        }

        private void performCubicMapping()
        {
            //textureCoords = new List<Vector2> { };

            for (int i = 0; i < Faces.Count; i++)
            {
                Face curFace = Faces[i];

                Vector3 faceNormal = (normals[curFace.Vertex[0].Vi] +
                normals[curFace.Vertex[1].Vi] +
                normals[curFace.Vertex[2].Vi] +
                normals[curFace.Vertex[3].Vi]) / 4f;

                Vector3 tmpN = faceNormal;

                tmpN.X *= Math.Sign(tmpN.X);
                tmpN.Y *= Math.Sign(tmpN.Y);
                tmpN.Z *= Math.Sign(tmpN.Z);

                int mappingMethod = -1;

                if (tmpN.X >= tmpN.Y && tmpN.X >= tmpN.Z)
                {
                    if (faceNormal.X < 0)
                        mappingMethod = 0;
                    else
                        mappingMethod = 1;
                }

                if (tmpN.Y >= tmpN.Z && tmpN.Y >= tmpN.X)
                {
                    if (faceNormal.Y < 0)
                        mappingMethod = 2;
                    else
                        mappingMethod = 3;
                }

                if (tmpN.Z >= tmpN.Y && tmpN.Z >= tmpN.X)
                {
                    if (faceNormal.Z < 0)
                        mappingMethod = 4;
                    else
                        mappingMethod = 5;
                }



                for (int j = 0; j < 4; j++)
                {
                    Vector3 pos = positions[curFace.Vertex[j].Vi];

                    curFace.Vertex[j].Ti = textureCoords.Count;

                    pos = pos * 0.5f + Vector3.One * 0.5f;

                    float mod = pos.LengthFast * 0.001f;

                    pos += Vector3.One * mod;

                    switch (mappingMethod)
                    {
                        case 0:
                            textureCoords.Add(new Vector2(-pos.Z, pos.Y));
                            break;
                        case 1:
                            textureCoords.Add(new Vector2(pos.Z, pos.Y));
                            break;

                        case 2:
                            textureCoords.Add(new Vector2(-pos.X, pos.Z));
                            break;
                        case 3:
                            textureCoords.Add(new Vector2(pos.X, pos.Z));
                            break;

                        case 4:
                            textureCoords.Add(new Vector2(pos.X, pos.Y));
                            break;
                        case 5:
                            textureCoords.Add(new Vector2(-pos.X, pos.Y));
                            break;

                        default:
                            textureCoords.Add(new Vector2(pos.X, pos.Y));
                            break;
                    }
                }


                Faces[i] = curFace;
            }
        }

        private void performSmoothing()
        {
            List<Vector3> positionsRaw = positions;
            List<Vector3> normalsRaw = normals;

            int noPositions = positions.Count;

            //regenerating position and normal list
            positions = new ListVector3 { };
            normals = new ListVector3 { };
            for (int i = 0; i < noPositions; i++)
            {
                positions.Add(Vector3.Zero);
                normals.Add(Vector3.Zero);
            }

            int[] noUserFaces = new int[noPositions];

            for (int i = 0; i < Faces.Count; i++)
            {
                Face curFace = Faces[i];

                Vector3 faceCenter =
                    (positionsRaw[curFace.Vertex[0].Vi] +
                    positionsRaw[curFace.Vertex[1].Vi] +
                    positionsRaw[curFace.Vertex[2].Vi] +
                    positionsRaw[curFace.Vertex[3].Vi]) / 4.0f;

                Vector3 faceNormal =
                    (normalsRaw[curFace.Vertex[0].Vi] +
                    normalsRaw[curFace.Vertex[1].Vi] +
                    normalsRaw[curFace.Vertex[2].Vi] +
                    normalsRaw[curFace.Vertex[3].Vi]) / 4.0f;

                for (int j = 0; j < 4; j++)
                {
                    int indice = curFace.Vertex[j].Vi;

                    positions[indice] += faceCenter;
                    normals[indice] += faceNormal;
                    noUserFaces[indice]++;
                }
            }

            for (int i = 0; i < noPositions; i++)
            {
                int curNoUserFaces = noUserFaces[i];
                if (curNoUserFaces > 1)
                {
                    positions[i] /= curNoUserFaces;
                    normals[i] = Vector3.Normalize(normals[i]);
                }
            }
        }

        private void generateRightFace(int i, bool isBorder, Vector3 postion)
        {
            Vector3 position1 = postion + new Vector3(1, 0, 0);
            Vector3 position2 = postion + new Vector3(1, 1, 0);
            Vector3 position3 = postion + new Vector3(1, 1, 1);
            Vector3 position4 = postion + new Vector3(1, 0, 1);

            Vector3 normal = new Vector3(1, 0, 0);

            generateFace(position1, position2, position3, position4, 1, normal, isBorder);
        }

        private void generateLeftFace(int i, bool isBorder, Vector3 postion)
        {
            Vector3 position1 = postion + new Vector3(0, 0, 0);
            Vector3 position2 = postion + new Vector3(0, 0, 1);
            Vector3 position3 = postion + new Vector3(0, 1, 1);
            Vector3 position4 = postion + new Vector3(0, 1, 0);

            Vector3 normal = new Vector3(-1, 0, 0);

            generateFace(position1, position2, position3, position4, 1, normal, isBorder);
        }

        private void generateFrontFace(int i, bool isBorder, Vector3 postion)
        {
            Vector3 position1 = postion + new Vector3(0, 0, 1);
            Vector3 position2 = postion + new Vector3(1, 0, 1);
            Vector3 position3 = postion + new Vector3(1, 1, 1);
            Vector3 position4 = postion + new Vector3(0, 1, 1);

            Vector3 normal = new Vector3(0, 0, 1);

            generateFace(position1, position2, position3, position4, 2, normal, isBorder);
        }

        private void generateBackFace(int i, bool isBorder, Vector3 postion)
        {
            Vector3 position1 = postion + new Vector3(0, 0, 0);
            Vector3 position2 = postion + new Vector3(0, 1, 0);
            Vector3 position3 = postion + new Vector3(1, 1, 0);
            Vector3 position4 = postion + new Vector3(1, 0, 0);

            Vector3 normal = new Vector3(0, 0, -1);

            generateFace(position1, position2, position3, position4, 2, normal, isBorder);
        }

        private void generateTopFace(int i, bool isBorder, Vector3 postion)
        {
            Vector3 position1 = postion + new Vector3(0, 1, 0);
            Vector3 position2 = postion + new Vector3(0, 1, 1);
            Vector3 position3 = postion + new Vector3(1, 1, 1);
            Vector3 position4 = postion + new Vector3(1, 1, 0);

            Vector3 normal = new Vector3(0, 1, 0);

            generateFace(position1, position2, position3, position4, 0, normal, isBorder);
        }

        private void generateDownFace(int i, bool isBorder, Vector3 postion)
        {
            Vector3 position1 = postion + new Vector3(0, 0, 0);
            Vector3 position2 = postion + new Vector3(1, 0, 0);
            Vector3 position3 = postion + new Vector3(1, 0, 1);
            Vector3 position4 = postion + new Vector3(0, 0, 1);

            Vector3 normal = new Vector3(0, -1, 0);

            generateFace(position1, position2, position3, position4, 0, normal, isBorder);
        }

        private void generateFace(Vector3 position1, Vector3 position2, Vector3 position3, Vector3 position4, int mapping, Vector3 normal, bool isBorder)
        {
            int Index1 = numberToID(position1);
            int Index2 = numberToID(position2);
            int Index3 = numberToID(position3);
            int Index4 = numberToID(position4);

            int textureIndex1 = 0;
            int textureIndex2 = 0;
            int textureIndex3 = 0;
            int textureIndex4 = 0;

            if (mapping == 0)
            {
                textureIndex1 = numberToID(new Vector3(position1.X, position1.Z, 0));
                textureIndex2 = numberToID(new Vector3(position2.X, position2.Z, 0));
                textureIndex3 = numberToID(new Vector3(position3.X, position3.Z, 0));
                textureIndex4 = numberToID(new Vector3(position4.X, position4.Z, 0));
            }
            else if (mapping == 1)
            {
                textureIndex1 = numberToID(new Vector3(position1.Z, position1.Y, 0));
                textureIndex2 = numberToID(new Vector3(position2.Z, position2.Y, 0));
                textureIndex3 = numberToID(new Vector3(position3.Z, position3.Y, 0));
                textureIndex4 = numberToID(new Vector3(position4.Z, position4.Y, 0));
            }
            else if (mapping == 2)
            {
                textureIndex1 = numberToID(new Vector3(position1.X, position1.Y, 0));
                textureIndex2 = numberToID(new Vector3(position2.X, position2.Y, 0));
                textureIndex3 = numberToID(new Vector3(position3.X, position3.Y, 0));
                textureIndex4 = numberToID(new Vector3(position4.X, position4.Y, 0));
            }

            normals[Index1] += normal;
            normals[Index2] += normal;
            normals[Index3] += normal;
            normals[Index4] += normal;


            VertexIndex vert1 = new VertexIndex(Index1, textureIndex1, Index1);
            VertexIndex vert2 = new VertexIndex(Index2, textureIndex2, Index2);
            VertexIndex vert3 = new VertexIndex(Index3, textureIndex3, Index3);
            VertexIndex vert4 = new VertexIndex(Index4, textureIndex4, Index4);

#if USE_QUADS
            Face newFace = new Face(vert1, vert2, vert3, vert4);
            newFace.isTemp = isBorder;

            Faces.Add(newFace);
#else
            Face tri1 = new Face(vert3, vert2, vert1);
            Face tri2 = new Face(vert2, vert3, vert4);
            tri1.isTemp = isBorder;
            tri2.isTemp = isBorder;
            Faces.Add(tri1);
            Faces.Add(tri2);
#endif

        }

        private void prepareData()
        {
            positions = new ListVector3 { };
            textureCoords = new ListVector2 { };
            normals = new ListVector3 { };
            Faces = new ListFace { };

            int posAmnt = (int)resolution.X * (int)resolution.Y * (int)resolution.Z;
            for (int i = 0; i < posAmnt; i++)
            {
                Vector3 localPos = idToPos(i);
                positions.Add(localPos * 2f - Vector3.One);
                normals.Add(Vector3.Zero);
            }

            int texAmnt = (int)resolution.X * (int)resolution.Y;
            for (int i = 0; i < texAmnt; i++)
            {
                Vector3 localPos = idToPos(i);
                textureCoords.Add(localPos.Xy);
            }
        }

        public Vector3 idToPos(int id)
        {
            return Vector3.Divide(idToNumber(id) - Vector3.One, resolution - Vector3.One * 3);
        }

        public Vector3 idToNumber(int id)
        {
            Vector3 pos = new Vector3();

            int zStep = (int)(resolution.X * resolution.Y); // calculate how many voxels make up one Z layer
            pos.Z = (int)(id) / zStep; // calculate Z position by dividing by this value

            int tmpId = id % zStep; // calculate the remaining part / removing unused zLayers

            int yStep = (int)(resolution.X); // calculate how many voxels make up one Y layer
            pos.Y = (int)(tmpId) / yStep; // calculate Y by dividing by this

            int tmpId2 = tmpId % yStep; // calculate remaining part

            int xStep = 1; // oviously one one voxel equals one layer
            pos.X = (int)(tmpId2) / xStep; // ''

            return pos; // add 1 because of smoothing bounds
        }

        public int numberToID(Vector3 pos)
        {
            if (pos.X < 0 || pos.X >= resolution.X)
                return -1;

            if (pos.Y < 0 || pos.Y >= resolution.Y)
                return -1;

            if (pos.Z < 0 || pos.Z >= resolution.Z)
                return -1;

            return (int)(resolution.X * resolution.Y * pos.Z + resolution.X * pos.Y + pos.X);
        }
    }
}
