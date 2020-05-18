namespace NthDimension.Compute
{
	public class InvalidDeviceComputeException : ComputeException
	{
		public InvalidDeviceComputeException()
			: base(ComputeErrorCode.InvalidDevice)
		{
		}
	}
}
