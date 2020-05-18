using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.Aircraft
{
    public class PW1000G : IDisposable
    {
        //// Input Values
        private double _M_0;               // Free Stream Mach Number
        private double _h;                 // Current Engine Altitude [ft]
        private Fuel _fuel;                // Set Fuel Source
        private double _th;                // Throttle Setting [0 - 1]
        private bool _bae;                 // Bleed Air Usage On or Off [True or False]
        private bool _nfl;                 // Nozzel Flaps Closed or Open [True or False]
        private double _alpha;             // Bypass Ratio
        private double _d;                 // Engine Inlet Diameter [ft]

        //// Engine Specific Values Based on Known Engine Data for the PurePower PW1000G and Assumptions
        private double pi_c = 36;         // Compression Ratio
        private double pi_f = 1.7;        // Fan Pressure Ratio
        private double pi_dmax = 0.99;    // Max Pressure Ratio During Deceleration
        private double ec = 0.9;          // Polytropic Factor, Compressor
        private double ef = 0.89;         // Polytropic Factor, Fan
        private double et = 0.89;         // Polytropic Factor, Turbine
        private double eta_b = 0.96;      // Combustion Efficiency
        private double eta_m = 0.99;      // Mechanical Efficiency
        private double pi_n = 0.99;       // Nozzle Efficiency
        private double P0P7 = 0.9;        // Pressure Ratio Free Stream and Core
        private double P0P9 = 0.9;        // Pressure Ratio Free Stream and Fan

        //// Output Values
        private double _TSFC = 0;         // Thrust Specific Fuel Consumption
        private double _PNdb = 0;         // Perceived Noise Level, Based on Input SLS Thrust [lbs] And Distance [ft]
        private double _Th = 0;           // Current Engine Thrust [lbs] Based on Throttle Setting
        private double _U_7;              // Core Exhaust Speed
        private double _U_9;              // Fan Exhaust Speed
        private double _W;                // Engine Dry Weight Estimate [lbs]
        private double _L;                // Engine Length Estimate [in]
        private double _ff;               // Fuel Flow Rate [gal/min]
        private double _fe;               // Engine C02 Emission Rate [lbs/min]
        private double _tff;              // Total Fuel Used [gal]
        private double _tfe;              // Total C02 Emissions [lbs]

        Turbofan engine;

        public PW1000G(double M_0, double h, Fuel fuel, double th, bool bae, bool nfl, double alpha, double d)
        {
            ///// LOAD BASIC TURBOFAN ENGINE
            this._M_0 = M_0; this._h = h; this._fuel = fuel; this._th = th; this._bae = bae; this._nfl = nfl; this._alpha = alpha; this._d = d;
            engine = new Turbofan(h, M_0, fuel.hpr(), fuel.T0_4(), pi_c, pi_f, pi_dmax, alpha, ec, ef, et, eta_b, eta_m, pi_n, P0P7, P0P9, bae, th, d, fuel.mr(), fuel.fw());
            UpdateEngine();                     // Populate Engine Running Data
        }
        public void SetAltitude(double h)
        {
            ///// SET ALTITUDE
            this._h = h;
            engine.Engine_SetAltitude(h);
            UpdateEngine();                     // Populate Engine Running Data
        }
        public void SetThrottle(double th)
        {
            ///// SET THROTTLE
            this._th = th;
            engine.Engine_SetThrottle(th);
            UpdateEngine();                     // Populate Engine Running Data
        }
        public void SetMach(double M_0)
        {
            ///// SET MACH
            this._M_0 = M_0;
            engine.Engine_SetMach(M_0);
            UpdateEngine();                     // Populate Engine Running Data
        }
        public void Engine_SetBleedAirUsage()
        {
            ///// SET BLEED AIR USAGE
            _bae = (_bae) ? false : true;         // Trigger Bleed Air Usage, if On: Turn Off, else Turn On
            engine.Engine_SetBleedAirUsage(_bae);
            UpdateEngine();                       // Populate Engine Running Data
        }
        public void Engine_SetNozzleFlaps()
        {
            ///// SET NOZZLE FLAPS
            _nfl = (_nfl) ? false : true;         // Trigger Nozzle Flaps Usage, if On: Turn Off, else Turn On
            UpdateEngine();                       // Trend Fit Data to PurePower PW1000G Specs
        }
        private void FitTrendData()
        {
            _TSFC = _TSFC * .85;                            // MTU Fact Sheet Claims a Reduction of 15% in Fuel Burn
            _PNdb = ((_PNdb - 20) > 0) ? _PNdb - 20 : 0;    // MTU Fact Sheet Claims a Reduction of 20dB in Engine Noise
            _TSFC = (_nfl) ? _TSFC : _TSFC * .98;           // Flight International Claims a Reduction of 2% in Fuel Burn When Nozzle Flaps Are Closed
            _PNdb = (_nfl) ? _PNdb : _PNdb * 1.2;           // Flight International Claims an Increase of 20% in Engine Noise When Nozzle Flaps Are Closed
            _W = _W - 401.241317;                           // Flight International Claims an Engine Dry Weight Reduction of 182kg (401.241317 lbs)
        }
        private void UpdateEngine()
        {
            _TSFC = engine._TSFC();     // Get Engine Efficiency
            _PNdb = engine._PNdb();     // Get Engine Noise Level
            _Th = engine._Th();         // Get Engine Thrust Output
            _U_7 = engine._U_7();       // Get Engine Core Exhaust Speed
            _U_9 = engine._U_9();       // Get Engine Fan Exhaust Speed
            _W = engine._W();           // Get Engine Dry Weight Estimate [lbs]
            _L = engine._L();           // Engine Length Estimate [in]
            _ff = engine._ff();         // Fuel Flow Rate [gal/min]
            _fe = engine._fe();         // Engine C02 Emission Rate [lbs/min]
            _tff = engine._tff();       // Total Fuel Used [gals]
            _tfe = engine._tfe();       // Total C02 Emissions [lbs]
            FitTrendData();             // Trend Fit Data to PurePower PW1000G Specs
        }
        public double U_7() { return _U_7; }                    // Core Exhaust Speed
        public double U_9() { return _U_9; }                    // Fan Exhaust Speed
        public double TSFC() { return _TSFC; }                  // Thrust Specific Fuel Consumption    
        public double PNdb() { return _PNdb; }                  // Perceived Noise Level, Based on Input SLS Thrust [lbs] And Distance [ft]
        public double Th() { return _Th; }                      // Current Engine Thrust [lbs] Based on Throttle Setting
        public double W() { return _W; }                        // Engine Dry Weight Estimate [lbs]
        public double L() { return _L; }                        // Engine Length Estimate [in]
        public double alpha() { return _alpha; }                // Engine Bypass Ratio
        public double d() { return _d; }                        // Engine Inlet Diameter [ft]
        public Fuel fuel() { return _fuel; }                    // Engine Fuel Type Data
        public bool nfl() { return _nfl; }                      // Engine Nozzle Flaps Configuration (Closed or Open)
        public bool bae() { return _bae; }                      // Engine Bleed Air Configuration (On or Off)
        public double ff() { return _ff; }                      // Fuel Flow Rate [gal/min]
        public double fe() { return _fe; }                      // Engine C02 Emission Rate [lbs/min]
        public double tff() { return _tff; }                    // Total Fuel Used [gal]
        public double tfe() { return _tfe; }                    // Total C02 Emissions [lbs]
        public double t() { return engine.Engine_RunTime(); }   // Engine RunTime [ms]
        public void r() { engine.Engine_Usage(); _tff = engine._tff(); _tfe = engine._tfe(); }
        public void Dispose() { }
    }
}
