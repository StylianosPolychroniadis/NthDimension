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
using NthDimension.Algebra;
using NthDimension.Rendering.Culling;

namespace NthDimension.Rendering.Partition
{
    public class OctreeBox : OctreeObject
    {
        public Vector3 postion;
        public Vector3 size;
        public float rotation;
        public string name;


        public OctreeBox(BoundingAABB box, string meshName)
            : base()
        {
            this.name = meshName;
            size = box.Max - box.Min;
            size *= 0.5f;
            postion = box.Min + size;
            rotation = 0;
            OctreeBounds = new BoundingAABB(box.Min, box.Max);
        }


    }
}
