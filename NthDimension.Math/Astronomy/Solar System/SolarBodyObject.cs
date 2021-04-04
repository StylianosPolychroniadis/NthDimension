using System;

namespace NthDimension.Math.Astronomy
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class SolarBodyObject : ISolarBodyObject
	{
		public LocationST       location = LocationST.GetInstance();
		public PlanetPos        position = new PlanetPos();
		public SkyPos           skyPosition = new SkyPos();

		protected double        PI=System.Math.PI, helDist, sunDist;
		protected double        rr, v, xeclip, yeclip, zeclip, lon, lat;

		public SolarBodyObject(){}

		virtual public void HeliocentricPos()
		{
			ResetPlanet();
			double E0 = M + (180/System.Math.PI * ec * System.Math.Sin(M*PI/180) * (1 + ec * System.Math.Cos(M*PI/180)));
			double E1=0;
			for (short k=0; k<100; ++k){
				E1 = E0 - (E0 - (180/PI)*ec*System.Math.Sin(E0*PI/180) - M)/(1 + ec*System.Math.Sin(E0*PI/180));
				if (System.Math.Abs(E0-E1) < 0.00005)	break;
				E0 = E1;
			}
			double E = E0;

			double x1 = a * (System.Math.Cos(E*PI/180) - ec);
			double y1 = a * System.Math.Sqrt(1-ec*ec) * System.Math.Sin(E*PI/180);
			rr = System.Math.Sqrt(x1*x1+y1*y1);
			v = (360 + System.Math.Atan2(y1,x1)*180/PI)%360;

			position.x = xeclip = rr*(System.Math.Cos(N*PI/180)*System.Math.Cos((v+w)*PI/180)-System.Math.Sin(N*PI/180)*System.Math.Sin((v+w)*PI/180)*System.Math.Cos(i*PI/180));
			position.y = yeclip = rr*(System.Math.Sin(N*PI/180)*System.Math.Cos((v+w)*PI/180)+System.Math.Cos(N*PI/180)*System.Math.Sin((v+w)*PI/180)*System.Math.Cos(i*PI/180));
			position.z = zeclip = rr*System.Math.Sin((v+w)*PI/180)*System.Math.Sin(i*PI/180);

			helDist = System.Math.Sqrt(xeclip*xeclip + yeclip*yeclip + zeclip*zeclip);

			lon = (360+(System.Math.Atan2(yeclip,xeclip)*180/PI))%360;
			lat = System.Math.Asin(zeclip/rr)*180/PI;
			a = System.Math.Sqrt(xeclip*xeclip + yeclip*yeclip + zeclip * zeclip);
		}

		virtual public void Perturbations(){}

		virtual public void GeocentricPos()
		{
			xeclip += location.xs;
			yeclip += location.ys;
			zeclip += location.zs;
			dist = System.Math.Sqrt(xeclip*xeclip + yeclip*yeclip + zeclip*zeclip);

			double xequat = xeclip;
			double yequat = yeclip*System.Math.Cos(location.oblecl*PI/180) - zeclip*System.Math.Sin(location.oblecl*PI/180);
			double zequat = yeclip*System.Math.Sin(location.oblecl*PI/180) + zeclip*System.Math.Cos(location.oblecl*PI/180);

			sunDist = System.Math.Sqrt(location.xs*location.xs + location.ys*location.ys + location.zs*location.zs);

			skyPosition.RA = (360+(System.Math.Atan2(yequat,xequat) * 180/PI))%360;
			skyPosition.decl = System.Math.Atan2(zequat,System.Math.Sqrt(xequat*xequat+yequat*yequat)) * 180/PI;
		}

		virtual public void TopocentricPos(){}

		virtual public void Ephemerides(){}

		public void ResetPlanet()
		{
			skyPosition.RA=0;	skyPosition.decl=0;
			position.x=0;	position.y=0;	position.z=0;
		}

		#region ISolObject Members

		abstract public void OrbitalElements();

		public double d {
			get {return v_d;}
			set {v_d = value;}
		}
		public double T {
			get {return v_T;}
			set {v_T = value;}
		}
		public double d0 {
			get {return v_d0;}
			set {v_d0 = value;}
		}
		public string name {
			get {return v_name;}
			set {v_name = value;}
		}
		public double N {
			get {return v_N;}
			set {v_N = value;}
		}
		public double i{
			get {return v_i;}
			set {v_i = value;}
		}
		public double w{
			get {return v_w;}
			set {v_w = value;}
		}
		public double a{
			get {return v_a;}
			set {v_a = value;}
		}
		public double ec {
			get {return v_ec;}
			set {v_ec = value;}
		}
		public double M {
			get {return v_M;}
			set {v_M = value;}
		}
		public double dist {
			get {return v_dist;}
			set {v_dist = value;}
		}
		public double diam {
			get {return v_diam;}
			set {v_diam = value;}
		}
		public double elong {
			get {return v_elong;}
			set {v_elong = value;}
		}
		public double FV {
			get {return v_FV;}
			set {v_FV = value;}
		}
		public double phase {
			get {return v_phase;}
			set {v_phase = value;}
		}
		public double magnitude {
			get {return v_mag;}
			set {v_mag = value;}
		}

		private double v_d, v_T, v_d0, v_N, v_i, v_w, v_a, v_ec, v_M,
			           v_dist, v_diam, v_elong, v_phase, v_FV, v_mag;
		private string v_name;

		#endregion
	}
}
