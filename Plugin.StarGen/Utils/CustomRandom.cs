using System;

namespace Plugin.StarGen
{
    /// <summary> This class provides a random number generator with additional
    /// access methods.
    /// </summary>
    [Serializable]
    public class CustomRandom : System.Random
    {
        protected internal bool normdone = false;
        protected internal double normstore;

        /// <summary> Default constructor.  Calls the constructor for Random with no seed.</summary>
        public CustomRandom()
            : base()
        {
        }

        /// <summary> Default constructor.  Calls the constructor for Random with the
        /// specified seed.
        /// </summary>
        public CustomRandom(long seed)
            : base((System.Int32)seed)
        {
        }

        /// <summary> Sets the random seed to the new value, performing additional housekeeping.</summary>
        /// <param name="seed">New pseudo-random generator seed
        /// </param>
        //UPGRADE_NOTE: The equivalent of method 'java.util.Random.setSeed' is not an override method. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1143'"
        public void setSeed(long s)
        {
            //UPGRADE_TODO: The differences in the expected value  of parameters for method 'java.util.Random.setSeed'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
            Random tmp = new System.Random((System.Int32)s);
            normdone = false;
        }

        /// <summary> Produces a Gaussian random variate with mean=0, standard deviation=1.</summary>
        public virtual double NormalDeviate()
        {
            double v1, v2, r, fac;

            if (normdone)
            {
                normdone = false;
                return normstore;
            }
            else
            {
                v1 = 2.0 * NextDouble() - 1.0;
                v2 = 2.0 * NextDouble() - 1.0;
                r = v1 * v1 + v2 * v2;
                if (r >= 1.0)
                {
                    return NormalDeviate();
                }
                else
                {
                    fac = (double)(System.Math.Sqrt((-2.0) * System.Math.Log(r) / (double)(r)));
                    normstore = v1 * fac;
                    normdone = true;
                    return v2 * fac;
                }
            }
        }

        /// <summary> Produces a random variate whose natural logarithm is from the
        /// Gaussian with mean=0 and the specified standard deviation.
        /// </summary>
        /// <param name="sigma">Standard deviation
        /// </param>
        public virtual double LognormalDeviate(double sigma)
        {
            return (double)(System.Math.Exp((double)(NormalDeviate() * sigma)));
        }

        /// <summary> Returns a uniformly distributed random real number between the specified
        /// inner and outer bounds.
        /// </summary>
        /// <param name="inner">Minimum value desired
        /// </param>
        /// <param name="outer">Maximum value desired
        /// </param>
        public virtual double random_number(double inner, double outer)
        {
            double range = outer - inner;
            return (NextDouble() * range + inner);
        }

        /// <summary>   Returns a value within a certain uniform variation
        /// from the central value.
        /// </summary>
        /// <param name="value">Central value
        /// </param>
        /// <param name="variation">Maximum (uniform) variation above or below center
        /// </param>
        public virtual double about(double value_Renamed, double variation)
        {
            return (value_Renamed + (value_Renamed * random_number(-variation, variation)));
        }

        /// <summary> Returns a value for orbital eccentricity between 0.0 and 1.0</summary>
        public virtual double random_eccentricity()
        {
            return (1.0 - System.Math.Pow(NextDouble(), PhysicalConstants.ECCENTRICITY_COEFF));
        }
    }
}
