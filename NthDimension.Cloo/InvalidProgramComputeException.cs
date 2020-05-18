namespace NthDimension.Compute
{
	public class InvalidProgramComputeException : ComputeException
	{
		public InvalidProgramComputeException()
			: base(ComputeErrorCode.InvalidProgram)
		{
		}
	}
}
