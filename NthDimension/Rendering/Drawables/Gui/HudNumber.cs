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
using NthDimension.Rendering.Shaders;
using NthDimension.Rasterizer;
//using NthDimension.OpenGL.GLSL.API3x;
#if _WINDOWS_
//using NthDimension.OpenGL.Windows.GLSL.API3x;
#endif

namespace NthDimension.Rendering.Drawables.Gui
{
    public class HudNumber : GuiElement
    {
        public int digits = 6;

        public HudNumber(Gui parent)
            : base(parent)
        {
            setMaterial("number.xmf");
        }

        public override void draw()
        {
            if (IsVisible)
            {
                //gameWindow.checkGlError("--uncaught ERROR--");

                Shader shader = activateMaterial(ref materials[0]);

                //GL.DepthMask(false);

                shader.InsertUniform(Uniform.in_hudvalue, ref elementValue);
                shader.InsertUniform(Uniform.in_hudsize, ref screenSize);
                shader.InsertUniform(Uniform.in_hudpos, ref screenPosition);
                shader.InsertUniform(Uniform.in_hudcolor, ref color);

                //GL.Uniform1(curShader.timeLocation, 1, ref mGameWindow.FrameTime);
                //GL.Uniform1(curShader.passLocation, 1, ref mGameWindow.currentPass);

#if _WINDOWS_
                ApplicationBase.Instance.Renderer.BindVertexArray(vaoHandle[0]);
#endif
                for (int i = 0; i < digits; i++)
                {
                    Vector2 positon = new Vector2(screenSize.X * (digits - 2 * i - 1), 0f) + screenPosition;
                    shader.InsertUniform(Uniform.in_hudpos, ref positon);

                    float value = elementValue / (float)System.Math.Pow(10, i);
                    shader.InsertUniform(Uniform.in_hudvalue, ref value);

#if _WINDOWS_
                    ApplicationBase.Instance.Renderer.DrawElements(PrimitiveType, meshes[0].MeshData.Indices.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);
#endif

                    //gameWindow.checkGlError("--Drawing ERROR Hud--");
                }
                //Game.Instance.Renderer.BindVertexArray(0);
            }
        }
    }
}
