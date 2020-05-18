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

    using NthDimension.Algebra;
    using NthDimension.Rendering.Partition;
    using NthDimension.Rendering.Culling;
    using System.Collections.Generic;
    using NthDimension.Rendering.Utilities;

    public partial class PlanetModel : Model // TODO:: Implement Model Routines
    {
        private Terrain[]               planetFace;
        private TerrainQuadTree[]              planetFaceQT;
        private TerrainQuadTreeNode            mainNode;

        // always 4 patches per terrain, for now. Or 4^N.
        private float                   planetRadius            = 100.0f;
        private int                     terrainSize             = 1025;
        private int                     patchSize               = 257;
        private int                     counter                 = 6;
        private int                     maxLOD                  = 7;   // 7

        public enum     LODCubeFaceDirection
        {
            LEFT    = 0,
            TOP     = 1,
            RIGHT   = 2,
            BOTTOM  = 3,
            FRONT   = 4,
            BACK    = 5
        }
        public enum     LODCubeEdgeDirection
        {
            NORTH   = 0,
            EAST    = 1,
            SOUTH   = 2,
            WEST    = 3
        }
        public class    CubeTextureIndices
        {
            public int      Left     { get; set; }
            public int      Right    { get; set; }
            public int      Top      { get; set; }
            public int      Bottom   { get; set; }
            public int      Front    { get; set; }
            public int      Back     { get; set; }
        }
        public class    CubeTexturePaths
        {
            public string   Left        { get; set; }
            public string   Right       { get; set; }
            public string   Top         { get; set; }
            public string   Bottom      { get; set; }
            public string   Front       { get; set; }
            public string   Back        { get; set; }
        }

        public static   QuadTreeNode[,]   LookupNeighbors;

        public PlanetModel(float radius, CubeTextureIndices textures, CubeTexturePaths heightmaps, string detailTexture)
        {
            this.planetRadius = radius;

            planetFace = new Terrain[counter];
            planetFaceQT = new TerrainQuadTree[counter];

            // Left face:
            planetFace[0] = new Terrain(new Vector3(-(terrainSize / 2), -(terrainSize / 2), -(terrainSize / 2)),
                                        new Vector3(1.0f, 0.0f, 0.0f),
                                        new Vector3(0.0f, 1.0f, 0.0f),
                                        maxLOD,
                                        terrainSize,
                                        patchSize,
                                        planetRadius,
                                        textures.Left,
                                        heightmaps.Left,
                                        detailTexture);

            // Top face:
            planetFace[1] = new Terrain(new Vector3(-(terrainSize / 2), (terrainSize / 2), -(terrainSize / 2)),
                                        new Vector3(1.0f, 0.0f, 0.0f),
                                        new Vector3(0.0f, 0.0f, 1.0f),
                                        maxLOD,
                                        terrainSize,
                                        patchSize,
                                        planetRadius,
                                        textures.Top,
                                        heightmaps.Top,
                                        detailTexture);

            // Right face:
            planetFace[2] = new Terrain(new Vector3((terrainSize / 2), -(terrainSize / 2), (terrainSize / 2)),
                                        new Vector3(-1.0f, 0.0f, 0.0f),
                                        new Vector3(0.0f, 1.0f, 0.0f),
                                        maxLOD,
                                        terrainSize,
                                        patchSize,
                                        planetRadius,
                                        textures.Right,
                                        heightmaps.Right,
                                        detailTexture);

            // Bottom face:
            planetFace[3] = new Terrain(new Vector3((terrainSize / 2), -(terrainSize / 2), -(terrainSize / 2)),
                                        new Vector3(-1.0f, 0.0f, 0.0f),
                                        new Vector3(0.0f, 0.0f, 1.0f),
                                        maxLOD,
                                        terrainSize,
                                        patchSize,
                                        planetRadius,
                                        textures.Bottom,
                                        heightmaps.Bottom,
                                        detailTexture);

            planetFaceQT[3] = new TerrainQuadTree(ref planetFace[3], radius * 0.8f);   // 1.5

            // Front face:
            planetFace[4] = new Terrain(new Vector3(-(terrainSize / 2), (terrainSize / 2), -(terrainSize / 2)),
                                        new Vector3(0.0f, 0.0f, 1.0f),
                                        new Vector3(0.0f, -1.0f, 0.0f),
                                        maxLOD,
                                        terrainSize,
                                        patchSize,
                                        planetRadius,
                                        textures.Front,
                                        heightmaps.Front,
                                        detailTexture);

            // Back face:
            planetFace[5] = new Terrain(new Vector3((terrainSize / 2), -(terrainSize / 2), -(terrainSize / 2)),
                                        new Vector3(0.0f, 0.0f, 1.0f),
                                        new Vector3(0.0f, 1.0f, 0.0f),
                                        maxLOD,
                                        terrainSize,
                                        patchSize,
                                        planetRadius,
                                        textures.Back,
                                        heightmaps.Back,
                                        detailTexture);

            // Since there are six terrain faces to create the sphere, there is need to assign the missing border neighbors manually.
            // So we create a new Quadtree.
            // Generate the first node and assign manually the boundingbox.
            mainNode = new TerrainQuadTreeNode(0, Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero);
            // Since its the first node, it gets the first id.
            mainNode.ID = -1;


            // Calculate quadtree.
            for (int i = 0; i < counter; i++)
            {
                planetFaceQT[i] = new TerrainQuadTree(ref planetFace[i], (float)radius * 1.5f);
                planetFaceQT[i].MainNode.Parent = mainNode;
            }

            // Assign manually the specific node ids
            planetFaceQT[0].MainNode.ID = 10 + (int)LODCubeFaceDirection.LEFT;
            planetFaceQT[1].MainNode.ID = 10 + (int)LODCubeFaceDirection.TOP;
            planetFaceQT[2].MainNode.ID = 10 + (int)LODCubeFaceDirection.RIGHT;
            planetFaceQT[3].MainNode.ID = 10 + (int)LODCubeFaceDirection.BOTTOM;
            planetFaceQT[4].MainNode.ID = 10 + (int)LODCubeFaceDirection.FRONT;
            planetFaceQT[5].MainNode.ID = 10 + (int)LODCubeFaceDirection.BACK;

            // Generate lookup table (ugle hack!)
            if (LookupNeighbors == null) CreateNeighborLookupTable();

            // Calculate and assign neighbors

            Console.WriteLine("BB: Level " + -1 + " -> Generated");

            // Calculate and assign the missing nodes neighbors.
            for (int i = 0; i < counter; i++)
            {
                QuadTreeNode mNode = planetFaceQT[i].MainNode;
                QuadTree.CalculateQuadtreeNeighbors(ref mNode);
            }


            #region tries
            //for (int i = 0; i < counter; i++)
            //{
            //    QuadTreeNode mNode = planetFaceQT[i].MainNode;
            //    QuadTree.CalculateQuadtreeNeighbors(ref mNode);
            //}

            ////// try 1
            /////*
            ////// Each node has 4 childs, so to cover all six cube terrain-sides we need four more nodes.
            ////// Neighbors for the left face:
            ////mainNode.NW = new QuadTreeNode(0, Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero);
            ////mainNode.NW.Parent = mainNode;
            ////mainNode.NW.Neighbor_N = planetFaceQT[1].MainNode;
            ////mainNode.NW.Neighbor_E = planetFaceQT[5].MainNode;
            ////mainNode.NW.Neighbor_S = planetFaceQT[3].MainNode;
            ////mainNode.NW.Neighbor_W = planetFaceQT[4].MainNode;
            ////mainNode.NW.ID = 0;
            ////// Neighbors for the back face:
            ////mainNode.NE = new QuadTreeNode(0, Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero);
            ////mainNode.NE.Parent = mainNode;
            ////mainNode.NE.Neighbor_N = planetFaceQT[2].MainNode;
            ////mainNode.NE.Neighbor_E = planetFaceQT[3].MainNode;
            ////mainNode.NE.Neighbor_S = planetFaceQT[0].MainNode;
            ////mainNode.NE.Neighbor_W = planetFaceQT[1].MainNode;
            ////mainNode.NE.ID = 0;
            ////// Neighbors for the right face:
            ////mainNode.SW = new QuadTreeNode(0, Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero);
            ////mainNode.SW.Parent = mainNode;
            ////mainNode.SW.Neighbor_N = planetFaceQT[3].MainNode;
            ////mainNode.SW.Neighbor_E = planetFaceQT[4].MainNode;
            ////mainNode.SW.Neighbor_S = planetFaceQT[1].MainNode;
            ////mainNode.SW.Neighbor_W = planetFaceQT[5].MainNode;
            ////mainNode.SW.ID = 0;
            ////// Neighbors for the front face:
            ////mainNode.SE = new QuadTreeNode(0, Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero);
            ////mainNode.SE.Parent = mainNode;
            ////mainNode.SE.Neighbor_N = planetFaceQT[2].MainNode;
            ////mainNode.SE.Neighbor_E = planetFaceQT[1].MainNode;
            ////mainNode.SE.Neighbor_S = planetFaceQT[0].MainNode;
            ////mainNode.SE.Neighbor_W = planetFaceQT[3].MainNode;
            ////mainNode.SE.ID = 0;
            ////*/
            // try 2
            /*
            for (int i = 0; i < counter; i++)
            {
                QuadTreeNode mNode = planetFaceQT[i].MainNode;
                QuadTree.CalculateQuadtreeNeighbors(ref mNode);
            }
            
            // Neighbors for the left face:
            planetFaceQT[0].MainNode.Neighbor_N = planetFaceQT[1].MainNode;
            planetFaceQT[0].MainNode.Neighbor_E = planetFaceQT[5].MainNode;
            planetFaceQT[0].MainNode.Neighbor_S = planetFaceQT[3].MainNode;
            planetFaceQT[0].MainNode.Neighbor_W = planetFaceQT[4].MainNode;

            // Neighbors for the top face:
            planetFaceQT[1].MainNode.Neighbor_N = planetFaceQT[2].MainNode;
            planetFaceQT[1].MainNode.Neighbor_E = planetFaceQT[5].MainNode;
            planetFaceQT[1].MainNode.Neighbor_S = planetFaceQT[0].MainNode;
            planetFaceQT[1].MainNode.Neighbor_W = planetFaceQT[4].MainNode;

            // Neighbors for the right face:
            planetFaceQT[2].MainNode.Neighbor_N = planetFaceQT[3].MainNode;
            planetFaceQT[2].MainNode.Neighbor_E = planetFaceQT[4].MainNode;
            planetFaceQT[2].MainNode.Neighbor_S = planetFaceQT[1].MainNode;
            planetFaceQT[2].MainNode.Neighbor_W = planetFaceQT[5].MainNode;

            // Neighbors for the bottom face:
            planetFaceQT[3].MainNode.Neighbor_N = planetFaceQT[0].MainNode;
            planetFaceQT[3].MainNode.Neighbor_E = planetFaceQT[4].MainNode;
            planetFaceQT[3].MainNode.Neighbor_S = planetFaceQT[2].MainNode;
            planetFaceQT[3].MainNode.Neighbor_W = planetFaceQT[5].MainNode;

            // Neighbors for the front face:
            planetFaceQT[4].MainNode.Neighbor_N = planetFaceQT[2].MainNode;
            planetFaceQT[4].MainNode.Neighbor_E = planetFaceQT[1].MainNode;
            planetFaceQT[4].MainNode.Neighbor_S = planetFaceQT[0].MainNode;
            planetFaceQT[4].MainNode.Neighbor_W = planetFaceQT[3].MainNode;

            // Neighbors for the back face:
            planetFaceQT[5].MainNode.Neighbor_N = planetFaceQT[2].MainNode;
            planetFaceQT[5].MainNode.Neighbor_E = planetFaceQT[3].MainNode;
            planetFaceQT[5].MainNode.Neighbor_S = planetFaceQT[0].MainNode;
            planetFaceQT[5].MainNode.Neighbor_W = planetFaceQT[1].MainNode;

            */
            #endregion



            // Neighborsuche geschieht über die Parents. Die müssen hier noch zugewiesen werden.
            // da es aber 6 terrains sind, brauche ich vier quadtrees mit jeweils vier neighbors.
            // als super node wird dann eine node als wurzel dienen.

            //Game.PopState();
            // ie  GL.PopClientAttrib();
            //     GL.PopMatrix();
            //
            // <disabled>

            Console.WriteLine("PlanetMesh generated!");
        }

        public void CreateNeighborLookupTable()
        {
            LookupNeighbors = new QuadTreeNode[6, 4];
            // Neighbors for the left face
            LookupNeighbors[(int)LODCubeFaceDirection.LEFT, (int)LODCubeEdgeDirection.NORTH] = planetFaceQT[(int)LODCubeFaceDirection.BOTTOM].MainNode;
            LookupNeighbors[(int)LODCubeFaceDirection.LEFT, (int)LODCubeEdgeDirection.EAST] = planetFaceQT[(int)LODCubeFaceDirection.BACK].MainNode;
            LookupNeighbors[(int)LODCubeFaceDirection.LEFT, (int)LODCubeEdgeDirection.SOUTH] = planetFaceQT[(int)LODCubeFaceDirection.TOP].MainNode;
            LookupNeighbors[(int)LODCubeFaceDirection.LEFT, (int)LODCubeEdgeDirection.WEST] = planetFaceQT[(int)LODCubeFaceDirection.FRONT].MainNode;
            // Neighbors for the top face
            LookupNeighbors[(int)LODCubeFaceDirection.TOP, (int)LODCubeEdgeDirection.NORTH] = planetFaceQT[(int)LODCubeFaceDirection.LEFT].MainNode;
            LookupNeighbors[(int)LODCubeFaceDirection.TOP, (int)LODCubeEdgeDirection.EAST] = planetFaceQT[(int)LODCubeFaceDirection.BACK].MainNode;
            LookupNeighbors[(int)LODCubeFaceDirection.TOP, (int)LODCubeEdgeDirection.SOUTH] = planetFaceQT[(int)LODCubeFaceDirection.RIGHT].MainNode;
            LookupNeighbors[(int)LODCubeFaceDirection.TOP, (int)LODCubeEdgeDirection.WEST] = planetFaceQT[(int)LODCubeFaceDirection.FRONT].MainNode;
            // Neighbors for the right face
            LookupNeighbors[(int)LODCubeFaceDirection.RIGHT, (int)LODCubeEdgeDirection.NORTH] = planetFaceQT[(int)LODCubeFaceDirection.BOTTOM].MainNode;
            LookupNeighbors[(int)LODCubeFaceDirection.RIGHT, (int)LODCubeEdgeDirection.EAST] = planetFaceQT[(int)LODCubeFaceDirection.FRONT].MainNode;
            LookupNeighbors[(int)LODCubeFaceDirection.RIGHT, (int)LODCubeEdgeDirection.SOUTH] = planetFaceQT[(int)LODCubeFaceDirection.TOP].MainNode;
            LookupNeighbors[(int)LODCubeFaceDirection.RIGHT, (int)LODCubeEdgeDirection.WEST] = planetFaceQT[(int)LODCubeFaceDirection.BACK].MainNode;
            // Neighbors for the bottom face
            LookupNeighbors[(int)LODCubeFaceDirection.BOTTOM, (int)LODCubeEdgeDirection.NORTH] = planetFaceQT[(int)LODCubeFaceDirection.LEFT].MainNode;
            LookupNeighbors[(int)LODCubeFaceDirection.BOTTOM, (int)LODCubeEdgeDirection.EAST] = planetFaceQT[(int)LODCubeFaceDirection.FRONT].MainNode;
            LookupNeighbors[(int)LODCubeFaceDirection.BOTTOM, (int)LODCubeEdgeDirection.SOUTH] = planetFaceQT[(int)LODCubeFaceDirection.RIGHT].MainNode;
            LookupNeighbors[(int)LODCubeFaceDirection.BOTTOM, (int)LODCubeEdgeDirection.WEST] = planetFaceQT[(int)LODCubeFaceDirection.BACK].MainNode;
            // Neighbors for the front face
            LookupNeighbors[(int)LODCubeFaceDirection.FRONT, (int)LODCubeEdgeDirection.NORTH] = planetFaceQT[(int)LODCubeFaceDirection.TOP].MainNode;
            LookupNeighbors[(int)LODCubeFaceDirection.FRONT, (int)LODCubeEdgeDirection.EAST] = planetFaceQT[(int)LODCubeFaceDirection.RIGHT].MainNode;
            LookupNeighbors[(int)LODCubeFaceDirection.FRONT, (int)LODCubeEdgeDirection.SOUTH] = planetFaceQT[(int)LODCubeFaceDirection.BOTTOM].MainNode;
            LookupNeighbors[(int)LODCubeFaceDirection.FRONT, (int)LODCubeEdgeDirection.WEST] = planetFaceQT[(int)LODCubeFaceDirection.LEFT].MainNode;
            // Neighbors for the back face
            LookupNeighbors[(int)LODCubeFaceDirection.BACK, (int)LODCubeEdgeDirection.NORTH] = planetFaceQT[(int)LODCubeFaceDirection.BOTTOM].MainNode;
            LookupNeighbors[(int)LODCubeFaceDirection.BACK, (int)LODCubeEdgeDirection.EAST] = planetFaceQT[(int)LODCubeFaceDirection.LEFT].MainNode;
            LookupNeighbors[(int)LODCubeFaceDirection.BACK, (int)LODCubeEdgeDirection.SOUTH] = planetFaceQT[(int)LODCubeFaceDirection.TOP].MainNode;
            LookupNeighbors[(int)LODCubeFaceDirection.BACK, (int)LODCubeEdgeDirection.WEST] = planetFaceQT[(int)LODCubeFaceDirection.RIGHT].MainNode;
        }

        public void Render(ref Frustum frustum)
        {
            for (int i = 0; i < planetFaceQT.Length; i++)
            {
                //throw new NotImplementedException("Ref passign a Frustum object");
                if (planetFaceQT[i] != null) planetFaceQT[i].Render(ref frustum);


            }
        }
        public override void draw(GameViews.ViewInfo curView)
        {
            // TODO:: See if you I create a visible frustum from the curView and draw calls as usual
            //base.draw(curView);
        }
        public override void draw()
        {
            //// TODO:: Implement Patches MeshVbo and Render accordingly
            ///// TODO:: See if I can create a visible frustum from the curView and draw calls as usual

            // if(null == meshes)
            // {
            //     meshes = new Geometry.MeshVbo[planetFace.Length];

            //     for(int i = 0; i < meshes.Length; i++)
            //     {
            //         //meshes[i].Type = Geometry.MeshVbo.MeshType.Generated;


            //         //meshes[i] = new Geometry.MeshVbo();
            //         //meshes[i].MeshData.Positions = new Vector3[planetFace[i].Vertices.Count];

            //         //for (int v = 0; v < planetFace[i].Vertices.Count; v++)
            //         //    meshes[i].MeshData.Positions[v] = planetFace[i].Vertices[v];

            //         //meshes[i].MeshData.Indices = new int[planetFace[i].IndexBuffer.Length];

            //         //for (int d = 0; d < planetFace[i].IndexBuffer.Length; d++)
            //         //    meshes[i].MeshData.Indices[d] = planetFace[i].IndexBuffer[d];

            //         Serialization.ListFace faces = new Serialization.ListFace();

            //         for (int d = 0; d < planetFace[i].IndexBuffer.Length; d+=3)
            //         {
            //             int A = planetFace[i].IndexBuffer[d];
            //             int B = planetFace[i].IndexBuffer[d + 1];
            //             int C = planetFace[i].IndexBuffer[d + 2];
            //             faces.Add(new Geometry.Face(new VertexIndices(A), new VertexIndices(B), new VertexIndices(C)));
            //         }

            //         Serialization.ListVector3 verts = new Serialization.ListVector3();
            //         verts.AddRange(planetFace[i].Vertices);

            //         meshes[i] = ApplicationBase.Instance.MeshLoader.FromMesh(verts, new Serialization.ListVector3(), new Serialization.ListVector2(), faces);
            //     }

            // }

            // if(null == materials)
            // {

            // }

            // base.draw();
        }

        public void Destroy()
        {

        }
    }

    class TerrainQuadTreeNode : QuadTreeNode
    {
        public new TerrainQuadTreeNode NW = null;
        public new TerrainQuadTreeNode NE = null;
        public new TerrainQuadTreeNode SW = null;
        public new TerrainQuadTreeNode SE = null;

        public new TerrainQuadTreeNode Neighbor_N = null;
        public new TerrainQuadTreeNode Neighbor_E = null;
        public new TerrainQuadTreeNode Neighbor_S = null;
        public new TerrainQuadTreeNode Neighbor_W = null;

        public new TerrainQuadTreeNode Parent = null;

        public PlanetModel.Terrain.TerrainPatch Patch;      // TODO:: Refactor <Generic Object> Reference


        public TerrainQuadTreeNode(int nodelevel, Vector3 topLeftBBV, Vector3 topRightBBV, Vector3 bottomLeftBBV, Vector3 bottomRightBBV)
            : base(nodelevel, topLeftBBV, topRightBBV, bottomLeftBBV, bottomRightBBV)
        {


        }

        public override void Destroy()
        {
            if (Patch != null)
                Patch.Destroy();
            
        }
    }

    class TerrainQuadTree : QuadTree
    {
        private PlanetModel.Terrain     terrainRef;                                 // TODO (major):: De-Coupple from QuadTree
        private float                   mDistanceFactor             = 150f;         // for radius 100

        private TerrainQuadTreeNode     mainNode;

        public new TerrainQuadTreeNode MainNode
        {
            get { return mainNode; }
        }
       

        public TerrainQuadTree(ref PlanetModel.Terrain terrain, float distanceFactor)
            :base()
        {
            mDistanceFactor = distanceFactor;
            terrainRef = terrain;

            // generate the main node with the 4 bounding box vertices
            int[] BoundingBox = new int[4];
            BoundingBox[0] = 0;
            BoundingBox[1] = terrain.Width - 1;
            BoundingBox[2] = (terrain.Height - 1) * terrain.Width;
            BoundingBox[3] = ((terrain.Height - 1) * terrain.Width) + (terrain.Width - 1);

            mainNode = new TerrainQuadTreeNode(0,
                terrain.Vertices[BoundingBox[0]],
                terrain.Vertices[BoundingBox[1]],
                terrain.Vertices[BoundingBox[2]],
                terrain.Vertices[BoundingBox[3]]);
            AssignBoundingBox3D(ref mainNode, BoundingBox[0], BoundingBox[1], BoundingBox[2], BoundingBox[3]);
            
            ConsoleUtil.log("BB: Level " + 1 + " -> " + BoundingBox[0] + " " + BoundingBox[1] + " " + BoundingBox[2] + " " + BoundingBox[3]);
            // since its the first node, it gets the first id.
            mainNode.ID = 0;

            TerrainQuadTreeNode parent = null;
            GenerateQuadtreeBranch(ref parent, ref mainNode, BoundingBox);
            Console.WriteLine("(Class: Quadtree) Quadtree generated");

        }

        public override void Destroy()
        {
            base.Destroy();
            terrainRef.Destroy();
        }

        private void AssignBoundingBox3D(ref TerrainQuadTreeNode node, int topLeftVertex, int topRightVertex, int bottomLeftVertex, int bottomRightVertex)
        {
            // Calculate the highest point of the terrain patch.
            // First use a dummy to make sure that we get the highest point.
            Vector3 highestV = new Vector3(terrainRef.Vertices[0].X - 999, terrainRef.Vertices[0].Y - 999, terrainRef.Vertices[0].Z - 999);
            for (int z = topLeftVertex; z <= bottomLeftVertex; z += terrainRef.Width)
            {
                for (int x = z; x < (z + (topRightVertex - topLeftVertex) + 1); x++)
                {
                    if (terrainRef.Vertices[x].X > highestV.X)
                    {
                        highestV.X = terrainRef.Vertices[x].X;
                    }
                    if (terrainRef.Vertices[x].Y > highestV.Y)
                    {
                        highestV.Y = terrainRef.Vertices[x].Y;
                    }
                    if (terrainRef.Vertices[x].Z > highestV.Z)
                    {
                        highestV.Z = terrainRef.Vertices[x].Z;
                    }
                }
            }
            // This prevents a bounding box with a height, width and length of 0
            if (highestV.X == 0.0f) { highestV.X = 1.0f; }
            if (highestV.Y == 0.0f) { highestV.Y = 1.0f; }
            if (highestV.Z == 0.0f) { highestV.Z = 1.0f; }

            Vector3 lowestV = new Vector3(highestV);
            // Calculate the lowest point. Used to calculate the boundingbox later on
            for (int z = topLeftVertex; z <= bottomLeftVertex; z += terrainRef.Width)
            {
                for (int x = z; x < (z + (topRightVertex - topLeftVertex) + 1); x++)
                {
                    if (terrainRef.Vertices[x].X < lowestV.X)
                    {
                        lowestV.X = terrainRef.Vertices[x].X;
                    }
                    if (terrainRef.Vertices[x].Y < lowestV.Y)
                    {
                        lowestV.Y = terrainRef.Vertices[x].Y;
                    }
                    if (terrainRef.Vertices[x].Z < lowestV.Z)
                    {
                        lowestV.Z = terrainRef.Vertices[x].Z;
                    }
                }
            }


            //highestV.Y = lowestV.Y = 0;

            // Calculate the 3d bounding box.
            Vector3[] bbV = new Vector3[8];
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

            node.BoundingBox3D = bbV;
        }
        private void AssignPatch(ref TerrainQuadTreeNode node, NodeDirection nodeDirection, int topLeftVertex, int topRightVertex, int bottomLeftVertex, int bottomRightVertex)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uvCoor = new List<Vector2>();

            for (int z = topLeftVertex; z <= bottomLeftVertex; z += terrainRef.Width)
            {
                for (int x = z; x < (z + (topRightVertex - topLeftVertex) + 1); x++)
                {
                    vertices.Add(terrainRef.Vertices[x]);
                    uvCoor.Add(terrainRef.uvCoordinates[x]);
                }
            }
            ((TerrainQuadTreeNode)node).Patch = new PlanetModel.Terrain.TerrainPatch((node.Level * 10) + (int)nodeDirection, node.BoundingBox2D, vertices, uvCoor, topRightVertex - topLeftVertex, mDistanceFactor);
        }

        private void GenerateQuadtreeBranch(ref TerrainQuadTreeNode parent, ref TerrainQuadTreeNode mainNode, int[] BoundingBox)
        {
            int[] bb = new int[4];
            int currentLevel = mainNode.Level + 1;

            if (parent != null) mainNode.Parent = parent;

            // Top Left child
            bb[0] = BoundingBox[0];                                                                                     // top left vertex (b[0])
            bb[1] = BoundingBox[0] + ((BoundingBox[1] - BoundingBox[0]) / 2);                                           // between b[0] and b[1]   
            bb[2] = BoundingBox[0] + ((BoundingBox[2] - BoundingBox[0]) / 2);                                           // between b[0] and b[2]
            bb[3] = BoundingBox[0] + ((BoundingBox[2] - BoundingBox[0]) / 2) + ((BoundingBox[1] - BoundingBox[0]) / 2); // middle of mainNode

            mainNode.NW = new TerrainQuadTreeNode(currentLevel,
                terrainRef.Vertices[bb[0]],
                terrainRef.Vertices[bb[1]],
                terrainRef.Vertices[bb[2]],
                terrainRef.Vertices[bb[3]]);
            mainNode.NW.Parent = mainNode;
            mainNode.NW.ID = (int)NodeDirection.NORTH_WEST;
            if ((bb[1] - bb[0]) > terrainRef.PatchSize)
            {
                // calculate 3d boundingbox for frustum check
                AssignBoundingBox3D(ref mainNode.NW, bb[0], bb[1], bb[2], bb[3]);
                // generate childs (if current node isn't a leaf)
                GenerateQuadtreeBranch(ref mainNode, ref mainNode.NW, bb);
                Console.WriteLine("BB: Level " + currentLevel + " -> " + bb[0] + " " + bb[1] + " " + bb[2] + " " + bb[3]);
            }
            else
            {
                // if node is a leaf (because the patchwidth = the desired patchsize for a leaf), assign the patches to this node
                AssignPatch(ref mainNode.NW, NodeDirection.NORTH_WEST, bb[0], bb[1], bb[2], bb[3]);
                Console.WriteLine("BB Leaf: Level " + currentLevel + " -> " + bb[0] + " " + bb[1] + " " + bb[2] + " " + bb[3]);
            }

            // Top Right child
            bb[0] = BoundingBox[0] + ((BoundingBox[1] - BoundingBox[0]) / 2);                                           // between b[0] and b[1]
            bb[1] = BoundingBox[1];                                                                                     // top right vertex (b[1])   
            bb[2] = BoundingBox[0] + ((BoundingBox[2] - BoundingBox[0]) / 2) + ((BoundingBox[1] - BoundingBox[0]) / 2); // middle of mainNode
            bb[3] = BoundingBox[0] + ((BoundingBox[2] - BoundingBox[0]) / 2) + ((BoundingBox[1] - BoundingBox[0]));     // between b[1] and b[3]

            mainNode.NE = new TerrainQuadTreeNode(currentLevel,
                terrainRef.Vertices[bb[0]],
                terrainRef.Vertices[bb[1]],
                terrainRef.Vertices[bb[2]],
                terrainRef.Vertices[bb[3]]);
            mainNode.NE.Parent = mainNode;
            mainNode.NE.ID = (int)NodeDirection.NORTH_EAST;
            if ((bb[1] - bb[0]) > terrainRef.PatchSize)
            {
                // calculate 3d boundingbox for frustum check
                AssignBoundingBox3D(ref mainNode.NE, bb[0], bb[1], bb[2], bb[3]);
                // generate childs (if current node isn't a leaf)
                GenerateQuadtreeBranch(ref mainNode, ref mainNode.NE, bb);
                Console.WriteLine("BB: Level " + currentLevel + " -> " + bb[0] + " " + bb[1] + " " + bb[2] + " " + bb[3]);
            }
            else
            {
                // if node is a leaf (because the patchwidth = the desired patchsize for a leaf), assign the patches to this node
                AssignPatch(ref mainNode.NE, NodeDirection.NORTH_EAST, bb[0], bb[1], bb[2], bb[3]);
                Console.WriteLine("BB Leaf: Level " + currentLevel + " -> " + bb[0] + " " + bb[1] + " " + bb[2] + " " + bb[3]);
            }

            // Bottom Left child
            bb[0] = BoundingBox[0] + ((BoundingBox[2] - BoundingBox[0]) / 2);                                           // between b[0] and b[2]
            bb[1] = BoundingBox[0] + ((BoundingBox[2] - BoundingBox[0]) / 2) + ((BoundingBox[1] - BoundingBox[0]) / 2); // middle of mainNode  
            bb[2] = BoundingBox[2];                                                                                     // bottom Left vertex (b[2])
            bb[3] = BoundingBox[2] + ((BoundingBox[3] - BoundingBox[2]) / 2);                                           // between b[2] and b[3]

            mainNode.SW = new TerrainQuadTreeNode(currentLevel,
                terrainRef.Vertices[bb[0]],
                terrainRef.Vertices[bb[1]],
                terrainRef.Vertices[bb[2]],
                terrainRef.Vertices[bb[3]]);
            mainNode.SW.Parent = mainNode;
            mainNode.SW.ID = (int)NodeDirection.SOUTH_WEST;
            if ((bb[1] - bb[0]) > terrainRef.PatchSize)
            {
                // calculate 3d boundingbox for frustum check
                AssignBoundingBox3D(ref mainNode.SW, bb[0], bb[1], bb[2], bb[3]);
                // generate childs (if current node isn't a leaf)
                GenerateQuadtreeBranch(ref mainNode, ref mainNode.SW, bb);
                Console.WriteLine("BB: Level " + currentLevel + " -> " + bb[0] + " " + bb[1] + " " + bb[2] + " " + bb[3]);
            }
            else
            {
                // if node is a leaf (because the patchwidth = the desired patchsize for a leaf), assign the patches to this node
                AssignPatch(ref mainNode.SW, NodeDirection.SOUTH_WEST, bb[0], bb[1], bb[2], bb[3]);
                Console.WriteLine("BB Leaf: Level " + currentLevel + " -> " + bb[0] + " " + bb[1] + " " + bb[2] + " " + bb[3]);
            }

            // Bottom Right child
            bb[0] = BoundingBox[0] + ((BoundingBox[2] - BoundingBox[0]) / 2) + ((BoundingBox[1] - BoundingBox[0]) / 2); // middle of mainNode
            bb[1] = BoundingBox[0] + ((BoundingBox[2] - BoundingBox[0]) / 2) + ((BoundingBox[1] - BoundingBox[0]));     // between b[1] and b[3]
            bb[2] = BoundingBox[2] + ((BoundingBox[3] - BoundingBox[2]) / 2);                                           // between b[2] and b[3]
            bb[3] = BoundingBox[3];                                                                                     // bottom Right vertex (b[3])

            mainNode.SE = new TerrainQuadTreeNode(currentLevel,
                terrainRef.Vertices[bb[0]],
                terrainRef.Vertices[bb[1]],
                terrainRef.Vertices[bb[2]],
                terrainRef.Vertices[bb[3]]);
            mainNode.SE.Parent = mainNode;
            mainNode.SE.ID = (int)NodeDirection.SOUTH_EAST;
            if ((bb[1] - bb[0]) > terrainRef.PatchSize)
            {
                // calculate 3d boundingbox for frustum check
                AssignBoundingBox3D(ref mainNode.SE, bb[0], bb[1], bb[2], bb[3]);
                // generate childs (if current node isn't a leaf)
                GenerateQuadtreeBranch(ref mainNode, ref mainNode.SE, bb);
                Console.WriteLine("BB: Level " + currentLevel + " -> " + bb[0] + " " + bb[1] + " " + bb[2] + " " + bb[3]);
            }
            else
            {
                // if node is a leaf (because the patchwidth = the desired patchsize for a leaf), assign the patches to this node
                AssignPatch(ref mainNode.SE, NodeDirection.SOUTH_EAST, bb[0], bb[1], bb[2], bb[3]);
                Console.WriteLine("BB Leaf: Level " + currentLevel + " -> " + bb[0] + " " + bb[1] + " " + bb[2] + " " + bb[3]);
            }

            return;
        }

        private void CheckNodeInsideFrustum(/*ref Frustum frustum,*/ ref TerrainQuadTreeNode mainNode, bool checkFrustum = true)
        {
            throw new NotImplementedException("Frustum Check");
            //Frustum.InFrustumCheck nodeInFrustum;
            //bool checkChildFrustum = true;
            //// if the node has childs, check if nodes childs are in or outside the frustum
            //if (mainNode.HasChilds())
            //{
            //    for (int i = 1; i <= 4; i++)
            //    {
            //        QuadTreeNode node = mainNode.GetChild(i);
            //        if (checkFrustum)
            //        {
            //            if (node.HasChilds())
            //            {
            //                nodeInFrustum = frustum.CubeInFrustum(node.BoundingBox3D);
            //            }
            //            else
            //            {
            //                nodeInFrustum = frustum.CubeInFrustum(node.Patch);
            //            }
            //            if (nodeInFrustum == Frustum.InFrustumCheck.OUT) { continue; }
            //            if (nodeInFrustum == Frustum.InFrustumCheck.IN) { checkChildFrustum = false; }
            //        }
            //        CheckNodeInsideFrustum(ref frustum, ref node, checkChildFrustum);
            //    }
            //}
            //// if it don't, it's a leaf and the terrain patch can be rendered
            //else
            {
                int maxLODLevels = terrainRef.LODLEVELS;
                int patchResolution = mainNode.Patch.GetResolution(maxLODLevels);
                if ((mainNode.Patch.UpdateToResolution > -1) && (mainNode.Patch.UpdateToResolution != patchResolution)) patchResolution = mainNode.Patch.UpdateToResolution;
                int mainPatchIndexBuffer = terrainRef.IndexBuffer[patchResolution];

                List<int> defaultBridgeIndexBuffer = new List<int>();
                List<int> lowerBridgeIndexBuffer = new List<int>();

                if (mainNode.Neighbor_N != null)
                {
                    // check north neighbor patch resolution
                    int nRes = mainNode.Neighbor_N.Patch.GetResolution(maxLODLevels);
                    if (nRes > patchResolution)
                    {
                        if (nRes > patchResolution * 2) mainNode.Neighbor_N.Patch.UpdateToResolution = patchResolution * 2;
                        lowerBridgeIndexBuffer.Add(terrainRef.BridgeIndexBuffer[patchResolution, 4]);
                    }
                    else
                    {
                        mainNode.Neighbor_N.Patch.UpdateToResolution = -1;
                        defaultBridgeIndexBuffer.Add(terrainRef.BridgeIndexBuffer[patchResolution, 0]);
                    }
                }
                else
                    defaultBridgeIndexBuffer.Add(terrainRef.BridgeIndexBuffer[patchResolution, 0]);
                if (mainNode.Neighbor_E != null)
                {
                    // check east neighbor patch resolution
                    int eRes = mainNode.Neighbor_E.Patch.GetResolution(maxLODLevels);
                    if (eRes > patchResolution)
                    {
                        if (eRes > patchResolution * 2) mainNode.Neighbor_E.Patch.UpdateToResolution = patchResolution * 2;
                        lowerBridgeIndexBuffer.Add(terrainRef.BridgeIndexBuffer[patchResolution, 5]);
                    }
                    else
                    {
                        mainNode.Neighbor_E.Patch.UpdateToResolution = -1;
                        defaultBridgeIndexBuffer.Add(terrainRef.BridgeIndexBuffer[patchResolution, 1]);
                    }
                }
                else
                    defaultBridgeIndexBuffer.Add(terrainRef.BridgeIndexBuffer[patchResolution, 1]);
                if (mainNode.Neighbor_S != null)
                {
                    // check south neighbor patch resolution
                    int sRes = mainNode.Neighbor_S.Patch.GetResolution(maxLODLevels);
                    if (sRes > patchResolution)
                    {
                        if (sRes > patchResolution * 2) mainNode.Neighbor_S.Patch.UpdateToResolution = patchResolution * 2;
                        lowerBridgeIndexBuffer.Add(terrainRef.BridgeIndexBuffer[patchResolution, 6]);
                    }
                    else
                    {
                        mainNode.Neighbor_S.Patch.UpdateToResolution = -1;
                        defaultBridgeIndexBuffer.Add(terrainRef.BridgeIndexBuffer[patchResolution, 2]);
                    }
                }
                else
                    defaultBridgeIndexBuffer.Add(terrainRef.BridgeIndexBuffer[patchResolution, 2]);
                if (mainNode.Neighbor_W != null)
                {
                    // check west neighbor patch resolution
                    int wRes = mainNode.Neighbor_W.Patch.GetResolution(maxLODLevels);
                    if (wRes > patchResolution)
                    {
                        if (wRes > patchResolution * 2) mainNode.Neighbor_W.Patch.UpdateToResolution = patchResolution * 2;
                        lowerBridgeIndexBuffer.Add(terrainRef.BridgeIndexBuffer[patchResolution, 7]);
                    }
                    else
                    {
                        mainNode.Neighbor_W.Patch.UpdateToResolution = -1;
                        defaultBridgeIndexBuffer.Add(terrainRef.BridgeIndexBuffer[patchResolution, 3]);
                    }
                }
                else
                    defaultBridgeIndexBuffer.Add(terrainRef.BridgeIndexBuffer[patchResolution, 3]);

                int[] indicesCount = new int[3];
                indicesCount[0] = terrainRef.IndicesCount[patchResolution];                 // main patch indices count
                indicesCount[1] = terrainRef.BridgeIndicesCount[0, patchResolution];        // default bridge indices count
                indicesCount[2] = terrainRef.BridgeIndicesCount[1, patchResolution];        // lower bridge indices count

                mainNode.Patch.Render(mainPatchIndexBuffer, defaultBridgeIndexBuffer.ToArray(), lowerBridgeIndexBuffer.ToArray(), indicesCount, terrainRef.TextureID, terrainRef.DetailTextureID);
            }
            return;
        }
    }
}
