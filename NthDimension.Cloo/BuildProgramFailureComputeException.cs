namespace NthDimension.Compute
{
	public class BuildProgramFailureComputeException : ComputeException
	{
		public BuildProgramFailureComputeException()
			: base(ComputeErrorCode.BuildProgramFailure)
		{
		}
	}
}
