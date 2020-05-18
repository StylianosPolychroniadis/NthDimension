namespace NthDimension.Compute
{
	public class InvalidBinaryComputeException : ComputeException
	{
		public InvalidBinaryComputeException()
			: base(ComputeErrorCode.InvalidBinary)
		{
		}
	}
}
