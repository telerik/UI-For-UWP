namespace Telerik.Data.Core.Aggregates
{
    /// <summary>
    /// Represents an aggregate that counts items.
    /// </summary>
    internal sealed class CountAggregate : AggregateValue
    {
        private ulong count;

        /// <inheritdoc />
        protected override object GetValueOverride()
        {
            return this.count;
        }

        /// <inheritdoc />
        protected override void AccumulateOverride(object item)
        {
            this.count++;
        }

        /// <inheritdoc />
        protected override void MergeOverride(AggregateValue childAggregate)
        {
            this.count += ((CountAggregate)childAggregate).count;
        }
    }
}
