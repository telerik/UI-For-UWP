using System;

namespace Telerik.Core
{
    /// <summary>
    /// Represents a structure that defines a range of two IComparable structures - like Single or Double.
    /// </summary>
    /// <typeparam name="T">Must implement the <see cref="IComparable"/> interface.</typeparam>
    public struct ValueRange<T> where T : struct, IComparable<T>
    {
        /// <summary>
        /// Empty value range where minimum and maximum are set to their default(T) value.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "Defining this field is essental to simplify the usage of Empty (rather than complicate it according to this rule).")]
        public static readonly ValueRange<T> Empty = new ValueRange<T>(default(T), default(T));

        internal T minimum;
        internal T maximum;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueRange&lt;T&gt;"/> struct.
        /// </summary>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        public ValueRange(T min, T max)
        {
            this.minimum = min;
            this.maximum = max;
        }

        /// <summary>
        /// Gets or sets the maximum of the range.
        /// </summary>
        public T Maximum
        {
            get
            {
                return this.maximum;
            }
            set
            {
                int compare = value.CompareTo(this.minimum);
                if (compare < 0)
                {
                    value = this.minimum;
                }

                this.maximum = value;
            }
        }

        /// <summary>
        /// Gets or sets the minimum of the range.
        /// </summary>
        public T Minimum
        {
            get
            {
                return this.minimum;
            }
            set
            {
                int compare = value.CompareTo(this.maximum);
                if (compare > 0)
                {
                    value = this.maximum;
                }

                this.minimum = value;
            }
        }

        /// <summary>
        /// Determines whether two ranges are equal.
        /// </summary>
        /// <param name="range1">First ValueRange to be compared.</param>
        /// <param name="range2">Second ValueRange to be compared.</param>
        public static bool operator ==(ValueRange<T> range1, ValueRange<T> range2)
        {
            return range1.minimum.CompareTo(range2.minimum) == 0 && range1.maximum.CompareTo(range2.maximum) == 0;
        }

        /// <summary>
        /// Determines whether two ranges are not equal.
        /// </summary>
        /// <param name="range1">First ValueRange to be compared.</param>
        /// <param name="range2">Second ValueRange to be compared.</param>
        public static bool operator !=(ValueRange<T> range1, ValueRange<T> range2)
        {
            return !(range1 == range2);
        }

        /// <summary>
        /// Determines whether the specified value is within the range, excluding its minimum and maximum values.
        /// </summary>
        /// <param name="value">The value.</param>
        public bool IsInRangeExclusive(T value)
        {
            return value.CompareTo(this.minimum) > 0 && value.CompareTo(this.maximum) < 0;
        }

        /// <summary>
        /// Determines whether the specified value is within the range, including its minimum and maximum values.
        /// </summary>
        /// <param name="value">The value.</param>
        public bool IsInRangeInclusive(T value)
        {
            return value.CompareTo(this.minimum) >= 0 && value.CompareTo(this.maximum) <= 0;
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
            if (obj is ValueRange<T>)
            {
                return (ValueRange<T>)obj == this;
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
