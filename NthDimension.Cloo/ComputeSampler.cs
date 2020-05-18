#define TRACE
using NthDimension.Compute.Bindings;
using System.Diagnostics;
using System.Threading;

namespace NthDimension.Compute
{
	/// <summary>
	/// Represents an OpenCL sampler.
	/// </summary>
	/// <remarks> An object that describes how to sample an image when the image is read in the kernel. The image read functions take a sampler as an argument. The sampler specifies the image addressing-mode i.e. how out-of-range image coordinates are handled, the filtering mode, and whether the input image coordinate is a normalized or unnormalized value. </remarks>
	/// <seealso cref="T:NthDimension.Compute.ComputeImage" />
	public class ComputeSampler : ComputeResource
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ComputeContext context;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ComputeImageAddressing addressing;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ComputeImageFiltering filtering;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly bool normalizedCoords;

		/// <summary>
		/// The handle of the <see cref="T:NthDimension.Compute.ComputeSampler" />.
		/// </summary>
		public CLSamplerHandle Handle
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the <see cref="T:NthDimension.Compute.ComputeContext" /> of the <see cref="T:NthDimension.Compute.ComputeSampler" />.
		/// </summary>
		/// <value> The <see cref="T:NthDimension.Compute.ComputeContext" /> of the <see cref="T:NthDimension.Compute.ComputeSampler" />. </value>
		public ComputeContext Context
		{
			get
			{
				return context;
			}
		}

		/// <summary>
		/// Gets the <see cref="T:NthDimension.Compute.ComputeImageAddressing" /> mode of the <see cref="T:NthDimension.Compute.ComputeSampler" />.
		/// </summary>
		/// <value> The <see cref="T:NthDimension.Compute.ComputeImageAddressing" /> mode of the <see cref="T:NthDimension.Compute.ComputeSampler" />. </value>
		public ComputeImageAddressing Addressing
		{
			get
			{
				return addressing;
			}
		}

		/// <summary>
		/// Gets the <see cref="T:NthDimension.Compute.ComputeImageFiltering" /> mode of the <see cref="T:NthDimension.Compute.ComputeSampler" />.
		/// </summary>
		/// <value> The <see cref="T:NthDimension.Compute.ComputeImageFiltering" /> mode of the <see cref="T:NthDimension.Compute.ComputeSampler" />. </value>
		public ComputeImageFiltering Filtering
		{
			get
			{
				return filtering;
			}
		}

		/// <summary>
		/// Gets the state of usage of normalized x, y and z coordinates when accessing a <see cref="T:NthDimension.Compute.ComputeImage" /> in a <see cref="T:NthDimension.Compute.ComputeKernel" /> through the <see cref="T:NthDimension.Compute.ComputeSampler" />.
		/// </summary>
		/// <value> The state of usage of normalized x, y and z coordinates when accessing a <see cref="T:NthDimension.Compute.ComputeImage" /> in a <see cref="T:NthDimension.Compute.ComputeKernel" /> through the <see cref="T:NthDimension.Compute.ComputeSampler" />. </value>
		public bool NormalizedCoords
		{
			get
			{
				return normalizedCoords;
			}
		}

		/// <summary>
		/// Creates a new <see cref="T:NthDimension.Compute.ComputeSampler" />.
		/// </summary>
		/// <param name="context"> A <see cref="T:NthDimension.Compute.ComputeContext" />. </param>
		/// <param name="normalizedCoords"> The usage state of normalized coordinates when accessing a <see cref="T:NthDimension.Compute.ComputeImage" /> in a <see cref="T:NthDimension.Compute.ComputeKernel" />. </param>
		/// <param name="addressing"> The <see cref="T:NthDimension.Compute.ComputeImageAddressing" /> mode of the <see cref="T:NthDimension.Compute.ComputeSampler" />. Specifies how out-of-range image coordinates are handled while reading. </param>
		/// <param name="filtering"> The <see cref="T:NthDimension.Compute.ComputeImageFiltering" /> mode of the <see cref="T:NthDimension.Compute.ComputeSampler" />. Specifies the type of filter that must be applied when reading data from an image. </param>
		public ComputeSampler(ComputeContext context, bool normalizedCoords, ComputeImageAddressing addressing, ComputeImageFiltering filtering)
		{
			ComputeErrorCode errcode_ret = ComputeErrorCode.Success;
			Handle = CL10.CreateSampler(context.Handle, normalizedCoords, addressing, filtering, out errcode_ret);
			ComputeException.ThrowOnError(errcode_ret);
			SetID(Handle.Value);
			this.addressing = addressing;
			this.context = context;
			this.filtering = filtering;
			this.normalizedCoords = normalizedCoords;
			Trace.WriteLine("Create " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
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
				CL10.ReleaseSampler(Handle);
				Handle.Invalidate();
			}
		}
	}
}
