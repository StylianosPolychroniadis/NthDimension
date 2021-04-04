using System;

namespace NthDimension.Math.Astronomy
{
	/// <summary>
	/// Summary description for Constellation.
	/// </summary>
	public class ConstellationLine
	{
		public SkyPos sp1, sp2;

		public ConstellationLine(double RA1, double decl1, double RA2, double decl2)
		{
			sp1.RA = RA1;
			sp1.decl = decl1;
			sp2.RA = RA2;
			sp2.decl = decl2;
		}
	}
}
