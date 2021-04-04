using NthDimension.Algebra;
using NthDimension.Math.Geography.Exceptions;
using System;

namespace NthDimension.Math.Geography.Norad
{
	internal abstract class NoradBase
	{
		protected double m_satInc;

		protected double m_satEcc;

		protected double m_cosio;

		protected double m_theta2;

		protected double m_x3thm1;

		protected double m_eosq;

		protected double m_betao2;

		protected double m_betao;

		protected double m_aodp;

		protected double m_xnodp;

		protected double m_s4;

		protected double m_qoms24;

		protected double m_perigee;

		protected double m_tsi;

		protected double m_eta;

		protected double m_etasq;

		protected double m_eeta;

		protected double m_coef;

		protected double m_coef1;

		protected double m_c1;

		protected double m_c3;

		protected double m_c4;

		protected double m_sinio;

		protected double m_x1mth2;

		protected double m_xmdot;

		protected double m_omgdot;

		protected double m_xnodot;

		protected double m_xnodcf;

		protected double m_t2cof;

		protected double m_xlcof;

		protected double m_aycof;

		protected double m_x7thm1;

		protected Orbit Orbit
		{
			get;
			private set;
		}

		public abstract GeoVector GetPosition(double tsince);

		public NoradBase(Orbit orbit)
		{
			Orbit = orbit;
			Initialize();
		}

		private void Initialize()
		{
			m_satInc = Orbit.Inclination;
			m_satEcc = Orbit.Eccentricity;
			m_cosio = System.Math.Cos(m_satInc);
			m_theta2 = m_cosio * m_cosio;
			m_x3thm1 = 3.0 * m_theta2 - 1.0;
			m_eosq = m_satEcc * m_satEcc;
			m_betao2 = 1.0 - m_eosq;
			m_betao = System.Math.Sqrt(m_betao2);
			m_aodp = Orbit.SemiMajor;
			m_xnodp = Orbit.MeanMotion;
			m_perigee = 6378.135 * (m_aodp * (1.0 - m_satEcc) - 1.0);
			m_s4 = 1.0122292801892716;
			m_qoms24 = Constants.Qoms2t;
			if (m_perigee < 156.0)
			{
				m_s4 = m_perigee - 78.0;
				if (m_perigee <= 98.0)
				{
					m_s4 = 20.0;
				}
				m_qoms24 = System.Math.Pow((120.0 - m_s4) * 1.0 / 6378.135, 4.0);
				m_s4 = m_s4 / 6378.135 + 1.0;
			}
			double num = 1.0 / (m_aodp * m_aodp * m_betao2 * m_betao2);
			m_tsi = 1.0 / (m_aodp - m_s4);
			m_eta = m_aodp * m_satEcc * m_tsi;
			m_etasq = m_eta * m_eta;
			m_eeta = m_satEcc * m_eta;
			double num2 = System.Math.Abs(1.0 - m_etasq);
			m_coef = m_qoms24 * System.Math.Pow(m_tsi, 4.0);
			m_coef1 = m_coef / System.Math.Pow(num2, 3.5);
			double num3 = m_coef1 * m_xnodp * (m_aodp * (1.0 + 1.5 * m_etasq + m_eeta * (4.0 + m_etasq)) + 0.000405980925 * m_tsi / num2 * m_x3thm1 * (8.0 + 3.0 * m_etasq * (8.0 + m_etasq)));
			m_c1 = Orbit.BStar * num3;
			m_sinio = System.Math.Sin(m_satInc);
			double num4 = 0.0046901403064688327 * System.Math.Pow(1.0, 3.0);
			m_c3 = m_coef * m_tsi * num4 * m_xnodp * 1.0 * m_sinio / m_satEcc;
			m_x1mth2 = 1.0 - m_theta2;
			m_c4 = 2.0 * m_xnodp * m_coef1 * m_aodp * m_betao2 * (m_eta * (2.0 + 0.5 * m_etasq) + m_satEcc * (0.5 + 2.0 * m_etasq) - 0.0010826158 * m_tsi / (m_aodp * num2) * (-3.0 * m_x3thm1 * (1.0 - 2.0 * m_eeta + m_etasq * (1.5 - 0.5 * m_eeta)) + 0.75 * m_x1mth2 * (2.0 * m_etasq - m_eeta * (1.0 + m_etasq)) * System.Math.Cos(2.0 * Orbit.ArgPerigee)));
			double num5 = m_theta2 * m_theta2;
			double num6 = 0.0016239237 * num * m_xnodp;
			double num7 = num6 * 0.0005413079 * num;
			double num8 = 7.7623593749999984E-07 * num * num * m_xnodp;
			m_xmdot = m_xnodp + 0.5 * num6 * m_betao * m_x3thm1 + 0.0625 * num7 * m_betao * (13.0 - 78.0 * m_theta2 + 137.0 * num5);
			double num9 = 1.0 - 5.0 * m_theta2;
			m_omgdot = -0.5 * num6 * num9 + 0.0625 * num7 * (7.0 - 114.0 * m_theta2 + 395.0 * num5) + num8 * (3.0 - 36.0 * m_theta2 + 49.0 * num5);
			double num10 = (0.0 - num6) * m_cosio;
			m_xnodot = num10 + (0.5 * num7 * (4.0 - 19.0 * m_theta2) + 2.0 * num8 * (3.0 - 7.0 * m_theta2)) * m_cosio;
			m_xnodcf = 3.5 * m_betao2 * num10 * m_c1;
			m_t2cof = 1.5 * m_c1;
			m_xlcof = 0.125 * num4 * m_sinio * (3.0 + 5.0 * m_cosio) / (1.0 + m_cosio);
			m_aycof = 0.25 * num4 * m_sinio;
			m_x7thm1 = 7.0 * m_theta2 - 1.0;
		}

		protected GeoVector FinalPosition(double incl, double omega, double e, double a, double xl, double xnode, double xn, double tsince)
		{
			if (e * e > 1.0)
			{
				throw new PropagationException("Error in satellite data");
			}
			double num = System.Math.Sqrt(1.0 - e * e);
			double num2 = e * System.Math.Cos(omega);
			double num3 = 1.0 / (a * num * num);
			double num4 = num3 * m_xlcof * num2;
			double num5 = num3 * m_aycof;
			double num6 = xl + num4;
			double num7 = e * System.Math.Sin(omega) + num5;
			double num8 = Constants.Fmod2p(num6 - xnode);
			double num9 = num8;
			double num10 = 0.0;
			double num11 = 0.0;
			double num12 = 0.0;
			double num13 = 0.0;
			double num14 = 0.0;
			double num15 = 0.0;
			bool flag = false;
			for (int i = 1; i <= 10; i++)
			{
				if (flag)
				{
					break;
				}
				num14 = System.Math.Sin(num9);
				num15 = System.Math.Cos(num9);
				num10 = num2 * num14;
				num11 = num7 * num15;
				num12 = num2 * num15;
				num13 = num7 * num14;
				double num16 = (num8 - num11 + num10 - num9) / (1.0 - num12 - num13) + num9;
				if (System.Math.Abs(num16 - num9) <= 1E-06)
				{
					flag = true;
				}
				else
				{
					num9 = num16;
				}
			}
			double num17 = num12 + num13;
			double num18 = num10 - num11;
			double num19 = num2 * num2 + num7 * num7;
			num3 = 1.0 - num19;
			double num20 = a * num3;
			double num21 = a * (1.0 - num17);
			double num22 = 1.0 / num21;
			double num23 = Constants.Xke * System.Math.Sqrt(a) * num18 * num22;
			double num24 = Constants.Xke * System.Math.Sqrt(num20) * num22;
			num9 = a * num22;
			double num25 = System.Math.Sqrt(num3);
			num10 = 1.0 / (1.0 + num25);
			double num26 = num9 * (num15 - num2 + num7 * num18 * num10);
			double num27 = num9 * (num14 - num7 - num2 * num18 * num10);
			double num28 = Constants.AcTan(num27, num26);
			double num29 = 2.0 * num27 * num26;
			double num30 = 2.0 * num26 * num26 - 1.0;
			num3 = 1.0 / num20;
			num22 = 0.0005413079 * num3;
			num9 = num22 * num3;
			double num31 = num21 * (1.0 - 1.5 * num9 * num25 * m_x3thm1) + 0.5 * num22 * m_x1mth2 * num30;
			double num32 = num28 - 0.25 * num9 * m_x7thm1 * num29;
			double num33 = xnode + 1.5 * num9 * m_cosio * num29;
			double num34 = incl + 1.5 * num9 * m_cosio * m_sinio * num30;
			double num35 = num23 - xn * num22 * m_x1mth2 * num29;
			double num36 = num24 + xn * num22 * (m_x1mth2 * num30 + 1.5 * m_x3thm1);
			double num37 = System.Math.Sin(num32);
			double num38 = System.Math.Cos(num32);
			double num39 = System.Math.Sin(num34);
			double num40 = System.Math.Cos(num34);
			double num41 = System.Math.Sin(num33);
			double num42 = System.Math.Cos(num33);
			double num43 = (0.0 - num41) * num40;
			double num44 = num42 * num40;
			double num45 = num43 * num37 + num42 * num38;
			double num46 = num44 * num37 + num41 * num38;
			double num47 = num39 * num37;
			double num48 = num43 * num38 - num42 * num37;
			double num49 = num44 * num38 - num41 * num37;
			double num50 = num39 * num38;
			double x = num31 * num45;
			double y = num31 * num46;
			double z = num31 * num47;
			Vector4d vector = new Vector4d(x, y, z, 0d);
			DateTime dateTime = Orbit.EpochTime.AddMinutes(tsince);
			double num51 = vector.Length * 6378.135;
			if (num51 < 6378.135)
			{
				throw new DecayException(dateTime, Orbit.SatNameLong);
			}
			double x2 = num35 * num45 + num36 * num48;
			double y2 = num35 * num46 + num36 * num49;
			double z2 = num35 * num47 + num36 * num50;
			Vector4d vel = new Vector4d(x2, y2, z2, 0d);
			return new GeoVector(vector, vel, new Calendar.JulianCalendar(dateTime), true);
		}
	}
}
