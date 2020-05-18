using NthDimension.Algebra;
using NthDimension.Rendering.Geometry;
using NthDimension.Rendering.Shaders;

namespace NthDimension.Rendering.Drawables.Models
{
    public class AvatarHairModel : Model
    {
        public AvatarHairModel(PlayerModel parent) : base(parent)
        {
            this.Renderlayer = RenderLayer.Solid;
        }

        //protected override void setSpecialUniforms(ref Shader curShader, ref Mesh CurMesh) // Added Mar-12-18
        //{
        //    base.setSpecialUniforms(ref curShader, ref CurMesh);
        //}
    }
}
