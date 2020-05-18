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

namespace NthDimension.Algebra
{
    using System;
    /// <summary>
    /// A pair of values that belong to an equal type, mostly used for coordinates. Kind of a more
    /// flexible "Vector Light" used for simple auxParam-pairs.
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    public struct Point<T>
    {
        /// <summary>
        /// X-Coordinate
        /// </summary>
        public T x;
        /// <summary>
        /// Y-Coordinate
        /// </summary>
        public T y;


        /// <summary>
        /// 
        /// </summary>
        public T X
        {
            get { return this.x; }
            set { this.x = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public T Y
        {
            get { return this.y; }
            set { this.y = value; }
        }


        /// <summary>
        /// A Points optional constructor.
        /// </summary>
        /// <param name="x">X-Coordinate</param>
        /// <param name="y">Y-Coordinate</param>
        public Point(T x, T y)
        {
            this.x = x;
            this.y = y;
        }


        /// <summary>
        /// Operator overload: p1 == p2
        /// </summary>
        /// <param name="p1">Point</param>
        /// <param name="p2">Point</param>
        /// <returns>p1 == p2</returns>
        public static bool operator ==(Point<T> p1, Point<T> p2)
        {
            return (
                p1.x.Equals(p2.x) &&
                p1.y.Equals(p2.y));
        }


        /// <summary>
        /// Operator overload: p1 != p2
        /// </summary>
        /// <param name="p1">Point</param>
        /// <param name="p2">Point</param>
        /// <returns>p1 != p2</returns>
        public static bool operator !=(Point<T> p1, Point<T> p2)
        {
            return (
                !p1.x.Equals(p2.x) ||
                !p1.y.Equals(p2.y));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is Point<T>)
                return (this == (Point<T>)obj);
            else
                return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (this.x.GetHashCode() + this.y.GetHashCode());
        }


        /// <summary>
        /// Generates a string giving information about the Points position.
        /// </summary>
        /// <returns>The generated string auxParam. Structure: "[X, Y]"</returns>
        public override string ToString()
        {
            return (String.Format("[{0}, {1}]", x, y));
        }
    }
}
