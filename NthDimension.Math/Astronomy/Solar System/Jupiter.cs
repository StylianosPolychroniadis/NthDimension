using System;

namespace NthDimension.Math.Astronomy
{
	/// <summary>
	/// 
	/// </summary>
	public class Jupiter : SolarBodyObject
	{
		public Jupiter(string name){this.name = name;}

		public override void OrbitalElements()
		{
			N = 100.4542 + 2.76854E-5 * location.dayNumber();
			i = 1.303 - 1.557E-7 * location.dayNumber();
			w = 273.8777 + 1.64505E-5 * location.dayNumber();
			a = 5.20256;
			ec = 0.048498 + 4.469E-9 * location.dayNumber();
			M = 19.895 + 0.0830853001 * location.dayNumber();
			M += ((int)System.Math.Abs(M/360)+1)*360;
			d=6;	T=11.9*365;	d0=196.94;	name="Jupiter";

			pert.Mj = M;
		}

		public override void Perturbations()
		{
			lon = (360+(System.Math.Atan2(yeclip,xeclip) * 180/PI))%360;
			lon += pj.PertInLon();
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
			magnitude = -9.25 + 5*System.Math.Log10(helDist*dist) + 0.014*FV;
		}

		private PertElements pert = PertElements.GetInstance();
		private PertJupiter pj = new PertJupiter();
	}
}
