using System;

namespace NthDimension.Math.Astronomy
{
	/// <summary>
	/// 
	/// </summary>
	public struct SkyPos
	{
		public double RA, decl, a, A;

		public void eqToaA (double SIDTIME, double LAT)
		{
			double ZPX = SIDTIME*15-RA;
			double pi = System.Math.PI;
			double AA=0, Atemp=0;

			a = System.Math.Asin(
				System.Math.Sin(decl*pi/180)*System.Math.Sin(LAT*pi/180)+
				System.Math.Cos(decl*pi/180)*System.Math.Cos(LAT*pi/180)*System.Math.Cos(ZPX*pi/180)
				         )*180/pi;

			Atemp = (-System.Math.Sin(decl*pi/180)*System.Math.Cos(LAT*pi/180)+
				     System.Math.Sin(LAT*pi/180)*System.Math.Cos(decl*pi/180)*System.Math.Cos(ZPX*pi/180))/
				     System.Math.Sin((90-a+0.0000001)*pi/180);
			if (Atemp > 1) Atemp=1;
			if (Atemp < -1) Atemp=-1;
			A = System.Math.Acos(Atemp)*180/pi;

			Atemp = System.Math.Sin(ZPX*pi/180)*System.Math.Cos(decl*pi/180)/System.Math.Sin((90-a)*pi/180);
			if (Atemp > 1) Atemp=1;
			if (Atemp < -1) Atemp=-1;
			AA = System.Math.Asin(Atemp)*180/pi;
			
			if (System.Math.Round(A-AA,3) == 180) { A = (-AA); return;}
			if (System.Math.Round(AA+A,0) == 0) { A=180+AA; return;}
			if (System.Math.Round(AA-A,0) == 0) { A=180+A; return;}
			if (System.Math.Round(AA+A,0) == 180) A=360-AA;
		}

		public void aAToeq (double SIDTIME, double LAT)
		{
			SIDTIME *= 15;
			double pi = System.Math.PI;
			double RR=0, Rtemp=0;
			LAT += 0.0001;

			Rtemp = System.Math.Cos((a-90)*pi/180)*System.Math.Sin(LAT*pi/180)-
				    System.Math.Sin((a-90)*pi/180)*System.Math.Cos(LAT*pi/180)*System.Math.Cos(A*pi/180);
			if (Rtemp > 1) Rtemp=1;
			if (Rtemp <-1) Rtemp=-1;
			decl = System.Math.Asin(Rtemp)*180/pi;

			Rtemp = System.Math.Sin((a-90)*pi/180)*System.Math.Sin(A*pi/180)/
				    System.Math.Cos(decl*pi/180);
			if (Rtemp > 1) Rtemp=1;
			if (Rtemp <-1) Rtemp=-1;
			RR = (SIDTIME - System.Math.Asin(Rtemp)*180/pi + 360)%360;

			Rtemp = (System.Math.Cos(A*pi/180)*System.Math.Sin((a-90)*pi/180)+
				     System.Math.Sin(decl*pi/180)*System.Math.Cos(LAT*pi/180))/
				    (System.Math.Sin(LAT*pi/180)*System.Math.Cos((decl)*pi/180));
			if (Rtemp > 1) Rtemp=1;
			if (Rtemp <-1) Rtemp=-1;
			RA = SIDTIME - System.Math.Acos(Rtemp)*180/pi;

			double RRR = (RR - RA + 360)%360;
			if ((System.Math.Round((RA+RR)-((2*SIDTIME+360)%360),0))%360==0 && (RRR<179.8 || System.Math.Round(RRR,1)==360)){
				RA = RR; return;
			}
			if (System.Math.Round(RRR,0)==180){RA=(180+2*SIDTIME-RR+360)%360; ;return;}
			if (System.Math.Round((RA+RR)-(2*SIDTIME),0) != 0){RA=(RR-RRR+360)%360;return;}
		}
	}
}
