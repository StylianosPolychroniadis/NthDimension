#define TRACE
using NthDimension.Compute.Bindings;
using System.Diagnostics;
using System.Threading;

namespace NthDimension.Compute
{
	/// <summary>
	/// Represents an user created event.
	/// </summary>
	/// <remarks> Requires OpenCL 1.1. </remarks>
	public class ComputeUserEvent : ComputeEventBase
	{
		/// <summary>
		/// Creates a new <see cref="T:NthDimension.Compute.ComputeUserEvent" />.
		/// </summary>
		/// <param name="context"> The <see cref="T:NthDimension.Compute.ComputeContext" /> in which the <see cref="T:NthDimension.Compute.ComputeUserEvent" /> is created. </param>
		/// <remarks> Requires OpenCL 1.1. </remarks>
		public ComputeUserEvent(ComputeContext context)
		{
			ComputeErrorCode errcode_ret;
			base.Handle = CL11.CreateUserEvent(context.Handle, out errcode_ret);
			ComputeException.ThrowOnError(errcode_ret);
			SetID(base.Handle.Value);
			base.Type = (ComputeCommandType)GetInfo<CLEventHandle, ComputeEventInfo, uint>(base.Handle, ComputeEventInfo.CommandType, CL10.GetEventInfo);
			base.Context = context;
			HookNotifier();
			Trace.WriteLine("Create " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
		}

		/// <summary>
		/// Sets the new status of the <see cref="T:NthDimension.Compute.ComputeUserEvent" />.
		/// </summary>
		/// <param name="status"> The new status of the <see cref="T:NthDimension.Compute.ComputeUserEvent" />. Allowed value is <see cref="F:Cloo.ComputeCommandExecutionStatus.Complete" />. </param>
		public void SetStatus(ComputeCommandExecutionStatus status)
		{
			SetStatus((int)status);
		}

		/// <summary>
		/// Sets the new status of the <see cref="T:NthDimension.Compute.ComputeUserEvent" /> to an error value.
		/// </summary>
		/// <param name="status"> The error status of the <see cref="T:NthDimension.Compute.ComputeUserEvent" />. This should be a negative value. </param>
		public void SetStatus(int status)
		{
			ComputeErrorCode errorCode = CL11.SetUserEventStatus(base.Handle, status);
			ComputeException.ThrowOnError(errorCode);
		}
	}
}
