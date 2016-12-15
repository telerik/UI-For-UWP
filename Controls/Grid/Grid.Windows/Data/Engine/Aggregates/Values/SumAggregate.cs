using System;
using System.Globalization;

namespace Telerik.Data.Core.Aggregates
{
    /// <summary>
    /// Represents an aggregate that computes the sum of items.
    /// </summary>
    internal sealed class SumAggregate : AggregateValue
    {
        private double sum;

        /// <inheritdoc />
        protected override object GetValueOverride()
        {
            return this.sum;
        }

        /// <inheritdoc />
        protected override void AccumulateOverride(object value)
        {
            this.sum += Convert.ToDouble(value, CultureInfo.InvariantCulture);
        }

        /// <inheritdoc />
        protected override void MergeOverride(AggregateValue childAggregate)
        {
            this.sum += ((SumAggregate)childAggregate).sum;
        }
    }
}
