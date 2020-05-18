namespace NthDimension.Compute
{
	public class InvalidProgramExecutableComputeException : ComputeException
	{
		public InvalidProgramExecutableComputeException()
			: base(ComputeErrorCode.InvalidProgramExecutable)
		{
		}
	}
}
