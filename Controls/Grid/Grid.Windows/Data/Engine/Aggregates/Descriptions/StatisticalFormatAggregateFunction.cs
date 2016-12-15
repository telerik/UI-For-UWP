using System;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Base class for generic statistical <see cref="AggregateFunctions"/>.
    /// It provides a basic functionality to select default string formats.
    /// </summary>
    internal abstract class StatisticalFormatAggregateFunction : AggregateFunction
    {
        /// <inheritdoc />
        public override string GetStringFormat(Type dataType, string format)
        {
            return "0.00";
        }
    }
}