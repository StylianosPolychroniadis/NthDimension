namespace NthDimension.Compute
{
	public class InvalidArgumentValueComputeException : ComputeException
	{
		public InvalidArgumentValueComputeException()
			: base(ComputeErrorCode.InvalidArgumentValue)
		{
		}
	}
}
