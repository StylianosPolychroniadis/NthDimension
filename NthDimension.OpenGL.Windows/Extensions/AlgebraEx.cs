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

using OpenTK.Graphics;

namespace NthDimension.Rasterizer.Windows
{
    using OpenTK;
    public static class AlgebraEx
    {
        #region Color4 Extensions
        public static OpenTK.Graphics.Color4 ToOpenTK(this NthDimension.Algebra.Color4 color)
        {
            return new Color4(color.R, color.G, color.B, color.A);
        }
        public static NthDimension.Algebra.Color4 ToNthDimension(this OpenTK.Graphics.Color4 color)
        {
            return new NthDimension.Algebra.Color4(color.R, color.G, color.B, color.A);
        }
        #endregion

        #region Vector2 Extensions
        public static OpenTK.Vector2 ToOpenTK(this NthDimension.Algebra.Vector2 vec)
        {
            return new OpenTK.Vector2(vec.X, vec.Y);
        }
        public static NthDimension.Algebra.Vector2 ToNthDimension(this OpenTK.Vector2 vec)
        {
            return new Algebra.Vector2(vec.X, vec.Y);
        }

        #endregion

        #region Vector3 Extensions 
        public static OpenTK.Vector3 ToOpenTK(this NthDimension.Algebra.Vector3 vec)
        {
            return new OpenTK.Vector3(vec.X, vec.Y, vec.Z);
        }
        public static NthDimension.Algebra.Vector3 ToNthDimension(this OpenTK.Vector3 vec)
        {
            return new Algebra.Vector3(vec.X, vec.Y, vec.Z);
        }


        #endregion

        #region Vector4 Extensions
        public static OpenTK.Vector4 ToOpenTK(this NthDimension.Algebra.Vector4 vec)
        {
            return new OpenTK.Vector4(vec.X, vec.Y, vec.Z, vec.W);
        }
        public static NthDimension.Algebra.Vector4 ToNthDimension(this OpenTK.Vector4 vec)
        {
            return new Algebra.Vector4(vec.X, vec.Y, vec.Z, vec.W);
        }


        #endregion

        #region Vector2d Extensions
        public static OpenTK.Vector2d ToOpenTK(this NthDimension.Algebra.Vector2d vec)
        {
            return new OpenTK.Vector2d(vec.X, vec.Y);
        }
        public static NthDimension.Algebra.Vector2d ToNthDimension(this OpenTK.Vector2d vec)
        {
            return new Algebra.Vector2d(vec.X, vec.Y);
        }

        #endregion

        #region Vector3d Extensions 
        public static OpenTK.Vector3d ToOpenTK(this NthDimension.Algebra.Vector3d vec)
        {
            return new OpenTK.Vector3d(vec.X, vec.Y, vec.Z);
        }
        public static NthDimension.Algebra.Vector3d ToNthDimension(this OpenTK.Vector3d vec)
        {
            return new Algebra.Vector3d(vec.X, vec.Y, vec.Z);
        }


        #endregion

        #region Vector4d Extensions
        public static OpenTK.Vector4d ToOpenTK(this NthDimension.Algebra.Vector4d vec)
        {
            return new OpenTK.Vector4d(vec.X, vec.Y, vec.Z, vec.W);
        }
        public static NthDimension.Algebra.Vector4d ToNthDimension(this OpenTK.Vector4d vec)
        {
            return new Algebra.Vector4d(vec.X, vec.Y, vec.Z, vec.W);
        }


        #endregion

        #region Matrix4 Extensions

        //public static OpenTK.Matrix3 ToOpenTK(this Rafa.Algebra.Matrix3 mat)
        //{
        //    return new Matrix3(mat.Row0.ToOpenTK(),
        //                       mat.Row1.ToOpenTK(),
        //                       mat.Row2.ToOpenTK());
        //}

        //public static Rafa.Algebra.Matrix3 ToRafa(this OpenTK.Matrix3 mat)
        //{
        //    return new Algebra.Matrix3(mat.Row0.ToRafa(),
        //                               mat.Row1.ToRafa(),
        //                                mat.Row2.ToRafa());
        //}
        //#endregion

        //#region Matrix4d Extensions

        //public static OpenTK.Matrix3d ToOpenTK(this Rafa.Algebra.Matrix3d mat)
        //{
        //    return new Matrix3d(mat.Row0.ToOpenTK(),
        //                        mat.Row1.ToOpenTK(),
        //                        mat.Row2.ToOpenTK());
        //}

        //public static Rafa.Algebra.Matrix3d ToRafa(this OpenTK.Matrix3d mat)
        //{
        //    return new Algebra.Matrix3d(mat.Row0.ToRafa(),
        //                                mat.Row1.ToRafa(),
        //                                mat.Row2.ToRafa());

        //}
        #endregion

        #region Matrix4 Extensions

        public static OpenTK.Matrix4 ToOpenTK(this NthDimension.Algebra.Matrix4 mat)
        {
            return new Matrix4(mat.Row0.ToOpenTK(),
                               mat.Row1.ToOpenTK(),
                               mat.Row2.ToOpenTK(),
                               mat.Row3.ToOpenTK());
        }

        public static NthDimension.Algebra.Matrix4 ToNthDimension(this OpenTK.Matrix4 mat)
        {
            return new Algebra.Matrix4(mat.Row0.ToNthDimension(),
                mat.Row1.ToNthDimension(),
                mat.Row2.ToNthDimension(),
                mat.Row3.ToNthDimension());
        }
        #endregion

        #region Matrix4d Extensions

        public static OpenTK.Matrix4d ToOpenTK(this NthDimension.Algebra.Matrix4d mat)
        {
            return new Matrix4d(mat.Row0.ToOpenTK(),
                               mat.Row1.ToOpenTK(),
                               mat.Row2.ToOpenTK(),
                               mat.Row3.ToOpenTK());
        }

        public static NthDimension.Algebra.Matrix4d ToNthDimension(this OpenTK.Matrix4d mat)
        {
            return new Algebra.Matrix4d(mat.Row0.ToNthDimension(),
                mat.Row1.ToNthDimension(),
                mat.Row2.ToNthDimension(),
                mat.Row3.ToNthDimension());
        }
        #endregion

    }
}
