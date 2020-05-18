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

namespace NthDimension.Rendering.Geometry.Simplification
{
    /// <summary>
    /// Represents a vertex pair
    /// </summary>
    /// <remarks>
    /// Pair instances are sortable, with ascending order according to cost, that is, The pair with the lowest cost is at the front.
    /// </remarks>
    public class Pair : IComparable<Pair>
    {
        /// <summary>
        /// The first Vertex in the pair
        /// </summary>
        public int                  Vertex1;
        /// <summary>
        /// The second Vertex in the pair
        /// </summary>
        public int                  Vertex2;
        /// <summary>
        /// The minimal cost of the contraction of the pair
        /// </summary>
        public double               Cost;
        /// <summary>
        /// The optimal position for the contraction of the pair
        /// </summary>
        public Vector3d              Target;


        #region Ctor
        public Pair(int v1, int v2, Vector3d target, double cost)
        {
            Vertex1 = v1;
            Vertex2 = v2;
            Target = target;
            Cost = cost;
        }
        #endregion

        #region ToString
        public override string ToString()
        {
            return string.Format("({0},{1})", Vertex1, Vertex2);
        }
        #endregion

        /// <summary>
        /// Compares this instance to the specified Pair instance.
        /// </summary>
        /// <param name="other">
        /// The Pair instance to compare this instance to.
        /// </param>
        /// <returns>
        /// 1 if this instance is greater than the other instance, or -1 if this instance
        /// is less than the other instance, or 0 if both instances are equal.
        /// </returns>
        public int CompareTo(Pair other)
        {
            if (Cost > other.Cost)
                return 1;
            if (Cost < other.Cost)
                return -1;
            return 0;
        }

    }
}
