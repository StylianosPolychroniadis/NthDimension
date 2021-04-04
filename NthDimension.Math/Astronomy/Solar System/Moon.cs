using System;

namespace NthDimension.Math.Astronomy
{
	/// <summary>
	/// 
	/// </summary>
	public class Moon : SolarBodyObject
	{
		public Moon(string name){this.name = name;}

		public override void OrbitalElements()
		{
			N = 125.1228 - 0.0529538083 * location.dayNumber();
			i = 5.1454;
			w = (360+(318.0634 + 0.1643573223 * location.dayNumber()))%360;
			a = 60.2666;
			ec = 0.0549;
			M = 115.3654 + 13.0649929509 * location.dayNumber();
			M += ((int)System.Math.Abs(M/360)+1)*360;

			pert.Mm = M;
			pert.Lm = N + w + M;
			pert.D = pert.Lm - pert.Ls;
			pert.F = pert.Lm - N;
		}

		public override void Perturbations()
		{
			lon += pm.PertInLon();
			lat += pm.PertInLat();
			dist = (a + pm.PertInDist());
		}

		public override void GeocentricPos()
		{
			double xeclip2 = System.Math.Cos(lon*PI/180) * System.Math.Cos(lat*PI/180);
			double yeclip2 = System.Math.Sin(lon*PI/180) * System.Math.Cos(lat*PI/180);
			double zeclip2 = System.Math.Sin(lat*PI/180);
			double xequat = xeclip2;
			double yequat = yeclip2 * System.Math.Cos(location.oblecl*PI/180) - zeclip2*System.Math.Sin(location.oblecl*PI/180);
			double zequat = yeclip2 * System.Math.Sin(location.oblecl*PI/180) + zeclip2*System.Math.Cos(location.oblecl*PI/180);
			skyPosition.RA = (360+(System.Math.Atan2(yequat,xequat) * 180/PI))%360;
			skyPosition.decl = System.Math.Atan2(zequat,System.Math.Sqrt(xequat*xequat+yequat*yequat)) * 180/PI;
			name = "Moon";
		}

		public override void TopocentricPos()
		{
			double LON = location.v_Lon, 
				   LAT = location.v_Lat + 0.00000000001;
			double mpar = System.Math.Asin(1/dist) * 180/PI;
			double GMST0 = (pert.Ls/15 + 12 + (location.v_mainDT.Hour - LON/15) + (double)location.v_mainDT.Minute/60 + (double)location.v_mainDT.Second/3600)%24;
			double SIDTIME = (GMST0 + LON/15);
			double LST = SIDTIME * 15;
			location.SIDTIME = SIDTIME;

			double HA = (360+(LST - skyPosition.RA))%360;
			double gclat = LAT - 0.1924*System.Math.Sin((2*LAT)*PI/180);
			double rho = 0.99833 + 0.00167*System.Math.Cos((2*LAT)*PI/180);
			double g = System.Math.Atan(System.Math.Tan(gclat*PI/180)/System.Math.Cos(HA*PI/180))*180/PI;
			double topRA = skyPosition.RA - mpar*rho*System.Math.Cos(gclat*PI/180)*System.Math.Sin(HA*PI/180)/System.Math.Cos((skyPosition.decl+0.000000001)*PI/180);
			double topDecl = skyPosition.decl - mpar*rho*System.Math.Sin(gclat*PI/180)*System.Math.Sin((g-skyPosition.decl)*PI/180)/System.Math.Sin((g+0.00000001)*PI/180);
			skyPosition.RA = topRA;
			skyPosition.decl = topDecl;
		}

		public override void Ephemerides()
		{
			diam = 1873.7*60/dist;
			dist = dist*6378.140/1.49597870E8;
			elong = System.Math.Acos(
				System.Math.Sin(skyPosition.decl*System.Math.PI/180)*System.Math.Sin(location.sDecl*System.Math.PI/180)+
				System.Math.Cos(skyPosition.decl*System.Math.PI/180)*System.Math.Cos(location.sDecl*System.Math.PI/180)*
				System.Math.Cos((skyPosition.RA-location.sRA)*System.Math.PI/180))*180/System.Math.PI;
			phase = 180 - elong;
			magnitude = -10;
		}

		private PertElements pert = PertElements.GetInstance();
		private PertMoon pm = new PertMoon();
	}
}
