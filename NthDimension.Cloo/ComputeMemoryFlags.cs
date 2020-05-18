using System;

namespace NthDimension.Compute
{
	/// <summary>
	///
	/// </summary>
	[Flags]
	public enum ComputeMemoryFlags : long
	{
		/// <summary> Let the OpenCL choose the default flags. </summary>
		None = 0x0,
		/// <summary> The <see cref="T:NthDimension.Compute.ComputeMemory" /> will be accessible from the <see cref="T:NthDimension.Compute.ComputeKernel" /> for read and write operations. </summary>
		ReadWrite = 0x1,
		/// <summary> The <see cref="T:NthDimension.Compute.ComputeMemory" /> will be accessible from the <see cref="T:NthDimension.Compute.ComputeKernel" /> for write operations only. </summary>
		WriteOnly = 0x2,
		/// <summary> The <see cref="T:NthDimension.Compute.ComputeMemory" /> will be accessible from the <see cref="T:NthDimension.Compute.ComputeKernel" /> for read operations only. </summary>
		ReadOnly = 0x4,
		/// <summary> </summary>
		UseHostPointer = 0x8,
		/// <summary> </summary>
		AllocateHostPointer = 0x10,
		/// <summary> </summary>
		CopyHostPointer = 0x20
	}
}
