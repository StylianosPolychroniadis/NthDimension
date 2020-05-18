using System;

namespace NthDimension.Compute
{
	/// <summary>
	///
	/// </summary>
	[Flags]
	public enum cl_device_partition_property_ext
	{
		/// <summary> </summary>
		CL_DEVICE_PARTITION_EQUALLY_EXT = 0x4050,
		/// <summary> </summary>
		CL_DEVICE_PARTITION_BY_COUNTS_EXT = 0x4051,
		/// <summary> </summary>
		CL_DEVICE_PARTITION_BY_NAMES_EXT = 0x4052,
		/// <summary> </summary>
		CL_DEVICE_PARTITION_BY_AFFINITY_DOMAIN_EXT = 0x4053,
		/// <summary> </summary>
		CL_AFFINITY_DOMAIN_L1_CACHE_EXT = 0x1,
		/// <summary> </summary>
		CL_AFFINITY_DOMAIN_L2_CACHE_EXT = 0x2,
		/// <summary> </summary>
		CL_AFFINITY_DOMAIN_L3_CACHE_EXT = 0x3,
		/// <summary> </summary>
		CL_AFFINITY_DOMAIN_L4_CACHE_EXT = 0x4,
		/// <summary> </summary>
		CL_AFFINITY_DOMAIN_NUMA_EXT = 0x10,
		/// <summary> </summary>
		CL_AFFINITY_DOMAIN_NEXT_FISSIONABLE_EXT = 0x100,
		/// <summary> </summary>
		CL_PROPERTIES_LIST_END_EXT = 0x0,
		/// <summary> </summary>
		CL_PARTITION_BY_COUNTS_LIST_END_EXT = 0x0,
		/// <summary> </summary>
		CL_PARTITION_BY_NAMES_LIST_END_EXT = -1
	}
}
