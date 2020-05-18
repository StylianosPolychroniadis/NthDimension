using System.Diagnostics;

namespace NthDimension.Compute
{
	/// <summary>
	/// Represents an OpenCL image format.
	/// </summary>
	/// <remarks> This structure defines the type, count and size of the image channels. </remarks>
	/// <seealso cref="T:NthDimension.Compute.ComputeImage" />
	public struct ComputeImageFormat
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ComputeImageChannelOrder channelOrder;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ComputeImageChannelType channelType;

		/// <summary>
		/// Gets the <see cref="T:NthDimension.Compute.ComputeImageChannelOrder" /> of the <see cref="T:NthDimension.Compute.ComputeImage" />.
		/// </summary>
		/// <value> The <see cref="T:NthDimension.Compute.ComputeImageChannelOrder" /> of the <see cref="T:NthDimension.Compute.ComputeImage" />. </value>
		public ComputeImageChannelOrder ChannelOrder
		{
			get
			{
				return channelOrder;
			}
		}

		/// <summary>
		/// Gets the <see cref="T:NthDimension.Compute.ComputeImageChannelType" /> of the <see cref="T:NthDimension.Compute.ComputeImage" />.
		/// </summary>
		/// <value> The <see cref="T:NthDimension.Compute.ComputeImageChannelType" /> of the <see cref="T:NthDimension.Compute.ComputeImage" />. </value>
		public ComputeImageChannelType ChannelType
		{
			get
			{
				return channelType;
			}
		}

		/// <summary>
		/// Creates a new <see cref="T:NthDimension.Compute.ComputeImageFormat" />.
		/// </summary>
		/// <param name="channelOrder"> The number of channels and the channel layout i.e. the memory layout in which channels are stored in the <see cref="T:NthDimension.Compute.ComputeImage" />. </param>
		/// <param name="channelType"> The type of the channel data. The number of bits per element determined by the <paramref name="channelType" /> and <paramref name="channelOrder" /> must be a power of two. </param>
		public ComputeImageFormat(ComputeImageChannelOrder channelOrder, ComputeImageChannelType channelType)
		{
			this.channelOrder = channelOrder;
			this.channelType = channelType;
		}
	}
}
