using System;
using System.Diagnostics;

namespace NthDimension.Compute
{
	/// <summary>
	/// Represents an error state that occurred while executing an OpenCL API call.
	/// </summary>
	/// <seealso cref="P:Cloo.ComputeException.ComputeErrorCode" />
	public class ComputeException : ApplicationException
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ComputeErrorCode code;

		/// <summary>
		/// Gets the <see cref="P:Cloo.ComputeException.ComputeErrorCode" /> of the <see cref="T:NthDimension.Compute.ComputeException" />.
		/// </summary>
		public ComputeErrorCode ComputeErrorCode
		{
			get
			{
				return code;
			}
		}

		/// <summary>
		/// Creates a new <see cref="T:NthDimension.Compute.ComputeException" /> with a specified <see cref="P:Cloo.ComputeException.ComputeErrorCode" />.
		/// </summary>
		/// <param name="code"> A <see cref="P:Cloo.ComputeException.ComputeErrorCode" />. </param>
		public ComputeException(ComputeErrorCode code)
			: base("OpenCL error code detected: " + code.ToString() + ".")
		{
			this.code = code;
		}

		/// <summary>
		/// Checks for an OpenCL error code and throws a <see cref="T:NthDimension.Compute.ComputeException" /> if such is encountered.
		/// </summary>
		/// <param name="errorCode"> The value to be checked for an OpenCL error. </param>
		public static void ThrowOnError(int errorCode)
		{
			ThrowOnError((ComputeErrorCode)errorCode);
		}

		/// <summary>
		/// Checks for an OpenCL error code and throws a <see cref="T:NthDimension.Compute.ComputeException" /> if such is encountered.
		/// </summary>
		/// <param name="errorCode"> The OpenCL error code. </param>
		public static void ThrowOnError(ComputeErrorCode errorCode)
		{
			switch (errorCode)
			{
			case ComputeErrorCode.Success:
				break;
			case ComputeErrorCode.DeviceNotFound:
				throw new DeviceNotFoundComputeException();
			case ComputeErrorCode.DeviceNotAvailable:
				throw new DeviceNotAvailableComputeException();
			case ComputeErrorCode.CompilerNotAvailable:
				throw new CompilerNotAvailableComputeException();
			case ComputeErrorCode.MemoryObjectAllocationFailure:
				throw new MemoryObjectAllocationFailureComputeException();
			case ComputeErrorCode.OutOfResources:
				throw new OutOfResourcesComputeException();
			case ComputeErrorCode.OutOfHostMemory:
				throw new OutOfHostMemoryComputeException();
			case ComputeErrorCode.ProfilingInfoNotAvailable:
				throw new ProfilingInfoNotAvailableComputeException();
			case ComputeErrorCode.MemoryCopyOverlap:
				throw new MemoryCopyOverlapComputeException();
			case ComputeErrorCode.ImageFormatMismatch:
				throw new ImageFormatMismatchComputeException();
			case ComputeErrorCode.ImageFormatNotSupported:
				throw new ImageFormatNotSupportedComputeException();
			case ComputeErrorCode.BuildProgramFailure:
				throw new BuildProgramFailureComputeException();
			case ComputeErrorCode.MapFailure:
				throw new MapFailureComputeException();
			case ComputeErrorCode.InvalidValue:
				throw new InvalidValueComputeException();
			case ComputeErrorCode.InvalidDeviceType:
				throw new InvalidDeviceTypeComputeException();
			case ComputeErrorCode.InvalidPlatform:
				throw new InvalidPlatformComputeException();
			case ComputeErrorCode.InvalidDevice:
				throw new InvalidDeviceComputeException();
			case ComputeErrorCode.InvalidContext:
				throw new InvalidContextComputeException();
			case ComputeErrorCode.InvalidCommandQueueFlags:
				throw new InvalidCommandQueueFlagsComputeException();
			case ComputeErrorCode.InvalidCommandQueue:
				throw new InvalidCommandQueueComputeException();
			case ComputeErrorCode.InvalidHostPointer:
				throw new InvalidHostPointerComputeException();
			case ComputeErrorCode.InvalidMemoryObject:
				throw new InvalidMemoryObjectComputeException();
			case ComputeErrorCode.InvalidImageFormatDescriptor:
				throw new InvalidImageFormatDescriptorComputeException();
			case ComputeErrorCode.InvalidImageSize:
				throw new InvalidImageSizeComputeException();
			case ComputeErrorCode.InvalidSampler:
				throw new InvalidSamplerComputeException();
			case ComputeErrorCode.InvalidBinary:
				throw new InvalidBinaryComputeException();
			case ComputeErrorCode.InvalidBuildOptions:
				throw new InvalidBuildOptionsComputeException();
			case ComputeErrorCode.InvalidProgram:
				throw new InvalidProgramComputeException();
			case ComputeErrorCode.InvalidProgramExecutable:
				throw new InvalidProgramExecutableComputeException();
			case ComputeErrorCode.InvalidKernelName:
				throw new InvalidKernelNameComputeException();
			case ComputeErrorCode.InvalidKernelDefinition:
				throw new InvalidKernelDefinitionComputeException();
			case ComputeErrorCode.InvalidKernel:
				throw new InvalidKernelComputeException();
			case ComputeErrorCode.InvalidArgumentIndex:
				throw new InvalidArgumentIndexComputeException();
			case ComputeErrorCode.InvalidArgumentValue:
				throw new InvalidArgumentValueComputeException();
			case ComputeErrorCode.InvalidArgumentSize:
				throw new InvalidArgumentSizeComputeException();
			case ComputeErrorCode.InvalidKernelArguments:
				throw new InvalidKernelArgumentsComputeException();
			case ComputeErrorCode.InvalidWorkDimension:
				throw new InvalidWorkDimensionsComputeException();
			case ComputeErrorCode.InvalidWorkGroupSize:
				throw new InvalidWorkGroupSizeComputeException();
			case ComputeErrorCode.InvalidWorkItemSize:
				throw new InvalidWorkItemSizeComputeException();
			case ComputeErrorCode.InvalidGlobalOffset:
				throw new InvalidGlobalOffsetComputeException();
			case ComputeErrorCode.InvalidEventWaitList:
				throw new InvalidEventWaitListComputeException();
			case ComputeErrorCode.InvalidEvent:
				throw new InvalidEventComputeException();
			case ComputeErrorCode.InvalidOperation:
				throw new InvalidOperationComputeException();
			case ComputeErrorCode.InvalidGLObject:
				throw new InvalidGLObjectComputeException();
			case ComputeErrorCode.InvalidBufferSize:
				throw new InvalidBufferSizeComputeException();
			case ComputeErrorCode.InvalidMipLevel:
				throw new InvalidMipLevelComputeException();
			default:
				throw new ComputeException(errorCode);
			}
		}
	}
}
