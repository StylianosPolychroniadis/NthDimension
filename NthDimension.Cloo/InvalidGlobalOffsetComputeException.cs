namespace NthDimension.Compute
{
	public class InvalidGlobalOffsetComputeException : ComputeException
	{
		public InvalidGlobalOffsetComputeException()
			: base(ComputeErrorCode.InvalidGlobalOffset)
		{
		}
	}
}
