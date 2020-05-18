using NthDimension.Compute.Bindings;
using System.Runtime.InteropServices;

namespace NthDimension.Compute
{
	/// <summary>
	/// Represents an OpenCL sub-buffer.
	/// </summary>
	/// <typeparam name="T"> The type of the elements of the <see cref="T:NthDimension.Compute.ComputeSubBuffer`1" />. <typeparamref name="T" /> is restricted to value types and <c>struct</c>s containing such types. </typeparam>
	/// <remarks> A sub-buffer is created from a standard buffer and represents all or part of its data content. <br /> Requires OpenCL 1.1. </remarks>
	public class ComputeSubBuffer<T> : ComputeBufferBase<T> where T : struct
	{
		/// <summary>
		/// Creates a new <see cref="T:NthDimension.Compute.ComputeSubBuffer`1" /> from a specified <see cref="T:NthDimension.Compute.ComputeBuffer`1" />.
		/// </summary>
		/// <param name="buffer"> The buffer to create the <see cref="T:NthDimension.Compute.ComputeSubBuffer`1" /> from. </param>
		/// <param name="flags"> A bit-field that is used to specify allocation and usage information about the <see cref="T:NthDimension.Compute.ComputeBuffer`1" />. </param>
		/// <param name="offset"> The index of the element of <paramref name="buffer" />, where the <see cref="T:NthDimension.Compute.ComputeSubBuffer`1" /> starts. </param>
		/// <param name="count"> The number of elements of <paramref name="buffer" /> to include in the <see cref="T:NthDimension.Compute.ComputeSubBuffer`1" />. </param>
		public ComputeSubBuffer(ComputeBuffer<T> buffer, ComputeMemoryFlags flags, long offset, long count)
			: base(buffer.Context, flags)
		{
			SysIntX2 buffer_create_info = new SysIntX2(offset * Marshal.SizeOf(typeof(T)), count * Marshal.SizeOf(typeof(T)));
			ComputeErrorCode errcode_ret;
			CLMemoryHandle cLMemoryHandle = CL11.CreateSubBuffer(base.Handle, flags, ComputeBufferCreateType.Region, ref buffer_create_info, out errcode_ret);
			ComputeException.ThrowOnError(errcode_ret);
			Init();
		}
	}
}
