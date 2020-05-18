using System;

namespace NthDimension.Compute
{
	/// <summary>
	///
	/// </summary>
	[Flags]
	public enum ComputeCommandQueueFlags : long
	{
		/// <summary> </summary>
		None = 0x0,
		/// <summary> </summary>
		OutOfOrderExecution = 0x1,
		/// <summary> </summary>
		Profiling = 0x2
	}
}
