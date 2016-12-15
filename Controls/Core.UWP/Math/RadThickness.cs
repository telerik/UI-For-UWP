using System;
using System.Diagnostics;

namespace Telerik.Core
{
    /// <summary>
    /// Represents a structure that defines margins (or padding) of an element.
    /// </summary>
    [DebuggerDisplay("{Left}, {Top}, {Right}, {Bottom}")]
    public struct RadThickness
    {
        /// <summary>
        /// A <see cref="RadThickness"/> instance with Left, Top, Right, and Bottom components equal to 0.
        /// </summary>
        public static readonly RadThickness Empty = new RadThickness();

        /// <summary>
        /// Left length.
        /// </summary>
        public double Left;

        /// <summary>
        /// Top length.
        /// </summary>
        public double Top;

        /// <summary>
        /// Right length.
        /// </summary>
        public double Right;

        /// <summary>
        /// Bottom length.
        /// </summary>
        public double Bottom;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadThickness"/> struct.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="right">The right.</param>
        /// <param name="bottom">The bottom.</param>
        public RadThickness(double left, double top, double right, double bottom)
        {
            this.Left = left;
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
        }

        /// <summary>
        /// Determines whether two <see cref="RadThickness"/> structures are equal.
        /// </summary>
        public static bool operator ==(RadThickness thickness1, RadThickness thickness2)
        {
            return thickness1.Left == thickness2.Left && thickness1.Right == thickness2.Right &&
                thickness1.Top == thickness2.Top && thickness1.Bottom == thickness2.Bottom;
        }

        /// <summary>
        /// Determines whether two <see cref="RadThickness"/> structures are not equal.
        /// </summary>
        public static bool operator !=(RadThickness thickness1, RadThickness thickness2)
        {
            return !(thickness1 == thickness2);
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
            if (obj is RadThickness)
            {
                return (RadThickness)obj == this;
            }

            return false;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
        public override int GetHashCode()
        {
            return this.Left.GetHashCode() ^ this.Top.GetHashCode() ^
                this.Right.GetHashCode() ^ this.Bottom.GetHashCode();
        }
    }
}
