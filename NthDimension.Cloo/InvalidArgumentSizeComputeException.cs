namespace NthDimension.Compute
{
	public class InvalidArgumentSizeComputeException : ComputeException
	{
		public InvalidArgumentSizeComputeException()
			: base(ComputeErrorCode.InvalidArgumentSize)
		{
		}
	}
}
