using NthDimension.Compute.Bindings;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace NthDimension.Compute
{
	/// <summary>
	/// Contains various helper methods.
	/// </summary>
	public class Tools
	{
		/// <summary>
		/// Parses an OpenCL version string.
		/// </summary>
		/// <param name="versionString"> The version string to parse. Must be in the format: <c>Additional substrings[space][major_version].[minor_version][space]Additional substrings</c>. </param>
		/// <param name="substringIndex"> The index of the substring that specifies the OpenCL version. </param>
		/// <returns> A <c>Version</c> instance containing the major and minor version from <paramref name="versionString" />. </returns>
		public static Version ParseVersionString(string versionString, int substringIndex)
		{
			string[] array = versionString.Split(new char[1]
			{
				' '
			}, StringSplitOptions.RemoveEmptyEntries);
			return new Version(array[substringIndex]);
		}

		internal static IntPtr[] ConvertArray(long[] array)
		{
			if (array == null)
			{
				return null;
			}
			NumberFormatInfo numberFormatInfo = new NumberFormatInfo();
			IntPtr[] array2 = new IntPtr[array.Length];
			for (long num = 0L; num < array.Length; num++)
			{
				array2[num] = new IntPtr(array[num]);
			}
			return array2;
		}

		internal static long[] ConvertArray(IntPtr[] array)
		{
			if (array == null)
			{
				return null;
			}
			NumberFormatInfo numberFormatInfo = new NumberFormatInfo();
			long[] array2 = new long[array.Length];
			for (long num = 0L; num < array.Length; num++)
			{
				array2[num] = array[num].ToInt64();
			}
			return array2;
		}

		internal static CLDeviceHandle[] ExtractHandles(ICollection<ComputeDevice> computeObjects, out int handleCount)
		{
			if (computeObjects == null || computeObjects.Count == 0)
			{
				handleCount = 0;
				return null;
			}
			CLDeviceHandle[] array = new CLDeviceHandle[computeObjects.Count];
			int num = 0;
			foreach (ComputeDevice computeObject in computeObjects)
			{
				array[num] = computeObject.Handle;
				num++;
			}
			handleCount = computeObjects.Count;
			return array;
		}

		internal static CLEventHandle[] ExtractHandles(ICollection<ComputeEventBase> computeObjects, out int handleCount)
		{
			if (computeObjects == null || computeObjects.Count == 0)
			{
				handleCount = 0;
				return null;
			}
			CLEventHandle[] array = new CLEventHandle[computeObjects.Count];
			int num = 0;
			foreach (ComputeEventBase computeObject in computeObjects)
			{
				array[num] = computeObject.Handle;
				num++;
			}
			handleCount = computeObjects.Count;
			return array;
		}

		internal static CLMemoryHandle[] ExtractHandles(ICollection<ComputeMemory> computeObjects, out int handleCount)
		{
			if (computeObjects == null || computeObjects.Count == 0)
			{
				handleCount = 0;
				return null;
			}
			CLMemoryHandle[] array = new CLMemoryHandle[computeObjects.Count];
			int num = 0;
			foreach (ComputeMemory computeObject in computeObjects)
			{
				array[num] = computeObject.Handle;
				num++;
			}
			handleCount = computeObjects.Count;
			return array;
		}
	}
}
