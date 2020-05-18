namespace NthDimension.Compute
{
	public class DeviceNotAvailableComputeException : ComputeException
	{
		public DeviceNotAvailableComputeException()
			: base(ComputeErrorCode.DeviceNotAvailable)
		{
		}
	}
}
