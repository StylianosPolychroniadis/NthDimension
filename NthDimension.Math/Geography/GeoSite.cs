using NthDimension.Algebra;
using System;
using System.Globalization;

namespace NthDimension.Math.Geography
{
	public sealed class GeoSite
	{
		public double Latitude
		{
			get
			{
				return Geo.Latitude;
			}
		}

		public double Longitude
		{
			get
			{
				return Geo.Longitude;
			}
		}

		public double Altitude
		{
			get
			{
				return Geo.Altitude;
			}
		}

		private GeoVector.CoordGeo Geo
		{
			get;
			set;
		}

		public GeoSite(double degLat, double degLon, double kmAlt)
		{
			Geo = new GeoVector.CoordGeo(Constants.Deg2Rad(degLat), Constants.Deg2Rad(degLon), kmAlt);
		}

		public GeoVector GetPosition(Calendar.JulianCalendar date)
		{
			return new GeoVector(Geo, date);
		}

		public GeoVector.CoordTopo GetLookAngle(GeoVector eci)
		{
			if (!eci.UnitsAreKm())
			{
				throw new ArgumentException("ECI units must be kilometer-based", "eci");
			}
			Calendar.JulianCalendar date = eci.Date;
			GeoVector geoVector = new GeoVector(Geo, date);
			Vector4d vector = new Vector4d(eci.Velocity.X - geoVector.Velocity.X, eci.Velocity.Y - geoVector.Velocity.Y, eci.Velocity.Z - geoVector.Velocity.Z, 0f);
			double x = eci.Position.X - geoVector.Position.X;
			double num = eci.Position.Y - geoVector.Position.Y;
			double num2 = eci.Position.Z - geoVector.Position.Z;
			double w = System.Math.Sqrt(Constants.Sqr(x) + Constants.Sqr(num) + Constants.Sqr(num2));
			Vector4d vector2 = new Vector4d(x, num, num2, w);
			double num3 = date.ToLmst(Longitude);
			double num4 = System.Math.Sin(Latitude);
			double num5 = System.Math.Cos(Latitude);
			double num6 = System.Math.Sin(num3);
			double num7 = System.Math.Cos(num3);
			double num8 = num4 * num7 * vector2.X + num4 * num6 * vector2.Y - num5 * vector2.Z;
			double num9 = (0.0 - num6) * vector2.X + num7 * vector2.Y;
			double num10 = num5 * num7 * vector2.X + num5 * num6 * vector2.Y + num4 * vector2.Z;
			double num11 = System.Math.Atan((0.0 - num9) / num8);
			if (num8 > 0.0)
			{
				num11 += System.Math.PI;
			}
			if (num11 < 0.0)
			{
				num11 += System.Math.PI * 2.0;
			}
			double el = System.Math.Asin(num10 / vector2.W);
			double rate = (vector2.X * vector.X + vector2.Y * vector.Y + vector2.Z * vector.Z) / vector2.W;
			return new GeoVector.CoordTopo(num11, el, vector2.W, rate);
		}

		public override string ToString()
		{
			bool flag = Geo.Latitude >= 0.0;
			bool flag2 = Geo.Longitude >= 0.0;
			string str = string.Format(CultureInfo.CurrentCulture, "{0:F3}{1} ", new object[2]
			{
				System.Math.Abs(Constants.Rad2Deg(Geo.Latitude)),
				flag ? 'N' : 'S'
			});
			str += string.Format(CultureInfo.CurrentCulture, "{0:F3}{1} ", new object[2]
			{
				System.Math.Abs(Constants.Rad2Deg(Geo.Longitude)),
				flag2 ? 'E' : 'W'
			});
			return str + string.Format(CultureInfo.CurrentCulture, "{0:F1}m", new object[1]
			{
				Geo.Altitude * 1000.0
			});
		}
	}
}
