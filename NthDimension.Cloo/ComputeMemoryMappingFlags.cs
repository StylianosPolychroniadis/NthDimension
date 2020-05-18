using System;

namespace NthDimension.Compute
{
	/// <summary>
	///
	/// </summary>
	[Flags]
	public enum ComputeMemoryMappingFlags : long
	{
		/// <summary> </summary>
		Read = 0x1,
		/// <summary> </summary>
		Write = 0x2
	}
}
