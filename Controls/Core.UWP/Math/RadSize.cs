using System;
using System.Diagnostics;

namespace Telerik.Core
{
    /// <summary>
    /// Represents a structure that defines a size in the two-dimensional space.
    /// </summary>
    [DebuggerDisplay("{Width}, {Height}")]
    public struct RadSize
    {
        /// <summary>
        /// A <see cref="RadSize"/> instance which Width and Height are set to 0.
        /// </summary>
        public static readonly RadSize Empty = new RadSize(0, 0);

        /// <summary>
        /// A <see cref="RadSize"/> instance which Width and Height are set to -1.
        /// </summary>
        public static readonly RadSize Invalid = new RadSize(-1, -1);

        /// <summary>
        /// The length along the horizontal axis.
        /// </summary>
        public double Width;

        /// <summary>
        /// The length along the vertical axis.
        /// </summary>
        public double Height;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadSize"/> struct.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public RadSize(double width, double height)
        {
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Determines whether two <see cref="RadSize"/> structures are equal.
        /// </summary>
        public static bool operator ==(RadSize size1, RadSize size2)
        {
            return size1.Width == size2.Width && size1.Height == size2.Height;
        }

        /// <summary>
        /// Determines whether two <see cref="RadSize"/> structures are not equal.
        /// </summary>
        public static bool operator !=(RadSize size1, RadSize size2)
        {
            return !(size1 == size2);
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
            if (obj is RadSize)
            {
                return (RadSize)obj == this;
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
