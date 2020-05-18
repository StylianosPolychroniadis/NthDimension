using System;

namespace NthDimension.Compute.Bindings
{
	/// <summary>
	/// A callback function that can be registered by the application.
	/// </summary>
	/// <param name="memobj"> The memory object being deleted. When the user callback is called, this memory object is not longer valid. <paramref name="memobj" /> is only provided for reference purposes. </param>
	/// <param name="user_data"> A pointer to user supplied data. </param>
	/// /// <remarks> This callback function may be called asynchronously by the OpenCL implementation. It is the application's responsibility to ensure that the callback function is thread-safe. </remarks>
	public delegate void ComputeMemoryDestructorNotifer(CLMemoryHandle memobj, IntPtr user_data);
}
