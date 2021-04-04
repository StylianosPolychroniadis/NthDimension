using System;

namespace NthDimension.Math.Astronomy
{
	/// <summary>
	/// 
	/// </summary>
	public class PertElements
	{
		public static PertElements GetInstance()
		{
			if( instance == null )
				instance = new PertElements();
			return instance;
		}

		public double Mj {
			get {return v_Mj;}
			set {v_Mj = value;}
		}

		public double Msat {
			get {return v_Msat;}
			set {v_Msat = value;}
		}

		public double Mu {
			get {return v_Mu;}
			set {v_Mu = value;}
		}

		public double Ls {
			get {return v_Ls;}
			set {v_Ls = value;}
		}

		public double Lm {
			get {return v_Lm;}
			set {v_Lm = value;}
		}

		public double Msun {
			get {return v_Msun;}
			set {v_Msun = value;}
		}

		public double Mm {
			get {return v_Mm;}
			set {v_Mm = value;}
		}

		public double D {
			get {return v_D;}
			set {v_D = value;}
		}

		public double F {
			get {return v_F;}
			set {v_F = value;}
		}

		private double v_Mj;
		private double v_Msat;
		private double v_Mu;
		private double v_Ls;
		private double v_Lm;
		private double v_Msun;
		private double v_Mm;
		private double v_D;
		private double v_F;

		private PertElements(){}
		private static PertElements instance;
	}
}
