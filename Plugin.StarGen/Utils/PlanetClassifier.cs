using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.StarGen
{
    /// <summary> Given a Planet object, a PlanetClassifier will supply descriptive
    /// text and a graphical image representing the planet.  You need one
    /// PlanetClassifier for each classification scheme you want to implement.
    /// Override the methods of this class to change the scheme, or reimplement
    /// the class to produce general behavior instead of the hard-wired stuff here.
    /// </summary>
    public class PlanetClassifier //: ImageObserver
    {


        /// <summary> Returns a string describing the planet in general terms.</summary>
        /// <param name="p">Planet object to be described
        /// </param>
        /// <returns>s Constant string description
        /// </returns>
        public virtual System.String planetType(Planet p)
        {
            if (p.mass < 0.01)
                return "Asteroidal";
            if (p.gas_giant)
            {
                //I use effective temp based on distance from sun, becuse gas giants don't
                //have a surface, and thus don't have surface temperature
                string hot = (p.eff_temp(p.r_ecosphere) > 500) ? "Hot, " : "";
                if (p.mass < 50.0)
                    return hot + "Small gas giant";
                else if (p.mass > 1000.0)
                    return hot + "Brown dwarf";
                else
                    return hot + "Large gas giant";
            }
            else
            {
                if ((p.age > 1e+9) // changed from 2.7, it's in megayears
                    && (p.a > 0.8 * p.r_ecosphere)
                    && (p.a < 1.2 * p.r_ecosphere)
                    && (p.day < 96.0)
                    && (p.surf_temp > (273.15 - 30.0)) //changed from -1     Even if average temp not suitable, on equator/poles can be habitable
                    && (p.surf_temp < (273.15 + 40.0)) //changed from +30
                    && (p.surf_pressure > 360)         //changed from 0.36 ? 
                    && (p.surf_pressure < 2600.0)
                    && ((p.ice_cover + p.hydrosphere < 1.0)
                    && (p.hydrosphere > 0.0)))
                    return "Habitable";
                if ((p.mass > 0.4) && (p.a > 0.65 * p.r_ecosphere) && (p.a < 1.35 * p.r_ecosphere) && (p.surf_temp > (273.15 - 45.0)) && (p.surf_pressure > 0.05) && (p.surf_pressure < 8000.0) && ((p.ice_cover > 0.0) || (p.hydrosphere > 0.0)))
                    return "Marginally habitable";
                if (p.ice_cover > 0.95)
                    return "Iceworld";
                if (p.hydro_fraction() > 0.95
                    && p.avg_temp > PhysicalConstants.FREEZING_POINT_OF_WATER
                    && p.avg_temp < p.boil_point)
                    return "Waterworld";
                if (p.surf_temp < 100.0)
                    return "Frigid airless rock";
                if (p.surf_pressure < 0.01)
                {
                    if (p.surf_temp < 273.0)
                        return "Cold airless rock";
                    return "Hot airless rock";
                }
                else if (p.surf_pressure > 10000.0)
                {
                    if (p.surf_temp > 273.0)
                        return "Hot, dense atmosphere";
                    else
                        return "Cold, dense atmosphere";
                }
                else
                {
                    if (p.surf_temp > (((p.boil_point - 273.0) / 2.0) + 273.0))
                        return "Hot terrestrial";
                    if (p.surf_temp < 273.0)
                        return "Frozen terrestrial";
                    return "Terrestrial";
                }
            }
        }
    }
}
