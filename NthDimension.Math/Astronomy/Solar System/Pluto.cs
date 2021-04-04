using System;

namespace NthDimension.Math.Astronomy
{
	/// <summary>
	/// 
	/// </summary>
	public class Pluto : SolarBodyObject
	{
		public Pluto(string name){this.name = name;}

		public override void OrbitalElements()
		{
			N = 110.30347 - 0.0000002839 * location.dayNumber();
			i = 17.14175 + 8.418889999999999E-8 * location.dayNumber();
			double lp = 224.06676 - .00000100578 * location.dayNumber();
			w = lp - N;
			ec = 0.24880766 + 0.00000000177002 * location.dayNumber();
			a = 39.48168677 - 0.0000000210574 * location.dayNumber();
			M = 14.882 + 0.00396*location.dayNumber();
			d=50;	T=248*365;	d0=3.14;	name="Pluto";
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
			magnitude = 14;
		}
	}
}
