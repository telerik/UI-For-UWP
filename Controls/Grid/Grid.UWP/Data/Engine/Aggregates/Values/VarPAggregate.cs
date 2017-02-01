namespace Telerik.Data.Core.Aggregates
{
    /// <summary>
    /// Represents an aggregate that estimates the variance based on the entire population.
    /// </summary>
    internal sealed class VarPAggregate : VarianceAggregateBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VarPAggregate"/> class.
        /// </summary>
        public VarPAggregate()
        {
        }

        /// <inheritdoc />
        protected override object GetValueOverride()
        {
            if (this.Count <= 0)
            {
                return AggregateValue.Error;
            }
            else
            {
                double variance = this.GetSquaredDifferencesSum() / this.Count;
                return variance;
            }
        }
    }
}