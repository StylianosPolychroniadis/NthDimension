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
using NthDimension.Rendering.Geometry;

namespace NthDimension.Rendering.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using NthDimension.Algebra;
    using  NthDimension.Rasterizer;
    public class RayHelper
    {

        public static Algebra.Vector4 Project(Algebra.Vector4 objPos, Algebra.Matrix4 projection, Algebra.Matrix4 view, System.Drawing.Size viewport)
        {
            Algebra.Vector4 vec = objPos;
            vec = Algebra.Vector4.Transform(vec, Algebra.Matrix4.Mult(projection, view));
            vec.X = (vec.X + 1) * ((float)viewport.Width / 2);
            vec.Y = (vec.Y + 1) * ((float)viewport.Height / 2);
            return vec;
        }

        public static Algebra.Vector3 UnProject(Algebra.Matrix4 projection, Algebra.Matrix4 view,
            System.Drawing.Size viewport, Algebra.Vector3 mouse)
        {
            Algebra.Vector4 vec;

            vec.X =   2.0f * mouse.X / (float)viewport.Width  - 1;
            vec.Y = -(2.0f * mouse.Y / (float)viewport.Height - 1);
            vec.Z = mouse.Z;//f; // was 0f
            vec.W = 1f;

            Algebra.Matrix4 viewInv = Algebra.Matrix4.Invert(view);
            Algebra.Matrix4 projInv = Algebra.Matrix4.Invert(projection);

            Algebra.Vector4.Transform(ref vec, ref projInv, out vec);
            Algebra.Vector4.Transform(ref vec, ref viewInv, out vec);

            if (vec.W > float.Epsilon || vec.W < float.Epsilon)
            {
                vec.X /= vec.W;
                vec.Y /= vec.W;
                vec.Z /= vec.W;
            }

            return vec.Xyz;
        }

        public static Ray CreateRay(int x, int y, Algebra.Vector3 origin, Matrix4 modelViewMatrix, Matrix4 projMatrix)
        {



            Matrix4 modelViewMatrix_inv = modelViewMatrix.Inverted();



            Vector3 rayOrigin = new Vector3((float)modelViewMatrix_inv.ExtractTranslation().X,
                                            (float)modelViewMatrix_inv.ExtractTranslation().Y,
                                            (float)modelViewMatrix_inv.ExtractTranslation().Z);


            


            int[] view = new int[4];
            ApplicationBase.Instance.Renderer.GetInteger(GetPName.Viewport, view);
           

            Vector3 screen_space;

            // device space to normalized screen space (NDC)
            screen_space.X = (((2.0f * (float)x) / view[2] - view[0]) - 1) / projMatrix.M11; //.right.X;
            screen_space.Y = -(((2.0f * (float)y) / view[3] - view[1]) - 1) / projMatrix.M22;
            screen_space.Z = -1.0f;

            Matrix4 m4 = new Matrix4((float)modelViewMatrix_inv.M11,
                                    (float)modelViewMatrix_inv.M12,
                                    (float)modelViewMatrix_inv.M13,
                                    (float)modelViewMatrix_inv.M14,
                                    (float)modelViewMatrix_inv.M21,
                                    (float)modelViewMatrix_inv.M22,
                                    (float)modelViewMatrix_inv.M23,
                                    (float)modelViewMatrix_inv.M24,
                                    (float)modelViewMatrix_inv.M31,
                                    (float)modelViewMatrix_inv.M32,
                                    (float)modelViewMatrix_inv.M33,
                                    (float)modelViewMatrix_inv.M34,
                                    (float)modelViewMatrix_inv.M41,
                                    (float)modelViewMatrix_inv.M42,
                                    (float)modelViewMatrix_inv.M43,
                                    (float)modelViewMatrix_inv.M44);

            Vector3 rayDir = Vector3.TransformVector(screen_space, m4);

            rayDir.Normalize();

            return new Ray(rayOrigin, rayDir);

        }
    }
    









}
