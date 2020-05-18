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
using NthDimension.Rendering.Configuration;
using NthDimension.Rendering.Drawables;

namespace NthDimension.Rendering.GameViews
{
    // January-19-2019 Restored to match the original version
    public class ViewInfoSun : ViewInfo         
    {
        new public Vector3 position;
        float widith;

        public ViewInfoSun(ApplicationObject parent, float width, float height)
            : base(parent)
        {
            this.widith = width;
                           
            projectionMatrix = Matrix4.CreateOrthographic(width, width, -height * 0.5f, height * 0.5f);
        }

        public override void Update()
        {
            Vector3 pos                 = new Vector3(Scene.EyePos.X, 0, Scene.EyePos.Z);
            float texelSize             = 1f / Settings.Instance.video.shadowResolution;
            position                    = new Vector3((float)Math.Floor(pos.X / texelSize) * texelSize,
                                                (float)Math.Floor(pos.Y / texelSize) * texelSize - 1f,
                                                (float)Math.Floor(pos.Z / texelSize) * texelSize);

            PointingDirection           = Parent.PointingDirection; ;

            modelviewMatrix             = Matrix4.LookAt(position, position + PointingDirection, new Vector3(0,1,0));

            GenerateViewProjectionMatrix();
        }

        //public  bool frustrumCheck(Drawable drawable) // warning CS0114 (hides and does not override)
        //{
        //    //return true; // Feb-15-18 Debug

        //    Vector4 vSpacePos = GenericMethods.Mult(new Vector4(drawable.Position, 1), modelviewProjectionMatrix);

        //    float range = drawable.BoundingSphere * 2f / widith;

        //    if (float.IsNaN(range) || float.IsInfinity(range))
        //        return false;

        //    vSpacePos /= vSpacePos.W;

        //    return (
        //        vSpacePos.X < (1f + range) && vSpacePos.X > -(1f + range) &&
        //        vSpacePos.Y < (1f + range) && vSpacePos.Y > -(1f + range) &&
        //        vSpacePos.Z < (1f) && vSpacePos.Z > -(1f)
        //        );
        //}

    }
}
