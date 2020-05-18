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
    using NthDimension.Algebra;

    using NthDimension.Rendering.Drawables.Models;   // TODO:: Refactor <Generic Object> Reference

    public class QuadTreeNode
    {
        public int ID;

        // the 4 childs
        public QuadTreeNode NW = null;
        public QuadTreeNode NE = null;
        public QuadTreeNode SW = null;
        public QuadTreeNode SE = null;

        // Refs to the adjacent nodes
        public QuadTreeNode Neighbor_N = null;
        public QuadTreeNode Neighbor_E = null;
        public QuadTreeNode Neighbor_S = null;
        public QuadTreeNode Neighbor_W = null;

        // used to find all neighbours, even if they are childs to an other node
        public QuadTreeNode Parent = null;

        private int level;
        public int Level
        { get { return level; } }
        // 2D Bounding Box vertices for quadtree generation
        private Vector3[] boundingV2D;
        public Vector3[] BoundingBox2D
        { get { return boundingV2D; } }
        public Vector3[] BoundingBox3D;
        /*private Vector3 centerV;
        public Vector3 CenterVertex
        { get { return centerV; } }*/

        // patch assigned to node, if node is a leaf
        //public PlanetModel.Terrain.TerrainPatch Patch;      // TODO:: Refactor <Generic Object> Reference

        public QuadTreeNode(int nodeLevel, Vector3 topLeftBBV, Vector3 topRightBBV, Vector3 bottomLeftBBV, Vector3 bottomRightBBV)
        {
            level = nodeLevel;
            boundingV2D = new Vector3[4] { topLeftBBV, topRightBBV, bottomLeftBBV, bottomRightBBV };
            BoundingBox3D = new Vector3[8];
        }

        // used to get the children at runtime
        public QuadTreeNode GetChild(int nodeDirection)
        {
            if ((nodeDirection < 1) && (nodeDirection > 4)) return null;
            if (nodeDirection == 1) return NW;
            if (nodeDirection == 2) return NE;
            if (nodeDirection == 3) return SW;
            return SE;
        }

        public bool HasChilds()
        {
            if (NW != null) return true;
            if (NE != null) return true;
            if (SW != null) return true;
            if (SE != null) return true;
            return false;
        }

        public virtual void Destroy()
        {
            
        }
    }
}
