#define TRACE
using NthDimension.Compute.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace NthDimension.Compute
{
	/// <summary>
	/// Represents an OpenCL program.
	/// </summary>
	/// <remarks> An OpenCL program consists of a set of kernels. Programs may also contain auxiliary functions called by the kernel functions and constant data. </remarks>
	/// <seealso cref="T:NthDimension.Compute.ComputeKernel" />
	public class ComputeProgram : ComputeResource
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ComputeContext context;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ReadOnlyCollection<ComputeDevice> devices;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ReadOnlyCollection<string> source;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ReadOnlyCollection<byte[]> binaries;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string buildOptions;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ComputeProgramBuildNotifier buildNotify;

		/// <summary>
		/// The handle of the <see cref="T:NthDimension.Compute.ComputeProgram" />.
		/// </summary>
		public CLProgramHandle Handle
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets a read-only collection of program binaries associated with the <see cref="P:Cloo.ComputeProgram.Devices" />.
		/// </summary>
		/// <value> A read-only collection of program binaries associated with the <see cref="P:Cloo.ComputeProgram.Devices" />. </value>
		/// <remarks> The bits returned can be an implementation-specific intermediate representation (a.k.a. IR) or device specific executable bits or both. The decision on which information is returned in the binary is up to the OpenCL implementation. </remarks>
		public ReadOnlyCollection<byte[]> Binaries
		{
			get
			{
				if (binaries == null)
				{
					binaries = GetBinaries();
				}
				return binaries;
			}
		}

		/// <summary>
		/// Gets the <see cref="T:NthDimension.Compute.ComputeProgram" /> build options as specified in <paramref name="options" /> argument of <see cref="T:NthDimension.Compute.ComputeProgram.Build(System.Collections.Generic.ICollection{Cloo.ComputeDevice},System.String,Cloo.Bindings.ComputeProgramBuildNotifier,System.IntPtr)" />.
		/// </summary>
		/// <value> The <see cref="T:NthDimension.Compute.ComputeProgram" /> build options as specified in <paramref name="options" /> argument of <see cref="T:NthDimension.Compute.ComputeProgram.Build(System.Collections.Generic.ICollection{Cloo.ComputeDevice},System.String,Cloo.Bindings.ComputeProgramBuildNotifier,System.IntPtr)" />. </value>
		public string BuildOptions
		{
			get
			{
				return buildOptions;
			}
		}

		/// <summary>
		/// Gets the <see cref="T:NthDimension.Compute.ComputeContext" /> of the <see cref="T:NthDimension.Compute.ComputeProgram" />.
		/// </summary>
		/// <value> The <see cref="T:NthDimension.Compute.ComputeContext" /> of the <see cref="T:NthDimension.Compute.ComputeProgram" />. </value>
		public ComputeContext Context
		{
			get
			{
				return context;
			}
		}

		/// <summary>
		/// Gets a read-only collection of <see cref="T:NthDimension.Compute.ComputeDevice" />s associated with the <see cref="T:NthDimension.Compute.ComputeProgram" />.
		/// </summary>
		/// <value> A read-only collection of <see cref="T:NthDimension.Compute.ComputeDevice" />s associated with the <see cref="T:NthDimension.Compute.ComputeProgram" />. </value>
		/// <remarks> This collection is a subset of <see cref="!:ComputeProgram.Context.Devices" />. </remarks>
		public ReadOnlyCollection<ComputeDevice> Devices
		{
			get
			{
				return devices;
			}
		}

		/// <summary>
		/// Gets a read-only collection of program source code strings specified when creating the <see cref="T:NthDimension.Compute.ComputeProgram" /> or <c>null</c> if <see cref="T:NthDimension.Compute.ComputeProgram" /> was created using program binaries.
		/// </summary>
		/// <value> A read-only collection of program source code strings specified when creating the <see cref="T:NthDimension.Compute.ComputeProgram" /> or <c>null</c> if <see cref="T:NthDimension.Compute.ComputeProgram" /> was created using program binaries. </value>
		public ReadOnlyCollection<string> Source
		{
			get
			{
				return source;
			}
		}

		/// <summary>
		/// Creates a new <see cref="T:NthDimension.Compute.ComputeProgram" /> from a source code string.
		/// </summary>
		/// <param name="context"> A <see cref="T:NthDimension.Compute.ComputeContext" />. </param>
		/// <param name="source"> The source code for the <see cref="T:NthDimension.Compute.ComputeProgram" />. </param>
		/// <remarks> The created <see cref="T:NthDimension.Compute.ComputeProgram" /> is associated with the <see cref="P:Cloo.ComputeContext.Devices" />. </remarks>
		public ComputeProgram(ComputeContext context, string source)
		{
			ComputeErrorCode errcode_ret = ComputeErrorCode.Success;
			Handle = CL10.CreateProgramWithSource(context.Handle, 1, new string[1]
			{
				source
			}, null, out errcode_ret);
			ComputeException.ThrowOnError(errcode_ret);
			SetID(Handle.Value);
			this.context = context;
			devices = context.Devices;
			this.source = new ReadOnlyCollection<string>(new string[1]
			{
				source
			});
			Trace.WriteLine("Create " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
		}

		/// <summary>
		/// Creates a new <see cref="T:NthDimension.Compute.ComputeProgram" /> from an array of source code strings.
		/// </summary>
		/// <param name="context"> A <see cref="T:NthDimension.Compute.ComputeContext" />. </param>
		/// <param name="source"> The source code lines for the <see cref="T:NthDimension.Compute.ComputeProgram" />. </param>
		/// <remarks> The created <see cref="T:NthDimension.Compute.ComputeProgram" /> is associated with the <see cref="P:Cloo.ComputeContext.Devices" />. </remarks>
		public ComputeProgram(ComputeContext context, string[] source)
		{
			ComputeErrorCode errcode_ret = ComputeErrorCode.Success;
			Handle = CL10.CreateProgramWithSource(context.Handle, source.Length, source, null, out errcode_ret);
			ComputeException.ThrowOnError(errcode_ret);
			this.context = context;
			devices = context.Devices;
			this.source = new ReadOnlyCollection<string>(source);
			Trace.WriteLine("Create " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
		}

		/// <summary>
		/// Creates a new <see cref="T:NthDimension.Compute.ComputeProgram" /> from a specified list of binaries.
		/// </summary>
		/// <param name="context"> A <see cref="T:NthDimension.Compute.ComputeContext" />. </param>
		/// <param name="binaries"> A list of binaries, one for each item in <paramref name="devices" />. </param>
		/// <param name="devices"> A subset of the <see cref="P:Cloo.ComputeContext.Devices" />. If <paramref name="devices" /> is <c>null</c>, OpenCL will associate every binary from <see cref="P:Cloo.ComputeProgram.Binaries" /> with a corresponding <see cref="T:NthDimension.Compute.ComputeDevice" /> from <see cref="P:Cloo.ComputeContext.Devices" />. </param>
		public ComputeProgram(ComputeContext context, IList<byte[]> binaries, IList<ComputeDevice> devices)
		{
			int handleCount;
			CLDeviceHandle[] device_list = (devices != null) ? Tools.ExtractHandles(devices, out handleCount) : Tools.ExtractHandles(context.Devices, out handleCount);
			IntPtr[] array = new IntPtr[handleCount];
			IntPtr[] array2 = new IntPtr[handleCount];
			int[] binary_status = new int[handleCount];
			ComputeErrorCode errcode_ret = ComputeErrorCode.Success;
			GCHandle[] array3 = new GCHandle[handleCount];
			try
			{
				for (int i = 0; i < handleCount; i++)
				{
					array3[i] = GCHandle.Alloc(binaries[i], GCHandleType.Pinned);
					array[i] = array3[i].AddrOfPinnedObject();
					array2[i] = new IntPtr(binaries[i].Length);
				}
				Handle = CL10.CreateProgramWithBinary(context.Handle, handleCount, device_list, array2, array, binary_status, out errcode_ret);
				ComputeException.ThrowOnError(errcode_ret);
			}
			finally
			{
				for (int i = 0; i < handleCount; i++)
				{
					array3[i].Free();
				}
			}
			this.binaries = new ReadOnlyCollection<byte[]>(binaries);
			this.context = context;
			this.devices = new ReadOnlyCollection<ComputeDevice>((devices != null) ? devices : context.Devices);
			Trace.WriteLine("Create " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
		}

		/// <summary>
		/// Builds (compiles and links) a program executable from the program source or binary for all or some of the <see cref="P:Cloo.ComputeProgram.Devices" />.
		/// </summary>
		/// <param name="devices"> A subset or all of <see cref="P:Cloo.ComputeProgram.Devices" />. If <paramref name="devices" /> is <c>null</c>, the executable is built for every item of <see cref="P:Cloo.ComputeProgram.Devices" /> for which a source or a binary has been loaded. </param>
		/// <param name="options"> A set of options for the OpenCL compiler. </param>
		/// <param name="notify"> A delegate instance that represents a reference to a notification routine. This routine is a callback function that an application can register and which will be called when the program executable has been built (successfully or unsuccessfully). If <paramref name="notify" /> is not <c>null</c>, <see cref="T:NthDimension.Compute.ComputeProgram.Build(System.Collections.Generic.ICollection{Cloo.ComputeDevice},System.String,Cloo.Bindings.ComputeProgramBuildNotifier,System.IntPtr)" /> does not need to wait for the build to complete and can return immediately. If <paramref name="notify" /> is <c>null</c>, <see cref="T:NthDimension.Compute.ComputeProgram.Build(System.Collections.Generic.ICollection{Cloo.ComputeDevice},System.String,Cloo.Bindings.ComputeProgramBuildNotifier,System.IntPtr)" /> does not return until the build has completed. The callback function may be called asynchronously by the OpenCL implementation. It is the application's responsibility to ensure that the callback function is thread-safe and that the delegate instance doesn't get collected by the Garbage Collector until the build operation triggers the callback. </param>
		/// <param name="notifyDataPtr"> Optional user data that will be passed to <paramref name="notify" />. </param>
		public void Build(ICollection<ComputeDevice> devices, string options, ComputeProgramBuildNotifier notify, IntPtr notifyDataPtr)
		{
			int handleCount;
			CLDeviceHandle[] device_list = Tools.ExtractHandles(devices, out handleCount);
			buildOptions = ((options != null) ? options : "");
			buildNotify = notify;
			ComputeErrorCode errorCode = CL10.BuildProgram(Handle, handleCount, device_list, options, buildNotify, notifyDataPtr);
			ComputeException.ThrowOnError(errorCode);
		}

		/// <summary>
		/// Creates a <see cref="T:NthDimension.Compute.ComputeKernel" /> for every <c>kernel</c> function in <see cref="T:NthDimension.Compute.ComputeProgram" />.
		/// </summary>
		/// <returns> The collection of created <see cref="T:NthDimension.Compute.ComputeKernel" />s. </returns>
		/// <remarks> <see cref="T:NthDimension.Compute.ComputeKernel" />s are not created for any <c>kernel</c> functions in <see cref="T:NthDimension.Compute.ComputeProgram" /> that do not have the same function definition across all <see cref="P:Cloo.ComputeProgram.Devices" /> for which a program executable has been successfully built. </remarks>
		public ICollection<ComputeKernel> CreateAllKernels()
		{
			ICollection<ComputeKernel> collection = new Collection<ComputeKernel>();
			int num_kernels_ret = 0;
			ComputeErrorCode errorCode = CL10.CreateKernelsInProgram(Handle, 0, null, out num_kernels_ret);
			ComputeException.ThrowOnError(errorCode);
			CLKernelHandle[] array = new CLKernelHandle[num_kernels_ret];
			errorCode = CL10.CreateKernelsInProgram(Handle, num_kernels_ret, array, out num_kernels_ret);
			ComputeException.ThrowOnError(errorCode);
			for (int i = 0; i < num_kernels_ret; i++)
			{
				collection.Add(new ComputeKernel(array[i], this));
			}
			return collection;
		}

		/// <summary>
		/// Creates a <see cref="T:NthDimension.Compute.ComputeKernel" /> for a kernel function of a specified name.
		/// </summary>
		/// <returns> The created <see cref="T:NthDimension.Compute.ComputeKernel" />. </returns>
		public ComputeKernel CreateKernel(string functionName)
		{
			return new ComputeKernel(functionName, this);
		}

		/// <summary>
		/// Gets the build log of the <see cref="T:NthDimension.Compute.ComputeProgram" /> for a specified <see cref="T:NthDimension.Compute.ComputeDevice" />.
		/// </summary>
		/// <param name="device"> The <see cref="T:NthDimension.Compute.ComputeDevice" /> building the <see cref="T:NthDimension.Compute.ComputeProgram" />. Must be one of <see cref="P:Cloo.ComputeProgram.Devices" />. </param>
		/// <returns> The build log of the <see cref="T:NthDimension.Compute.ComputeProgram" /> for <paramref name="device" />. </returns>
		public string GetBuildLog(ComputeDevice device)
		{
			return GetStringInfo(Handle, device.Handle, ComputeProgramBuildInfo.BuildLog, CL10.GetProgramBuildInfo);
		}

		/// <summary>
		/// Gets the <see cref="T:NthDimension.Compute.ComputeProgramBuildStatus" /> of the <see cref="T:NthDimension.Compute.ComputeProgram" /> for a specified <see cref="T:NthDimension.Compute.ComputeDevice" />.
		/// </summary>
		/// <param name="device"> The <see cref="T:NthDimension.Compute.ComputeDevice" /> building the <see cref="T:NthDimension.Compute.ComputeProgram" />. Must be one of <see cref="P:Cloo.ComputeProgram.Devices" />. </param>
		/// <returns> The <see cref="T:NthDimension.Compute.ComputeProgramBuildStatus" /> of the <see cref="T:NthDimension.Compute.ComputeProgram" /> for <paramref name="device" />. </returns>
		public ComputeProgramBuildStatus GetBuildStatus(ComputeDevice device)
		{
			return (ComputeProgramBuildStatus)GetInfo<CLProgramHandle, CLDeviceHandle, ComputeProgramBuildInfo, uint>(Handle, device.Handle, ComputeProgramBuildInfo.Status, CL10.GetProgramBuildInfo);
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
				CL10.ReleaseProgram(Handle);
				Handle.Invalidate();
			}
		}

		private ReadOnlyCollection<byte[]> GetBinaries()
		{
			IntPtr[] arrayInfo = GetArrayInfo<CLProgramHandle, ComputeProgramInfo, IntPtr>(Handle, ComputeProgramInfo.BinarySizes, CL10.GetProgramInfo);
			GCHandle[] array = new GCHandle[arrayInfo.Length];
			IntPtr[] array2 = new IntPtr[arrayInfo.Length];
			IList<byte[]> list = new List<byte[]>();
			GCHandle gCHandle = GCHandle.Alloc(array2, GCHandleType.Pinned);
			try
			{
				for (int i = 0; i < arrayInfo.Length; i++)
				{
					byte[] array3 = new byte[arrayInfo[i].ToInt64()];
					array[i] = GCHandle.Alloc(array3, GCHandleType.Pinned);
					array2[i] = array[i].AddrOfPinnedObject();
					list.Add(array3);
				}
				IntPtr param_value_size_ret;
				ComputeErrorCode programInfo = CL10.GetProgramInfo(Handle, ComputeProgramInfo.Binaries, new IntPtr(array2.Length * IntPtr.Size), gCHandle.AddrOfPinnedObject(), out param_value_size_ret);
				ComputeException.ThrowOnError(programInfo);
			}
			finally
			{
				for (int i = 0; i < arrayInfo.Length; i++)
				{
					array[i].Free();
				}
				gCHandle.Free();
			}
			return new ReadOnlyCollection<byte[]>(list);
		}
	}
}
