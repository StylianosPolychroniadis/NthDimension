using System;

namespace NthDimension.Math.Astronomy
{
	/// <summary>
	/// Summary description for MoonShadow.
	/// </summary>
	public class EarthShadow : SolarBodyObject
	{
		public EarthShadow(string name){this.name = name;}

		public override void OrbitalElements(){}

		public override void GeocentricPos()
		{
			skyPosition.RA = (location.sRA+180)%360;
			skyPosition.decl = -location.sDecl;
		}

		public double DU 
		{
			get {return v_du;}
			set {v_du = value;}
		}
		public double DP 
		{
			get {return v_dp;}
			set {v_dp = value;}
		}

		private double v_du, v_dp;
	}
}
