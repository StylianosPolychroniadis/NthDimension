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
using NthDimension.Algebra;

namespace NthDimension.Rasterizer
{
    public partial class RendererBaseGL3
    {
        public virtual void Immediate_DrawLine(Vector3 start, Vector3 end, Color4 color, int lineWidth = 1) { }

        public virtual void PolygonOffset(float factor, float units) { }
        public virtual void Frustum(float left, float right, float bottom, float top, float near, float far) { }
        public virtual void LoadIdentity() { }
        public virtual void MatrixMode(MatrixMode mode) { }
        public virtual void MultMatrix(ref Matrix4 matrix) { }
        public virtual void Translate(Vector3 translation) { }

        public virtual void CopyTexImage2D(TextureTarget target, int level, PixelInternalFormat internalFormat, int x, int y, int width, int height, int border) {  }
    }
}
