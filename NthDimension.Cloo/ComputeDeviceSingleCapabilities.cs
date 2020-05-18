using System;

namespace NthDimension.Compute
{
	/// <summary>
	///
	/// </summary>
	[Flags]
	public enum ComputeDeviceSingleCapabilities : long
	{
		/// <summary> </summary>
		Denorm = 0x1,
		/// <summary> </summary>
		InfNan = 0x2,
		/// <summary> </summary>
		RoundToNearest = 0x4,
		/// <summary> </summary>
		RoundToZero = 0x8,
		/// <summary> </summary>
		RoundToInf = 0x10,
		/// <summary> </summary>
		Fma = 0x20,
		/// <summary> </summary>
		SoftFloat = 0x40
	}
}
