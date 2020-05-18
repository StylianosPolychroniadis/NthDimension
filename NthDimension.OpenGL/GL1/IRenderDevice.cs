using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using SYSCON.Graphics.Geometry;

namespace NthDimension.Rasterizer.GL1
{
    public interface IRenderDevice : IDisposable
    {
        
        void BeginFixedPipeline3D(enuPolygonMode beginMode);
        void EndFixedPipeline3D();
        void BeginFixedPipelineOrtho(Rectangle clientRectangle);
        void EndFixedPipelineOrtho();
    }
}
