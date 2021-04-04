using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Windowing.Desktop
{
    /// <summary>
    ///  Defines the interface for GLFW OpenGL context management.
    /// </summary>
    public interface IGLFWGraphicsContext : IGraphicsContext
    {
        /// <summary>
        /// The GLFW Window that represents the context.
        /// </summary>
        unsafe IntPtr WindowPtr { get; }
    }
}
