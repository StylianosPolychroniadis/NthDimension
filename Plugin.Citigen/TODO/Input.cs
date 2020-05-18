using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.Citigen
{
    public static class Input
    {
        public static bool GetMouseButton(int button)
        {
            return false;
        }
        public static float GetAxis(string axis)
        {
            switch(axis)
            {
                case "Mouse X":
                    return 0f;

                case "Mouse Y":
                    return 0f;


                default:
                    return 0f;
                    break;
            }
            return 0f;
        }

    }
}
