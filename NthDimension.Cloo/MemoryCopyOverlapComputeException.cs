namespace NthDimension.Compute
{
	public class MemoryCopyOverlapComputeException : ComputeException
	{
		public MemoryCopyOverlapComputeException()
			: base(ComputeErrorCode.MemoryCopyOverlap)
		{
		}
	}
}
