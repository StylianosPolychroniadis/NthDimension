//#define _SHADERDEBUG_

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NthDimension.Rendering.Shaders;

namespace NthDimension.Rendering.Factories
{

    public partial class ShaderLoader
    {
        public static List<Shader> UsedShaders = new List<Shader>();

        public static void UpdateShaderDebug(Shader shader)
        {
#if _SHADERDEBUG_
            if (!UsedShaders.Contains(shader))
                UsedShaders.Add(shader);
#endif

        }
    }

}
