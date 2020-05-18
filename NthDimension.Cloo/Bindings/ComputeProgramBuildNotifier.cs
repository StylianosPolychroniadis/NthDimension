using System;

namespace NthDimension.Compute.Bindings
{
	/// <summary>
	/// A callback function that can be registered by the application to report the <see cref="T:NthDimension.Compute.ComputeProgram" /> build status.
	/// </summary>
	/// <param name="programHandle"> The handle of the <see cref="T:NthDimension.Compute.ComputeProgram" /> being built. </param>
	/// <param name="notifyDataPtr"> The pointer to the optional user data specified in <paramref name="notifyDataPtr" /> argument of <see cref="M:NthDimension.Compute.ComputeProgram.Build(System.Collections.Generic.ICollection{Cloo.ComputeDevice},System.String,Cloo.Bindings.ComputeProgramBuildNotifier,System.IntPtr)" />. </param>
	/// <remarks> This callback function may be called asynchronously by the OpenCL implementation. It is the application's responsibility to ensure that the callback function is thread-safe. </remarks>
	public delegate void ComputeProgramBuildNotifier(CLProgramHandle programHandle, IntPtr notifyDataPtr);
}
