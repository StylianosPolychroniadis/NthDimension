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

namespace NthDimension.Rasterizer.Windows
{
    using NthDimension.Algebra;
    public partial class RendererGL3x : RendererBaseGL3
    {
        public override void PolygonOffset(float factor, float units)
        {
            OpenTK.Graphics.OpenGL.GL.PolygonOffset(factor, units);
        }
        public override void Frustum(float left, float right, float bottom, float top, float near, float far)
        {
            OpenTK.Graphics.OpenGL.GL.Frustum(left, right, bottom, top, near, far);
        }
        public override void LoadIdentity()
        {
            OpenTK.Graphics.OpenGL.GL.LoadIdentity();
        }
        public override void MatrixMode(MatrixMode mode)
        {
            OpenTK.Graphics.OpenGL.GL.MatrixMode(mode.ToOpenTK());
        }
        public override void MultMatrix(ref Matrix4 matrix)
        {
            OpenTK.Matrix4 ret = matrix.ToOpenTK();
            OpenTK.Graphics.OpenGL.GL.MultMatrix(ref ret);
            matrix = ret.ToNthDimension();
        }
        public override void Translate(Vector3 translation)
        {
            OpenTK.Graphics.OpenGL.GL.Translate(translation.ToOpenTK());
        }
        public override void Immediate_DrawLine(Vector3 start, Vector3 end, Color4 color, int linewidth = 1)
        {
            OpenTK.Graphics.OpenGL.GL.LineWidth(linewidth);
            OpenTK.Graphics.OpenGL.GL.Color4(color.R, color.G, color.B, color.A);
            OpenTK.Graphics.OpenGL.GL.Begin(OpenTK.Graphics.OpenGL.PrimitiveType.Lines);
            OpenTK.Graphics.OpenGL.GL.Vertex3(start.ToOpenTK());
            OpenTK.Graphics.OpenGL.GL.Vertex3(end.ToOpenTK());
            OpenTK.Graphics.OpenGL.GL.End();
        }

        public override void CopyTexImage2D(TextureTarget target, int level, PixelInternalFormat internalFormat, int x, int y, int width, int height, int border)
        {
            OpenTK.Graphics.OpenGL.GL.CopyTexImage2D(target.ToOpenTK(), level, internalFormat.ToOpenTK(), x, y, width, height, border);
        }
    }
}
