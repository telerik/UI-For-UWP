using System;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Attribute for defining a range for a value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ValueRangeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValueRangeAttribute"/> class.
        /// </summary>
        public ValueRangeAttribute(double min, double max, double step)
        {
            this.Range = new NumericalRange(min, max, step);
        }

        /// <summary>
        /// Gets or sets the range of the value.
        /// </summary>
        public INumericalRange Range { get; set; }
    }
}
