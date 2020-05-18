#define TRACE
using NthDimension.Compute.Bindings;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace NthDimension.Compute
{
	/// <summary>
	/// Represents an OpenCL event.
	/// </summary>
	/// <remarks> An event encapsulates the status of an operation such as a command. It can be used to synchronize operations in a context. </remarks>
	/// <seealso cref="T:NthDimension.Compute.ComputeUserEvent" />
	/// <seealso cref="T:NthDimension.Compute.ComputeCommandQueue" />
	/// <seealso cref="T:NthDimension.Compute.ComputeContext" />
	public class ComputeEvent : ComputeEventBase
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private GCHandle gcHandle;

		/// <summary>
		/// Gets the <see cref="T:NthDimension.Compute.ComputeCommandQueue" /> associated with the <see cref="T:NthDimension.Compute.ComputeEvent" />.
		/// </summary>
		/// <value> The <see cref="T:NthDimension.Compute.ComputeCommandQueue" /> associated with the <see cref="T:NthDimension.Compute.ComputeEvent" />. </value>
		public ComputeCommandQueue CommandQueue
		{
			get;
			private set;
		}

		internal ComputeEvent(CLEventHandle handle, ComputeCommandQueue queue)
		{
			base.Handle = handle;
			SetID(base.Handle.Value);
			CommandQueue = queue;
			base.Type = (ComputeCommandType)GetInfo<CLEventHandle, ComputeEventInfo, int>(base.Handle, ComputeEventInfo.CommandType, CL10.GetEventInfo);
			base.Context = queue.Context;
			if (Tools.ParseVersionString(CommandQueue.Device.Platform.Version, 1) > new Version(1, 0))
			{
				HookNotifier();
			}
			Trace.WriteLine("Create " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
		}

		internal void TrackGCHandle(GCHandle handle)
		{
			gcHandle = handle;
			base.Completed += Cleanup;
			base.Aborted += Cleanup;
		}

		/// <summary>
		/// Releases the associated OpenCL object.
		/// </summary>
		/// <param name="manual"> Specifies the operation mode of this method. </param>
		/// <remarks> <paramref name="manual" /> must be <c>true</c> if this method is invoked directly by the application. </remarks>
		protected override void Dispose(bool manual)
		{
			FreeGCHandle();
			base.Dispose(manual);
		}

		private void Cleanup(object sender, ComputeCommandStatusArgs e)
		{
			lock (CommandQueue.Events)
			{
				if (CommandQueue.Events.Contains(this))
				{
					CommandQueue.Events.Remove(this);
					Dispose();
				}
				else
				{
					FreeGCHandle();
				}
			}
		}

		private void FreeGCHandle()
		{
			if (gcHandle.IsAllocated && gcHandle.Target != null)
			{
				gcHandle.Free();
			}
		}
	}
}
