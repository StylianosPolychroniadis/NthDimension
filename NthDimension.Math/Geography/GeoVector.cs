using NthDimension.Algebra;
using System;

namespace NthDimension.Math.Geography
{
	public class GeoVector
	{
		public class CoordGeo
		{
			private double m_Latitude;

			private double m_Longitude;

			private double m_Altitude;

			public double Latitude
			{
				get
				{
					return m_Latitude;
				}
				set
				{
					m_Latitude = value;
				}
			}

			public double Longitude
			{
				get
				{
					return m_Longitude;
				}
				set
				{
					m_Longitude = value;
				}
			}

			public double Altitude
			{
				get
				{
					return m_Altitude;
				}
				set
				{
					m_Altitude = value;
				}
			}

			public CoordGeo()
			{
			}

			public CoordGeo(double lat, double lon, double alt)
			{
				m_Latitude = lat;
				m_Longitude = lon;
				m_Altitude = alt;
			}
		}

		public class CoordTopo
		{
			private double m_Azimuth;

			private double m_Elevation;

			private double m_Range;

			private double m_RangeRate;

			public double Azimuth
			{
				get
				{
					return m_Azimuth;
				}
				set
				{
					m_Azimuth = value;
				}
			}

			public double Elevation
			{
				get
				{
					return m_Elevation;
				}
				set
				{
					m_Elevation = value;
				}
			}

			public double Range
			{
				get
				{
					return m_Range;
				}
				set
				{
					m_Range = value;
				}
			}

			public double RangeRate
			{
				get
				{
					return m_RangeRate;
				}
				set
				{
					m_RangeRate = value;
				}
			}

			public CoordTopo()
			{
			}

			public CoordTopo(double az, double el, double rng, double rate)
			{
				m_Azimuth = az;
				m_Elevation = el;
				m_Range = rng;
				m_RangeRate = rate;
			}
		}

		protected enum VectorUnits
		{
			None,
			Ae,
			Km
		}

		private Vector4d m_Position;

		private Vector4d m_Velocity;

		private Calendar.JulianCalendar m_Date;

		private VectorUnits m_VectorUnits;

		public Vector4d Position
		{
			get
			{
				return m_Position;
			}
		}

		public Vector4d Velocity
		{
			get
			{
				return m_Velocity;
			}
		}

		public Calendar.JulianCalendar Date
		{
			get
			{
				return m_Date;
			}
		}

		protected VectorUnits Units
		{
			get
			{
				return m_VectorUnits;
			}
			set
			{
				m_VectorUnits = value;
			}
		}

		public GeoVector()
		{
			m_VectorUnits = VectorUnits.None;
		}

		public GeoVector(Vector4d pos, Vector4d vel, Calendar.JulianCalendar date, bool IsAeUnits)
		{
			m_Position = pos;
			m_Velocity = vel;
			m_Date = date;
			m_VectorUnits = (IsAeUnits ? VectorUnits.Ae : VectorUnits.None);
		}

		public GeoVector(CoordGeo geo, Calendar.JulianCalendar date)
		{
			m_VectorUnits = VectorUnits.Km;
			double num = 7.2921158552280831E-05;
			double latitude = geo.Latitude;
			double longitude = geo.Longitude;
			double altitude = geo.Altitude;
			double num2 = date.ToLmst(longitude);
			double num3 = 1.0 / System.Math.Sqrt(1.0 + -0.0066943177782667227 * Constants.Sqr(System.Math.Sin(latitude)));
			double num4 = Constants.Sqr(0.99664722054583255) * num3;
			double num5 = (6378.135 * num3 + altitude) * System.Math.Cos(latitude);
			m_Date = date;
			m_Position = new Vector4d();
			m_Position.X = num5 * System.Math.Cos(num2);
			m_Position.Y = num5 * System.Math.Sin(num2);
			m_Position.Z = (6378.135 * num4 + altitude) * System.Math.Sin(latitude);
			m_Position.W = System.Math.Sqrt(Constants.Sqr(m_Position.X) + Constants.Sqr(m_Position.Y) + Constants.Sqr(m_Position.Z));
			m_Velocity = new Vector4d();
			m_Velocity.X = (0.0 - num) * m_Position.Y;
			m_Velocity.Y = num * m_Position.X;
			m_Velocity.Z = 0.0;
			m_Velocity.W = System.Math.Sqrt(Constants.Sqr(m_Velocity.X) + Constants.Sqr(m_Velocity.Y));
		}

		public void SetUnitsAe()
		{
			Units = VectorUnits.Ae;
		}

		public void SetUnitsKm()
		{
			Units = VectorUnits.Km;
		}

		public bool UnitsAreAe()
		{
			return Units == VectorUnits.Ae;
		}

		public bool UnitsAreKm()
		{
			return Units == VectorUnits.Km;
		}

		public CoordGeo ToGeo()
		{
			Ae2Km();
			double num = Constants.AcTan(m_Position.Y, m_Position.X);
			double num2 = (num - m_Date.ToGmst()) % (System.Math.PI * 2.0);
			if (num2 < 0.0)
			{
				num2 += System.Math.PI * 2.0;
			}
			double num3 = System.Math.Sqrt(Constants.Sqr(m_Position.X) + Constants.Sqr(m_Position.Y));
			double num4 = 0.0066943177782667227;
			double num5 = Constants.AcTan(m_Position.Z, num3);
			double num6;
			double num7;
			do
			{
				num6 = num5;
				num7 = 1.0 / System.Math.Sqrt(1.0 - num4 * Constants.Sqr(System.Math.Sin(num6)));
				num5 = Constants.AcTan(m_Position.Z + 6378.135 * num7 * num4 * System.Math.Sin(num6), num3);
			}
			while (System.Math.Abs(num5 - num6) > 1E-07);
			double alt = num3 / System.Math.Cos(num5) - 6378.135 * num7;
			return new CoordGeo(num5, num2, alt);
		}

		public void Ae2Km()
		{
			if (UnitsAreAe())
			{
				MulPos(6378.135);
				MulVel(106.30225);
				m_VectorUnits = VectorUnits.Km;
			}
		}

		protected void MulPos(double factor)
		{
			m_Position = Vector4d.Multiply(m_Position, factor);
		}

		protected void MulVel(double factor)
		{
			m_Velocity = Vector4d.Multiply(m_Velocity, factor);
		}
	}
}
