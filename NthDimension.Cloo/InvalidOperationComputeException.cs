namespace NthDimension.Compute
{
	public class InvalidOperationComputeException : ComputeException
	{
		public InvalidOperationComputeException()
			: base(ComputeErrorCode.InvalidOperation)
		{
		}
	}
}
