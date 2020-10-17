/* Copyright (C) <2009-2011> <Thorben Linneweber, Jitter Physics>
* 
*  This software is provided 'as-is', without any express or implied
*  warranty.  In no event will the authors be held liable for any damages
*  arising from the use of this software.
*
*  Permission is granted to anyone to use this software for any purpose,
*  including commercial applications, and to alter it and redistribute it
*  freely, subject to the following restrictions:
*
*  1. The origin of this software must not be misrepresented; you must not
*      claim that you wrote the original software. If you use this software
*      in a product, an acknowledgment in the product documentation would be
*      appreciated but is not required.
*  2. Altered source versions must be plainly marked as such, and must not be
*      misrepresented as being the original software.
*  3. This notice may not be removed or altered from any source distribution. 
*/

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



#region Using Statements
using System;
using System.Collections.Generic;

using NthDimension.Physics.Dynamics;
using NthDimension.Physics.Collision.Shapes;
using NthDimension.Physics.LinearMath;
using NthDimension.Algebra;

#endregion

namespace NthDimension.Physics.Collision.Shapes
{

    /// <summary>
    /// Represents a terrain.
    /// </summary>
    public class TerrainShape : Multishape
    {
        private float[,] heights;
        private float scaleX, scaleZ;
        private float width, length;
        private int heightsLength0, heightsLength1;

        private int minX, maxX;
        private int minZ, maxZ;
        private int numX, numZ;

        private JBBox boundings;

        private float sphericalExpansion = 0.05f;

        /// <summary>
        /// Expands the triangles by the specified amount.
        /// This stabilizes collision detection for flat shapes.
        /// </summary>
        public float SphericalExpansion
        {
            get { return sphericalExpansion; }
            set { sphericalExpansion = value; }
        }


        /// <summary>
        /// NOTE: ERRONEOUS!    Initializes a new instance of the TerrainShape class.
        /// </summary>
        /// <param name="heights">An array containing the heights of the terrain surface.</param>
        /// <param name="scaleX">The x-scale factor. (The x-space between neighbour heights)</param>
        /// <param name="scaleZ">The y-scale factor. (The y-space between neighbour heights)</param>
        public TerrainShape(float[,] heights, float scaleX, float scaleZ, float width, float length)
        {
            //heightsLength0 = heights.GetLength(0);
            //heightsLength1 = heights.GetLength(1);

            heightsLength0 = (int)(width * scaleX);
            heightsLength1 = (int)(length * scaleZ);

            #region Bounding Box
            boundings = JBBox.SmallBox;

            for (int i = 0; i < heightsLength0; i++)
            {                
                for (int e = 0; e < heightsLength1; e++)
                {
                    if (heights[i, e] > boundings.Max.Y)
                        boundings.Max.Y = heights[i, e];
                    else if (heights[i, e] < boundings.Min.Y)
                        boundings.Min.Y = heights[i, e];

                    if (heights[i, e] > boundings.Max.Y)
                        boundings.Max.Y = heights[i, e];
                    else if (heights[i, e] < boundings.Min.Y)
                        boundings.Min.Y = heights[i, e];
                }
            }

            //boundings.Min.X = checked((-length/2) * scaleX);
            //boundings.Min.Z = checked((-width/2) * scaleZ);

            //boundings.Max.X = checked(heightsLength0 * scaleX);
            //boundings.Max.Z = checked(heightsLength1 * scaleZ);

            boundings.Min.X = ((-length / 2) * scaleX);
            boundings.Min.Z = ((-width / 2) * scaleZ);

            boundings.Max.X = (heightsLength0 * scaleX);
            boundings.Max.Z = (heightsLength1 * scaleZ);


#endregion

            this.heights = heights;
            this.scaleX = scaleX;
            this.scaleZ = scaleZ;
            this.width = width;
            this.length = length;


            UpdateShape();
        }

      

        public TerrainShape(Vector3[] points, float[,] heights, float scaleX, float scaleZ)
        {
            if (points == null)
                throw new ArgumentNullException();

            // TODO: Just check that Count > 0
            bool empty = true;
            Vector3 minVec = Vector3.Zero;
            Vector3 maxVec = Vector3.Zero;
            
            
            foreach (Vector3 vector3 in points)
            {
                if (minVec == Vector3.Zero)
                {
                    minVec = new Vector3(vector3);
                    maxVec = new Vector3(vector3);
                }
                else
                {
                    minVec = Vector3.Min(minVec, vector3);
                    maxVec = Vector3.Max(maxVec, vector3);
                }
                empty = false;
            }
            if (empty)
                throw new ArgumentException();

            boundings.Min = new JVector(minVec);
            boundings.Max = new JVector(maxVec);

            this.scaleX = scaleX;
            this.scaleZ = scaleZ;

            heightsLength0 = heights.GetLength(0);
            heightsLength1 = heights.GetLength(1);

            UpdateShape();
        }

        internal TerrainShape() { }

 
        protected override Multishape CreateWorkingClone()
        {
            TerrainShape clone = new TerrainShape();
            clone.heights = this.heights;
            clone.scaleX = this.scaleX;
            clone.scaleZ = this.scaleZ;
            clone.boundings = this.boundings;
            clone.heightsLength0 = this.heightsLength0;
            clone.heightsLength1 = this.heightsLength1;
            clone.sphericalExpansion = this.sphericalExpansion;
            return clone;
        }


        private JVector[] points = new JVector[3];
        private JVector normal = JVector.Up;

        /// <summary>
        /// Sets the current shape. First <see cref="Prepare"/> has to be called.
        /// After SetCurrentShape the shape immitates another shape.
        /// </summary>
        /// <param name="index"></param>
        public override void SetCurrentShape(int index)
        {
            bool leftTriangle = false;

            if (index >= numX * numZ)
            {
                leftTriangle = true;
                index -= numX * numZ;
            }

            int quadIndexX = index % numX;
            int quadIndexZ = index / numX;

            // each quad has two triangles, called 'leftTriangle' and !'leftTriangle'
            if (leftTriangle)
            {
                points[0].Set((minX + quadIndexX + 0) * scaleX, heights[minX + quadIndexX + 0, minZ + quadIndexZ + 0], (minZ + quadIndexZ + 0) * scaleZ);
                points[1].Set((minX + quadIndexX + 1) * scaleX, heights[minX + quadIndexX + 1, minZ + quadIndexZ + 0], (minZ + quadIndexZ + 0) * scaleZ);
                points[2].Set((minX + quadIndexX + 0) * scaleX, heights[minX + quadIndexX + 0, minZ + quadIndexZ + 1], (minZ + quadIndexZ + 1) * scaleZ);
            }
            else
            {
                points[0].Set((minX + quadIndexX + 1) * scaleX, heights[minX + quadIndexX + 1, minZ + quadIndexZ + 0], (minZ + quadIndexZ + 0) * scaleZ);
                points[1].Set((minX + quadIndexX + 1) * scaleX, heights[minX + quadIndexX + 1, minZ + quadIndexZ + 1], (minZ + quadIndexZ + 1) * scaleZ);
                points[2].Set((minX + quadIndexX + 0) * scaleX, heights[minX + quadIndexX + 0, minZ + quadIndexZ + 1], (minZ + quadIndexZ + 1) * scaleZ);
            }

            JVector sum = points[0];
            JVector.Add(ref sum, ref points[1], out sum);
            JVector.Add(ref sum, ref points[2], out sum);
            JVector.Multiply(ref sum, 1.0f / 3.0f, out sum);
            geomCen = sum;

            JVector.Subtract(ref points[1], ref points[0], out sum);
            JVector.Subtract(ref points[2], ref points[0], out normal);
            JVector.Cross(ref sum, ref normal, out normal);
        }

        public void CollisionNormal(out JVector normal)
        {
            normal = this.normal;
        }


        /// <summary>
        /// Passes a axis aligned bounding box to the shape where collision
        /// could occour.
        /// </summary>
        /// <param name="box">The bounding box where collision could occur.</param>
        /// <returns>The upper index with which <see cref="SetCurrentShape"/> can be 
        /// called.</returns>
        public override int Prepare(ref JBBox box)
        {
            // simple idea: the terrain is a grid. x and z is the position in the grid.
            // y the height. we know compute the min and max grid-points. All quads
            // between these points have to be checked.

            // including overflow exception prevention

            if (box.Min.X < boundings.Min.X) minX = 0;
            else
            {
                minX = (int)System.Math.Floor((float)((box.Min.X - sphericalExpansion) / scaleX));
                minX = System.Math.Max(minX, 0);
            }

            if (box.Max.X > boundings.Max.X) maxX = heightsLength0 - 1;
            else
            {
                maxX = (int)System.Math.Ceiling((float)((box.Max.X + sphericalExpansion) / scaleX));
                maxX = System.Math.Min(maxX, heightsLength0 - 1);
            }

            if (box.Min.Z < boundings.Min.Z) minZ = 0;
            else
            {
                minZ = (int)System.Math.Floor((float)((box.Min.Z - sphericalExpansion) / scaleZ));
                minZ = System.Math.Max(minZ, 0);
            }

            if (box.Max.Z > boundings.Max.Z) maxZ = heightsLength1 - 1;
            else
            {
                maxZ = (int)System.Math.Ceiling((float)((box.Max.Z + sphericalExpansion) / scaleZ));
                maxZ = System.Math.Min(maxZ, heightsLength1 - 1);
            }

            numX = maxX - minX;
            numZ = maxZ - minZ;

            // since every quad contains two triangles we multiply by 2.
            return numX * numZ * 2;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void CalculateMassInertia()
        {
            this.inertia = JMatrix.Identity;
            this.Mass = 1.0f;
        }

        /// <summary>
        /// Gets the axis aligned bounding box of the orientated shape. This includes
        /// the whole shape.
        /// </summary>
        /// <param name="orientation">The orientation of the shape.</param>
        /// <param name="box">The axis aligned bounding box of the shape.</param>
        public override void GetBoundingBox(ref JMatrix orientation, out JBBox box)
        {
            box = boundings;

#region Expand Spherical
            box.Min.X -= sphericalExpansion;
            box.Min.Y -= sphericalExpansion;
            box.Min.Z -= sphericalExpansion;
            box.Max.X += sphericalExpansion;
            box.Max.Y += sphericalExpansion;
            box.Max.Z += sphericalExpansion;
#endregion

            box.Transform(ref orientation);
        }

        public override void MakeHull(ref List<JVector> triangleList, int generationThreshold)
        {
            for (int index = 0; index < (heightsLength0 - 1) * (heightsLength1 - 1); index++)
            {
                int quadIndexX = index % (heightsLength0 - 1);
                int quadIndexZ = index / (heightsLength0 - 1);

                triangleList.Add(new JVector((0 + quadIndexX + 0) * scaleX, heights[0 + quadIndexX + 0, 0 + quadIndexZ + 0], (0 + quadIndexZ + 0) * scaleZ));
                triangleList.Add(new JVector((0 + quadIndexX + 1) * scaleX, heights[0 + quadIndexX + 1, 0 + quadIndexZ + 0], (0 + quadIndexZ + 0) * scaleZ));
                triangleList.Add(new JVector((0 + quadIndexX + 0) * scaleX, heights[0 + quadIndexX + 0, 0 + quadIndexZ + 1], (0 + quadIndexZ + 1) * scaleZ));

                triangleList.Add(new JVector((0 + quadIndexX + 1) * scaleX, heights[0 + quadIndexX + 1, 0 + quadIndexZ + 0], (0 + quadIndexZ + 0) * scaleZ));
                triangleList.Add(new JVector((0 + quadIndexX + 1) * scaleX, heights[0 + quadIndexX + 1, 0 + quadIndexZ + 1], (0 + quadIndexZ + 1) * scaleZ));
                triangleList.Add(new JVector((0 + quadIndexX + 0) * scaleX, heights[0 + quadIndexX + 0, 0 + quadIndexZ + 1], (0 + quadIndexZ + 1) * scaleZ));
            }
        }

        /// <summary>
        /// SupportMapping. Finds the point in the shape furthest away from the given direction.
        /// Imagine a plane with a normal in the search direction. Now move the plane along the normal
        /// until the plane does not intersect the shape. The last intersection point is the result.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <param name="result">The result.</param>
        public override void SupportMapping(ref JVector direction, out JVector result)
        {
            JVector expandVector;
            JVector.Normalize(ref direction, out expandVector);
            JVector.Multiply(ref expandVector, sphericalExpansion, out expandVector);

            int minIndex = 0;
            float min = JVector.Dot(ref points[0], ref direction);
            float dot = JVector.Dot(ref points[1], ref direction);
            if (dot > min)
            {
                min = dot;
                minIndex = 1;
            }
            dot = JVector.Dot(ref points[2], ref direction);
            if (dot > min)
            {
                min = dot;
                minIndex = 2;
            }

            JVector.Add(ref points[minIndex], ref expandVector, out result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rayOrigin"></param>
        /// <param name="rayDelta"></param>
        /// <returns></returns>
        public override int Prepare(ref JVector rayOrigin, ref JVector rayDelta)
        {
            JBBox box = JBBox.SmallBox;

#region RayEnd + Expand Spherical
            JVector rayEnd;
            JVector.Normalize(ref rayDelta, out rayEnd);
            rayEnd = rayOrigin + rayDelta + rayEnd * sphericalExpansion;
#endregion

            box.AddPoint(ref rayOrigin);
            box.AddPoint(ref rayEnd);

            return this.Prepare(ref box);
        }
    }
}
