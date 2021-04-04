using System;

namespace NthDimension.Math.Geography.Norad
{
	internal class NoradSGP4 : NoradBase
	{
		private double m_c5;

		private double m_omgcof;

		private double m_xmcof;

		private double m_delmo;

		private double m_sinmo;

		public NoradSGP4(Orbit orbit)
			: base(orbit)
		{
			m_c5 = 2.0 * m_coef1 * m_aodp * m_betao2 * (1.0 + 2.75 * (m_etasq + m_eeta) + m_eeta * m_etasq);
			m_omgcof = base.Orbit.BStar * m_c3 * System.Math.Cos(base.Orbit.ArgPerigee);
			m_xmcof = -2.0 / 3.0 * m_coef * base.Orbit.BStar * 1.0 / m_eeta;
			m_delmo = System.Math.Pow(1.0 + m_eta * System.Math.Cos(base.Orbit.MeanAnomaly), 3.0);
			m_sinmo = System.Math.Sin(base.Orbit.MeanAnomaly);
		}

		public override GeoVector GetPosition(double tsince)
		{
			bool flag = false;
			if (m_aodp * (1.0 - m_satEcc) / 1.0 < 1.0344928415594841)
			{
				flag = true;
			}
			double num = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			double num4 = 0.0;
			double num5 = 0.0;
			double num6 = 0.0;
			if (!flag)
			{
				double num7 = m_c1 * m_c1;
				num = 4.0 * m_aodp * m_tsi * num7;
				double num8 = num * m_tsi * m_c1 / 3.0;
				num2 = (17.0 * m_aodp + m_s4) * num8;
				num3 = 0.5 * num8 * m_aodp * m_tsi * (221.0 * m_aodp + 31.0 * m_s4) * m_c1;
				num4 = num + 2.0 * num7;
				num5 = 0.25 * (3.0 * num2 + m_c1 * (12.0 * num + 10.0 * num7));
				num6 = 0.2 * (3.0 * num3 + 12.0 * m_c1 * num2 + 6.0 * num * num + 15.0 * num7 * (2.0 * num + num7));
			}
			double num9 = base.Orbit.MeanAnomaly + m_xmdot * tsince;
			double num10 = base.Orbit.ArgPerigee + m_omgdot * tsince;
			double num11 = base.Orbit.RAAN + m_xnodot * tsince;
			double num12 = num10;
			double num13 = num9;
			double num14 = tsince * tsince;
			double num15 = num11 + m_xnodcf * num14;
			double num16 = 1.0 - m_c1 * tsince;
			double num17 = base.Orbit.BStar * m_c4 * tsince;
			double num18 = m_t2cof * num14;
			if (!flag)
			{
				double num19 = m_omgcof * tsince;
				double num20 = m_xmcof * (System.Math.Pow(1.0 + m_eta * System.Math.Cos(num9), 3.0) - m_delmo);
				double num21 = num19 + num20;
				num13 = num9 + num21;
				num12 = num10 - num21;
				double num22 = num14 * tsince;
				double num23 = tsince * num22;
				num16 = num16 - num * num14 - num2 * num22 - num3 * num23;
				num17 += base.Orbit.BStar * m_c5 * (System.Math.Sin(num13) - m_sinmo);
				num18 = num18 + num4 * num22 + num23 * (num5 + tsince * num6);
			}
			double num24 = m_aodp * Constants.Sqr(num16);
			double e = m_satEcc - num17;
			double xl = num13 + num12 + num15 + m_xnodp * num18;
			double xn = Constants.Xke / System.Math.Pow(num24, 1.5);
			return FinalPosition(m_satInc, num10, e, num24, xl, num15, xn, tsince);
		}
	}
}
