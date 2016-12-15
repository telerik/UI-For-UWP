using System;
using System.Globalization;

namespace Telerik.Data.Core.Aggregates
{
    /// <summary>
    /// Represents an aggregate that computes the average of items.
    /// </summary>
    internal sealed class AverageAggregate : AggregateValue
    {
        private double sum;
        private int count;

        /// <inheritdoc />
        protected override object GetValueOverride()
        {
            return this.sum / this.count;
        }

        /// <inheritdoc />
        protected override void AccumulateOverride(object value)
        {
            this.sum += Convert.ToDouble(value, CultureInfo.InvariantCulture);
            this.count++;
        }

        /// <inheritdoc />
        protected override void MergeOverride(AggregateValue childAggregate)
        {
            AverageAggregate averageChildAggregate = (AverageAggregate)childAggregate;
            this.sum += averageChildAggregate.sum;
            this.count += averageChildAggregate.count;
        }
    }
}
