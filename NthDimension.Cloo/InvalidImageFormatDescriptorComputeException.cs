namespace NthDimension.Compute
{
	public class InvalidImageFormatDescriptorComputeException : ComputeException
	{
		public InvalidImageFormatDescriptorComputeException()
			: base(ComputeErrorCode.InvalidImageFormatDescriptor)
		{
		}
	}
}
