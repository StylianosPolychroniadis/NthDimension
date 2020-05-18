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
    /// Four values of equal type, mostly used to describe a rect - or similar.
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    public struct Rect<T>
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
        /// Width
        /// </summary>
        public T w;
        /// <summary>
        /// Height
        /// </summary>
        public T h;


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
        /// 
        /// </summary>
        public T W
        {
            get { return this.w; }
            set { this.w = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public T H
        {
            get { return this.h; }
            set { this.h = value; }
        }


        /// <summary>
        /// A Rects optional constructor.
        /// </summary>
        /// <param name="x">X-Coordinate</param>
        /// <param name="y">Y-Coordinate</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        public Rect(T x, T y, T w, T h)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
        }


        /// <summary>
        /// Operator overload: r1 == r2
        /// </summary>
        /// <param name="r1">Rect</param>
        /// <param name="r2">Rect</param>
        /// <returns>r1 == r2</returns>
        public static bool operator ==(Rect<T> r1, Rect<T> r2)
        {
            return (
                r1.x.Equals(r2.x) &&
                r1.y.Equals(r2.y) &&
                r1.w.Equals(r2.w) &&
                r1.h.Equals(r2.h));
        }


        /// <summary>
        /// Operator overload: r1 != r2
        /// </summary>
        /// <param name="r1">Rect</param>
        /// <param name="r2">Rect</param>
        /// <returns>r1 != r2</returns>
        public static bool operator !=(Rect<T> r1, Rect<T> r2)
        {
            return (
                !r1.x.Equals(r2.x) ||
                !r1.y.Equals(r2.y) ||
                !r1.w.Equals(r2.w) ||
                !r1.h.Equals(r2.h));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is Rect<T>)
                return (this == (Rect<T>)obj);
            else
                return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (this.x.GetHashCode() + this.y.GetHashCode() + this.w.GetHashCode() + this.h.GetHashCode());
        }


        /// <summary>
        /// Generates a string giving information about the Rects position, width and height.
        /// </summary>
        /// <returns>The generated string auxParam. Structure: "[X, Y, W, H]"</returns>
        public override string ToString()
        {
            return (String.Format("[{0}, {1}, {2}, {3}]", x, y, w, h));
        }
    }
}
