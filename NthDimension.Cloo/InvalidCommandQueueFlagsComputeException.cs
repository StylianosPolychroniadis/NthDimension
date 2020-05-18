namespace NthDimension.Compute
{
	public class InvalidCommandQueueFlagsComputeException : ComputeException
	{
		public InvalidCommandQueueFlagsComputeException()
			: base(ComputeErrorCode.InvalidCommandQueueFlags)
		{
		}
	}
}
