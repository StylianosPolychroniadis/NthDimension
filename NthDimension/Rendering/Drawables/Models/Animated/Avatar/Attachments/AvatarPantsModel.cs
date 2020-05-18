using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NthDimension.Algebra;
using NthDimension.Rendering.Geometry;
using NthDimension.Rendering.Shaders;

namespace NthDimension.Rendering.Drawables.Models
{
    public class AvatarPantsModel : AnimatedModel
    {

        public AvatarPantsModel(PlayerModel parent) : base(parent)
        {
             
        }

        protected override void setSpecialUniforms(ref Shader curShader, ref MeshVbo CurMesh)  // Added Mar-12-18
        {
            base.setSpecialUniforms(ref curShader, ref CurMesh);

            if (curShader.Loaded)
            {
                Vector4 skinColor = new Vector4(0f, 0f, 0f, 0f);
                curShader.InsertUniform(Uniform.in_skinColor, ref skinColor);

            }
        }
    }
}
