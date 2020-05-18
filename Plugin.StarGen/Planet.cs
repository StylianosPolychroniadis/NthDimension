using System;
using System.Collections;
using System.Linq;
using System.Xml.Serialization;

namespace Plugin.StarGen
{
    /// <summary> This class provides an object representing a planetary body,
    /// complete with atmosphere.  Two separate sets of methods are
    /// provided.  The first set is empirical, usually simplistic, and
    /// built for use in various games I was experimenting with at the
    /// time.  The second set is based more on physics, using calculations
    /// presented in journals such as Icarus.
   
    /// </summary>
    [Serializable]
    [XmlRoot("Planet")]
    public class Planet : Blackbody
    {
        public double a;                  // semi-major axis of the orbit (in AU)
        public double e;                  // eccentricity of the orbit	     
        public double where_in_orbit;     // position along orbit (in radians) 
        public double mass;               // mass (in Earth masses)	     
        public bool gas_giant;          // true if the planet is a gas giant 
        public int orbit_zone;         // the 'zone' of the planet          
        public double radius;             // equatorial radius (in km)	     
        public double density;            // density (in g/cc)		     
        public double orb_period;         // length of the local year (days)   
        public double day;                // length of the local day (hours)   
        public int resonant_period;    // true if in resonant rotation   
        public int axial_tilt;         // units of degrees		     
        public double esc_velocity;       // units of cm/sec		     
        public double surf_accel;         // units of cm/sec2		     
        public double surf_grav;          // units of Earth gravities	     
        public double rms_velocity;       // units of cm/sec		     
        public double molec_weight;       // smallest molecular weight retained
        public double volatile_gas_inventory;
        public double GH2, GH2O, GN2, GO2, GCO2; // gas retention percentages
        public double surf_pressure;      // units of millibars (mb)	     
        public bool greenhouse_effect;  // runaway greenhouse effect?	
        public double boil_point;         // the boiling point of water (Kelvin)
        public double albedo;             // albedo of the planet		     
        public double surf_temp; // surface temperature in Kelvin     
        public double min_temp, max_temp; // surface temperature ranges 
        public double avg_temp; // weighted average of iterations 
        public double hydrosphere; // fraction of surface covered	     
        public double cloud_cover; // fraction of surface covered	     
        public double ice_cover; // fraction of surface covered	     
        public char plan_class; // general type classification
        public bool binary_planet = false;
        //public Planet next_planet;
        //public Planet first_moon;

        [XmlElement(ElementName = "MoonsCount")]
        public int MoonsCount { get { return this.moons.Count; } }

        [XmlIgnore]
        public ArrayList moons;
        //        public ArrayList planets;

        // these data elements are star related, but are stored here for my use
        public double r_ecosphere;
        public double resonance;
        public double stell_mass_ratio;
        public double age;
        // albedo factors
        public double cloud_factor, water_factor, rock_factor, airless_rock_factor, ice_factor, airless_ice_factor;
        [XmlIgnore]
        public int its; //iterations
        [XmlIgnore]
        public bool temp_unstable;
        [XmlIgnore]
        public string description;
        //public bool planetary_ring; //indicates wether the moon has been
        //
        // EMPIRICAL METHODS (SET 1)
        //

        /// <summary> Generates a rough-and-ready calculation into Terrestrial and
        /// Gas Giant categories based on temperature.
        /// </summary>
        /// <returns>s 'T' for Terrestrial, 'G' for Gas Giant.
        /// </returns>
        public virtual char classify_by_temperature()
        // based on temperature
        {
            if (TEMP > 200.0)
            {
                gas_giant = false;
                return 'T';
            }
            else
            {
                gas_giant = true;
                return 'G';
            }
        }

        /// <summary> Generates a rough-and-ready calculation into Terrestrial and
        /// Gas Giant categories based on whether the protoplanet accreted gas.
        /// </summary>
        /// <returns>s 'T' for Terrestrial, 'G' for Gas Giant.
        /// </returns>
        public virtual char classify_by_accretion()
        // based on accretion status
        {
            return (gas_giant ? 'G' : 'T');
        }

        /// <summary> Determines planetary mass through a quick 'accretion' of
        /// mass within a portion of a disc. Gives a mass profile similar
        /// to the solar system when used with Bode's Law calculations.
        /// </summary>
        /// <param name="MASS">Mass of primary in Solar masses
        /// </param>
        /// <param name="DIST">Semi-major axis of orbit in AU
        /// </param>
        /// <param name="PLAN_CLASS">'T' or 'G' denoting 'class' of planet
        /// </param>
        /// <returns>s Planetary mass in Solar masses
        /// </returns>
        public virtual double mass_by_integration(double MASS, double DIST, char PLAN_CLASS)
        {
            double BV;
            double RF;

            RF = LognormalDeviate(1.0);
            if (PLAN_CLASS == 'T')
            {
                BV = DIST * DIST; // accretion from a disk 
                return 2.7 * MASS * System.Math.Exp((-DIST) * System.Math.Sqrt(DIST)) * BV * RF;
            }
            else
            {
                BV = DIST * DIST * DIST; // accretion from a volume 
                return 2.7 * System.Math.Exp((-DIST) / (2.7 * MASS)) * BV * RF;
            }
        }

        /// <summary> Estimates planetary density by planetary class, randomly
        /// distributed around central values.
        /// </summary>
        /// <param name="PLAN_CLASS">'T' for Terrestrial, 'G' for Gas Giant
        /// </param>
        /// <returns>s Density in grams per cubic centimeter
        /// </returns>
        public virtual double density_by_temperature(char PLAN_CLASS)
        {
            if (PLAN_CLASS == 'T')
            {
                return LognormalDeviate(0.1) * 5.5;
            }
            else
            {
                if (PLAN_CLASS == 'G')
                    return LognormalDeviate(0.1);
                else
                    return 0.0;
            }
        }

        /// <summary> Returns radius of the planet in kilometers.  Almost identical
        /// to 'volume_radius()', but remains here by historical accident.
        /// </summary>
        /// <param name="MASS">Mass in Earth masses
        /// </param>
        /// <param name="DENSITY">Average density in grams per cubic centimeter
        /// </param>
        /// <param name="PLAN_CLASS">'T' for Terrestrial, 'G' for Gas Giant
        /// </param>
        /// <returns>s Planetary radius in kilometers
        /// </returns>
        public virtual double planet_radius(double MASS, double DENSITY, char PLAN_CLASS)
        {
            if (PLAN_CLASS == '-')
                return 0.0;
            else
                return System.Math.Exp(System.Math.Log(MASS * 6.0e+12 / (DENSITY * (4.0 / 3.0) * System.Math.PI)) / 3.0);
        }

        /// <summary> Calculates surface gravity in cm/sec^2</summary>
        /// <param name="MASS">Mass in Earth masses
        /// </param>
        /// <param name="RAD">Radius in kilometers
        /// </param>
        /// <returns>s Surface gravity (acceleration) in cm/sec^2.
        /// </returns>
        public virtual double surface_gravity(double MASS, double RAD)
        {
            if (RAD == 0.0)
                return 0.0;
            else
                return (PhysicalConstants.G * MASS * PhysicalConstants.MEARTH) / (RAD * RAD * 1.0e10);
        }

        /// <summary> Calculates 'escape energy'. Escape energy was an idea I [Carl] was
        /// toying with for a game, and this routine tries to find planetary
        /// escape energy in the same units as constant accelleration travel
        /// across the star system.  Probably not useful to anybody else (and
        /// may not be useful to me if I can't find those PBEM notes :)
        /// </summary>
        /// <param name="MASS">Mass in Earth masses
        /// </param>
        /// <param name="RADIUS">Radius in kilometers
        /// </param>
        /// <returns>s Energy of escape, in 'standard' units
        /// </returns>
        public virtual double PESCAPE(double MASS, double RADIUS)
        {
            double GME, AU, RE, A;

            GME = 2.0 * 6.67e-08 * 6.0e+27 * MASS;
            AU = 1.5e+13; // 1 AU IN CM 
            RE = RADIUS * 1.0e5; // RADIUS IN CM 
            RE = RE / AU;
            A = System.Math.Sqrt(GME / RE) - RE;
            return A / AU;
        }

        /// <summary> Estimates blackbody temperature of a planet. The temperature is
        /// calculated for a planet with a given albedo at a specific distance
        /// from a primary of the specified luminosity.
        /// </summary>
        /// <param name="DIST">Semi-major axis of orbit in AU
        /// </param>
        /// <param name="LUM">Luminosity of primary (relative to Sol???)
        /// </param>
        /// <returns>s Temperature in degrees Kelvin
        /// </returns>
        public virtual double planet_temperature(double DIST, double LUM)
        {
            double PTEMP = (((1.0 - albedo) / (DIST * DIST)) * LUM * (1370000.0 / SIGMA));
            return System.Math.Exp(System.Math.Log(PTEMP) / 4.0);
        }

        /// <summary> Calculates the approximate ratio (from none to all) of a gas
        /// which is retained in the atmosphere of this planet.  Does not
        /// attempt to decide just how much gas that really is, or the
        /// resulting contribution to atmospheric pressure.
        /// </summary>
        /// <param name="WT">Molecular weight of gas in question
        /// </param>
        /// <param name="TEMP">Blackbody temperature
        /// </param>
        /// <param name="MASS">Mass in Earth masses
        /// </param>
        /// <param name="RAD">Radius in centimeters
        /// </param>
        /// <returns>s Unitless ratio between 0.0 and 1.0
        /// </returns>
        public virtual double gas_retention(double WT, double TEMP, double MASS, double RAD)
        {
            // 6*(MEAN VELOCITY)/(ESCAPE VELOCITY)
            // (modified to a factor of 8 because I liked the results better)
            double SPD, ESC;

            if (RAD == 0.0)
            {
                return 10.0;
            }
            ;
            SPD = System.Math.Sqrt((3.0 * PhysicalConstants.BK * TEMP) / (WT * PhysicalConstants.MH));
            ESC = System.Math.Sqrt((2.0 * PhysicalConstants.G * MASS) / RAD);
            return 8.0 * (SPD / ESC);
        }

        public Planet()
        {

        }

        /// <summary> Simplistic constructor, intended for use with Bode's Law routines</summary>
        /// <param name="AuDist">Semi-major axis of orbit in AU
        /// </param>
        /// <param name="EM">Emission of the star relative to Sol
        /// </param>
        /// <param name="SM">Stellar mass relative to Sol
        /// </param>
        public Planet(double AuDist, double EM, double SM)
        {
            double PT;

            //next_planet = null;
            //first_moon = null;

            a = AuDist; // is AU * BODE[I]
            e = 0.0;
            where_in_orbit = (nextDouble() * 360.0) * PhysicalConstants.DEG_TO_RAD;
            albedo = 0.0; // start with albedo of 0
            TEMP = planet_temperature(AuDist, EM);
            plan_class = classify_by_temperature();
            mass = mass_by_integration(SM, a, plan_class);
            if (mass < 1.0e-6)
            {
                mass = 0.0;
                plan_class = '-';
            }
            density = density_by_temperature(plan_class);
            radius = planet_radius(mass, density, plan_class);
            GH2 = gas_retention(PhysicalConstants.H2, TEMP, mass * PhysicalConstants.MEARTH, radius * 1.0e+5);
            GH2O = gas_retention(PhysicalConstants.H2O, TEMP, mass * PhysicalConstants.MEARTH, radius * 1.0e+5);
            GN2 = gas_retention(PhysicalConstants.N2, TEMP, mass * PhysicalConstants.MEARTH, radius * 1.0e+5);
            GO2 = gas_retention(PhysicalConstants.O2, TEMP, mass * PhysicalConstants.MEARTH, radius * 1.0e+5);
            GCO2 = gas_retention(PhysicalConstants.CO2, TEMP, mass * PhysicalConstants.MEARTH, radius * 1.0e+5);
            // classification scheme needs to be redone
            while (true)
            {
                if (plan_class == 'T')
                {
                    albedo = 1.0 - GH2O;
                    if (albedo < 0.0)
                    {
                        albedo = 0.0;
                    }
                    ;
                    if (GH2 < 1.0)
                    {
                        plan_class = 'E';
                    }
                    ;
                    if (GN2 > 1.0)
                    {
                        plan_class = 'O';
                    }
                    ;
                    if (GO2 > 1.0)
                    {
                        plan_class = 'C';
                    }
                    ;
                    if (GCO2 > 1.0)
                    {
                        plan_class = 'V';
                    }
                    ; // venus type
                    if ((plan_class != 'V') && (TEMP > 340.0))
                    {
                        plan_class = 'R'; // RUNAWAY GREENHOUSE EFFECT
                        albedo = 0.0;
                    }
                    if ((plan_class != 'V') && (TEMP < 230.0))
                    {
                        plan_class = 'I'; // ICE BALL/ICE AGE
                        albedo = 0.95;
                    }
                }
                else
                {
                    if (plan_class == 'G')
                    {
                        albedo = 0.5;
                    }
                    ;
                }
                PT = planet_temperature(AuDist, EM);
                if (PT == TEMP)
                    break; // temperature converged to a steady value
                // it is actually fairly common for Earthlike planets to have chaotic
                // fluctuations in temperature around the freezing or boiling points;
                // those fluctuations cause an infinite loop here.  Need to terminate
                // the loop with a special flag if chaotic variations are discovered.
                TEMP = PT;
            }
            surf_grav = surface_gravity(mass, radius);

            // other adjustments get done in System, based on neighbors
        }

        /// <summary> 
        /// Constructor from an accreted protoplanet.
        /// </summary>
        /// <param name="p">Protoplanet that forms the basis of this planet
        /// </param>
        public Planet(Protoplanet p)
        {
            //next_planet = null;
            //first_moon = null;
            a = p.a; e = p.e; mass = p.mass * PhysicalConstants.SUN_MASS_IN_EARTH_MASSES;
            gas_giant = p.gas_giant;
            where_in_orbit = (nextDouble() * 360.0) * PhysicalConstants.DEG_TO_RAD;
            albedo = 0.0; // start with albedo of 0
            plan_class = classify_by_accretion();
            if (mass < 1.0e-6)
            {
                mass = 0.0;
                plan_class = '-';
            }
            density = density_by_temperature(plan_class);
            radius = planet_radius(mass, density, plan_class);
            moons = new ArrayList();
            if (p.moons != null)
            {
                foreach (Protoplanet moon in p.moons)
                {
                    moons.Add(moon);
                }
            }
        }

        //
        // *** CALCULATION METHODS (SET 2)
        //

        /// <summary> Calculates radius based on volume, calibrated to Earth.</summary>
        /// <param name="mass">Mass in Earth masses
        /// </param>
        /// <param name="density">Average density in grams per cubic centimeter
        /// </param>
        /// <returns>s Radius in kilometers
        /// </returns>
        public virtual double volume_radius(double mass, double density)
        {
            double volume;

            if (density == 0.0)
                return 0.0; // sometimes it happens, for grit
            mass *= PhysicalConstants.EARTH_MASS_IN_GRAMS;
            volume = mass / density;
            return (System.Math.Pow((3.0 * volume) / (4.0 * System.Math.PI), (1.0 / 3.0)) / PhysicalConstants.CM_PER_KM);
        }

        /// <summary> Calculates the equatorial radius of the planet given mass, 'zone', and
        /// whether it's a gas giant or not.
        /// This formula is listed as eq.9 in Fogg's article, although some typos  
        /// crop up in that eq.  See "The Internal Constitution of Planets", by    
        /// Dr. D. S. Kothari, Mon. Not. of the Royal Astronomical Society, vol 96 
        /// pp.833-843, 1936 for the derivation.  Specifically, this is Kothari's  
        /// eq.23, which appears on page 840.
        /// </summary>
        /// <param name="mass">Mass in Earth masses
        /// </param>
        /// <param name="giant">True if planet is a gas giant
        /// </param>
        /// <param name="zone">Orbital zone, 1 to 3
        /// </param>
        /// <returns>s Equatorial radius in kilometers
        /// </returns>
        public virtual double kothari_radius(double mass, bool giant, int zone)
        {
            double temp, temp2, atomic_weight, atomic_num;

            if (mass == 0.0)
                return 0.0; // for grit belts (see constructor, mass_by_integration)

            if (zone == 1)
            {
                if (giant)
                {
                    atomic_weight = 9.5;
                    atomic_num = 4.5;
                }
                else
                {
                    atomic_weight = 15.0;
                    atomic_num = 8.0;
                }
            }
            else if (zone == 2)
            {
                if (giant)
                {
                    atomic_weight = 2.47;
                    atomic_num = 2.0;
                }
                else
                {
                    atomic_weight = 10.0;
                    atomic_num = 5.0;
                }
            }
            else
            {
                if (giant)
                {
                    atomic_weight = 7.0;
                    atomic_num = 4.0;
                }
                else
                {
                    atomic_weight = 10.0;
                    atomic_num = 5.0;
                }
            }
            temp = atomic_weight * atomic_num;
            temp = (2.0 * PhysicalConstants.BETA_20 * System.Math.Pow(PhysicalConstants.EARTH_MASS_IN_GRAMS, (1.0 / 3.0))) / (PhysicalConstants.A1_20 * System.Math.Pow(temp, (1.0 / 3.0)));
            temp2 = PhysicalConstants.A2_20 * System.Math.Pow(atomic_weight, (4.0 / 3.0)) * System.Math.Pow(PhysicalConstants.EARTH_MASS_IN_GRAMS, (2.0 / 3.0));
            temp2 = temp2 * System.Math.Pow(mass, (2.0 / 3.0));
            temp2 = temp2 / (PhysicalConstants.A1_20 * atomic_num * atomic_num);
            temp2 = 1.0 + temp2;
            temp /= temp2;
            temp *= System.Math.Pow(mass, (1.0 / 3.0)) / PhysicalConstants.CM_PER_KM;
            return (temp);
        }

        /// <summary> Empirically determine density based on distance from primary.</summary>
        /// <param name="mass">Mass in Earth masses
        /// </param>
        /// <param name="orb_radius">Semi-major axis of orbit in AU
        /// </param>
        /// <param name="gas_giant">True if planet is a gas giant
        /// </param>
        /// <returns>s Density in grams per cubic centimeter
        /// </returns>
        public virtual double empirical_density(double mass, double orb_radius, bool gas_giant)
        {
            double temp;

            if (mass == 0.0)
                return 0.0; // for grit belts (see constructor, mass_by_integration)

            temp = System.Math.Pow(mass, (1.0 / 8.0));
            temp = temp * System.Math.Pow(r_ecosphere / orb_radius, 0.25);
            if (gas_giant)
                return (temp * 1.2);
            else
                return (temp * 5.5);
        }

        /// <summary> Calculates average density of body, given mass and radius.</summary>
        /// <param name="mass">Mass in Earth masses.
        /// </param>
        /// <param name="equat_radius">Equatorial radius in kilometers.
        /// </param>
        /// <returns>s Average density in grams per cubic centimeter.
        /// </returns>
        public virtual double volume_density(double mass, double equat_radius)
        {
            double volume;

            if (equat_radius == 0.0)
                return 0.0;

            mass *= PhysicalConstants.EARTH_MASS_IN_GRAMS;
            equat_radius *= PhysicalConstants.CM_PER_KM;
            volume = (4.0 * System.Math.PI * equat_radius * equat_radius * equat_radius) / 3.0;
            return (mass / volume);
        }

        /// <summary> Calculates orbital period (year) of the two bodies.</summary>
        /// <param name="separation">Distance in AU between bodies
        /// </param>
        /// <param name="small_mass">Mass of smaller body in Solar masses
        /// </param>
        /// <param name="large_mass">Mass of larger body in Solar masses
        /// </param>
        /// <returns>s Orbital period in Earth days.
        /// </returns>
        public virtual double period(double separation, double small_mass, double large_mass)
        {
            double period_in_years;

            period_in_years = System.Math.Sqrt(separation * separation * separation / (small_mass + large_mass));
            return (period_in_years * PhysicalConstants.DAYS_IN_A_YEAR);
        }

        /// <summary> Estimates length of the planet's day.
        /// Fogg's information for this routine came from Dole "Habitable Planets  
        /// for Man", Blaisdell Publishing Company, NY, 1964.  From this, he came    
        /// up with his eq.12, which is the equation for the 'base_angular_velocity' 
        /// below.  He then used an equation for the change in angular velocity per  
        /// time (dw/dt) from P. Goldreich and S. Soter's paper "Q in the Solar      
        /// System" in Icarus, vol 5, pp.375-389 (1966).  Using as a comparison the  
        /// change in angular velocity for the Earth, Fogg has come up with an	    
        /// approximation for our new Planet (his eq.13) and take that into account. 
        /// This is used to find 'change_in_angular_velocity' below.
        /// </summary>
        /// <param name="mass">Mass in Earth masses.
        /// </param>
        /// <param name="radius">Equatorial radius in kilometers.
        /// </param>
        /// <param name="eccentricity">Eccentricity of orbit
        /// </param>
        /// <param name="density">Average planetary density in grams per cubic centimeter
        /// </param>
        /// <param name="orb_radius">Semi-major axis of orbit in AU
        /// </param>
        /// <param name="orb_period">Orbital period (year) in Earth days. (???)
        /// </param>
        /// <param name="giant">True if this planet is a gas giant
        /// </param>
        /// <returns>s Length of day in Earth hours.
        /// </returns>
        public virtual double day_length(double mass, double radius, double eccentricity, double density, double orb_radius, double orb_period, bool giant)
        {
            double base_angular_velocity, planetary_mass_in_grams, k2, ang_velocity, equatorial_radius_in_cm, change_in_angular_velocity, spin_resonance_factor, year_in_hours, day_in_hours;
            bool stopped = false;

            resonance = 0.0;
            if (giant)
                k2 = 0.24;
            else
                k2 = 0.33;
            planetary_mass_in_grams = mass * PhysicalConstants.EARTH_MASS_IN_GRAMS;
            equatorial_radius_in_cm = radius * PhysicalConstants.CM_PER_KM;
            day_in_hours = year_in_hours = orb_period * 24.0;

            if (mass == 0.0)
                return year_in_hours;
            if (radius == 0.0)
                return year_in_hours;

            base_angular_velocity = System.Math.Sqrt(2.0 * PhysicalConstants.J * (planetary_mass_in_grams) / (k2 * equatorial_radius_in_cm * equatorial_radius_in_cm));
            //  This next calculation determines how much the Planet's rotation is
            //  slowed by the presence of the star.
            change_in_angular_velocity = PhysicalConstants.CHANGE_IN_EARTH_ANG_VEL * (density / PhysicalConstants.EARTH_DENSITY) * (equatorial_radius_in_cm / PhysicalConstants.EARTH_RADIUS) * (PhysicalConstants.EARTH_MASS_IN_GRAMS / planetary_mass_in_grams) * (stell_mass_ratio * stell_mass_ratio) * (1.0 / System.Math.Pow(orb_radius, 6.0));
            ang_velocity = base_angular_velocity + (change_in_angular_velocity * age);
            // Now we change from rad/sec to hours/rotation.			    
            if (ang_velocity <= 0.0)
                stopped = true;
            else
                day_in_hours = PhysicalConstants.RADIANS_PER_ROTATION / (PhysicalConstants.SECONDS_PER_HOUR * ang_velocity);
            if ((day_in_hours >= year_in_hours) || stopped)
            {
                resonance = 1.0; // had been only w/large eccentricity, but...
                if (eccentricity > 0.1)
                {
                    spin_resonance_factor = (1.0 - eccentricity) / (1.0 + eccentricity);
                    return (spin_resonance_factor * year_in_hours);
                }
                else
                    return (year_in_hours);
            }
            return (day_in_hours);
        }

        /// <summary> Estimates planetary inclination (axial tilt).  Calibrated to Earth.</summary>
        /// <param name="orb_radius">Semi-major axis of orbit in AU
        /// </param>
        /// <returns>s Tilt in degrees
        /// </returns>
        public virtual int inclination(double orb_radius)
        {
            int temp;

            //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
            temp = (int)(System.Math.Pow(orb_radius, 0.2) * about(PhysicalConstants.EARTH_AXIAL_TILT, 0.4));
            return (temp % 360);
        }

        /// <summary> This function implements the escape velocity calculation.  Note that   
        /// it appears that Fogg's eq.15 is incorrect.
        /// </summary>
        /// <param name="mass">Mass in Earth masses.
        /// </param>
        /// <param name="radius">Radius in kilometers.
        /// </param>
        /// <returns>s Escape velocity in cm/sec.
        /// </returns>
        public virtual double escape_vel(double mass, double radius)
        {
            double mass_in_grams, radius_in_cm;

            if (radius == 0.0)
                return 0.0;

            mass_in_grams = mass * PhysicalConstants.EARTH_MASS_IN_GRAMS;
            radius_in_cm = radius * PhysicalConstants.CM_PER_KM;
            return (System.Math.Sqrt(2.0 * PhysicalConstants.GRAV_CONSTANT * mass_in_grams / radius_in_cm));
        }

        /// <summary> Calculates Root Mean Square (RMS) velocity of a molecule or atom.
        /// This is Fogg's eq.16.  Calibrated to Earth exospheric temperature,
        /// which implies that the orbital radius has been preadjusted so that
        /// temperature comparisons are meaningful.
        /// </summary>
        /// <param name="molecular_weight">The molecular weight (usually assumed to be N2)
        /// of the molecule or atom in question.
        /// </param>
        /// <param name="orb_radius">Semi-major axis of orbit in AU
        /// </param>
        /// <returns>s RMS velocity in cm/sec^2
        /// </returns>
        public virtual double rms_vel(double molecular_weight, double orb_radius)
        {
            double exospheric_temp;

            exospheric_temp = PhysicalConstants.EARTH_EXOSPHERE_TEMP / (orb_radius * orb_radius);
            return (System.Math.Sqrt((3.0 * PhysicalConstants.MOLAR_GAS_CONST * exospheric_temp) / molecular_weight) * PhysicalConstants.CM_PER_METER);
        }

        /// <summary> Calculates the smallest molecular weight retained by the    
        /// body, which is useful for determining the atmosphere composition.
        /// </summary>
        /// <param name="mass">Planetary mass in Earth masses.
        /// </param>
        /// <param name="equat_radius">Equatorial radius in kilometers.
        /// </param>
        /// <returns>s Molecular weight (in ???)
        /// </returns>
        public virtual double molecule_limit(double mass, double equat_radius)
        {
            double esc_velocity;

            esc_velocity = escape_vel(mass, equat_radius);
            if (esc_velocity == 0.0)
                return 0.0;
            return ((3.0 * System.Math.Pow(PhysicalConstants.GAS_RETENTION_THRESHOLD * PhysicalConstants.CM_PER_METER, 2.0) * (PhysicalConstants.MOLAR_GAS_CONST * PhysicalConstants.EARTH_EXOSPHERE_TEMP)) / (esc_velocity * esc_velocity));
        }

        /// <summary> Calculates the surface acceleration of a Planet.</summary>
        /// <param name="mass">Planetary mass in Earth masses.
        /// </param>
        /// <param name="radius">Equatorial radius in kilometers.
        /// </param>
        /// <returns>s Acceleration in cm/sec^2
        /// </returns>
        public virtual double acceleration(double mass, double radius)
        {
            if (radius == 0.0)
                return 0.0;
            return (PhysicalConstants.GRAV_CONSTANT * (mass * PhysicalConstants.EARTH_MASS_IN_GRAMS) / System.Math.Pow(radius * PhysicalConstants.CM_PER_KM, 2.0));
        }

        /// <summary> Calculates the surface gravity of the planet.</summary>
        /// <param name="acceleration">Surface gravity in cm/sec^2
        /// </param>
        /// <returns>s Surface gravity in Earth gravities.
        /// </returns>
        public virtual double gravity(double acceleration)
        {
            return (acceleration / PhysicalConstants.EARTH_ACCELERATION);
        }

        /// <summary> Determines if the planet suffers from runaway greenhouse effect.
        /// Note that if the orbital radius of the Planet is greater than or equal  
        /// to R_inner, 99% of it's volatiles are assumed to have been deposited in 
        /// surface reservoirs (otherwise, it suffers from the greenhouse effect).
        /// </summary>
        /// <param name="zone">Orbital 'zone'.
        /// </param>
        /// <param name="orb_radius">Semi-major axis of orbit, in AU
        /// </param>
        /// <param name="r_greenhouse">'Greenhouse effect' radius in AU
        /// </param>
        /// <returns>s 'true' if planet is a greenhouse.
        /// </returns>
        public virtual bool grnhouse(int zone, double orb_radius, double r_greenhouse)
        {
            if ((orb_radius < r_greenhouse) && (zone == 1))
                return (true);
            else
                return (false);
        }

        /// <summary> Calculates the unitless 'volatile gas inventory'.
        /// This implements Fogg's eq.17.
        /// </summary>
        /// <param name="mass">Planetary mass in Earth masses.
        /// </param>
        /// <param name="escape_vel">Escape velocity in kilometers/second.
        /// </param>
        /// <param name="rms_vel">Average velocity at molecules at top of atmosphere.
        /// </param>
        /// <param name="stellar_mass">Stellar mass ration in Solar masses.
        /// </param>
        /// <param name="zone">Orbital 'zone', between 1 and 3.
        /// </param>
        /// <param name="greenhouse_effect">True if planet has runaway greenhouse effect.
        /// </param>
        /// <returns>s A unitless 'inventory' calibrated to Earth=1000.
        /// </returns>
        public virtual double vol_inventory(double mass, double escape_vel, double rms_vel, double stellar_mass, int zone, bool greenhouse_effect)
        {
            double velocity_ratio, proportion_const = 1.0, temp1, temp2, earth_units;

            if (rms_vel == 0.0)
                return 0.0;
            velocity_ratio = escape_vel / rms_vel;
            if (velocity_ratio >= PhysicalConstants.GAS_RETENTION_THRESHOLD)
            {
                switch (zone)
                {

                    case 1:
                        proportion_const = 100000.0;
                        break;

                    case 2:
                        proportion_const = 75000.0;
                        break;

                    case 3:
                        proportion_const = 250.0;
                        break;

                    default:
                        System.Console.Out.WriteLine("Error: orbital zone not initialized correctly!\n");
                        break;

                }
                earth_units = mass;
                temp1 = (proportion_const * earth_units) / stellar_mass;
                temp2 = about(temp1, 0.2);
                if (greenhouse_effect)
                    return (temp2);
                else
                    return (temp2 / 100.0);
            }
            else
                return (0.0);
        }

        /// <summary> Calculates surface pressure on this planet. Uses volatile gas inventory,
        /// equatorial radius, and surface gravity, and is calibrated so that Earth
        /// (with a gas inventory of 1000 and gravity of 1 G) has a pressure of 1000
        /// millibars. This implements Fogg's eq.18.
        /// </summary>
        /// <param name="equat_radius">Equatorial radius in kilometers.
        /// </param>
        /// <param name="gravity">Surface gravity in units of Earth gravities.
        /// </param>
        /// <returns>s Surface pressure in millibars.
        /// </returns>
        public virtual double pressure(double equat_radius, double gravity)
        {
            if (equat_radius == 0.0)
                return 0.0;
            equat_radius = PhysicalConstants.KM_EARTH_RADIUS / equat_radius;
            return (volatile_gas_inventory * gravity / (equat_radius * equat_radius));
        }

        /// <summary> Calculates the boiling point of water in this planet's atmosphere,   
        /// stored in millibars. This is Fogg's eq.21.
        /// </summary>
        /// <returns>s Boiling point in degrees Kelvin.
        /// </returns>
        public virtual double boiling_point()
        {
            double surface_pressure_in_bars;

            surface_pressure_in_bars = surf_pressure / PhysicalConstants.MILLIBARS_PER_BAR;
            if (surface_pressure_in_bars == 0)
                return 0.0;
            return (1.0 / (System.Math.Log(surface_pressure_in_bars) / (-5050.5) + 1.0 / 373.0));
        }

        /// <summary> Calculates the fraction of planet surface covered by water.
        /// This function is Fogg's eq.22. Uses the volatile gas inventory 
        /// (a dimensionless quantity, calibrated to Earth==1000) and   
        /// planetary radius in Km.                             
        /// I [Matt] have changed the function very slightly:  the fraction of Earth's    
        /// surface covered by water is 71%, not 75% as Fogg used.                 
        /// </summary>
        public virtual double hydro_fraction()
        {
            double temp;

            if (radius == 0.0)
                return 0.0;
            temp = (0.71 * volatile_gas_inventory / 1000.0) * System.Math.Pow(PhysicalConstants.KM_EARTH_RADIUS / radius, 2.0);
            if (temp >= 1.0)
                return (1.0);
            else
                return (temp);
        }

        /// <summary>   Given the surface temperature of a Planet (in Kelvin), this function   
        /// returns the fraction of cloud cover available.  This is Fogg's eq.23.  
        /// See Hart in "Icarus" (vol 33, pp23 - 39, 1978) for an explanation.     
        /// This equation is Hart's eq.3.                                          
        /// I [Matt] have modified it slightly using constants and relationships from     
        /// Glass's book "Introduction to Planetary Geology", p.46.                
        /// The 'CLOUD_COVERAGE_FACTOR' is the amount of surface area on Earth     
        /// covered by one Kg. of cloud.					    
        /// </summary>
        public virtual double cloud_fraction()
        {
            double water_vapor_in_kg, fraction, surf_area, hydro_mass;

            if (radius == 0.0)
                return 0.0;
            if (surf_pressure == 0.0)
                return 0.0;
            if (molec_weight > PhysicalConstants.WATER_VAPOR)
                return (0.0);

            surf_area = 4.0 * Math.PI * (radius * radius);
            hydro_mass = hydrosphere * surf_area * PhysicalConstants.EARTH_WATER_MASS_PER_AREA;
            if (hydro_mass <= 0.0)
                return 0.0;
            // log is used to reduce chance of overflow, which had happened
            // in some previous implementations on some systems.
            water_vapor_in_kg = System.Math.Log(0.00000001 * hydro_mass);
            water_vapor_in_kg += (PhysicalConstants.Q2_36 * (surf_temp - 288.0));
            fraction = System.Math.Log(PhysicalConstants.CLOUD_COVERAGE_FACTOR) + water_vapor_in_kg - System.Math.Log(surf_area);
            if (fraction >= 0.0)
                return (1.0);
            else
                return (System.Math.Exp(fraction));
        }

        /// <summary>   Given the surface temperature of a Planet (in Kelvin), this function   
        /// returns the fraction of the Planet's surface covered by ice.  This is  
        /// Fogg's eq.24.  See Hart[24] in Icarus vol.33, p.28 for an explanation. 
        /// I [Matt] have changed a constant from 70 to 90 in order to bring it more in   
        /// line with the fraction of the Earth's surface covered with ice, which  
        /// is approximatly .016 (=1.6%).                                          
        /// </summary>
        public virtual double ice_fraction()
        {
            double temp = surf_temp; // don't change actual temperature here!

            if (temp > 328.0)
                temp = 328.0;
            temp = System.Math.Pow(((328.0 - temp) / 90.0), 5.0);
            if (temp > (1.5 * hydrosphere))
                temp = (1.5 * hydrosphere);
            if (temp >= 1.0)
                temp = 1.0;
            return (temp);
        }

        /// <summary> Calculates effective temperature of the planet, based on semi-major
        /// axis, albedo, and provided ecosphere radius.  The equation is calibrated
        /// to the effective temperature of Earth.
        /// This is Fogg's eq.19.		    
        /// </summary>
        /// <param name="ecosphere_radius">Radius of ecosphere in AU
        /// </param>
        /// <returns>s Effective temperature in degrees Kelvin
        /// </returns>
        public virtual double eff_temp(double ecosphere_radius)
        {
            return (System.Math.Sqrt(ecosphere_radius / a) * System.Math.Pow((1.0 - albedo) / 0.7, 0.25) * PhysicalConstants.EARTH_EFFECTIVE_TEMP);
        }

        /// <summary> Calculates the rise in temperature due to greenhouse effect.
        /// This is Fogg's eq.20, and is also Hart's eq.20 in his "Evolution of     
        /// Earth's Atmosphere" article.                                   
        /// </summary>
        /// <param name="optical">depth Dimensionless quantity representing atmospheric absorption
        /// </param>
        /// <param name="effective_temp">Temperature in Kelvin of a blackbody here
        /// </param>
        /// <returns>s Temperature rise in degrees Kelvin
        /// </returns>
        public virtual double green_rise(double optical_depth, double effective_temp)
        {
            double convection_factor;

            convection_factor = PhysicalConstants.EARTH_CONVECTION_FACTOR * System.Math.Pow(surf_pressure / PhysicalConstants.EARTH_SURF_PRES_IN_MILLIBARS, 0.25);
            return (System.Math.Pow(1.0 + 0.75 * optical_depth, 0.25) - 1.0) * effective_temp * convection_factor;
        }

        /// <summary>   Calculates the albedo of the planet, which is the fraction of light
        /// reflected rather than absorbed.
        /// The cloud adjustment is the fraction of cloud cover obscuring each     
        /// of the three major components of albedo that lie below the clouds.     
        /// </summary>
        public virtual double planet_albedo()
        {
            double water_fraction = hydrosphere;
            double cloud_fraction = cloud_cover;
            double ice_fraction = ice_cover;
            double rock_fraction, cloud_adjustment, components, cloud_part, rock_part, water_part, ice_part;

            rock_fraction = (1.0 - water_fraction - ice_fraction);
            components = 0.0;
            if (water_fraction > 0.0)
                components += 1.0;
            if (ice_fraction > 0.0)
                components += 1.0;
            if (rock_fraction > 0.0)
                components += 1.0;
            cloud_adjustment = cloud_fraction / components;
            if (rock_fraction >= cloud_adjustment)
                rock_fraction -= cloud_adjustment;
            else
                rock_fraction = 0.0;
            if (water_fraction > cloud_adjustment)
                water_fraction -= cloud_adjustment;
            else
                water_fraction = 0.0;
            if (ice_fraction > cloud_adjustment)
                ice_fraction -= cloud_adjustment;
            else
                ice_fraction = 0.0;
            cloud_part = cloud_fraction * about(PhysicalConstants.CLOUD_ALBEDO, 0.2);
            rock_part = rock_fraction * ((surf_pressure == 0.0) ? about(PhysicalConstants.ROCKY_AIRLESS_ALBEDO, 0.3) : about(PhysicalConstants.ROCKY_ALBEDO, 0.1));
            water_part = water_fraction * about(PhysicalConstants.WATER_ALBEDO, 0.2);
            ice_part = ice_fraction * ((surf_pressure == 0.0) ? about(PhysicalConstants.AIRLESS_ICE_ALBEDO, 0.4) : about(PhysicalConstants.ICE_ALBEDO, 0.1));
            return (cloud_part + rock_part + water_part + ice_part);
        }

        /// <summary> Calculates the albedo of the planet, which is the fraction of light
        /// reflected rather than absorbed.  This routine expects the albedo
        /// contribution for each of the components to be precalculated rather
        /// than randomly varied on each call; this is to accellerate execution
        /// of the planetary temperature loop.</p>
        /// The cloud adjustment is the fraction of cloud cover obscuring each     
        /// of the three major components of albedo that lie below the clouds.     
        /// We assume that each surface component is obscured to an equal degree.
        /// </summary>
        public virtual double new_planet_albedo()
        {
            double water_fraction = hydrosphere;
            double cloud_fraction = cloud_cover;
            double ice_fraction = ice_cover;
            double rock_fraction, /*cloud_adjustment, components,*/ cloud_part, rock_part, water_part, ice_part;
            double rock_adjustment, water_adjustment, ice_adjustment;
            double result;

            rock_fraction = 1.0 - water_fraction - ice_fraction;
            if (rock_fraction < 0.0)
                rock_fraction = 0.0;
            rock_adjustment = cloud_fraction * rock_fraction;
            water_adjustment = cloud_fraction * water_fraction;
            ice_adjustment = cloud_fraction * ice_fraction;
            rock_fraction = (rock_fraction >= rock_adjustment) ? (rock_fraction - rock_adjustment) : 0.0;
            water_fraction = (water_fraction >= water_adjustment) ? (water_fraction - water_adjustment) : 0.0;
            ice_fraction = (ice_fraction >= ice_adjustment) ? (ice_fraction - ice_adjustment) : 0.0;
            cloud_part = cloud_fraction * cloud_factor;
            rock_part = rock_fraction * ((surf_pressure == 0.0) ? airless_rock_factor : rock_factor);
            ice_part = ice_fraction * ((surf_pressure == 0.0) ? airless_ice_factor : ice_factor);
            water_part = water_fraction * water_factor;
            result = (cloud_part + rock_part + water_part + ice_part);
            if (result < 0.0)
                result = 0.0;
            return result;
        }

        /// <summary>   This function returns the dimensionless quantity of optical depth,     
        /// which is useful in determining the amount of greenhouse effect on a    
        /// Planet.                                                                
        /// </summary>
        public virtual double opacity()
        {
            double optical_depth = 0.0;
            double molecular_weight = molec_weight;
            if ((molecular_weight > 0.0) && (molecular_weight < 10.0))
                optical_depth += 3.0;
            if ((molecular_weight >= 10.0) && (molecular_weight < 20.0))
                optical_depth += 2.34;
            if ((molecular_weight >= 20.0) && (molecular_weight < 30.0))
                optical_depth += 1.0;
            if ((molecular_weight >= 30.0) && (molecular_weight < 45.0))
                optical_depth += 0.15;
            if ((molecular_weight >= 45.0) && (molecular_weight < 100.0))
                optical_depth += 0.05;
            if (surf_pressure >= (70.0 * PhysicalConstants.EARTH_SURF_PRES_IN_MILLIBARS))
                optical_depth *= 8.333;
            else if (surf_pressure >= (50.0 * PhysicalConstants.EARTH_SURF_PRES_IN_MILLIBARS))
                optical_depth *= 6.666;
            else if (surf_pressure >= (30.0 * PhysicalConstants.EARTH_SURF_PRES_IN_MILLIBARS))
                optical_depth *= 3.333;
            else if (surf_pressure >= (10.0 * PhysicalConstants.EARTH_SURF_PRES_IN_MILLIBARS))
                optical_depth *= 2.0;
            else if (surf_pressure >= (5.0 * PhysicalConstants.EARTH_SURF_PRES_IN_MILLIBARS))
                optical_depth *= 1.5;
            return (optical_depth);
        }

        protected internal const int CHAOTIC_ITERATIONS = 100;
        /// <summary> Iteratively calculates the temperature of the atmosphere.
        /// For most planets, this iteration terminates very quickly, but for
        /// some planets the state is chaotic.  Typically, at least one planet
        /// will be discovered within a few hundred systems for which this
        /// equation <em>never</em> converges. These are primarily Earthlike planets
        /// where surface temperature is near the freezing point of water,
        /// but it has also occurred in planets near the boiling point as well
        /// (typically high pressure worlds with large oceans).  Adjust the
        /// CHAOTIC_ITERATIONS parameter to change the cutoff point.
        /// <p>It may be desirable to iteratively calculate the volatiles inventory
        /// as well, to simulate atmospheric evolution; doing this <em>correctly</em>
        /// is beyond what I [Carl] feel comfortable with.</p>
        /// </summary>
        public virtual void iterate_surface_temp()
        {
            double effective_temp, greenhs_rise, previous_temp;
            double total_temp;

            // albedo factors; declared constant for each planet
            cloud_factor = about(PhysicalConstants.CLOUD_ALBEDO, 0.2);
            airless_rock_factor = about(PhysicalConstants.ROCKY_AIRLESS_ALBEDO, 0.3);
            rock_factor = about(PhysicalConstants.ROCKY_ALBEDO, 0.1);
            water_factor = about(PhysicalConstants.WATER_ALBEDO, 0.2);
            airless_ice_factor = about(PhysicalConstants.AIRLESS_ICE_ALBEDO, 0.4);
            ice_factor = about(PhysicalConstants.ICE_ALBEDO, 0.1);

            its = 0;
            temp_unstable = false;
            albedo = PhysicalConstants.EARTH_ALBEDO;
            effective_temp = eff_temp(r_ecosphere);
            greenhs_rise = green_rise(opacity(), effective_temp);
            surf_temp = effective_temp + greenhs_rise;
            total_temp = surf_temp;
            previous_temp = surf_temp - 5.0; // force the while loop the first time 
            while ((System.Math.Abs(surf_temp - previous_temp) > 1.0))
            {
                previous_temp = surf_temp;
                hydrosphere = hydro_fraction();
                cloud_cover = cloud_fraction();
                ice_cover = ice_fraction();
                if ((surf_temp >= boil_point) || (surf_temp <= PhysicalConstants.FREEZING_POINT_OF_WATER))
                    hydrosphere = 0.0;
                albedo = new_planet_albedo();
                effective_temp = eff_temp(r_ecosphere);
                greenhs_rise = green_rise(opacity(), effective_temp);
                surf_temp = effective_temp + greenhs_rise;
                if (its == 0)
                {
                    min_temp = surf_temp;
                    max_temp = surf_temp;
                }
                if (surf_temp < min_temp)
                    min_temp = surf_temp;
                if (surf_temp > max_temp)
                    max_temp = surf_temp;
                its++;
                total_temp += surf_temp;
                if (its > CHAOTIC_ITERATIONS)
                {
                    temp_unstable = true;
                    break; // abandon search here; declare unstable
                }
            }
            avg_temp = total_temp / ((double)(its + 1));
        }

        /// <summary> Calculates planetary characteristics. At this point the body
        /// of the planet has already been calculated (mass, orbital characteristics,
        /// whether it accreted gas as well as dust); remaining characteristics
        /// are calculated using stellar characteristics provided as parameters.
        /// </summary>
        /// <param name="smr">Stellar mass ratio (in Solar masses)
        /// </param>
        /// <param name="r_gr">Greenhouse radius (in AU)
        /// </param>
        /// <param name="r_ec">Ecosphere radius (in AU)
        /// </param>
        /// <param name="age">Age of star (in gigayears)
        /// </param>
        public virtual void set_vital_stats(double smr, double r_gr, double r_ec, double age)
        {
            r_ecosphere = r_ec;
            stell_mass_ratio = smr;
            //age = a;
            resonance = 0.0;

            if (gas_giant)
            {
                density = empirical_density(mass, a, gas_giant);
                radius = volume_radius(mass, density);
            }
            else
            {
                radius = kothari_radius(mass, gas_giant, orbit_zone);
                density = volume_density(mass, radius);
            }
            orb_period = period(a, mass / PhysicalConstants.SUN_MASS_IN_EARTH_MASSES, smr);
            day = day_length(mass, radius, e, density, a, orb_period, gas_giant);
            resonant_period = (int)resonance;
            axial_tilt = inclination(a);
            esc_velocity = escape_vel(mass, radius);
            surf_accel = acceleration(mass, radius);
            rms_velocity = rms_vel(PhysicalConstants.MOL_NITROGEN, a);
            molec_weight = molecule_limit(mass, radius);
            if ((gas_giant))
            {
                surf_grav = PhysicalConstants.INCREDIBLY_LARGE_NUMBER;
                greenhouse_effect = false;
                volatile_gas_inventory = PhysicalConstants.INCREDIBLY_LARGE_NUMBER;
                surf_pressure = PhysicalConstants.INCREDIBLY_LARGE_NUMBER;
                boil_point = PhysicalConstants.INCREDIBLY_LARGE_NUMBER;
                hydrosphere = PhysicalConstants.INCREDIBLY_LARGE_NUMBER;
                albedo = about(PhysicalConstants.GAS_GIANT_ALBEDO, 0.1);
                surf_temp = PhysicalConstants.INCREDIBLY_LARGE_NUMBER;
            }
            else
            {
                surf_grav = gravity(surf_accel);
                greenhouse_effect = grnhouse(orbit_zone, a, r_gr);
                volatile_gas_inventory = vol_inventory(mass, esc_velocity, rms_velocity, smr, orbit_zone, greenhouse_effect);
                surf_pressure = pressure(radius, surf_grav);
                // sometimes airless rocks are showing a greenhouse effect;
                // remove that effect if the rock has very little air.
                if (surf_pressure < 0.01)
                    greenhouse_effect = false;
                boil_point = (surf_pressure == 0.0) ? 0.0 : boiling_point();
                iterate_surface_temp();
            }
        }

        /// <summary> Calculates distance to closest moon, if any.  I {Carl] am unsure
        /// about the logic behind this routine, and it is currently unused.
        /// It might be related to the size of the Roche lobes of this planet,
        /// but I don't have the time right now to check it.
        /// </summary>
        /// <returns>s Distance in AU
        /// </returns>
        public virtual double nearest_moon()
        {
            return (0.3 / 40.0 * System.Math.Pow(mass / PhysicalConstants.SUN_MASS_IN_EARTH_MASSES, (1.0 / 3.0)));
        }

        /// <summary> Calculates distance to farthest moon, if any.  I {Carl] am unsure
        /// about the logic behind this routine, and it is currently unused.
        /// It might be related to the size of the accretion disk around this planet.
        /// </summary>
        /// <returns>s Distance in AU
        /// </returns>
        public virtual double farthest_moon()
        {
            return (50.0 / 40.0 * System.Math.Pow(mass / PhysicalConstants.SUN_MASS_IN_EARTH_MASSES, (1.0 / 3.0)));
        }

        #region cartesian coordinates
        //maybe those can be moved to separate object and inherit here

        public double X
        {
            get
            {
                float snt = (float)Math.Sin(where_in_orbit * Math.PI / 180);
                float cnp = (float)Math.Cos(e * Math.PI / 180);
                return (this.a * PhysicalConstants.KM_PER_AU * snt * cnp) / 1000000;
            }
        }

        public double Y
        {
            get
            {
                float cnt = (float)Math.Cos(where_in_orbit * Math.PI / 180);
                return (this.a * PhysicalConstants.KM_PER_AU * cnt) / 1000000;
            }
        }

        public double Z
        {
            get
            {
                float snt = (float)Math.Sin(where_in_orbit * Math.PI / 180);
                float snp = (float)Math.Sin(e * Math.PI / 180);
                return (-this.a * PhysicalConstants.KM_PER_AU * snt * snp) / 1000000;
            }
        }

        public double Velocity
        {
            get
            {
                return 1 / Math.Pow(a * PhysicalConstants.KM_PER_AU, .5) * 1000000;
            }
        }
        #endregion
    }
}
