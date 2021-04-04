using System;

namespace NthDimension.Math.Astronomy
{
	/// <summary>
	/// 
	/// </summary>
	public class PertMoon : PerturbationsObject
	{
		public PertMoon(){}

		public override double PertInLon()
		{
			double lon1 =  -1.274 * System.Math.Sin((pert.Mm-2*pert.D)*PI/180);
			double lon2 =   0.658 * System.Math.Sin((2*pert.D)*PI/180);
			double lon3 =  -0.186 * System.Math.Sin((pert.Msun)*PI/180);
			double lon4 =  -0.059 * System.Math.Sin((2*pert.Mm-2*pert.D)*PI/180);
			double lon5 =  -0.057 * System.Math.Sin((pert.Mm-2*pert.D+pert.Msun)*PI/180);
			double lon6 =   0.053 * System.Math.Sin((pert.Mm+2*pert.D)*PI/180);
			double lon7 =   0.046 * System.Math.Sin((2*pert.D-pert.Msun)*PI/180);
			double lon8 =   0.041 * System.Math.Sin((pert.Mm-pert.Msun)*PI/180);
			double lon9 =  -0.035 * System.Math.Sin((pert.D)*PI/180);
			double lon10 = -0.031 * System.Math.Sin((pert.Mm+pert.Msun)*PI/180);
			double lon11 = -0.015 * System.Math.Sin((2*pert.F-2*pert.D)*PI/180);
			double lon12 = +0.011 * System.Math.Sin((pert.Mm-4*pert.D)*PI/180);
			double lon = lon1 + lon2 + lon3 + lon4 + lon5 + lon6 + 
				         lon7 + lon8 + lon9 + lon10 + lon11 + lon12;
			return lon;
		}

		public override double PertInLat()
		{
			double lat1 = -0.173 * System.Math.Sin((pert.F-2*pert.D)*PI/180);
			double lat2 = -0.055 * System.Math.Sin((pert.Mm-pert.F-2*pert.D)*PI/180);
			double lat3 = -0.046 * System.Math.Sin((pert.Mm+pert.F-2*pert.D)*PI/180);
			double lat4 = 0.033 * System.Math.Sin((pert.F+2*pert.D)*PI/180);
			double lat5 = 0.017 * System.Math.Sin((2*pert.Mm+pert.F)*PI/180);
			double lat = lat1 + lat2 + lat3 + lat4 + lat5;
			return lat;
		}

		public override double PertInDist()
		{
			double d1 = -0.58 * System.Math.Cos((pert.Mm-2*pert.D)*PI/180);
			double d2 = -0.46 * System.Math.Cos((2*pert.D)*PI/180);
			double dist = d1 + d2;
			return dist;
		}

		private double PI = System.Math.PI;
	}
}
