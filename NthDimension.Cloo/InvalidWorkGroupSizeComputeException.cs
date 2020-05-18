namespace NthDimension.Compute
{
	public class InvalidWorkGroupSizeComputeException : ComputeException
	{
		public InvalidWorkGroupSizeComputeException()
			: base(ComputeErrorCode.InvalidWorkGroupSize)
		{
		}
	}
}
