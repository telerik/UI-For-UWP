using System;
using System.Diagnostics;

namespace Telerik.Core
{
    /// <summary>
    /// Represents a Rectangle in the Euclidean plane geometry.
    /// </summary>
    [DebuggerDisplay("{X}, {Y}, {Width}, {Height}")]
    public struct RadRect
    {
        /// <summary>
        /// Invalid rectangle, which Width and Height properties are set to (-1).
        /// </summary>
        public static readonly RadRect Invalid = new RadRect(-1, -1, -1, -1);

        /// <summary>
        /// Empty rectangle which values are zeroes.
        /// </summary>
        public static readonly RadRect Empty = new RadRect(0, 0, 0, 0);

        /// <summary>
        /// The X-coordinate of the rectangle.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = nameof(X))]
        public double X;

        /// <summary>
        /// The Y-coordinate of the rectangle.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = nameof(Y))]
        public double Y;

        /// <summary>
        /// The length of the rectangle along the X-axis.
        /// </summary>
        public double Width;

        /// <summary>
        /// The length of the rectangle along the Y-axis.
        /// </summary>
        public double Height;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadRect"/> struct.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public RadRect(double width, double height)
        {
            this.X = 0;
            this.Y = 0;
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RadRect" /> struct.
        /// </summary>
        /// <param name="point1">The point1.</param>
        /// <param name="point2">The point2.</param>
        public RadRect(RadPoint point1, RadPoint point2)
        {
            this.X = Math.Min(point1.X, point2.X);
            this.Y = Math.Min(point1.Y, point2.Y);
            this.Width = Math.Max(Math.Max(point1.X, point2.X) - this.X, 0);
            this.Height = Math.Max(Math.Max(point1.Y, point2.Y) - this.Y, 0);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RadRect"/> struct.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "y"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "x")]
        public RadRect(double x, double y, double width, double height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Gets the Y-coordinate of the bottom side of the rectangle.
        /// </summary>
        public double Bottom
        {
            get
            {
                return this.Y + this.Height;
            }
        }

        /// <summary>
        /// Gets the X-coordinate of the right side of the rectangle.
        /// </summary>
        public double Right
        {
            get
            {
                return this.X + this.Width;
            }
        }

        /// <summary>
        /// Gets the center of this rectangle.
        /// </summary>
        public RadPoint Center
        {
            get
            {
                return new RadPoint(this.X + (this.Width / 2), this.Y + (this.Height / 2));
            }
        }

        /// <summary>
        /// Gets the location (Top-Left corner) of the rectangle.
        /// </summary>
        public RadPoint Location
        {
            get
            {
                return new RadPoint(this.X, this.Y);
            }
        }

        /// <summary>
        /// Rounds the rectangle's values to the closed whole number.
        /// </summary>
        public static RadRect Round(RadRect rect)
        {
            return new RadRect((int)(rect.X + .5d), (int)(rect.Y + .5d), (int)(rect.Width + .5d), (int)(rect.Height + .5d));
        }

        /// <summary>
        /// Rounds the rectangle's value to the closest less than or equal to whole numbers.
        /// </summary>
        public static RadRect Floor(RadRect rect)
        {
            return new RadRect((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
        }

        /// <summary>
        /// Determines whether two <see cref="RadRect"/> structures are equal.
        /// </summary>
        public static bool operator ==(RadRect rect1, RadRect rect2)
        {
            return rect1.X == rect2.X && rect1.Y == rect2.Y && rect1.Width == rect2.Width && rect1.Height == rect2.Height;
        }

        /// <summary>
        /// Determines whether two <see cref="RadRect"/> structures are not equal.
        /// </summary>
        public static bool operator !=(RadRect rect1, RadRect rect2)
        {
            return !(rect1 == rect2);
        }

        /// <summary>
        /// Gets the difference between two <see cref="RadRect"/> structures.
        /// </summary>
        public static RadThickness Subtract(RadRect rect1, RadRect rect2)
        {
            double left = Math.Abs(rect1.X - rect2.X);
            double top = Math.Abs(rect1.Y - rect2.Y);
            double right = Math.Abs(rect1.Right - rect2.Right);
            double bottom = Math.Abs(rect1.Bottom - rect2.Bottom);

            return new RadThickness(left, top, right, bottom);
        }

        /// <summary>
        /// Gets a rectangle that has equal width and height and is centered within the specified rect.
        /// </summary>
        /// <param name="rect">The rect to create the square from.</param>
        /// <param name="offset">True to offset the rectangle's location to meet the smaller of the Width and Height properties.</param>
        public static RadRect ToSquare(RadRect rect, bool offset)
        {
            double minLength = Math.Min(rect.Width, rect.Height);

            if (offset)
            {
                rect.X += (rect.Width - minLength) / 2;
                rect.Y += (rect.Height - minLength) / 2;
            }

            rect.Width = minLength;
            rect.Height = minLength;

            return rect;
        }

        /// <summary>
        /// Centers the specified rectangle within the provided available one.
        /// </summary>
        public static RadRect CenterRect(RadRect rect, RadRect bounds)
        {
            double offsetX = (bounds.Width - rect.Width) / 2;
            double offsetY = (bounds.Height - rect.Height) / 2;

            rect.X = bounds.X + offsetX;
            rect.Y = bounds.Y + offsetY;

            return rect;
        }

        /// <summary>
        /// Determines whether the current rect intersects with the specified one.
        /// </summary>
        public bool IntersectsWith(RadRect rect)
        {
            return rect.X <= this.X + this.Width &&
                rect.X + rect.Width >= this.X &&
                rect.Y <= this.Y + this.Height &&
                rect.Y + rect.Height >= this.Y;
        }

        /// <summary>
        /// Determines whether the size of this rect is valid - that is both Width and Height should be bigger than zero.
        /// </summary>
        public bool IsSizeValid()
        {
            return this.Width > 0 && this.Height > 0;
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
            if (obj is RadRect)
            {
                return (RadRect)obj == this;
            }

            return false;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "X = " + this.X + ", Y = " + this.Y + ", Width = " + this.Width + ", Height = " + this.Height;
        }

        /// <summary>
        /// Determines if this RadRect instance contains the point that is described by the arguments.
        /// </summary>
        /// <param name="x">The X coordinate of the point to check.</param>
        /// <param name="y">The Y coordinate of the point to check.</param>
        /// <returns>Returns true if this rectangle contains the point from the arguments and false otherwise.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "y"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "x")]
        public bool Contains(double x, double y)
        {
            return x >= this.X && x <= this.X + this.Width &&
                   y >= this.Y && y <= this.Y + this.Height;
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
