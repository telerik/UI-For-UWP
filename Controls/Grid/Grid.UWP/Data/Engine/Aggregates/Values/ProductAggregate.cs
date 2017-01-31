using System;
using System.Globalization;

namespace Telerik.Data.Core.Aggregates
{
    /// <summary>
    /// Represents an aggregate that computes the product of items.
    /// </summary>
    internal sealed class ProductAggregate : AggregateValue
    {
        private double product = 1d;

        /// <inheritdoc />
        protected override object GetValueOverride()
        {
            return this.product;
        }

        /// <inheritdoc />
        protected override void AccumulateOverride(object value)
        {
            this.product *= Convert.ToDouble(value, CultureInfo.InvariantCulture);
        }

        /// <inheritdoc />
        protected override void MergeOverride(AggregateValue childAggregate)
        {
            this.product *= ((ProductAggregate)childAggregate).product;
        }
    }
}
