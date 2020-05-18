using System;

namespace Plugin.StarGen
{
    /// <summary> Implements a Star object for building alternative solar systems.
    /// </summary>
    public class Star : Blackbody
    {
        private const double GREENHOUSE_EFFECT_CONST = (0.93); // affects inner radius..
        private const double B = (1.2e-5); // Used in Crit_mass calc

        public const System.String CLASS_CODE = "OBAFGKM";

        // NOTE: Stellar temperatures were calculated using some pretty rough-and-ready measures,
        // so they could be quite far off (particularly at the high and low ends)

        // SPECT - TEMPERATURE TABLE BY SPECTRAL CLASS (old version)
        //UPGRADE_NOTE: Final was removed from the declaration of 'OLDSPECT'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
        private static readonly double[] OLDSPECT = new double[] { 25000.0, 23600.0, 22200.0, 20800.0, 19400.0, 18000.0, 16600.0, 15200.0, 13800.0, 12400.0, 11000.0, 10650.0, 10300.0, 9950.0, 9600.0, 9250.0, 8900.0, 8550.0, 8200.0, 7850.0, 7500.0, 7350.0, 7200.0, 7050.0, 6900.0, 6750.0, 6600.0, 6450.0, 6300.0, 6150.0, 6000.0, 5900.0, 5800.0, 5700.0, 5600.0, 5500.0, 5400.0, 5300.0, 5200.0, 5100.0, 5000.0, 4850.0, 4700.0, 4550.0, 4400.0, 4250.0, 4100.0, 3950.0, 3800.0, 3650.0, 3500.0, 3150.0, 2800.0, 2450.0, 2100.0, 1750.0, 1400.0, 1050.0, 700.0, 350.0 };

        // SPECT - TEMPERATURE TABLE BY SPECTRAL PLAN_CLASS (new version) 
        //UPGRADE_NOTE: Final was removed from the declaration of 'SPECT'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
        private static readonly double[] SPECT = new double[] { 25000.0, 23600.0, 22200.0, 20800.0, 19400.0, 18000.0, 16600.0, 15200.0, 13800.0, 12400.0, 11000.0, 10650.0, 10300.0, 9950.0, 9600.0, 9250.0, 8900.0, 8550.0, 8200.0, 7850.0, 7500.0, 7350.0, 7200.0, 7050.0, 6900.0, 6750.0, 6600.0, 6450.0, 6300.0, 6150.0, 6000.0, 5900.0, 5800.0, 5700.0, 5600.0, 5500.0, 5400.0, 5300.0, 5200.0, 5100.0, 5000.0, 4850.0, 4700.0, 4550.0, 4400.0, 4250.0, 4100.0, 3950.0, 3800.0, 3650.0, 3500.0, 3200.0, 2900.0, 2600.0, 2300.0, 2000.0, 1700.0, 1400.0, 1100.0, 800.0 };

        // BC - 'BOLOMETRIC CORRECTION', ESTABLISHED BY CALCULATION OF RATIO OF
        // OVERALL ENERGY TO VISIBLE ENERGY, AND SHIFTING THE MAGNITUDES TO AGREE
        // WITH G2 STAR AT G2 TEMPERATURE. ADD THIS TO VISUAL MAGNITUDE 
        // CURVE HAS BEEN MANUALLY TRIMMED AT THE LOWER END 
        //UPGRADE_NOTE: Final was removed from the declaration of 'BC'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
        private static readonly double[] BC = new double[] { 1.8770e+0, 1.7344e+0, 1.5864e+0, 1.4329e+0, 1.2737e+0, 1.1089e+0, 9.3888e-1, 7.6439e-1, 5.8704e-1, 4.0991e-1, 2.3869e-1, 1.9790e-1, 1.5835e-1, 1.2030e-1, 8.4102e-2, 5.0144e-2, 1.8896e-2, -9.0779e-3, -3.3103e-2, -5.2363e-2, -6.5867e-2, -6.9601e-2, -7.1950e-2, -7.2793e-2, -7.1999e-2, -6.9424e-2, -6.4909e-2, -5.8280e-2, -4.9344e-2, -3.7890e-2, -2.3682e-2, -1.2553e-2, -8.8050e-8, 1.4066e-2, 2.9742e-2, 4.7134e-2, 6.6354e-2, 8.7522e-2, 1.1077e-1, 1.3624e-1, 1.6409e-1, 2.1068e-1, 2.6363e-1, 3.2363e-1, 3.9149e-1, 4.6814e-1, 5.5465e-1, 6.5224e-1, 7.6234e-1, 8.8663e-1, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0 };

        /// <summary>
        /// Stellar class code
        /// </summary>
        public int ST; // stellar class code
        /// <summary>
        /// Absolute visual magnitude
        /// </summary>
        public double VM; // absolute visual magnitude
        public double LUM; // luminosity
        public double EM; // emission
        /// <summary>
        /// Stellar masses
        /// </summary>
        public double SM; // Stellar masses
        public double AU; // putative distance for 1 "AU"
        public double main_seq_life;
        public double age;
        public double radius;
        public double r_ecosphere;
        public double r_greenhouse;

        /// <summary> Creates a string representation of the star's 'class code'.
        /// For Star objects built with the 'starform'-style constructor,
        /// the results are undefined.  At some future time, additional
        /// code may be added to calculate main sequence class given the
        /// stellar mass; that day is not today.
        /// </summary>
        public string CODE;

        public virtual System.String classCode()
        {
            return CLASS_CODE.Substring(ST / 10 + 1, (ST / 10 + 2) - (ST / 10 + 1)) + System.Convert.ToString(ST % 10);
        }

        /// <summary> Performs calculations common to both constructors.</summary>
        protected internal virtual void commonConstructor()
        {
            main_seq_life = 1.0e10 * (SM / LUM);
            if ((main_seq_life >= 6.0e9))
                age = random_number(1.0e9, 6.0e9);
            else
                age = random_number(1.0e9, main_seq_life);
            radius = RADIUS();
            r_ecosphere = System.Math.Sqrt(LUM);
            r_greenhouse = r_ecosphere * GREENHOUSE_EFFECT_CONST;
        }

        public Star()
        {

        }

        /// <summary> Makes a new star given the integer 'Class Code'.  There are 60 classes
        /// which can be handled (B0 through M9); 'O' class stars and non-main-sequence
        /// stars are not currently handled.
        /// </summary>
        /// <param name="StellarClassCode">Integer code (0-59) corresponding to stellar
        /// class (B0 - M9)
        /// </param>
        public Star(int StellarClassCode)
        {
            ST = StellarClassCode;
            TEMP = SPECT[ST];
            VM = MAINSEQ() - BC[ST];
            LUM = LUMINOSITY();
            EM = LUM;
            SM = MASS();
            AU = AUSTAR(SM);
            TEMP += NormalDeviate() * (TEMP / 200.0);
            commonConstructor();
            CODE = classCode();
        }

        /// <summary> Constructor using the 'starform' star generation method.  That method
        /// involves a simple selection of stellar mass ratio relative to Sol,
        /// which allows the user to focus on stars likely to have habitable planets.
        /// </summary>
        /// <param name="StellarMassRatio">Ratio of star's mass to Sol
        /// </param>
        public Star(double StellarMassRatio)
        {
            // ought to guess at stellar class here...
            // also need to estimate temperature here...
            SM = StellarMassRatio;
            LUM = luminosity();
            EM = LUM;
            AU = AUSTAR(SM);
        }

        /// <summary> Calculate absolute visual magnitude of a main sequence star given
        /// the temperature.  Generated from table of stellar magnitudes by
        /// fitting to a cubic curve; there are some known problems with the
        /// fit, e.g. the curve isn't steep enough at the high and low ends,
        /// but I [Carl] don't want to work through it yet again.
        /// </summary>
        /// <returns>s Absolute visual magnitude
        /// </returns>
        public virtual double MAINSEQ()
        {
            double A0 = 26.281;
            double A1 = -3.4495;
            double A2 = -0.77271;
            double A3 = 0.076305;
            double LTEMP;

            LTEMP = System.Math.Log(TEMP / 100.0); // scaled by 100 as an artifact of the fit
            return A0 + A1 * LTEMP + A2 * LTEMP * LTEMP + A3 * LTEMP * LTEMP * LTEMP;
        }

        /// <summary> Calculates stellar luminosity based on visual magnitude.</summary>
        /// <returns>s Luminosity of star relative to Sol
        /// </returns>
        public virtual double LUMINOSITY()
        {
            return System.Math.Exp((4.7 - VM) / 2.5);
        }

        /// <summary> Calculates stellar mass using mass-luminosity relationship.</summary>
        /// <returns>s Mass in Solar masses
        /// </returns>
        public virtual double MASS()
        {
            return System.Math.Exp(System.Math.Log(LUM) / 3.5);
        }

        /// <summary> Calculates stellar radius using luminosity and temperature.
        /// I [Carl] forget where I got this; maybe derived from an astronomy text?
        /// </summary>
        /// <returns>s Radius in Solar radii
        /// </returns>
        public virtual double RADIUS()
        {
            return Math.Sqrt(LUM) * Math.Pow(6100.0 / TEMP, 2);
        }

        /// <summary> Calculates distance to farthest edge of proplyd.
        /// I [Carl] am unsure where this calculation is derived from.
        /// </summary>
        /// <returns>s Maximum dust distance in AU
        /// </returns>
        public virtual double stellar_dust_limit()
        {
            //return (250.0 * System.Math.Pow(SM, (1.0 / 3.0)));
            return 250 * SM;
        }

        public virtual double roche_limit(Star primary, Planet innermost_planet)
        {
            double d;
            d = primary.radius * Math.Pow(
                                            ((primary.MASS() * PhysicalConstants.SUN_MASS_IN_EARTH_MASSES / (4 / 3 * Math.PI * Math.Pow(primary.radius, 3))) /
                                            innermost_planet.density)
                                         , 1 / 3);
            return d;
        }

        /// <summary> Calculates distance to nearest possible planetary orbit.
        /// I [Carl] am unsure where this calculation is derived from.
        /// </summary>
        /// <returns>s Minimum planet distance in AU
        /// </returns>
        public virtual double nearest_planet()
        {
            return (0.3 * System.Math.Pow(SM, (1.0 / 3.0)));
        }

        /// <summary> Calculates distance to farthest possible planetary orbit.
        /// I [Carl] am unsure where this calculation is derived from.
        /// </summary>
        /// <returns>s Maximum planet distance in AU
        /// </returns>
        public virtual double farthest_planet()
        {
            //return (50.0 * System.Math.Pow(SM, (1.0 / 3.0)));
            return (60.0 * SM);
        }

        /// <summary> Calculates the mass at which a protoplanet orbiting this star
        /// will accrete gas as well as dust.
        /// </summary>
        /// <param name="orb_radius">Semi-major axis of protoplanet
        /// </param>
        /// <param name="eccentricity">Orbital eccentricity of protoplanet
        /// </param>
        /// <returns>s Critical mass of protoplanet, in Solar masses
        /// </returns>
        public virtual double critical_limit(double orb_radius, double eccentricity)
        {
            double perihelion_dist = (orb_radius - orb_radius * eccentricity);
            double temp = perihelion_dist * System.Math.Sqrt(LUM);
            return (B * System.Math.Pow(temp, -0.75));
        }

        /// <summary> Estimates luminosity of star.  Probably generated through curve fitting.</summary>
        /// <returns>s Luminosity of star
        /// </returns>
        public virtual double luminosity()
        {
            double n;

            if (SM < 1.0)
                n = 1.75 * (SM - 0.1) + 3.325;
            else
                n = 0.5 * (2.0 - SM) + 4.4;
            return (System.Math.Pow(SM, n));
        }

        /// <summary> Calculates the 'orbital zone' of a particle (1, 2, or 3) based
        /// on distance from the star.  Orbital zone is a rough measure of
        /// composition for dust particles.
        /// </summary>
        /// <param name="orb_radius">Distance fom star in AU
        /// </param>
        /// <returns>s Zone of particle (1, 2, or 3)
        /// </returns>
        public virtual int orb_zone(double orb_radius)
        {
            if (orb_radius < (4.0 * System.Math.Sqrt(LUM)))
                return (1);
            else
            {
                if ((orb_radius >= (4.0 * System.Math.Sqrt(LUM))) && (orb_radius < (15.0 * System.Math.Sqrt(LUM))))
                    return (2);
                else
                    return (3);
            }
        }

        /// <summary> Provides an estimation of 1 'Astronomical Unit' for other planets
        /// around other stars. Purely invented.
        /// </summary>
        /// <param name="MASS">Mass in Solar masses
        /// </param>
        public virtual double AUSTAR(double MASS)
        {
            // an alternative approach is to use r_ecosphere, which is 1 AU for Sol.
            return 1.0 / System.Math.Sqrt(1.0 / MASS);
        }

        /// <summary> Provides an estimation of 1 'Astronomical Unit' for moons
        /// around other planets. Purely invented.
        /// </summary>
        /// <param name="MASS">Mass in Earth masses
        /// </param>
        public virtual double AUPLAN(double MASS)
        {
            return 150000000.0 / (6.8 * System.Math.Sqrt(333000.0 / MASS));
        }
    }
}
