using System;
using System.Runtime.InteropServices;

namespace NthDimension.Compute.Bindings
{
	/// <summary>
	/// Contains bindings to the OpenCL extension functions.
	/// </summary>
	/// <remarks> See the Khronos OpenCL API extensions registry for documentation regarding these functions. </remarks>
	public class CLx
	{
		internal static class Delegates
		{
			internal delegate ComputeErrorCode clCreateSubDevicesEXT(IntPtr in_device, [MarshalAs(UnmanagedType.LPArray)] cl_device_partition_property_ext[] properties, int num_entries, [MarshalAs(UnmanagedType.LPArray)] IntPtr[] out_devices, out int num_devices);

			internal delegate ComputeErrorCode clEnqueueMigrateMemObjectEXT(IntPtr command_queue, int num_mem_objects, [MarshalAs(UnmanagedType.LPArray)] IntPtr[] mem_objects, cl_mem_migration_flags_ext flags, int num_events_in_wait_list, [MarshalAs(UnmanagedType.LPArray)] IntPtr[] event_wait_list, [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)] IntPtr[] new_event);

			internal delegate ComputeErrorCode clGetGLContextInfoKHR([MarshalAs(UnmanagedType.LPArray)] IntPtr[] properties, ComputeGLContextInfo param_name, IntPtr param_value_size, IntPtr param_value, out IntPtr param_value_size_ret);

			internal delegate ComputeErrorCode clIcdGetPlatformIDsKHR(int num_entries, [MarshalAs(UnmanagedType.LPArray)] IntPtr[] platforms, out int num_platforms);

			internal delegate ComputeErrorCode clReleaseDeviceEXT(IntPtr device);

			internal delegate ComputeErrorCode clRetainDeviceEXT(IntPtr device);
		}

		private readonly Delegates.clCreateSubDevicesEXT clCreateSubDevicesEXT = null;

		private readonly Delegates.clEnqueueMigrateMemObjectEXT clEnqueueMigrateMemObjectEXT = null;

		private readonly Delegates.clGetGLContextInfoKHR clGetGLContextInfoKHR = null;

		private readonly Delegates.clReleaseDeviceEXT clReleaseDeviceEXT = null;

		private readonly Delegates.clRetainDeviceEXT clRetainDeviceEXT = null;

		/// <summary> </summary>
		public ComputeErrorCode CreateSubDevicesEXT(IntPtr in_device, cl_device_partition_property_ext[] properties, int num_entries, IntPtr[] out_devices, out int num_devices)
		{
			if (clCreateSubDevicesEXT == null)
			{
				throw new EntryPointNotFoundException();
			}
			return clCreateSubDevicesEXT(in_device, properties, num_entries, out_devices, out num_devices);
		}

		/// <summary> </summary>
		public ComputeErrorCode EnqueueMigrateMemObjectEXT(IntPtr command_queue, int num_mem_objects, IntPtr[] mem_objects, cl_mem_migration_flags_ext flags, int num_events_in_wait_list, IntPtr[] event_wait_list, IntPtr[] new_event)
		{
			if (clEnqueueMigrateMemObjectEXT == null)
			{
				throw new EntryPointNotFoundException();
			}
			return clEnqueueMigrateMemObjectEXT(command_queue, num_mem_objects, mem_objects, flags, num_events_in_wait_list, event_wait_list, new_event);
		}

		/// <summary> </summary>
		public ComputeErrorCode GetGLContextInfoKHR(IntPtr[] properties, ComputeGLContextInfo param_name, IntPtr param_value_size, IntPtr param_value, out IntPtr param_value_size_ret)
		{
			if (clGetGLContextInfoKHR == null)
			{
				throw new EntryPointNotFoundException();
			}
			return clGetGLContextInfoKHR(properties, param_name, param_value_size, param_value, out param_value_size_ret);
		}

		/// <summary> </summary>
		public ComputeErrorCode ReleaseDeviceEXT(IntPtr device)
		{
			if (clReleaseDeviceEXT == null)
			{
				throw new EntryPointNotFoundException();
			}
			return clReleaseDeviceEXT(device);
		}

		/// <summary> </summary>
		public ComputeErrorCode RetainDeviceEXT(IntPtr device)
		{
			if (clRetainDeviceEXT == null)
			{
				throw new EntryPointNotFoundException();
			}
			return clRetainDeviceEXT(device);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="platform"></param>
		public CLx(ComputePlatform platform)
		{
			if (platform.Extensions.Contains("cl_ext_device_fission"))
			{
				clCreateSubDevicesEXT = (Delegates.clCreateSubDevicesEXT)Marshal.GetDelegateForFunctionPointer(CL10.GetExtensionFunctionAddress("clCreateSubDevicesEXT"), typeof(Delegates.clCreateSubDevicesEXT));
				clReleaseDeviceEXT = (Delegates.clReleaseDeviceEXT)Marshal.GetDelegateForFunctionPointer(CL10.GetExtensionFunctionAddress("clReleaseDeviceEXT"), typeof(Delegates.clReleaseDeviceEXT));
				clRetainDeviceEXT = (Delegates.clRetainDeviceEXT)Marshal.GetDelegateForFunctionPointer(CL10.GetExtensionFunctionAddress("clRetainDeviceEXT"), typeof(Delegates.clRetainDeviceEXT));
			}
			if (platform.Extensions.Contains("cl_ext_migrate_memobject"))
			{
				clEnqueueMigrateMemObjectEXT = (Delegates.clEnqueueMigrateMemObjectEXT)Marshal.GetDelegateForFunctionPointer(CL10.GetExtensionFunctionAddress("clEnqueueMigrateMemObjectEXT"), typeof(Delegates.clEnqueueMigrateMemObjectEXT));
			}
			if (platform.Extensions.Contains("cl_khr_gl_sharing"))
			{
				clGetGLContextInfoKHR = (Delegates.clGetGLContextInfoKHR)Marshal.GetDelegateForFunctionPointer(CL10.GetExtensionFunctionAddress("clGetGLContextInfoKHR"), typeof(Delegates.clGetGLContextInfoKHR));
			}
		}
	}
}
