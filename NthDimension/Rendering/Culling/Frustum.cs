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

// Nov-27-17
// from https://stackoverflow.com/questions/25821037/opentk-opengl-frustum-culling-clipping-too-soon

using System;
using NthDimension.Algebra;
using NthDimension.Rendering.Drawables.Models;

//using static NthDimension.Graphics.Drawables.Models.Planet.Terrain;
//using  NthDimension.Graphics.Drawables.Models.Planet.Terrain;

namespace NthDimension.Rendering.Culling
{
    //ToDo: Perform Merging (Nth+Planet)
    public class Frustum
    {
        public enum InFrustumCheck
        {
            IN = 0,
            INTERSECT = 1,
            OUT = 2
        }

        enum FrustumSide
        {
            RIGHT = 0,		// The RIGHT side of the frustum
            LEFT = 1,		// The LEFT	 side of the frustum
            BOTTOM = 2,		// The BOTTOM side of the frustum
            TOP = 3,		// The TOP side of the frustum
            BACK = 4,		// The BACK	side of the frustum
            FRONT = 5	    // The FRONT side of the frustum
        }

        enum PlaneData
        {
            A = 0,				// The X value of the plane's normal
            B = 1,				// The Y value of the plane's normal
            C = 2,				// The Z value of the plane's normal
            D = 3				// The distance the plane is from the origin
        }

        private readonly float[] _clipMatrix = new float[16];

        #region todo : merge
        private readonly float[,]       _frustum = new float[6, 4];
        private float[,]                FrustumModel = new float[6, 4];
        #endregion todo

        public const int A = 0;
        public const int B = 1;
        public const int C = 2;
        public const int D = 3;

        public enum ClippingPlane : int
        {
            Right = 0,
            Left = 1,
            Bottom = 2,
            Top = 3,
            Back = 4,
            Front = 5
        }

        public Frustum()
        {
        }

        private void NormalizePlane(float[,] frustum, int side) // Generic
        {
            float magnitude = (float)System.Math.Sqrt((frustum[side, 0] * frustum[side, 0]) + (frustum[side, 1] * frustum[side, 1])
                                                + (frustum[side, 2] * frustum[side, 2]));
            frustum[side, 0] /= magnitude;
            frustum[side, 1] /= magnitude;
            frustum[side, 2] /= magnitude;
            frustum[side, 3] /= magnitude;
        }
        private void normalizePlane(float[,] frustum, int side) // Planet Side
        {
            float magnitude = (float)System.Math.Sqrt(frustum[side, (int)PlaneData.A] * frustum[side, (int)PlaneData.A] +
                                           frustum[side, (int)PlaneData.B] * frustum[side, (int)PlaneData.B] +
                                           frustum[side, (int)PlaneData.C] * frustum[side, (int)PlaneData.C]);

            frustum[side, (int)PlaneData.A] /= magnitude;
            frustum[side, (int)PlaneData.B] /= magnitude;
            frustum[side, (int)PlaneData.C] /= magnitude;
            frustum[side, (int)PlaneData.D] /= magnitude;
        }

        public bool PointVsFrustum(float x, float y, float z)
        {
            for (int i = 0; i < 6; i++)
            {
                if (this._frustum[i, 0] * x + this._frustum[i, 1] * y + this._frustum[i, 2] * z + this._frustum[i, 3] <= 0.0f)
                {
                    return false;
                }
            }
            return true;
        }

        public bool PointVsFrustum(Vector3 location)
        {
            for (int i = 0; i < 6; i++)
            {
                if (this._frustum[i, 0] * location.X + this._frustum[i, 1] * location.Y + this._frustum[i, 2] * location.Z + this._frustum[i, 3] <= 0.0f)
                {
                    return false;
                }
            }
            return true;
        }

        public bool SphereVsFrustum(float x, float y, float z, float radius)
        {
            for (int p = 0; p < 6; p++)
            {
                float d = _frustum[p, 0] * x + _frustum[p, 1] * y + _frustum[p, 2] * z + _frustum[p, 3];
                if (d <= -radius)
                {
                    return false;
                }
            }
            return true;
        }

        public bool SphereVsFrustum(Vector3 location, float radius)
        {
            for (int p = 0; p < 6; p++)
            {
                float d = _frustum[p, 0] * location.X + _frustum[p, 1] * location.Y + _frustum[p, 2] * location.Z + _frustum[p, 3];
                if (d <= -radius)
                {
                    return false;
                }
            }
            return true;
        }

        public bool VolumeVsFrustum(float x, float y, float z, float width, float height, float length)
        {
            for (int i = 0; i < 6; i++)
            {
                if (_frustum[i, A] * (x - width) + _frustum[i, B] * (y - height) + _frustum[i, C] * (z - length) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (x + width) + _frustum[i, B] * (y - height) + _frustum[i, C] * (z - length) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (x - width) + _frustum[i, B] * (y + height) + _frustum[i, C] * (z - length) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (x + width) + _frustum[i, B] * (y + height) + _frustum[i, C] * (z - length) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (x - width) + _frustum[i, B] * (y - height) + _frustum[i, C] * (z + length) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (x + width) + _frustum[i, B] * (y - height) + _frustum[i, C] * (z + length) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (x - width) + _frustum[i, B] * (y + height) + _frustum[i, C] * (z + length) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (x + width) + _frustum[i, B] * (y + height) + _frustum[i, C] * (z + length) + _frustum[i, D] > 0)
                    continue;
                return false;
            }
            return true;
        }

        //public bool VolumeVsFrustum(BoundingVolume volume)
        //{
        //    for (int i = 0; i < 6; i++)
        //    {
        //        if (_frustum[i, A] * (volume.X - volume.Width) + _frustum[i, B] * (volume.Y - volume.Height) + _frustum[i, C] * (volume.Z - volume.Length) + _frustum[i, D] > 0)
        //            continue;
        //        if (_frustum[i, A] * (volume.X + volume.Width) + _frustum[i, B] * (volume.Y - volume.Height) + _frustum[i, C] * (volume.Z - volume.Length) + _frustum[i, D] > 0)
        //            continue;
        //        if (_frustum[i, A] * (volume.X - volume.Width) + _frustum[i, B] * (volume.Y + volume.Height) + _frustum[i, C] * (volume.Z - volume.Length) + _frustum[i, D] > 0)
        //            continue;
        //        if (_frustum[i, A] * (volume.X + volume.Width) + _frustum[i, B] * (volume.Y + volume.Height) + _frustum[i, C] * (volume.Z - volume.Length) + _frustum[i, D] > 0)
        //            continue;
        //        if (_frustum[i, A] * (volume.X - volume.Width) + _frustum[i, B] * (volume.Y - volume.Height) + _frustum[i, C] * (volume.Z + volume.Length) + _frustum[i, D] > 0)
        //            continue;
        //        if (_frustum[i, A] * (volume.X + volume.Width) + _frustum[i, B] * (volume.Y - volume.Height) + _frustum[i, C] * (volume.Z + volume.Length) + _frustum[i, D] > 0)
        //            continue;
        //        if (_frustum[i, A] * (volume.X - volume.Width) + _frustum[i, B] * (volume.Y + volume.Height) + _frustum[i, C] * (volume.Z + volume.Length) + _frustum[i, D] > 0)
        //            continue;
        //        if (_frustum[i, A] * (volume.X + volume.Width) + _frustum[i, B] * (volume.Y + volume.Height) + _frustum[i, C] * (volume.Z + volume.Length) + _frustum[i, D] > 0)
        //            continue;
        //        return false;
        //    }
        //    return true;
        //}

        public bool VolumeVsFrustum(Vector3 location, float width, float height, float length)
        {
            for (int i = 0; i < 6; i++)
            {
                if (_frustum[i, A] * (location.X - width) + _frustum[i, B] * (location.Y - height) + _frustum[i, C] * (location.Z - length) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (location.X + width) + _frustum[i, B] * (location.Y - height) + _frustum[i, C] * (location.Z - length) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (location.X - width) + _frustum[i, B] * (location.Y + height) + _frustum[i, C] * (location.Z - length) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (location.X + width) + _frustum[i, B] * (location.Y + height) + _frustum[i, C] * (location.Z - length) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (location.X - width) + _frustum[i, B] * (location.Y - height) + _frustum[i, C] * (location.Z + length) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (location.X + width) + _frustum[i, B] * (location.Y - height) + _frustum[i, C] * (location.Z + length) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (location.X - width) + _frustum[i, B] * (location.Y + height) + _frustum[i, C] * (location.Z + length) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (location.X + width) + _frustum[i, B] * (location.Y + height) + _frustum[i, C] * (location.Z + length) + _frustum[i, D] > 0)
                    continue;
                return false;
            }
            return true;
        }

        public bool CubeVsFrustum(float x, float y, float z, float size)
        {
            for (int i = 0; i < 6; i++)
            {
                if (_frustum[i, A] * (x - size) + _frustum[i, B] * (y - size) + _frustum[i, C] * (z - size) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (x + size) + _frustum[i, B] * (y - size) + _frustum[i, C] * (z - size) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (x - size) + _frustum[i, B] * (y + size) + _frustum[i, C] * (z - size) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (x + size) + _frustum[i, B] * (y + size) + _frustum[i, C] * (z - size) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (x - size) + _frustum[i, B] * (y - size) + _frustum[i, C] * (z + size) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (x + size) + _frustum[i, B] * (y - size) + _frustum[i, C] * (z + size) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (x - size) + _frustum[i, B] * (y + size) + _frustum[i, C] * (z + size) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (x + size) + _frustum[i, B] * (y + size) + _frustum[i, C] * (z + size) + _frustum[i, D] > 0)
                    continue;
                return false;
            }
            return true;
        }

        public InFrustumCheck CubeInFrustum(PlanetModel.Terrain.TerrainPatch patch)
        {
            //return InFrustumCheck.IN;
            int totalIn = 0;
            // Test all 6 sides against all 8 corners
            for (int s = 0; s < 6; s++)
            {
                int inCount = 8;
                int cIn = 1;
                for (int c = 0; c < 8; c++)
                {
                    // test corner against all planes
                    if (FrustumModel[s, (int)PlaneData.A] * patch.BoundingBoxV[c].X + FrustumModel[s, (int)PlaneData.B] * patch.BoundingBoxV[c].Y + FrustumModel[s, (int)PlaneData.C] * patch.BoundingBoxV[c].Z + FrustumModel[s, (int)PlaneData.D] < 0)
                    {
                        cIn = 0;
                        inCount--;
                    }

                    // if all points where outside, return false
                    if (inCount == 0) { return InFrustumCheck.OUT; }

                    // check if all points where on the right side of the plane
                    totalIn += cIn;
                }
            }
            // if all points where inside, return true
            if (totalIn == 6) { return InFrustumCheck.IN; }
            // at this point the cube must be partly inside
            return InFrustumCheck.INTERSECT;
        }
        public InFrustumCheck CubeInFrustum(Vector3[] BoundingBox3D)
        {
            //return InFrustumCheck.IN;
            int totalIn = 0;
            // Test all 6 sides against all 8 corners
            for (int s = 0; s < 6; s++)
            {
                int inCount = 8;
                int cIn = 1;
                for (int c = 0; c < 8; c++)
                {
                    // test corner against all planes
                    if (FrustumModel[s, (int)PlaneData.A] * BoundingBox3D[c].X + FrustumModel[s, (int)PlaneData.B] * BoundingBox3D[c].Y + FrustumModel[s, (int)PlaneData.C] * BoundingBox3D[c].Z + FrustumModel[s, (int)PlaneData.D] < 0)
                    {
                        cIn = 0;
                        inCount--;
                    }

                    // if all points where outside, return false
                    if (inCount == 0) { return InFrustumCheck.OUT; }

                    // check if all points where on the right side of the plane
                    totalIn += cIn;
                }
            }
            // if all points where inside, return true
            if (totalIn == 6) { return InFrustumCheck.IN; }
            // at this point the cube must be partly inside
            return InFrustumCheck.INTERSECT;
        }

        /// <summary>
        /// Planet Terrain function
        /// </summary>
        public void CalculateFrustum()
        {
            float[] proj = new float[16];								// This will hold our projection matrix
            float[] modl = new float[16];								// This will hold our modelview matrix
            float[] clip = new float[16];                  				// This will hold the clipping planes

            throw new Exception("No projection and modelview matrices in Frustum.CalculateFrustum() for planetary terrain");
#pragma warning disable CS0162
            //// glGetFloatv() is used to extract information about our OpenGL Scene.
            //// Below, we pass in GL_PROJECTION_MATRIX to abstract our projection matrix.
            //// It then stores the matrix into an array of [16].
            //GL.GetFloat(GetPName.ProjectionMatrix, proj);



            //// By passing in GL_MODELVIEW_MATRIX, we can abstract our model view matrix.
            //// This also stores it in an array of [16].
            //GL.GetFloat(GetPName.ModelviewMatrix, modl);

            // Now that we have our modelview and projection matrix, if we combine these 2 matrices,
            // it will give us our clipping planes.  To combine 2 matrices, we multiply them.

            clip[0] = modl[0] * proj[0] + modl[1] * proj[4] + modl[2] * proj[8] + modl[3] * proj[12];
            clip[1] = modl[0] * proj[1] + modl[1] * proj[5] + modl[2] * proj[9] + modl[3] * proj[13];
            clip[2] = modl[0] * proj[2] + modl[1] * proj[6] + modl[2] * proj[10] + modl[3] * proj[14];
            clip[3] = modl[0] * proj[3] + modl[1] * proj[7] + modl[2] * proj[11] + modl[3] * proj[15];

            clip[4] = modl[4] * proj[0] + modl[5] * proj[4] + modl[6] * proj[8] + modl[7] * proj[12];
            clip[5] = modl[4] * proj[1] + modl[5] * proj[5] + modl[6] * proj[9] + modl[7] * proj[13];
            clip[6] = modl[4] * proj[2] + modl[5] * proj[6] + modl[6] * proj[10] + modl[7] * proj[14];
            clip[7] = modl[4] * proj[3] + modl[5] * proj[7] + modl[6] * proj[11] + modl[7] * proj[15];

            clip[8] = modl[8] * proj[0] + modl[9] * proj[4] + modl[10] * proj[8] + modl[11] * proj[12];
            clip[9] = modl[8] * proj[1] + modl[9] * proj[5] + modl[10] * proj[9] + modl[11] * proj[13];
            clip[10] = modl[8] * proj[2] + modl[9] * proj[6] + modl[10] * proj[10] + modl[11] * proj[14];
            clip[11] = modl[8] * proj[3] + modl[9] * proj[7] + modl[10] * proj[11] + modl[11] * proj[15];

            clip[12] = modl[12] * proj[0] + modl[13] * proj[4] + modl[14] * proj[8] + modl[15] * proj[12];
            clip[13] = modl[12] * proj[1] + modl[13] * proj[5] + modl[14] * proj[9] + modl[15] * proj[13];
            clip[14] = modl[12] * proj[2] + modl[13] * proj[6] + modl[14] * proj[10] + modl[15] * proj[14];
            clip[15] = modl[12] * proj[3] + modl[13] * proj[7] + modl[14] * proj[11] + modl[15] * proj[15];

            // Now we actually want to get the sides of the frustum.  To do this we take
            // the clipping planes we received above and extract the sides from them.

            // This will extract the RIGHT side of the frustum
            FrustumModel[(int)FrustumSide.RIGHT, (int)PlaneData.A] = (clip[3]) - clip[0];
            FrustumModel[(int)FrustumSide.RIGHT, (int)PlaneData.B] = (clip[7]) - clip[4];
            FrustumModel[(int)FrustumSide.RIGHT, (int)PlaneData.C] = (clip[11]) - clip[8];
            FrustumModel[(int)FrustumSide.RIGHT, (int)PlaneData.D] = (clip[15]) - clip[12];

            // Now that we have a normal (A,B,C) and a distance (D) to the plane,
            // we want to normalize that normal and distance.

            // Normalize the RIGHT side
            normalizePlane(FrustumModel, (int)FrustumSide.RIGHT);

            // This will extract the LEFT side of the frustum
            FrustumModel[(int)FrustumSide.LEFT, (int)PlaneData.A] = (clip[3]) + clip[0];
            FrustumModel[(int)FrustumSide.LEFT, (int)PlaneData.B] = (clip[7]) + clip[4];
            FrustumModel[(int)FrustumSide.LEFT, (int)PlaneData.C] = (clip[11]) + clip[8];
            FrustumModel[(int)FrustumSide.LEFT, (int)PlaneData.D] = (clip[15]) + clip[12];

            // Normalize the LEFT side
            normalizePlane(FrustumModel, (int)FrustumSide.LEFT);

            // This will extract the BOTTOM side of the frustum
            FrustumModel[(int)FrustumSide.BOTTOM, (int)PlaneData.A] = (clip[3]) + clip[1];
            FrustumModel[(int)FrustumSide.BOTTOM, (int)PlaneData.B] = (clip[7]) + clip[5];
            FrustumModel[(int)FrustumSide.BOTTOM, (int)PlaneData.C] = (clip[11]) + clip[9];
            FrustumModel[(int)FrustumSide.BOTTOM, (int)PlaneData.D] = (clip[15]) + clip[13];

            // Normalize the BOTTOM side
            normalizePlane(FrustumModel, (int)FrustumSide.BOTTOM);

            // This will extract the TOP side of the frustum
            FrustumModel[(int)FrustumSide.TOP, (int)PlaneData.A] = (clip[3]) - clip[1];
            FrustumModel[(int)FrustumSide.TOP, (int)PlaneData.B] = (clip[7]) - clip[5];
            FrustumModel[(int)FrustumSide.TOP, (int)PlaneData.C] = (clip[11]) - clip[9];
            FrustumModel[(int)FrustumSide.TOP, (int)PlaneData.D] = (clip[15]) - clip[13];

            // Normalize the TOP side
            normalizePlane(FrustumModel, (int)FrustumSide.TOP);

            // This will extract the BACK side of the frustum
            FrustumModel[(int)FrustumSide.BACK, (int)PlaneData.A] = (clip[3]) - clip[2];
            FrustumModel[(int)FrustumSide.BACK, (int)PlaneData.B] = (clip[7]) - clip[6];
            FrustumModel[(int)FrustumSide.BACK, (int)PlaneData.C] = (clip[11]) - clip[10];
            FrustumModel[(int)FrustumSide.BACK, (int)PlaneData.D] = (clip[15]) - clip[14];

            // Normalize the BACK side
            normalizePlane(FrustumModel, (int)FrustumSide.BACK);

            // This will extract the FRONT side of the frustum
            FrustumModel[(int)FrustumSide.FRONT, (int)PlaneData.A] = (clip[3]) + clip[2];
            FrustumModel[(int)FrustumSide.FRONT, (int)PlaneData.B] = (clip[7]) + clip[6];
            FrustumModel[(int)FrustumSide.FRONT, (int)PlaneData.C] = (clip[11]) + clip[10];
            FrustumModel[(int)FrustumSide.FRONT, (int)PlaneData.D] = (clip[15]) + clip[14];

            // Normalize the FRONT side
            normalizePlane(FrustumModel, (int)FrustumSide.FRONT);
        }
        public void CalculateFrustum(Matrix4 projectionMatrix, Matrix4 modelViewMatrix)
        {
            _clipMatrix[0] = (modelViewMatrix.M11 * projectionMatrix.M11) + (modelViewMatrix.M12 * projectionMatrix.M21) + (modelViewMatrix.M13 * projectionMatrix.M31) + (modelViewMatrix.M14 * projectionMatrix.M41);
            _clipMatrix[1] = (modelViewMatrix.M11 * projectionMatrix.M12) + (modelViewMatrix.M12 * projectionMatrix.M22) + (modelViewMatrix.M13 * projectionMatrix.M32) + (modelViewMatrix.M14 * projectionMatrix.M42);
            _clipMatrix[2] = (modelViewMatrix.M11 * projectionMatrix.M13) + (modelViewMatrix.M12 * projectionMatrix.M23) + (modelViewMatrix.M13 * projectionMatrix.M33) + (modelViewMatrix.M14 * projectionMatrix.M43);
            _clipMatrix[3] = (modelViewMatrix.M11 * projectionMatrix.M14) + (modelViewMatrix.M12 * projectionMatrix.M24) + (modelViewMatrix.M13 * projectionMatrix.M34) + (modelViewMatrix.M14 * projectionMatrix.M44);

            _clipMatrix[4] = (modelViewMatrix.M21 * projectionMatrix.M11) + (modelViewMatrix.M22 * projectionMatrix.M21) + (modelViewMatrix.M23 * projectionMatrix.M31) + (modelViewMatrix.M24 * projectionMatrix.M41);
            _clipMatrix[5] = (modelViewMatrix.M21 * projectionMatrix.M12) + (modelViewMatrix.M22 * projectionMatrix.M22) + (modelViewMatrix.M23 * projectionMatrix.M32) + (modelViewMatrix.M24 * projectionMatrix.M42);
            _clipMatrix[6] = (modelViewMatrix.M21 * projectionMatrix.M13) + (modelViewMatrix.M22 * projectionMatrix.M23) + (modelViewMatrix.M23 * projectionMatrix.M33) + (modelViewMatrix.M24 * projectionMatrix.M43);
            _clipMatrix[7] = (modelViewMatrix.M21 * projectionMatrix.M14) + (modelViewMatrix.M22 * projectionMatrix.M24) + (modelViewMatrix.M23 * projectionMatrix.M34) + (modelViewMatrix.M24 * projectionMatrix.M44);

            _clipMatrix[8] = (modelViewMatrix.M31 * projectionMatrix.M11) + (modelViewMatrix.M32 * projectionMatrix.M21) + (modelViewMatrix.M33 * projectionMatrix.M31) + (modelViewMatrix.M34 * projectionMatrix.M41);
            _clipMatrix[9] = (modelViewMatrix.M31 * projectionMatrix.M12) + (modelViewMatrix.M32 * projectionMatrix.M22) + (modelViewMatrix.M33 * projectionMatrix.M32) + (modelViewMatrix.M34 * projectionMatrix.M42);
            _clipMatrix[10] = (modelViewMatrix.M31 * projectionMatrix.M13) + (modelViewMatrix.M32 * projectionMatrix.M23) + (modelViewMatrix.M33 * projectionMatrix.M33) + (modelViewMatrix.M34 * projectionMatrix.M43);
            _clipMatrix[11] = (modelViewMatrix.M31 * projectionMatrix.M14) + (modelViewMatrix.M32 * projectionMatrix.M24) + (modelViewMatrix.M33 * projectionMatrix.M34) + (modelViewMatrix.M34 * projectionMatrix.M44);

            _clipMatrix[12] = (modelViewMatrix.M41 * projectionMatrix.M11) + (modelViewMatrix.M42 * projectionMatrix.M21) + (modelViewMatrix.M43 * projectionMatrix.M31) + (modelViewMatrix.M44 * projectionMatrix.M41);
            _clipMatrix[13] = (modelViewMatrix.M41 * projectionMatrix.M12) + (modelViewMatrix.M42 * projectionMatrix.M22) + (modelViewMatrix.M43 * projectionMatrix.M32) + (modelViewMatrix.M44 * projectionMatrix.M42);
            _clipMatrix[14] = (modelViewMatrix.M41 * projectionMatrix.M13) + (modelViewMatrix.M42 * projectionMatrix.M23) + (modelViewMatrix.M43 * projectionMatrix.M33) + (modelViewMatrix.M44 * projectionMatrix.M43);
            _clipMatrix[15] = (modelViewMatrix.M41 * projectionMatrix.M14) + (modelViewMatrix.M42 * projectionMatrix.M24) + (modelViewMatrix.M43 * projectionMatrix.M34) + (modelViewMatrix.M44 * projectionMatrix.M44);

            _frustum[(int)ClippingPlane.Right, 0] = _clipMatrix[3] - _clipMatrix[0];
            _frustum[(int)ClippingPlane.Right, 1] = _clipMatrix[7] - _clipMatrix[4];
            _frustum[(int)ClippingPlane.Right, 2] = _clipMatrix[11] - _clipMatrix[8];
            _frustum[(int)ClippingPlane.Right, 3] = _clipMatrix[15] - _clipMatrix[12];
            NormalizePlane(_frustum, (int)ClippingPlane.Right);

            _frustum[(int)ClippingPlane.Left, 0] = _clipMatrix[3] + _clipMatrix[0];
            _frustum[(int)ClippingPlane.Left, 1] = _clipMatrix[7] + _clipMatrix[4];
            _frustum[(int)ClippingPlane.Left, 2] = _clipMatrix[11] + _clipMatrix[8];
            _frustum[(int)ClippingPlane.Left, 3] = _clipMatrix[15] + _clipMatrix[12];
            NormalizePlane(_frustum, (int)ClippingPlane.Left);

            _frustum[(int)ClippingPlane.Bottom, 0] = _clipMatrix[3] + _clipMatrix[1];
            _frustum[(int)ClippingPlane.Bottom, 1] = _clipMatrix[7] + _clipMatrix[5];
            _frustum[(int)ClippingPlane.Bottom, 2] = _clipMatrix[11] + _clipMatrix[9];
            _frustum[(int)ClippingPlane.Bottom, 3] = _clipMatrix[15] + _clipMatrix[13];
            NormalizePlane(_frustum, (int)ClippingPlane.Bottom);

            _frustum[(int)ClippingPlane.Top, 0] = _clipMatrix[3] - _clipMatrix[1];
            _frustum[(int)ClippingPlane.Top, 1] = _clipMatrix[7] - _clipMatrix[5];
            _frustum[(int)ClippingPlane.Top, 2] = _clipMatrix[11] - _clipMatrix[9];
            _frustum[(int)ClippingPlane.Top, 3] = _clipMatrix[15] - _clipMatrix[13];
            NormalizePlane(_frustum, (int)ClippingPlane.Top);

            _frustum[(int)ClippingPlane.Back, 0] = _clipMatrix[3] - _clipMatrix[2];
            _frustum[(int)ClippingPlane.Back, 1] = _clipMatrix[7] - _clipMatrix[6];
            _frustum[(int)ClippingPlane.Back, 2] = _clipMatrix[11] - _clipMatrix[10];
            _frustum[(int)ClippingPlane.Back, 3] = _clipMatrix[15] - _clipMatrix[14];
            NormalizePlane(_frustum, (int)ClippingPlane.Back);

            _frustum[(int)ClippingPlane.Front, 0] = _clipMatrix[3] + _clipMatrix[2];
            _frustum[(int)ClippingPlane.Front, 1] = _clipMatrix[7] + _clipMatrix[6];
            _frustum[(int)ClippingPlane.Front, 2] = _clipMatrix[11] + _clipMatrix[10];
            _frustum[(int)ClippingPlane.Front, 3] = _clipMatrix[15] + _clipMatrix[14];
            NormalizePlane(_frustum, (int)ClippingPlane.Front);
        }
        public InFrustumCheck CubeInFrustum(Vector3 vCenter, float size)
        {
            for (int i = 0; i < 6; i++)
            {
                if (FrustumModel[i, (int)PlaneData.A] * (vCenter.X - size) + FrustumModel[i, (int)PlaneData.B] * (vCenter.Y - size) + FrustumModel[i, (int)PlaneData.C] * (vCenter.Z - size) + FrustumModel[i, (int)PlaneData.D] >= 0) continue;
                if (FrustumModel[i, (int)PlaneData.A] * (vCenter.X + size) + FrustumModel[i, (int)PlaneData.B] * (vCenter.Y - size) + FrustumModel[i, (int)PlaneData.C] * (vCenter.Z - size) + FrustumModel[i, (int)PlaneData.D] >= 0) continue;
                if (FrustumModel[i, (int)PlaneData.A] * (vCenter.X - size) + FrustumModel[i, (int)PlaneData.B] * (vCenter.Y + size) + FrustumModel[i, (int)PlaneData.C] * (vCenter.Z - size) + FrustumModel[i, (int)PlaneData.D] >= 0) continue;
                if (FrustumModel[i, (int)PlaneData.A] * (vCenter.X + size) + FrustumModel[i, (int)PlaneData.B] * (vCenter.Y + size) + FrustumModel[i, (int)PlaneData.C] * (vCenter.Z - size) + FrustumModel[i, (int)PlaneData.D] >= 0) continue;
                if (FrustumModel[i, (int)PlaneData.A] * (vCenter.X - size) + FrustumModel[i, (int)PlaneData.B] * (vCenter.Y - size) + FrustumModel[i, (int)PlaneData.C] * (vCenter.Z + size) + FrustumModel[i, (int)PlaneData.D] >= 0) continue;
                if (FrustumModel[i, (int)PlaneData.A] * (vCenter.X + size) + FrustumModel[i, (int)PlaneData.B] * (vCenter.Y - size) + FrustumModel[i, (int)PlaneData.C] * (vCenter.Z + size) + FrustumModel[i, (int)PlaneData.D] >= 0) continue;
                if (FrustumModel[i, (int)PlaneData.A] * (vCenter.X - size) + FrustumModel[i, (int)PlaneData.B] * (vCenter.Y + size) + FrustumModel[i, (int)PlaneData.C] * (vCenter.Z + size) + FrustumModel[i, (int)PlaneData.D] >= 0) continue;
                if (FrustumModel[i, (int)PlaneData.A] * (vCenter.X + size) + FrustumModel[i, (int)PlaneData.B] * (vCenter.Y + size) + FrustumModel[i, (int)PlaneData.C] * (vCenter.Z + size) + FrustumModel[i, (int)PlaneData.D] >= 0) continue;
                return InFrustumCheck.OUT;
            }
            return InFrustumCheck.IN;
        }
    }
}
