using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.Aircraft
{
    public class PropulsionPanel
    {
        public static void InitSample()
        {
            double takeoffAltitude_Ft = 0.0d;
            double throttle = 1.0d;
            double throttleMax = 1.0d;
            double throttleMin = 0.0d;
            double fuelHV_Btulbm = 0.2d;
            double flameTemperature_R = 1000; //??
            double compressionRatio = 0.47; //???
            double fanPressureRatio = 0.1; //??
            double maxDecel_P_Ratio = 1.2; //??
            double bypassRatio = 0.65; //???

            if (throttle > throttleMax) throttle = throttleMax;
            if (throttle < throttleMin) throttle = throttleMin;
            //Turbofan t = new Turbofan(takeoffAltitude_Ft,
            //                            throttle,
            //                            fuelHV_Btulbm,
            //                            flameTemperature_R,
            //                            compressionRatio,
            //                            fanPressureRatio,
            //                            maxDecel_P_Ratio,
            //                            bypassRatio,
            //    );
        }
    }
}
