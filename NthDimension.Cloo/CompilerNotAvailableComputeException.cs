namespace NthDimension.Compute
{
	public class CompilerNotAvailableComputeException : ComputeException
	{
		public CompilerNotAvailableComputeException()
			: base(ComputeErrorCode.CompilerNotAvailable)
		{
		}
	}
}
