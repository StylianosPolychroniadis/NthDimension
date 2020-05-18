namespace NthDimension.Compute
{
	public class OutOfResourcesComputeException : ComputeException
	{
		public OutOfResourcesComputeException()
			: base(ComputeErrorCode.OutOfResources)
		{
		}
	}
}
