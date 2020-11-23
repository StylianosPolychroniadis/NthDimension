using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Rendering.Drawables.Models.Vehicle
{
    public class BmwX5 : CarModel
    {

        public BmwX5(ApplicationObject parent, 
                     VehicleController controller, 
                     string path, 
                     int color, 
                     ref Loaders.MeshLoader meshLoader)
            : base(parent, controller, path, color, ref meshLoader)
        {

        }
    }
}
