using System;
using Telerik.Data.Core.Aggregates;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Estimates the standard deviation of a population based on the entire population.
    /// </summary>
    internal sealed class StdDevPAggregateFunction : StatisticalFormatAggregateFunction
    {
        /// <inheritdoc />
        public override int GetHashCode()
        {
            return 7;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is StdDevPAggregateFunction;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "StdDevP";
        }

        /// <inheritdoc />
        protected internal override AggregateValue CreateAggregate(Type dataType)
        {
            return new StdDevPAggregate();
        }

        /// <inheritdoc />
        protected override Cloneable CreateInstanceCore()
        {
            return new StdDevPAggregateFunction();
        }

        /// <inheritdoc />
        protected override void CloneCore(Cloneable source)
        {
        }
    }
}