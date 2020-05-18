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
using NthDimension.Rendering.Culling;

namespace NthDimension.Rendering.Geometry
{
    public struct Ray : IEquatable<Ray>
    {
        #region Public Fields

        public Vector3 Direction;
        public Vector3 Origin;

        #endregion

        #region Ctor


        public Ray(Vector3 origin, Vector3 direction)
        {
            this.Origin = origin;
            this.Direction = direction;

        }
        #endregion

        #region Equals - IEquatable
        public override bool Equals(object obj)
        {
            return (obj is Ray) ? this.Equals((Ray)obj) : false;
        }
        public bool Equals(Ray other)
        {
            return this.Origin.Equals(other.Origin) && this.Direction.Equals(other.Direction);
        }
        public override int GetHashCode()
        {
            return Origin.GetHashCode() ^ Direction.GetHashCode();
        }
        #endregion

        #region Intersections
        #region - Plane
        public float? Intersects(Plane plane)
        {
            throw new NotImplementedException();
        }
        public void Intersects(ref Plane plane, out float? result)
        {
            result = Intersects(plane);
        }
        #endregion

        #region - AABB
        public float? Intersects(BoundingAABB aabb)
        {
            //first test if start in box
            if (Origin.X >= aabb.Min.X
                && Origin.X <= aabb.Max.X
                && Origin.Y >= aabb.Min.Y
                && Origin.Y <= aabb.Max.Y
                && Origin.Z >= aabb.Min.Z
                && Origin.Z <= aabb.Max.Z)
                return 0.0f;// here we concidere cube is full and origine is in cube so intersect at origine

            //Second we check each face
            Vector3 maxT = new Vector3(-1.0f, -1.0f, -1.0f);
            //Vector3 minT = new Vector3(-1.0f);
            //calcul intersection with each faces
            if (Origin.X < aabb.Min.X && Direction.X != 0.0f)
                maxT.X = (aabb.Min.X - Origin.X) / Direction.X;
            else if (Origin.X > aabb.Max.X && Direction.X != 0.0f)
                maxT.X = (aabb.Max.X - Origin.X) / Direction.X;
            if (Origin.Y < aabb.Min.Y && Direction.Y != 0.0f)
                maxT.Y = (aabb.Min.Y - Origin.Y) / Direction.Y;
            else if (Origin.Y > aabb.Max.Y && Direction.Y != 0.0f)
                maxT.Y = (aabb.Max.Y - Origin.Y) / Direction.Y;
            if (Origin.Z < aabb.Min.Z && Direction.Z != 0.0f)
                maxT.Z = (aabb.Min.Z - Origin.Z) / Direction.Z;
            else if (Origin.Z > aabb.Max.Z && Direction.Z != 0.0f)
                maxT.Z = (aabb.Max.Z - Origin.Z) / Direction.Z;

            //get the maximum maxT
            if (maxT.X > maxT.Y && maxT.X > maxT.Z)
            {
                if (maxT.X < 0.0f)
                    return null;// ray go on opposite of face
                                //coordonate of hit point of face of cube
                float coord = Origin.Z + maxT.X * Direction.Z;
                // if hit point coord ( intersect face with ray) is out of other plane coord it miss 
                if (coord < aabb.Min.Z || coord > aabb.Max.Z)
                    return null;
                coord = Origin.Y + maxT.X * Direction.Y;
                if (coord < aabb.Min.Y || coord > aabb.Max.Y)
                    return null;
                return maxT.X;
            }
            if (maxT.Y > maxT.X && maxT.Y > maxT.Z)
            {
                if (maxT.Y < 0.0f)
                    return null;// ray go on opposite of face
                                //coordonate of hit point of face of cube
                float coord = Origin.Z + maxT.Y * Direction.Z;
                // if hit point coord ( intersect face with ray) is out of other plane coord it miss 
                if (coord < aabb.Min.Z || coord > aabb.Max.Z)
                    return null;
                coord = Origin.X + maxT.Y * Direction.X;
                if (coord < aabb.Min.X || coord > aabb.Max.X)
                    return null;
                return maxT.Y;
            }
            else //Z
            {
                if (maxT.Z < 0.0f)
                    return null;// ray go on opposite of face
                                //coordonate of hit point of face of cube
                float coord = Origin.X + maxT.Z * Direction.X;
                // if hit point coord ( intersect face with ray) is out of other plane coord it miss 
                if (coord < aabb.Min.X || coord > aabb.Max.X)
                    return null;
                coord = Origin.Y + maxT.Z * Direction.Y;
                if (coord < aabb.Min.Y || coord > aabb.Max.Y)
                    return null;
                return maxT.Z;
            }
        }
        public void Intersects(ref BoundingAABB aabb, out float? result)
        {
            result = Intersects(aabb);
        }
        #endregion

        #region - Frustum
        public float? Intersects(BoundingFrustum frustum)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region - Sphere
        public float? Intersects(BoundingSphere sphere)
        {
            float? result;
            Intersects(ref sphere, out result);
            return result;
        }
        public void Intersects(ref BoundingSphere sphere, out float? result)
        {
            // Find the vector between where the ray starts the the sphere's centre
            Vector3 difference = sphere.Center - this.Origin;

            float differenceLengthSquared = difference.LengthSquared;
            float sphereRadiusSquared = sphere.Radius * sphere.Radius;

            float distanceAlongRay;

            // If the distance between the ray start and the sphere's centre is less than
            // the radius of the sphere, it means we've intersected. N.B. checking the LengthSquared is faster.
            if (differenceLengthSquared < sphereRadiusSquared)
            {
                result = 0.0f;
                return;
            }

            Vector3.Dot(ref this.Direction, ref difference, out distanceAlongRay);
            // If the ray is pointing away from the sphere then we don't ever intersect
            if (distanceAlongRay < 0)
            {
                result = null;
                return;
            }

            // Next we kinda use Pythagoras to check if we are within the bounds of the sphere
            // if x = radius of sphere
            // if y = distance between ray position and sphere centre
            // if z = the distance we've travelled along the ray
            // if x^2 + z^2 - y^2 < 0, we do not intersect
            float dist = sphereRadiusSquared + distanceAlongRay * distanceAlongRay - differenceLengthSquared;

            result = (dist < 0) ? null : distanceAlongRay - (float?)System.Math.Sqrt(dist);
        }
        #endregion
        #endregion

        #region Operators
        public static bool operator !=(Ray a, Ray b)
        {
            return !a.Equals(b);
        }
        public static bool operator ==(Ray a, Ray b)
        {
            return a.Equals(b);
        }
        #endregion

        #region General
        public override string ToString()
        {
            return string.Format("{{Position:{0} Direction:{1}}}", Origin.ToString(), Direction.ToString());
        }
        #endregion
    }
}
