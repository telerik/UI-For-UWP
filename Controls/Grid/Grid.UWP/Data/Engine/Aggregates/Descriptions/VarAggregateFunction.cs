using System;
using Telerik.Data.Core.Aggregates;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Estimates the variance based on a sample.
    /// </summary>
    internal sealed class VarAggregateFunction : StatisticalFormatAggregateFunction
    {
        /// <inheritdoc />
        public override int GetHashCode()
        {
            return 8;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is VarAggregateFunction;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "Var";
        }

        /// <inheritdoc />
        protected internal override AggregateValue CreateAggregate(Type dataType)
        {
            return new VarAggregate();
        }

        /// <inheritdoc />
        protected override Cloneable CreateInstanceCore()
        {
            return new VarAggregateFunction();
        }

        /// <inheritdoc />
        protected override void CloneCore(Cloneable source)
        {
        }
    }
}