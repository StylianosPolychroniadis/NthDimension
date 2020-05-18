namespace NthDimension.Compute
{
	public class InvalidBufferSizeComputeException : ComputeException
	{
		public InvalidBufferSizeComputeException()
			: base(ComputeErrorCode.InvalidBufferSize)
		{
		}
	}
}
