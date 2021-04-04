using System;

namespace NthDimension.Math.Astronomy
{
	/// <summary>
	/// 
	/// </summary>
	public class Earth : SolarBodyObject
	{
		public Earth(string name){this.name = name;}

		public override void OrbitalElements(){d=1.2;	T=365;}

		public override void HeliocentricPos()
		{
			position.x = -location.xs;
			position.y = -location.ys;
			position.z = -location.zs;
		}
	}
}
