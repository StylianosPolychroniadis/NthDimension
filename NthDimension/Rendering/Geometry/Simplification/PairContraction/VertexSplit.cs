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
using NthDimension.Algebra;

namespace NthDimension.Rendering.Geometry.Simplification
{
    /// <summary>
    /// Represents a vertex split operation, ie the opposite of an edge
    /// Pair-contract operation
    /// </summary>
    public class VertexSplit
    {
        /// <summary>
        /// The index of the vertex, which is "split"
        /// </summary>
        public int              S;
        /// <summary>
        /// The new position of the vertex after splitting
        /// </summary>
        public Vector3d         SPosition;
        /// <summary>
        /// The position of the second vertex of the splitting occurs
        /// </summary>
        public Vector3d         TPosition;
        /// <summary>
        /// A subset of the facets of vertex S are to be assigned the vertex T after the splitting
        /// </summary>
        public ISet<Face>   Faces = new HashSet<Face>();
    }
}
