using System;

namespace NthDimension.Math.Astronomy
{
	/// <summary>
	/// 
	/// </summary>
	public class Neptune : SolarBodyObject
	{
		public Neptune(string name){this.name = name;}

		public override void OrbitalElements()
		{
			N = 131.7806 + 3.0173E-5 * location.dayNumber();
			i = 1.77 - 2.55E-7 * location.dayNumber();
			w = 272.8461 - 6.027E-6 * location.dayNumber();
			a = 30.05826 + 3.313E-8 * location.dayNumber();
			ec = 0.008606 + 2.15E-9 * location.dayNumber();
			M = 260.2471 + 0.005995147 * location.dayNumber();
			d=33;	T=160*365;	d0=62.2;	name="Neptune";
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
			magnitude = -6.9 + 5*System.Math.Log10(helDist*dist) + 0.001*FV;
		}
	}
}
