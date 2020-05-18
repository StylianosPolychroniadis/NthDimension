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

using NthDimension.Rasterizer;

namespace NthDimension.Rendering.Drawables.Framebuffers
{
    using NthDimension.Algebra;


    public class DefaultFramebuffer : Framebuffer
    {
        public DefaultFramebuffer(Vector2 size, FramebufferCreator parent)
        {
            Parent = parent;
            this.size = size;
        }

        public override void enable(bool wipe)
        {
            ApplicationBase.Instance.CheckGlError(string.Format("(!) OpenGL Error: Enable {0}", this.GetType()));

            ApplicationBase.Instance.Renderer.BindFramebuffer(FramebufferTarget.FramebufferExt, 0); // disable rendering into the FBO

            ApplicationBase.Instance.Renderer.Viewport(0, 0, (int)(size.X), (int)(size.Y));

            if (wipe)
            {
                ApplicationBase.Instance.Renderer.ClearColor(ClearColor);
                ApplicationBase.Instance.Renderer.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            }

            //Game.Instance.Renderer.Enable(EnableCap.Texture2D); // enable Texture Mapping
            //Game.Instance.Renderer.BindTexture(TextureTarget.Texture2D, 0); // bind default texture

            ApplicationBase.Instance.VAR_ScreenSize_Current = size;

            ApplicationBase.Instance.CheckGlError(string.Format("(!) OpenGL Error: Enable {0}", this.GetType()));
        }
    }
}
