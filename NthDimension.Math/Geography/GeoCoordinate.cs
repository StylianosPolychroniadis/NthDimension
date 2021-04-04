using System;
using System.Text.RegularExpressions;

namespace NthDimension.Math.Geography
{
	public class GeoCoordinate
	{
		public struct LatLong : IComparable
		{
			public double Lat;

			public double Long;

			public override string ToString()
			{
				return Long.ToString("#.###") + ((Long >= 0.0) ? "N" : "S") + " " + Lat.ToString("#.###") + ((Lat >= 0.0) ? "E" : "W");
			}

			public int CompareTo(object to)
			{
				if (to is LatLong)
				{
					if (Lat == ((LatLong)to).Lat && Long == ((LatLong)to).Long)
					{
						return 0;
					}
					return -1;
				}
				return -1;
			}
		}

		public static LatLong LocatorToLatLong(string locator)
		{
			locator = locator.Trim().ToUpper();
			if (Regex.IsMatch(locator, "^[A-R]{2}[0-9]{2}$"))
			{
				LatLong result = default(LatLong);
				result.Long = (double)((locator[0] - 65) * 20) + ((double)(locator[2] - 48) + 0.5) * 2.0 - 180.0;
				result.Lat = (double)((locator[1] - 65) * 10) + ((double)(locator[3] - 48) + 0.5) - 90.0;
				return result;
			}
			if (Regex.IsMatch(locator, "^[A-R]{2}[0-9]{2}[A-X]{2}$"))
			{
				LatLong result2 = default(LatLong);
				result2.Long = (double)((locator[0] - 65) * 20 + (locator[2] - 48) * 2) + ((double)(locator[4] - 65) + 0.5) / 12.0 - 180.0;
				result2.Lat = (double)((locator[1] - 65) * 10 + (locator[3] - 48)) + ((double)(locator[5] - 65) + 0.5) / 24.0 - 90.0;
				return result2;
			}
			if (Regex.IsMatch(locator, "^[A-R]{2}[0-9]{2}[A-X]{2}[0-9]{2}$"))
			{
				LatLong result3 = default(LatLong);
				result3.Long = (double)((locator[0] - 65) * 20 + (locator[2] - 48) * 2) + ((double)(locator[4] - 65) + 0.0) / 12.0 + ((double)(locator[6] - 48) + 0.5) / 120.0 - 180.0;
				result3.Lat = (double)((locator[1] - 65) * 10 + (locator[3] - 48)) + ((double)(locator[5] - 65) + 0.0) / 24.0 + ((double)(locator[7] - 48) + 0.5) / 240.0 - 90.0;
				return result3;
			}
			if (Regex.IsMatch(locator, "^[A-R]{2}[0-9]{2}[A-X]{2}[0-9]{2}[A-X]{2}$"))
			{
				LatLong result4 = default(LatLong);
				result4.Long = (double)((locator[0] - 65) * 20 + (locator[2] - 48) * 2) + ((double)(locator[4] - 65) + 0.0) / 12.0 + ((double)(locator[6] - 48) + 0.0) / 120.0 + ((double)(locator[8] - 65) + 0.5) / 120.0 / 24.0 - 180.0;
				result4.Lat = (double)((locator[1] - 65) * 10 + (locator[3] - 48)) + ((double)(locator[5] - 65) + 0.0) / 24.0 + ((double)(locator[7] - 48) + 0.0) / 240.0 + ((double)(locator[9] - 65) + 0.5) / 240.0 / 24.0 - 90.0;
				return result4;
			}
			LatLong result5 = default(LatLong);
			result5.Lat = 1000.0;
			result5.Long = 1000.0;
			return result5;
		}

		public static string LatLongToLocator(LatLong ll)
		{
			return LatLongToLocator(ll.Lat, ll.Long, 0);
		}

		public static string LatLongToLocator(LatLong ll, int Ext)
		{
			return LatLongToLocator(ll.Lat, ll.Long, Ext);
		}

		public static string LatLongToLocator(double Lat, double Long)
		{
			return LatLongToLocator(Lat, Long, 0);
		}

		public static string LatLongToLocator(double Lat, double Long, int Ext)
		{
			string str = "";
			Lat += 90.0;
			Long += 180.0;
			str += (char)(65.0 + System.Math.Floor(Long / 20.0));
			str += (char)(65.0 + System.Math.Floor(Lat / 10.0));
			Long = System.Math.IEEERemainder(Long, 20.0);
			if (Long < 0.0)
			{
				Long += 20.0;
			}
			Lat = System.Math.IEEERemainder(Lat, 10.0);
			if (Lat < 0.0)
			{
				Lat += 10.0;
			}
			str += (char)(48.0 + System.Math.Floor(Long / 2.0));
			str += (char)(48.0 + System.Math.Floor(Lat / 1.0));
			Long = System.Math.IEEERemainder(Long, 2.0);
			if (Long < 0.0)
			{
				Long += 2.0;
			}
			Lat = System.Math.IEEERemainder(Lat, 1.0);
			if (Lat < 0.0)
			{
				Lat += 1.0;
			}
			str += (char)(65.0 + System.Math.Floor(Long * 12.0));
			str += (char)(65.0 + System.Math.Floor(Lat * 24.0));
			Long = System.Math.IEEERemainder(Long, 0.083333333333333329);
			if (Long < 0.0)
			{
				Long += 0.083333333333333329;
			}
			Lat = System.Math.IEEERemainder(Lat, 0.041666666666666664);
			if (Lat < 0.0)
			{
				Lat += 0.041666666666666664;
			}
			if (Ext >= 1)
			{
				str += (char)(48.0 + System.Math.Floor(Long * 120.0));
				str += (char)(48.0 + System.Math.Floor(Lat * 240.0));
				Long = System.Math.IEEERemainder(Long, 0.0083333333333333332);
				if (Long < 0.0)
				{
					Long += 0.0083333333333333332;
				}
				Lat = System.Math.IEEERemainder(Lat, 0.0041666666666666666);
				if (Lat < 0.0)
				{
					Lat += 0.0041666666666666666;
				}
			}
			if (Ext >= 2)
			{
				str += (char)(65.0 + System.Math.Floor(Long * 120.0 * 24.0));
				str += (char)(65.0 + System.Math.Floor(Lat * 240.0 * 24.0));
				Long = System.Math.IEEERemainder(Long, 0.00034722222222222224);
				if (Long < 0.0)
				{
					Long += 0.00034722222222222224;
				}
				Lat = System.Math.IEEERemainder(Lat, 0.00017361111111111112);
				if (Lat < 0.0)
				{
					Lat += 0.00017361111111111112;
				}
			}
			return str;
		}

		public static double RadToDeg(double rad)
		{
			return rad / System.Math.PI * 180.0;
		}

		public static double DegToRad(double deg)
		{
			return deg / 180.0 * System.Math.PI;
		}

		public static double Distance(string A, string B)
		{
			return Distance(LocatorToLatLong(A), LocatorToLatLong(B));
		}

		public static double Distance(LatLong A, LatLong B)
		{
			if (A.CompareTo(B) == 0)
			{
				return 0.0;
			}
			double num = DegToRad(A.Lat);
			double num2 = DegToRad(A.Long);
			double num3 = DegToRad(B.Lat);
			double num4 = DegToRad(B.Long);
			double num5 = System.Math.Cos(num2 - num4) * System.Math.Cos(num) * System.Math.Cos(num3) + System.Math.Sin(num) * System.Math.Sin(num3);
			double num6 = System.Math.Atan(System.Math.Abs(System.Math.Sqrt(1.0 - num5 * num5) / num5));
			if (num5 < 0.0)
			{
				num6 = System.Math.PI - num6;
			}
			return 6367.0 * num6;
		}

		public static double Azimuth(string A, string B)
		{
			return Azimuth(LocatorToLatLong(A), LocatorToLatLong(B));
		}

		public static double Azimuth(LatLong A, LatLong B)
		{
			if (A.CompareTo(B) == 0)
			{
				return 0.0;
			}
			double num = DegToRad(A.Lat);
			double num2 = DegToRad(A.Long);
			double num3 = DegToRad(B.Lat);
			double num4 = DegToRad(B.Long);
			double num5 = System.Math.Cos(num2 - num4) * System.Math.Cos(num) * System.Math.Cos(num3) + System.Math.Sin(num) * System.Math.Sin(num3);
			double num6 = System.Math.Atan(System.Math.Abs(System.Math.Sqrt(1.0 - num5 * num5) / num5));
			if (num5 < 0.0)
			{
				num6 = System.Math.PI - num6;
			}
			double num7 = System.Math.Sin(num4 - num2) * System.Math.Cos(num3) * System.Math.Cos(num);
			num5 = System.Math.Sin(num3) - System.Math.Sin(num) * System.Math.Cos(num6);
			double num8 = System.Math.Atan(System.Math.Abs(num7 / num5));
			if (num5 < 0.0)
			{
				num8 = System.Math.PI - num8;
			}
			if (num7 < 0.0)
			{
				num8 = 0.0 - num8;
			}
			if (num8 < 0.0)
			{
				num8 += System.Math.PI * 2.0;
			}
			return RadToDeg(num8);
		}
	}
}
