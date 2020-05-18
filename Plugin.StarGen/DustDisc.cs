namespace Plugin.StarGen
{
    public class DustDisc
    {
        /// <summary> A DustDisc manages the collection of DustBands representing
        /// the dust and gas components of a protoplanetary disc.
        /// </summary>
        public bool dust_left;
        public bool gas; // true if gas available in current working band
        public double cloud_eccentricity;
        public DustBand dust_head;
        public double body_inner_bound;
        public double body_outer_bound;

        /// <summary> Determines whether dust is present within the effect radius of
        /// a specific Protoplanet.
        /// </summary>
        /// <param name="p">Protoplanet within the disc
        /// </param>
        /// <returns>s true if there is a band containing dust which this body
        /// can accrete.
        /// </returns>
        public virtual bool dust_available(Protoplanet p)
        {
            double inside_range = p.inner_effect_limit(cloud_eccentricity);
            double outside_range = p.outer_effect_limit(cloud_eccentricity);
            DustBand current_dust_band;
            bool dust_here = false;

            current_dust_band = dust_head;
            while ((current_dust_band != null) && (current_dust_band.outer_edge < inside_range))
                current_dust_band = current_dust_band.next_band;
            if (current_dust_band == null)
                dust_here = false;
            else
                dust_here = current_dust_band.dust_present;
            while ((current_dust_band != null) && (current_dust_band.inner_edge < outside_range))
            {
                dust_here = dust_here || current_dust_band.dust_present;
                current_dust_band = current_dust_band.next_band;
            }
            return (dust_here);
        }

        /// <summary> Removes a band of dust from the specified DustBand, supplementing it
        /// with 2 new bands.
        /// </summary>
        /// <param name="node1">Band from which dust has been removed
        /// </param>
        /// <param name="min">Inner limit of cleared lane (in AU)
        /// </param>
        /// <param name="max">Outer limit of cleared lane (in AU)
        /// </param>
        /// <returns>s Next band in disc, outside affected band 'node1'.
        /// </returns>
        public virtual DustBand splitband(DustBand node1, double min, double max)
        {
            DustBand node2 = new DustBand(node1);
            DustBand node3 = new DustBand(node1);
            node2.dust_present = false; // dust sucked up by planetesimal
            node2.gas_present = node1.gas_present && gas;
            node2.inner_edge = min;
            node2.outer_edge = max;
            node3.inner_edge = max;
            node1.outer_edge = min;
            node1.next_band = node2;
            node2.next_band = node3;
            return node3.next_band;
        }

        /// <summary> Removes outer portion of the specified DustBand, following it
        /// with a new band.
        /// </summary>
        /// <param name="node1">Band from which dust has been removed
        /// </param>
        /// <param name="outer">Inner limit of cleared lane (in AU)
        /// </param>
        /// <returns>s Next band in disc, outside affected band 'node1'.
        /// </returns>
        public virtual DustBand splithigh(DustBand node1, double outer)
        {
            DustBand node2 = new DustBand(node1);
            node1.next_band = node2;
            node1.dust_present = false;
            node1.gas_present = node1.gas_present && gas;
            node2.inner_edge = outer;
            node1.outer_edge = outer;
            return node2.next_band;
        }

        /// <summary> Removes inner portion of the specified DustBand, preceding it
        /// with a new band.
        /// </summary>
        /// <param name="node1">Band from which dust has been removed
        /// </param>
        /// <param name="inner">Outer limit of cleared lane (in AU)
        /// </param>
        /// <returns>s Next band in disc, outside affected band 'node1'.
        /// </returns>
        public virtual DustBand splitlow(DustBand node1, double inner)
        {
            DustBand node2 = new DustBand(node1);
            node1.next_band = node2;
            node2.dust_present = false;
            node2.gas_present = node1.gas_present && gas;
            node2.inner_edge = inner;
            node1.outer_edge = inner;
            return node2.next_band;
        }

        /// <summary> Identifies dust bands which may be affected by the specified protoplanet,
        /// then modifies the disc accordingly.  Adjacent bands with identical
        /// characteristics are merged to help reduce fragmentation of the disc.
        /// </summary>
        /// <param name="min">Inner limit of cleared lane (in AU)
        /// </param>
        /// <param name="max">Outer limit of cleared lane (in AU)
        /// </param>
        public virtual void update_dust_lanes(double min, double max)
        {
            DustBand node1;

            dust_left = false;
            // update dust bands under influence of Protoplanet
            node1 = dust_head;
            while (node1 != null)
            {
                int intval = node1.intersect(min, max);
                if (intval == (DustBand.DBI_INNER_OK | DustBand.DBI_OUTER_OK))
                {
                    node1 = splitband(node1, min, max);
                }
                else if (intval == DustBand.DBI_OUTER_OK)
                {
                    node1 = splithigh(node1, max);
                }
                else if (intval == DustBand.DBI_INNER_OK)
                {
                    node1 = splitlow(node1, min);
                }
                else if (intval == DustBand.DBI_CONTAINED)
                {
                    node1.dust_present = false;
                    node1.gas_present = node1.gas_present && gas;
                    node1 = node1.next_band;
                }
                else
                    node1 = node1.next_band;
            }
            // calculate whether accretable dust is left
            for (node1 = dust_head; node1 != null; node1 = node1.next_band)
            {
                dust_left |= (node1.dust_present && (node1.outer_edge >= body_inner_bound) && (node1.inner_edge <= body_outer_bound));
                while (node1.mergeNext())
                    ; // merge fragmented dust bands, if any
            }
        }

        /// <summary> Accretes dust and/or gas from all bands onto the specified protoplanet,
        /// iterating until the marginal mass increase approaches zero.  Once the
        /// new mass has been calculated, the dust bands are updated to reflect the
        /// accretion of dust and/or gas onto the protoplanet.
        /// </summary>
        /// <param name="p">Protoplanet accreting in this cycle
        /// </param>
        public virtual void accrete_dust(Protoplanet p)
        {
            //double temp_mass;
            double start_mass = p.mass;
            double minimum_accretion = 0.0001 * start_mass;
            double r_inner, r_outer, gatherLast, gatherNow;
            DustBand db;

            gatherNow = 0.0;
            do
            {
                gatherLast = gatherNow;
                // calculate new mass of protoplanet, considering last calculated
                // quantity of accreted matter, then calculate region to be swept
                // based on the updated mass.
                p.mass = start_mass + gatherLast;
                p.reduce_mass();
                gas = !p.accretes_gas();
                r_inner = p.inner_reduced_limit(cloud_eccentricity);
                r_outer = p.outer_reduced_limit(cloud_eccentricity);
                if (r_inner < 0.0)
                    r_inner = 0.0;
                // sweep through all dust bands, collecting matter within the
                // effective reach of the protoplanet's gravity.
                gatherNow = 0.0;
                for (db = dust_head; db != null; db = db.next_band)
                {
                    gatherNow += db.collect_dust(r_inner, r_outer, p);
                }
            }
            while ((gatherNow - gatherLast) >= minimum_accretion);
            update_dust_lanes(p.inner_effect_limit(cloud_eccentricity), p.outer_effect_limit(cloud_eccentricity));
        }

        /// <summary> Public constructor.</summary>
        /// <param name="inner_limit_of_dust">Innermost limit of dust
        /// </param>
        /// <param name="outer_limit_of_dust">Outermost limit of dust
        /// </param>
        /// <param name="inner_bound">Innermost limit of planetary orbits
        /// </param>
        /// <param name="outer_bound">Outermost limit of planetary orbits
        /// </param>
        public DustDisc(double inner_limit_of_dust, double outer_limit_of_dust, double inner_bound, double outer_bound)
        {
            dust_head = new DustBand(inner_limit_of_dust, outer_limit_of_dust);
            body_inner_bound = inner_bound;
            body_outer_bound = outer_bound;
            dust_left = true;
        }
    }
}
