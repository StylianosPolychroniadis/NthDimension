using System;

namespace NthDimension.Math.Astronomy
{
	/// <summary>
	/// 
	/// </summary>
	public class Uranus : SolarBodyObject
	{
		public Uranus(string name){this.name = name;}

		public override void OrbitalElements()
		{
			N = 74.0005 + 1.3978E-5 * location.dayNumber();
			i = 0.7733 + 1.9E-8 * location.dayNumber();
			w = 96.6612 + 3.0565E-5 * location.dayNumber();
			a = 19.18171 - 1.55E-8 * location.dayNumber();
			ec = 0.047318 + 7.45E-9 * location.dayNumber();
			M = 142.5905 + 0.011725806 * location.dayNumber();

			d=24;	T=84*365;	d0=56.8;	name="Uranus";

			pert.Mu = M;
		}

		public override void Perturbations()
		{
			lon = (360+(System.Math.Atan2(yeclip,xeclip) * 180/PI))%360;
			lon += pu.PertInLon();
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
			magnitude = -7.15 + 5*System.Math.Log10(helDist*dist) + 0.001*FV;
		}

		private PertElements pert = PertElements.GetInstance();
		private PertUranus pu = new PertUranus();
	}
}
