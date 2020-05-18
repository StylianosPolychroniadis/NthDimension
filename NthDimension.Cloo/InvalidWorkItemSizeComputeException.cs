namespace NthDimension.Compute
{
	public class InvalidWorkItemSizeComputeException : ComputeException
	{
		public InvalidWorkItemSizeComputeException()
			: base(ComputeErrorCode.InvalidWorkItemSize)
		{
		}
	}
}
