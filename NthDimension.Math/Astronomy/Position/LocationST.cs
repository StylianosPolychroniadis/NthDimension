using System;
using Microsoft.Win32;

namespace NthDimension.Math.Astronomy
{
	/// <summary>
	/// 
	/// </summary>
	public class LocationST
	{
		public int v_HomeLat, v_HomeLon, v_Lat, v_Lon;
		public double oblecl;
		public DateTime v_mainDT;
		public short v_step = 1;
		public string v_unit = "minutes";
		public short rot;

		public DateTime dtMin = new DateTime(1900,1,1,0,0,0);
		public DateTime dtMax = new DateTime(2099,12,31,23,59,59);

		public static LocationST GetInstance()
		{
			if (instance == null)
				instance = new LocationST();
			return instance;
		}

		public double dayNumber()
		{
			d = v_mainDT.Day - 32076 + 1461*(v_mainDT.Year + 
				4800 + (v_mainDT.Month - 14)/12)/4 +
				367*(v_mainDT.Month - 2 - (v_mainDT.Month - 14)/12*12)/12 - 
				3*((v_mainDT.Year + 4900 + (v_mainDT.Month - 14)/12)/100)/4;


			d += (v_mainDT.Hour - (double)v_Lon/15 + 12.0)/24.0;
			d += (v_mainDT.Minute)/1440.0;
			d += (v_mainDT.Second)/86400.0;
			d -= 2451543.5;

			return d;
		}

		public void FixTime()
		{
			if (dts.Year != 1)	dts = dts.AddYears (-(v_mainDT.Year-1));
			if (dte.Year != 1)	dte = dte.AddYears (-(v_mainDT.Year-1));

			dts = dts.AddYears(v_mainDT.Year-1);
			dte = dte.AddYears(v_mainDT.Year-1);

			if (v_mainDT >= dts  &&  v_mainDT <= dte)
				v_mainDT = v_mainDT.AddHours(-1);
		}

		public void TimeNow()
		{
			v_mainDT = DateTime.Now;
		}

		public double SIDTIME{
			get {return v_SIDTIME;}
			set {v_SIDTIME = value;}
		}
		public double xs{
			get {return v_xs;}
			set {v_xs = value;}
		}
		public double ys{
			get {return v_ys;}
			set {v_ys = value;}
		}
		public double zs{
			get {return v_zs;}
			set {v_zs = value;}
		}
		public double slon{
			get {return v_slon;}
			set {v_slon = value;}
		}
		public double sRA{
			get {return v_sRA;}
			set {v_sRA = value;}
		}
		public double sDecl{
			get {return v_sDecl;}
			set {v_sDecl = value;}
		}

		public void ChangeRegistry()
		{
			regKey.SetValue ("Lon", v_HomeLon);
			regKey.SetValue ("Lat", v_HomeLat);
		}

		/// <summary>
		/// Creates Registry Key
		/// 'Software\\Syscon\\NthDimension'
		/// Adds Values
		/// 'Lat'
		/// 'Lon'
		/// </summary>
		private LocationST()
		{
			try
			{
				regKey = Registry.CurrentUser;
				if ((regKey = regKey.OpenSubKey("Software\\Syscon\\NthDimension", true)) == null)
				{
					regKey = Registry.CurrentUser;
					regKey = regKey.OpenSubKey("Software", true);
					regKey.CreateSubKey("Syscon");
					regKey.CreateSubKey(@"Syscon\\NthDimension");

					regKey = Registry.CurrentUser;
					regKey = regKey.OpenSubKey("Software\\Syscon\\NthDimension", true);
					regKey.SetValue("Lon", 16);
					regKey.SetValue("Lat", 46);
					v_HomeLon = v_Lon = 16;
					v_HomeLat = v_Lat = 46;
				}
				else
				{
					v_HomeLon = v_Lon = (int)regKey.GetValue("Lon");
					v_HomeLat = v_Lat = (int)regKey.GetValue("Lat");
				}
			}
			catch(Exception e)
            {
				
            }
		}
		
		private static LocationST instance;
		private DateTime dts = new DateTime(1,3,28,2,0,0);
		private DateTime dte = new DateTime(1,10,30,2,0,0);
		private double d, v_xs, v_ys, v_zs, v_slon, v_sRA, v_sDecl, v_SIDTIME;
		private RegistryKey regKey;
	}
}
