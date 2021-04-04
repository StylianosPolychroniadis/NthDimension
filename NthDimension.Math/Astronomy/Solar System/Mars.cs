using System;

namespace NthDimension.Math.Astronomy
{
	/// <summary>
	/// 
	/// </summary>
	public class Mars : SolarBodyObject
	{
		public Mars(string name){this.name = name;}

		public override void OrbitalElements()
		{
			N = 49.5574 + 2.11081E-5 * location.dayNumber();
			i = 1.8497 - 1.78E-8 * location.dayNumber();
			w = 286.5016 + 2.92961E-5 * location.dayNumber();
			a = 1.523688;
			ec = 0.093405 + 2.516E-9 * location.dayNumber();
			M = 18.6021 + 0.5240207766 * location.dayNumber();
			M += ((int)System.Math.Abs(M/360)+1)*360;

			d=1.7;	T=687;	d0=9.36;	name="Mars";
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
			magnitude = -1.51 + 5*System.Math.Log10(helDist*dist) + 0.016*FV;
		}
	}
}
