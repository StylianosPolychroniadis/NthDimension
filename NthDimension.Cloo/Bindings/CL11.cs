#define TRACE
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;

namespace NthDimension.Compute.Bindings
{
	/// <summary>
	/// Contains bindings to the OpenCL 1.1 functions.
	/// </summary>
	/// <remarks> See the OpenCL specification for documentation regarding these functions. </remarks>
	[SuppressUnmanagedCodeSecurity]
	public class CL11 : CL10
	{
		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clCreateSubBuffer")]
		public static extern CLMemoryHandle CreateSubBuffer(CLMemoryHandle buffer, ComputeMemoryFlags flags, ComputeBufferCreateType buffer_create_type, ref SysIntX2 buffer_create_info, out ComputeErrorCode errcode_ret);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clSetMemObjectDestructorCallback")]
		public static extern ComputeErrorCode SetMemObjectDestructorCallback(CLMemoryHandle memobj, ComputeMemoryDestructorNotifer pfn_notify, IntPtr user_data);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clCreateUserEvent")]
		public static extern CLEventHandle CreateUserEvent(CLContextHandle context, out ComputeErrorCode errcode_ret);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clSetUserEventStatus")]
		public static extern ComputeErrorCode SetUserEventStatus(CLEventHandle @event, int execution_status);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clSetEventCallback")]
		public static extern ComputeErrorCode SetEventCallback(CLEventHandle @event, int command_exec_callback_type, ComputeEventCallback pfn_notify, IntPtr user_data);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clEnqueueReadBufferRect")]
		public static extern ComputeErrorCode EnqueueReadBufferRect(CLCommandQueueHandle command_queue, CLMemoryHandle buffer, [MarshalAs(UnmanagedType.Bool)] bool blocking_read, ref SysIntX3 buffer_offset, ref SysIntX3 host_offset, ref SysIntX3 region, IntPtr buffer_row_pitch, IntPtr buffer_slice_pitch, IntPtr host_row_pitch, IntPtr host_slice_pitch, IntPtr ptr, int num_events_in_wait_list, [MarshalAs(UnmanagedType.LPArray)] CLEventHandle[] event_wait_list, [Out] [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)] CLEventHandle[] new_event);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clEnqueueWriteBufferRect")]
		public static extern ComputeErrorCode EnqueueWriteBufferRect(CLCommandQueueHandle command_queue, CLMemoryHandle buffer, [MarshalAs(UnmanagedType.Bool)] bool blocking_write, ref SysIntX3 buffer_offset, ref SysIntX3 host_offset, ref SysIntX3 region, IntPtr buffer_row_pitch, IntPtr buffer_slice_pitch, IntPtr host_row_pitch, IntPtr host_slice_pitch, IntPtr ptr, int num_events_in_wait_list, [MarshalAs(UnmanagedType.LPArray)] CLEventHandle[] event_wait_list, [Out] [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)] CLEventHandle[] new_event);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clEnqueueCopyBufferRect")]
		public static extern ComputeErrorCode EnqueueCopyBufferRect(CLCommandQueueHandle command_queue, CLMemoryHandle src_buffer, CLMemoryHandle dst_buffer, ref SysIntX3 src_origin, ref SysIntX3 dst_origin, ref SysIntX3 region, IntPtr src_row_pitch, IntPtr src_slice_pitch, IntPtr dst_row_pitch, IntPtr dst_slice_pitch, int num_events_in_wait_list, [MarshalAs(UnmanagedType.LPArray)] CLEventHandle[] event_wait_list, [Out] [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)] CLEventHandle[] new_event);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[Obsolete("This function has been deprecated in OpenCL 1.1.")]
		public new static ComputeErrorCode SetCommandQueueProperty(CLCommandQueueHandle command_queue, ComputeCommandQueueFlags properties, [MarshalAs(UnmanagedType.Bool)] bool enable, out ComputeCommandQueueFlags old_properties)
		{
			Trace.WriteLine("WARNING! clSetCommandQueueProperty has been deprecated in OpenCL 1.1.");
			return CL10.SetCommandQueueProperty(command_queue, properties, enable, out old_properties);
		}
	}
}
