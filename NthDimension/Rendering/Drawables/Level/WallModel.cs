using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Rendering.Drawables.Level
{
    using NthDimension.Rendering.Drawables.Models;

    
    public class WallModel : StaticModel
    {
#pragma warning disable CS0067
#pragma warning disable CS0169
        Template template;
        public WallModel(ApplicationObject parent) : base(parent)
        {

        }
    }
}
