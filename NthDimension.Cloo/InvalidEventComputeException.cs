namespace NthDimension.Compute
{
	public class InvalidEventComputeException : ComputeException
	{
		public InvalidEventComputeException()
			: base(ComputeErrorCode.InvalidEvent)
		{
		}
	}
}
