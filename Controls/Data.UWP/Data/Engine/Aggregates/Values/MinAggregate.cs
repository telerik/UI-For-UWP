using System;
using System.Globalization;
namespace Telerik.Data.Core.Aggregates
{
    /// <summary>
    /// Represents an aggregate that computes the minimum.
    /// </summary>
    internal sealed class MinAggregate : AggregateValue
    {
        private double min = double.PositiveInfinity;

        /// <inheritdoc />
        protected override object GetValueOverride()
        {
            return this.min;
        }

        /// <inheritdoc />
        protected override void AccumulateOverride(object value)
        {
            this.min = Math.Min(this.min, Convert.ToDouble(value, CultureInfo.InvariantCulture));
        }

        /// <inheritdoc />
        protected override void MergeOverride(AggregateValue childAggregate)
        {
            this.min = Math.Min(this.min, ((MinAggregate)childAggregate).min);
        }
    }
}