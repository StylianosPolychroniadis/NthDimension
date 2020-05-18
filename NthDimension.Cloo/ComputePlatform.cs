using NthDimension.Compute.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace NthDimension.Compute
{
	/// <summary>
	/// Represents an OpenCL platform.
	/// </summary>
	/// <remarks> The host plus a collection of devices managed by the OpenCL framework that allow an application to share resources and execute kernels on devices in the platform. </remarks>
	/// <seealso cref="T:NthDimension.Compute.ComputeDevice" />
	/// <seealso cref="T:NthDimension.Compute.ComputeKernel" />
	/// <seealso cref="T:NthDimension.Compute.ComputeResource" />
	public class ComputePlatform : ComputeObject
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ReadOnlyCollection<ComputeDevice> devices;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ReadOnlyCollection<string> extensions;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string name;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static ReadOnlyCollection<ComputePlatform> platforms;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string profile;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string vendor;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string version;

		/// <summary>
		/// The handle of the <see cref="T:NthDimension.Compute.ComputePlatform" />.
		/// </summary>
		public CLPlatformHandle Handle
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets a read-only collection of <see cref="T:NthDimension.Compute.ComputeDevice" />s available on the <see cref="T:NthDimension.Compute.ComputePlatform" />.
		/// </summary>
		/// <value> A read-only collection of <see cref="T:NthDimension.Compute.ComputeDevice" />s available on the <see cref="T:NthDimension.Compute.ComputePlatform" />. </value>
		public ReadOnlyCollection<ComputeDevice> Devices
		{
			get
			{
				return devices;
			}
		}

		/// <summary>
		/// Gets a read-only collection of extension names supported by the <see cref="T:NthDimension.Compute.ComputePlatform" />.
		/// </summary>
		/// <value> A read-only collection of extension names supported by the <see cref="T:NthDimension.Compute.ComputePlatform" />. </value>
		public ReadOnlyCollection<string> Extensions
		{
			get
			{
				return extensions;
			}
		}

		/// <summary>
		/// Gets the <see cref="T:NthDimension.Compute.ComputePlatform" /> name.
		/// </summary>
		/// <value> The <see cref="T:NthDimension.Compute.ComputePlatform" /> name. </value>
		public string Name
		{
			get
			{
				return name;
			}
		}

		/// <summary>
		/// Gets a read-only collection of available <see cref="T:NthDimension.Compute.ComputePlatform" />s.
		/// </summary>
		/// <value> A read-only collection of available <see cref="T:NthDimension.Compute.ComputePlatform" />s. </value>
		/// <remarks> The collection will contain no items, if no OpenCL platforms are found on the system. </remarks>
		public static ReadOnlyCollection<ComputePlatform> Platforms
		{
			get
			{
				return platforms;
			}
		}

		/// <summary>
		/// Gets the name of the profile supported by the <see cref="T:NthDimension.Compute.ComputePlatform" />.
		/// </summary>
		/// <value> The name of the profile supported by the <see cref="T:NthDimension.Compute.ComputePlatform" />. </value>
		public string Profile
		{
			get
			{
				return profile;
			}
		}

		/// <summary>
		/// Gets the <see cref="T:NthDimension.Compute.ComputePlatform" /> vendor.
		/// </summary>
		/// <value> The <see cref="T:NthDimension.Compute.ComputePlatform" /> vendor. </value>
		public string Vendor
		{
			get
			{
				return vendor;
			}
		}

		/// <summary>
		/// Gets the OpenCL version string supported by the <see cref="T:NthDimension.Compute.ComputePlatform" />.
		/// </summary>
		/// <value> The OpenCL version string supported by the <see cref="T:NthDimension.Compute.ComputePlatform" />. It has the following format: <c>OpenCL[space][major_version].[minor_version][space][vendor-specific information]</c>. </value>
		public string Version
		{
			get
			{
				return version;
			}
		}

		static ComputePlatform()
		{
			lock (typeof(ComputePlatform))
			{
				try
				{
					if (platforms == null)
					{
						int num_platforms;
						ComputeErrorCode platformIDs = CL10.GetPlatformIDs(0, null, out num_platforms);
						ComputeException.ThrowOnError(platformIDs);
						CLPlatformHandle[] array = new CLPlatformHandle[num_platforms];
						platformIDs = CL10.GetPlatformIDs(num_platforms, array, out num_platforms);
						ComputeException.ThrowOnError(platformIDs);
						List<ComputePlatform> list = new List<ComputePlatform>(num_platforms);
						CLPlatformHandle[] array2 = array;
						foreach (CLPlatformHandle handle in array2)
						{
							list.Add(new ComputePlatform(handle));
						}
						platforms = list.AsReadOnly();
					}
				}
				catch (DllNotFoundException)
				{
					platforms = new List<ComputePlatform>().AsReadOnly();
				}
			}
		}

		private ComputePlatform(CLPlatformHandle handle)
		{
			Handle = handle;
			SetID(Handle.Value);
			string stringInfo = GetStringInfo(Handle, ComputePlatformInfo.Extensions, CL10.GetPlatformInfo);
			extensions = new ReadOnlyCollection<string>(stringInfo.Split(new char[1]
			{
				' '
			}, StringSplitOptions.RemoveEmptyEntries));
			name = GetStringInfo(Handle, ComputePlatformInfo.Name, CL10.GetPlatformInfo);
			profile = GetStringInfo(Handle, ComputePlatformInfo.Profile, CL10.GetPlatformInfo);
			vendor = GetStringInfo(Handle, ComputePlatformInfo.Vendor, CL10.GetPlatformInfo);
			version = GetStringInfo(Handle, ComputePlatformInfo.Version, CL10.GetPlatformInfo);
			QueryDevices();
		}

		/// <summary>
		/// Gets a <see cref="T:NthDimension.Compute.ComputePlatform" /> of a specified handle.
		/// </summary>
		/// <param name="handle"> The handle of the queried <see cref="T:NthDimension.Compute.ComputePlatform" />. </param>
		/// <returns> The <see cref="T:NthDimension.Compute.ComputePlatform" /> of the matching handle or <c>null</c> if none matches. </returns>
		public static ComputePlatform GetByHandle(IntPtr handle)
		{
			foreach (ComputePlatform platform in Platforms)
			{
				if (platform.Handle.Value == handle)
				{
					return platform;
				}
			}
			return null;
		}

		/// <summary>
		/// Gets the first matching <see cref="T:NthDimension.Compute.ComputePlatform" /> of a specified name.
		/// </summary>
		/// <param name="platformName"> The name of the queried <see cref="T:NthDimension.Compute.ComputePlatform" />. </param>
		/// <returns> The first <see cref="T:NthDimension.Compute.ComputePlatform" /> of the specified name or <c>null</c> if none matches. </returns>
		public static ComputePlatform GetByName(string platformName)
		{
			foreach (ComputePlatform platform in Platforms)
			{
				if (platform.Name.Equals(platformName))
				{
					return platform;
				}
			}
			return null;
		}

		/// <summary>
		/// Gets the first matching <see cref="T:NthDimension.Compute.ComputePlatform" /> of a specified vendor.
		/// </summary>
		/// <param name="platformVendor"> The vendor of the queried <see cref="T:NthDimension.Compute.ComputePlatform" />. </param>
		/// <returns> The first <see cref="T:NthDimension.Compute.ComputePlatform" /> of the specified vendor or <c>null</c> if none matches. </returns>
		public static ComputePlatform GetByVendor(string platformVendor)
		{
			foreach (ComputePlatform platform in Platforms)
			{
				if (platform.Vendor.Equals(platformVendor))
				{
					return platform;
				}
			}
			return null;
		}

		/// <summary>
		/// Gets a read-only collection of available <see cref="T:NthDimension.Compute.ComputeDevice" />s on the <see cref="T:NthDimension.Compute.ComputePlatform" />.
		/// </summary>
		/// <returns> A read-only collection of the available <see cref="T:NthDimension.Compute.ComputeDevice" />s on the <see cref="T:NthDimension.Compute.ComputePlatform" />. </returns>
		/// <remarks> This method resets the <c>ComputePlatform.Devices</c>. This is useful if one or more of them become unavailable (<c>ComputeDevice.Available</c> is <c>false</c>) after a <see cref="T:NthDimension.Compute.ComputeContext" /> and <see cref="T:NthDimension.Compute.ComputeCommandQueue" />s that use the <see cref="T:NthDimension.Compute.ComputeDevice" /> have been created and commands have been queued to them. Further calls will trigger an <c>OutOfResourcesComputeException</c> until this method is executed. You will also need to recreate any <see cref="T:NthDimension.Compute.ComputeResource" /> that was created on the no longer available <see cref="T:NthDimension.Compute.ComputeDevice" />. </remarks>
		public ReadOnlyCollection<ComputeDevice> QueryDevices()
		{
			int num_devices = 0;
			ComputeErrorCode deviceIDs = CL10.GetDeviceIDs(Handle, ComputeDeviceTypes.All, 0, null, out num_devices);
			ComputeException.ThrowOnError(deviceIDs);
			CLDeviceHandle[] array = new CLDeviceHandle[num_devices];
			deviceIDs = CL10.GetDeviceIDs(Handle, ComputeDeviceTypes.All, num_devices, array, out num_devices);
			ComputeException.ThrowOnError(deviceIDs);
			ComputeDevice[] array2 = new ComputeDevice[num_devices];
			for (int i = 0; i < num_devices; i++)
			{
				array2[i] = new ComputeDevice(this, array[i]);
			}
			devices = new ReadOnlyCollection<ComputeDevice>(array2);
			return devices;
		}
	}
}
