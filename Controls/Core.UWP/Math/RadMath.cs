using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Telerik.Core
{
    /// <summary>
    /// Provides static mathematical functions and constants.
    /// </summary>
    public static class RadMath
    {
        /// <summary>
        /// The factor used to convert degrees to their radians equivalent.
        /// </summary>
        public const double DegToRadFactor = Math.PI / 180;

        /// <summary>
        /// The factor used to convert radians to their degree equivalent.
        /// </summary>
        public const double RadToDegFactor = 180 / Math.PI;

        /// <summary>
        /// Smallest unit such that 1.0+DBL_EPSILON != 1.0.
        /// </summary>
        public const double Epsilon = 2.2204460492503131e-9;

        /// <summary>
        /// Determines whether the specified value is close to 0 within the order of EPSILON.
        /// </summary>
        public static bool IsZero(double value)
        {
            return Math.Abs(value) < 10.0 * Epsilon;
        }

        /// <summary>
        /// Determines whether the specified value is close to 0 within the order of EPSILON.
        /// </summary>
        public static bool IsZero(decimal value)
        {
            return IsZero((double)value);
        }

        /// <summary>
        /// Determines whether the specified value is close to 1 within the order of EPSILON.
        /// </summary>
        public static bool IsOne(double value)
        {
            return Math.Abs(value - 1.0) < 10.0 * Epsilon;
        }

        /// <summary>
        /// Determines whether the specified value is close to 1 within the order of EPSILON.
        /// </summary>
        public static bool IsOne(decimal value)
        {
            return IsOne((double)value);
        }

        /// <summary>
        /// Determines whether the two specified values are close within the order of EPSILON.
        /// </summary>
        public static bool AreClose(double value1, double value2)
        {
            return AreClose(value1, value2, Epsilon);
        }

        /// <summary>
        /// Determines whether the two specified values are close within the order of <paramref name="epsilon"/>.
        /// </summary>
        public static bool AreClose(double value1, double value2, double epsilon)
        {
            // in case they are Infinities (then epsilon check does not work)
            if (value1 == value2)
            {
                return true;
            }

            // This computes (|value1-value2| / (|value1| + |value2| + 10.0)) < DBL_EPSILON
            double eps = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * epsilon;
            double delta = value1 - value2;
            return (-eps < delta) && (eps > delta);
        }

        /// <summary>
        /// Gets the distance between two points in a plane.
        /// </summary>
        /// <param name="x1">The x-coordinate of the first point.</param>
        /// <param name="x2">The x-coordinate of the second point.</param>
        /// <param name="y1">The y-coordinate of the first point.</param>
        /// <param name="y2">The y-coordinate of the second point.</param>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "y"), SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "x")]
        public static double GetPointDistance(double x1, double x2, double y1, double y2)
        {
            double dx = x1 - x2;
            double dy = y1 - y2;

            return Math.Sqrt((dx * dx) + (dy * dy));
        }

        /// <summary>
        /// Gets the point that lies on the arc segment of the ellipse, described by the center and radius parameters.
        /// </summary>
        public static RadPoint GetArcPoint(double angle, RadPoint center, double radius)
        {
            double angleInRad = angle * RadMath.DegToRadFactor;

            double x = center.X + (Math.Cos(angleInRad) * radius);
            double y = center.Y + (Math.Sin(angleInRad) * radius);

            return new RadPoint(x, y);
        }

        /// <summary>
        /// Converts cartesian into polar coordinates.
        /// </summary>
        /// <param name="point">The point we are converting.</param>
        /// <param name="centerPoint">The (0,0) point of the the coordinate system.</param>
        /// <param name="reverse">True to reverse the calculated angle using the (360 - angle) expression, false otherwise.</param>
        /// <returns> Coordinates as radius and angle (in degrees).</returns>
        internal static Tuple<double, double> ToPolarCoordinates(RadPoint point, RadPoint centerPoint, bool reverse = false)
        {
            var xOffset = point.X - centerPoint.X;
            var yLength = Math.Abs(point.Y - centerPoint.Y);

            var pointRadius = Math.Sqrt(xOffset * xOffset + yLength * yLength);

            double pointAngle = Math.Asin(yLength / pointRadius) * 180 / Math.PI;

            // Determine quadrant and adjust the point angle accordingly
            if (centerPoint.X < point.X && centerPoint.Y > point.Y)
            {
                // I quadrant
                pointAngle = 360 - pointAngle;
            }
            else if (centerPoint.X >= point.X && centerPoint.Y > point.Y)
            {
                // II quadrant
                pointAngle += 180;
            }
            else if (centerPoint.X >= point.X && centerPoint.Y <= point.Y)
            {
                // III quadrant
                pointAngle = 180 - pointAngle;
            }

            if (reverse)
            {
                pointAngle = (360 - pointAngle) % 360;
            }

            return new Tuple<double, double>(pointRadius, pointAngle);
        }

        internal static RadPoint ToCartesianCoordinates(double radius, double angleDeg)
        {
            var x = radius * Math.Cos(angleDeg / 180 * Math.PI);
            var y = radius * Math.Sin(angleDeg / 180 * Math.PI);

            return new RadPoint(x, y);
        }

        internal static double CoerceValue(double value, double min, double max)
        {
            return Math.Min(Math.Max(min, value), max);
        }

        internal static double CalculateIntersectionY(double x1, double y1, double x2, double y2, double verticalLineX)
        {
            return (verticalLineX - x1) * (y2 - y1) / (x2 - x1) + y1;
        }

        internal static double CalculateIntersectionX(double x1, double y1, double x2, double y2, double horizontalLineY)
        {
            return (horizontalLineY - y1) * (x2 - x1) / (y2 - y1) + x1;
        }

        internal static T Clamp<T>(T value, T min, T max)
        {
            Comparer<T> comparer = Comparer<T>.Default;

            int c = comparer.Compare(value, min);
            if (c < 0)
            {
                return min;
            }

            c = comparer.Compare(value, max);
            if (c > 0)
            {
                return max;
            }

            return value;
        }
    }
}