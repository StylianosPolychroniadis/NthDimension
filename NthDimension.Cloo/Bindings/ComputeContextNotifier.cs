using System;

namespace NthDimension.Compute.Bindings
{
	/// <summary>
	/// A callback function that can be registered by the application to report information on errors that occur in the <see cref="T:NthDimension.Compute.ComputeContext" />.
	/// </summary>
	/// <param name="errorInfo"> An error string. </param>
	/// <param name="clDataPtr"> A pointer to binary data that is returned by the OpenCL implementation that can be used to log additional information helpful in debugging the error.</param>
	/// <param name="clDataSize"> The size of the binary data that is returned by the OpenCL. </param>
	/// <param name="userDataPtr"> The pointer to the optional user data specified in <paramref name="userDataPtr" /> argument of <see cref="T:NthDimension.Compute.ComputeContext" /> constructor. </param>
	/// <remarks> This callback function may be called asynchronously by the OpenCL implementation. It is the application's responsibility to ensure that the callback function is thread-safe. </remarks>
	public delegate void ComputeContextNotifier(string errorInfo, IntPtr clDataPtr, IntPtr clDataSize, IntPtr userDataPtr);
}
