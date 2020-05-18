using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.Aircraft
{
    public class Fuel : IDisposable
    {
        //// Input Values
        private double t = 0;       // Temperature [R]

        //// Output Values
        private double _hpr;        // Fuel Heating Value [BTU/lbm]
        private double _T0_4;       // Flame Temperature [R]
        private double _mr;         // Mole Ratio of Fuel to CO2
        private double _fw;         // Fuel Weight per Gallon
        private string _name;       // Fuel Name

        public Fuel(string a)
        {
            //// INITIALIZE THE JET FUEL BASED ON NAME
            if (a == "Jet A")
            {
                _name = "Jet A";  // Fuel Name
                _hpr = 18400;     // Fuel Heating Value [BTU/lbm]
                _T0_4 = 3000;     // Flame Temperature [R]
                _mr = 12;         // Mole Ratio of Fuel to CO2
                _fw = 6.76;       // Fuel Weight per Gallon
            }
            else if (a == "Jatrol Light")
            {
                _name = "Jatrol Light";  // Fuel Name
                _hpr = 20036.973344798;  // Fuel Heating Value [BTU/lbm]
                _T0_4 = 3450;         // Flame Temperature [R]
                _mr = 12;                // Mole Ratio of Fuel to CO2
                _fw = 8.10419276;        // Fuel Weight per Gallon
            }
            else // DEFAULT FUEL
            {
                _name = "Jet A";  // Fuel Name
                _hpr = 18400;     // Fuel Heating Value [BTU/lbm]
                _T0_4 = 3000;     // Flame Temperature [R]
                _mr = 12;         // Mole Ratio of Fuel to CO2
                _fw = 6.76;       // Fuel Weight per Gallon
            }
        }
        public string name() { return _name; }
        public double hpr() { return _hpr; }
        public double T0_4() { return _T0_4; }
        public double mr() { return _mr; }
        public double fw() { return _fw; }
        public void Dispose() { }
    }
}
