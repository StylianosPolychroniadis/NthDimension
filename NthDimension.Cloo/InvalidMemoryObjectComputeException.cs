namespace NthDimension.Compute
{
	public class InvalidMemoryObjectComputeException : ComputeException
	{
		public InvalidMemoryObjectComputeException()
			: base(ComputeErrorCode.InvalidMemoryObject)
		{
		}
	}
}
