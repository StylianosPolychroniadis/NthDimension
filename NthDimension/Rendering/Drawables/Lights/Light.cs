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

using NthDimension.Rendering.Drawables.Models;
using NthDimension.Rendering.GameViews;

namespace NthDimension.Rendering.Drawables.Lights
{
    using NthDimension.Algebra;
    using Rendering.Drawables.Models;
    using NthDimension.Rasterizer;

#if _WINDOWS_
    //using NthDimension.OpenGL.Windows.GLSL.API3x;
#endif

    public class Light : Drawable
    {
        public bool Enabled = true;

        public ViewInfo viewInfo;

        public Matrix4 shadowMatrix;

        public static Matrix4 shadowBias = new Matrix4(0.5f, 0.0f, 0.0f, 0.0f,
                                                  0.0f, 0.5f, 0.0f, 0.0f,
                                                  0.0f, 0.0f, 0.5f, 0.0f,
                                                  0.5f, 0.5f, 0.5f, 1.0f);



        public LightVolume drawable;

        public float shadowQuality;

        public Light()
        {
            
        }


        public virtual void activate(Shaders.Shader shader, Drawable drawable) { }

        public virtual void activateDeferred(Shaders.Shader shader) { }

    }
}
