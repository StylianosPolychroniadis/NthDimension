namespace Plugin.StarGen
{
    /// <summary> DustBands represent sections of the protoplanetary disc which
    /// contain gas and/or dust to be accreted.
    /// </summary>
    public class DustBand : System.Object
    {
        public const int DBI_NO_INTERSECTION = 0;
        public const int DBI_INNER_OK = 1;
        public const int DBI_OUTER_OK = 2;
        public const int DBI_CONTAINED = 4;

        public double inner_edge;
        public double outer_edge;
        public bool dust_present;
        public bool gas_present;
        public DustBand next_band;

        /// <summary> Public constructor.</summary>
        /// <param name="inner_limit_of_dust">Inner edge of the dust band (in AU)
        /// </param>
        /// <param name="outer_limit_of_dust">Outer edge of the dust band (in AU)
        /// </param>
        public DustBand(double inner_limit_of_dust, double outer_limit_of_dust)
        {
            next_band = null;
            outer_edge = outer_limit_of_dust;
            inner_edge = inner_limit_of_dust;
            dust_present = true;
            gas_present = true;
        }

        /// <summary> Copy constructor.</summary>
        /// <param name="db">Parent DustBand from which to extract values.
        /// </param>
        public DustBand(DustBand db)
        {
            if (db == null)
                return;
            inner_edge = db.inner_edge;
            outer_edge = db.outer_edge;
            dust_present = db.dust_present;
            gas_present = db.gas_present;
            next_band = db.next_band;
        }

        /// <summary> Calculates the intersection of this dust band with a range of
        /// distances and returns a mask constructed from the DBI_xxx flags
        /// exported by this class.  Typically used to identify bands which
        /// overlap the effect radius of a protoplanet.
        /// </summary>
        /// <param name="inner">Inner edge of the intersecting band (in AU)
        /// </param>
        /// <param name="outer">Outer edge of the intersecting band (in AU)
        /// </param>
        public virtual int intersect(double inner, double outer)
        {
            int intval = 0;
            if (outer_edge <= inner || inner_edge >= outer)
                return DBI_NO_INTERSECTION;
            if (inner_edge < inner)
                intval |= DBI_INNER_OK;
            if (outer_edge > outer)
                intval |= DBI_OUTER_OK;
            if (inner_edge >= inner && outer_edge <= outer)
                intval |= DBI_CONTAINED;
            return intval;
        }

        /// <summary> Compares two dust bands for compatibility.</summary>
        /// <returns>s true if the two bands agree on the presence of dust and gas.
        /// </returns>
        /// <param name="db">DustBand to be compared to this one.
        /// </param>
        public virtual bool isCompatibleWith(DustBand db)
        {
            return (dust_present == db.dust_present) && (gas_present == db.gas_present);
        }

        /// <summary> Merge a dust band with its successor, allowing the successor to be
        /// garbage collected.
        /// </summary>
        /// <returns>s true is the merge was successful 
        /// </returns>
        public virtual bool mergeNext()
        {
            if (next_band != null)
            {
                if (isCompatibleWith(next_band))
                {
                    outer_edge = next_band.outer_edge;
                    next_band = next_band.next_band;
                    return true;
                }
            }
            return false;
        }

        /// <summary> Calculate the amount of dust which the specified protoplanet can
        /// accrete from this dust band, if any.
        /// </summary>
        /// <returns>s Quantity of dust, in units of solar masses. 
        /// </returns>
        public virtual double collect_dust(double r_inner, double r_outer, Protoplanet p)
        {
            double gather = 0.0;

            /* as last_mass increases, temp approaches 1.
            * reduced_mass approaches 1 even quicker.
            * as reduced_mass approaches 1, r_inner approaches 0 and
            * r_outer approaches 2*a*(1.0 + e).  Apparently the integration
            * is from 0 to 2a.
            * The masses are expressed in terms of solar masses; temp is therefore
            * the ratio of the planetary mass to the total system (sun + planet)
            */
            if (intersect(r_inner, r_outer) != DBI_NO_INTERSECTION)
            {
                double bandwidth = (r_outer - r_inner);
                double temp1 = r_outer - outer_edge;
                double temp2 = inner_edge - r_inner;
                double width;
                double temp;
                double volume;

                if (bandwidth <= 0.0)
                    bandwidth = 0.0;
                if (temp1 < 0.0)
                    temp1 = 0.0;
                width = bandwidth - temp1;
                if (temp2 < 0.0)
                    temp2 = 0.0;
                width -= temp2;
                if (width < 0.0)
                    width = 0.0;
                temp = 4.0 * System.Math.PI * System.Math.Pow(p.a, 2.0) * p.reduced_mass * (1.0 - p.e * (temp1 - temp2) / bandwidth);
                volume = temp * width;
                gather = (volume * p.mass_density(dust_present, gas_present));
            }
            return gather;
        }
    }
}
