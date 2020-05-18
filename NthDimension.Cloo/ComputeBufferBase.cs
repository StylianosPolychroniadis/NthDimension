#define TRACE
using NthDimension.Compute.Bindings;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace NthDimension.Compute
{
	/// <summary>
	/// Represents the parent type to any Cloo buffer types.
	/// </summary>
	/// <typeparam name="T"> The type of the elements of the buffer. </typeparam>
	public abstract class ComputeBufferBase<T> : ComputeMemory where T : struct
	{
		/// <summary>
		/// Gets the number of elements in the <see cref="T:NthDimension.Compute.ComputeBufferBase`1" />.
		/// </summary>
		/// <value> The number of elements in the <see cref="T:NthDimension.Compute.ComputeBufferBase`1" />. </value>
		public long Count
		{
			get;
			private set;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="context"></param>
		/// <param name="flags"></param>
		protected ComputeBufferBase(ComputeContext context, ComputeMemoryFlags flags)
			: base(context, flags)
		{
		}

		/// <summary>
		///
		/// </summary>
		protected void Init()
		{
			SetID(base.Handle.Value);
			base.Size = (long)GetInfo<CLMemoryHandle, ComputeMemoryInfo, IntPtr>(base.Handle, ComputeMemoryInfo.Size, CL10.GetMemObjectInfo);
			Count = base.Size / Marshal.SizeOf(typeof(T));
			Trace.WriteLine("Create " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
		}
	}
}
