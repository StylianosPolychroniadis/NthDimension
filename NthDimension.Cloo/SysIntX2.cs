using System;

namespace NthDimension.Compute
{
	/// <summary>
	///             A structure of two integers of platform specific size.
	/// </summary>
	public struct SysIntX2
	{
		/// <summary>
		/// The first coordinate.
		/// </summary>
		public IntPtr X;

		/// <summary>
		/// The second coordinate.
		/// </summary>
		public IntPtr Y;

		/// <summary>
		///
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public SysIntX2(int x, int y)
		{
			this = new SysIntX2(new IntPtr(x), new IntPtr(y));
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public SysIntX2(long x, long y)
		{
			this = new SysIntX2(new IntPtr(x), new IntPtr(y));
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public SysIntX2(IntPtr x, IntPtr y)
		{
			X = x;
			Y = y;
		}

		/// <summary>
		/// Gets the string representation of the SysIntX2.
		/// </summary>
		/// <returns> The string representation of the SysIntX2. </returns>
		public override string ToString()
		{
			return X + " " + Y;
		}
	}
}
