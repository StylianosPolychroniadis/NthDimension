namespace NthDimension.Compute
{
	public class InvalidDeviceTypeComputeException : ComputeException
	{
		public InvalidDeviceTypeComputeException()
			: base(ComputeErrorCode.InvalidDeviceType)
		{
		}
	}
}
