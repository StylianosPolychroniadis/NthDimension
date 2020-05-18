using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.Aircraft
{
    public class Atmosphere : IDisposable
    {
        //// Output Values
        public double rho = 0;        // Air Density [slugs/ft^3]
        public double t = 0;          // Air Temperature [R]
        public double p = 0;          // Air Pressure [lb/ft^2]
        public double a = 0;          // Speed of Sound [ft/s]
        public double mu = 0;         // Viscosity [lb/(ft*s)]
        public double rho_SL = 0;     // Air Density [slugs/ft^3]
        public double t_SL = 0;       // Air Temperature [R]
        public double p_SL = 0;       // Air Pressure [lb/ft^2]
        public double a_SL = 0;       // Speed of Sound [ft/s]
        public double mu_SL = 0;      // Viscosity [lb/(ft*s)]

        //// Private Scientific Constants (READ ONLY)
        private double tSL = 518.67;                            // Air Temperature at Sea level [R]
        private double pSL = 2115.7159372471838;                // Air Pressure at Sea level [lb/ft^2]
        private double rhoSL = 2.3769 * Math.Pow(10, -3);       // Air Density at Sea level [slugs/ft^3]
        private double rhot = 7.0613 * Math.Pow(10, -4);        // Air Denisty at Tropopause [slugs/ft^3]
        private double rhoplus = 1.7083 * Math.Pow(10, -4);     // Air Density at Stratosphere [slugs/ft^3]
        private double ht = 36089;                              // Altitude of Tropopause [ft]
        private double hplus = 65617;                           // Height of Troposphere [ft]

        /// <summary>
        /// Creates and calculates an atmospheric model
        /// </summary>
        /// <param name="h">Height in ft</param>
        public Atmosphere(double h)
        {
            CalcATM(h);
        }

        /// <summary>
        /// Executes the atmosperic model calculator 
        /// </summary>
        /// <param name="h">Height in ft</param>
        public void CalcATM(double h)
        {
            if (h <= 36089) // Troposhpere Calculation
            {
                rho = rhoSL * Math.Exp(-h / 29730); // eq3.13 pg49
                t = tSL - (3.5662 * Math.Pow(10, -3)) * h; // eq3.4 pg 45
                p = (1.1376 * Math.Pow(10, -11)) * Math.Pow(t, 5.2560); // eq3.4 pg45
                rho_SL = rhoSL;
                t_SL = tSL;
                p_SL = pSL;
            }
            else if (h > 36089 && h < 65617) // Stratosphere I Calculation
            {
                rho = rhot * Math.Exp(-(h - ht) / 20806); // eq3.13 pg49
                t = 389.99; // eq3.7 -no t change
                p = 2678.4 * Math.Exp((-4.8063 * Math.Pow(10, -5)) * h);
            }
            else // Stratosphere II Calculation
            {
                rho = rhoplus * Math.Exp(-(h - hplus) / 20770); // eq3.13 pg49
                t = 389.99 + (5.4864 * Math.Pow(10, -4)) * (h - 65617); // eq3.9 pg46
                p = 3.7930E90 * Math.Pow(t, -34.164);
            }

            a = 49.021 * Math.Pow(t, (.5)); // Speed of Sound
            a_SL = 49.021 * Math.Pow(t_SL, (.5)); // Speed of Sound at Sea Level
            mu = ((.0000000227) * Math.Pow(t, 1.5)) / (t + 198.6); // Viscosity, Sutherlands Formula
            mu_SL = ((.0000000227) * Math.Pow(t_SL, 1.5)) / (t_SL + 198.6); // Viscosity, Sutherlands Formula at Sea Level
        }
        public void Dispose() { }
    }
}
