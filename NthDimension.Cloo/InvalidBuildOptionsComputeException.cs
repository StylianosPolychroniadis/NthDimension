namespace NthDimension.Compute
{
	public class InvalidBuildOptionsComputeException : ComputeException
	{
		public InvalidBuildOptionsComputeException()
			: base(ComputeErrorCode.InvalidBuildOptions)
		{
		}
	}
}
