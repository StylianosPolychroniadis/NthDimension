namespace NthDimension.Algebra
{

    public sealed class Line
    {
        
        //
        // Summary:
        //     Initializes a new instance of the NthDimension.Math.Line class.
        // TODO:: Algebraic functions
        public Line()
        {
            X1 = 0d;
            X2 = 0d;
            Y1 = 0d;
            Y2 = 0d;
            Z1 = 0d;
            Z2 = 0d;

            Start = new Vector3d(X1, Y1, Z1);
            End = new Vector3d(X2,Y2,Z2);

            //Assert.Debug(Start == new Vector3d(0d, 0d, 0d));
            //Assert.Debug(End == new Vector3d(0d, 0d, 0d));
        }


        // Summary:
        //     Gets or sets the start point of NthDimension.Math.Line.
        //
        // Returns:
        //     The Vector3d of the start point. The default is 0.
        private Vector3d Start { get { return new Vector3d(X1, Y1, Z1); } set { X1 = value.X; Y1 = value.Y; Z1 = value.Z; } }
        private Vector3d End { get { return new Vector3d(X2, Y2, Z2); } set { X2 = value.X; Y2 = value.Y; Z2 = value.Z; } }


        // Summary:
        //     Gets or sets the x-coordinate of the NthDimension.Math.Line start point.
        //
        // Returns:
        //     The x-coordinate for the start point of the line. The default is 0.
        //[TypeConverter(typeof(LengthConverter))]
        public double X1 { get; set; }
        //
        // Summary:
        //     Gets or sets the y-coordinate of the NthDimension.Math.Line start point.
        //
        // Returns:
        //     The y-coordinate for the start point of the line. The default is 0.
        //[TypeConverter(typeof(LengthConverter))]
        public double Y1 { get; set; }
        //
        // Summary:
        //     Gets or sets the x-coordinate of the NthDimension.Math.Line end point.
        //
        // Returns:
        //     The x-coordinate for the end point of the line. The default is 0.
        //[TypeConverter(typeof(LengthConverter))]
        public double X2 { get; set; }
        //
        // Summary:
        //     Gets or sets the y-coordinate of the NthDimension.Math.Line end point.
        //
        // Returns:
        //     The y-coordinate for the end point of the line. The default is 0.
        //[TypeConverter(typeof(LengthConverter))]
        public double Y2 { get; set; }
        //
        // Summary:
        //     Gets or sets the z-coordinate of the NthDimension.Math.Line start point.
        //
        // Returns:
        //     The y-coordinate for the end point of the line. The default is 0.
        public double Z1 { get; set; }
        //
        // Summary:
        //     Gets or sets the z-coordinate of the NthDimension.Math.Line end point.
        //
        // Returns:
        //     The y-coordinate for the end point of the line. The default is 0.
        public double Z2 { get; set; }


        // TODO :: Algebraic functions

    }
}
