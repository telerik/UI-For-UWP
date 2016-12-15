using System;

namespace Telerik.Data.Core
{
    ///// <summary>
    ///// Base class for generic <see cref="AggregateFunctions"/> that preserve the meaning of the underlying data.
    ///// It provides a basic functionality to select default string formats.
    ///// </summary>

    /// <summary>
    /// Base class for generic AggregateFunctions that preserve the meaning of the underlying data.
    /// It provides a basic functionality to select default string formats.
    /// </summary>
    internal abstract class NumericFormatAggregateFunction : AggregateFunction
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
    }
}