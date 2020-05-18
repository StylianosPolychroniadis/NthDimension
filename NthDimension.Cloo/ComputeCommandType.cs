namespace NthDimension.Compute
{
	/// <summary>
	///
	/// </summary>
	public enum ComputeCommandType
	{
		/// <summary> </summary>
		NDRangeKernel = 4592,
		/// <summary> </summary>
		Task = 4593,
		/// <summary> </summary>
		NativeKernel = 4594,
		/// <summary> </summary>
		ReadBuffer = 4595,
		/// <summary> </summary>
		WriteBuffer = 4596,
		/// <summary> </summary>
		CopyBuffer = 4597,
		/// <summary> </summary>
		ReadImage = 4598,
		/// <summary> </summary>
		WriteImage = 4599,
		/// <summary> </summary>
		CopyImage = 4600,
		/// <summary> </summary>
		CopyImageToBuffer = 4601,
		/// <summary> </summary>
		CopyBufferToImage = 4602,
		/// <summary> </summary>
		MapBuffer = 4603,
		/// <summary> </summary>
		MapImage = 4604,
		/// <summary> </summary>
		UnmapMemory = 4605,
		/// <summary> </summary>
		Marker = 4606,
		/// <summary> </summary>
		AcquireGLObjects = 4607,
		/// <summary> </summary>
		ReleaseGLObjects = 4608,
		/// <summary> </summary>
		ReadBufferRectangle = 4609,
		/// <summary> </summary>
		WriteBufferRectangle = 4610,
		/// <summary> </summary>
		CopyBufferRectangle = 4611,
		/// <summary> </summary>
		User = 4612,
		/// <summary> </summary>
		CL_COMMAND_MIGRATE_MEM_OBJECT_EXT = 16448
	}
}
