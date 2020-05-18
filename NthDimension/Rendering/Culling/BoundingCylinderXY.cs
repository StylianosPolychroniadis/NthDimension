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

namespace NthDimension.Rendering.Culling
{
    using System;
    using NthDimension.Algebra;

    public struct BoundingCylinderXY : IEquatable<BoundingCylinderXY>
    {
        #region Properties
        public Vector2                      Center;
        public float                        MaxZ;
        public float                        MinZ;
        public float                        Radius;

        public static BoundingCylinderXY    Empty           = new BoundingCylinderXY(new Vector3(0, 0, 0), 0, 0);
        #endregion

        #region Ctor
        public BoundingCylinderXY(Vector3 cp, float height, float radius)
        {
            this.Center.X = cp.X;
            this.Center.Y = cp.Y;

            this.Radius = radius;

            this.MinZ = cp.Z;
            this.MaxZ = cp.Z + height;
        }
        #endregion

        #region Equals // ToDo NearlyEqual avoid floating point precision errors
        public bool Equals(BoundingCylinderXY other)
        {
            return this.Center == other.Center && this.Radius == other.Radius && this.MinZ == other.MinZ && this.MaxZ == other.MaxZ;
        }
        public override bool Equals(object obj)
        {
            if (obj is BoundingCylinderXY)
                return this.Equals((BoundingCylinderXY)obj);

            return false;
        }
        public static bool operator ==(BoundingCylinderXY a, BoundingCylinderXY b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(BoundingCylinderXY a, BoundingCylinderXY b)
        {
            return !a.Equals(b);
        }
        #endregion Equals

        #region GetHashCode()
        public override int GetHashCode()
        {
            return this.Center.GetHashCode() + this.Radius.GetHashCode() + this.MaxZ.GetHashCode() + this.MinZ.GetHashCode();
        }
        #endregion GetHashCode()

        #region ToString()
        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.CurrentCulture, "{{Center:{0} Radius:{1} MinZ:{2} MaxZ:{3}}}", this.Center.ToString(), this.Radius.ToString(), this.MinZ.ToString(), this.MaxZ.ToString());
        }
        #endregion ToString()

        private bool pointInXY(float X, float Y)
        {
            float distSquare = (X - Center.X) * (X - Center.X) + (Y - Center.Y) * (Y - Center.Y);
            return distSquare <= Radius * Radius;
        }

        public enuContainmentType Contains(BoundingAABB box)
        {
            // above or below
            if (box.Min.Z > MaxZ || box.Max.Z < MinZ)
                return enuContainmentType.Disjoint;

            // for containment it MUST fit in Z
            if (MaxZ <= box.Max.Z && MinZ >= box.Min.Z)
            {
                if (!pointInXY(box.Max.X, box.Max.Y) || !pointInXY(box.Min.X, box.Max.Y) || !pointInXY(box.Min.X, box.Min.Y) || !pointInXY(box.Max.X, box.Min.Y))
                    return enuContainmentType.Intersects;

                return enuContainmentType.Contains;
            }

            if (!pointInXY(box.Max.X, box.Max.Y) || !pointInXY(box.Min.X, box.Max.Y) || !pointInXY(box.Min.X, box.Min.Y) || !pointInXY(box.Max.X, box.Min.Y))
                return enuContainmentType.Disjoint;

            return enuContainmentType.Intersects;
        }

    }
}
