namespace NthDimension.Compute
{
	public class ImageFormatNotSupportedComputeException : ComputeException
	{
		public ImageFormatNotSupportedComputeException()
			: base(ComputeErrorCode.ImageFormatNotSupported)
		{
		}
	}
}
