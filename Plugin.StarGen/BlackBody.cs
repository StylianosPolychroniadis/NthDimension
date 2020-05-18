using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.StarGen
{
    /// <summary> A Blackbody represents a body which radiates energy in a predictable
    /// curve based solely on temperature.  Main sequence stars follow this
    /// curve fairly well, as do most planets.
    /// </summary>
    public class Blackbody : AccreteObject
    {
        //  Global constants

        public const double SIGMA = 5.669e-5;

        // EM radiation: approximate frequency ranges
        // radio: 10 - 1000m
        // TV: 1 - 10m
        // microwaves; 10e-3 - 1
        // IR: 7e-7 - 1e-3
        // vis: 4e-7 - 7e-7
        // UV: 1e-8 - 4e-7
        // Xray: 5e-12 - 1e-8
        // gamma: 1e-15 - 5e-12

        /// <summary> Temperature of the body, in degrees Kelvin.</summary>
        public double TEMP;

        /// <summary> Returns blackbody emission of this body in ergs
        /// per square centimeter by solving the emission equation
        /// at a specified wavelength.
        /// </summary>
        /// <param name="LAMBDA">Wavelength in angstroms
        /// </param>
        public virtual double E(double LAMBDA)
        // emission at wavelength
        {
            double TERM1, TERM2, /*LAMBDA1,*/ LAMBDA5;
            double H, K0, C;

            H = (6.626e-27); // ERG/S
            K0 = (1.380e-16); // ERG/DEG
            C = (2.997925e+10); // CM/S2
            TERM1 = 2.0 * H * C * C;
            LAMBDA5 = System.Math.Exp(System.Math.Log(LAMBDA * PhysicalConstants.ANGSTROM) * 5.0);
            TERM2 = H * C / (LAMBDA * PhysicalConstants.ANGSTROM * K0 * TEMP);
            return (TERM1 / LAMBDA5) * (1.0 / (System.Math.Exp(TERM2) - 1.0));
        }

        /// <summary> Returns visible light blackbody emission of this body in ergs
        /// per square centimeter by integrating the emission equation
        /// between 4,000 and 7,000 angstroms.
        /// </summary>
        public virtual double VISEMIT()
        {
            int I;
            double SUM = 0.0;

            for (I = 4000; I < 7000; I += 50)
            {
                SUM += 50.0 * PhysicalConstants.ANGSTROM * (E((double)I) + E((double)(I + 50.0))) / 2.0;
            }
            return SUM;
        }

        /// <summary> Returns infrared blackbody emission of this body in ergs
        /// per square centimeter by integrating the emission equation
        /// between 7,000 and 10,000,000 angstroms.
        /// </summary>
        public virtual double IREMIT()
        {
            int I;
            double SUM = 0.0;

            for (I = 7000; I < 10000; I += 50)
            // to 1e-6
            {
                SUM += 50.0 * PhysicalConstants.ANGSTROM * (E((double)I) + E((double)(I + 50.0))) / 2.0;
            }
            for (I = 10000; I < 100000; I += 500)
            // to 1e-5
            {
                SUM += 500.0 * PhysicalConstants.ANGSTROM * (E((double)I) + E((double)(I + 500.0))) / 2.0;
            }
            for (I = 100000; I < 1000000; I += 5000)
            // to 1e-4
            {
                SUM += 5000.0 * PhysicalConstants.ANGSTROM * (E((double)I) + E((double)(I + 5000.0))) / 2.0;
            }
            for (I = 1000000; I < 10000000; I += 50000)
            // to 1e-3
            {
                SUM += 50000.0 * PhysicalConstants.ANGSTROM * (E((double)I) + E((double)(I + 50000.0))) / 2.0;
            }
            return SUM;
        }

        /// <summary> Returns ultraviolet blackbody emission of this body in ergs
        /// per square centimeter by integrating the emission equation
        /// between 100 and 4,000 angstroms.
        /// </summary>
        public virtual double UVEMIT()
        {
            double SUM = 0.0;

            for (int I = 100; I < 4000; I += 20)
            {
                SUM += 20.0 * PhysicalConstants.ANGSTROM * (E((double)I) + E((double)(I + 20.0))) / 2.0;
            }
            return SUM;
        }

        /// <summary> Returns total blackbody emission of this body in ergs
        /// per square centimeter.
        /// </summary>
        public virtual double EMIT()
        {
            return SIGMA * (Math.Pow(TEMP, 4));
        }
    }
}
