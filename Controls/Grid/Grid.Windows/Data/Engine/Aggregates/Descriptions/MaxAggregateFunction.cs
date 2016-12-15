using System;
using Telerik.Data.Core.Aggregates;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Computes the maximum.
    /// </summary>
    internal sealed class MaxAggregateFunction : NumericFormatAggregateFunction
    {
        /// <inheritdoc />
        public override int GetHashCode()
        {
            return 3;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is MaxAggregateFunction;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "Max";
        }

        /// <inheritdoc />
        protected internal override AggregateValue CreateAggregate(Type dataType)
        {
            return new MaxAggregate();
        }

        /// <inheritdoc />
        protected override Cloneable CreateInstanceCore()
        {
            return new MaxAggregateFunction();
        }

        /// <inheritdoc />
        protected override void CloneCore(Cloneable source)
        {
        }
    }
}