namespace Telerik.Core
{
    internal struct RadLine
    {
        /// <summary>
        /// The X-coordinate of the line start point.
        /// </summary>
        public double X1;

        /// <summary>
        /// The X-coordinate of the line end point.
        /// </summary>
        public double X2;

        /// <summary>
        /// The Y-coordinate of the line start point.
        /// </summary>
        public double Y1;

        /// <summary>
        /// The Y-coordinate of the line end point.
        /// </summary>
        public double Y2;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadLine" /> struct.
        /// </summary>
        /// <param name="x1">The x1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="y2">The y2.</param>
        public RadLine(double x1, double x2, double y1, double y2)
        {
            this.X1 = x1;
            this.X2 = x2;
            this.Y1 = y1;
            this.Y2 = y2;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RadLine" /> struct.
        /// </summary>
        /// <param name="point1">The point1.</param>
        /// <param name="point2">The point2.</param>
        public RadLine(RadPoint point1, RadPoint point2)
        {
            this.X1 = point1.X;
            this.Y1 = point1.Y;
            this.X2 = point2.X;
            this.Y2 = point2.Y;
        }

        /// <summary>
        /// Rounds the line's values to the closed whole number.
        /// </summary>
        public static RadLine Round(RadLine line)
        {
            return new RadLine((int)(line.X1 + .5d), (int)(line.X2 + .5d), (int)(line.Y1 + .5d), (int)(line.Y2 + .5d));
        }
    }
}
