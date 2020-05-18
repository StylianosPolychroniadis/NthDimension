namespace NthDimension.Compute
{
	public class MapFailureComputeException : ComputeException
	{
		public MapFailureComputeException()
			: base(ComputeErrorCode.MapFailure)
		{
		}
	}
}
