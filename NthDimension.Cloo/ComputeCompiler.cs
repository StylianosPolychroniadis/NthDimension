using NthDimension.Compute.Bindings;

namespace NthDimension.Compute
{
	/// <summary>
	/// Represents the OpenCL compiler.
	/// </summary>
	public class ComputeCompiler
	{
		/// <summary>
		/// Unloads the OpenCL compiler.
		/// </summary>
		public static void Unload()
		{
			ComputeErrorCode errorCode = CL10.UnloadCompiler();
			ComputeException.ThrowOnError(errorCode);
		}
	}
}
