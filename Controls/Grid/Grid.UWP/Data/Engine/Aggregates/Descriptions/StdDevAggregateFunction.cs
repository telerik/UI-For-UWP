using System;
using Telerik.Data.Core.Aggregates;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Estimates the standard deviation of a population based on a sample.
    /// </summary>
    internal sealed class StdDevAggregateFunction : StatisticalFormatAggregateFunction
    {
        /// <inheritdoc />
        public override int GetHashCode()
        {
            return 6;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is StdDevAggregateFunction;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "StdDev";
        }

        /// <inheritdoc />
        protected internal override AggregateValue CreateAggregate(Type dataType)
        {
            return new StdDevAggregate();
        }

        /// <inheritdoc />
        protected override Cloneable CreateInstanceCore()
        {
            return new StdDevAggregateFunction();
        }

        /// <inheritdoc />
        protected override void CloneCore(Cloneable source)
        {
        }
    }
}