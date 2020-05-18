namespace NthDimension.Compute
{
	public class InvalidWorkDimensionsComputeException : ComputeException
	{
		public InvalidWorkDimensionsComputeException()
			: base(ComputeErrorCode.InvalidWorkDimension)
		{
		}
	}
}
