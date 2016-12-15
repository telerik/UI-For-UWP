namespace Telerik.Data.Core.Aggregates
{
    /// <summary>
    /// Represents an aggregate that estimates the variance based on a sample.
    /// </summary>
    internal sealed class VarAggregate : VarianceAggregateBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VarAggregate"/> class.
        /// </summary>
        public VarAggregate()
        {
        }

        /// <inheritdoc />
        protected override object GetValueOverride()
        {
            if (this.Count <= 1)
            {
                return AggregateValue.Error;
            }
            else
            {
                double variance = this.GetSquaredDifferencesSum() / (this.Count - 1);
                return variance;
            }
        }
    }
}