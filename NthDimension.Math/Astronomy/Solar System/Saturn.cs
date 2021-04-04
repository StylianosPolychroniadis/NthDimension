using System;

namespace NthDimension.Math.Astronomy
{
	/// <summary>
	/// 
	/// </summary>
	public class Saturn : SolarBodyObject
	{
		public double ringTilt;

		public Saturn(string name){this.name = name;}

		public override void OrbitalElements()
		{
			N = 113.6634 + 2.3898E-5 * location.dayNumber();
			i = 2.4886 - 1.081E-7 * location.dayNumber();
			w = 339.3939 + 2.97661E-5 * location.dayNumber();
			a = 9.55475;
			ec = 0.055546 - 9.499E-9 * location.dayNumber();
			M = 316.967 + 0.0334442282 * location.dayNumber();

			d=12;	T=29.5*365;	d0=165.6;	name="Saturn";

			pert.Msat = M;
		}

		public override void Perturbations()
		{
			lon = (360+(System.Math.Atan2(yeclip,xeclip) * 180/PI))%360;
			lat = System.Math.Asin(zeclip/rr) * 180/PI;

			lon += ps.PertInLon();
			lat += ps.PertInLat();
		}

		public override void Ephemerides()
		{
			diam = d0/dist;
			double test = (sunDist*sunDist + dist*dist - helDist*helDist)/
				          (2*sunDist*dist+0.000000001);
			if (test < -1) test=-1;
			if (test > 1) test=1;
			elong = System.Math.Acos(test)*180/PI;

			test = (helDist*helDist + dist*dist - sunDist*sunDist)/
				   (2*helDist*dist+0.000000001);
			if (test < -1) test=-1;
			if (test > 1) test=1;
			FV = System.Math.Acos(test)*180/PI;
			phase = (1+System.Math.Cos(FV*PI/180))/2;

			double ir = 28.06;
			double Nr = 169.51 + 3.82E-5*location.dayNumber();
			double ringTilt = System.Math.Asin(
				System.Math.Sin(lat*PI/180)*System.Math.Cos(ir*PI/180)-
				System.Math.Cos(lat*PI/180)*System.Math.Sin(ir*PI/180)*System.Math.Sin((lon-Nr)*PI/180)
				                        )*180/PI;
			double ring_mag = -2.6*System.Math.Sin(System.Math.Abs(ringTilt)*PI/180) + 1.2*System.Math.Pow(System.Math.Sin(ringTilt*PI/180),2);
			magnitude = -9.0 + 5*System.Math.Log10(helDist*dist) + 0.044*FV + ring_mag;
		}

		private PertElements pert = PertElements.GetInstance();
		private PertSaturn ps = new PertSaturn();
	}
}
