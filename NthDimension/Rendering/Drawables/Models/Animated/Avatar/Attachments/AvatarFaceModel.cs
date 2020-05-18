using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NthDimension.Algebra;
using NthDimension.Rendering.Geometry;
using NthDimension.Rendering.Shaders;

namespace NthDimension.Rendering.Drawables.Models
{
    public class AvatarFaceModel : Model
    {
        public AvatarFaceModel(PlayerModel parent) : base(parent)
        {
            
        }

        protected override void setSpecialUniforms(ref Shader curShader, ref MeshVbo CurMesh)  
        {
            base.setSpecialUniforms(ref curShader, ref CurMesh);

            Color SkinColor = ((PlayerModel)Parent).SkinColor;

            if (curShader.Loaded)
            {
                float r = (SkinColor.R / 255f);
                float g = (SkinColor.G / 255f);
                float b = (SkinColor.B / 255f);
                float a = 1f;   // Setting alpha to zero will force shader to skip color (defInfoFace.fs)

                Vector4 skinColor = new Vector4(r, g, b, a);
                

                curShader.InsertUniform(Uniform.in_skinColor, ref skinColor);

            }
        }

        public void Hide()
        {
            this.IsVisible = false;
        }
        public void Show()
        {
            this.IsVisible = true;
        }

    }
}
