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
    using System.Collections.Generic;
    using NthDimension.Algebra;
    using NthDimension.Rendering.Geometry;

    [Serializable]
    public struct BoundingAABB : IEquatable<BoundingAABB>//, IRenderProvider
    {
        #region Properties
        public static BoundingAABB Empty = new BoundingAABB(new Vector3(0, 0, 0), new Vector3(0, 0, 0));

        public Vector3 Center;

        public Vector3 Min;
        public Vector3 Max;
        public Vector3 Extends;

        public Vector3 Size {
            get { return Extends*2; }
            set { Extends = value*0.5f; }
        }
        #endregion

        

        #region Ctor
        public BoundingAABB(Vector3 min, Vector3 max)
        {
            this.Min = min;
            this.Max = max;
            Extends = (max - min) * 0.5f;
            this.Center = min + Extends;
        }
        public BoundingAABB(float min = float.PositiveInfinity, float max = float.NegativeInfinity) : this(new Vector3(min), new Vector3(max))
        { }
        public BoundingAABB(Vector3 center, float size)
        {
            this.Center = center;
            this.Extends = new Vector3(size)*2;
            this.Min = this.Center - this.Extends;
            this.Max = this.Center + this.Extends;

        }
        #endregion

        #region Create
        public static BoundingAABB CreateFromPoints(IEnumerable<Vector3> points)
        {
            if (points == null)
                throw new ArgumentNullException();

            // TODO: Just check that Count > 0
            bool empty = true;
            Vector3 vector2 = Vector3.Zero;
            Vector3 vector1 = Vector3.Zero;
            foreach (Vector3 vector3 in points)
            {
                if (vector2 == Vector3.Zero)
                {
                    vector2 = new Vector3(vector3);
                    vector1 = new Vector3(vector3);
                }
                else
                {
                    vector2 = Vector3.Min(vector2, vector3);
                    vector1 = Vector3.Max(vector1, vector3);

                }
                empty = false;
            }
            if (empty)
                throw new ArgumentException();

            return new BoundingAABB(vector2, vector1);
        }
        public static BoundingAABB CreateFromPoints(Vector3[] points)
        {
            if (points == null)
                throw new ArgumentNullException();

            // TODO: Just check that Count > 0
            bool empty = true;
            Vector3 vector2 = Vector3.Zero;
            Vector3 vector1 = Vector3.Zero;
            foreach (Vector3 vector3 in points)
            {
                if (vector2 == Vector3.Zero)
                {
                    vector2 = new Vector3(vector3);
                    vector1 = new Vector3(vector3);
                }
                else
                {
                    vector2 = Vector3.Min(vector2, vector3);
                    vector1 = Vector3.Max(vector1, vector3);

                }
                empty = false;
            }
            if (empty)
                throw new ArgumentException();

            return new BoundingAABB(vector2, vector1);
        }
        public static BoundingAABB CreateFromSphere(BoundingSphere boundingSphere)
        {
            Vector3 vector1 = new Vector3(boundingSphere.Radius, boundingSphere.Radius, boundingSphere.Radius);
            return new BoundingAABB(boundingSphere.Center - vector1, boundingSphere.Center + vector1);
        }
        public static void CreateFromSphere(ref BoundingSphere boundingSphere, out BoundingAABB result)
        {
            result = BoundingAABB.CreateFromSphere(boundingSphere);
        }
        public static BoundingAABB CreateFromCylinderXY(BoundingCylinderXY cylinder)
        {
            Vector3 vector1 = new Vector3(cylinder.Radius, cylinder.Radius, cylinder.Radius);
            BoundingAABB box = new BoundingAABB(new Vector3(cylinder.Center) - vector1, new Vector3(cylinder.Center) + vector1);
            box.Max.Z = cylinder.MaxZ;
            box.Min.Z = cylinder.MinZ;
            return box;
        }
        public static void CreateFromCylinderXY(ref BoundingCylinderXY cylinder, out BoundingAABB result)
        {
            result = BoundingAABB.CreateFromCylinderXY(cylinder);
        }
        public static BoundingAABB CreateMerged(BoundingAABB original, BoundingAABB additional)
        {
            return new BoundingAABB(
                Vector3.Min(original.Min, additional.Min), Vector3.Max(original.Max, additional.Max));
        }
        public static void CreateMerged(ref BoundingAABB original, ref BoundingAABB additional, out BoundingAABB result)
        {
            result = BoundingAABB.CreateMerged(original, additional);
        }

        // SSM
        public static BoundingAABB FromSphere(Vector3 pos, float radius) // SSM - Use CreateFromSphere
        {
            BoundingAABB box = new BoundingAABB();
            box.Min.X = pos.X - radius;
            box.Max.X = pos.X + radius;
            box.Min.Y = pos.Y - radius;
            box.Max.Y = pos.Y + radius;
            box.Min.Z = pos.Z - radius;
            box.Max.Z = pos.Z + radius;

            return box;
        }
        //SSM
        public static BoundingAABB CreateFromFrustum(ref Matrix4 axisTransform, ref Matrix4 modelViewProj)
        {
            BoundingAABB ret = new BoundingAABB(float.PositiveInfinity, float.NegativeInfinity);
            Matrix4 inverse = modelViewProj;
            inverse.Invert();
            for (int i = 0; i < c_homogenousCorners.Length; ++i)
            {
                Vector4 corner = Vector4.Transform(c_homogenousCorners[i], inverse);
                Vector3 transfPt = Vector3.Transform(corner.Xyz / corner.W, axisTransform);
                ret.UpdateMin(transfPt);
                ret.UpdateMax(transfPt);
            }
            return ret;
        }
        // SSM - Used only in FromFrustum call
        private static readonly Vector4[] c_homogenousCorners = {
            new Vector4(-1f, -1f, -1f, 1f),
            new Vector4(-1f, 1f, -1f, 1f),
            new Vector4(1f, 1f, -1f, 1f),
            new Vector4(1f, -1f, -1f, 1f),

            new Vector4(-1f, -1f, 1f, 1f),
            new Vector4(-1f, 1f, 1f, 1f),
            new Vector4(1f, 1f, 1f, 1f),
            new Vector4(1f, -1f, 1f, 1f),
        };
        #endregion

        #region Contains
        public enuContainmentType Contains(BoundingAABB aabb)
        {
            //test if all corner is in the same side of a face by just checking min and max
            if (aabb.Max.X < Min.X
                || aabb.Min.X > Max.X
                || aabb.Max.Y < Min.Y
                || aabb.Min.Y > Max.Y
                || aabb.Max.Z < Min.Z
                || aabb.Min.Z > Max.Z)
                return enuContainmentType.Disjoint;

            if (aabb.Min.X >= Min.X
                && aabb.Max.X <= Max.X
                && aabb.Min.Y >= Min.Y
                && aabb.Max.Y <= Max.Y
                && aabb.Min.Z >= Min.Z
                && aabb.Max.Z <= Max.Z)
                return enuContainmentType.Contains;


            return enuContainmentType.Intersects;
        }
        public void Contains(ref BoundingAABB aabb, out enuContainmentType result)
        {
            result = Contains(aabb);
        }
        public enuContainmentType Contains(BoundingFrustum frustum)
        {
            //TODO: bad done here need a fix. 
            //Because question is not frustum contain box but reverse and this is not the same
            int i;
            enuContainmentType contained;
            Vector3[] corners = frustum.GetCorners();


            // First we check if frustum is in box
            for (i = 0; i < corners.Length; i++)
            {
                this.Contains(ref corners[i], out contained);
                if (contained == enuContainmentType.Disjoint)
                    break;
            }


            if (i == corners.Length) // This means we checked all the corners and they were all contain or instersect
                return enuContainmentType.Contains;


            if (i != 0)             // if i is not equal to zero, we can fastpath and say that this box intersects
                return enuContainmentType.Intersects;

            // If we get here, it means the first (and only) point we checked was actually contained in the frustum.
            // So we assume that all other points will also be contained. If one of the points is disjoint, we can
            // exit immediately saying that the result is Intersects
            i++;
            for (; i < corners.Length; i++)
            {
                this.Contains(ref corners[i], out contained);
                if (contained != enuContainmentType.Contains)
                    return enuContainmentType.Intersects;


            }


            // If we get here, then we know all the points were actually contained, therefore result is Contains
            return enuContainmentType.Contains;
        }
        public enuContainmentType Contains(BoundingSphere boundingSphere)
        {
            if (boundingSphere.Center.X - Min.X > boundingSphere.Radius
                && boundingSphere.Center.Y - Min.Y > boundingSphere.Radius
                && boundingSphere.Center.Z - Min.Z > boundingSphere.Radius
                && Max.X - boundingSphere.Center.X > boundingSphere.Radius
                && Max.Y - boundingSphere.Center.Y > boundingSphere.Radius
                && Max.Z - boundingSphere.Center.Z > boundingSphere.Radius)
                return enuContainmentType.Contains;


            double dmin = 0;


            if (boundingSphere.Center.X - Min.X <= boundingSphere.Radius)
                dmin += (boundingSphere.Center.X - Min.X) * (boundingSphere.Center.X - Min.X);
            else if (Max.X - boundingSphere.Center.X <= boundingSphere.Radius)
                dmin += (boundingSphere.Center.X - Max.X) * (boundingSphere.Center.X - Max.X);
            if (boundingSphere.Center.Y - Min.Y <= boundingSphere.Radius)
                dmin += (boundingSphere.Center.Y - Min.Y) * (boundingSphere.Center.Y - Min.Y);
            else if (Max.Y - boundingSphere.Center.Y <= boundingSphere.Radius)
                dmin += (boundingSphere.Center.Y - Max.Y) * (boundingSphere.Center.Y - Max.Y);
            if (boundingSphere.Center.Z - Min.Z <= boundingSphere.Radius)
                dmin += (boundingSphere.Center.Z - Min.Z) * (boundingSphere.Center.Z - Min.Z);
            else if (Max.Z - boundingSphere.Center.Z <= boundingSphere.Radius)
                dmin += (boundingSphere.Center.Z - Max.Z) * (boundingSphere.Center.Z - Max.Z);


            if (dmin <= boundingSphere.Radius * boundingSphere.Radius)
                return enuContainmentType.Intersects;


            return enuContainmentType.Disjoint;
        }
        public void Contains(ref BoundingSphere boundingSphere, out enuContainmentType result)
        {
            result = this.Contains(boundingSphere);
        }
        public enuContainmentType Contains(Vector3 point)
        {
            enuContainmentType result;
            this.Contains(ref point, out result);
            return result;
        }
        public void Contains(ref Vector3 point, out enuContainmentType result)
        {
            //first we get if point is out of box
            if (point.X < this.Min.X
                || point.X > this.Max.X
                || point.Y < this.Min.Y
                || point.Y > this.Max.Y
                || point.Z < this.Min.Z
                || point.Z > this.Max.Z)
            {
                result = enuContainmentType.Disjoint;
            }//or if point is on box because coordonate of point is lesser or equal
            else if (point.X == this.Min.X
                || point.X == this.Max.X
                || point.Y == this.Min.Y
                || point.Y == this.Max.Y
                || point.Z == this.Min.Z
                || point.Z == this.Max.Z)
                result = enuContainmentType.Intersects;
            else
                result = enuContainmentType.Contains;
        }
        #endregion Contains

        #region Intersects
        public bool Intersects(BoundingAABB aabb)
        {
            bool result;
            Intersects(ref aabb, out result);
            return result;
        }
        public void Intersects(ref BoundingAABB aabb, out bool result)
        {
            if ((this.Max.X >= aabb.Min.X) && (this.Min.X <= aabb.Max.X))
            {
                if ((this.Max.Y < aabb.Min.Y) || (this.Min.Y > aabb.Max.Y))
                {
                    result = false;
                    return;
                }


                result = (this.Max.Z >= aabb.Min.Z) && (this.Min.Z <= aabb.Max.Z);
                return;
            }


            result = false;
            return;
        }
        public bool Intersects(BoundingFrustum frustum)
        {
            return frustum.Contains(this) != enuContainmentType.Disjoint;
        }
        public bool Intersects(BoundingSphere boundingSphere)
        {
            if (boundingSphere.Center.X - Min.X > boundingSphere.Radius
                && boundingSphere.Center.Y - Min.Y > boundingSphere.Radius
                && boundingSphere.Center.Z - Min.Z > boundingSphere.Radius
                && Max.X - boundingSphere.Center.X > boundingSphere.Radius
                && Max.Y - boundingSphere.Center.Y > boundingSphere.Radius
                && Max.Z - boundingSphere.Center.Z > boundingSphere.Radius)
                return true;

            double dmin = 0;

            if (boundingSphere.Center.X - Min.X <= boundingSphere.Radius)
                dmin += (boundingSphere.Center.X - Min.X) * (boundingSphere.Center.X - Min.X);
            else if (Max.X - boundingSphere.Center.X <= boundingSphere.Radius)
                dmin += (boundingSphere.Center.X - Max.X) * (boundingSphere.Center.X - Max.X);


            if (boundingSphere.Center.Y - Min.Y <= boundingSphere.Radius)
                dmin += (boundingSphere.Center.Y - Min.Y) * (boundingSphere.Center.Y - Min.Y);
            else if (Max.Y - boundingSphere.Center.Y <= boundingSphere.Radius)
                dmin += (boundingSphere.Center.Y - Max.Y) * (boundingSphere.Center.Y - Max.Y);


            if (boundingSphere.Center.Z - Min.Z <= boundingSphere.Radius)
                dmin += (boundingSphere.Center.Z - Min.Z) * (boundingSphere.Center.Z - Min.Z);
            else if (Max.Z - boundingSphere.Center.Z <= boundingSphere.Radius)
                dmin += (boundingSphere.Center.Z - Max.Z) * (boundingSphere.Center.Z - Max.Z);

            if (dmin <= boundingSphere.Radius * boundingSphere.Radius)
                return true;

            return false;
        }
        public void Intersects(ref BoundingSphere boundingSphere, out bool result)
        {
            result = Intersects(boundingSphere);
        }
        public enuPlaneIntersection Intersects(Plane plane)
        {
            //check all corner side of plane
            Vector3[] corners = this.GetCorners();
            float lastdistance = Vector3.Dot(plane.Normal, corners[0]) + plane.D;

            for (int i = 1; i < corners.Length; i++)
            {
                float distance = Vector3.Dot(plane.Normal, corners[i]) + plane.D;
                if ((distance <= 0.0f && lastdistance > 0.0f) || (distance >= 0.0f && lastdistance < 0.0f))
                    return enuPlaneIntersection.Intersecting;
                lastdistance = distance;
            }

            if (lastdistance > 0.0f)
                return enuPlaneIntersection.Front;

            return enuPlaneIntersection.Back;
        }
        public void Intersects(ref Plane plane, out enuPlaneIntersection result)
        {
            result = Intersects(plane);
        }
        public Nullable<float> Intersects(Ray ray)
        {
            return ray.Intersects(this);
        }
        public void Intersects(ref Ray ray, out Nullable<float> result)
        {
            result = Intersects(ray);
        }

        // SSM
        public bool IntersectsSphere(Vector3 origin, float radius)
        {
            if (
                (origin.X + radius < Min.X) ||
                (origin.Y + radius < Min.Y) ||
                (origin.Z + radius < Min.Z) ||
                (origin.X - radius > Max.X) ||
                (origin.Y - radius > Max.Y) ||
                (origin.Z - radius > Max.Z)
               )
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region General Methods
        public bool Equals(BoundingAABB other)
        {
            return (this.Min == other.Min) && (this.Max == other.Max);
        }
        public override bool Equals(object obj)
        {
            return (obj is BoundingAABB) ? this.Equals((BoundingAABB)obj) : false;
        }
        public Vector3[] GetCorners()
        {
            return new Vector3[] {
                new Vector3(this.Min.X, this.Max.Y, this.Max.Z),
                new Vector3(this.Max.X, this.Max.Y, this.Max.Z),
                new Vector3(this.Max.X, this.Min.Y, this.Max.Z),
                new Vector3(this.Min.X, this.Min.Y, this.Max.Z),
                new Vector3(this.Min.X, this.Max.Y, this.Min.Z),
                new Vector3(this.Max.X, this.Max.Y, this.Min.Z),
                new Vector3(this.Max.X, this.Min.Y, this.Min.Z),
                new Vector3(this.Min.X, this.Min.Y, this.Min.Z)
            };
        }
        public override int GetHashCode()
        {
            return this.Min.GetHashCode() + this.Max.GetHashCode();
        }
        public override string ToString()
        {
            return string.Format("{{Min:{0} Max:{1}}}", this.Min.ToString(), this.Max.ToString());
        }
        #endregion Public Methods

        #region Operators
        public static bool operator ==(BoundingAABB a, BoundingAABB b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(BoundingAABB a, BoundingAABB b)
        {
            return !a.Equals(b);
        }
        #endregion

        #region IRenderProvider Implementation
        public void Render()
        {
            throw new NotImplementedException();
        }
        #endregion


        public void Combine(ref BoundingAABB other) // SSM
        {
            Min = Vector3.ComponentMin(Min, other.Min);
            Max = Vector3.ComponentMax(Max, other.Max);
        }
        public void UpdateMin(Vector3 localMin) // SSM
        {
            Min = Vector3.ComponentMin(Min, localMin);
        }
        public void UpdateMax(Vector3 localMax) // SSM
        {
            Max = Vector3.ComponentMax(Max, localMax);
        }
        
        public Vector3 Diff() // SSM
        {
            return Max - Min;
        }
        public BoundingSphere ToSphere() // SSM
        {
            float r = (Diff().LengthFast + 0.001f) / 2f;
            return new BoundingSphere(Center, r);
        }

        internal void ExpandToFit(BoundingAABB b) // SSM
        {
            if (b.Min.X < this.Min.X) { this.Min.X = b.Min.X; }
            if (b.Min.Y < this.Min.Y) { this.Min.Y = b.Min.Y; }
            if (b.Min.Z < this.Min.Z) { this.Min.Z = b.Min.Z; }

            if (b.Max.X > this.Max.X) { this.Max.X = b.Max.X; }
            if (b.Max.Y > this.Max.Y) { this.Max.Y = b.Max.Y; }
            if (b.Max.Z > this.Max.Z) { this.Max.Z = b.Max.Z; }
        }
        public BoundingAABB ExpandedBy(BoundingAABB b) // SSM
        {
            BoundingAABB newbox = this;
            if (b.Min.X < newbox.Min.X) { newbox.Min.X = b.Min.X; }
            if (b.Min.Y < newbox.Min.Y) { newbox.Min.Y = b.Min.Y; }
            if (b.Min.Z < newbox.Min.Z) { newbox.Min.Z = b.Min.Z; }

            if (b.Max.X > newbox.Max.X) { newbox.Max.X = b.Max.X; }
            if (b.Max.Y > newbox.Max.Y) { newbox.Max.Y = b.Max.Y; }
            if (b.Max.Z > newbox.Max.Z) { newbox.Max.Z = b.Max.Z; }

            return newbox;
        }
        public void ExpandBy(BoundingAABB b) // SSM
        {
            this = this.ExpandedBy(b);
        }
    }
}
