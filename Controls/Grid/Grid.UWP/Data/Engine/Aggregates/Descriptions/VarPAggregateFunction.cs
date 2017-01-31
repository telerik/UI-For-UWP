using System;
using Telerik.Data.Core.Aggregates;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Estimates the variance based on the entire population.
    /// </summary>
    internal sealed class VarPAggregateFunction : StatisticalFormatAggregateFunction
    {
        /// <inheritdoc />
        public override int GetHashCode()
        {
            return 9;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is VarPAggregateFunction;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "VarP";
        }

        /// <inheritdoc />
        protected internal override AggregateValue CreateAggregate(Type dataType)
        {
            return new VarPAggregate();
        }

        /// <inheritdoc />
        protected override Cloneable CreateInstanceCore()
        {
            return new VarPAggregateFunction();
        }

        /// <inheritdoc />
        protected override void CloneCore(Cloneable source)
        {
        }
    }
}