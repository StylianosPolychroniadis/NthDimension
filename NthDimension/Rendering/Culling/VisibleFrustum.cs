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

using NthDimension.Algebra;
using NthDimension.Rendering.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Rendering.Culling
{
    public class VisibleFrustum : BoundingFrustum
    {
        #region Properties
        public Matrix4 view = new Matrix4();
        public Matrix4 ViewMatrix
        {
            get { return view; }
        }
        public Matrix4 projection = new Matrix4();
        public Matrix4 ProjectionMatrix
        {
            get { return projection; }
        }

        Matrix4 billboard = new Matrix4();
        public Matrix4 BillboardMatrix
        {
            get { return billboard; }
        }

        bool zIsUp = true;
        public bool ZIsUp
        {
            get { return ZIsUp; }
            set { zIsUp = ZIsUp; BuildFrustum(); }
        }
        #endregion

        #region Protected Variables
        protected Vector3 RightVec = new Vector3();
        protected Vector3 Up = new Vector3();
        protected Vector3 ViewDir = new Vector3();
        protected Vector3 EyePoint = new Vector3();

        protected float nearClip = 0;
        protected float farClip = 0;

        protected Vector3[] edge;
        #endregion

        #region Public Constructors

        public VisibleFrustum() : base(Matrix4.Identity)
        {
            //BuildFrustum();
        }

        public VisibleFrustum(VisibleFrustum value)
            : base(value.Matrix)
        {
            projection = new Matrix4(value.projection.Row0, value.projection.Row1, value.projection.Row2, value.projection.Row3);
            view = new Matrix4(value.view.Row0, value.view.Row1, value.view.Row2, value.view.Row3);
            Matrix = new Matrix4(value.Matrix.Row0, value.Matrix.Row1, value.Matrix.Row2, value.Matrix.Row3);
            billboard = new Matrix4(value.billboard.Row0, value.billboard.Row1, value.billboard.Row2, value.billboard.Row3);

            zIsUp = value.zIsUp;
            nearClip = value.nearClip;
            farClip = value.farClip;

            ViewDir = new Vector3(value.ViewDir);
            EyePoint = new Vector3(value.EyePoint);
            RightVec = new Vector3(value.RightVec);
            Up = new Vector3(value.Up);

            this.LeftPlane = new Plane(value.LeftPlane.Normal, value.LeftPlane.D);
            this.RightPlane = new Plane(value.RightPlane.Normal, value.RightPlane.D);
            this.TopPlane = new Plane(value.TopPlane.Normal, value.TopPlane.D);
            this.BottomPlane = new Plane(value.BottomPlane.Normal, value.BottomPlane.D);
            this.NearPlane = new Plane(value.NearPlane.Normal, value.NearPlane.D);
            this.FarPlane = new Plane(value.FarPlane.Normal, value.FarPlane.D);

            edge = new Vector3[4];
            edge[0] = new Vector3(value.edge[0]);
            edge[1] = new Vector3(value.edge[1]);
            edge[2] = new Vector3(value.edge[2]);
            edge[3] = new Vector3(value.edge[3]);
        }
        #endregion

        #region Public Methods

        public void SetProjection(float fov, float aspect, float hither, float yon, int width, int height)
        {
            nearClip = hither;
            farClip = yon;

            // compute projectionMatrix
            float s = 1.0f / (float)System.Math.Tan(fov / 2.0f);
            float fracHeight = 1.0f - (float)height / (float)height;
            projection.M11 = s;
            projection.M22 = (1.0f - fracHeight) * s * (float)width / (float)height;
            projection.M31 = 0.0f;
            projection.M32 = -fracHeight;
            projection.M33 = -(yon + hither) / (yon - hither);
            projection.M34 = -1.0f;
            projection.M41 = 0.0f;
            projection.M43 = -2.0f * yon * hither / (yon - hither);
            projection.M44 = 0.0f;

            projection.Transpose();

            BuildFrustum();
        }

        public void SetView(Matrix4 mat)
        {
            view = mat;
            BuildFrustum();
        }

        public void LookAt(Vector3 eye, Vector3 target)
        {
            EyePoint = new Vector3(eye);

            // compute forward vector and normalize
            ViewDir = GenericMethods.Subtract(target, eye);
            ViewDir.Normalize();

            if (!zIsUp)
                throw new NotImplementedException();

            // compute left vector (by crossing forward with
            // world-up [0 0 1]T and normalizing)
            RightVec.X = ViewDir.Y;
            RightVec.Y = -ViewDir.X;
            float rd = 1.0f / GenericMethods.Hypot(RightVec.X, RightVec.Y);
            RightVec.X *= rd;
            RightVec.Y *= rd;
            RightVec.Z = 0.0f;

            // compute local up vector (by crossing right and forward,
            // normalization unnecessary)
            Up.X = RightVec.Y * ViewDir.Z;
            Up.Y = -RightVec.X * ViewDir.Z;
            Up.Z = (RightVec.X * ViewDir.Y) - (RightVec.Y * ViewDir.X);

            // build view matrix, including a transformation bringing
            // world up [0 0 1 0]T to eye up [0 1 0 0]T, world north
            // [0 1 0 0]T to eye forward [0 0 -1 0]T.
            GenericMethods.m0(ref view, RightVec.X);
            GenericMethods.m4(ref view, RightVec.Y);
            GenericMethods.m8(ref view, 0.0f);

            GenericMethods.m1(ref view, Up.X);
            GenericMethods.m5(ref view, Up.Y);
            GenericMethods.m9(ref view, Up.Z);

            GenericMethods.m2(ref view, -ViewDir.X);
            GenericMethods.m6(ref view, -ViewDir.Y);
            GenericMethods.m10(ref view, -ViewDir.Z);

            GenericMethods.m12(ref view, -(GenericMethods.m0(view) * eye.X +
                               GenericMethods.m4(view) * eye.Y +
                               GenericMethods.m8(view) * eye.Z));
            GenericMethods.m13(ref view, -(GenericMethods.m1(view) * eye.X +
                               GenericMethods.m5(view) * eye.Y +
                               GenericMethods.m9(view) * eye.Z));
            GenericMethods.m14(ref view, -(GenericMethods.m2(view) * eye.X +
                               GenericMethods.m6(view) * eye.Y +
                               GenericMethods.m10(view) * eye.Z));

            GenericMethods.m15(ref view, 1.0f);

            GenericMethods.M11(ref billboard, GenericMethods.M11(view));
            GenericMethods.M12(ref billboard, GenericMethods.M21(view));
            GenericMethods.M13(ref billboard, GenericMethods.M31(view));
            GenericMethods.M21(ref billboard, GenericMethods.M12(view));
            GenericMethods.M22(ref billboard, GenericMethods.M22(view));
            GenericMethods.M23(ref billboard, GenericMethods.M32(view));
            GenericMethods.M31(ref billboard, GenericMethods.M13(view));
            GenericMethods.M32(ref billboard, GenericMethods.M23(view));
            GenericMethods.M33(ref billboard, GenericMethods.M33(view));

            BuildFrustum();

        }
        #endregion

        #region Protected Methods

        public void BuildFrustum()
        {
            // save off the composite matrix to the base
            base.Matrix = Matrix4.Mult(view, projection);

            // compute vectors of frustum edges
            float xs = (float)System.Math.Abs(1.0f / projection.Column0.X);
            float ys = (float)System.Math.Abs(1.0f / projection.Column1.Y);
            edge = new Vector3[4];

            edge[0] = ViewDir - (xs * RightVec) - (ys * Up);
            edge[1] = ViewDir + (xs * RightVec) - (ys * Up);
            edge[2] = ViewDir + (xs * RightVec) + (ys * Up);
            edge[3] = ViewDir - (xs * RightVec) + (ys * Up);

            // make frustum planes
            this.NearPlane.Normal = ViewDir;
            this.NearPlane.D = -Vector3.Dot(EyePoint, ViewDir);

            makePlane(edge[0], edge[3], EyePoint, ref this.m_leftPlane);
            makePlane(edge[2], edge[1], EyePoint, ref this.m_rightPlane);
            makePlane(edge[1], edge[0], EyePoint, ref this.m_bottomPlane);
            makePlane(edge[3], edge[2], EyePoint, ref this.m_topPlane);

            this.FarPlane.Normal = -NearPlane.Normal;
            this.FarPlane.D = NearPlane.D + farClip;

            CreateCorners();
        }

        protected void makePlane(Vector3 v1, Vector3 v2, Vector3 eye, ref Plane plane)
        {
            if (plane == null)
                plane = new Plane();

            // get normal by crossing v1 and v2 and normalizing
            plane.Normal = Vector3.Cross(v1, v2);
            plane.Normal.Normalize();
            plane.D = -Vector3.Dot(eye, plane.Normal);
        }
        #endregion
    }
}
