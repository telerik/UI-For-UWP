using System;
using Telerik.Data.Core.Aggregates;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Computes the average.
    /// </summary>
    internal sealed class AverageAggregateFunction : AggregateFunction
    {
        /// <inheritdoc />
        public override string GetStringFormat(Type dataType, string format)
        {
            if (format == null)
            {
                return "0.00";
            }

            return format;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return 2;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is AverageAggregateFunction;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "Average";
        }

        /// <inheritdoc />
        protected internal override AggregateValue CreateAggregate(Type dataType)
        {
            return new AverageAggregate();
        }

        /// <inheritdoc />
        protected override Cloneable CreateInstanceCore()
        {
            return new AverageAggregateFunction();
        }

        /// <inheritdoc />
        protected override void CloneCore(Cloneable source)
        {
        }
    }
}