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

using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using NthDimension.Rendering.Geometry;

namespace NthDimension.Rendering.Culling
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using NthDimension.Algebra;
    using NthDimension.Rasterizer;

    public class BoundingFrustum : IEquatable<BoundingFrustum>
// , 
                                                              //IRenderProvider
    {
        #region Properties

        public Plane TopPlane
        {
            get { return this.m_topPlane; }
            set { this.m_topPlane = value; }
        }

        public Plane BottomPlane
        {
            get { return this.m_bottomPlane; }
            set { this.m_bottomPlane = value; }
        }

        public Plane LeftPlane
        {
            get { return this.m_leftPlane; }
            set { this.m_leftPlane = value; }
        }

        public Plane RightPlane
        {
            get { return this.m_rightPlane; }
            set { this.m_rightPlane = value; }
        }

        public Plane FarPlane
        {
            get { return this.m_farPlane; }
            set { this.m_farPlane = value; }
        }

        public Plane NearPlane
        {
            get { return this.m_nearPlane; }
            set { this.m_nearPlane = value; }
        }

        public Matrix4 Matrix
        {
            get { return this.m_matrix; }
            set
            {
                this.m_matrix = value;
                this.CreatePlanes();    // FIXME: The odds are the planes will be used a lot more often than the matrix
                this.CreateCorners();   // is updated, so this should help performance. I hope ;)
            }
        }

        public Vector3[] Corners
        { get { return corners; } set { corners = value; } }
        #endregion Public Properties

        #region Fields
        protected Plane m_topPlane;
        protected Plane m_bottomPlane;
        protected Plane m_leftPlane;
        protected Plane m_rightPlane;
        protected Plane m_farPlane;
        protected Plane m_nearPlane;
        protected Matrix4 m_matrix;
        protected Vector3[] corners;
        #endregion fields

        #region Ctor
        public BoundingFrustum(Matrix4 matrix)
        {
            this.m_matrix = matrix;
            CreatePlanes();
            CreateCorners();
        }

        #endregion

        #region Operator overloads
        public static bool operator ==(BoundingFrustum a, BoundingFrustum b)
        {
            if (Equals(a, null))
                return (Equals(b, null));

            if (Equals(b, null))
                return (Equals(a, null));

            return a.m_matrix == (b.m_matrix);
        }
        public static bool operator !=(BoundingFrustum a, BoundingFrustum b)
        {
            return !(a == b);
        }
        #endregion Operator overloads

        #region IEquatable
        public bool Equals(BoundingFrustum other)
        {
            return (this == other);
        }
        public override bool Equals(object obj)
        {
            BoundingFrustum f = obj as BoundingFrustum;
            return (Equals(f, null)) ? false : (this == f);
        }
        public override int GetHashCode()
        {
            return this.m_matrix.GetHashCode();
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(256);
            sb.Append("{Near:");
            sb.Append(this.m_nearPlane.ToString());
            sb.Append(" Far:");
            sb.Append(this.m_farPlane.ToString());
            sb.Append(" Left:");
            sb.Append(this.m_leftPlane.ToString());
            sb.Append(" Right:");
            sb.Append(this.m_rightPlane.ToString());
            sb.Append(" Top:");
            sb.Append(this.m_topPlane.ToString());
            sb.Append(" Bottom:");
            sb.Append(this.m_bottomPlane.ToString());
            sb.Append("}");
            return sb.ToString();
        }
        #endregion        

        #region protected methods
        protected void CreateCorners()
        {
            this.corners = new Vector3[8];
            this.corners[0] = IntersectionPoint(ref this.m_nearPlane, ref this.m_leftPlane, ref this.m_topPlane);
            this.corners[1] = IntersectionPoint(ref this.m_nearPlane, ref this.m_rightPlane, ref this.m_topPlane);
            this.corners[2] = IntersectionPoint(ref this.m_nearPlane, ref this.m_rightPlane, ref this.m_bottomPlane);
            this.corners[3] = IntersectionPoint(ref this.m_nearPlane, ref this.m_leftPlane, ref this.m_bottomPlane);
            this.corners[4] = IntersectionPoint(ref this.m_farPlane, ref this.m_leftPlane, ref this.m_topPlane);
            this.corners[5] = IntersectionPoint(ref this.m_farPlane, ref this.m_rightPlane, ref this.m_topPlane);
            this.corners[6] = IntersectionPoint(ref this.m_farPlane, ref this.m_rightPlane, ref this.m_bottomPlane);
            this.corners[7] = IntersectionPoint(ref this.m_farPlane, ref this.m_leftPlane, ref this.m_bottomPlane);
        }
        protected void CreatePlanes()
        {
            // Pre-calculate the different planes needed
            this.m_leftPlane = new Plane(-m_matrix.M14 - m_matrix.M11, -m_matrix.M24 - m_matrix.M21,
                                         -m_matrix.M34 - m_matrix.M31, -m_matrix.M44 - m_matrix.M41);

            this.m_rightPlane = new Plane(m_matrix.M11 - m_matrix.M14, m_matrix.M21 - m_matrix.M24,
                                          m_matrix.M31 - m_matrix.M34, m_matrix.M41 - m_matrix.M44);

            this.m_topPlane = new Plane(m_matrix.M12 - m_matrix.M14, m_matrix.M22 - m_matrix.M24,
                                        m_matrix.M32 - m_matrix.M34, m_matrix.M42 - m_matrix.M44);

            this.m_bottomPlane = new Plane(-m_matrix.M14 - m_matrix.M12, -m_matrix.M24 - m_matrix.M22,
                                           -m_matrix.M34 - m_matrix.M32, -m_matrix.M44 - m_matrix.M42);

            this.m_nearPlane = new Plane(-m_matrix.M13, -m_matrix.M23, -m_matrix.M33, -m_matrix.M43);


            this.m_farPlane = new Plane(m_matrix.M13 - m_matrix.M14, m_matrix.M23 - m_matrix.M24,
                                 m_matrix.M33 - m_matrix.M34, m_matrix.M43 - m_matrix.M44);

            this.NormalizePlane(ref this.m_leftPlane);
            this.NormalizePlane(ref this.m_rightPlane);
            this.NormalizePlane(ref this.m_topPlane);
            this.NormalizePlane(ref this.m_bottomPlane);
            this.NormalizePlane(ref this.m_nearPlane);
            this.NormalizePlane(ref this.m_farPlane);
        }
        protected void NormalizePlane(ref Plane p)
        {
            float factor = 1f / p.Normal.LengthFast;

            p.Normal *= factor;
        
            p.D *= factor;

            #region todo
            //kill
            //p.Normal.X *= factor;
            //p.Normal.Y *= factor;
            //p.Normal.Z *= factor;
            #endregion
        }
        #endregion

        #region λογική εργασίας
        #region --Containment
        public enuContainmentType Contains(BoundingAABB aabb)
        {
            enuContainmentType result;
            this.Contains(ref aabb, out result);
            return result;
        }
        public void Contains(ref BoundingAABB aabb, out enuContainmentType result)
        {
            // FIXME: Is this a bug?
            // If the bounding box is of W * D * H = 0, then return disjoint
            if (aabb.Min == aabb.Max)
            {
                result = enuContainmentType.Disjoint;
                return;
            }

            int i;
            enuContainmentType contained;

            Vector3[] corners = aabb.GetCorners();

            // First we assume completely disjoint. So if we find a point that is contained, we break out of this loop
            for (i = 0; i < corners.Length; i++)
            {
                this.Contains(ref corners[i], out contained);
                if (contained != enuContainmentType.Disjoint)
                    break;
            }

            if (i == corners.Length) // This means we checked all the corners and they were all disjoint
            {
                result = enuContainmentType.Disjoint;
                return;
            }

            if (i != 0)             // if i is not equal to zero, we can fastpath and say that this box intersects
            {                       // because we know at least one point is outside and one is inside.
                result = enuContainmentType.Intersects;
                return;
            }

            // If we get here, it means the first (and only) point we checked was actually contained in the frustum.
            // So we assume that all other points will also be contained. If one of the points is disjoint, we can
            // exit immediately saying that the result is Intersects
            i++;
            for (; i < corners.Length; i++)
            {
                this.Contains(ref corners[i], out contained);
                if (contained != enuContainmentType.Contains)
                {
                    result = enuContainmentType.Intersects;
                    return;
                }
            }

            // If we get here, then we know all the points were actually contained, therefore result is Contains
            result = enuContainmentType.Contains;
            return;
        }
        public enuContainmentType Contains(BoundingFrustum frustum)
        {
            if (this == frustum)                // We check to see if the two frustums are equal
                return enuContainmentType.Contains;// If they are, there's no need to go any further.

            throw new NotImplementedException();
        }
        public enuContainmentType Contains(BoundingSphere sphere)
        {
            enuContainmentType result;
            this.Contains(ref sphere, out result);
            return result;
        }
        public void Contains(ref BoundingSphere sphere, out enuContainmentType result)
        {
            float val;
            enuContainmentType contained;

            // We first check if the sphere is inside the frustum
            this.Contains(ref sphere.Center, out contained);

            // The sphere is inside. Now we need to check if it's fully contained or not
            // So we see if the perpendicular distance to each plane is less than or equal to the sphere's radius.
            // If the perpendicular distance is less, just return Intersects.
            if (contained == enuContainmentType.Contains)
            {
                val = Plane.PerpendicularDistance(ref sphere.Center, this.m_bottomPlane);
                if (val < sphere.Radius)
                {
                    result = enuContainmentType.Intersects;
                    return;
                }

                val = Plane.PerpendicularDistance(ref sphere.Center, this.m_farPlane);
                if (val < sphere.Radius)
                {
                    result = enuContainmentType.Intersects;
                    return;
                }

                val = Plane.PerpendicularDistance(ref sphere.Center, this.m_leftPlane);
                if (val < sphere.Radius)
                {
                    result = enuContainmentType.Intersects;
                    return;
                }

                val = Plane.PerpendicularDistance(ref sphere.Center, this.m_nearPlane);
                if (val < sphere.Radius)
                {
                    result = enuContainmentType.Intersects;
                    return;
                }

                val = Plane.PerpendicularDistance(ref sphere.Center, this.m_rightPlane);
                if (val < sphere.Radius)
                {
                    result = enuContainmentType.Intersects;
                    return;
                }

                val = Plane.PerpendicularDistance(ref sphere.Center, this.m_topPlane);
                if (val < sphere.Radius)
                {
                    result = enuContainmentType.Intersects;
                    return;
                }

                // If we get here, the sphere is fully contained
                result = enuContainmentType.Contains;
                return;
            }
            //duff idea : test if all corner is in same side of a plane if yes and outside it is disjoint else intersect
            // issue is that we can have some times when really close aabb 



            // If we're here, the the sphere's centre was outside of the frustum. This makes things hard :(
            // We can't use perpendicular distance anymore. I'm not sure how to code this.
            //throw new NotImplementedException();
            result = enuContainmentType.Disjoint;


            
        }
        public enuContainmentType Contains(Vector3 point)
        {
            enuContainmentType result;
            this.Contains(ref point, out result);
            return result;
        }
        public void Contains(ref Vector3 point, out enuContainmentType result)
        {
            float val;
            // If a point is on the POSITIVE side of the plane, then the point is not contained within the frustum

            // Check the top
            val = Plane.ClassifyPoint(ref point, this.m_topPlane);
            if (val > 0)
            {
                result = enuContainmentType.Disjoint;
                return;
            }

            // Check the bottom
            val = Plane.ClassifyPoint(ref point, this.m_bottomPlane);
            if (val > 0)
            {
                result = enuContainmentType.Disjoint;
                return;
            }

            // Check the left
            val = Plane.ClassifyPoint(ref point, this.m_leftPlane);
            if (val > 0)
            {
                result = enuContainmentType.Disjoint;
                return;
            }

            // Check the right
            val = Plane.ClassifyPoint(ref point, this.m_rightPlane);
            if (val > 0)
            {
                result = enuContainmentType.Disjoint;
                return;
            }

            // Check the near
            val = Plane.ClassifyPoint(ref point, this.m_nearPlane);
            if (val > 0)
            {
                result = enuContainmentType.Disjoint;
                return;
            }

            // Check the far
            val = Plane.ClassifyPoint(ref point, this.m_farPlane);
            if (val > 0)
            {
                result = enuContainmentType.Disjoint;
                return;
            }

            // If we get here, it means that the point was on the correct side of each plane to be
            // contained. Therefore this point is contained
            result = enuContainmentType.Contains;
        }
        #endregion Containment

        #region --Intersection
        protected static Vector3 IntersectionPoint(ref Plane a, ref Plane b, ref Plane c)
        {
            // Formula used
            //                d1 ( N2 * N3 ) + d2 ( N3 * N1 ) + d3 ( N1 * N2 )
            //P =   -------------------------------------------------------------------------
            //                             N1 . ( N2 * N3 )
            //
            // Note: N refers to the normal, d refers to the displacement. '.' means dot product. '*' means cross product

            Vector3 v1, v2, v3;
            float f = -Vector3.Dot(a.Normal, Vector3.Cross(b.Normal, c.Normal));

            v1 = (a.D * (Vector3.Cross(b.Normal, c.Normal)));
            v2 = (b.D * (Vector3.Cross(c.Normal, a.Normal)));
            v3 = (c.D * (Vector3.Cross(a.Normal, b.Normal)));

            Vector3 vec = new Vector3(v1.X + v2.X + v3.X, v1.Y + v2.Y + v3.Y, v1.Z + v2.Z + v3.Z);
            return vec / f;
        }

        // Note:: some throw NotImplemented exceptions
        //public bool Intersects(BoundingAABB box)
        //{
        //    foreach (Plane plane in GetPlanes(this))
        //    {
        //        if ((plane.Normal.X * box.Min.X) + (plane.Normal.Y * box.Min.Y) + (plane.Normal.Z * box.Min.Z) + plane.D > 0)
        //            continue;

        //        if ((plane.Normal.X * box.Max.X) + (plane.Normal.Y * box.Min.Y) + (plane.Normal.Z * box.Min.Z) + plane.D > 0)
        //            continue;

        //        if ((plane.Normal.X * box.Max.X) + (plane.Normal.Y * box.Min.Y) + (plane.Normal.Z * box.Max.Z) + plane.D > 0)
        //            continue;

        //        if ((plane.Normal.X * box.Min.X) + (plane.Normal.Y * box.Min.Y) + (plane.Normal.Z * box.Max.Z) + plane.D > 0)
        //            continue;

        //        if ((plane.Normal.X * box.Min.X) + (plane.Normal.Y * box.Max.Y) + (plane.Normal.Z * box.Min.Z) + plane.D > 0)
        //            continue;

        //        if ((plane.Normal.X * box.Max.X) + (plane.Normal.Y * box.Max.Y) + (plane.Normal.Z * box.Min.Z) + plane.D > 0)
        //            continue;

        //        if ((plane.Normal.X * box.Max.X) + (plane.Normal.Y * box.Max.Y) + (plane.Normal.Z * box.Max.Z) + plane.D > 0)
        //            continue;

        //        if ((plane.Normal.X * box.Min.X) + (plane.Normal.Y * box.Max.Y) + (plane.Normal.Z * box.Max.Z) + plane.D > 0)
        //            continue;

        //        // all points are behind the one plane so they can't be inside any other plane
        //        return false;
        //    }

        //    return true;
        //}
        public bool Intersects(BoundingAABB aabb)
        {
            bool result = true;

            Plane[] planes = GetPlanes(this);

            ParallelLoopResult res = Parallel.ForEach(planes, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, (plane, state) =>
                {
                    float NormalX_MinX = plane.Normal.X * aabb.Min.X;
                    float NormalX_MaxX = plane.Normal.X * aabb.Max.X;

                    float NormalY_MinY = plane.Normal.Y * aabb.Min.Y;
                    float NormalY_MaxY = plane.Normal.Y * aabb.Max.Y;

                    float NormalZ_MinZ = plane.Normal.Z * aabb.Min.Z;
                    float NormalZ_MaxZ = plane.Normal.Z * aabb.Max.Z;


                    if (NormalX_MinX + NormalY_MinY + (NormalZ_MinZ) + plane.D > 0)
                        return;

                    if (NormalX_MaxX + (NormalY_MinY) + (NormalZ_MinZ) + plane.D > 0)
                        return;

                    if (NormalX_MaxX + (NormalY_MinY) + (NormalZ_MaxZ) + plane.D > 0)
                        return;

                    if (NormalX_MinX + (NormalY_MinY) + (NormalZ_MaxZ) + plane.D > 0)
                        return;

                    if (NormalX_MinX + (NormalY_MaxY) + (NormalZ_MinZ) + plane.D > 0)
                        return;

                    if (NormalX_MaxX + (NormalY_MaxY) + (NormalZ_MinZ) + plane.D > 0)
                        return;

                    if (NormalX_MaxX + (NormalY_MaxY) + (NormalZ_MaxZ) + plane.D > 0)
                        return;

                    if (NormalX_MinX + (NormalY_MaxY) + (NormalZ_MaxZ) + plane.D > 0)
                        return;

                    result = false;
                    // all points are behind the one plane so they can't be inside any other plane
                    
                });
            return result;

        }
        public bool Intersects(IEnumerable<Vector3> polygon)
        {
            foreach (Plane plane in GetPlanes(this))
            {
                bool gotOne = false;
                foreach (Vector3 vert in polygon)
                {
                    if ((plane.Normal.X * vert.X) + (plane.Normal.Y * vert.Y) + (plane.Normal.Z * vert.Z) + plane.D > 0)
                    {
                        gotOne = true;
                        break;
                    }
                }
                if (gotOne)
                    continue;

                // all points are behind the one plane so they can't be inside any other plane
                return false;
            }
            return true;
        }
        public void Intersects(ref BoundingAABB aabb, out bool result)
        {
            result = Intersects(aabb);
        }
        public void Intersects(ref IEnumerable<Vector3> polygon, out bool result)
        {
            result = Intersects(polygon);
        }
        public bool Intersects(BoundingFrustum frustum)
        {
            return Contains(frustum) != enuContainmentType.Disjoint;
        }
        public bool Intersects(BoundingSphere sphere)
        {
            return Contains(sphere) != enuContainmentType.Disjoint;
        }
        public void Intersects(ref BoundingSphere sphere, out bool result)
        {
            result = Contains(sphere) != enuContainmentType.Disjoint;
        }
        public enuPlaneIntersection Intersects(Plane plane)
        {
            throw new NotImplementedException();
        }
        public void Intersects(ref Plane plane, out enuPlaneIntersection result)
        {
            throw new NotImplementedException();
        }
        public Nullable<float> Intersects(Ray ray)
        {
            throw new NotImplementedException();
        }
        public void Intersects(ref Ray ray, out Nullable<float> result)
        {
            throw new NotImplementedException();
        }
        #endregion

        #endregion

        
        public Vector3[] GetCorners()
        {
            return corners;
        }

        public static Plane[] GetPlanes(BoundingFrustum frustum)
        {
            Plane[] l = new Plane[6];
            l[0] = frustum.NearPlane;
            l[1] = frustum.LeftPlane;
            l[2] = frustum.RightPlane;
            l[3] = frustum.TopPlane;
            l[4] = frustum.BottomPlane;
            l[5] = frustum.FarPlane;

            return l;
        }

        #region IRenderProvider Implementation
        //public virtual void Render()
        //{
        //    //throw new NotImplementedException();
        //}
        #endregion
    }
}
