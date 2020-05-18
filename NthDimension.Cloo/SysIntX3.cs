using System;

namespace NthDimension.Compute
{
	/// <summary>
	/// A structure of three integers of platform specific size.
	/// </summary>
	public struct SysIntX3
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
		/// The third coordinate.
		/// </summary>
		public IntPtr Z;

		/// <summary>
		///
		/// </summary>
		/// <param name="x2"></param>
		/// <param name="z"></param>
		public SysIntX3(SysIntX2 x2, long z)
		{
			this = new SysIntX3(x2.X, x2.Y, new IntPtr(z));
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		public SysIntX3(int x, int y, int z)
		{
			this = new SysIntX3(new IntPtr(x), new IntPtr(y), new IntPtr(z));
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		public SysIntX3(long x, long y, long z)
		{
			this = new SysIntX3(new IntPtr(x), new IntPtr(y), new IntPtr(z));
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		public SysIntX3(IntPtr x, IntPtr y, IntPtr z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		/// <summary>
		/// Gets the string representation of the SysIntX2.
		/// </summary>
		/// <returns> The string representation of the SysIntX2. </returns>
		public override string ToString()
		{
			return X + " " + Y + " " + Z;
		}
	}
}
