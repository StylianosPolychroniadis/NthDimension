using System;

namespace NthDimension.Math.Astronomy
{
	/// <summary>
	/// 
	/// </summary>
	public class Sun : SolarBodyObject
	{
		public Sun(string name){this.name = name;}

		public override void OrbitalElements()
		{
			N=0; i=0; a=1;
			w = 282.9404 + 4.70935E-5 * location.dayNumber();
			ec = 0.016709 - 1.151E-9 * location.dayNumber();
			M = 356.047 + 0.9856002585 * location.dayNumber();
			M += ((int)System.Math.Abs(M/360)+1)*360;

			location.oblecl = 23.4393 - 3.563E-7 * location.dayNumber();

			pert.Msun = M;
		}

		public override void GeocentricPos()
		{
			double L = (w + M)%360; 
			double E = M + (180/System.Math.PI * ec * System.Math.Sin(M*PI/180) * (1 + ec * System.Math.Cos(M*PI/180)));
			double x1 = System.Math.Cos(E*PI/180) - ec;
			double y1 = System.Math.Sin(E*PI/180)*System.Math.Sqrt(1-ec*ec);
			double r = System.Math.Sqrt(x1*x1+y1*y1);
			double v = System.Math.Atan2(y1,x1) * 180/PI;
			double lon = v + w;
			position.x = r * System.Math.Cos(lon*PI/180);
			position.y = r * System.Math.Sin(lon*PI/180);
			position.z = 0.0;
			double xeq = position.x;
			double yeq = position.y * System.Math.Cos(location.oblecl*PI/180) - position.z * System.Math.Sin(location.oblecl*PI/180);
			double zeq = position.y * System.Math.Sin(location.oblecl*PI/180) + position.z * System.Math.Cos(location.oblecl*PI/180);
			dist = System.Math.Sqrt(xeq*xeq + yeq*yeq + zeq*zeq);
			name = "Sun";

			skyPosition.RA = (360+(System.Math.Atan2(yeq,xeq) * 180/PI))%360;
			skyPosition.decl = System.Math.Asin(zeq/dist) * 180/PI;

			pert.Ls = L; 
			location.xs = position.x;
			location.ys = position.y;
			location.zs = position.z;
			location.slon = lon;
			location.sRA = skyPosition.RA;
			location.sDecl = skyPosition.decl;
		}

		public override void Ephemerides()
		{
			diam = 1919.26;
			magnitude = -24;
		}

		private PertElements pert = PertElements.GetInstance();
	}
}
