using NthDimension.Algebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.Citigen
{
    public static class Mathf
    {
        public static float Sqrt(float d)
        {
            return (float)Math.Sqrt(d);
        }

        public static float Deg2Rad
        {
            get
            {
                return MathHelper.DegreesToRadians(1.0f);
            }
        }

        public static float Atan(float d)
        {
            return (float)Math.Atan(d);
        }
    }
}
