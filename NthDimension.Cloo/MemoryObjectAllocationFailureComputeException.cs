namespace NthDimension.Compute
{
	public class MemoryObjectAllocationFailureComputeException : ComputeException
	{
		public MemoryObjectAllocationFailureComputeException()
			: base(ComputeErrorCode.MemoryObjectAllocationFailure)
		{
		}
	}
}
