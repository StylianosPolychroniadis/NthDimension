namespace Triangulator.Geometry
{
	/// <summary>
	/// 2D Point with double precision
	/// </summary>
	public class Point
	{
		/// <summary>
		/// X component of point
		/// </summary>
		protected double _X;

		/// <summary>
		/// Y component of point
		/// </summary>
		protected double _Y;

		/// <summary>
		/// Gets or sets the X component of the point
		/// </summary>
		public double X
		{
			get
			{
				return _X;
			}
			set
			{
				_X = value;
			}
		}

		/// <summary>
		/// Gets or sets the Y component of the point
		/// </summary>
		public double Y
		{
			get
			{
				return _Y;
			}
			set
			{
				_Y = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of a point
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public Point(double x, double y)
		{
			_X = x;
			_Y = y;
		}

		/// <summary>
		/// Makes a planar checks for if the points is spatially equal to another point.
		/// </summary>
		/// <param name="other">Point to check against</param>
		/// <returns>True if X and Y values are the same</returns>
		public bool Equals2D(Point other)
		{
			if (X == other.X)
			{
				return Y == other.Y;
			}
			return false;
		}
	}
	/// <summary>
	/// A point with an attribute value of type 'T'
	/// </summary>
	public class Point<T> : Point
	{
		private T _attr;

		/// <summary>
		/// Gets or sets the attribute component of the point
		/// </summary>
		public T Attribute
		{
			get
			{
				return _attr;
			}
			set
			{
				_attr = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the point
		/// </summary>
		/// <param name="x">X component</param>
		/// <param name="y">Y component</param>
		/// <param name="attribute">Attribute</param>
		public Point(double x, double y, T attribute)
			: base(x, y)
		{
			_attr = attribute;
		}

		/// <summary>
		/// Initializes a new instance of the point and sets the attribute to its default value
		/// </summary>
		/// <param name="x">X component</param>
		/// <param name="y">Y component</param>
		public Point(double x, double y)
			: this(x, y, default(T))
		{
		}
	}
}
