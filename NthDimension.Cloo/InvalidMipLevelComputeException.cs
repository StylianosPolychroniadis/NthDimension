namespace NthDimension.Compute
{
	public class InvalidMipLevelComputeException : ComputeException
	{
		public InvalidMipLevelComputeException()
			: base(ComputeErrorCode.InvalidMipLevel)
		{
		}
	}
}
