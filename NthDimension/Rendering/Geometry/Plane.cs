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

using NthDimension.Rendering.Culling;

namespace NthDimension.Rendering.Geometry
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using NthDimension.Algebra;
    using NthDimension.Rasterizer;

    public class Plane : IEquatable<Plane>
    {
        #region Static

        public static Plane Up = new Plane(0, 0, 1, 0);
        public static Plane Down = new Plane(0, 0, -1, 0);
        public static Plane Empty = new Plane(0, 0, 0, 0);

        public static float InsersectionTolerance = 0.0001f;        // epsilon
        #endregion Static

        #region Public Fields

        private float m_d = .0f;     // converted to struct
        private Vector3 m_normal = new Vector3(); // converted to struct

        public float A { get { return m_normal.X; } set { m_normal.X = value; } }
        public float B { get { return m_normal.Y; } set { m_normal.Y = value; } }
        public float C { get { return m_normal.Z; } set { m_normal.Z = value; } }
        public float D { get { return m_d; } set { m_d = value; } }
        public Vector3 Normal { get { return m_normal; } set { m_normal = value; } }


        #endregion Public Fields

        #region Ctor
        public Plane()
        {
            m_d = 0f;
            m_normal = new Vector3();
        }
        public Plane(Vector4 value)
            : this(new Vector3(value.X, value.Y, value.Z), value.W)
        {


        }
        public Plane(Vector3 normal, float d)
        {
            m_normal = normal;
            m_d = d;
        }
        public Plane(Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 ab = b - a;
            Vector3 ac = c - a;


            Vector3 cross = Vector3.Cross(ab, ac);
            m_normal = Vector3.Normalize(cross);
            m_d = -(Vector3.Dot(cross, a));
        }
        public Plane(float a, float b, float c, float d)
            : this(new Vector3(a, b, c), d)
        {


        }
        #endregion

        public float Distance(Vector3 point)
        {
            return PerpendicularDistance(ref point, this);
        }
        public float Distance(BoundingSphere boundingSphere)
        {
            return PerpendicularDistance(ref boundingSphere.Center, this) - boundingSphere.Radius;
        }

        #region Equals -IEquatable
        public override bool Equals(object other)
        {
            return (other is Plane) && this.Equals((Plane)other);
        }
        public bool Equals(Plane other)
        {
            if (null != Normal /*&& null != D*/ && null == other)
                return false;

            return ((Normal == other.Normal) && (D == other.D));
        }
        public override int GetHashCode()
        {
            return m_normal.GetHashCode() ^ m_d.GetHashCode();
        }
        #endregion

        #region Operators
        public static bool operator !=(Plane plane1, Plane plane2)
        {
            if (object.ReferenceEquals(plane1, null))
                return !object.ReferenceEquals(plane2, null);

            return !plane1.Equals(plane2);
        }
        public static bool operator ==(Plane plane1, Plane plane2)
        {
            if (object.ReferenceEquals(plane1, null))
                return object.ReferenceEquals(plane2, null);
            
            return plane1.Equals(plane2);
        }
        #endregion

        #region Intersections
        public enuPlaneIntersection Intersects(BoundingAABB aabb)
        {
            return aabb.Intersects(this);
        }
        public void Intersects(ref BoundingAABB aabb, out enuPlaneIntersection result)
        {
            result = Intersects(aabb);
        }
        public enuPlaneIntersection Intersects(BoundingFrustum frustum)
        {
            return frustum.Intersects(this);
        }
        public enuPlaneIntersection Intersects(BoundingSphere boundingSphere)
        {
            return boundingSphere.Intersects(this);
        }
        public enuPlaneIntersection Intersects(Vector3 point)
        {
            float dist = ClassifyPoint(ref point, this);
            if (dist > InsersectionTolerance)
                return enuPlaneIntersection.Front;
            if (dist < -InsersectionTolerance)
                return enuPlaneIntersection.Back;
            return enuPlaneIntersection.Intersecting;
        }
        public enuPlaneIntersection IntersectsPoint(Vector3 point)
        {
            Vector3 vec = GenericMethods.Subtract(Normal * D, point);

            float dot = Vector3.Dot(vec, Normal);

            if (dot < -InsersectionTolerance)
                return enuPlaneIntersection.Front;
            if (dot > InsersectionTolerance)
                return enuPlaneIntersection.Back;
            return enuPlaneIntersection.Intersecting;
        }
        public void Intersects(ref BoundingSphere boundingSphere, out enuPlaneIntersection result)
        {
            result = Intersects(boundingSphere);
        }
        public enuPlaneIntersection IntersectsRay(Ray ray)
        {
            throw new NotImplementedException();
            ////ray.Intersects()

            //Vector3 N, V, W, Point;
            //Point = new Vector3();

            //float e, d;
            //N = new Vector3(0, 0, 1);

            //V = createVector(x, a);
            ////debug(V, "v");

            //d = dotProduct(N, V);
            ////debug("d = " + d.ToString());
            //W = createVector(x, y);
            ////debug(W, "W");
            //e = dotProduct(N, W);
            ////debug("e = " + e.ToString());

            //if (e != 0)
            //{
            //    Point.X = x.X + W.X * d / e;
            //    Point.Y = x.Y + W.Y * d / e;
            //    Point.Z = x.Z + W.Z * d / e;
            //    NotIntersectPlaneLine = false;
            //}

            //return Point;
        }
        public void Intersects(ref Ray ray, out enuPlaneIntersection result)
        {
            throw new NotImplementedException();
        }

        #region SSM routines
        // FROM SSM (with a note, SSM Class was NOT working properly in Aril 2016
        /// <summary>
        /// Whether the plane and a ray intersect and where
        /// http://en.wikipedia.org/wiki/Line%E2%80%93plane_intersection
        /// </summary>
        public bool intersects(ref Ray ray, out Vector3 intersectPt)
        {
            Vector3 n = this.Normal;
            var rayDirDotPlaneN = Vector3.Dot(ray.Direction, n);
            if (Math.Abs(rayDirDotPlaneN) < Plane.InsersectionTolerance)
            {
                // rayDirDotPlaneN == 0; ray and the plane are parallel
                intersectPt = new Vector3(float.NaN);
                return false;
            }
            else
            {
                // plug parametric equation of a line into the plane normal equation
                // solve (rayPos + rayDir * t - planeP0) dot planeN == 0 
                // rayDir dot planeN + (rayPos - planeP0) dot planeN == 0
                Vector3 p0 = this.pickASurfacePoint();
                float t = Vector3.Dot(p0 - ray.Origin, n) / rayDirDotPlaneN;
                if (t < -Plane.InsersectionTolerance)
                {
                    // this means that the line-plane intersection is behind the ray origin (in the wrong
                    // direction). in the context of a ray this means no intersection
                    intersectPt = new Vector3(float.NaN);
                    return false;
                }
                intersectPt = ray.Origin + ray.Direction * t;
                return true;
            }
        }

        /// <summary>
        /// Picks a point p0 (x0, y0, z0) that lies on a plane.
        /// </summary>
        public Vector3 pickASurfacePoint()
        {
            // TODO epsilonize
            //if (Math.Abs(A) > Plane.InsersectionTolerance)
            if (Math.Abs(m_normal.X) > Plane.InsersectionTolerance)
            {
                // if A != 0
                // let y0 == 0 and z0 == 0; then -A * x0 == D
                float x0 = D / -m_normal.X;
                return new Vector3(x0, 0f, 0f);
            }
            else if (Math.Abs(m_normal.Y) > Plane.InsersectionTolerance)
            {
                // else if B != 0
                // let x0 == 0 and z0 == 0; then -B * y0 == D
                float y0 = D / -m_normal.Y;
                return new Vector3(0f, y0, 0f);
            }
            else if (Math.Abs(m_normal.Z) > Plane.InsersectionTolerance)
            {
                // else if C != 0
                // let x0 == 0 and y0 == 0; then -C * z0 == D
                float z0 = D / -m_normal.Z;
                return new Vector3(0f, 0f, z0);
            }
            else
            {
                throw new Exception("invalid plane normal: " + m_normal.ToString());
            }
        }

        public float DistanceToPoint(Vector3 point) // SSM
        {
            return Plane.PerpendicularDistance(ref point, this);
        }

        public void Normalize()
        {
            float t = (float)Math.Sqrt(m_normal.X * m_normal.X + m_normal.Y * m_normal.Y + m_normal.Z * m_normal.Z);
            m_normal.X /= t;
            m_normal.Y /= t;
            m_normal.Z /= t;
            D /= t;
        }
        #endregion

        #endregion

        public override string ToString()
        {
            return string.Format("{{Normal:{0} D:{1}}}", Normal, D);
        }
        #region Internal Methods

        // Indicating which side (positive/negative) of a plane a point is on.
        // Returns > 0 if on the positive side, < 0 if on the negative size, 0 if on the plane.
        internal static float ClassifyPoint(ref Vector3 point, Plane plane)
        {
            return point.X * plane.Normal.X + point.Y * plane.Normal.Y + point.Z * plane.Normal.Z + plane.D;
        }

        // Calculates the perpendicular distance from a point to a plane
        internal static float PerpendicularDistance(ref Vector3 point, Plane plane)
        {

            // dist = (ax + by + cz + d) / sqrt(a*a + b*b + c*c)
            return (float)System.Math.Abs((plane.Normal.X * point.X +
                                           plane.Normal.Y * point.Y +
                                           plane.Normal.Z * point.Z) /
                                          System.Math.Sqrt(plane.Normal.X * plane.Normal.X +
                                                           plane.Normal.Y * plane.Normal.Y +
                                                           plane.Normal.Z * plane.Normal.Z));
        }



        #endregion
    }
}
