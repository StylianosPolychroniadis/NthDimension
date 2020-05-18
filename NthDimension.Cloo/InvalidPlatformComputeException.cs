namespace NthDimension.Compute
{
	public class InvalidPlatformComputeException : ComputeException
	{
		public InvalidPlatformComputeException()
			: base(ComputeErrorCode.InvalidPlatform)
		{
		}
	}
}
