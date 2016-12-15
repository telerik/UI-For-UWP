using System;
using Telerik.Data.Core.Aggregates;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Counts items.
    /// </summary>
    internal sealed class CountAggregateFunction : AggregateFunction
    {
        /// <inheritdoc />
        public override string GetStringFormat(Type dataType, string format)
        {
            return "G";
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return 1;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is CountAggregateFunction;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "Count";
        }

        /// <inheritdoc />
        protected internal override AggregateValue CreateAggregate(Type dataType)
        {
            return new CountAggregate();
        }

        /// <inheritdoc />
        protected override Cloneable CreateInstanceCore()
        {
            return new CountAggregateFunction();
        }

        /// <inheritdoc />
        protected override void CloneCore(Cloneable source)
        {
        }
    }
}