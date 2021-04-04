using System;

namespace NthDimension.Math.Astronomy
{
	/// <summary>
	/// 
	/// </summary>
	public class PertSaturn : PerturbationsObject
	{
		public PertSaturn(){}

		public override double PertInLon()
		{
			double lon1 = 0.812*System.Math.Sin((2*pert.Mj-5*pert.Msat-67.6)*PI/180); 
			double lon2 = -0.229*System.Math.Cos((2*pert.Mj-4*pert.Msat-2)*PI/180); 
			double lon3 = 0.119*System.Math.Sin((pert.Mj-2*pert.Msat-3)*PI/180); 
			double lon4 = 0.046*System.Math.Sin((2*pert.Mj-6*pert.Msat-69)*PI/180); 
			double lon5 = 0.014*System.Math.Sin((pert.Mj-3*pert.Msat+32)*PI/180); 
			double lon = lon1 + lon2 + lon3 + lon4 + lon5;
			return lon;
		}

		public override double PertInLat()
		{
			double lat1 = -0.02*System.Math.Cos((2*pert.Mj-4*pert.Msat-2)*PI/180); 
			double lat2 = 0.018*System.Math.Sin((2*pert.Mj-6*pert.Msat-49)*PI/180);
			double lat = lat1 + lat2;
			return lat;
		}

		public override double PertInDist()
		{
			return 0;
		}

		private double PI = System.Math.PI;
	}
}
