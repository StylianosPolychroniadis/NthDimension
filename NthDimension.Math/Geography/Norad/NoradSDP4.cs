using System;

namespace NthDimension.Math.Geography.Norad
{
	internal class NoradSDP4 : NoradBase
	{
		private const double zns = 1.19459E-05;

		private const double zes = 0.01675;

		private const double znl = 0.00015835218;

		private const double zel = 0.0549;

		private const double thdt = 0.0043752691;

		private double dp_e3;

		private double dp_ee2;

		private double dp_se2;

		private double dp_se3;

		private double dp_sgh2;

		private double dp_sgh3;

		private double dp_sgh4;

		private double dp_sh2;

		private double dp_sh3;

		private double dp_si2;

		private double dp_si3;

		private double dp_sl2;

		private double dp_sl3;

		private double dp_sl4;

		private double dp_xgh2;

		private double dp_xgh3;

		private double dp_xgh4;

		private double dp_xh2;

		private double dp_xh3;

		private double dp_xi2;

		private double dp_xi3;

		private double dp_xl2;

		private double dp_xl3;

		private double dp_xl4;

		private double dp_xqncl;

		private double dp_zmol;

		private double dp_zmos;

		private double dp_atime;

		private double dp_d2201;

		private double dp_d2211;

		private double dp_d3210;

		private double dp_d3222;

		private double dp_d4410;

		private double dp_d4422;

		private double dp_d5220;

		private double dp_d5232;

		private double dp_d5421;

		private double dp_d5433;

		private double dp_del1;

		private double dp_del2;

		private double dp_del3;

		private double dp_omegaq;

		private double dp_sse;

		private double dp_ssg;

		private double dp_ssh;

		private double dp_ssi;

		private double dp_ssl;

		private double dp_step2;

		private double dp_stepn;

		private double dp_stepp;

		private double dp_thgr;

		private double dp_xfact;

		private double dp_xlamo;

		private double dp_xli;

		private double dp_xni;

		private bool gp_reso;

		private bool gp_sync;

		public NoradSDP4(Orbit orbit)
			: base(orbit)
		{
			double num = System.Math.Sin(base.Orbit.ArgPerigee);
			double num2 = System.Math.Cos(base.Orbit.ArgPerigee);
			Calendar.JulianCalendar epoch = base.Orbit.Epoch;
			dp_thgr = epoch.ToGmst();
			double eccentricity = base.Orbit.Eccentricity;
			double num3 = 1.0 / base.Orbit.SemiMajor;
			dp_xqncl = base.Orbit.Inclination;
			double meanAnomaly = base.Orbit.MeanAnomaly;
			double num4 = m_omgdot + m_xnodot;
			double num5 = System.Math.Sin(base.Orbit.RAAN);
			double num6 = System.Math.Cos(base.Orbit.RAAN);
			dp_omegaq = base.Orbit.ArgPerigee;
			double num7 = epoch.FromJan0_12h_1900();
			double num8 = 4.523602 - 0.00092422029 * num7;
			double num9 = System.Math.Sin(num8);
			double num10 = System.Math.Cos(num8);
			double num11 = 0.91375164 - 0.03568096 * num10;
			double num12 = System.Math.Sqrt(1.0 - num11 * num11);
			double num13 = 0.089683511 * num9 / num12;
			double num14 = System.Math.Sqrt(1.0 - num13 * num13);
			double num15 = 4.7199672 + 0.2299715 * num7;
			double num16 = 5.8351514 + 0.001944368 * num7;
			dp_zmol = Constants.Fmod2p(num15 - num16);
			double sinx = 0.39785416 * num9 / num12;
			double cosx = num14 * num10 + 0.91744867 * num13 * num9;
			sinx = Constants.AcTan(sinx, cosx) + num16 - num8;
			double num17 = System.Math.Cos(sinx);
			double num18 = System.Math.Sin(sinx);
			dp_zmos = 6.2565837 + 0.017201977 * num7;
			dp_zmos = Constants.Fmod2p(dp_zmos);
			double num19 = 0.1945905;
			double num20 = -0.98088458;
			double num21 = 0.91744867;
			double num22 = 0.39785416;
			double num23 = num6;
			double num24 = num5;
			double num25 = 2.9864797E-06;
			double num26 = 1.19459E-05;
			double num27 = 0.01675;
			double num28 = 1.0 / base.Orbit.MeanMotion;
			double num29 = 0.0;
			double num30 = 0.0;
			double num31 = 0.0;
			double num32 = 0.0;
			double num33 = 0.0;
			double num34 = Constants.Sqr(base.Orbit.Eccentricity);
			for (int i = 1; i <= 2; i++)
			{
				double num35 = num19 * num23 + num20 * num21 * num24;
				double num36 = (0.0 - num20) * num23 + num19 * num21 * num24;
				double num37 = (0.0 - num19) * num24 + num20 * num21 * num23;
				double num38 = num20 * num22;
				double num39 = num20 * num24 + num19 * num21 * num23;
				double num40 = num19 * num22;
				double num41 = m_cosio * num37 + m_sinio * num38;
				double num42 = m_cosio * num39 + m_sinio * num40;
				double num43 = (0.0 - m_sinio) * num37 + m_cosio * num38;
				double num44 = (0.0 - m_sinio) * num39 + m_cosio * num40;
				double num45 = num35 * num2 + num41 * num;
				double num46 = num36 * num2 + num42 * num;
				double num47 = (0.0 - num35) * num + num41 * num2;
				double num48 = (0.0 - num36) * num + num42 * num2;
				double num49 = num43 * num;
				double num50 = num44 * num;
				double num51 = num43 * num2;
				double num52 = num44 * num2;
				double num53 = 12.0 * num45 * num45 - 3.0 * num47 * num47;
				double num54 = 24.0 * num45 * num46 - 6.0 * num47 * num48;
				double num55 = 12.0 * num46 * num46 - 3.0 * num48 * num48;
				double num56 = 3.0 * (num35 * num35 + num41 * num41) + num53 * num34;
				double num57 = 6.0 * (num35 * num36 + num41 * num42) + num54 * num34;
				double num58 = 3.0 * (num36 * num36 + num42 * num42) + num55 * num34;
				double num59 = -6.0 * num35 * num43 + num34 * (-24.0 * num45 * num51 - 6.0 * num47 * num49);
				double num60 = -6.0 * (num35 * num44 + num36 * num43) + num34 * (-24.0 * (num46 * num51 + num45 * num52) - 6.0 * (num47 * num50 + num48 * num49));
				double num61 = -6.0 * num36 * num44 + num34 * (-24.0 * num46 * num52 - 6.0 * num48 * num50);
				double num62 = 6.0 * num41 * num43 + num34 * (24.0 * num45 * num49 - 6.0 * num47 * num51);
				double num63 = 6.0 * (num42 * num43 + num41 * num44) + num34 * (24.0 * (num46 * num49 + num45 * num50) - 6.0 * (num48 * num51 + num47 * num52));
				double num64 = 6.0 * num42 * num44 + num34 * (24.0 * num46 * num50 - 6.0 * num48 * num52);
				num56 = num56 + num56 + m_betao2 * num53;
				num57 = num57 + num57 + m_betao2 * num54;
				num58 = num58 + num58 + m_betao2 * num55;
				double num65 = num25 * num28;
				double num66 = -0.5 * num65 / m_betao;
				double num67 = num65 * m_betao;
				double num68 = -15.0 * eccentricity * num67;
				double num69 = num45 * num47 + num46 * num48;
				double num70 = num46 * num47 + num45 * num48;
				double num71 = num46 * num48 - num45 * num47;
				num29 = num68 * num26 * num69;
				num30 = num66 * num26 * (num59 + num61);
				num31 = (0.0 - num26) * num65 * (num56 + num58 - 14.0 - 6.0 * num34);
				num32 = num67 * num26 * (num53 + num55 - 6.0);
				num33 = ((!(base.Orbit.Inclination < 0.052359877)) ? ((0.0 - num26) * num66 * (num62 + num64)) : 0.0);
				dp_ee2 = 2.0 * num68 * num70;
				dp_e3 = 2.0 * num68 * num71;
				dp_xi2 = 2.0 * num66 * num60;
				dp_xi3 = 2.0 * num66 * (num61 - num59);
				dp_xl2 = -2.0 * num65 * num57;
				dp_xl3 = -2.0 * num65 * (num58 - num56);
				dp_xl4 = -2.0 * num65 * (-21.0 - 9.0 * num34) * num27;
				dp_xgh2 = 2.0 * num67 * num54;
				dp_xgh3 = 2.0 * num67 * (num55 - num53);
				dp_xgh4 = -18.0 * num67 * num27;
				dp_xh2 = -2.0 * num66 * num63;
				dp_xh3 = -2.0 * num66 * (num64 - num62);
				if (i == 1)
				{
					dp_sse = num29;
					dp_ssi = num30;
					dp_ssl = num31;
					dp_ssh = num33 / m_sinio;
					dp_ssg = num32 - m_cosio * dp_ssh;
					dp_se2 = dp_ee2;
					dp_si2 = dp_xi2;
					dp_sl2 = dp_xl2;
					dp_sgh2 = dp_xgh2;
					dp_sh2 = dp_xh2;
					dp_se3 = dp_e3;
					dp_si3 = dp_xi3;
					dp_sl3 = dp_xl3;
					dp_sgh3 = dp_xgh3;
					dp_sh3 = dp_xh3;
					dp_sl4 = dp_xl4;
					dp_sgh4 = dp_xgh4;
					num19 = num17;
					num20 = num18;
					num21 = num11;
					num22 = num12;
					num23 = num14 * num6 + num13 * num5;
					num24 = num5 * num14 - num6 * num13;
					num26 = 0.00015835218;
					num25 = 4.7968065E-07;
					num27 = 0.0549;
				}
			}
			dp_sse += num29;
			dp_ssi += num30;
			dp_ssl += num31;
			dp_ssg = dp_ssg + num32 - m_cosio / m_sinio * num33;
			dp_ssh += num33 / m_sinio;
			gp_reso = false;
			gp_sync = false;
			double num72 = 0.0;
			if (base.Orbit.MeanMotion > 0.0034906585 && base.Orbit.MeanMotion < 0.0052359877)
			{
				gp_reso = true;
				gp_sync = true;
				double num73 = 1.0 + num34 * (-2.5 + 0.8125 * num34);
				double num74 = 1.0 + 2.0 * num34;
				double num75 = 1.0 + num34 * (-6.0 + 6.60937 * num34);
				double num76 = 0.75 * (1.0 + m_cosio) * (1.0 + m_cosio);
				double num77 = 0.9375 * m_sinio * m_sinio * (1.0 + 3.0 * m_cosio) - 0.75 * (1.0 + m_cosio);
				double num78 = 1.0 + m_cosio;
				num78 = 1.875 * num78 * num78 * num78;
				dp_del1 = 3.0 * m_xnodp * m_xnodp * num3 * num3;
				dp_del2 = 2.0 * dp_del1 * num76 * num73 * 1.7891679E-06;
				dp_del3 = 3.0 * dp_del1 * num78 * num75 * 2.2123015E-07 * num3;
				dp_del1 = dp_del1 * num77 * num74 * 2.1460748E-06 * num3;
				dp_xlamo = meanAnomaly + base.Orbit.RAAN + base.Orbit.ArgPerigee - dp_thgr;
				num72 = m_xmdot + num4 - 0.0043752691;
				num72 = num72 + dp_ssl + dp_ssg + dp_ssh;
			}
			else if (base.Orbit.MeanMotion >= 0.00826 && base.Orbit.MeanMotion <= 0.00924 && eccentricity >= 0.5)
			{
				gp_reso = true;
				double num79 = eccentricity * num34;
				double num80 = -0.306 - (eccentricity - 0.64) * 0.44;
				double num81;
				double num82;
				double num83;
				double num84;
				double num85;
				double num86;
				if (eccentricity <= 0.65)
				{
					num81 = 3.616 - 13.247 * eccentricity + 16.29 * num34;
					num82 = -19.302 + 117.39 * eccentricity - 228.419 * num34 + 156.591 * num79;
					num83 = -18.9068 + 109.7927 * eccentricity - 214.6334 * num34 + 146.5816 * num79;
					num84 = -41.122 + 242.694 * eccentricity - 471.094 * num34 + 313.953 * num79;
					num85 = -146.407 + 841.88 * eccentricity - 1629.014 * num34 + 1083.435 * num79;
					num86 = -532.114 + 3017.977 * eccentricity - 5740.0 * num34 + 3708.276 * num79;
				}
				else
				{
					num81 = -72.099 + 331.819 * eccentricity - 508.738 * num34 + 266.724 * num79;
					num82 = -346.844 + 1582.851 * eccentricity - 2415.925 * num34 + 1246.113 * num79;
					num83 = -342.585 + 1554.908 * eccentricity - 2366.899 * num34 + 1215.972 * num79;
					num84 = -1052.797 + 4758.686 * eccentricity - 7193.992 * num34 + 3651.957 * num79;
					num85 = -3581.69 + 16178.11 * eccentricity - 24462.77 * num34 + 12422.52 * num79;
					num86 = ((!(eccentricity <= 0.715)) ? (-5149.66 + 29936.92 * eccentricity - 54087.36 * num34 + 31324.56 * num79) : (1464.74 - 4664.75 * eccentricity + 3763.64 * num34));
				}
				double num87;
				double num88;
				double num89;
				if (eccentricity < 0.7)
				{
					num87 = -919.2277 + 4988.61 * eccentricity - 9064.77 * num34 + 5542.21 * num79;
					num88 = -822.71072 + 4568.6173 * eccentricity - 8491.4146 * num34 + 5337.524 * num79;
					num89 = -853.666 + 4690.25 * eccentricity - 8624.77 * num34 + 5341.4 * num79;
				}
				else
				{
					num87 = -37995.78 + 161616.52 * eccentricity - 229838.2 * num34 + 109377.94 * num79;
					num88 = -51752.104 + 218913.95 * eccentricity - 309468.16 * num34 + 146349.42 * num79;
					num89 = -40023.88 + 170470.89 * eccentricity - 242699.48 * num34 + 115605.82 * num79;
				}
				double num90 = m_sinio * m_sinio;
				double num91 = m_cosio * m_cosio;
				double num92 = 0.75 * (1.0 + 2.0 * m_cosio + num91);
				double num93 = 1.5 * num90;
				double num94 = 1.875 * m_sinio * (1.0 - 2.0 * m_cosio - 3.0 * num91);
				double num95 = -1.875 * m_sinio * (1.0 + 2.0 * m_cosio - 3.0 * num91);
				double num96 = 35.0 * num90 * num92;
				double num97 = 39.375 * num90 * num90;
				double num98 = 9.84375 * m_sinio * (num90 * (1.0 - 2.0 * m_cosio - 5.0 * num91) + 0.33333333 * (-2.0 + 4.0 * m_cosio + 6.0 * num91));
				double num99 = m_sinio * (4.92187512 * num90 * (-2.0 - 4.0 * m_cosio + 10.0 * num91) + 6.56250012 * (1.0 + 2.0 * m_cosio - 3.0 * num91));
				double num100 = 29.53125 * m_sinio * (2.0 - 8.0 * m_cosio + num91 * (-12.0 + 8.0 * m_cosio + 10.0 * num91));
				double num101 = 29.53125 * m_sinio * (-2.0 - 8.0 * m_cosio + num91 * (12.0 + 8.0 * m_cosio - 10.0 * num91));
				double num102 = m_xnodp * m_xnodp;
				double num103 = num3 * num3;
				double num104 = 3.0 * num102 * num103;
				double num105 = num104 * 1.7891679E-06;
				dp_d2201 = num105 * num92 * num80;
				dp_d2211 = num105 * num93 * num81;
				num104 *= num3;
				num105 = num104 * 3.7393792E-07;
				dp_d3210 = num105 * num94 * num82;
				dp_d3222 = num105 * num95 * num83;
				num104 *= num3;
				num105 = 2.0 * num104 * 7.3636953E-09;
				dp_d4410 = num105 * num96 * num84;
				dp_d4422 = num105 * num97 * num85;
				num104 *= num3;
				num105 = num104 * 1.1428639E-07;
				dp_d5220 = num105 * num98 * num86;
				dp_d5232 = num105 * num99 * num89;
				num105 = 2.0 * num104 * 2.1765803E-09;
				dp_d5421 = num105 * num100 * num88;
				dp_d5433 = num105 * num101 * num87;
				dp_xlamo = meanAnomaly + base.Orbit.RAAN + base.Orbit.RAAN - dp_thgr - dp_thgr;
				num72 = m_xmdot + m_xnodot + m_xnodot - 0.0043752691 - 0.0043752691;
				num72 = num72 + dp_ssl + dp_ssh + dp_ssh;
			}
			if (gp_reso || gp_sync)
			{
				dp_xfact = num72 - m_xnodp;
				dp_xli = dp_xlamo;
				dp_xni = m_xnodp;
				dp_stepp = 720.0;
				dp_stepn = -720.0;
				dp_step2 = 259200.0;
			}
		}

		private bool DeepCalcDotTerms(ref double pxndot, ref double pxnddt, ref double pxldot)
		{
			if (gp_sync)
			{
				pxndot = dp_del1 * System.Math.Sin(dp_xli - 0.13130908) + dp_del2 * System.Math.Sin(2.0 * (dp_xli - 2.8843198)) + dp_del3 * System.Math.Sin(3.0 * (dp_xli - 0.37448087));
				pxnddt = dp_del1 * System.Math.Cos(dp_xli - 0.13130908) + 2.0 * dp_del2 * System.Math.Cos(2.0 * (dp_xli - 2.8843198)) + 3.0 * dp_del3 * System.Math.Cos(3.0 * (dp_xli - 0.37448087));
			}
			else
			{
				double num = dp_omegaq + m_omgdot * dp_atime;
				double num2 = num + num;
				double num3 = dp_xli + dp_xli;
				pxndot = dp_d2201 * System.Math.Sin(num2 + dp_xli - 5.7686396) + dp_d2211 * System.Math.Sin(dp_xli - 5.7686396) + dp_d3210 * System.Math.Sin(num + dp_xli - 0.95240898) + dp_d3222 * System.Math.Sin(0.0 - num + dp_xli - 0.95240898) + dp_d4410 * System.Math.Sin(num2 + num3 - 1.8014998) + dp_d4422 * System.Math.Sin(num3 - 1.8014998) + dp_d5220 * System.Math.Sin(num + dp_xli - 1.050833) + dp_d5232 * System.Math.Sin(0.0 - num + dp_xli - 1.050833) + dp_d5421 * System.Math.Sin(num + num3 - 4.4108898) + dp_d5433 * System.Math.Sin(0.0 - num + num3 - 4.4108898);
				pxnddt = dp_d2201 * System.Math.Cos(num2 + dp_xli - 5.7686396) + dp_d2211 * System.Math.Cos(dp_xli - 5.7686396) + dp_d3210 * System.Math.Cos(num + dp_xli - 0.95240898) + dp_d3222 * System.Math.Cos(0.0 - num + dp_xli - 0.95240898) + dp_d5220 * System.Math.Cos(num + dp_xli - 1.050833) + dp_d5232 * System.Math.Cos(0.0 - num + dp_xli - 1.050833) + 2.0 * (dp_d4410 * System.Math.Cos(num2 + num3 - 1.8014998) + dp_d4422 * System.Math.Cos(num3 - 1.8014998) + dp_d5421 * System.Math.Cos(num + num3 - 4.4108898) + dp_d5433 * System.Math.Cos(0.0 - num + num3 - 4.4108898));
			}
			pxldot = dp_xni + dp_xfact;
			pxnddt *= pxldot;
			return true;
		}

		private void DeepCalcIntegrator(ref double pxndot, ref double pxnddt, ref double pxldot, double delt)
		{
			DeepCalcDotTerms(ref pxndot, ref pxnddt, ref pxldot);
			dp_xli = dp_xli + pxldot * delt + pxndot * dp_step2;
			dp_xni = dp_xni + pxndot * delt + pxnddt * dp_step2;
			dp_atime += delt;
		}

		private bool DeepSecular(ref double xmdf, ref double omgadf, ref double xnode, ref double emm, ref double xincc, ref double xnn, ref double tsince)
		{
			xmdf += dp_ssl * tsince;
			omgadf += dp_ssg * tsince;
			xnode += dp_ssh * tsince;
			emm = base.Orbit.Eccentricity + dp_sse * tsince;
			xincc = base.Orbit.Inclination + dp_ssi * tsince;
			if (xincc < 0.0)
			{
				xincc = 0.0 - xincc;
				xnode += System.Math.PI;
				omgadf -= System.Math.PI;
			}
			double pxnddt = 0.0;
			double pxndot = 0.0;
			double pxldot = 0.0;
			double num = 0.0;
			double delt = 0.0;
			bool flag = false;
			if (gp_reso)
			{
				while (!flag)
				{
					if (dp_atime == 0.0 || (tsince >= 0.0 && dp_atime < 0.0) || (tsince < 0.0 && dp_atime >= 0.0))
					{
						delt = ((tsince < 0.0) ? dp_stepn : dp_stepp);
						dp_atime = 0.0;
						dp_xni = m_xnodp;
						dp_xli = dp_xlamo;
						flag = true;
					}
					else if (System.Math.Abs(tsince) < System.Math.Abs(dp_atime))
					{
						delt = dp_stepp;
						if (tsince >= 0.0)
						{
							delt = dp_stepn;
						}
						DeepCalcIntegrator(ref pxndot, ref pxnddt, ref pxldot, delt);
					}
					else
					{
						delt = dp_stepn;
						if (tsince > 0.0)
						{
							delt = dp_stepp;
						}
						flag = true;
					}
				}
				while (System.Math.Abs(tsince - dp_atime) >= dp_stepp)
				{
					DeepCalcIntegrator(ref pxndot, ref pxnddt, ref pxldot, delt);
				}
				num = tsince - dp_atime;
				DeepCalcDotTerms(ref pxndot, ref pxnddt, ref pxldot);
				xnn = dp_xni + pxndot * num + pxnddt * num * num * 0.5;
				double num2 = dp_xli + pxldot * num + pxndot * num * num * 0.5;
				double num3 = 0.0 - xnode + dp_thgr + tsince * 0.0043752691;
				xmdf = num2 - omgadf + num3;
				if (!gp_sync)
				{
					xmdf = num2 + num3 + num3;
				}
			}
			return true;
		}

		private bool DeepPeriodics(ref double e, ref double xincc, ref double omgadf, ref double xnode, ref double xmam, double tsince)
		{
			double num = System.Math.Sin(xincc);
			double num2 = System.Math.Cos(xincc);
			double num3 = 0.0;
			double num4 = 0.0;
			double num5 = 0.0;
			double num6 = 0.0;
			double num7 = 0.0;
			double num8 = 0.0;
			double num9 = 0.0;
			double num10 = dp_zmos + 1.19459E-05 * tsince;
			double num11 = num10 + 0.0335 * System.Math.Sin(num10);
			double num12 = System.Math.Sin(num11);
			double num13 = 0.5 * num12 * num12 - 0.25;
			double num14 = -0.5 * num12 * System.Math.Cos(num11);
			double num15 = dp_se2 * num13 + dp_se3 * num14;
			double num16 = dp_si2 * num13 + dp_si3 * num14;
			double num17 = dp_sl2 * num13 + dp_sl3 * num14 + dp_sl4 * num12;
			num3 = dp_sgh2 * num13 + dp_sgh3 * num14 + dp_sgh4 * num12;
			num4 = dp_sh2 * num13 + dp_sh3 * num14;
			num10 = dp_zmol + 0.00015835218 * tsince;
			num11 = num10 + 0.1098 * System.Math.Sin(num10);
			num12 = System.Math.Sin(num11);
			num13 = 0.5 * num12 * num12 - 0.25;
			num14 = -0.5 * num12 * System.Math.Cos(num11);
			double num18 = dp_ee2 * num13 + dp_e3 * num14;
			double num19 = dp_xi2 * num13 + dp_xi3 * num14;
			double num20 = dp_xl2 * num13 + dp_xl3 * num14 + dp_xl4 * num12;
			num9 = dp_xgh2 * num13 + dp_xgh3 * num14 + dp_xgh4 * num12;
			num5 = dp_xh2 * num13 + dp_xh3 * num14;
			num6 = num15 + num18;
			num7 = num16 + num19;
			num8 = num17 + num20;
			double num21 = num3 + num9;
			double num22 = num4 + num5;
			xincc += num7;
			e += num6;
			if (dp_xqncl >= 0.2)
			{
				num22 /= m_sinio;
				num21 -= m_cosio * num22;
				omgadf += num21;
				xnode += num22;
				xmam += num8;
			}
			else
			{
				double num23 = System.Math.Sin(xnode);
				double num24 = System.Math.Cos(xnode);
				double num25 = num * num23;
				double num26 = num * num24;
				double num27 = num22 * num24 + num7 * num2 * num23;
				double num28 = (0.0 - num22) * num23 + num7 * num2 * num24;
				num25 += num27;
				num26 += num28;
				double num29 = xmam + omgadf + num2 * xnode;
				double num30 = num8 + num21 - num7 * xnode * num;
				num29 += num30;
				xnode = Constants.AcTan(num25, num26);
				xmam += num8;
				omgadf = num29 - xmam - System.Math.Cos(xincc) * xnode;
			}
			return true;
		}

		public override GeoVector GetPosition(double tsince)
		{
			double xmdf = base.Orbit.MeanAnomaly + m_xmdot * tsince;
			double omgadf = base.Orbit.ArgPerigee + m_omgdot * tsince;
			double num = base.Orbit.RAAN + m_xnodot * tsince;
			double num2 = tsince * tsince;
			double xnode = num + m_xnodcf * num2;
			double x = 1.0 - m_c1 * tsince;
			double num3 = base.Orbit.BStar * m_c4 * tsince;
			double num4 = m_t2cof * num2;
			double xnn = m_xnodp;
			double emm = 0.0;
			double xincc = 0.0;
			DeepSecular(ref xmdf, ref omgadf, ref xnode, ref emm, ref xincc, ref xnn, ref tsince);
			double num5 = System.Math.Pow(Constants.Xke / xnn, 2.0 / 3.0) * Constants.Sqr(x);
			double e = emm - num3;
			double xmam = xmdf + m_xnodp * num4;
			DeepPeriodics(ref e, ref xincc, ref omgadf, ref xnode, ref xmam, tsince);
			double xl = xmam + omgadf + xnode;
			xnn = Constants.Xke / System.Math.Pow(num5, 1.5);
			return FinalPosition(xincc, omgadf, e, num5, xl, xnode, xnn, tsince);
		}
	}
}
