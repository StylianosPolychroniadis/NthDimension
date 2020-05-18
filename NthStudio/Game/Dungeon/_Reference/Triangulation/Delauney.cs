using System;
using System.Collections.Generic;
using Triangulator.Geometry;

namespace Triangulator
{
	/// <summary>
	/// Performs the Delauney triangulation on a set of vertices.
	/// </summary>
	/// <remarks>
	/// Based on Paul Bourke's "An Algorithm for Interpolating Irregularly-Spaced Data
	/// with Applications in Terrain Modelling"
	/// http://astronomy.swin.edu.au/~pbourke/modelling/triangulate/
	/// </remarks>
	public class Delauney
	{
		/// <summary>
		/// Performs Delauney triangulation on a set of points.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The triangulation doesn't support multiple points with the same planar location.
		/// Vertex-lists with duplicate points may result in strange triangulation with intersecting edges.
		/// To avoid adding multiple points to your vertex-list you can use the following anonymous predicate
		/// method:
		/// <code>
		/// if(!Vertices.Exists(delegate(Triangulator.Geometry.Point p) { return pNew.Equals2D(p); }))
		/// 	Vertices.Add(pNew);
		/// </code>
		/// </para>
		/// <para>The triangulation algorithm may be described in pseudo-code as follows:
		/// <code>
		/// subroutine Triangulate
		/// input : vertex list
		/// output : triangle list
		///    initialize the triangle list
		///    determine the supertriangle
		///    add supertriangle vertices to the end of the vertex list
		///    add the supertriangle to the triangle list
		///    for each sample point in the vertex list
		///       initialize the edge buffer
		///       for each triangle currently in the triangle list
		///          calculate the triangle circumcircle center and radius
		///          if the point lies in the triangle circumcircle then
		///             add the three triangle edges to the edge buffer
		///             remove the triangle from the triangle list
		///          endif
		///       endfor
		///       delete all doubly specified edges from the edge buffer
		///          this leaves the edges of the enclosing polygon only
		///       add to the triangle list all triangles formed between the point 
		///          and the edges of the enclosing polygon
		///    endfor
		///    remove any triangles from the triangle list that use the supertriangle vertices
		///    remove the supertriangle vertices from the vertex list
		/// end
		/// </code>
		/// </para>
		/// </remarks>
		/// <param name="Vertex">List of vertices to triangulate.</param>
		/// <returns>Triangles referencing vertex indices arranged in clockwise order</returns>
		public static List<Triangle> Triangulate(List<Point> Vertex)
		{
			int count = Vertex.Count;
			if (count < 3)
			{
				throw new ArgumentException("Need at least three vertices for triangulation");
			}
			int num = 4 * count;
			double x = Vertex[0].X;
			double y = Vertex[0].Y;
			double num2 = x;
			double num3 = y;
			for (int i = 1; i < count; i++)
			{
				if (Vertex[i].X < x)
				{
					x = Vertex[i].X;
				}
				if (Vertex[i].X > num2)
				{
					num2 = Vertex[i].X;
				}
				if (Vertex[i].Y < y)
				{
					y = Vertex[i].Y;
				}
				if (Vertex[i].Y > num3)
				{
					num3 = Vertex[i].Y;
				}
			}
			double num4 = num2 - x;
			double num5 = num3 - y;
			double num6 = (num4 > num5) ? num4 : num5;
			double num7 = (num2 + x) * 0.5;
			double num8 = (num3 + y) * 0.5;
			Vertex.Add(new Point(num7 - 2.0 * num6, num8 - num6));
			Vertex.Add(new Point(num7, num8 + 2.0 * num6));
			Vertex.Add(new Point(num7 + 2.0 * num6, num8 - num6));
			List<Triangle> list = new List<Triangle>();
			list.Add(new Triangle(count, count + 1, count + 2));
			for (int j = 0; j < count; j++)
			{
				List<Edge> list2 = new List<Edge>();
				for (int k = 0; k < list.Count; k++)
				{
					if (InCircle(Vertex[j], Vertex[list[k].p1], Vertex[list[k].p2], Vertex[list[k].p3]))
					{
						list2.Add(new Edge(list[k].p1, list[k].p2));
						list2.Add(new Edge(list[k].p2, list[k].p3));
						list2.Add(new Edge(list[k].p3, list[k].p1));
						list.RemoveAt(k);
						k--;
					}
				}
				if (j >= count)
				{
					continue;
				}
				for (int num9 = list2.Count - 2; num9 >= 0; num9--)
				{
					for (int num10 = list2.Count - 1; num10 >= num9 + 1; num10--)
					{
						if (list2[num9].Equals(list2[num10]))
						{
							list2.RemoveAt(num10);
							list2.RemoveAt(num9);
							num10--;
						}
					}
				}
				for (int l = 0; l < list2.Count; l++)
				{
					if (list.Count >= num)
					{
						throw new ApplicationException("Exceeded maximum edges");
					}
					list.Add(new Triangle(list2[l].p1, list2[l].p2, j));
				}
				list2.Clear();
				list2 = null;
			}
			for (int num11 = list.Count - 1; num11 >= 0; num11--)
			{
				if (list[num11].p1 >= count || list[num11].p2 >= count || list[num11].p3 >= count)
				{
					list.RemoveAt(num11);
				}
			}
			Vertex.RemoveAt(Vertex.Count - 1);
			Vertex.RemoveAt(Vertex.Count - 1);
			Vertex.RemoveAt(Vertex.Count - 1);
			list.TrimExcess();
			return list;
		}

		/// <summary>
		/// Returns true if the point (p) lies inside the circumcircle made up by points (p1,p2,p3)
		/// </summary>
		/// <remarks>
		/// NOTE: A point on the edge is inside the circumcircle
		/// </remarks>
		/// <param name="p">Point to check</param>
		/// <param name="p1">First point on circle</param>
		/// <param name="p2">Second point on circle</param>
		/// <param name="p3">Third point on circle</param>
		/// <returns>true if p is inside circle</returns>
		private static bool InCircle(Point p, Point p1, Point p2, Point p3)
		{
			if (Math.Abs(p1.Y - p2.Y) < double.Epsilon && Math.Abs(p2.Y - p3.Y) < double.Epsilon)
			{
				return false;
			}
			double num4;
			double num5;
			if (Math.Abs(p2.Y - p1.Y) < double.Epsilon)
			{
				double num = (0.0 - (p3.X - p2.X)) / (p3.Y - p2.Y);
				double num2 = (p2.X + p3.X) * 0.5;
				double num3 = (p2.Y + p3.Y) * 0.5;
				num4 = (p2.X + p1.X) * 0.5;
				num5 = num * (num4 - num2) + num3;
			}
			else if (Math.Abs(p3.Y - p2.Y) < double.Epsilon)
			{
				double num6 = (0.0 - (p2.X - p1.X)) / (p2.Y - p1.Y);
				double num7 = (p1.X + p2.X) * 0.5;
				double num8 = (p1.Y + p2.Y) * 0.5;
				num4 = (p3.X + p2.X) * 0.5;
				num5 = num6 * (num4 - num7) + num8;
			}
			else
			{
				double num6 = (0.0 - (p2.X - p1.X)) / (p2.Y - p1.Y);
				double num = (0.0 - (p3.X - p2.X)) / (p3.Y - p2.Y);
				double num7 = (p1.X + p2.X) * 0.5;
				double num2 = (p2.X + p3.X) * 0.5;
				double num8 = (p1.Y + p2.Y) * 0.5;
				double num3 = (p2.Y + p3.Y) * 0.5;
				num4 = (num6 * num7 - num * num2 + num3 - num8) / (num6 - num);
				num5 = num6 * (num4 - num7) + num8;
			}
			double num9 = p2.X - num4;
			double num10 = p2.Y - num5;
			double num11 = num9 * num9 + num10 * num10;
			num9 = p.X - num4;
			num10 = p.Y - num5;
			double num12 = num9 * num9 + num10 * num10;
			return num12 <= num11;
		}
	}
}
