using System;

namespace Telerik.Core
{
    /// <summary>
    /// Represents a logical definition of a circle.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification = "Override these methods as needed.")]
    public struct RadCircle
    {
        /// <summary>
        /// The center of the circle.
        /// </summary>
        public RadPoint Center;

        /// <summary>
        /// The radius of the circle.
        /// </summary>
        public double Radius;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadCircle"/> struct.
        /// </summary>
        /// <param name="center">The center.</param>
        /// <param name="radius">The radius.</param>
        public RadCircle(RadPoint center, double radius)
        {
            this.Center = center;
            this.Radius = radius;
        }

        /// <summary>
        /// Gets the rectangle that encloses this circle.
        /// </summary>
        public RadRect Bounds
        {
            get
            {
                return new RadRect(this.Center.X - this.Radius, this.Center.Y - this.Radius, this.Radius * 2, this.Radius * 2);
            }
        }
    }
}
