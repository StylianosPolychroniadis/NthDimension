using NthDimension.Algebra;
using NthDimension.Rasterizer;
using NthDimension.Rendering.Configuration;
using NthDimension.Rendering.Drawables.Framebuffers;
using NthDimension.Rendering.Drawables.Lights;
using NthDimension.Rendering.GameViews;
using NthDimension.Rendering.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Rendering
{
    #region enum ShaderTypes
    public enum enuShaderFilters
    {
        ssaoPreShader,
        ssaoShader,
        ssaoBlrShader,
        ssaoBlrShaderA,
        bloomCurveShader,
        bloomShader,
        dofpreShader,
        dofShader,
        composite,
        ssaoBlendShader,
        copycatShader,
        wipingShader,
        reflectionShader,
        lightBlurShader
    }
    #endregion
}
