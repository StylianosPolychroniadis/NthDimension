namespace NthDimension.Compute
{
	public class OutOfHostMemoryComputeException : ComputeException
	{
		public OutOfHostMemoryComputeException()
			: base(ComputeErrorCode.OutOfHostMemory)
		{
		}
	}
}
