using System.Collections;
using System.Xml.Serialization;

namespace Plugin.StarGen
{
    public class Protosystem : System.Object
    {
        public Star star;
        public Planet planet;
        public DustDisc disc;
        [XmlIgnore]
        public ArrayList protoplanetsList = new ArrayList();
        public double body_inner_bound;
        public double body_outer_bound;

        /// <summary> Star system contructor.  Builds an accretion disc for the
        /// specified star.
        /// </summary>
        /// <param name="s">Primary for this system.
        /// </param>
        public Protosystem(Star s)
        {
            star = s;
            planet = null;
            //planet_head = null;
            body_inner_bound = star.nearest_planet();
            body_outer_bound = star.farthest_planet();
            disc = new DustDisc(0.0, star.stellar_dust_limit(), star.nearest_planet(), star.farthest_planet());
            disc.cloud_eccentricity = 0.2;
        }

        /// <summary> Planetary system contructor.  Builds an accretion disc for the
        /// specified planet around the specified star.  This has not been
        /// exercised to any significant degree, because moon formation
        /// doesn't seem to follow Dole's model quite as well.
        /// </summary>
        /// <param name="s">Primary for this system.
        /// </param>
        /// <param name="p">Planet around which these moons will form.
        /// </param>
        public Protosystem(Star s, Planet p)
        {
            star = s;
            planet = p;
            body_inner_bound = planet.nearest_moon();
            body_outer_bound = planet.farthest_moon();
            disc = new DustDisc(0.0, star.stellar_dust_limit(), planet.nearest_moon(), planet.farthest_moon());
            disc.cloud_eccentricity = 0.2;
        }

        /// <summary> Searches the planetesimals already present in this system
        /// for a possible collision.  Does not run any long-term simulation
        /// of orbits, doesn't try to eject bodies...
        /// </summary>
        /// <param name="p">
        /// Newly injected accreting protoplanet to test
        /// </param>
        public virtual void coalesce_planetesimals(Protoplanet p)
        {
            // note that p is not consumed by this routine...
            Protoplanet node2, node3;
            bool finished = false;
            double temp, dist1, dist2, a3;
            double reduced_mass = p.mass;

            // try to merge Protoplanets
            //node2 = node1 = planet_head;
            foreach (Protoplanet node1 in this.protoplanetsList)
            {
                node2 = node1;
                temp = node1.a - p.a;
                if ((temp > 0.0))
                {
                    dist1 = (p.a * (1.0 + p.e) * (1.0 + p.mass)) - p.a;
                    /* x aphelion   */
                    if (node1.mass <= 0.0)
                        reduced_mass = 0.0;
                    else
                        reduced_mass = System.Math.Pow((node1.mass / (1.0 + node1.mass)), (1.0 / 4.0));
                    dist2 = node1.a - (node1.a * (1.0 - node1.e) * (1.0 - reduced_mass));
                }
                else
                {
                    dist1 = p.a - (p.a * (1.0 - p.e) * (1.0 - p.mass));
                    /* x perihelion */
                    if (node1.mass <= 0.0)
                        reduced_mass = 0.0;
                    else
                        reduced_mass = System.Math.Pow(node1.mass / (1.0 + node1.mass), (1.0 / 4.0));
                    dist2 = (node1.a * (1.0 + node1.e) * (1.0 + reduced_mass)) - node1.a;
                }
                if (((System.Math.Abs(temp) <= System.Math.Abs(dist1)) || (System.Math.Abs(temp) <= System.Math.Abs(dist2))))
                {
                    //collision
                    if (p.mass > 0.2 /*&& (node1.mass / p.mass) > 0.1*/)
                    {

                    }

                    a3 = (node1.mass + p.mass) / ((node1.mass / node1.a) + (p.mass / p.a));
                    temp = node1.mass * System.Math.Sqrt(node1.a) * System.Math.Sqrt(1.0 - (node1.e * node1.e));
                    temp += (p.mass * System.Math.Sqrt(p.a) * System.Math.Sqrt(System.Math.Sqrt(1.0 - (p.e * p.e))));
                    temp /= ((node1.mass + p.mass) * System.Math.Sqrt(a3));
                    temp = 1.0 - (temp * temp);
                    if (((temp < 0.0) || (temp >= 1.0)))
                        temp = 0.0;
                    p.e = System.Math.Sqrt(temp);
                    temp = node1.mass + p.mass;
                    node1.a = a3;
                    node1.e = p.e;
                    node1.mass = temp;
                    disc.accrete_dust(node1);
                    //node1 = null;
                    finished = true;
                }
            }
            if (!finished)
            {
                // add copy of planetesimal to planets list
                node3 = new Protoplanet(p);
                node3.gas_giant = (p.mass >= p.crit_mass);
                protoplanetsList.Add(node3);
                protoplanetsList.Sort(new PlanetSort(SortDirection.Ascending));

                #region Original code, made obsolete by sort method of ArrayList
                //if ((planet_head == null))
                //{
                //    planet_head = node3;
                //}
                //else
                //{
                //    node1 = planet_head;
                //    if ((p.a < node1.a))
                //    {
                //        node3.next_planet = node1;
                //        planet_head = node3;
                //    }
                //    else if ((planet_head.next_planet == null))
                //    {
                //        planet_head.next_planet = node3;
                //    }
                //    else
                //    {
                //        while (((node1 != null) && (node1.a < p.a)))
                //        {
                //            node2 = node1;
                //            node1 = node1.next_planet;
                //        }
                //        node3.next_planet = node1;
                //        node2.next_planet = node3;
                //    }
                //}
                #endregion
            }
        }

        /// <summary> Accretes protoplanets from the dust disc in this system.</summary>
        /// <returns>s First protoplanet of accreted system, as the head
        /// element of a list of protoplanets.
        /// </returns>
        public virtual void dist_planetary_masses()
        {
            Protoplanet p0;

            while (disc.dust_left)
            {
                p0 = new Protoplanet(disc.body_inner_bound, disc.body_outer_bound);
                if (disc.dust_available(p0))
                {
                    p0.dust_density = PhysicalConstants.DUST_DENSITY_COEFF * System.Math.Sqrt(star.SM) * System.Math.Exp((-PhysicalConstants.ALPHA) * System.Math.Pow(p0.a, (1.0 / PhysicalConstants.N)));
                    p0.crit_mass = star.critical_limit(p0.a, p0.e);
                    disc.accrete_dust(p0);
                    if (p0.massOK())
                        coalesce_planetesimals(p0);
                    else
                    {
                        //System.out.println(".. failed due to large neighbor.");
                    }
                }
            }
        }

        /// <summary> Accretes protoplanets from the dust disc in this system.
        /// This ought to work, but has not been extensively tested.
        /// </summary>
        /// <returns>s First protoplanet of accreted system, as the head
        /// element of a list of protoplanets.
        /// </returns>
        public virtual void dist_moon_masses()
        {
            Protoplanet p0;

            while (disc.dust_left)
            {
                p0 = new Protoplanet(disc.body_inner_bound, disc.body_outer_bound);
                if (disc.dust_available(p0))
                {
                    p0.dust_density = PhysicalConstants.DUST_DENSITY_COEFF * System.Math.Sqrt(planet.mass * 1000 / PhysicalConstants.SUN_MASS_IN_EARTH_MASSES) * System.Math.Exp((-PhysicalConstants.ALPHA) * System.Math.Pow(p0.a, (1.0 / PhysicalConstants.N)));
                    p0.crit_mass = star.critical_limit(planet.a, planet.e);
                    disc.accrete_dust(p0);
                    if (p0.massOK())
                        coalesce_planetesimals(p0);
                    else
                    {
                        return;
                        //System.out.println(".. failed due to large neighbor.");
                    }
                }
            }
        }

        //public virtual void  print_Protoplanets()
        //{
        //    Protoplanet p = planet_head;
        //    while (p != null)
        //    {
        //        p.print(); p = p.next_planet;
        //    }
        //}
    }
}
