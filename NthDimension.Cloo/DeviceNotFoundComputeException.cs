namespace NthDimension.Compute
{
	public class DeviceNotFoundComputeException : ComputeException
	{
		public DeviceNotFoundComputeException()
			: base(ComputeErrorCode.DeviceNotFound)
		{
		}
	}
}
