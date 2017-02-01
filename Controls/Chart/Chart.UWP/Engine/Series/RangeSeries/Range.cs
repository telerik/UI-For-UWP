using System.Diagnostics;

namespace Telerik.Charting
{
    /// <summary>
    /// Represents a struct, which defines a set of two values - High, Low.
    /// </summary>
    [DebuggerDisplay("Low: {Low}, High: {High}")]
    internal struct Range
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Range" /> structure.
        /// </summary>
        /// <param name="low">The low value.</param>
        /// <param name="high">The high value.</param>
        public Range(double low, double high)
            : this()
        {
            this.Low = low;
            this.High = high;
        }

        /// <summary>
        /// Gets or sets the low value.
        /// </summary>
        /// <value>The low value.</value>
        public double Low { get; set; }

        /// <summary>
        /// Gets or sets the high value.
        /// </summary>
        /// <value>The high value.</value>
        public double High { get; set; }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="range1">The first <see cref="Range"/> struct.</param>
        /// <param name="range2">The second <see cref="Range"/> struct.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Range range1, Range range2)
        {
            return range1.High == range2.High && range1.Low == range2.Low;
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="range1">The first <see cref="Range"/> struct.</param>
        /// <param name="range2">The second <see cref="Range"/> struct.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Range range1, Range range2)
        {
            return !(range1 == range2);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object" /> is equal
        /// to the current <see cref="T:System.Object" />.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        /// True if the specified <see cref="T:System.Object" /> is equal to the
        /// current <see cref="T:System.Object" />; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is Range && this == (Range)obj;
        }

        /// <summary>
        /// Determines whether the specified <see cref="Range" /> is equal
        /// to the current <see cref="Range" />.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        /// True if the specified <see cref="Range" /> is equal to the
        /// current <see cref="Range" />; otherwise, false.
        /// </returns>
        public bool Equals(Range obj)
        {
            return this == obj;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
