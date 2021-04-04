using System;

namespace NthDimension.Math.Astronomy
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class PerturbationsObject : IPerturbationsObject
	{
		public PertElements pert = PertElements.GetInstance();

		public PerturbationsObject(){}

		#region IPerturbations Members

		abstract public double PertInLon();
		abstract public double PertInLat();
		abstract public double PertInDist();

		#endregion
	}
}
