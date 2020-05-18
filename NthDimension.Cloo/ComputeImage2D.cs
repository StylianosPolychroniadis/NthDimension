using NthDimension.Compute.Bindings;
using System;
using System.Collections.Generic;

namespace NthDimension.Compute
{
	/// <summary>
	/// Represents an OpenCL 2D image.
	/// </summary>
	/// <seealso cref="T:NthDimension.Compute.ComputeImage" />
	public class ComputeImage2D : ComputeImage
	{
		/// <summary>
		/// Creates a new <see cref="T:NthDimension.Compute.ComputeImage2D" />.
		/// </summary>
		/// <param name="context"> A valid <see cref="T:NthDimension.Compute.ComputeContext" /> in which the <see cref="T:NthDimension.Compute.ComputeImage2D" /> is created. </param>
		/// <param name="flags"> A bit-field that is used to specify allocation and usage information about the <see cref="T:NthDimension.Compute.ComputeImage2D" />. </param>
		/// <param name="format"> A structure that describes the format properties of the <see cref="T:NthDimension.Compute.ComputeImage2D" />. </param>
		/// <param name="width"> The width of the <see cref="T:NthDimension.Compute.ComputeImage2D" /> in pixels. </param>
		/// <param name="height"> The height of the <see cref="T:NthDimension.Compute.ComputeImage2D" /> in pixels. </param>
		/// <param name="rowPitch"> The size in bytes of each row of elements of the <see cref="T:NthDimension.Compute.ComputeImage2D" />. If <paramref name="rowPitch" /> is zero, OpenCL will compute the proper value based on <see cref="P:Cloo.ComputeImage.Width" /> and <see cref="P:Cloo.ComputeImage.ElementSize" />. </param>
		/// <param name="data"> The data to initialize the <see cref="T:NthDimension.Compute.ComputeImage2D" />. Can be <c>IntPtr.Zero</c>. </param>
		public ComputeImage2D(ComputeContext context, ComputeMemoryFlags flags, ComputeImageFormat format, int width, int height, long rowPitch, IntPtr data)
			: base(context, flags)
		{
			ComputeErrorCode errcode_ret = ComputeErrorCode.Success;
			base.Handle = CL10.CreateImage2D(context.Handle, flags, ref format, new IntPtr(width), new IntPtr(height), new IntPtr(rowPitch), data, out errcode_ret);
			ComputeException.ThrowOnError(errcode_ret);
			Init();
		}

		private ComputeImage2D(CLMemoryHandle handle, ComputeContext context, ComputeMemoryFlags flags)
			: base(context, flags)
		{
			base.Handle = handle;
			Init();
		}

		/// <summary>
		/// Creates a new <see cref="T:NthDimension.Compute.ComputeImage2D" /> from an OpenGL renderbuffer object.
		/// </summary>
		/// <param name="context"> A <see cref="T:NthDimension.Compute.ComputeContext" /> with enabled CL/GL sharing. </param>
		/// <param name="flags"> A bit-field that is used to specify usage information about the <see cref="T:NthDimension.Compute.ComputeImage2D" />. Only <c>ComputeMemoryFlags.ReadOnly</c>, <c>ComputeMemoryFlags.WriteOnly</c> and <c>ComputeMemoryFlags.ReadWrite</c> are allowed. </param>
		/// <param name="renderbufferId"> The OpenGL renderbuffer object id to use. </param>
		/// <returns> The created <see cref="T:NthDimension.Compute.ComputeImage2D" />. </returns>
		public static ComputeImage2D CreateFromGLRenderbuffer(ComputeContext context, ComputeMemoryFlags flags, int renderbufferId)
		{
			ComputeErrorCode errcode_ret = ComputeErrorCode.Success;
			CLMemoryHandle handle = CL10.CreateFromGLRenderbuffer(context.Handle, flags, renderbufferId, out errcode_ret);
			ComputeException.ThrowOnError(errcode_ret);
			return new ComputeImage2D(handle, context, flags);
		}

		/// <summary>
		/// Creates a new <see cref="T:NthDimension.Compute.ComputeImage2D" /> from an OpenGL 2D texture object.
		/// </summary>
		/// <param name="context"> A <see cref="T:NthDimension.Compute.ComputeContext" /> with enabled CL/GL sharing. </param>
		/// <param name="flags"> A bit-field that is used to specify usage information about the <see cref="T:NthDimension.Compute.ComputeImage2D" />. Only <c>ComputeMemoryFlags.ReadOnly</c>, <c>ComputeMemoryFlags.WriteOnly</c> and <c>ComputeMemoryFlags.ReadWrite</c> are allowed. </param>
		/// <param name="textureTarget"> One of the following values: GL_TEXTURE_2D, GL_TEXTURE_CUBE_MAP_POSITIVE_X, GL_TEXTURE_CUBE_MAP_POSITIVE_Y, GL_TEXTURE_CUBE_MAP_POSITIVE_Z, GL_TEXTURE_CUBE_MAP_NEGATIVE_X, GL_TEXTURE_CUBE_MAP_NEGATIVE_Y, GL_TEXTURE_CUBE_MAP_NEGATIVE_Z, or GL_TEXTURE_RECTANGLE. Using GL_TEXTURE_RECTANGLE for texture_target requires OpenGL 3.1. Alternatively, GL_TEXTURE_RECTANGLE_ARB may be specified if the OpenGL extension GL_ARB_texture_rectangle is supported. </param>
		/// <param name="mipLevel"> The mipmap level of the OpenGL 2D texture object to be used. </param>
		/// <param name="textureId"> The OpenGL 2D texture object id to use. </param>
		/// <returns> The created <see cref="T:NthDimension.Compute.ComputeImage2D" />. </returns>
		public static ComputeImage2D CreateFromGLTexture2D(ComputeContext context, ComputeMemoryFlags flags, int textureTarget, int mipLevel, int textureId)
		{
			ComputeErrorCode errcode_ret = ComputeErrorCode.Success;
			CLMemoryHandle handle = CL10.CreateFromGLTexture2D(context.Handle, flags, textureTarget, mipLevel, textureId, out errcode_ret);
			ComputeException.ThrowOnError(errcode_ret);
			return new ComputeImage2D(handle, context, flags);
		}

		/// <summary>
		/// Gets a collection of supported <see cref="T:NthDimension.Compute.ComputeImage2D" /> <see cref="T:NthDimension.Compute.ComputeImageFormat" />s in a <see cref="T:NthDimension.Compute.ComputeContext" />.
		/// </summary>
		/// <param name="context"> The <see cref="T:NthDimension.Compute.ComputeContext" /> for which the collection of <see cref="T:NthDimension.Compute.ComputeImageFormat" />s is queried. </param>
		/// <param name="flags"> The <c>ComputeMemoryFlags</c> for which the collection of <see cref="T:NthDimension.Compute.ComputeImageFormat" />s is queried. </param>
		/// <returns> The collection of the required <see cref="T:NthDimension.Compute.ComputeImageFormat" />s. </returns>
		public static ICollection<ComputeImageFormat> GetSupportedFormats(ComputeContext context, ComputeMemoryFlags flags)
		{
			return ComputeImage.GetSupportedFormats(context, flags, ComputeMemoryType.Image2D);
		}
	}
}
