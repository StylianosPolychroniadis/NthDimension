namespace NthDimension.Compute
{
	public class ProfilingInfoNotAvailableComputeException : ComputeException
	{
		public ProfilingInfoNotAvailableComputeException()
			: base(ComputeErrorCode.ProfilingInfoNotAvailable)
		{
		}
	}
}
