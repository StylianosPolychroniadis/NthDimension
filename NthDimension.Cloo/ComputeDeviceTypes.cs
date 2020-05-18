using System;

namespace NthDimension.Compute
{
	/// <summary>
	/// The types of devices.
	/// </summary>
	[Flags]
	public enum ComputeDeviceTypes : long
	{
		/// <summary> </summary>
		Default = 0x1,
		/// <summary> </summary>
		Cpu = 0x2,
		/// <summary> </summary>
		Gpu = 0x4,
		/// <summary> </summary>
		Accelerator = 0x8,
		/// <summary> </summary>
		All = 0xFFFFFFFF
	}
}
