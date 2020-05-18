namespace NthDimension.Compute
{
	public class InvalidArgumentIndexComputeException : ComputeException
	{
		public InvalidArgumentIndexComputeException()
			: base(ComputeErrorCode.InvalidArgumentIndex)
		{
		}
	}
}
