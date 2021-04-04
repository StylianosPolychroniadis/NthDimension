using NthDimension.Math.Geography.Norad;
using System;

namespace NthDimension.Math.Geography
{
	public class Orbit
	{
		private TimeSpan m_Period = new TimeSpan(0, 0, 0, -1);

		private double m_Inclination;

		private double m_Eccentricity;

		private double m_RAAN;

		private double m_ArgPerigee;

		private double m_BStar;

		private double m_Drag;

		private double m_TleMeanMotion;

		private double m_MeanAnomaly;

		private double m_aeAxisSemiMajorRec;

		private double m_aeAxisSemiMinorRec;

		private double m_rmMeanMotionRec;

		private double m_kmPerigeeRec;

		private double m_kmApogeeRec;

		private NoradTle Tle
		{
			get;
			set;
		}

		private NoradBase NoradModel
		{
			get;
			set;
		}

		public Calendar.JulianCalendar Epoch
		{
			get;
			set;
		}

		public DateTime EpochTime
		{
			get
			{
				return Epoch.ToTime();
			}
		}

		public double SemiMajor
		{
			get
			{
				return m_aeAxisSemiMajorRec;
			}
		}

		public double SemiMinor
		{
			get
			{
				return m_aeAxisSemiMinorRec;
			}
		}

		public double MeanMotion
		{
			get
			{
				return m_rmMeanMotionRec;
			}
		}

		public double Major
		{
			get
			{
				return 2.0 * SemiMajor;
			}
		}

		public double Minor
		{
			get
			{
				return 2.0 * SemiMinor;
			}
		}

		public double Perigee
		{
			get
			{
				return m_kmPerigeeRec;
			}
		}

		public double Apogee
		{
			get
			{
				return m_kmApogeeRec;
			}
		}

		public double Inclination
		{
			get
			{
				return m_Inclination;
			}
		}

		public double Eccentricity
		{
			get
			{
				return m_Eccentricity;
			}
		}

		public double RAAN
		{
			get
			{
				return m_RAAN;
			}
		}

		public double ArgPerigee
		{
			get
			{
				return m_ArgPerigee;
			}
		}

		public double BStar
		{
			get
			{
				return m_BStar;
			}
		}

		public double Drag
		{
			get
			{
				return m_Drag;
			}
		}

		public double MeanAnomaly
		{
			get
			{
				return m_MeanAnomaly;
			}
		}

		private double TleMeanMotion
		{
			get
			{
				return m_TleMeanMotion;
			}
		}

		public string SatNoradId
		{
			get
			{
				return Tle.NoradNumber;
			}
		}

		public string SatName
		{
			get
			{
				return Tle.Name;
			}
		}

		public string SatNameLong
		{
			get
			{
				return SatName + " #" + SatNoradId;
			}
		}

		public TimeSpan Period
		{
			get
			{
				if (m_Period.TotalSeconds < 0.0)
				{
					if (MeanMotion == 0.0)
					{
						m_Period = new TimeSpan(0, 0, 0);
					}
					else
					{
						double num = System.Math.PI * 2.0 / MeanMotion * 60.0;
						int milliseconds = (int)((num - (double)(int)num) * 1000.0);
						m_Period = new TimeSpan(0, 0, 0, (int)num, milliseconds);
					}
				}
				return m_Period;
			}
		}

		public Orbit(NoradTle tle)
		{
			Tle = tle;
			Epoch = Tle.EpochJulian;
			m_Inclination = GetRad(NoradTle.Field.Inclination);
			m_Eccentricity = Tle.GetField(NoradTle.Field.Eccentricity);
			m_RAAN = GetRad(NoradTle.Field.Raan);
			m_ArgPerigee = GetRad(NoradTle.Field.ArgPerigee);
			m_BStar = Tle.GetField(NoradTle.Field.BStarDrag);
			m_Drag = Tle.GetField(NoradTle.Field.MeanMotionDt);
			m_MeanAnomaly = GetRad(NoradTle.Field.MeanAnomaly);
			m_TleMeanMotion = Tle.GetField(NoradTle.Field.MeanMotion);
			double tleMeanMotion = TleMeanMotion;
			double num = tleMeanMotion * (System.Math.PI * 2.0) / 1440.0;
			double num2 = System.Math.Pow(Constants.Xke / num, 2.0 / 3.0);
			double eccentricity = Eccentricity;
			double inclination = Inclination;
			double num3 = 0.00081196185 * (3.0 * Constants.Sqr(System.Math.Cos(inclination)) - 1.0) / System.Math.Pow(1.0 - eccentricity * eccentricity, 1.5);
			double num4 = num3 / (num2 * num2);
			double num5 = num2 * (1.0 - num4 * (0.33333333333333331 + num4 * (1.0 + 1.654320987654321 * num4)));
			double num6 = num3 / (num5 * num5);
			m_rmMeanMotionRec = num / (1.0 + num6);
			m_aeAxisSemiMajorRec = num5 / (1.0 - num6);
			m_aeAxisSemiMinorRec = m_aeAxisSemiMajorRec * System.Math.Sqrt(1.0 - eccentricity * eccentricity);
			m_kmPerigeeRec = 6378.135 * (m_aeAxisSemiMajorRec * (1.0 - eccentricity) - 1.0);
			m_kmApogeeRec = 6378.135 * (m_aeAxisSemiMajorRec * (1.0 + eccentricity) - 1.0);
			if (Period.TotalMinutes >= 225.0)
			{
				NoradModel = new NoradSDP4(this);
			}
			else
			{
				NoradModel = new NoradSGP4(this);
			}
		}

		public TimeSpan TPlusEpoch(DateTime gmt)
		{
			return gmt - EpochTime;
		}

		public TimeSpan TPlusEpoch()
		{
			return TPlusEpoch(DateTime.UtcNow);
		}

		public GeoVector GetPosition(double minutesPastEpoch)
		{
			GeoVector position = NoradModel.GetPosition(minutesPastEpoch);
			position.Ae2Km();
			return position;
		}

		public GeoVector GetPosition(DateTime gmt)
		{
			return GetPosition(TPlusEpoch(gmt).TotalMinutes);
		}

		protected double GetRad(NoradTle.Field fld)
		{
			return Tle.GetField(fld, NoradTle.Unit.Radians);
		}

		protected double GetDeg(NoradTle.Field fld)
		{
			return Tle.GetField(fld, NoradTle.Unit.Degrees);
		}
	}
}
