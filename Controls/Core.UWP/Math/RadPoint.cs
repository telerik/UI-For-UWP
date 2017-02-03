using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Telerik.Core
{
    /// <summary>
    /// Represents a structure that defines a location (X, Y) in a two-dimensional space.
    /// </summary>
    [DebuggerDisplay("{X}, {Y}")]
    public struct RadPoint
    {
        /// <summary>
        /// A <see cref="RadPoint"/> instance which X and Y values are set to 0.
        /// </summary>
        public static readonly RadPoint Empty = new RadPoint(0, 0);

        /// <summary>
        /// The X-coordinate of the point.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = nameof(X))]
        public double X;

        /// <summary>
        /// The Y-coordinate of the point.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = nameof(Y))]
        public double Y;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadPoint"/> struct.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "y"), SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "x")]
        public RadPoint(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Determines whether two <see cref="RadPoint"/> structures are equal.
        /// </summary>
        public static bool operator ==(RadPoint point1, RadPoint point2)
        {
            return point1.X == point2.X && point1.Y == point2.Y;
        }

        /// <summary>
        /// Determines whether two <see cref="RadSize"/> structures are not equal.
        /// </summary>
        public static bool operator !=(RadPoint point1, RadPoint point2)
        {
            return !(point1 == point2);
        }

        /// <summary>
        /// Rounds the X and Y members of the specified <see cref="RadPoint"/>.
        /// </summary>
        public static RadPoint Round(RadPoint point)
        {
            point.X = Math.Round(point.X);
            point.Y = Math.Round(point.Y);

            return point;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is RadPoint)
            {
                return (RadPoint)obj == this;
            }

            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}