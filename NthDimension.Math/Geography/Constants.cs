using NthDimension.Algebra;
using System;

namespace NthDimension.Math.Geography
{
	// TODO	:: Post Integration from Syscon Namespace:
	//		:: Eliminate System.Math.PI and replace by NthDimension.Algebra.MathHelper.Pi
	public static class Constants
	{
		public const double Pi = System.Math.PI;

		public const double TwoPi = System.Math.PI * 2.0;

		public const double RadsPerDegree = System.Math.PI / 180.0;

		public const double DegreesPerRad = 180.0 / System.Math.PI;

		public const double Gm = 398601.2;

		public const double GeoSyncAlt = 42241.892;

		public const double EarthDiam = 12800.0;

		public const double DaySidereal = 86164.09;

		public const double DaySolar = 86400.0;

		public const double Ae = 1.0;

		public const double Au = 149597870.0;

		public const double Sr = 696000.0;

		public const double Xkmper = 6378.135;

		public const double F = 0.003352779454167505;

		public const double Ge = 398600.8;

		public const double J2 = 0.0010826158;

		public const double J3 = -2.53881E-06;

		public const double J4 = -1.65597E-06;

		public const double Ck2 = 0.0005413079;

		public const double Ck4 = 6.2098874999999992E-07;

		public const double Xj3 = -2.53881E-06;

		public const double Qo = 1.0188142772142641;

		public const double S = 1.0122292801892716;

		public const double MinPerDay = 1440.0;

		public const double SecPerDay = 86400.0;

		public const double OmegaE = 1.00273790934;

		public static double Xke;

		public static double Qoms2t;

		static Constants()
		{
			Xke = 0.0;
			Qoms2t = 0.0;
			Xke =  System.Math.Sqrt(0.0055304382151584478);
			Qoms2t = System.Math.Pow(0.0065849970249924894, 4.0);
		}

		public static double Sqr(double x)
		{
			return x * x;
		}

		public static double Fmod2p(double arg)
		{
			double num = arg % (System.Math.PI * 2.0);
			if (num < 0.0)
			{
				num += System.Math.PI * 2.0;
			}
			return num;
		}

		public static double AcTan(double sinx, double cosx)
		{
			if (cosx == 0.0)
			{
				if (sinx > 0.0)
				{
					return System.Math.PI / 2.0;
				}
				return 4.71238898038469;
			}
			if (cosx > 0.0)
			{
				return System.Math.Atan(sinx / cosx);
			}
			return System.Math.PI + System.Math.Atan(sinx / cosx);
		}

		public static double Rad2Deg(double r)
		{
			return r * (180.0 / System.Math.PI);
		}

		public static double Deg2Rad(double d)
		{
			return d * (System.Math.PI / 180.0);
		}
	}
}
