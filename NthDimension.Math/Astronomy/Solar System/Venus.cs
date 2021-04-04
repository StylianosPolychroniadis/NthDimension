using System;

namespace NthDimension.Math.Astronomy
{
	/// <summary>
	/// 
	/// </summary>
	public class Venus : SolarBodyObject
	{
		public Venus(string name){this.name = name;}

		public override void OrbitalElements()
		{
			N = 76.6799 + 2.4659E-5 * location.dayNumber();
			i = 3.3946 + 2.75E-8 * location.dayNumber();
			w = 54.891 + 1.38374E-5 * location.dayNumber();
			a = 0.72333;
			ec = 0.006773 - 1.302E-9 * location.dayNumber();
			M = 48.0052 + 1.6021302244 * location.dayNumber();
			M += ((int)System.Math.Abs(M/360)+1)*360;

			d=0.9;	T=225;	d0=16.92;	name="Venus";
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
			magnitude = -4.34 + 5*System.Math.Log10(helDist*dist) + 0.013*FV + (4.2E-7)*System.Math.Pow(FV,3);
		}
	}
}
