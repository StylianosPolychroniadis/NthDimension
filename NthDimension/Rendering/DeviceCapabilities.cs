using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Rendering
{
    public class DeviceCapabilities
    {
        public int Renderer_MAX_TEXTURE_SIZE            = 0;
        public int Renderer_DEPTH_BITS                  = 0;
        public int Renderer_RED_BITS                    = 0;
        public int Renderer_GREEN_BITS                  = 0;
        public int Renderer_BLUE_BITS                   = 0;
        public int Renderer_ALPHA_BITS                  = 0;
        public int Renderer_STENCIL_BITS                = 0;
        public int Shader_MAX_VERTEX_ATTRIBS            = 0;
        public int Shader_MAX_VERTEX_UNIFORMS           = 0;
        public int Shader_MAX_FRAGMENT_UNIFORMS         = 0;
        public int Shader_MAX_VARYING_COMPONENTS        = 0;    // Deprecated???
        public int Shader_MAX_VERTEX_TEXTURE_UNITS      = 0;
        public int Shader_MAX_FRAGMENT_TEXTURE_UNITS    = 0;
    }
}
