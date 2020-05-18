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

using System.Collections.Generic;
using NthDimension.Rendering.Culling;
using NthDimension.Rendering.Drawables;
namespace NthDimension.Rendering.Partition
{
    //public class OctreeObject : OctreeObject
    //{
    //}

    public class OctreeWorld : Octree
    {
        public List<OctreeObject> Objects = new List<OctreeObject>();

        public new void Add(OctreeObject item)
        {
            Objects.Add(item);
        }
        public void Remove(OctreeObject item)
        {
            Objects.Remove(item);
        }

        public void BuildTree(BoundingAABB initalBounds)
        {
            foreach (OctreeObject item in Objects)
                ContainedObjects.Add(item as OctreeObject);

            Container = new BoundingAABB(initalBounds.Min, initalBounds.Max);
            base.Distribute(0);
        }

        public void BuildTree()
        {
            foreach (OctreeObject item in Objects)
                ContainedObjects.Add(item as OctreeObject);

            Bounds();
            base.Distribute(0);
        }

        public void ObjectsInFrustum(List<OctreeObject> objects, BoundingFrustum boundingFrustum, bool exact)
        {
            base.ObjectsInFrustum(objects, boundingFrustum);
            if (exact)
            {
                for (int i = objects.Count - 1; i >= 0; i--)
                {
                    if (!boundingFrustum.Intersects(objects[i].OctreeBounds))
                        objects.RemoveAt(i);
                }
            }
        }

        public void ObjectsInBoundingBox(List<OctreeObject> objects, BoundingAABB boundingbox, bool exact)
        {
            base.ObjectsInBoundingAABB(objects, boundingbox);
            if (exact)
            {
                for (int i = objects.Count - 1; i >= 0; i--)
                {
                    if (!boundingbox.Intersects(objects[i].OctreeBounds))
                        objects.RemoveAt(i);
                }
            }
        }
    }
}
