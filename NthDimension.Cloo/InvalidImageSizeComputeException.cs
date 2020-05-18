namespace NthDimension.Compute
{
	public class InvalidImageSizeComputeException : ComputeException
	{
		public InvalidImageSizeComputeException()
			: base(ComputeErrorCode.InvalidImageSize)
		{
		}
	}
}
