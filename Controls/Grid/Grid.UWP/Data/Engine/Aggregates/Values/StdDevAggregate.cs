using System;

namespace Telerik.Data.Core.Aggregates
{
    /// <summary>
    /// Represents an aggregate that estimates the standard deviation of a population based on a sample.
    /// </summary>
    internal sealed class StdDevAggregate : VarianceAggregateBase
    {
        /// <inheritdoc />
        protected override object GetValueOverride()
        {
            if (this.Count <= 1)
            {
                return AggregateValue.Error;
            }
            else
            {
                double standardDeviation = Math.Sqrt(this.GetSquaredDifferencesSum() / (this.Count - 1));
                return standardDeviation;
            }
        }
    }
}