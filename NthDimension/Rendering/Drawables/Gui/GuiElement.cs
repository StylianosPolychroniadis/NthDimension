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

using Shader = NthDimension.Rendering.Shaders.Shader;
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
    public class GuiElement : Gui
    {
        protected Vector2 screenSize = Vector2.One;
        protected Vector2 screenPosition = Vector2.Zero;

        protected float elementValue = 0f;

        public GuiElement(Gui parent)
        {
            Parent = parent;

            this.sizePx = parent.sizePx;

            setMaterial("firstperson\\crosshair.xmf");
            setMesh("base\\sprite_plane.obj");

            color = Gui.colorA;
        }

        public override void draw()
        {
            if (IsVisible)
            {
                Shaders.Shader shader = activateMaterial(ref materials[0]);

                //Game.Instance.Renderer.DepthMask(false);
                shader.InsertUniform(Uniform.in_hudvalue, ref elementValue);
                shader.InsertUniform(Uniform.in_hudsize, ref screenSize);
                shader.InsertUniform(Uniform.in_hudpos, ref screenPosition);
                shader.InsertUniform(Uniform.in_hudcolor, ref color);

                //if (ApplicationBase.Instance.VAR_ScreenSize_Virtual.X < ApplicationBase.Instance.VAR_ScreenSize_Current.X ||
                //    ApplicationBase.Instance.VAR_ScreenSize_Virtual.Y < ApplicationBase.Instance.VAR_ScreenSize_Current.Y)
                //    ApplicationBase.Instance.VAR_ScreenSize_Virtual = ApplicationBase.Instance.VAR_ScreenSize_Current;

                shader.InsertUniform(Uniform.in_screensize, ref ApplicationBase.Instance.VAR_ScreenSize_Virtual);
                shader.InsertUniform(Uniform.in_rendersize, ref ApplicationBase.Instance.VAR_ScreenSize_Current);


                //Game.Instance.Renderer.Uniform1(curShader.timeLocation, 1, ref mGame.FrameTime);
                //Game.Instance.Renderer.Uniform1(curShader.passLocation, 1, ref mGame.currentPass);

                ApplicationBase.Instance.Renderer.BindVertexArray(vaoHandle[0]);
                ApplicationBase.Instance.Renderer.DrawElements(PrimitiveType, meshes[0].MeshData.Indices.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);
                //Game.Instance.Renderer.BindVertexArray(0);
                drawChilds();
            }
        }

        public void setSizePix(Vector2 newSize)
        {
            Size = Vector2.Divide(newSize, ApplicationBase.Instance.VAR_ScreenSize_Virtual);
        }

        static Vector2 virtualScreenSize = new Vector2(1920, 1080);
        public void setSizeRel(Vector2 newSize)
        {
            Size = Vector2.Divide(newSize, virtualScreenSize);
        }

        public new virtual Vector2 Size { get { return screenSize; } set { screenSize = value; } }

        public new virtual Vector2 Position { get { return screenPosition; } set { screenPosition = value; } }

        public void setPositionPix(Vector2 newPos)
        {
            Position = Vector2.Divide(newPos, ApplicationBase.Instance.VAR_ScreenSize_Virtual);
        }

        public void setValue(float newValue)
        {
            this.elementValue = newValue;
        }
    }
}
