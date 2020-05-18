using System;

namespace NthDimension.Compute
{
	/// <summary>
	/// Represents the arguments of a command status change.
	/// </summary>
	public class ComputeCommandStatusArgs : EventArgs
	{
		/// <summary>
		/// Gets the event associated with the command that had its status changed.
		/// </summary>
		public ComputeEventBase Event
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the execution status of the command represented by the event.
		/// </summary>
		/// <remarks> Returns a negative integer if the command was abnormally terminated. </remarks>
		public ComputeCommandExecutionStatus Status
		{
			get;
			private set;
		}

		/// <summary>
		/// Creates a new <c>ComputeCommandStatusArgs</c> instance.
		/// </summary>
		/// <param name="ev"> The event representing the command that had its status changed. </param>
		/// <param name="status"> The status of the command. </param>
		public ComputeCommandStatusArgs(ComputeEventBase ev, ComputeCommandExecutionStatus status)
		{
			Event = ev;
			Status = status;
		}

		/// <summary>
		/// Creates a new <c>ComputeCommandStatusArgs</c> instance.
		/// </summary>
		/// <param name="ev"> The event of the command that had its status changed. </param>
		/// <param name="status"> The status of the command. </param>
		public ComputeCommandStatusArgs(ComputeEventBase ev, int status)
			: this(ev, (ComputeCommandExecutionStatus)status)
		{
		}
	}
}
