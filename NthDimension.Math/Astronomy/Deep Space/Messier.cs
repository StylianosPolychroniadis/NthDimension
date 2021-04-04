using System;

namespace NthDimension.Math.Astronomy
{
	/// <summary>
	/// Summary description for Messier.
	/// </summary>
	public class Messier : DeepSpaceObject
	{
		public Messier(){}

		public Messier(string designation, double RA, double decl, string type, string name)
		{
			if (designation != "")
				this.designation = "M"+designation;
			else this.designation = name;
			this.name = name;
			this.skyPosition.RA = RA;
			this.skyPosition.decl = decl;
			this.type = type;
		}

		public string type
		{
			get {return v_type;}
			set {v_type = value;}
		}

		private string v_type;
	}
}
