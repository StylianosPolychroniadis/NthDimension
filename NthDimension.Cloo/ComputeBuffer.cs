using NthDimension.Compute.Bindings;
using System;
using System.Runtime.InteropServices;

namespace NthDimension.Compute
{
	/// <summary>
	/// Represents an OpenCL buffer.
	/// </summary>
	/// <typeparam name="T"> The type of the elements of the <see cref="T:NthDimension.Compute.ComputeBuffer`1" />. <typeparamref name="T" /> is restricted to value types and <c>struct</c>s containing such types. </typeparam>
	/// <remarks> A memory object that stores a linear collection of bytes. Buffer objects are accessible using a pointer in a kernel executing on a device. </remarks>
	/// <seealso cref="T:NthDimension.Compute.ComputeDevice" />
	/// <seealso cref="T:NthDimension.Compute.ComputeKernel" />
	/// <seealso cref="T:NthDimension.Compute.ComputeMemory" />
	public class ComputeBuffer<T> : ComputeBufferBase<T> where T : struct
	{
		/// <summary>
		/// Creates a new <see cref="T:NthDimension.Compute.ComputeBuffer`1" />.
		/// </summary>
		/// <param name="context"> A <see cref="T:NthDimension.Compute.ComputeContext" /> used to create the <see cref="T:NthDimension.Compute.ComputeBuffer`1" />. </param>
		/// <param name="flags"> A bit-field that is used to specify allocation and usage information about the <see cref="T:NthDimension.Compute.ComputeBuffer`1" />. </param>
		/// <param name="count"> The number of elements of the <see cref="T:NthDimension.Compute.ComputeBuffer`1" />. </param>
		public ComputeBuffer(ComputeContext context, ComputeMemoryFlags flags, long count)
			: this(context, flags, count, IntPtr.Zero)
		{
		}

		/// <summary>
		/// Creates a new <see cref="T:NthDimension.Compute.ComputeBuffer`1" />.
		/// </summary>
		/// <param name="context"> A <see cref="T:NthDimension.Compute.ComputeContext" /> used to create the <see cref="T:NthDimension.Compute.ComputeBuffer`1" />. </param>
		/// <param name="flags"> A bit-field that is used to specify allocation and usage information about the <see cref="T:NthDimension.Compute.ComputeBuffer`1" />. </param>
		/// <param name="count"> The number of elements of the <see cref="T:NthDimension.Compute.ComputeBuffer`1" />. </param>
		/// <param name="dataPtr"> A pointer to the data for the <see cref="T:NthDimension.Compute.ComputeBuffer`1" />. </param>
		public ComputeBuffer(ComputeContext context, ComputeMemoryFlags flags, long count, IntPtr dataPtr)
			: base(context, flags)
		{
			ComputeErrorCode errcode_ret = ComputeErrorCode.Success;
			base.Handle = CL10.CreateBuffer(context.Handle, flags, new IntPtr(Marshal.SizeOf(typeof(T)) * count), dataPtr, out errcode_ret);
			ComputeException.ThrowOnError(errcode_ret);
			Init();
		}

		/// <summary>
		/// Creates a new <see cref="T:NthDimension.Compute.ComputeBuffer`1" />.
		/// </summary>
		/// <param name="context"> A <see cref="T:NthDimension.Compute.ComputeContext" /> used to create the <see cref="T:NthDimension.Compute.ComputeBuffer`1" />. </param>
		/// <param name="flags"> A bit-field that is used to specify allocation and usage information about the <see cref="T:NthDimension.Compute.ComputeBuffer`1" />. </param>
		/// <param name="data"> The data for the <see cref="T:NthDimension.Compute.ComputeBuffer`1" />. </param>
		/// <remarks> Note, that <paramref name="data" /> cannot be an "immediate" parameter, i.e.: <c>new T[100]</c>, because it could be quickly collected by the GC causing Cloo to send and invalid reference to OpenCL. </remarks>
		public ComputeBuffer(ComputeContext context, ComputeMemoryFlags flags, T[] data)
			: base(context, flags)
		{
			GCHandle gCHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
			try
			{
				ComputeErrorCode errcode_ret = ComputeErrorCode.Success;
				base.Handle = CL10.CreateBuffer(context.Handle, flags, new IntPtr(Marshal.SizeOf(typeof(T)) * data.Length), gCHandle.AddrOfPinnedObject(), out errcode_ret);
				ComputeException.ThrowOnError(errcode_ret);
			}
			finally
			{
				gCHandle.Free();
			}
			Init();
		}

		private ComputeBuffer(CLMemoryHandle handle, ComputeContext context, ComputeMemoryFlags flags)
			: base(context, flags)
		{
			base.Handle = handle;
			Init();
		}

		/// <summary>
		/// Creates a new <see cref="T:NthDimension.Compute.ComputeBuffer`1" /> from an existing OpenGL buffer object.
		/// </summary>
		/// <typeparam name="DataType"> The type of the elements of the <see cref="T:NthDimension.Compute.ComputeBuffer`1" />. <typeparamref name="T" /> should match the type of the elements in the OpenGL buffer. </typeparam>
		/// <param name="context"> A <see cref="T:NthDimension.Compute.ComputeContext" /> with enabled CL/GL sharing. </param>
		/// <param name="flags"> A bit-field that is used to specify usage information about the <see cref="T:NthDimension.Compute.ComputeBuffer`1" />. Only <see cref="F:Cloo.ComputeMemoryFlags.ReadOnly" />, <see cref="F:Cloo.ComputeMemoryFlags.WriteOnly" /> and <see cref="F:Cloo.ComputeMemoryFlags.ReadWrite" /> are allowed. </param>
		/// <param name="bufferId"> The OpenGL buffer object id to use for the creation of the <see cref="T:NthDimension.Compute.ComputeBuffer`1" />. </param>
		/// <returns> The created <see cref="T:NthDimension.Compute.ComputeBuffer`1" />. </returns>
		public static ComputeBuffer<DataType> CreateFromGLBuffer<DataType>(ComputeContext context, ComputeMemoryFlags flags, int bufferId) where DataType : struct
		{
			ComputeErrorCode errcode_ret = ComputeErrorCode.Success;
			CLMemoryHandle handle = CL10.CreateFromGLBuffer(context.Handle, flags, bufferId, out errcode_ret);
			ComputeException.ThrowOnError(errcode_ret);
			return new ComputeBuffer<DataType>(handle, context, flags);
		}
	}
}
