using System;
using Telerik.Data.Core.Aggregates;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Computes the sum.
    /// </summary>
    internal sealed class SumAggregateFunction : NumericFormatAggregateFunction
    {
        /// <inheritdoc />
        public override int GetHashCode()
        {
            return 0;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is SumAggregateFunction;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "Sum";
        }

        /// <inheritdoc />
        protected internal override AggregateValue CreateAggregate(Type dataType)
        {
            return new SumAggregate();
        }

        /// <inheritdoc />
        protected override Cloneable CreateInstanceCore()
        {
            return new SumAggregateFunction();
        }

        /// <inheritdoc />
        protected override void CloneCore(Cloneable source)
        {
        }
    }
}