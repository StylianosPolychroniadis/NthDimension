using System;
using System.Collections;
using System.Xml.Serialization;

namespace Plugin.StarGen
{
    [Serializable]
    [XmlRootAttribute("StarSystem")]
    public class StarSystem : AccreteObject
    {
        public double x, y, z;
        public Star Primary { get; set; }

        [XmlElement("Planet")]
        public ArrayList planetsList = new ArrayList();
        private ArrayList moonsList = new ArrayList();

        /// <summary> Public constructor builds a star system with a random star.</summary>
        public StarSystem()
        {
            Primary = new Star((int)(nextDouble() * 60)); // more variety
            //primary = new Star(random_number(0.6, 1.3)); // starform method
            Initialize();
            //initializeBode();
        }

        /// <summary> Creates the planets of this system using Dole's accretion algorithm.</summary>
        private void Initialize()
        {
            PlanetClassifier pc = new PlanetClassifier();
            Planet last_planet = null, cur_planet = null;
            //Protoplanet p;
            ArrayList protoplanetsList = new ArrayList();
            //int I;

            Protosystem ps = new Protosystem(Primary);
            ps.dist_planetary_masses();
            //p = ps.planet_head;
            foreach (Protoplanet p in ps.protoplanetsList)
            {
                if (p.mass > 0.0)
                {
                    cur_planet = new Planet(p);
                    cur_planet.age = Primary.age; // not sure why, but planets are missing age when generated, so i'll put this here
                    cur_planet.orbit_zone = Primary.orb_zone(cur_planet.a);
                    cur_planet.set_vital_stats(Primary.SM, Primary.r_greenhouse, Primary.r_ecosphere, Primary.age);
                    cur_planet.description = pc.planetType(cur_planet);

                    // could generate moons here
                    // 1. generate a new protosystem based on the planet and star
                    // 2. pull out all of the protoplanets and create moons from them
                    // 3. delete the protosystem
                    #region Added: moons [Yan]

                    //not sure if it's ok to calculate this way, satellites can be created individually as planets and then get snatched by bigger planet but it works this way too
                    //planet migration due to orbital drag not calculated too
                    Protosystem ps_moons = new Protosystem(Primary, cur_planet);
                    ps_moons.dist_moon_masses();
                    Planet last_moon = null, cur_moon = null;

                    foreach (Protoplanet p_moon in ps_moons.protoplanetsList)
                    {
                        if (p_moon.mass > 0.0)
                        {
                            cur_moon = new Planet(p_moon);
                            if (last_moon != null)
                            {
                                if (cur_moon.mass > 0.0000001) // fine-tweaked to give some moons, but not too many
                                {
                                    cur_planet.moons.Add(cur_moon);
                                }
                            }
                            last_moon = cur_moon;
                        }
                    }
                    #endregion

                    if (last_planet != null)
                    {
                        this.planetsList.Add(cur_planet);
                    }
                    last_planet = cur_planet;
                }
            }
            ps = null;
            planetsList.Sort(new PlanetSort(SortDirection.Ascending));
        }

        #region Obsolete
        /// <summary> Creates the planets of this system using a diddled Bode's Law.</summary>
        //public virtual void  initializeBode()
        //{
        //    /* BODE - BODE-TITIUS SEQUENCE FOR SATELLITE ORBITS */
        //    double[] BODE = new double[]{0.4, 0.7, 1.0, 1.6, 2.8, 5.2, 10.0, 19.6, 29.2, 38.8, 77.2, 154, 307.6};
        //    Planet last_planet = null, cur_planet = null;
        //    int I;

        //    for (I = 0; I < Math.Floor(nextDouble() * 13); I++)
        //    {
        //        cur_planet = new Planet(primary.AU * BODE[I], primary.EM, primary.SM);
        //        cur_planet.orbit_zone = primary.orb_zone(cur_planet.a);
        //        cur_planet.set_vital_stats(primary.SM, primary.r_greenhouse, primary.r_ecosphere, primary.age);
        //        if (I == 0)
        //            planets = cur_planet;
        //        else
        //            last_planet.next_planet = cur_planet;
        //        last_planet = cur_planet;
        //    }

        //    // the following loop adjusts planet types based on system layout
        //    // beyond a certain mass ratio, assume the smaller planet couldn't form
        //    cur_planet = planets;
        //    last_planet = null;
        //    while (cur_planet != null)
        //    {
        //        if (cur_planet.plan_class != '-')
        //        {
        //            if (last_planet != null)
        //            {
        //                if (last_planet.plan_class != '-')
        //                {
        //                    if ((cur_planet.mass / last_planet.mass) < 0.005)
        //                    {
        //                        cur_planet.plan_class = 'B';
        //                    }
        //                }
        //            }
        //            if (cur_planet.next_planet != null)
        //            {
        //                if (cur_planet.next_planet.plan_class != '-')
        //                {
        //                    if ((cur_planet.mass / cur_planet.next_planet.mass) < 0.005)
        //                    {
        //                        cur_planet.plan_class = 'B';
        //                    }
        //                }
        //            }
        //        }
        //        last_planet = cur_planet;
        //        cur_planet = cur_planet.next_planet;
        //    }
        //}
        #endregion
    }
}
