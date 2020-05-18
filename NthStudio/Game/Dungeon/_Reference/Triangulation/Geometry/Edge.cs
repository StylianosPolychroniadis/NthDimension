using System;

namespace Triangulator.Geometry
{
	/// <summary>
	/// Edge made from two point indexes
	/// </summary>
	public class Edge : IEquatable<Edge>
	{
		/// <summary>
		/// Start of edge index
		/// </summary>
		public int p1;

		/// <summary>
		/// End of edge index
		/// </summary>
		public int p2;

		/// <summary>
		/// Initializes a new edge instance
		/// </summary>
		/// <param name="point1">Start edge vertex index</param>
		/// <param name="point2">End edge vertex index</param>
		public Edge(int point1, int point2)
		{
			p1 = point1;
			p2 = point2;
		}

		/// <summary>
		/// Initializes a new edge instance with start/end indexes of '0'
		/// </summary>
		public Edge()
			: this(0, 0)
		{
		}

		/// <summary>
		/// Checks whether two edges are equal disregarding the direction of the edges
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(Edge other)
		{
			if (p1 != other.p2 || p2 != other.p1)
			{
				if (p1 == other.p1)
				{
					return p2 == other.p2;
				}
				return false;
			}
			return true;
		}
	}
}
