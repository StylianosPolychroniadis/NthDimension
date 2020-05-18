namespace NthDimension.Compute
{
	public class InvalidHostPointerComputeException : ComputeException
	{
		public InvalidHostPointerComputeException()
			: base(ComputeErrorCode.InvalidHostPointer)
		{
		}
	}
}
