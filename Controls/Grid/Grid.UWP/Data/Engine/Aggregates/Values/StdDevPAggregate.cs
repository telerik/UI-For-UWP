using System;

namespace Telerik.Data.Core.Aggregates
{
    /// <summary>
    /// Represents an aggregate that estimates the standard deviation of a population based on the entire population.
    /// </summary>
    internal sealed class StdDevPAggregate : VarianceAggregateBase
    {
        /// <inheritdoc />
        protected override object GetValueOverride()
        {
            if (this.Count <= 0)
            {
                return AggregateValue.Error;
            }
            else
            {
                double standardDeviation = Math.Sqrt(this.GetSquaredDifferencesSum() / this.Count);
                return standardDeviation;
            }
        }
    }
}