using System;
using Telerik.Data.Core.Aggregates;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Computes the product.
    /// </summary>
    internal sealed class ProductAggregateFunction : AggregateFunction
    {
        /// <inheritdoc />
        public override string GetStringFormat(Type dataType, string format)
        {
            return "0.00 E+00";
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return 5;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is ProductAggregateFunction;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "Product";
        }

        /// <inheritdoc />
        protected internal override AggregateValue CreateAggregate(Type dataType)
        {
            return new ProductAggregate();
        }

        /// <inheritdoc />
        protected override Cloneable CreateInstanceCore()
        {
            return new ProductAggregateFunction();
        }

        /// <inheritdoc />
        protected override void CloneCore(Cloneable source)
        {
        }
    }
}