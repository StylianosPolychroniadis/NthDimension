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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace NthDimension.Rasterizer.Windows
{
    public partial class RendererGL3x : RendererBaseGL3
    {
        public override void RenderbufferStorageEXT(RenderbufferTarget target, RenderbufferStorage internalformat, Int32 width, Int32 height)
        {
            OpenTK.Graphics.OpenGL.GL.RenderbufferStorage(target.ToOpenTK(), internalformat.ToOpenTK(), width, height);
        }

        public override void ReadBuffer(ReadBufferMode src)
        {
            OpenTK.Graphics.OpenGL.GL.ReadBuffer(src.ToOpenTK());
        }

        public override void BlitFramebuffer(int srcX0, int srcY0, int srcX1, int srcY1, int dstX0, int dstY0, int dstX1, int dstY1, ClearBufferMask mask, BlitFramebufferFilter filter )
        {
            OpenTK.Graphics.OpenGL.GL.BlitFramebuffer(srcX0, srcY0, srcX1, srcY1, dstX0, dstY0, dstX1, dstY1, mask.ToOpenTK(), filter.ToOpenTK());
        }

        public override void DrawBuffer(DrawBufferMode buf)
        {
            OpenTK.Graphics.OpenGL.GL.DrawBuffer(buf.ToOpenTK());
        }
    }
}
