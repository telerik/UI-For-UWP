using System;
using System.Globalization;
namespace Telerik.Data.Core.Aggregates
{
    /// <summary>
    /// Represents an aggregate that computes the maximum.
    /// </summary>
    internal sealed class MaxAggregate : AggregateValue
    {
        private double max = double.NegativeInfinity;

        /// <inheritdoc />
        protected override object GetValueOverride()
        {
            return this.max;
        }

        /// <inheritdoc />
        protected override void AccumulateOverride(object value)
        {
            this.max = Math.Max(this.max, Convert.ToDouble(value, CultureInfo.InvariantCulture));
        }

        /// <inheritdoc />
        protected override void MergeOverride(AggregateValue childAggregate)
        {
            this.max = Math.Max(this.max, ((MaxAggregate)childAggregate).max);
        }
    }
}