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

namespace NthDimension.Rendering.Partition
{
    using System;
    using System.Collections.Generic;

    using NthDimension.Algebra;
    using NthDimension.Rendering.Drawables.Models;   // TODO:: (Refactor and kill reference)
    using NthDimension.Rendering.Culling;

    public class QuadTree
    {
        #region Enumerators
        public enum NodeDirection
        {
            NORTH_WEST = 1,
            NORTH_EAST = 2,
            SOUTH_WEST = 3,
            SOUTH_EAST = 4
        }

        public enum NodeNeighborDirection
        {
            NORTH = 1,
            EAST = 2,
            SOUTH = 3,
            WEST = 4
        }
        #endregion
#pragma warning disable CS0649
        private QuadTreeNode mainNode;      // TODO:: Refactor <Generic Object> Reference

        //private int totalLeaves;
        //private int maxLevels;


        public QuadTreeNode MainNode
        { get { return mainNode; } }
        
        //private PlanetModel.Terrain terrainRef;         // TODO (major):: De-Coupple from QuadTree

        //private float mDistanceFactor = 150f; // for radius 100

        public QuadTree(/*ref PlanetModel.Terrain terrain, float distanceFactor*/)
        {
            //mDistanceFactor = distanceFactor;
            //terrainRef = terrain;

            // the number of leaves to create
            //totalLeaves = terrainRef.PatchesPerRow;

            // calculate the last level where the nodes are being referenced to the patches (each node to one patch)
            //maxLevels = CalcMaxLevels(totalLeaves);

        
        }

        private int CalcMaxLevels(int val)      // obsolete?
        {
            // calulate the 4th root of 'val'
            int result = (int)Math.Pow(val, 0.25);
            Console.WriteLine("(Class: QuadTree) Calculated " + result + " as the maximum level of depth (With " + val + " patches)");
            return result;
        }

        

        

        public static void CalculateQuadtreeNeighbors(ref QuadTreeNode mainNode)
        {
            // assign all the adjacent nodes
            if (mainNode.NW != null)
            {
                mainNode.NW.Neighbor_N = FindNeighborNorth(ref mainNode.NW);
                mainNode.NW.Neighbor_E = FindNeighborEast(ref mainNode.NW); //mainNode.NE;
                mainNode.NW.Neighbor_S = FindNeighborSouth(ref mainNode.NW); //mainNode.SW;
                mainNode.NW.Neighbor_W = FindNeighborWest(ref mainNode.NW);
            }
            if (mainNode.NE != null)
            {
                mainNode.NE.Neighbor_N = FindNeighborNorth(ref mainNode.NE);
                mainNode.NE.Neighbor_E = FindNeighborEast(ref mainNode.NE);
                mainNode.NE.Neighbor_S = FindNeighborSouth(ref mainNode.NE);  //mainNode.SE;
                mainNode.NE.Neighbor_W = FindNeighborWest(ref mainNode.NE); //mainNode.NW;
            }
            if (mainNode.SW != null)
            {
                mainNode.SW.Neighbor_N = FindNeighborNorth(ref mainNode.SW);
                mainNode.SW.Neighbor_E = FindNeighborEast(ref mainNode.SW); //mainNode.SE;
                mainNode.SW.Neighbor_S = FindNeighborSouth(ref mainNode.SW);
                mainNode.SW.Neighbor_W = FindNeighborWest(ref mainNode.SW);
            }
            if (mainNode.SE != null)
            {
                mainNode.SE.Neighbor_N = FindNeighborNorth(ref mainNode.SE);
                mainNode.SE.Neighbor_E = FindNeighborEast(ref mainNode.SE);
                mainNode.SE.Neighbor_S = FindNeighborSouth(ref mainNode.SE);
                mainNode.SE.Neighbor_W = FindNeighborWest(ref mainNode.SE);
            }

            if ((mainNode.NW != null) && (mainNode.NW.HasChilds()))
            {
                CalculateQuadtreeNeighbors(ref mainNode.NW);
            }
            if ((mainNode.NE != null) && (mainNode.NE.HasChilds()))
            {
                CalculateQuadtreeNeighbors(ref mainNode.NE);
            }
            if ((mainNode.SW != null) && (mainNode.SW.HasChilds()))
            {
                CalculateQuadtreeNeighbors(ref mainNode.SW);
            }
            if ((mainNode.SE != null) && (mainNode.SE.HasChilds()))
            {
                CalculateQuadtreeNeighbors(ref mainNode.SE);
            }

            return;
        }

        private static QuadTreeNode FindNeighborNorth(ref QuadTreeNode node)
        {
            if (node.ID == -1) return null;
            if ((node.ID >= 10) && (node.ID < 20)) return PlanetModel.LookupNeighbors[node.ID - 10, (int)PlanetModel.LODCubeEdgeDirection.NORTH];
            if (node.ID == (int)NodeDirection.SOUTH_WEST)
                return node.Parent.NW;
            if (node.ID == (int)NodeDirection.SOUTH_EAST)
                return node.Parent.NE;
            QuadTreeNode tempNode = FindNeighborNorth(ref node.Parent);
            if ((tempNode == null) || (!tempNode.HasChilds()))
                return tempNode;
            else
                if (node.ID == (int)NodeDirection.NORTH_WEST)
                return tempNode.SW;
            return tempNode.SE;
        }

        private static QuadTreeNode FindNeighborEast(ref QuadTreeNode node)
        {
            if (node.ID == -1) return null;
            if ((node.ID >= 10) && (node.ID < 20)) return PlanetModel.LookupNeighbors[node.ID - 10, (int)PlanetModel.LODCubeEdgeDirection.EAST];
            if (node.ID == (int)NodeDirection.NORTH_WEST)
                return node.Parent.NE;
            if (node.ID == (int)NodeDirection.SOUTH_WEST)
                return node.Parent.SE;
            QuadTreeNode tempNode = FindNeighborEast(ref node.Parent);
            if ((tempNode == null) || (!tempNode.HasChilds()))
                return tempNode;
            else
                if (node.ID == (int)NodeDirection.NORTH_EAST)
                return tempNode.NW;
            return tempNode.SW;
        }

        private static QuadTreeNode FindNeighborSouth(ref QuadTreeNode node)
        {
            if (node.ID == -1) return null;
            if ((node.ID >= 10) && (node.ID < 20)) return PlanetModel.LookupNeighbors[node.ID - 10, (int)PlanetModel.LODCubeEdgeDirection.SOUTH];
            if (node.ID == (int)NodeDirection.NORTH_WEST)
                return node.Parent.SW;
            if (node.ID == (int)NodeDirection.NORTH_EAST)
                return node.Parent.SE;
            QuadTreeNode tempNode = FindNeighborSouth(ref node.Parent);
            if ((tempNode == null) || (!tempNode.HasChilds()))
                return tempNode;
            else
                if (node.ID == (int)NodeDirection.SOUTH_WEST)
                return tempNode.NW;
            return tempNode.NE;
        }

        private static QuadTreeNode FindNeighborWest(ref QuadTreeNode node)
        {
            if (node.ID == -1) return null;
            if ((node.ID >= 10) && (node.ID < 20)) return PlanetModel.LookupNeighbors[node.ID - 10, (int)PlanetModel.LODCubeEdgeDirection.WEST];
            if (node.ID == (int)NodeDirection.NORTH_EAST)
                return node.Parent.NW;
            if (node.ID == (int)NodeDirection.SOUTH_EAST)
                return node.Parent.SW;
            QuadTreeNode tempNode = FindNeighborWest(ref node.Parent);
            if ((tempNode == null) || (!tempNode.HasChilds()))
                return tempNode;
            else
                if (node.ID == (int)NodeDirection.NORTH_WEST)
                return tempNode.NE;
            return tempNode.SE;
        }

        public virtual void Destroy()
        {
            if (mainNode == null) return;
            RecuDestroy(mainNode);
            
        }

        private void RecuDestroy(QuadTreeNode mainNode)
        {
            if (mainNode.HasChilds())
            {
                for (int i = 1; i <= 4; i++)
                {
                    QuadTreeNode node = mainNode.GetChild(i);
                    RecuDestroy(node);
                }
            }
            mainNode.Destroy();
        }

        public void Render(ref Frustum frustum)
        {
            throw new NotImplementedException("Frustum check and GL Call");
            //Frustum.InFrustumCheck nodeInFrustum = frustum.CubeInFrustum(mainNode.BoundingBox3D);
            //if (((nodeInFrustum != Frustum.InFrustumCheck.OUT) && (mainNode.HasChilds())))
            //{
            //    bool checkFrustum = true;
            //    // if all boundingbox corner where inside the frustum, there is no need to check the childs too
            //    if (nodeInFrustum == Frustum.InFrustumCheck.IN) { checkFrustum = false; }


            //    GL.Enable(EnableCap.Lighting);
            //    GL.Color4(1.0, 1.0, 1.0, 1.0);

            //    GL.EnableClientState(ArrayCap.VertexArray);
            //    GL.EnableClientState(ArrayCap.TextureCoordArray);



            //    CheckNodeInsideFrustum(ref frustum, ref mainNode, checkFrustum);

            //    if (terrainRef.TextureID != -1)
            //    {
            //        GL.ClientActiveTexture(TextureUnit.Texture0);

            //    }

            //    GL.DisableClientState(ArrayCap.TextureCoordArray);
            //    GL.DisableClientState(ArrayCap.VertexArray);

            //}
            //return;
        }

        //private void CheckNodeInsideFrustum(/*ref Frustum frustum,*/ ref QuadTreeNode mainNode, bool checkFrustum = true)
        //{
        //    throw new NotImplementedException("Frustum Check");
        //    //Frustum.InFrustumCheck nodeInFrustum;
        //    //bool checkChildFrustum = true;
        //    //// if the node has childs, check if nodes childs are in or outside the frustum
        //    //if (mainNode.HasChilds())
        //    //{
        //    //    for (int i = 1; i <= 4; i++)
        //    //    {
        //    //        QuadTreeNode node = mainNode.GetChild(i);
        //    //        if (checkFrustum)
        //    //        {
        //    //            if (node.HasChilds())
        //    //            {
        //    //                nodeInFrustum = frustum.CubeInFrustum(node.BoundingBox3D);
        //    //            }
        //    //            else
        //    //            {
        //    //                nodeInFrustum = frustum.CubeInFrustum(node.Patch);
        //    //            }
        //    //            if (nodeInFrustum == Frustum.InFrustumCheck.OUT) { continue; }
        //    //            if (nodeInFrustum == Frustum.InFrustumCheck.IN) { checkChildFrustum = false; }
        //    //        }
        //    //        CheckNodeInsideFrustum(ref frustum, ref node, checkChildFrustum);
        //    //    }
        //    //}
        //    //// if it don't, it's a leaf and the terrain patch can be rendered
        //    //else
        //    {
        //        int maxLODLevels = terrainRef.LODLEVELS;
        //        int patchResolution = mainNode.Patch.GetResolution(maxLODLevels);
        //        if ((mainNode.Patch.UpdateToResolution > -1) && (mainNode.Patch.UpdateToResolution != patchResolution)) patchResolution = mainNode.Patch.UpdateToResolution;
        //        int mainPatchIndexBuffer = terrainRef.IndexBuffer[patchResolution];

        //        List<int> defaultBridgeIndexBuffer = new List<int>();
        //        List<int> lowerBridgeIndexBuffer = new List<int>();

        //        if (mainNode.Neighbor_N != null)
        //        {
        //            // check north neighbor patch resolution
        //            int nRes = mainNode.Neighbor_N.Patch.GetResolution(maxLODLevels);
        //            if (nRes > patchResolution)
        //            {
        //                if (nRes > patchResolution * 2) mainNode.Neighbor_N.Patch.UpdateToResolution = patchResolution * 2;
        //                lowerBridgeIndexBuffer.Add(terrainRef.BridgeIndexBuffer[patchResolution, 4]);
        //            }
        //            else
        //            {
        //                mainNode.Neighbor_N.Patch.UpdateToResolution = -1;
        //                defaultBridgeIndexBuffer.Add(terrainRef.BridgeIndexBuffer[patchResolution, 0]);
        //            }
        //        }
        //        else
        //            defaultBridgeIndexBuffer.Add(terrainRef.BridgeIndexBuffer[patchResolution, 0]);
        //        if (mainNode.Neighbor_E != null)
        //        {
        //            // check east neighbor patch resolution
        //            int eRes = mainNode.Neighbor_E.Patch.GetResolution(maxLODLevels);
        //            if (eRes > patchResolution)
        //            {
        //                if (eRes > patchResolution * 2) mainNode.Neighbor_E.Patch.UpdateToResolution = patchResolution * 2;
        //                lowerBridgeIndexBuffer.Add(terrainRef.BridgeIndexBuffer[patchResolution, 5]);
        //            }
        //            else
        //            {
        //                mainNode.Neighbor_E.Patch.UpdateToResolution = -1;
        //                defaultBridgeIndexBuffer.Add(terrainRef.BridgeIndexBuffer[patchResolution, 1]);
        //            }
        //        }
        //        else
        //            defaultBridgeIndexBuffer.Add(terrainRef.BridgeIndexBuffer[patchResolution, 1]);
        //        if (mainNode.Neighbor_S != null)
        //        {
        //            // check south neighbor patch resolution
        //            int sRes = mainNode.Neighbor_S.Patch.GetResolution(maxLODLevels);
        //            if (sRes > patchResolution)
        //            {
        //                if (sRes > patchResolution * 2) mainNode.Neighbor_S.Patch.UpdateToResolution = patchResolution * 2;
        //                lowerBridgeIndexBuffer.Add(terrainRef.BridgeIndexBuffer[patchResolution, 6]);
        //            }
        //            else
        //            {
        //                mainNode.Neighbor_S.Patch.UpdateToResolution = -1;
        //                defaultBridgeIndexBuffer.Add(terrainRef.BridgeIndexBuffer[patchResolution, 2]);
        //            }
        //        }
        //        else
        //            defaultBridgeIndexBuffer.Add(terrainRef.BridgeIndexBuffer[patchResolution, 2]);
        //        if (mainNode.Neighbor_W != null)
        //        {
        //            // check west neighbor patch resolution
        //            int wRes = mainNode.Neighbor_W.Patch.GetResolution(maxLODLevels);
        //            if (wRes > patchResolution)
        //            {
        //                if (wRes > patchResolution * 2) mainNode.Neighbor_W.Patch.UpdateToResolution = patchResolution * 2;
        //                lowerBridgeIndexBuffer.Add(terrainRef.BridgeIndexBuffer[patchResolution, 7]);
        //            }
        //            else
        //            {
        //                mainNode.Neighbor_W.Patch.UpdateToResolution = -1;
        //                defaultBridgeIndexBuffer.Add(terrainRef.BridgeIndexBuffer[patchResolution, 3]);
        //            }
        //        }
        //        else
        //            defaultBridgeIndexBuffer.Add(terrainRef.BridgeIndexBuffer[patchResolution, 3]);

        //        int[] indicesCount = new int[3];
        //        indicesCount[0] = terrainRef.IndicesCount[patchResolution];                 // main patch indices count
        //        indicesCount[1] = terrainRef.BridgeIndicesCount[0, patchResolution];        // default bridge indices count
        //        indicesCount[2] = terrainRef.BridgeIndicesCount[1, patchResolution];        // lower bridge indices count

        //        mainNode.Patch.Render(mainPatchIndexBuffer, defaultBridgeIndexBuffer.ToArray(), lowerBridgeIndexBuffer.ToArray(), indicesCount, terrainRef.TextureID, terrainRef.DetailTextureID);
        //    }
        //    return;
        //}
    }
}
