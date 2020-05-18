namespace NthDimension.Compute
{
	public class InvalidKernelComputeException : ComputeException
	{
		public InvalidKernelComputeException()
			: base(ComputeErrorCode.InvalidKernel)
		{
		}
	}
}
