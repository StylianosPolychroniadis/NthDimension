namespace Triangulator.Geometry
{
	/// <summary>
	/// Triangle made from three point indexes
	/// </summary>
	public struct Triangle
	{
		/// <summary>
		/// First vertex index in triangle
		/// </summary>
		public int p1;

		/// <summary>
		/// Second vertex index in triangle
		/// </summary>
		public int p2;

		/// <summary>
		/// Third vertex index in triangle
		/// </summary>
		public int p3;

		/// <summary>
		/// Initializes a new instance of a triangle
		/// </summary>
		/// <param name="point1">Vertex 1</param>
		/// <param name="point2">Vertex 2</param>
		/// <param name="point3">Vertex 3</param>
		public Triangle(int point1, int point2, int point3)
		{
			p1 = point1;
			p2 = point2;
			p3 = point3;
		}
	}
}
