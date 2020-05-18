namespace NthDimension.Compute
{
	public class InvalidEventWaitListComputeException : ComputeException
	{
		public InvalidEventWaitListComputeException()
			: base(ComputeErrorCode.InvalidEventWaitList)
		{
		}
	}
}
