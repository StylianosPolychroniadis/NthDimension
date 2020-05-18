namespace NthDimension.Compute
{
	public class InvalidGLObjectComputeException : ComputeException
	{
		public InvalidGLObjectComputeException()
			: base(ComputeErrorCode.InvalidGLObject)
		{
		}
	}
}
