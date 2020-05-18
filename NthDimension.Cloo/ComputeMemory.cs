#define TRACE
using NthDimension.Compute.Bindings;
using System.Diagnostics;
using System.Threading;

namespace NthDimension.Compute
{
	/// <summary>
	/// Represents an OpenCL memory object.
	/// </summary>
	/// <remarks> A memory object is a handle to a region of global memory. </remarks>
	/// <seealso cref="T:NthDimension.Compute.ComputeBuffer`1" />
	/// <seealso cref="T:NthDimension.Compute.ComputeImage" />
	public abstract class ComputeMemory : ComputeResource
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ComputeContext context;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ComputeMemoryFlags flags;

		/// <summary>
		/// The handle of the <see cref="T:NthDimension.Compute.ComputeMemory" />.
		/// </summary>
		public CLMemoryHandle Handle
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the <see cref="T:NthDimension.Compute.ComputeContext" /> of the <see cref="T:NthDimension.Compute.ComputeMemory" />.
		/// </summary>
		/// <value> The <see cref="T:NthDimension.Compute.ComputeContext" /> of the <see cref="T:NthDimension.Compute.ComputeMemory" />. </value>
		public ComputeContext Context
		{
			get
			{
				return context;
			}
		}

		/// <summary>
		/// Gets the <see cref="T:NthDimension.Compute.ComputeMemoryFlags" /> of the <see cref="T:NthDimension.Compute.ComputeMemory" />.
		/// </summary>
		/// <value> The <see cref="T:NthDimension.Compute.ComputeMemoryFlags" /> of the <see cref="T:NthDimension.Compute.ComputeMemory" />. </value>
		public ComputeMemoryFlags Flags
		{
			get
			{
				return flags;
			}
		}

		/// <summary>
		/// Gets or sets (protected) the size in bytes of the <see cref="T:NthDimension.Compute.ComputeMemory" />.
		/// </summary>
		/// <value> The size in bytes of the <see cref="T:NthDimension.Compute.ComputeMemory" />. </value>
		public long Size
		{
			get;
			protected set;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="context"></param>
		/// <param name="flags"></param>
		protected ComputeMemory(ComputeContext context, ComputeMemoryFlags flags)
		{
			this.context = context;
			this.flags = flags;
		}

		/// <summary>
		/// Releases the associated OpenCL object.
		/// </summary>
		/// <param name="manual"> Specifies the operation mode of this method. </param>
		/// <remarks> <paramref name="manual" /> must be <c>true</c> if this method is invoked directly by the application. </remarks>
		protected override void Dispose(bool manual)
		{
			if (Handle.IsValid)
			{
				Trace.WriteLine("Dispose " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
				CL10.ReleaseMemObject(Handle);
				Handle.Invalidate();
			}
		}
	}
}
