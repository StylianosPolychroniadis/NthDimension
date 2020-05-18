namespace NthDimension.Compute
{
	public class InvalidValueComputeException : ComputeException
	{
		public InvalidValueComputeException()
			: base(ComputeErrorCode.InvalidValue)
		{
		}
	}
}
