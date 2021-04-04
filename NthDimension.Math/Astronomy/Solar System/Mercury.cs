using System;

namespace NthDimension.Math.Astronomy
{
	/// <summary>
	/// 
	/// </summary>
	public class Mercury : SolarBodyObject
	{
		public Mercury(string name){this.name = name;}

		public override void OrbitalElements()
		{
			N = 48.3313 + 3.24587E-5 * location.dayNumber();
			i = 7.0047 + 5E-8 * location.dayNumber();
			w = 29.1241 + 1.01444E-5 * location.dayNumber();
			a = 0.387098;
			ec = 0.205635 + 5.59E-10 * location.dayNumber();
			M = 168.6562 + 4.0923344368 * location.dayNumber();
			M += ((int)System.Math.Abs(M/360)+1)*360;

			d=0.7;	T=87;	d0=6.74;	name="Mercury";
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
			magnitude = -0.36 + 5*System.Math.Log10(helDist*dist) + 0.027*FV + (2.2E-13)*System.Math.Pow(FV,6);
		}
	}
}
