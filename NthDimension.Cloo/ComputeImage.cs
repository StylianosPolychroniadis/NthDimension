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
	/// Represents an OpenCL image.
	/// </summary>
	/// <remarks> A memory object that stores a two- or three- dimensional structured array. Image data can only be accessed with read and write functions. The read functions use a sampler. </remarks>
	/// <seealso cref="T:NthDimension.Compute.ComputeMemory" />
	/// <seealso cref="T:NthDimension.Compute.ComputeSampler" />
	public abstract class ComputeImage : ComputeMemory
	{
		/// <summary>
		/// Gets or sets (protected) the depth in pixels of the <see cref="T:NthDimension.Compute.ComputeImage" />.
		/// </summary>
		/// <value> The depth in pixels of the <see cref="T:NthDimension.Compute.ComputeImage" />. </value>
		public int Depth
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets or sets (protected) the size of the elements (pixels) of the <see cref="T:NthDimension.Compute.ComputeImage" />.
		/// </summary>
		/// <value> The size of the elements (pixels) of the <see cref="T:NthDimension.Compute.ComputeImage" />. </value>
		public int ElementSize
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets or sets (protected) the height in pixels of the <see cref="T:NthDimension.Compute.ComputeImage" />.
		/// </summary>
		/// <value> The height in pixels of the <see cref="T:NthDimension.Compute.ComputeImage" />. </value>
		public int Height
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets or sets (protected) the size in bytes of a row of elements of the <see cref="T:NthDimension.Compute.ComputeImage" />.
		/// </summary>
		/// <value> The size in bytes of a row of elements of the <see cref="T:NthDimension.Compute.ComputeImage" />. </value>
		public long RowPitch
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets or sets (protected) the size in bytes of a 2D slice of a <see cref="T:NthDimension.Compute.ComputeImage3D" />.
		/// </summary>
		/// <value> The size in bytes of a 2D slice of a <see cref="T:NthDimension.Compute.ComputeImage3D" />. For a <see cref="T:NthDimension.Compute.ComputeImage2D" /> this value is 0. </value>
		public long SlicePitch
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets or sets (protected) the width in pixels of the <see cref="T:NthDimension.Compute.ComputeImage" />.
		/// </summary>
		/// <value> The width in pixels of the <see cref="T:NthDimension.Compute.ComputeImage" />. </value>
		public int Width
		{
			get;
			protected set;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="context"></param>
		/// <param name="flags"></param>
		protected ComputeImage(ComputeContext context, ComputeMemoryFlags flags)
			: base(context, flags)
		{
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="context"></param>
		/// <param name="flags"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		protected static ICollection<ComputeImageFormat> GetSupportedFormats(ComputeContext context, ComputeMemoryFlags flags, ComputeMemoryType type)
		{
			int num_image_formats = 0;
			ComputeErrorCode supportedImageFormats = CL10.GetSupportedImageFormats(context.Handle, flags, type, 0, null, out num_image_formats);
			ComputeException.ThrowOnError(supportedImageFormats);
			ComputeImageFormat[] array = new ComputeImageFormat[num_image_formats];
			supportedImageFormats = CL10.GetSupportedImageFormats(context.Handle, flags, type, num_image_formats, array, out num_image_formats);
			ComputeException.ThrowOnError(supportedImageFormats);
			return new Collection<ComputeImageFormat>(array);
		}

		/// <summary>
		///
		/// </summary>
		protected void Init()
		{
			SetID(base.Handle.Value);
			Depth = (int)GetInfo<CLMemoryHandle, ComputeImageInfo, IntPtr>(base.Handle, ComputeImageInfo.Depth, CL10.GetImageInfo);
			ElementSize = (int)GetInfo<CLMemoryHandle, ComputeImageInfo, IntPtr>(base.Handle, ComputeImageInfo.ElementSize, CL10.GetImageInfo);
			Height = (int)GetInfo<CLMemoryHandle, ComputeImageInfo, IntPtr>(base.Handle, ComputeImageInfo.Height, CL10.GetImageInfo);
			RowPitch = (long)GetInfo<CLMemoryHandle, ComputeImageInfo, IntPtr>(base.Handle, ComputeImageInfo.RowPitch, CL10.GetImageInfo);
			base.Size = (long)GetInfo<CLMemoryHandle, ComputeMemoryInfo, IntPtr>(base.Handle, ComputeMemoryInfo.Size, CL10.GetMemObjectInfo);
			SlicePitch = (long)GetInfo<CLMemoryHandle, ComputeImageInfo, IntPtr>(base.Handle, ComputeImageInfo.SlicePitch, CL10.GetImageInfo);
			Width = (int)GetInfo<CLMemoryHandle, ComputeImageInfo, IntPtr>(base.Handle, ComputeImageInfo.Width, CL10.GetImageInfo);
			Trace.WriteLine("Create " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
		}
	}
}
