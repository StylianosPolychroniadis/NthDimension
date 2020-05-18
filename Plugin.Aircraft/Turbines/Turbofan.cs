using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.Aircraft
{
    public class Turbofan : IDisposable
    {
        //// Input Values
        private double M_0;            // Free Stream Mach Number
        private double alpha;          // Bypass Ratio
        private double pi_c;           // Compression Ratio
        private double pi_f;           // Fan Pressure Ratio
        private double h;              // Current Engine Altitude [ft]
        private double pi_dmax;        // Max Pressure Ratio During Deceleration
        private double ec;             // Polytropic Factor, Compressor
        private double ef;             // Polytropic Factor, Fan
        private double et;             // Polytropic Factor, Turbine
        private double eta_b;          // Combustion Efficiency
        private double eta_m;          // Mechanical Efficiency
        private double pi_n;           // Nozzle Efficiency
        private double P0P7;           // Pressure Ratio Free Stream and Core
        private double P0P9;           // Pressure Ratio Free Stream and Fan
        private double hpr;            // Fuel Heating Value [BTU/lbm]
        private double T0_4;           // Flame Temperature [R]
        private double th;             // Throttle Setting [0 - 1]
        private bool bae;              // Bleed Air Usage On or Off [true or false]
        private double d;              // Engine Inlet Diameter [ft]
        private double mr;             // Mole Ratio of Fuel to CO2
        private double fw;             // Fuel Weight [lbs/gallon]

        //// Conditional Input Values
        private double a_0;            // Ambient Speed of Sound [ft/s]
        private double T_0;            // Free Stream Air Temperature [R]
        private double P_0;            // Free Stream Air Pressure [lb/ft^2]
        private double Rho_0;          // Free Stream Air Density [slugs/ft^3]
        private double Rho_SL;         // Free Stream Air Density at Sea Level [slugs/ft^3]

        //// Output Values
        private double U_7;            // Core Exhaust Speed
        private double U_9;            // Fan Exhaust Speed
        private double TSFC;           // Thrust Specific Fuel Consumption    
        private double PNdb;           // Perceived Noise Level, Based on Input SLS Thrust [lbs] And Distance [ft]
        private double Th;             // Current Engine Thrust [lbs] Based on Throttle Setting
        private double W;              // Engine Dry Weight Estimate [lbs]
        private double L;              // Engine Length Estimate [in]
        private double fe;             // Fuel C02 Emission Rate [gals/min]
        private double ff;             // Fuel Flow [lbs/min]
        private double tfe = 0;        // Total C02 Emissions [lbs]
        private double tff = 0;        // Total Fuel Used [gals]

        //// Private Scientific Constants
        private double g = 32.2;                                // Gravity [ft/s^2]
        private double gc = 1.4;                                // Ratio of Specific Heats [cold air](c_p/c_v)
        private double gt = 1.33;                               // Ratio of Specific Heats [hot air](c_p/c_v)
        private double cpc = 0.24;                              // Cold Air Specific Heat [BTU/(lmb-R)]
        private double cpt = 0.276;                             // Hot Air Specific Heat [BTU/(lmb-R)]
        private double Rc = (1.4 - 1) / 1.4 * 0.24 * 778;       // Gas Constant, Cold Air, (BTU=778) -> [(ft-lbf)/(lbm R)]
        private double Rt = (1.33 - 1) / 1.33 * 0.276 * 778;    // Gas Constant, Cold Air, (BTU=778) -> [(ft-lbf)/(lbm R)]

        //// Private Internal Engine Data
        private double F_s;             // Specific Thrust
        private double SLTFSC;          // Thrust Specific Fuel Consumption at Sea Level
        private double eta_p;           // Propulsive Efficiency
        private double eta_t;           // Thermal Efficiency
        private double eta_o;           // Overall Efficiency
        private double f;               // Fuel to Air Mass Flow Ratio
        private double tau_r;           // Ram Temperature Ratio
        private double pi_r;            // Ram Pressure Ratio
        private double eta_r;           // Ram Efficiency
        private double T0_2;            // Stagnation Temperature @2
        private double P0_2;            // Stagnation Pressure @2
        private double pi_d;            // Deceleration Pressure Ratio
        private double T0_3;            // Stagnation Temperature @3
        private double P0_3;            // Stagnation Pressure @3
        private double tau_c;           // Compressor Temperature Ratio
        private double eta_c;           // Compressor Efficiency
        private double pi_b;            // Ideal Combustion
        private double P0_4;            // Combustion Pressure @4
        private double tau_b;           // Combustion Temperature Ratio
        private double tau_lambda;      // Overall Temperature Ratio
        private double tau_f;           // Fan Temperature Ratio
        private double M_9;             // Fan Exit Mach Number
        private double eta_f;           // Fan Efficiency
        private double tau_t;           // Turbine Temperature Ratio
        private double T0_5;            // Turbine Stagnation Temperature
        private double P0_5;            // Turbine Stagnation Pressure
        private double pi_t;            // Turbine Pressure Ratio
        private double tau_n;           // Nozzle Temp Ratio
        private double P_7;             // Exit Pressure = Free Stream Pressure
        private double P0_7P7;          // Stagnation to Static Pressure Ratio @ 7
        private double P0_7;            // Nozzle Exit Stagnation Pressure
        private double T0_7;            // Nozzle Exit Stagnation Temperature
        private double T7T0;            // Overall Temperature Ratio
        private double T7;              // Exit Temperature
        private double M_7;             // Core Exhaust Mach Number
        private double U_0;             // Freestream Velocity
        private double baf;             // Bleed Air Efficiency Loss
        private double thf;             // Throttle Setting Efficiency Loss
        private double basePNdb = 100;  // Perceived Noise Level Base: 25,000 LB SLS Thrust Geared Engine Engine at 1000 FT
        private double bprPNdb;         // Bypass Ratio Perceived Noise Level
        private double tPNdb;           // Engine Thrust Perceived Noise Level
        private double thPNdb;          // Engine Throttle Perceived Noise Level
        private double NTh;             // Engine Net Thrust at Full Throttle [lbs]
        private double mdot;            // Mass Flow Area
        private double a;               // Engine Inlet Area [ft^2]
        private double m_g;             // Fan Inlet Velocity [ft/s]
        private Int64 t = 0;            // Engine Start Time, Used for Consumption Calculations [ms]
        private Int64 t2 = 0;           // Last Checked Time, Used for Consumption Calculations [ms]

        public Turbofan(double h, double M_0, double hpr, double T0_4, double pi_c, double pi_f, double pi_dmax, double alpha, double ec, double ef, double et, double eta_b, double eta_m, double pi_n, double P0P7, double P0P9, bool bae, double th, double d, double mr, double fw)
        {
            // INITIALIZE THE ENGINE WITH INPUT PARAMETERS //
            this.hpr = hpr; this.T0_4 = T0_4; this.pi_c = pi_c; this.pi_f = pi_f; this.pi_dmax = pi_dmax; this.alpha = alpha; this.ec = ec; this.ef = ef; this.et = et; this.eta_b = eta_b; this.eta_m = eta_m; this.pi_n = pi_n; this.P0P7 = P0P7; this.P0P9 = P0P9; this.bae = bae; this.d = d; this.mr = mr; this.fw = fw;

            // TO ESTIMATE ENGINE WEIGHT, WE NEED TO OBTAIN TAKE OFF THRUST //
            this.h = 5000;                  // Set Altitude to 5000 [ft] (For Takeoff)
            this.th = 1;                    // Set Throttle Setting to Full Throttle (For Takeoff)
            this.M_0 = 0.2;                 // Set Freestream Mach Number (For Takeoff)
            Atmospheric(h);                 // Calculate Atmospheric Conditions Based on Altitude
            Engine_Start();                 // Start The Engine
            Engine_Feedback();              // Calculate Running Engine Data
            Engine_Specs();                 // Calculate Engine Dry Weight

            ////// START ENGINE
            if ((h != this.h) || (M_0 != this.M_0) || (th != this.th))  // See If We Need to Re-Calculate Based on Input
            {
                this.h = h; this.M_0 = M_0; this.th = th;
                Atmospheric(h);                                                                      // Calculate Atmospheric Conditions Based on Altitude
                Engine_Start();                                                                      // Start The Engine
                Engine_Feedback();                                                                   // Calculate Running Engine Data
                t = (Int64)((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000);    // Engine Start Timestamp based on Epoch Time and Date
                t2 = t;                                                                              // Set Last Checked Time to Now
            }
        }
        public void Engine_SetAltitude(double h)
        {
            this.h = h;
            Engine_Usage();             // Calculate Engine Fuel Usage and CO2 Production
            Atmospheric(h);             // Re-Calculate Atmospheric Conditions Based on New Altitude
            Engine_Start();             // Re-Start The Engine
            Engine_Feedback();          // Re-Calculate Running Engine Data
        }
        public void Engine_SetMach(double M_0)
        {
            this.M_0 = M_0;
            Engine_Usage();             // Calculate Engine Fuel Usage and CO2 Production
            Engine_Start();             // Re-Start The Engine
            Engine_Feedback();          // Re-Calculate Running Engine Data
        }
        public void Engine_SetBleedAirUsage(bool bae)
        {
            this.bae = bae;             // Trigger Bleed Air Usage, if On: Turn Off, else Turn On
            Engine_Usage();             // Calculate Engine Fuel Usage and CO2 Production
            Engine_Feedback();          // Re-Calculate Running Engine Data
        }
        private void Atmospheric(double h)
        {
            using (Atmosphere a = new Atmosphere(h))
            {
                a_0 = a.a;          // Ambient Speed of Sound [ft/s] Based on Current Engine Altitude
                T_0 = a.t;          // Free Stream Temperature [R] Based on Current Engine Altitude
                P_0 = a.p;          // Free Stream Pressure [lb/ft^2] Based on Current Engine Altitude
                Rho_0 = a.rho;      // Free Stream Air Density [slugs/ft^3] Based on Current Engine Altitude
                Rho_SL = a.rho_SL;  // Free Stream Air Density [slugs/ft^3] Based on Current Engine Altitude
            }
        }
        private void Engine_Start()
        {
            //// Used to Start Engine [Stages 1 -> 7]
            Ram_Compression();
            Deceleration();
            Compression();
            Combustion();
            Fan();
            Turbine();
            Core_Nozzle();
        }


        private void Engine_Specs()             // Estimated Dry Weight and Length
        {
            W = 0.135 * Th;                     // Curve Fit of Known Engine Weights Based on Takeoff Thrust
            L = 40 + 0.59 * Math.Sqrt(Th);      // Curve Fit of Known Engine Weights Based on Takeoff Thrust
        }
        private void InletFanVelocity() // Inlet Fan Velocity Calculations
        {
            m_g = th * (d * 30 + 300) * Rho_0 * 1000 * ((0.769 * M_0) + 0.346);      // Fan Intake Velocity as a Function of Mach, Rho, Throttle Setting and Fan Diameter
        }
        private void Ram_Compression() // Ram Compression [Stage 0 -> 1]
        {
            tau_r = 1 + ((gc - 1) / 2) * Math.Pow(M_0, 2);      // Ram Temperature Ratio Calculation
            pi_r = Math.Pow(tau_r, (gc / (gc - 1)));            // Ram Pressure Ratio Calculation
            if (M_0 < 1)                                        // Ram Efficiency = 1 if M_0 < 1
                eta_r = 1;                                      // Ram Efficiency Calculation
        }
        private void Deceleration() // Deceleration [Stage 1 -> 2]
        {
            T0_2 = T_0 * ((1 + (gc - 1) / 2) * Math.Pow(M_0, 2));                               // Stagnation Temperature @2
            P0_2 = P_0 * Math.Pow((1 + ((gc - 1) / 2) * Math.Pow(M_0, 2)), (gc / (gc - 1)));    // Stagnation Pressure @2
            pi_d = pi_dmax * eta_r;                                                             // Deceleration Pressure Ratio
        }
        private void Compression() // Compression [Stage 2 -> 3]
        {
            P0_3 = pi_c * P0_2;                                             // Stagnation Pressure @3
            tau_c = Math.Pow(pi_c, ((gc - 1) / (gc * ec)));                 // Compression Temperature Ratio
            T0_3 = tau_c * T0_2;                                            // Stagnation Temperature @3
            eta_c = (Math.Pow(pi_c, ((gc - 1) / gc)) - 1) / (tau_c - 1);    // Compressor Efficiency
        }
        private void Combustion() // Combustion [Stage 3 -> 4]
        {
            pi_b = 1;                                                                                         // Ideal Combustion at Constant Pressure
            P0_4 = P0_3;                                                                                      // Constant Pressure Combustion
            tau_b = T0_4 / T0_3;                                                                              // Combustion Temperature Ratio
            tau_lambda = (cpt * T0_4) / (cpc * T_0);                                                          // Overall Temperature Ratio
            f = ((tau_lambda - (tau_r * tau_c)) / ((eta_b * hpr) / (cpc * T_0) - tau_lambda)) * et;           // Fuel to Air Mass Flow Ratio
        }
        private void Fan() // Fan [Stage 2 -> 8]
        {
            tau_f = Math.Pow(pi_f, ((gc - 1) / (gc * ef)));                 // Fan Temperature ratio
            P0_2 = P_0 * pi_f;                                              // Fan Pressure
            T0_2 = T_0 * tau_f;                                             // Fan Temperature
            M_9 = Math.Sqrt((2 / (gc - 1)) * (tau_r * tau_f - 1));          // Fan Exit Mach Number
            U_9 = M_9 * a_0;                                                // Fan Exit Velocity
            eta_f = (Math.Pow(pi_f, ((gc - 1) / gc)) - 1) / (tau_f - 1);    // Fan Efficiency
        }
        private void Turbine() // Turbine [Stage 4 -> 5]
        {
            tau_t = 1 - (1 / (eta_m * (1 + f))) * (tau_r / tau_lambda) * (tau_c - 1 + alpha * (tau_f - 1));     // Turbine Temperature Ratio
            T0_5 = tau_t * T0_4;                                                                                // Turbine Stagnation Temperature
            pi_t = Math.Pow(tau_t, ((et * gt) / (gt - 1)));                                                     // Turbine Pressure Ratio
            P0_5 = pi_t * P0_4;                                                                                 // Turbine Stagnation Pressure
            eta_t = (1 - tau_t) / (1 - Math.Pow(tau_t, (1 / et)));                                              // Turbine Efficiency
        }
        private void Core_Nozzle() // Core Nozzle [Stage 5 -> 7]
        {
            tau_n = 1;                                                                      // Nozzle Temperature Ratio
            P_7 = P_0;                                                                      // Exit Pressure = Free Stream Pressure
            P0_7P7 = P0P7 * pi_r * pi_d * pi_c * pi_b * pi_t * pi_n;                        // Stagnation to Static Pressure Ratio @ 7
            P0_7 = pi_n * P0_5;                                                             // Nozzle Exit Stagnation Pressure
            T0_7 = T0_5 * tau_n;                                                            // Nozzle Exit Stagnation Temperature
            T7T0 = tau_lambda * tau_t / Math.Pow(P0_7P7, ((gt - 1) / gt)) * (cpc / cpt);    // Overall Temperature Ratio
            T7 = T7T0 * T_0;                                                                // Exit Temperature
            M_7 = Math.Sqrt((2 / (gt - 1)) * (Math.Pow(P0_7P7, ((gt - 1) / gt)) - 1));      // Core Exhaust Mach Number
            U_7 = a_0 * M_7 * Math.Sqrt((gt * Rt * T7) / (gc * Rc * T_0));                  // Core Speed
        }
        private void Engine_Feedback() // Engine Feedback Calculations and Efficiencies
        {
            InletFanVelocity();                                                         // Calculate Inlet Velocity based on Fan Size and Throttle Setting
            U_0 = a_0 * M_0;                                                            // Freestream Velocity
                                                                                        // Specific Thrust for a Turbofan [lbf/(lbm/s)]
            F_s = 1 / (1 + alpha) * (U_7 - U_0) + alpha / (1 + alpha) * (U_9 - U_0);
            TSFC = (th > 0) ? (f / ((1 + alpha) * F_s) * 3600 * g) : 0;                 // Thrust Specific Fuel Consumption [(lbm/hr)/lbf]
            baf = ((0.0731 * th) + 0.0206);                                             // Bleed Air Efficiency Loss (Based on Honeywell AGT1500 Engine) Given a Throttle Setting
            TSFC = (bae) ? TSFC + (TSFC * baf) : TSFC;                                  // If Bleed Air is Used, Reduce the TSFC by Bleed Air Defficiency
                                                                                        // Throttle Setting Efficiency Loss (Based on Honeywell AGT1500 Engine) Given a Throttle Setting
            thf = -0.146 + 0.149 * th + 0.198 * Math.Pow(th, 2) - 0.00496 * Math.Pow(th, 3) - 0.604 * Math.Pow(th, 4) + 0.409 * Math.Pow(th, 5);
            TSFC = TSFC + (-thf * TSFC);                                                // Throttle Setting Efficiency Reduction
            SLTFSC = Math.Pow((Rho_0 / Rho_SL), 0.9) * TSFC;                            // Thrust Specific Fuel Consumption [(lbm/hr)/lbf] at Sea Level
                                                                                        // Propulsive Efficiency
            eta_p = 2 * M_0 * (((U_7 / a_0) - M_0 + alpha * ((U_9 / a_0) - M_0)) / ((Math.Pow(U_7, 2) / Math.Pow(a_0, 2)) - Math.Pow(M_0, 2) + alpha * ((Math.Pow(U_9, 2) / Math.Pow(a_0, 2)) - Math.Pow(M_0, 2))));
            eta_t = 1 - (1 / (tau_r * tau_c));                                          // Thermal Efficiency
            eta_o = eta_p * eta_t;                                                      // Overall Efficiency
            a = Math.PI * Math.Pow(d / 2, 2);                                           // Area of Inlet
                                                                                        // Mass Flow Rate based on Fan Inlet Properties and Freestream Velocity and Fan Efficiency
            mdot = ((m_g * eta_f) > U_0) ? Rho_0 * m_g * eta_f * a : Rho_0 * m_g * a;
            NTh = F_s * mdot;                                                           // Net Thrust, based on Mass Flow Rate
            Th = NTh * th;                                                              // Output Thrust, Based on Throttle Setting
            ff = (TSFC * Th * th / (double)60) / fw;                                    // Fuel Flow Rate [gals/min]
            fe = mr * (TSFC * Th * th / (double)60) * eta_b * (double)44 / (double)170; // Engine C02 Emission Rate [lbs/min]
            Engine_Noise();                                                             // Calculate Engine Noise
        }
        public void Engine_SetThrottle(double th) // Set Engine Throttle, Output Thrust
        {
            this.th = th;               // Set Throttle Based on Input Setting
            Engine_Usage();             // Calculate Engine Fuel Usage and CO2 Production
            Engine_Feedback();          // Re-Calculate Running Engine Data
        }
        private void Engine_Noise() // Determine Engine Noise
        {
            PNdb = (th > 0) ? basePNdb : 0;                                                        // Set The Perceived Noise Level to Base Perceived Noise Level
                                                                                                   // Bypass Ratio Affect on Engine Noise
            bprPNdb = (th > 0) ? 8.67 - 4.19 * alpha + 0.577 * Math.Pow(alpha, 2) - 0.0214 * Math.Pow(alpha, 3) : 0;
            tPNdb = (NTh > 0) ? Math.Log10(NTh / 25000) : 0;                                       // Engine Net Thrust Affect on Engine Noise
            thPNdb = (th > 0) ? -0.00161 * Math.Pow(th * 100, 2) + 0.358 * th * 100 - 19.7 : 0;    // Engine Throttle Setting Affect on Engine Noise
            PNdb = (Math.Abs(tPNdb) > Math.Abs(thPNdb)) ? PNdb + bprPNdb + tPNdb : PNdb + bprPNdb + thPNdb;
        }
        public void Engine_Usage() // Determine Engine Usage
        {
            // Only Run If Engine Has Started
            if (t != 0)
            {
                tff = (double.IsNaN(ff)) ? tff : tff + (((Int64)((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000) - t2) * ((double)1 / (double)60) * ((double)1 / (double)1000) * ff);   // Fuel Consumed [gals]
                tfe = (double.IsNaN(fe)) ? tfe : tfe + (((Int64)((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000) - t2) * ((double)1 / (double)60) * ((double)1 / (double)1000) * fe);   // C02 Emissions [lbs]
                t2 = (Int64)((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000);                                                                                // Set New Timestamp [ms]
            }
        }
        public Int64 Engine_RunTime() // Determine Engine RunTime
        {
            // Only Calculate If Engine Has Started
            if (t != 0)
                return (Int64)((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000) - t;    // RunTime [ms]
            else
                return 0;
        }
        public double _TSFC() { return TSFC; }      // Thrust Specific Fuel Consumption    
        public double _PNdb() { return PNdb * th; } // Perceived Noise Level, Based on Input SLS Thrust [lbs] And Distance [ft]
        public double _Th() { return Th; }          // Current Engine Thrust [lbs] Based on Throttle Setting
        public double _U_7() { return U_7 * th; }   // Core Exhaust Speed
        public double _U_9() { return U_9 * th; }   // Fan Exhaust Speed
        public double _W() { return W; }            // Engine Dry Weight Estimate [lbs]
        public double _L() { return L; }            // Engine Length Estimate [in]
        public double _ff() { return ff; }          // Fuel Flow Rate [gal/min]
        public double _fe() { return fe; }          // Engine C02 Emission Rate [lbs/min]
        public double _tff() { return tff; }        // Total Fuel Used [gal]
        public double _tfe() { return tfe; }        // Total C02 Emissions [lbs]
        public void Dispose() { }
    }
}
