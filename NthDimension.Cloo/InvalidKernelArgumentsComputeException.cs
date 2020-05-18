namespace NthDimension.Compute
{
	public class InvalidKernelArgumentsComputeException : ComputeException
	{
		public InvalidKernelArgumentsComputeException()
			: base(ComputeErrorCode.InvalidKernelArguments)
		{
		}
	}
}
