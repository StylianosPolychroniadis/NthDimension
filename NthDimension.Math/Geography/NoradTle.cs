using System;
using System.Collections.Generic;
using System.Globalization;

namespace NthDimension.Math.Geography
{
	public class NoradTle
	{
		// TODO:: CulturalInfo incomplete

		public enum Line
		{
			Zero,
			One,
			Two
		}

		public enum Field
		{
			NoradNum,
			IntlDesc,
			SetNumber,
			EpochYear,
			EpochDay,
			OrbitAtEpoch,
			Inclination,
			Raan,
			Eccentricity,
			ArgPerigee,
			MeanAnomaly,
			MeanMotion,
			MeanMotionDt,
			MeanMotionDt2,
			BStarDrag
		}

		public enum Unit
		{
			Radians,
			Degrees,
			Native
		}

		private const int TLE_LEN_LINE_DATA = 69;

		private const int TLE_LEN_LINE_NAME = 24;

		private const int TLE1_COL_SATNUM = 2;

		private const int TLE1_LEN_SATNUM = 5;

		private const int TLE1_COL_INTLDESC_A = 9;

		private const int TLE1_LEN_INTLDESC_A = 2;

		private const int TLE1_COL_INTLDESC_B = 11;

		private const int TLE1_LEN_INTLDESC_B = 3;

		private const int TLE1_COL_INTLDESC_C = 14;

		private const int TLE1_LEN_INTLDESC_C = 3;

		private const int TLE1_COL_EPOCH_A = 18;

		private const int TLE1_LEN_EPOCH_A = 2;

		private const int TLE1_COL_EPOCH_B = 20;

		private const int TLE1_LEN_EPOCH_B = 12;

		private const int TLE1_COL_MEANMOTIONDT = 33;

		private const int TLE1_LEN_MEANMOTIONDT = 10;

		private const int TLE1_COL_MEANMOTIONDT2 = 44;

		private const int TLE1_LEN_MEANMOTIONDT2 = 8;

		private const int TLE1_COL_BSTAR = 53;

		private const int TLE1_LEN_BSTAR = 8;

		private const int TLE1_COL_EPHEMTYPE = 62;

		private const int TLE1_LEN_EPHEMTYPE = 1;

		private const int TLE1_COL_ELNUM = 64;

		private const int TLE1_LEN_ELNUM = 4;

		private const int TLE2_COL_SATNUM = 2;

		private const int TLE2_LEN_SATNUM = 5;

		private const int TLE2_COL_INCLINATION = 8;

		private const int TLE2_LEN_INCLINATION = 8;

		private const int TLE2_COL_RAASCENDNODE = 17;

		private const int TLE2_LEN_RAASCENDNODE = 8;

		private const int TLE2_COL_ECCENTRICITY = 26;

		private const int TLE2_LEN_ECCENTRICITY = 7;

		private const int TLE2_COL_ARGPERIGEE = 34;

		private const int TLE2_LEN_ARGPERIGEE = 8;

		private const int TLE2_COL_MEANANOMALY = 43;

		private const int TLE2_LEN_MEANANOMALY = 8;

		private const int TLE2_COL_MEANMOTION = 52;

		private const int TLE2_LEN_MEANMOTION = 11;

		private const int TLE2_COL_REVATEPOCH = 63;

		private const int TLE2_LEN_REVATEPOCH = 5;

		private string m_Line0;

		private string m_Line1;

		private string m_Line2;

		private Dictionary<Field, string> m_Field;

		private Dictionary<int, double> m_Cache;

		public string Name
		{
			get
			{
				return m_Line0;
			}
		}

		public string Line1
		{
			get
			{
				return m_Line1;
			}
		}

		public string Line2
		{
			get
			{
				return m_Line2;
			}
		}

		public string NoradNumber
		{
			get
			{
				return GetField(Field.NoradNum, false);
			}
		}

		public string Eccentricity
		{
			get
			{
				return GetField(Field.Eccentricity, false);
			}
		}

		public string Inclination
		{
			get
			{
				return GetField(Field.Inclination, true);
			}
		}

		public string Epoch
		{
			get
			{
				return GetField(Field.EpochYear).ToString(CultureInfo.InvariantCulture) + GetField(Field.EpochDay).ToString(CultureInfo.InvariantCulture);
			}
		}

		public string IntlDescription
		{
			get
			{
				return GetField(Field.IntlDesc, false);
			}
		}

		public string SetNumber
		{
			get
			{
				return GetField(Field.SetNumber, false);
			}
		}

		public string OrbitAtEpoch
		{
			get
			{
				return GetField(Field.OrbitAtEpoch, false);
			}
		}

		public string RAAscendingNode
		{
			get
			{
				return GetField(Field.Raan, true);
			}
		}

		public string ArgPerigee
		{
			get
			{
				return GetField(Field.ArgPerigee, true);
			}
		}

		public string MeanAnomaly
		{
			get
			{
				return GetField(Field.MeanAnomaly, true);
			}
		}

		public string MeanMotion
		{
			get
			{
				return GetField(Field.MeanMotion, true);
			}
		}

		public string MeanMotionDt
		{
			get
			{
				return GetField(Field.MeanMotionDt, false);
			}
		}

		public string MeanMotionDt2
		{
			get
			{
				return GetField(Field.MeanMotionDt2, false);
			}
		}

		public string BStarDrag
		{
			get
			{
				return GetField(Field.BStarDrag, false);
			}
		}

		public Calendar.JulianCalendar EpochJulian
		{
			get
			{
				int num = (int)GetField(Field.EpochYear);
				double field = GetField(Field.EpochDay);
				num = ((num >= 57) ? (num + 1900) : (num + 2000));
				return new Calendar.JulianCalendar(num, field);
			}
		}

		private static int Key(Unit u, Field f)
		{
			return (int)((int)u * 100 + f);
		}

		public NoradTle(string strName, string strLine1, string strLine2)
		{
			m_Line0 = strName;
			m_Line1 = strLine1;
			m_Line2 = strLine2;
			Initialize();
		}

		public NoradTle(NoradTle tle)
			: this(tle.Name, tle.Line1, tle.Line2)
		{
		}

		private void Initialize()
		{
			m_Field = new Dictionary<Field, string>();
			m_Cache = new Dictionary<int, double>();
			m_Field[Field.NoradNum] = m_Line1.Substring(2, 5);
			m_Field[Field.IntlDesc] = m_Line1.Substring(9, 8);
			m_Field[Field.EpochYear] = m_Line1.Substring(18, 2);
			m_Field[Field.EpochDay] = m_Line1.Substring(20, 12);
			if (m_Line1[33] == '-')
			{
				m_Field[Field.MeanMotionDt] = "-0";
			}
			else
			{
				m_Field[Field.MeanMotionDt] = "0";
			}
			m_Field[Field.MeanMotionDt] += m_Line1.Substring(34, 10);
			m_Field[Field.MeanMotionDt2] = ExpToDecimal(m_Line1.Substring(44, 8));
			m_Field[Field.BStarDrag] = ExpToDecimal(m_Line1.Substring(53, 8));
			m_Field[Field.SetNumber] = m_Line1.Substring(64, 4).TrimStart();
			m_Field[Field.Inclination] = m_Line2.Substring(8, 8).TrimStart();
			m_Field[Field.Raan] = m_Line2.Substring(17, 8).TrimStart();
			m_Field[Field.Eccentricity] = "0." + m_Line2.Substring(26, 7);
			m_Field[Field.ArgPerigee] = m_Line2.Substring(34, 8).TrimStart();
			m_Field[Field.MeanAnomaly] = m_Line2.Substring(43, 8).TrimStart();
			m_Field[Field.MeanMotion] = m_Line2.Substring(52, 11).TrimStart();
			m_Field[Field.OrbitAtEpoch] = m_Line2.Substring(63, 5).TrimStart();
		}

		public double GetField(Field fld)
		{
			return GetField(fld, Unit.Native);
		}

		public double GetField(Field fld, Unit units)
		{
			int key = Key(units, fld);
			if (m_Cache.ContainsKey(key))
			{
				return m_Cache[key];
			}
			double valNative = double.Parse(m_Field[fld].ToString(), CultureInfo.InvariantCulture);
			double num = ConvertUnits(valNative, fld, units);
			m_Cache[key] = num;
			return num;
		}

		public string GetField(Field fld, bool AppendUnits)
		{
			string text = m_Field[fld].ToString();
			if (AppendUnits)
			{
				text += GetUnits(fld);
			}
			return text;
		}

		protected static double ConvertUnits(double valNative, Field fld, Unit units)
		{
			if ((fld == Field.Inclination || fld == Field.Raan || fld == Field.ArgPerigee || fld == Field.MeanAnomaly) && units == Unit.Radians)
			{
				return valNative * (System.Math.PI / 180.0);
			}
			return valNative;
		}

		protected static string GetUnits(Field fld)
		{
			switch (fld)
			{
			case Field.Inclination:
			case Field.Raan:
			case Field.ArgPerigee:
			case Field.MeanAnomaly:
				return " degrees";
			case Field.MeanMotion:
				return " revs / day";
			default:
				return string.Empty;
			}
		}

		protected static string ExpToDecimal(string str)
		{
			string text = str.Substring(0, 1);
			string text2 = str.Substring(1, 5);
			string text3 = str.Substring(6, 2).TrimStart();
			double num = double.Parse(text + "0." + text2 + "e" + text3, CultureInfo.InvariantCulture);
			int num2 = text2.Length + System.Math.Abs(int.Parse(text3, CultureInfo.InvariantCulture));
			return num.ToString("F" + num2, CultureInfo.InvariantCulture);
		}

		public static bool IsValidLine(string str, Line line)
		{
			str = str.Trim();
			int length = str.Length;
			if (line == Line.Zero)
			{
				return str.Length <= 24;
			}
			if (length == 69 && str[0] - 48 == (int)line)
			{
				return str[1] == ' ';
			}
			return false;
		}

		private static int CheckSum(string str)
		{
			int num = str.Length - 1;
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				char c = str[i];
				if (char.IsDigit(c))
				{
					num2 += c - 48;
				}
				else if (c == '-')
				{
					num2++;
				}
			}
			return num2 % 10;
		}
	}
}
