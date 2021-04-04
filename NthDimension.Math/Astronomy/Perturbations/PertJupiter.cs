using System;

namespace NthDimension.Math.Astronomy
{
	/// <summary>
	/// 
	/// </summary>
	public class PertJupiter : PerturbationsObject
	{
		public PertJupiter(){}

		public override double PertInLon()
		{
			double lon1 = -0.332*(System.Math.Sin((2*pert.Mj-5*pert.Msat-67.6)*PI/180)); 
			double lon2 = -0.056*System.Math.Sin((2*pert.Mj-2*pert.Msat+21)*PI/180); 
			double lon3 = 0.042*System.Math.Sin((3*pert.Mj-5*pert.Msat+21)*PI/180); 
			double lon4 = -0.036*System.Math.Sin((pert.Mj-2*pert.Msat)*PI/180); 
			double lon5 = 0.022*System.Math.Cos((pert.Mj-pert.Msat)*PI/180); 
			double lon6 = 0.023*System.Math.Sin((2*pert.Mj-3*pert.Msat+52)*PI/180); 
			double lon7 = -0.016*System.Math.Sin((pert.Mj-5*pert.Msat-69)*PI/180); 
			double lon = lon1 + lon2 + lon3 + lon4 + lon5 + lon6 + lon7;
			return lon;
		}

		public override double PertInLat()
		{
			return 0;
		}

		public override double PertInDist()
		{
			return 0;
		}

		private double PI = System.Math.PI;
	}
}
