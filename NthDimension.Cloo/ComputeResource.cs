#define TRACE
using System;
using System.Diagnostics;

namespace NthDimension.Compute
{
	/// <summary>
	/// Represents an OpenCL resource.
	/// </summary>
	/// <remarks> An OpenCL resource is an OpenCL object that can be created and deleted by the application. </remarks>
	/// <seealso cref="T:NthDimension.Compute.ComputeObject" />
	public abstract class ComputeResource : ComputeObject, IDisposable
	{
		/// <summary>
		/// Deletes the <see cref="T:NthDimension.Compute.ComputeResource" /> and frees its accompanying OpenCL resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
			GC.KeepAlive(this);
		}

		/// <summary>
		/// Releases the associated OpenCL object.
		/// </summary>
		/// <param name="manual"> Specifies the operation mode of this method. </param>
		/// <remarks> <paramref name="manual" /> must be <c>true</c> if this method is invoked directly by the application. </remarks>
		protected abstract void Dispose(bool manual);

		/// <summary>
		/// Releases the associated OpenCL object.
		/// </summary>
		~ComputeResource()
		{
			Trace.WriteLine(ToString() + " leaked!", "Warning");
			Dispose(false);
		}
	}
}
