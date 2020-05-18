namespace NthDimension.Compute
{
	public class InvalidContextComputeException : ComputeException
	{
		public InvalidContextComputeException()
			: base(ComputeErrorCode.InvalidContext)
		{
		}
	}
}
