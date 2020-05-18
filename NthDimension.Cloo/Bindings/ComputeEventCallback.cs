using System;

namespace NthDimension.Compute.Bindings
{
	/// <summary>
	/// The event callback function that can be registered by the application.
	/// </summary>
	/// <param name="eventHandle"> The event object for which the callback function is invoked. </param>
	/// <param name="cmdExecStatusOrErr"> Represents the execution status of the command for which this callback function is invoked. If the callback is called as the result of the command associated with the event being abnormally terminated, an appropriate error code for the error that caused the termination will be passed to <paramref name="cmdExecStatusOrErr" /> instead. </param>
	/// <param name="userData"> A pointer to user supplied data. </param>
	/// /// <remarks> This callback function may be called asynchronously by the OpenCL implementation. It is the application's responsibility to ensure that the callback function is thread-safe. </remarks>
	public delegate void ComputeEventCallback(CLEventHandle eventHandle, int cmdExecStatusOrErr, IntPtr userData);
}
