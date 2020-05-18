﻿/* LICENSE
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

namespace NthDimension.Rendering.Culling
{
    using System.Collections.Generic;
    using NthDimension.Rendering.Partition;
    using NthDimension.Algebra;

    public class BoundingOctreeObject : OctreeObject
    {
        public string               Name            = string.Empty;
        public Boundary             Boundary        = new Boundary();
        public List<string>         Attributes      = new List<string>();
        public bool                 SkipTree        = false;
        public object               Tag             = null;
        // Mesh
    }

    public class CopyOfBoundingOctreeObject : OctreeObject
    {
        public string Name = string.Empty;
        public Boundary Boundary = new Boundary();
        public List<string> Attributes = new List<string>();
        public bool SkipTree = false;
        public object Tag = null;
        // Mesh
    }
}
