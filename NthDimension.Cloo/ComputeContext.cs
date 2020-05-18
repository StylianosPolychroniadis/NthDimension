#define TRACE
using NthDimension.Compute.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;

namespace NthDimension.Compute
{
	/// <summary>
	/// Represents an OpenCL context.
	/// </summary>
	/// <remarks> The environment within which the kernels execute and the domain in which synchronization and memory management is defined. </remarks>
	/// <br />
	/// <example> 
	/// This example shows how to create a <see cref="T:NthDimension.Compute.ComputeContext" /> that is able to share data with an OpenGL context in a Microsoft Windows OS:
	/// <code>
	/// <![CDATA[
	///
	/// // NOTE: If you see some non C# bits surrounding this code section, ignore them. They're not part of the code.
	///
	/// // We will need the device context, which is obtained through an OS specific function.
	/// [DllImport("opengl32.dll")]
	/// extern static IntPtr wglGetCurrentDC();
	///
	/// // Query the device context.
	/// IntPtr deviceContextHandle = wglGetCurrentDC();
	///
	/// // Select a platform which is capable of OpenCL/OpenGL interop.
	/// ComputePlatform platform = ComputePlatform.GetByName(name);
	///
	/// // Create the context property list and populate it.
	/// ComputeContextProperty p1 = new ComputeContextProperty(ComputeContextPropertyName.Platform, platform.Handle.Value);
	/// ComputeContextProperty p2 = new ComputeContextProperty(ComputeContextPropertyName.CL_GL_CONTEXT_KHR, openGLContextHandle);
	/// ComputeContextProperty p3 = new ComputeContextProperty(ComputeContextPropertyName.CL_WGL_HDC_KHR, deviceContextHandle);
	/// ComputeContextPropertyList cpl = new ComputeContextPropertyList(new ComputeContextProperty[] { p1, p2, p3 });
	///
	/// // Create the context. Usually, you'll want this on a GPU but other options might be available as well.
	/// ComputeContext context = new ComputeContext(ComputeDeviceTypes.Gpu, cpl, null, IntPtr.Zero);
	///
	/// // Create a shared OpenCL/OpenGL buffer.
	/// // The generic type should match the type of data that the buffer contains.
	/// // glBufferId is an existing OpenGL buffer identifier.
	/// ComputeBuffer<float> clglBuffer = ComputeBuffer.CreateFromGLBuffer<float>(context, ComputeMemoryFlags.ReadWrite, glBufferId);
	///
	/// ]]>
	/// </code>
	/// Before working with the <c>clglBuffer</c> you should make sure of two things:<br />
	/// 1) OpenGL isn't using <c>glBufferId</c>. You can achieve this by calling <c>glFinish</c>.<br />
	/// 2) Make it available to OpenCL through the <see cref="T:NthDimension.Compute.ComputeCommandQueue.AcquireGLObjects(System.Collections.Generic.ICollection{Cloo.ComputeMemory},System.Collections.Generic.ICollection{Cloo.ComputeEventBase})" /> method.<br />
	/// When finished, you should wait until <c>clglBuffer</c> isn't used any longer by OpenCL. After that, call <see cref="T:NthDimension.Compute.ComputeCommandQueue.ReleaseGLObjects(System.Collections.Generic.ICollection{Cloo.ComputeMemory},System.Collections.Generic.ICollection{Cloo.ComputeEventBase})" /> to make the buffer available to OpenGL again.
	/// </example>
	/// <seealso cref="T:NthDimension.Compute.ComputeDevice" />
	/// <seealso cref="T:NthDimension.Compute.ComputePlatform" />
	public class ComputeContext : ComputeResource
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ReadOnlyCollection<ComputeDevice> devices;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ComputePlatform platform;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ComputeContextPropertyList properties;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ComputeContextNotifier callback;

		/// <summary>
		/// The handle of the <see cref="T:NthDimension.Compute.ComputeContext" />.
		/// </summary>
		public CLContextHandle Handle
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets a read-only collection of the <see cref="T:NthDimension.Compute.ComputeDevice" />s of the <see cref="T:NthDimension.Compute.ComputeContext" />.
		/// </summary>
		/// <value> A read-only collection of the <see cref="T:NthDimension.Compute.ComputeDevice" />s of the <see cref="T:NthDimension.Compute.ComputeContext" />. </value>
		public ReadOnlyCollection<ComputeDevice> Devices
		{
			get
			{
				return devices;
			}
		}

		/// <summary>
		/// Gets the <see cref="T:NthDimension.Compute.ComputePlatform" /> of the <see cref="T:NthDimension.Compute.ComputeContext" />.
		/// </summary>
		/// <value> The <see cref="T:NthDimension.Compute.ComputePlatform" /> of the <see cref="T:NthDimension.Compute.ComputeContext" />. </value>
		public ComputePlatform Platform
		{
			get
			{
				return platform;
			}
		}

		/// <summary>
		/// Gets a collection of <see cref="T:NthDimension.Compute.ComputeContextProperty" />s of the <see cref="T:NthDimension.Compute.ComputeContext" />.
		/// </summary>
		/// <value> A collection of <see cref="T:NthDimension.Compute.ComputeContextProperty" />s of the <see cref="T:NthDimension.Compute.ComputeContext" />. </value>
		public ComputeContextPropertyList Properties
		{
			get
			{
				return properties;
			}
		}

		/// <summary>
		/// Creates a new <see cref="T:NthDimension.Compute.ComputeContext" /> on a collection of <see cref="T:NthDimension.Compute.ComputeDevice" />s.
		/// </summary>
		/// <param name="devices"> A collection of <see cref="T:NthDimension.Compute.ComputeDevice" />s to associate with the <see cref="T:NthDimension.Compute.ComputeContext" />. </param>
		/// <param name="properties"> A <see cref="T:NthDimension.Compute.ComputeContextPropertyList" /> of the <see cref="T:NthDimension.Compute.ComputeContext" />. </param>
		/// <param name="notify"> A delegate instance that refers to a notification routine. This routine is a callback function that will be used by the OpenCL implementation to report information on errors that occur in the <see cref="T:NthDimension.Compute.ComputeContext" />. The callback function may be called asynchronously by the OpenCL implementation. It is the application's responsibility to ensure that the callback function is thread-safe and that the delegate instance doesn't get collected by the Garbage Collector until <see cref="T:NthDimension.Compute.ComputeContext" /> is disposed. If <paramref name="notify" /> is <c>null</c>, no callback function is registered. </param>
		/// <param name="notifyDataPtr"> Optional user data that will be passed to <paramref name="notify" />. </param>
		public ComputeContext(ICollection<ComputeDevice> devices, ComputeContextPropertyList properties, ComputeContextNotifier notify, IntPtr notifyDataPtr)
		{
			int handleCount;
			CLDeviceHandle[] array = Tools.ExtractHandles(devices, out handleCount);
			IntPtr[] array2 = (properties != null) ? properties.ToIntPtrArray() : null;
			callback = notify;
			ComputeErrorCode errcode_ret = ComputeErrorCode.Success;
			Handle = CL10.CreateContext(array2, handleCount, array, notify, notifyDataPtr, out errcode_ret);
			ComputeException.ThrowOnError(errcode_ret);
			SetID(Handle.Value);
			this.properties = properties;
			ComputeContextProperty byName = properties.GetByName(ComputeContextPropertyName.Platform);
			platform = ComputePlatform.GetByHandle(byName.Value);
			this.devices = GetDevices();
			Trace.WriteLine("Create " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
		}

		/// <summary>
		/// Creates a new <see cref="T:NthDimension.Compute.ComputeContext" /> on all the <see cref="T:NthDimension.Compute.ComputeDevice" />s that match the specified <see cref="T:NthDimension.Compute.ComputeDeviceTypes" />.
		/// </summary>
		/// <param name="deviceType"> A bit-field that identifies the type of <see cref="T:NthDimension.Compute.ComputeDevice" /> to associate with the <see cref="T:NthDimension.Compute.ComputeContext" />. </param>
		/// <param name="properties"> A <see cref="T:NthDimension.Compute.ComputeContextPropertyList" /> of the <see cref="T:NthDimension.Compute.ComputeContext" />. </param>
		/// <param name="notify"> A delegate instance that refers to a notification routine. This routine is a callback function that will be used by the OpenCL implementation to report information on errors that occur in the <see cref="T:NthDimension.Compute.ComputeContext" />. The callback function may be called asynchronously by the OpenCL implementation. It is the application's responsibility to ensure that the callback function is thread-safe and that the delegate instance doesn't get collected by the Garbage Collector until <see cref="T:NthDimension.Compute.ComputeContext" /> is disposed. If <paramref name="notify" /> is <c>null</c>, no callback function is registered. </param>
		/// <param name="userDataPtr"> Optional user data that will be passed to <paramref name="notify" />. </param>
		public ComputeContext(ComputeDeviceTypes deviceType, ComputeContextPropertyList properties, ComputeContextNotifier notify, IntPtr userDataPtr)
		{
			IntPtr[] array = (properties != null) ? properties.ToIntPtrArray() : null;
			callback = notify;
			ComputeErrorCode errcode_ret = ComputeErrorCode.Success;
			Handle = CL10.CreateContextFromType(array, deviceType, notify, userDataPtr, out errcode_ret);
			ComputeException.ThrowOnError(errcode_ret);
			SetID(Handle.Value);
			this.properties = properties;
			ComputeContextProperty byName = properties.GetByName(ComputeContextPropertyName.Platform);
			platform = ComputePlatform.GetByHandle(byName.Value);
			devices = GetDevices();
			Trace.WriteLine("Create " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
		}

		/// <summary>
		/// Releases the associated OpenCL object.
		/// </summary>
		/// <param name="manual"> Specifies the operation mode of this method. </param>
		/// <remarks> <paramref name="manual" /> must be <c>true</c> if this method is invoked directly by the application. </remarks>
		protected override void Dispose(bool manual)
		{
			if (manual)
			{
			}
			if (Handle.IsValid)
			{
				Trace.WriteLine("Dispose " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
				CL10.ReleaseContext(Handle);
				Handle.Invalidate();
			}
		}

		private ReadOnlyCollection<ComputeDevice> GetDevices()
		{
			List<CLDeviceHandle> list = new List<CLDeviceHandle>(GetArrayInfo<CLContextHandle, ComputeContextInfo, CLDeviceHandle>(Handle, ComputeContextInfo.Devices, CL10.GetContextInfo));
			List<ComputeDevice> list2 = new List<ComputeDevice>();
			foreach (ComputePlatform platform2 in ComputePlatform.Platforms)
			{
				foreach (ComputeDevice device in platform2.Devices)
				{
					if (list.Contains(device.Handle))
					{
						list2.Add(device);
					}
				}
			}
			return new ReadOnlyCollection<ComputeDevice>(list2);
		}
	}
}
