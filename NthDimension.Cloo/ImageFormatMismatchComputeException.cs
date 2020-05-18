namespace NthDimension.Compute
{
	public class ImageFormatMismatchComputeException : ComputeException
	{
		public ImageFormatMismatchComputeException()
			: base(ComputeErrorCode.ImageFormatMismatch)
		{
		}
	}
}
