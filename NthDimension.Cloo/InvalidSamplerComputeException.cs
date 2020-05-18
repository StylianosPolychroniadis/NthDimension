namespace NthDimension.Compute
{
	public class InvalidSamplerComputeException : ComputeException
	{
		public InvalidSamplerComputeException()
			: base(ComputeErrorCode.InvalidSampler)
		{
		}
	}
}
