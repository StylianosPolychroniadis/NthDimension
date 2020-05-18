namespace NthDimension.Compute
{
	public class InvalidCommandQueueComputeException : ComputeException
	{
		public InvalidCommandQueueComputeException()
			: base(ComputeErrorCode.InvalidCommandQueue)
		{
		}
	}
}
