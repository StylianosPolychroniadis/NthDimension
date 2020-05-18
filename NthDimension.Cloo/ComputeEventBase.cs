#define TRACE
using NthDimension.Compute.Bindings;
using System;
using System.Diagnostics;
using System.Threading;

namespace NthDimension.Compute
{
	/// <summary>
	/// Represents the parent type to any Cloo event types.
	/// </summary>
	/// <seealso cref="T:NthDimension.Compute.ComputeEvent" />
	/// <seealso cref="T:NthDimension.Compute.ComputeUserEvent" />
	public abstract class ComputeEventBase : ComputeResource
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ComputeCommandStatusArgs status;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ComputeEventCallback statusNotify;

		/// <summary>
		/// The handle of the <see cref="T:NthDimension.Compute.ComputeEventBase" />.
		/// </summary>
		public CLEventHandle Handle
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the <see cref="T:NthDimension.Compute.ComputeContext" /> associated with the <see cref="T:NthDimension.Compute.ComputeEventBase" />.
		/// </summary>
		/// <value> The <see cref="T:NthDimension.Compute.ComputeContext" /> associated with the <see cref="T:NthDimension.Compute.ComputeEventBase" />. </value>
		public ComputeContext Context
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the <see cref="T:NthDimension.Compute.ComputeDevice" /> time counter in nanoseconds when the associated command has finished execution.
		/// </summary>
		/// <value> The <see cref="T:NthDimension.Compute.ComputeDevice" /> time counter in nanoseconds when the associated command has finished execution. </value>
		public long FinishTime
		{
			get
			{
				return GetInfo<CLEventHandle, ComputeCommandProfilingInfo, long>(Handle, ComputeCommandProfilingInfo.Ended, CL10.GetEventProfilingInfo);
			}
		}

		/// <summary>
		/// Gets the <see cref="T:NthDimension.Compute.ComputeDevice" /> time counter in nanoseconds when the associated command is enqueued in the <see cref="T:NthDimension.Compute.ComputeCommandQueue" /> by the host.
		/// </summary>
		/// <value> The <see cref="T:NthDimension.Compute.ComputeDevice" /> time counter in nanoseconds when the associated command is enqueued in the <see cref="T:NthDimension.Compute.ComputeCommandQueue" /> by the host. </value>
		public long EnqueueTime
		{
			get
			{
				return GetInfo<CLEventHandle, ComputeCommandProfilingInfo, long>(Handle, ComputeCommandProfilingInfo.Queued, CL10.GetEventProfilingInfo);
			}
		}

		/// <summary>
		/// Gets the execution status of the associated command.
		/// </summary>
		/// <value> The execution status of the associated command or a negative value if the execution was abnormally terminated. </value>
		public ComputeCommandExecutionStatus Status
		{
			get
			{
				return (ComputeCommandExecutionStatus)GetInfo<CLEventHandle, ComputeEventInfo, int>(Handle, ComputeEventInfo.ExecutionStatus, CL10.GetEventInfo);
			}
		}

		/// <summary>
		/// Gets the <see cref="T:NthDimension.Compute.ComputeDevice" /> time counter in nanoseconds when the associated command starts execution.
		/// </summary>
		/// <value> The <see cref="T:NthDimension.Compute.ComputeDevice" /> time counter in nanoseconds when the associated command starts execution. </value>
		public long StartTime
		{
			get
			{
				return (long)GetInfo<CLEventHandle, ComputeCommandProfilingInfo, ulong>(Handle, ComputeCommandProfilingInfo.Started, CL10.GetEventProfilingInfo);
			}
		}

		/// <summary>
		/// Gets the <see cref="T:NthDimension.Compute.ComputeDevice" /> time counter in nanoseconds when the associated command that has been enqueued is submitted by the host to the device.
		/// </summary>
		/// <value> The <see cref="T:NthDimension.Compute.ComputeDevice" /> time counter in nanoseconds when the associated command that has been enqueued is submitted by the host to the device. </value>
		public long SubmitTime
		{
			get
			{
				return (long)GetInfo<CLEventHandle, ComputeCommandProfilingInfo, ulong>(Handle, ComputeCommandProfilingInfo.Submitted, CL10.GetEventProfilingInfo);
			}
		}

		/// <summary>
		/// Gets the <see cref="T:NthDimension.Compute.ComputeCommandType" /> associated with the event.
		/// </summary>
		/// <value> The <see cref="T:NthDimension.Compute.ComputeCommandType" /> associated with the event. </value>
		public ComputeCommandType Type
		{
			get;
			protected set;
		}

		private event ComputeCommandStatusChanged aborted;

		private event ComputeCommandStatusChanged completed;

		/// <summary>
		/// Occurs when the command associated with the event is abnormally terminated.
		/// </summary>
		/// <remarks> Requires OpenCL 1.1. </remarks>
		public event ComputeCommandStatusChanged Aborted
		{
			add
			{
				aborted += value;
				if (status != null && status.Status != 0)
				{
					value(this, status);
				}
			}
			remove
			{
				aborted -= value;
			}
		}

		/// <summary>
		/// Occurs when <c>ComputeEventBase.Status</c> changes to <c>ComputeCommandExecutionStatus.Complete</c>.
		/// </summary>
		/// <remarks> Requires OpenCL 1.1. </remarks>
		public event ComputeCommandStatusChanged Completed
		{
			add
			{
				completed += value;
				if (status != null && status.Status == ComputeCommandExecutionStatus.Complete)
				{
					value(this, status);
				}
			}
			remove
			{
				completed -= value;
			}
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
				CL10.ReleaseEvent(Handle);
				Handle.Invalidate();
			}
		}

		/// <summary>
		///
		/// </summary>
		protected void HookNotifier()
		{
			statusNotify = StatusNotify;
			ComputeErrorCode errorCode = CL11.SetEventCallback(Handle, 0, statusNotify, IntPtr.Zero);
			ComputeException.ThrowOnError(errorCode);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="evArgs"></param>
		protected virtual void OnCompleted(object sender, ComputeCommandStatusArgs evArgs)
		{
			Trace.WriteLine("Complete " + Type + " operation of " + this + ".", "Information");
			if (this.completed != null)
			{
				this.completed(sender, evArgs);
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="evArgs"></param>
		protected virtual void OnAborted(object sender, ComputeCommandStatusArgs evArgs)
		{
			Trace.WriteLine("Abort " + Type + " operation of " + this + ".", "Information");
			if (this.aborted != null)
			{
				this.aborted(sender, evArgs);
			}
		}

		private void StatusNotify(CLEventHandle eventHandle, int cmdExecStatusOrErr, IntPtr userData)
		{
			status = new ComputeCommandStatusArgs(this, (ComputeCommandExecutionStatus)cmdExecStatusOrErr);
			if (cmdExecStatusOrErr == 0)
			{
				OnCompleted(this, status);
			}
			else
			{
				OnAborted(this, status);
			}
		}
	}
}
