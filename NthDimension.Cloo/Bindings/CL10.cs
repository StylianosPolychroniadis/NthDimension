using System;
using System.Runtime.InteropServices;
using System.Security;

namespace NthDimension.Compute.Bindings
{
	/// <summary>
	/// Contains bindings to the OpenCL 1.0 functions.
	/// </summary>
	/// <remarks> See the OpenCL specification for documentation regarding these functions. </remarks>
	[SuppressUnmanagedCodeSecurity]
	public class CL10
	{
		/// <summary>
		/// The name of the library that contains the available OpenCL function points.
		/// </summary>
		protected const string libName = "OpenCL.dll";

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clGetPlatformIDs")]
		public static extern ComputeErrorCode GetPlatformIDs(int num_entries, [Out] [MarshalAs(UnmanagedType.LPArray)] CLPlatformHandle[] platforms, out int num_platforms);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clGetPlatformInfo")]
		public static extern ComputeErrorCode GetPlatformInfo(CLPlatformHandle platform, ComputePlatformInfo param_name, IntPtr param_value_size, IntPtr param_value, out IntPtr param_value_size_ret);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clGetDeviceIDs")]
		public static extern ComputeErrorCode GetDeviceIDs(CLPlatformHandle platform, ComputeDeviceTypes device_type, int num_entries, [Out] [MarshalAs(UnmanagedType.LPArray)] CLDeviceHandle[] devices, out int num_devices);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clGetDeviceInfo")]
		public static extern ComputeErrorCode GetDeviceInfo(CLDeviceHandle device, ComputeDeviceInfo param_name, IntPtr param_value_size, IntPtr param_value, out IntPtr param_value_size_ret);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clCreateContext")]
		public static extern CLContextHandle CreateContext([MarshalAs(UnmanagedType.LPArray)] IntPtr[] properties, int num_devices, [MarshalAs(UnmanagedType.LPArray)] CLDeviceHandle[] devices, ComputeContextNotifier pfn_notify, IntPtr user_data, out ComputeErrorCode errcode_ret);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clCreateContextFromType")]
		public static extern CLContextHandle CreateContextFromType([MarshalAs(UnmanagedType.LPArray)] IntPtr[] properties, ComputeDeviceTypes device_type, ComputeContextNotifier pfn_notify, IntPtr user_data, out ComputeErrorCode errcode_ret);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clRetainContext")]
		public static extern ComputeErrorCode RetainContext(CLContextHandle context);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clReleaseContext")]
		public static extern ComputeErrorCode ReleaseContext(CLContextHandle context);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clGetContextInfo")]
		public static extern ComputeErrorCode GetContextInfo(CLContextHandle context, ComputeContextInfo param_name, IntPtr param_value_size, IntPtr param_value, out IntPtr param_value_size_ret);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clCreateCommandQueue")]
		public static extern CLCommandQueueHandle CreateCommandQueue(CLContextHandle context, CLDeviceHandle device, ComputeCommandQueueFlags properties, out ComputeErrorCode errcode_ret);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clRetainCommandQueue")]
		public static extern ComputeErrorCode RetainCommandQueue(CLCommandQueueHandle command_queue);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clReleaseCommandQueue")]
		public static extern ComputeErrorCode ReleaseCommandQueue(CLCommandQueueHandle command_queue);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clGetCommandQueueInfo")]
		public static extern ComputeErrorCode GetCommandQueueInfo(CLCommandQueueHandle command_queue, ComputeCommandQueueInfo param_name, IntPtr param_value_size, IntPtr param_value, out IntPtr param_value_size_ret);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clSetCommandQueueProperty")]
		public static extern ComputeErrorCode SetCommandQueueProperty(CLCommandQueueHandle command_queue, ComputeCommandQueueFlags properties, [MarshalAs(UnmanagedType.Bool)] bool enable, out ComputeCommandQueueFlags old_properties);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clCreateBuffer")]
		public static extern CLMemoryHandle CreateBuffer(CLContextHandle context, ComputeMemoryFlags flags, IntPtr size, IntPtr host_ptr, out ComputeErrorCode errcode_ret);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clCreateImage2D")]
		public static extern CLMemoryHandle CreateImage2D(CLContextHandle context, ComputeMemoryFlags flags, ref ComputeImageFormat image_format, IntPtr image_width, IntPtr image_height, IntPtr image_row_pitch, IntPtr host_ptr, out ComputeErrorCode errcode_ret);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clCreateImage3D")]
		public static extern CLMemoryHandle CreateImage3D(CLContextHandle context, ComputeMemoryFlags flags, ref ComputeImageFormat image_format, IntPtr image_width, IntPtr image_height, IntPtr image_depth, IntPtr image_row_pitch, IntPtr image_slice_pitch, IntPtr host_ptr, out ComputeErrorCode errcode_ret);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clRetainMemObject")]
		public static extern ComputeErrorCode RetainMemObject(CLMemoryHandle memobj);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clReleaseMemObject")]
		public static extern ComputeErrorCode ReleaseMemObject(CLMemoryHandle memobj);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clGetSupportedImageFormats")]
		public static extern ComputeErrorCode GetSupportedImageFormats(CLContextHandle context, ComputeMemoryFlags flags, ComputeMemoryType image_type, int num_entries, [Out] [MarshalAs(UnmanagedType.LPArray)] ComputeImageFormat[] image_formats, out int num_image_formats);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clGetMemObjectInfo")]
		public static extern ComputeErrorCode GetMemObjectInfo(CLMemoryHandle memobj, ComputeMemoryInfo param_name, IntPtr param_value_size, IntPtr param_value, out IntPtr param_value_size_ret);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clGetImageInfo")]
		public static extern ComputeErrorCode GetImageInfo(CLMemoryHandle image, ComputeImageInfo param_name, IntPtr param_value_size, IntPtr param_value, out IntPtr param_value_size_ret);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clCreateSampler")]
		public static extern CLSamplerHandle CreateSampler(CLContextHandle context, [MarshalAs(UnmanagedType.Bool)] bool normalized_coords, ComputeImageAddressing addressing_mode, ComputeImageFiltering filter_mode, out ComputeErrorCode errcode_ret);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clRetainSampler")]
		public static extern ComputeErrorCode RetainSampler(CLSamplerHandle sample);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clReleaseSampler")]
		public static extern ComputeErrorCode ReleaseSampler(CLSamplerHandle sample);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clGetSamplerInfo")]
		public static extern ComputeErrorCode GetSamplerInfo(CLSamplerHandle sample, ComputeSamplerInfo param_name, IntPtr param_value_size, IntPtr param_value, out IntPtr param_value_size_ret);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clCreateProgramWithSource")]
		public static extern CLProgramHandle CreateProgramWithSource(CLContextHandle context, int count, string[] strings, [MarshalAs(UnmanagedType.LPArray)] IntPtr[] lengths, out ComputeErrorCode errcode_ret);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clCreateProgramWithBinary")]
		public static extern CLProgramHandle CreateProgramWithBinary(CLContextHandle context, int num_devices, [MarshalAs(UnmanagedType.LPArray)] CLDeviceHandle[] device_list, [MarshalAs(UnmanagedType.LPArray)] IntPtr[] lengths, [MarshalAs(UnmanagedType.LPArray)] IntPtr[] binaries, [MarshalAs(UnmanagedType.LPArray)] int[] binary_status, out ComputeErrorCode errcode_ret);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clRetainProgram")]
		public static extern ComputeErrorCode RetainProgram(CLProgramHandle program);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clReleaseProgram")]
		public static extern ComputeErrorCode ReleaseProgram(CLProgramHandle program);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clBuildProgram")]
		public static extern ComputeErrorCode BuildProgram(CLProgramHandle program, int num_devices, [MarshalAs(UnmanagedType.LPArray)] CLDeviceHandle[] device_list, string options, ComputeProgramBuildNotifier pfn_notify, IntPtr user_data);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clUnloadCompiler")]
		public static extern ComputeErrorCode UnloadCompiler();

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clGetProgramInfo")]
		public static extern ComputeErrorCode GetProgramInfo(CLProgramHandle program, ComputeProgramInfo param_name, IntPtr param_value_size, IntPtr param_value, out IntPtr param_value_size_ret);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clGetProgramBuildInfo")]
		public static extern ComputeErrorCode GetProgramBuildInfo(CLProgramHandle program, CLDeviceHandle device, ComputeProgramBuildInfo param_name, IntPtr param_value_size, IntPtr param_value, out IntPtr param_value_size_ret);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clCreateKernel")]
		public static extern CLKernelHandle CreateKernel(CLProgramHandle program, string kernel_name, out ComputeErrorCode errcode_ret);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clCreateKernelsInProgram")]
		public static extern ComputeErrorCode CreateKernelsInProgram(CLProgramHandle program, int num_kernels, [Out] [MarshalAs(UnmanagedType.LPArray)] CLKernelHandle[] kernels, out int num_kernels_ret);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clRetainKernel")]
		public static extern ComputeErrorCode RetainKernel(CLKernelHandle kernel);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clReleaseKernel")]
		public static extern ComputeErrorCode ReleaseKernel(CLKernelHandle kernel);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clSetKernelArg")]
		public static extern ComputeErrorCode SetKernelArg(CLKernelHandle kernel, int arg_index, IntPtr arg_size, IntPtr arg_value);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clGetKernelInfo")]
		public static extern ComputeErrorCode GetKernelInfo(CLKernelHandle kernel, ComputeKernelInfo param_name, IntPtr param_value_size, IntPtr param_value, out IntPtr param_value_size_ret);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clGetKernelWorkGroupInfo")]
		public static extern ComputeErrorCode GetKernelWorkGroupInfo(CLKernelHandle kernel, CLDeviceHandle device, ComputeKernelWorkGroupInfo param_name, IntPtr param_value_size, IntPtr param_value, out IntPtr param_value_size_ret);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clWaitForEvents")]
		public static extern ComputeErrorCode WaitForEvents(int num_events, [MarshalAs(UnmanagedType.LPArray)] CLEventHandle[] event_list);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clGetEventInfo")]
		public static extern ComputeErrorCode GetEventInfo(CLEventHandle @event, ComputeEventInfo param_name, IntPtr param_value_size, IntPtr param_value, out IntPtr param_value_size_ret);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clRetainEvent")]
		public static extern ComputeErrorCode RetainEvent(CLEventHandle @event);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clReleaseEvent")]
		public static extern ComputeErrorCode ReleaseEvent(CLEventHandle @event);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clGetEventProfilingInfo")]
		public static extern ComputeErrorCode GetEventProfilingInfo(CLEventHandle @event, ComputeCommandProfilingInfo param_name, IntPtr param_value_size, IntPtr param_value, out IntPtr param_value_size_ret);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clFlush")]
		public static extern ComputeErrorCode Flush(CLCommandQueueHandle command_queue);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clFinish")]
		public static extern ComputeErrorCode Finish(CLCommandQueueHandle command_queue);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clEnqueueReadBuffer")]
		public static extern ComputeErrorCode EnqueueReadBuffer(CLCommandQueueHandle command_queue, CLMemoryHandle buffer, [MarshalAs(UnmanagedType.Bool)] bool blocking_read, IntPtr offset, IntPtr cb, IntPtr ptr, int num_events_in_wait_list, [MarshalAs(UnmanagedType.LPArray)] CLEventHandle[] event_wait_list, [Out] [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)] CLEventHandle[] new_event);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clEnqueueWriteBuffer")]
		public static extern ComputeErrorCode EnqueueWriteBuffer(CLCommandQueueHandle command_queue, CLMemoryHandle buffer, [MarshalAs(UnmanagedType.Bool)] bool blocking_write, IntPtr offset, IntPtr cb, IntPtr ptr, int num_events_in_wait_list, [MarshalAs(UnmanagedType.LPArray)] CLEventHandle[] event_wait_list, [Out] [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)] CLEventHandle[] new_event);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clEnqueueCopyBuffer")]
		public static extern ComputeErrorCode EnqueueCopyBuffer(CLCommandQueueHandle command_queue, CLMemoryHandle src_buffer, CLMemoryHandle dst_buffer, IntPtr src_offset, IntPtr dst_offset, IntPtr cb, int num_events_in_wait_list, [MarshalAs(UnmanagedType.LPArray)] CLEventHandle[] event_wait_list, [Out] [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)] CLEventHandle[] new_event);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clEnqueueReadImage")]
		public static extern ComputeErrorCode EnqueueReadImage(CLCommandQueueHandle command_queue, CLMemoryHandle image, [MarshalAs(UnmanagedType.Bool)] bool blocking_read, ref SysIntX3 origin, ref SysIntX3 region, IntPtr row_pitch, IntPtr slice_pitch, IntPtr ptr, int num_events_in_wait_list, [MarshalAs(UnmanagedType.LPArray)] CLEventHandle[] event_wait_list, [Out] [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)] CLEventHandle[] new_event);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clEnqueueWriteImage")]
		public static extern ComputeErrorCode EnqueueWriteImage(CLCommandQueueHandle command_queue, CLMemoryHandle image, [MarshalAs(UnmanagedType.Bool)] bool blocking_write, ref SysIntX3 origin, ref SysIntX3 region, IntPtr input_row_pitch, IntPtr input_slice_pitch, IntPtr ptr, int num_events_in_wait_list, [MarshalAs(UnmanagedType.LPArray)] CLEventHandle[] event_wait_list, [Out] [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)] CLEventHandle[] new_event);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clEnqueueCopyImage")]
		public static extern ComputeErrorCode EnqueueCopyImage(CLCommandQueueHandle command_queue, CLMemoryHandle src_image, CLMemoryHandle dst_image, ref SysIntX3 src_origin, ref SysIntX3 dst_origin, ref SysIntX3 region, int num_events_in_wait_list, [MarshalAs(UnmanagedType.LPArray)] CLEventHandle[] event_wait_list, [Out] [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)] CLEventHandle[] new_event);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clEnqueueCopyImageToBuffer")]
		public static extern ComputeErrorCode EnqueueCopyImageToBuffer(CLCommandQueueHandle command_queue, CLMemoryHandle src_image, CLMemoryHandle dst_buffer, ref SysIntX3 src_origin, ref SysIntX3 region, IntPtr dst_offset, int num_events_in_wait_list, [MarshalAs(UnmanagedType.LPArray)] CLEventHandle[] event_wait_list, [Out] [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)] CLEventHandle[] new_event);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clEnqueueCopyBufferToImage")]
		public static extern ComputeErrorCode EnqueueCopyBufferToImage(CLCommandQueueHandle command_queue, CLMemoryHandle src_buffer, CLMemoryHandle dst_image, IntPtr src_offset, ref SysIntX3 dst_origin, ref SysIntX3 region, int num_events_in_wait_list, [MarshalAs(UnmanagedType.LPArray)] CLEventHandle[] event_wait_list, [Out] [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)] CLEventHandle[] new_event);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clEnqueueMapBuffer")]
		public static extern IntPtr EnqueueMapBuffer(CLCommandQueueHandle command_queue, CLMemoryHandle buffer, [MarshalAs(UnmanagedType.Bool)] bool blocking_map, ComputeMemoryMappingFlags map_flags, IntPtr offset, IntPtr cb, int num_events_in_wait_list, [MarshalAs(UnmanagedType.LPArray)] CLEventHandle[] event_wait_list, [Out] [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)] CLEventHandle[] new_event, out ComputeErrorCode errcode_ret);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clEnqueueMapImage")]
		public static extern IntPtr EnqueueMapImage(CLCommandQueueHandle command_queue, CLMemoryHandle image, [MarshalAs(UnmanagedType.Bool)] bool blocking_map, ComputeMemoryMappingFlags map_flags, ref SysIntX3 origin, ref SysIntX3 region, out IntPtr image_row_pitch, out IntPtr image_slice_pitch, int num_events_in_wait_list, [MarshalAs(UnmanagedType.LPArray)] CLEventHandle[] event_wait_list, [Out] [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)] CLEventHandle[] new_event, out ComputeErrorCode errcode_ret);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clEnqueueUnmapMemObject")]
		public static extern ComputeErrorCode EnqueueUnmapMemObject(CLCommandQueueHandle command_queue, CLMemoryHandle memobj, IntPtr mapped_ptr, int num_events_in_wait_list, [MarshalAs(UnmanagedType.LPArray)] CLEventHandle[] event_wait_list, [Out] [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)] CLEventHandle[] new_event);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clEnqueueNDRangeKernel")]
		public static extern ComputeErrorCode EnqueueNDRangeKernel(CLCommandQueueHandle command_queue, CLKernelHandle kernel, int work_dim, [MarshalAs(UnmanagedType.LPArray)] IntPtr[] global_work_offset, [MarshalAs(UnmanagedType.LPArray)] IntPtr[] global_work_size, [MarshalAs(UnmanagedType.LPArray)] IntPtr[] local_work_size, int num_events_in_wait_list, [MarshalAs(UnmanagedType.LPArray)] CLEventHandle[] event_wait_list, [Out] [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)] CLEventHandle[] new_event);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clEnqueueTask")]
		public static extern ComputeErrorCode EnqueueTask(CLCommandQueueHandle command_queue, CLKernelHandle kernel, int num_events_in_wait_list, [MarshalAs(UnmanagedType.LPArray)] CLEventHandle[] event_wait_list, [Out] [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)] CLEventHandle[] new_event);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clEnqueueMarker")]
		public static extern ComputeErrorCode EnqueueMarker(CLCommandQueueHandle command_queue, out CLEventHandle new_event);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clEnqueueWaitForEvents")]
		public static extern ComputeErrorCode EnqueueWaitForEvents(CLCommandQueueHandle command_queue, int num_events, [MarshalAs(UnmanagedType.LPArray)] CLEventHandle[] event_list);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clEnqueueBarrier")]
		public static extern ComputeErrorCode EnqueueBarrier(CLCommandQueueHandle command_queue);

		/// <summary>
		/// Gets the extension function address for the given function name,
		/// or NULL if a valid function can not be found. The client must
		/// check to make sure the address is not NULL, before using or 
		/// calling the returned function address.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clGetExtensionFunctionAddress")]
		public static extern IntPtr GetExtensionFunctionAddress(string func_name);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clCreateFromGLBuffer")]
		public static extern CLMemoryHandle CreateFromGLBuffer(CLContextHandle context, ComputeMemoryFlags flags, int bufobj, out ComputeErrorCode errcode_ret);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clCreateFromGLTexture2D")]
		public static extern CLMemoryHandle CreateFromGLTexture2D(CLContextHandle context, ComputeMemoryFlags flags, int target, int miplevel, int texture, out ComputeErrorCode errcode_ret);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clCreateFromGLTexture3D")]
		public static extern CLMemoryHandle CreateFromGLTexture3D(CLContextHandle context, ComputeMemoryFlags flags, int target, int miplevel, int texture, out ComputeErrorCode errcode_ret);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clCreateFromGLRenderbuffer")]
		public static extern CLMemoryHandle CreateFromGLRenderbuffer(CLContextHandle context, ComputeMemoryFlags flags, int renderbuffer, out ComputeErrorCode errcode_ret);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clGetGLObjectInfo")]
		public static extern ComputeErrorCode GetGLObjectInfo(CLMemoryHandle memobj, out ComputeGLObjectType gl_object_type, out int gl_object_name);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clGetGLTextureInfo")]
		public static extern ComputeErrorCode GetGLTextureInfo(CLMemoryHandle memobj, ComputeGLTextureInfo param_name, IntPtr param_value_size, IntPtr param_value, out IntPtr param_value_size_ret);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clEnqueueAcquireGLObjects")]
		public static extern ComputeErrorCode EnqueueAcquireGLObjects(CLCommandQueueHandle command_queue, int num_objects, [MarshalAs(UnmanagedType.LPArray)] CLMemoryHandle[] mem_objects, int num_events_in_wait_list, [MarshalAs(UnmanagedType.LPArray)] CLEventHandle[] event_wait_list, [Out] [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)] CLEventHandle[] new_event);

		/// <summary>
		/// See the OpenCL specification.
		/// </summary>
		[DllImport("OpenCL.dll", EntryPoint = "clEnqueueReleaseGLObjects")]
		public static extern ComputeErrorCode EnqueueReleaseGLObjects(CLCommandQueueHandle command_queue, int num_objects, [MarshalAs(UnmanagedType.LPArray)] CLMemoryHandle[] mem_objects, int num_events_in_wait_list, [MarshalAs(UnmanagedType.LPArray)] CLEventHandle[] event_wait_list, [Out] [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)] CLEventHandle[] new_event);
	}
}
