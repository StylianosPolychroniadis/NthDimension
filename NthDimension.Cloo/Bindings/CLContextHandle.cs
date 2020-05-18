using System;
using System.Diagnostics;

namespace NthDimension.Compute.Bindings
{
	/// <summary>
	/// Represents the <see cref="T:NthDimension.Compute.ComputeContext" /> ID.
	/// </summary>
	public struct CLContextHandle
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private IntPtr value;

		/// <summary>
		/// Gets a logic value indicating whether the handle is valid.
		/// </summary>
		public bool IsValid
		{
			get
			{
				return value != IntPtr.Zero;
			}
		}

		/// <summary>
		/// Gets the value of the handle.
		/// </summary>
		public IntPtr Value
		{
			get
			{
				return value;
			}
		}

		/// <summary>
		/// Invalidates the handle.
		/// </summary>
		public void Invalidate()
		{
			value = IntPtr.Zero;
		}
	}
}
