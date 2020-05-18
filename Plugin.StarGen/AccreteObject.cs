using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.StarGen
{
    /// <summary> This class handles elements which are global to every object
    /// in the simulation.  In particular, it provides consistent access
    /// to the central random number generator.
    /// </summary>
    public class AccreteObject
    {

        /// <summary> Produces a Gaussian random variate with mean=0, standard deviation=1.
        /// Provides a local method with access to global random number generator.
        /// </summary>
        static public double NormalDeviate()
        {
            return cr.NormalDeviate();
        }

        /// <summary> Produces a random variate whose natural logarithm is from the
        /// Gaussian with mean=0 and the specified standard deviation.
        /// Provides a local method with access to global random number generator.
        /// </summary>
        /// <param name="sigma">Standard deviation
        /// </param>
        static public double LognormalDeviate(double sigma)
        {
            return cr.LognormalDeviate(sigma);
        }

        /// <summary> Returns a uniformly distributed random real number between the specified
        /// inner and outer bounds.
        /// Provides a local method with access to global random number generator.
        /// </summary>
        /// <param name="inner">Minimum value desired
        /// </param>
        /// <param name="outer">Maximum value desired
        /// </param>
        static public double random_number(double inner, double outer)
        {
            return cr.random_number(inner, outer);
        }

        /// <summary>   Returns a value within a certain uniform variation
        /// from the central value.
        /// Provides a local method with access to global random number generator.
        /// </summary>
        /// <param name="value">Central value
        /// </param>
        /// <param name="variation">Maximum (uniform) variation above or below center
        /// </param>
        static public double about(double value_Renamed, double variation)
        {
            return cr.about(value_Renamed, variation);
        }

        /// <summary> Returns a value for orbital eccentricity between 0.0 and 1.0.
        /// Provides a local method with access to global random number generator.
        /// </summary>
        static public double random_eccentricity()
        {
            return cr.random_eccentricity();
        }

        /// <summary> Returns a pseudo-random value between 0.0 and 1.0.
        /// Provides a local method with access to global random number generator.
        /// </summary>
        static public double nextDouble()
        {
            return cr.NextDouble();
        }

        public static CustomRandom cr;
        static AccreteObject()
        {
            {
                cr = new CustomRandom();
            }
        }
    }
}
