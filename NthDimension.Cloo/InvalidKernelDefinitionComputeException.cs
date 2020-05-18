namespace NthDimension.Compute
{
	public class InvalidKernelDefinitionComputeException : ComputeException
	{
		public InvalidKernelDefinitionComputeException()
			: base(ComputeErrorCode.InvalidKernelDefinition)
		{
		}
	}
}
