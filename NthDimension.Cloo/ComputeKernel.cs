#define TRACE
using NthDimension.Compute.Bindings;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace NthDimension.Compute
{
	/// <summary>
	/// Represents an OpenCL kernel.
	/// </summary>
	/// <remarks> A kernel object encapsulates a specific kernel function declared in a program and the argument values to be used when executing this kernel function. </remarks>
	/// <seealso cref="T:NthDimension.Compute.ComputeCommandQueue" />
	/// <seealso cref="T:NthDimension.Compute.ComputeProgram" />
	public class ComputeKernel : ComputeResource
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ComputeContext context;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string functionName;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ComputeProgram program;

		/// <summary>
		/// The handle of the <see cref="T:NthDimension.Compute.ComputeKernel" />.
		/// </summary>
		public CLKernelHandle Handle
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the <see cref="T:NthDimension.Compute.ComputeContext" /> associated with the <see cref="T:NthDimension.Compute.ComputeKernel" />.
		/// </summary>
		/// <value> The <see cref="T:NthDimension.Compute.ComputeContext" /> associated with the <see cref="T:NthDimension.Compute.ComputeKernel" />. </value>
		public ComputeContext Context
		{
			get
			{
				return context;
			}
		}

		/// <summary>
		/// Gets the function name of the <see cref="T:NthDimension.Compute.ComputeKernel" />.
		/// </summary>
		/// <value> The function name of the <see cref="T:NthDimension.Compute.ComputeKernel" />. </value>
		public string FunctionName
		{
			get
			{
				return functionName;
			}
		}

		/// <summary>
		/// Gets the <see cref="T:NthDimension.Compute.ComputeProgram" /> that the <see cref="T:NthDimension.Compute.ComputeKernel" /> belongs to.
		/// </summary>
		/// <value> The <see cref="T:NthDimension.Compute.ComputeProgram" /> that the <see cref="T:NthDimension.Compute.ComputeKernel" /> belongs to. </value>
		public ComputeProgram Program
		{
			get
			{
				return program;
			}
		}

		internal ComputeKernel(CLKernelHandle handle, ComputeProgram program)
		{
			Handle = handle;
			SetID(Handle.Value);
			context = program.Context;
			functionName = GetStringInfo(Handle, ComputeKernelInfo.FunctionName, CL10.GetKernelInfo);
			this.program = program;
			Trace.WriteLine("Create " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
		}

		internal ComputeKernel(string functionName, ComputeProgram program)
		{
			ComputeErrorCode errcode_ret = ComputeErrorCode.Success;
			Handle = CL10.CreateKernel(program.Handle, functionName, out errcode_ret);
			ComputeException.ThrowOnError(errcode_ret);
			SetID(Handle.Value);
			context = program.Context;
			this.functionName = functionName;
			this.program = program;
			Trace.WriteLine("Create " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
		}

		/// <summary>
		/// Gets the amount of local memory in bytes used by the <see cref="T:NthDimension.Compute.ComputeKernel" />.
		/// </summary>
		/// <param name="device"> One of the <see cref="!:ComputeKernel.Program.Device" />s. </param>
		/// <returns> The amount of local memory in bytes used by the <see cref="T:NthDimension.Compute.ComputeKernel" />. </returns>
		public long GetLocalMemorySize(ComputeDevice device)
		{
			return GetInfo<CLKernelHandle, CLDeviceHandle, ComputeKernelWorkGroupInfo, long>(Handle, device.Handle, ComputeKernelWorkGroupInfo.LocalMemorySize, CL10.GetKernelWorkGroupInfo);
		}

		/// <summary>
		/// Gets the compile work-group size specified by the <c>__attribute__((reqd_work_group_size(X, Y, Z)))</c> qualifier.
		/// </summary>
		/// <param name="device"> One of the <see cref="!:ComputeKernel.Program.Device" />s. </param>
		/// <returns> The compile work-group size specified by the <c>__attribute__((reqd_work_group_size(X, Y, Z)))</c> qualifier. If no such qualifier is specified, (0, 0, 0) is returned. </returns>
		public long[] GetCompileWorkGroupSize(ComputeDevice device)
		{
			return Tools.ConvertArray(GetArrayInfo<CLKernelHandle, CLDeviceHandle, ComputeKernelWorkGroupInfo, IntPtr>(Handle, device.Handle, ComputeKernelWorkGroupInfo.CompileWorkGroupSize, CL10.GetKernelWorkGroupInfo));
		}

		/// <summary>
		/// Gets the preferred multiple of workgroup size for launch. 
		/// </summary>
		/// <param name="device"> One of the <see cref="!:ComputeKernel.Program.Device" />s. </param>
		/// <returns> The preferred multiple of workgroup size for launch. </returns>
		/// <remarks> The returned value is a performance hint. Specifying a workgroup size that is not a multiple of the value returned by this query as the value of the local work size argument to ComputeCommandQueue.Execute will not fail to enqueue the kernel for execution unless the work-group size specified is larger than the device maximum. </remarks>
		/// <remarks> Requires OpenCL 1.1. </remarks>
		public long GetPreferredWorkGroupSizeMultiple(ComputeDevice device)
		{
			return (long)GetInfo<CLKernelHandle, CLDeviceHandle, ComputeKernelWorkGroupInfo, IntPtr>(Handle, device.Handle, ComputeKernelWorkGroupInfo.PreferredWorkGroupSizeMultiple, CL10.GetKernelWorkGroupInfo);
		}

		/// <summary>
		/// Gets the minimum amount of memory, in bytes, used by each work-item in the kernel.
		/// </summary>
		/// <param name="device"> One of the <see cref="!:ComputeKernel.Program.Device" />s. </param>
		/// <returns> The minimum amount of memory, in bytes, used by each work-item in the kernel. </returns>
		/// <remarks> The returned value may include any private memory needed by an implementation to execute the kernel, including that used by the language built-ins and variable declared inside the kernel with the <c>__private</c> or <c>private</c> qualifier. </remarks>
		public long GetPrivateMemorySize(ComputeDevice device)
		{
			return GetInfo<CLKernelHandle, CLDeviceHandle, ComputeKernelWorkGroupInfo, long>(Handle, device.Handle, ComputeKernelWorkGroupInfo.PrivateMemorySize, CL10.GetKernelWorkGroupInfo);
		}

		/// <summary>
		/// Gets the maximum work-group size that can be used to execute the <see cref="T:NthDimension.Compute.ComputeKernel" /> on a <see cref="T:NthDimension.Compute.ComputeDevice" />.
		/// </summary>
		/// <param name="device"> One of the <see cref="!:ComputeKernel.Program.Device" />s. </param>
		/// <returns> The maximum work-group size that can be used to execute the <see cref="T:NthDimension.Compute.ComputeKernel" /> on <paramref name="device" />. </returns>
		public long GetWorkGroupSize(ComputeDevice device)
		{
			return (long)GetInfo<CLKernelHandle, CLDeviceHandle, ComputeKernelWorkGroupInfo, IntPtr>(Handle, device.Handle, ComputeKernelWorkGroupInfo.WorkGroupSize, CL10.GetKernelWorkGroupInfo);
		}

		/// <summary>
		/// Sets an argument of the <see cref="T:NthDimension.Compute.ComputeKernel" /> (no argument tracking).
		/// </summary>
		/// <param name="index"> The argument index. </param>
		/// <param name="dataSize"> The size of the argument data in bytes. </param>
		/// <param name="dataAddr"> A pointer to the data that should be used as the argument value. </param>
		/// <remarks> 
		/// Arguments to the kernel are referred by indices that go from 0 for the leftmost argument to n-1, where n is the total number of arguments declared by the kernel.
		/// <br />
		/// Note that this method does not provide argument tracking. It is up to the user to reference the kernel arguments (i.e. prevent them from being garbage collected) until the kernel has finished execution.
		/// </remarks>
		public void SetArgument(int index, IntPtr dataSize, IntPtr dataAddr)
		{
			ComputeErrorCode errorCode = CL10.SetKernelArg(Handle, index, dataSize, dataAddr);
			ComputeException.ThrowOnError(errorCode);
		}

		/// <summary>
		/// Sets the size in bytes of an argument specfied with the <c>local</c> or <c>__local</c> address space qualifier.
		/// </summary>
		/// <param name="index"> The argument index. </param>
		/// <param name="dataSize"> The size of the argument data in bytes. </param>
		/// <remarks> Arguments to the kernel are referred by indices that go from 0 for the leftmost argument to n-1, where n is the total number of arguments declared by the kernel. </remarks>
		public void SetLocalArgument(int index, long dataSize)
		{
			SetArgument(index, new IntPtr(dataSize), IntPtr.Zero);
		}

		/// <summary>
		/// Sets a <c>T*</c>, <c>image2d_t</c> or <c>image3d_t</c> argument of the <see cref="T:NthDimension.Compute.ComputeKernel" />.
		/// </summary>
		/// <param name="index"> The argument index. </param>
		/// <param name="memObj"> The <see cref="T:NthDimension.Compute.ComputeMemory" /> that is passed as the argument. </param>
		/// <remarks> This method will automatically track <paramref name="memObj" /> to prevent it from being collected by the GC.<br /> Arguments to the kernel are referred by indices that go from 0 for the leftmost argument to n-1, where n is the total number of arguments declared by the kernel. </remarks>
		public void SetMemoryArgument(int index, ComputeMemory memObj)
		{
			SetValueArgument(index, memObj.Handle);
		}

		/// <summary>
		/// Sets a <c>sampler_t</c> argument of the <see cref="T:NthDimension.Compute.ComputeKernel" />.
		/// </summary>
		/// <param name="index"> The argument index. </param>
		/// <param name="sampler"> The <see cref="T:NthDimension.Compute.ComputeSampler" /> that is passed as the argument. </param>
		/// <remarks> This method will automatically track <paramref name="sampler" /> to prevent it from being collected by the GC.<br /> Arguments to the kernel are referred by indices that go from 0 for the leftmost argument to n-1, where n is the total number of arguments declared by the kernel. </remarks>
		public void SetSamplerArgument(int index, ComputeSampler sampler)
		{
			SetValueArgument(index, sampler.Handle);
		}

		/// <summary>
		/// Sets a value argument of the <see cref="T:NthDimension.Compute.ComputeKernel" />.
		/// </summary>
		/// <typeparam name="T"> The type of the argument. </typeparam>
		/// <param name="index"> The argument index. </param>
		/// <param name="data"> The data that is passed as the argument value. </param>
		/// <remarks> Arguments to the kernel are referred by indices that go from 0 for the leftmost argument to n-1, where n is the total number of arguments declared by the kernel. </remarks>
		public void SetValueArgument<T>(int index, T data) where T : struct
		{
			GCHandle gCHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
			try
			{
				SetArgument(index, new IntPtr(Marshal.SizeOf(typeof(T))), gCHandle.AddrOfPinnedObject());
			}
			finally
			{
				gCHandle.Free();
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
				CL10.ReleaseKernel(Handle);
				Handle.Invalidate();
			}
		}
	}
}
