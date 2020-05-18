namespace NthDimension.Compute
{
	public class InvalidKernelNameComputeException : ComputeException
	{
		public InvalidKernelNameComputeException()
			: base(ComputeErrorCode.InvalidKernelName)
		{
		}
	}
}
