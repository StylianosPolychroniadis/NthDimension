using System;

namespace NthDimension.Math.Geography.Calendar
{
	public class JulianCalendar
	{
		private const double EPOCH_JAN1_00H_1900 = 2415020.5;

		private const double EPOCH_JAN1_12H_1900 = 2415021.0;

		private const double EPOCH_JAN0_12H_1900 = 2415020.0;

		private const double EPOCH_JAN1_12H_2000 = 2451545.0;

		private double m_Date;

		private int m_Year;

		private double m_Day;

		public double Date
		{
			get
			{
				return m_Date;
			}
		}

		public JulianCalendar(DateTime dt)
		{
			double day = (double)dt.DayOfYear + ((double)dt.Hour + ((double)dt.Minute + ((double)dt.Second + (double)dt.Millisecond / 1000.0) / 60.0) / 60.0) / 24.0;
			Initialize(dt.Year, day);
		}

		public JulianCalendar(int year, double day)
		{
			Initialize(year, day);
		}

		public double FromJan1_00h_1900()
		{
			return m_Date - 2415020.5;
		}

		public double FromJan1_12h_1900()
		{
			return m_Date - 2415021.0;
		}

		public double FromJan0_12h_1900()
		{
			return m_Date - 2415020.0;
		}

		public double FromJan1_12h_2000()
		{
			return m_Date - 2451545.0;
		}

		public TimeSpan Diff(JulianCalendar date)
		{
			return new TimeSpan((long)((m_Date - date.m_Date) * 864000000000.0));
		}

		protected void Initialize(int year, double day)
		{
			if (year < 1900 || year > 2100)
			{
				throw new ArgumentOutOfRangeException("year");
			}
			if (day < 1.0 || day >= 367.0)
			{
				throw new ArgumentOutOfRangeException("day");
			}
			m_Year = year;
			m_Day = day;
			year--;
			int num = year / 100;
			int num2 = 2 - num + num / 4;
			double num3 = (double)((int)(365.25 * (double)year) + 428) + 1720994.5 + (double)num2;
			m_Date = num3 + day;
		}

		public double ToGmst()
		{
			double num = (m_Date + 0.5) % 1.0;
			double num2 = (FromJan1_12h_2000() - num) / 36525.0;
			double num3 = 24110.54841 + num2 * (8640184.812866 + num2 * (0.093104 - num2 * 6.2E-06));
			num3 = (num3 + 86636.555366976 * num) % 86400.0;
			if (num3 < 0.0)
			{
				num3 += 86400.0;
			}
			return System.Math.PI * 2.0 * (num3 / 86400.0);
		}

		public double ToLmst(double lon)
		{
			return (ToGmst() + lon) % (System.Math.PI * 2.0);
		}

		public DateTime ToTime()
		{
			return new DateTime(m_Year, 1, 1).AddDays(m_Day - 1.0);
		}
	}
}
