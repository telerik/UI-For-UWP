using System;
using Telerik.Data.Core.Aggregates;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Computes the minimum.
    /// </summary>
    internal sealed class MinAggregateFunction : NumericFormatAggregateFunction
    {
        /// <inheritdoc />
        public override int GetHashCode()
        {
            return 4;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is MinAggregateFunction;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "Min";
        }

        /// <inheritdoc />
        protected internal override AggregateValue CreateAggregate(Type dataType)
        {
            return new MinAggregate();
        }

        /// <inheritdoc />
        protected override Cloneable CreateInstanceCore()
        {
            return new MinAggregateFunction();
        }

        /// <inheritdoc />
        protected override void CloneCore(Cloneable source)
        {
        }
    }
}