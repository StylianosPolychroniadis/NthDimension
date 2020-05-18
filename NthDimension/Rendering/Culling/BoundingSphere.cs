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
    using System.Globalization;
    using System.Text;
    using NthDimension.Algebra;
    using NthDimension.Rasterizer;
    using NthDimension.Rendering.Geometry;

    public struct BoundingSphere : IEquatable<BoundingSphere>//, IRenderProvider
    {
        #region Public Fields


        public Vector3 Center;
        public float Radius;


        #endregion Public Fields

        #region Constructors

        public BoundingSphere(Vector3 center, float radius)
        {
            this.Center = center;
            this.Radius = radius;
        }

        #endregion Constructors

        #region Public Methods

        public static BoundingSphere Empty = new BoundingSphere(new Vector3(0, 0, 0), 0);

        public enuContainmentType Contains(BoundingAABB aabb)
        {
            //check if all corner is in sphere
            bool inside = true;
            foreach (Vector3 corner in aabb.GetCorners())
            {
                if (this.Contains(corner) == enuContainmentType.Disjoint)
                {
                    inside = false;
                    break;
                }
            }


            if (inside)
                return enuContainmentType.Contains;


            //check if the distance from sphere center to cube face < radius
            double dmin = 0;


            if (Center.X < aabb.Min.X)
                dmin += (Center.X - aabb.Min.X) * (Center.X - aabb.Min.X);


            else if (Center.X > aabb.Max.X)
                dmin += (Center.X - aabb.Max.X) * (Center.X - aabb.Max.X);


            if (Center.Y < aabb.Min.Y)
                dmin += (Center.Y - aabb.Min.Y) * (Center.Y - aabb.Min.Y);


            else if (Center.Y > aabb.Max.Y)
                dmin += (Center.Y - aabb.Max.Y) * (Center.Y - aabb.Max.Y);


            if (Center.Z < aabb.Min.Z)
                dmin += (Center.Z - aabb.Min.Z) * (Center.Z - aabb.Min.Z);


            else if (Center.Z > aabb.Max.Z)
                dmin += (Center.Z - aabb.Max.Z) * (Center.Z - aabb.Max.Z);


            if (dmin <= Radius * Radius)
                return enuContainmentType.Intersects;

            //else disjoint
            return enuContainmentType.Disjoint;


        }


        public void Contains(ref BoundingAABB aabb, out enuContainmentType result)
        {
            result = this.Contains(aabb);
        }


        public enuContainmentType Contains(BoundingFrustum frustum)
        {
            //check if all corner is in sphere
            bool inside = true;


            Vector3[] corners = frustum.GetCorners();
            foreach (Vector3 corner in corners)
            {
                if (this.Contains(corner) == enuContainmentType.Disjoint)
                {
                    inside = false;
                    break;
                }
            }
            if (inside)
                return enuContainmentType.Contains;


            //check if the distance from sphere center to frustrum face < radius
            double dmin = 0;
            //TODO : calcul dmin


            if (dmin <= Radius * Radius)
                return enuContainmentType.Intersects;


            //else disjoint
            return enuContainmentType.Disjoint;
        }


        public enuContainmentType Contains(BoundingSphere sphere)
        {
            float val = GenericMethods.Distance(sphere.Center, Center);


            if (val > sphere.Radius + Radius)
                return enuContainmentType.Disjoint;


            else if (val <= Radius - sphere.Radius)
                return enuContainmentType.Contains;


            else
                return enuContainmentType.Intersects;
        }


        public void Contains(ref BoundingSphere sphere, out enuContainmentType result)
        {
            result = Contains(sphere);
        }


        public enuContainmentType Contains(Vector3 point)
        {
            float distance = GenericMethods.Distance(point, Center);


            if (distance > this.Radius)
                return enuContainmentType.Disjoint;


            else if (distance < this.Radius)
                return enuContainmentType.Contains;


            return enuContainmentType.Intersects;
        }


        public void Contains(ref Vector3 point, out enuContainmentType result)
        {
            result = Contains(point);
        }


        public static BoundingSphere CreateFromBoundingBox(BoundingAABB aabb)
        {
            // Find the center of the box.
            Vector3 center = new Vector3((aabb.Min.X + aabb.Max.X) / 2.0f,
                                         (aabb.Min.Y + aabb.Max.Y) / 2.0f,
                                         (aabb.Min.Z + aabb.Max.Z) / 2.0f);


            // Find the distance between the center and one of the corners of the box.
            float radius = GenericMethods.Distance(center, aabb.Max);


            return new BoundingSphere(center, radius);
        }


        public static void CreateFromBoundingBox(ref BoundingAABB aabb, out BoundingSphere result)
        {
            result = CreateFromBoundingBox(aabb);
        }


        public static BoundingSphere CreateFromFrustum(BoundingFrustum frustum)
        {
            return BoundingSphere.CreateFromPoints(frustum.GetCorners());
        }


        public static BoundingSphere CreateFromPoints(IEnumerable<Vector3> points)
        {
            if (points == null)
                throw new ArgumentNullException("points");


            float radius = 0;
            Vector3 center = new Vector3();
            // First, we'll find the center of gravity for the point 'cloud'.
            int num_points = 0; // The number of points (there MUST be a better way to get this instead of counting the number of points one by one?)

            foreach (Vector3 v in points)
            {
                center += v;    // If we actually knew the number of points, we'd get better accuracy by adding v / num_points.
                ++num_points;
            }

            center /= (float)num_points;


            // Calculate the radius of the needed sphere (it equals the distance between the center and the point further away).
            foreach (Vector3 v in points)
            {
                float distance = ((Vector3)(v - center)).LengthFast;

                if (distance > radius)
                    radius = distance;
            }


            return new BoundingSphere(center, radius);
        }


        public static BoundingSphere CreateMerged(BoundingSphere original, BoundingSphere additional)
        {
            Vector3 ocenterToaCenter = GenericMethods.Subtract(additional.Center, original.Center);
            float distance = ocenterToaCenter.LengthFast;
            if (distance <= original.Radius + additional.Radius)//intersect
            {
                if (distance <= original.Radius - additional.Radius)//original contain additional
                    return original;
                if (distance <= additional.Radius - original.Radius)//additional contain original
                    return additional;
            }


            //else find center of new sphere and radius
            float leftRadius = System.Math.Max(original.Radius - distance, additional.Radius);
            float Rightradius = System.Math.Max(original.Radius + distance, additional.Radius);
            ocenterToaCenter = ocenterToaCenter + (((leftRadius - Rightradius) / (2 * ocenterToaCenter.LengthFast)) * ocenterToaCenter);//oCenterToResultCenter

            BoundingSphere result = new BoundingSphere();
            result.Center = original.Center + ocenterToaCenter;
            result.Radius = (leftRadius + Rightradius) / 2;
            return result;
        }


        public static void CreateMerged(ref BoundingSphere original, ref BoundingSphere additional, out BoundingSphere result)
        {
            result = BoundingSphere.CreateMerged(original, additional);
        }


        public float Distance(BoundingSphere sphere)
        {
            Vector3 dist = Center - sphere.Center;
            return dist.LengthFast - sphere.Radius - Radius;
        }


        public float Distance(Vector3 point)
        {
            Vector3 dist = Center - point;
            return dist.LengthFast - Radius;
        }


        public float DistanceSquared(BoundingSphere sphere)
        {
            Vector3 dist = Center - sphere.Center;
            return dist.LengthSquared - sphere.Radius * sphere.Radius - Radius * Radius;
        }


        public float DistanceSquared(Vector3 point)
        {
            Vector3 dist = Center - point;
            return dist.LengthSquared - Radius * Radius;
        }


        public bool Equals(BoundingSphere other)
        {
            return this.Center == other.Center && this.Radius == other.Radius;
        }


        public override bool Equals(object obj)
        {
            if (obj is BoundingSphere)
                return this.Equals((BoundingSphere)obj);


            return false;
        }


        public override int GetHashCode()
        {
            return this.Center.GetHashCode() + this.Radius.GetHashCode();
        }


        public bool Intersects(BoundingAABB aabb)
        {
            return aabb.Intersects(this);
        }


        public void Intersects(ref BoundingAABB aabb, out bool result)
        {
            result = Intersects(aabb);
        }





        public bool Intersects(BoundingSphere sphere)
        {
            float val = GenericMethods.Distance(sphere.Center, Center);
            if (val > sphere.Radius + Radius)
                return false;
            return true;
        }


        public void Intersects(ref BoundingSphere sphere, out bool result)
        {
            result = Intersects(sphere);
        }


        public enuPlaneIntersection Intersects(Plane plane)
        {
            float distance = Vector3.Dot(plane.Normal, this.Center) + plane.D;
            if (distance > this.Radius)
                return enuPlaneIntersection.Front;
            if (distance < -this.Radius)
                return enuPlaneIntersection.Back;
            //else it intersect
            return enuPlaneIntersection.Intersecting;
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


        public static bool operator ==(BoundingSphere a, BoundingSphere b)
        {
            return a.Equals(b);
        }


        public static bool operator !=(BoundingSphere a, BoundingSphere b)
        {
            return !a.Equals(b);
        }


        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{{Center:{0} Radius:{1}}}", this.Center.ToString(), this.Radius.ToString());
        }


        #endregion Public Methods

        //#region IRenderProvider implementation
        //public void Render(enuRenderMode renderMode)
        //{
        //    throw new NotImplementedException();
        //}
        //#endregion
    }
}
