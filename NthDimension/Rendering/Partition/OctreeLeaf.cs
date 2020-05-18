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
    using System.Collections.Generic;
    using NthDimension.Algebra;
    using NthDimension.Rendering.Culling;
    using NthDimension.Rendering.Drawables;

    public class OctreeLeaf
    {
        #region Properties
        public List<OctreeLeaf>                     Children
        {
            get { return children; }
            set { children = value; }
        }
        public List<OctreeObject>                   ContainedObjects
        {
            get {  return containedObjects; }
            set { containedObjects = value; }
        }
        public BoundingAABB                         Container
        {
            get { return bounds; }
            set { bounds = value; }
        }

        #endregion

        private BoundingAABB                        bounds;
        private List<OctreeLeaf>                    children            = null;
        private List<OctreeObject>                  containedObjects    = new List<OctreeObject>();

        private int                                 maxObjects          = 8;

        private const int                           maxDepth            = 40;
        private const bool                          doFastOut           = true;

        #region Ctor
        public OctreeLeaf(BoundingAABB container)
        {
            bounds = container;
        }
        #endregion

        protected void AddChildren(List<OctreeObject> objects)
        {
            foreach (OctreeObject item in containedObjects)
                objects.Add(item);

            if (Children != null)
            {
                foreach (OctreeLeaf leaf in Children)
                    leaf.AddChildren(objects);
            }
        }
        protected void Distribute(int depth)
        {
            if (this.containedObjects.Count > maxObjects && depth <= maxDepth)
            {
                this.Split();

                for (int i = this.containedObjects.Count - 1; i >= 0; i--)
                    foreach (OctreeLeaf leaf in this.children)
                        if (leaf.Container.Contains(containedObjects[i].OctreeBounds) == enuContainmentType.Contains)
                        {
                            leaf.ContainedObjects.Add(containedObjects[i]);
                            containedObjects.Remove(containedObjects[i]);
                            break;
                        }

                depth++;
                foreach(OctreeLeaf leaf in this.children)
                    leaf.Distribute(depth);
                depth--;
            }
        }
        protected void Split()
        {
            if(null != this.children)
                return;

            Vector3 half = Container.Max - Container.Min;
            half *= 0.5f;
            Vector3 halfx = new Vector3(half.X, 0, 0);
            Vector3 halfy = new Vector3(0, half.Y, 0);
            Vector3 halfz = new Vector3(0, 0, half.Z);

            this.children = new List<OctreeLeaf>();

            this.children.Add(new OctreeLeaf(new BoundingAABB(Container.Min, Container.Min + half)));
            this.children.Add(new OctreeLeaf(new BoundingAABB(Container.Min + halfx, Container.Max - half + halfx)));
            this.children.Add(new OctreeLeaf(new BoundingAABB(Container.Min + halfz, Container.Min + half + halfz)));
            this.children.Add(new OctreeLeaf(new BoundingAABB(Container.Min + halfx + halfz, Container.Max - halfy)));
            this.children.Add(new OctreeLeaf(new BoundingAABB(Container.Min + halfy, Container.Max - halfx - halfz)));
            this.children.Add(new OctreeLeaf(new BoundingAABB(Container.Min + halfy + halfx, Container.Max - halfz)));
            this.children.Add(new OctreeLeaf(new BoundingAABB(Container.Min + halfy + halfz, Container.Max - halfx)));
            this.children.Add(new OctreeLeaf(new BoundingAABB(Container.Min + half, Container.Max)));
        }

        public virtual void ObjectsInFrustum(List<OctreeObject> objects, BoundingFrustum boundingFrustum)
        {
            // if the current box is totally contained in our leaf, then add me and all my kids
            if (doFastOut && boundingFrustum.Contains(Container) == enuContainmentType.Contains)
                AddChildren(objects);
            else
            {
                // ok so we know that we are probably intersecting or outside
                foreach (OctreeObject item in containedObjects) // add our straglers
                    objects.Add(item);

                if (Children != null)
                {
                    foreach (OctreeLeaf leaf in Children)
                    {
                        // if the child is totally in the volume then add it and it's kids
                        if (doFastOut && boundingFrustum.Contains(leaf.Container) == enuContainmentType.Contains)
                            leaf.AddChildren(objects);
                        else
                        {
                            if (boundingFrustum.Intersects(leaf.Container))
                                leaf.ObjectsInFrustum(objects, boundingFrustum);
                        }

                    }
                }
            }
        }

        public virtual void ObjectsInBoundingAABB(List<OctreeObject> objects, BoundingAABB boundingBox)
        {
            // if the current box is totally contained in our leaf, then add me and all my kids
            if (boundingBox.Contains(Container) == enuContainmentType.Contains)
                AddChildren(objects);
            else
            {
                // ok so we know that we are probably intersecting or outside
                foreach (OctreeObject item in containedObjects) // add our straglers
                    objects.Add(item);

                if (Children != null)
                {
                    foreach (OctreeLeaf leaf in Children)
                    {
                        // see if any of the sub boxes intesect our frustum
                        if (leaf.Container.Intersects(boundingBox))
                            leaf.ObjectsInBoundingAABB(objects, boundingBox);
                    }
                }
            }
        }

        public virtual void ObjectsInBoundingSphere(List<OctreeObject> objects, BoundingSphere boundingSphere)
        {
            // if the current box is totally contained in our leaf, then add me and all my kids
            if (boundingSphere.Contains(Container) == enuContainmentType.Contains)
                AddChildren(objects);
            else
            {
                // ok so we know that we are probably intersecting or outside
                foreach (OctreeObject item in containedObjects) // add our straglers
                    objects.Add(item);

                if (Children != null)
                {
                    foreach (OctreeLeaf leaf in Children)
                    {
                        // see if any of the sub boxes intesect our frustum
                        if (leaf.Container.Intersects(boundingSphere))
                            leaf.ObjectsInBoundingSphere(objects, boundingSphere);
                    }
                }
            }
        }

    }
}
