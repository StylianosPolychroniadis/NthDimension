using System;


namespace Plugin.StarGen
{
    /// <summary> This 'interface' is used strictly as a holding tank for
    /// constants which are used across multiple classes.
    /// </summary>
    public static class PhysicalConstants
    {
        public readonly static double       RADIANS_PER_ROTATION                = 2 * Math.PI;
        public readonly static double       DEG_TO_RAD                          = Math.PI / 180.0;
        public readonly static double       RAD_TO_DEG                          = 180.0 / Math.PI;
        public readonly static double       ANGSTROM                            = 1.0e-8;
        public readonly static double       G                                   = 6.67e-08; // gravitational constant 
        public readonly static double       MEARTH                              = 6.0e+24; // mass of Earth in kilograms 
        public readonly static double       BK                                  = 1.38e-16; // Boltzmann constant 
        public readonly static double       MH                                  = 1.673e-24; // mass of hydrogen atom 
        public readonly static double       H2                                  = 2.016;
        public readonly static double       H2O                                 = 18.016;
        public readonly static double       N2                                  = 28.016;
        public readonly static double       O2                                  = 32.0;
        public readonly static double       CO2                                 = 44.011;
        //CHANGE_IN_EARTH_ANG_VEL: I presume this is because of tidal forces / our moon, which is relatively big for an earth sized planet
        //maybe replace this with function to calculate the slowing of rotation [Yan]
        public readonly static double       CHANGE_IN_EARTH_ANG_VEL             = (-1.3e-15); // Units of radians/sec/year
        public readonly static double       SOLAR_MASS_IN_GRAMS                 = (1.989e33); // Units of grams           
        public readonly static double       EARTH_MASS_IN_GRAMS                 = (5.977e27); // Units of grams           
        public readonly static double       EARTH_RADIUS                        = (6.378e8); // Units of cm		    
        public readonly static double       EARTH_DENSITY                       = (5.52); // Units of g/cc	    
        public readonly static double       KM_EARTH_RADIUS                     = (6378.0); // Units of km              
        public readonly static double       EARTH_ACCELERATION                  = (981.0); // Units of cm/sec2         
        public readonly static double       EARTH_AXIAL_TILT                    = (23.4); // Units of degrees         
        public readonly static double       EARTH_EXOSPHERE_TEMP                = (1273.0); // Units of degrees Kelvin  
        public readonly static double       SUN_MASS_IN_EARTH_MASSES            = (332775.64);
        public readonly static double       SUN_RADIUS                          = 6.955E+5; //Km
        public readonly static double       EARTH_EFFECTIVE_TEMP                = (255.0); // Units of degrees Kelvin  
        public readonly static double       EARTH_ALBEDO                        = (0.3);
        public readonly static double       CLOUD_COVERAGE_FACTOR               = (1.839e-8); // Km2/kg                   
        public readonly static double       EARTH_WATER_MASS_PER_AREA           = (3.83e15); // grams per square km     
        public readonly static double       EARTH_SURF_PRES_IN_MILLIBARS        = (1000.0);
        public readonly static double       EARTH_CONVECTION_FACTOR             = (0.43); // from Hart, eq.20         
        public readonly static double       FREEZING_POINT_OF_WATER             = (273.0); // Units of degrees Kelvin  
        public readonly static double       DAYS_IN_A_YEAR                      = (365.256); // Earth days per Earth year
        public readonly static double       GAS_RETENTION_THRESHOLD             = (5.0); // ratio of esc vel to RMS vel (was 6.0) 
        public readonly static double       GAS_GIANT_ALBEDO                    = (0.5); // albedo of a gas giant    
        public readonly static double       CLOUD_ALBEDO                        = (0.52);
        public readonly static double       ROCKY_AIRLESS_ALBEDO                = (0.07);
        public readonly static double       ROCKY_ALBEDO                        = (0.15);
        public readonly static double       WATER_ALBEDO                        = (0.04);
        public readonly static double       AIRLESS_ICE_ALBEDO                  = (0.5);
        public readonly static double       ICE_ALBEDO                          = (0.7);
        public readonly static double       SECONDS_PER_HOUR                    = (3600.0);
        public readonly static double       CM_PER_AU                           = (1.495978707e13); // number of cm in an AU    
        public readonly static double       CM_PER_KM                           = (1.0e5); // number of cm in a km     
        public readonly static double       KM_PER_AU                           = CM_PER_AU / CM_PER_KM;
        public readonly static double       CM_PER_METER                        = (100.0);
        public readonly static double       MILLIBARS_PER_BAR                   = (1000.0);
        public readonly static double       KELVIN_CELCIUS_DIFFERENCE           = (273.0);
        public readonly static double       GRAV_CONSTANT                       = (6.672e-8); // units of dyne cm2/gram2  
        public readonly static double       GREENHOUSE_EFFECT_CONST             = (0.93); // affects inner radius..   
        public readonly static double       MOLAR_GAS_CONST                     = (8314.41); // units: g*m2/(sec2*K*mol) 
        public readonly static double       K                                   = (50.0); // K = gas/dust ratio       
        public readonly static double       B                                   = (1.2e-5); // Used in Crit_mass calc   
        public readonly static double       DUST_DENSITY_COEFF                  = (2.0e-3); // A in Dole's paper        
        public readonly static double       ALPHA                               = (5.0); // Used in density calcs    
        public readonly static double       N                                   = (3.0); // Used in density calcs    
        public readonly static double       J                                   = (1.46e-19); // Used in day-length calcs (cm2/sec2 g) 
        public readonly static double       INCREDIBLY_LARGE_NUMBER             = (9.9999e37);
        public readonly static double       ECCENTRICITY_COEFF                  = (0.077);
        //  Now for a few molecular weights (used for RMS velocity calcs):
        //  This table is from Dole's book "Habitable Planets for Man", p. 38

        public readonly static double       ATOMIC_HYDROGEN                     = (1.0); // H   
        public readonly static double       MOL_HYDROGEN                        = (2.0); // H2  
        public readonly static double       HELIUM                              = (4.0); // He  
        public readonly static double       ATOMIC_NITROGEN                     = (14.0); // N   
        public readonly static double       ATOMIC_OXYGEN                       = (16.0); // O   
        public readonly static double       METHANE                             = (16.0); // CH4 
        public readonly static double       AMMONIA                             = (17.0); // NH3 
        public readonly static double       WATER_VAPOR                         = (18.0); // H2O 
        public readonly static double       NEON                                = (20.2); // Ne  
        public readonly static double       MOL_NITROGEN                        = (28.0); // N2  
        public readonly static double       CARBON_MONOXIDE                     = (28.0); // CO  
        public readonly static double       NITRIC_OXIDE                        = (30.0); // NO  
        public readonly static double       MOL_OXYGEN                          = (32.0); // O2  
        public readonly static double       HYDROGEN_SULPHIDE                   = (34.1); // H2S 
        public readonly static double       ARGON                               = (39.9); // Ar  
        public readonly static double       CARBON_DIOXIDE                      = (44.0); // CO2 
        public readonly static double       NITROUS_OXIDE                       = (44.0); // N2O 
        public readonly static double       NITROGEN_DIOXIDE                    = (46.0); // NO2 
        public readonly static double       OZONE                               = (48.0); // O3  
        public readonly static double       SULPH_DIOXIDE                       = (64.1); // SO2 
        public readonly static double       SULPH_TRIOXIDE                      = (80.1); // SO3 
        public readonly static double       KRYPTON                             = (83.8); // Kr  
        public readonly static double       XENON                               = (131.3); // Xe  
        //  The following constants are used in the kothari_radius function

        public readonly static double       A1_20                               = (6.485e12); // All units are in cgs system.  
        public readonly static double       A2_20                               = (4.0032e-8); //   ie: cm, g, dynes, etc.      
        public readonly static double       BETA_20                             = (5.71e12);
        //   The following values are used in determining the fraction of a planet
        //  covered with clouds in function cloud_fraction

        public readonly static double       Q1_36                               = (1.258e19); // grams    
        public readonly static double       Q2_36                               = (0.0698); // 1/Kelvin 
    }
}
