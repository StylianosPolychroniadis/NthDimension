using System;

namespace NthDimension.Math.Astronomy
{
	/// <summary>
	/// Summary description for Stars.
	/// </summary>
	public class Star : DeepSpaceObject
	{
		public Star(){}

		public Star(string designation, string name, double RA, double decl, double magnitude, string spectrum)
		{
			this.designation = designation;
			this.name = name;
			this.skyPosition.RA = RA;
			this.skyPosition.decl = decl;
			this.magnitude = magnitude;
			this.spectrum = spectrum;
		}

		public string spectrum
		{
			get {return v_spectrum;}
			set {v_spectrum = value;}
		}

		public double magnitude
		{
			get {return v_magnitude;}
			set {v_magnitude = value;}
		}

		private string v_spectrum;
		private double v_magnitude;
	}
}
