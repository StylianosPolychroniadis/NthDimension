using NthDimension.Rendering.Drawables.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS0067
#pragma warning disable CS0169

namespace NthDimension.Rendering.Drawables.Level
{
    using NthDimension.Rendering.Drawables.Models;
    public class WallWindowModel : StaticModel
    {
        Template template;
        public WallWindowModel(ApplicationObject parent) : base(parent)
        {

        }
    }
}
