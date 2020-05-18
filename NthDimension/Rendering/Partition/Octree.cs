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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NthDimension.Rendering.Culling;

namespace NthDimension.Rendering.Partition
{
    using NthDimension.Rendering.Drawables;
    public class Octree : OctreeLeaf
    {
        public Octree()
            : base(new BoundingAABB())
        {
        }

        public void Bounds()
        {
            foreach (OctreeObject item in ContainedObjects)
                Container = BoundingAABB.CreateMerged(Container, item.OctreeBounds);
        }

        public virtual void Add(IEnumerable<OctreeObject> items)
        {
            foreach (OctreeObject item in items)
                ContainedObjects.Add(item);

            Bounds();
            base.Distribute(0);
        }

        public virtual void Add(OctreeObject item)
        {
            ContainedObjects.Add(item);

            Bounds();
            base.Distribute(0);
        }

        
        public override void ObjectsInFrustum(List<OctreeObject> objects, BoundingFrustum boundingFrustum)
        {
            bool useTree = true;
            if (useTree)
                base.ObjectsInFrustum(objects, boundingFrustum);
            else // brute force to see if our box in frustum works
                AddInFrustum(objects, boundingFrustum, this);
        }

        protected void AddInFrustum(List<OctreeObject> objects, BoundingFrustum boundingFrustum, OctreeLeaf leaf)
        {
            foreach (OctreeObject item in leaf.ContainedObjects)
            {
                if (boundingFrustum.Intersects(item.OctreeBounds))
                    objects.Add(item);
            }

            if (leaf.Children != null)
            {
                foreach (OctreeLeaf child in leaf.Children)
                    AddInFrustum(objects, boundingFrustum, child);
            }
        }

        public override void ObjectsInBoundingAABB(List<OctreeObject> objects, BoundingAABB box)
        {
            base.ObjectsInBoundingAABB(objects, box);
        }

        public override void ObjectsInBoundingSphere(List<OctreeObject> objects, BoundingSphere sphere)
        {
            base.ObjectsInBoundingSphere(objects, sphere);
        }
    }
}
