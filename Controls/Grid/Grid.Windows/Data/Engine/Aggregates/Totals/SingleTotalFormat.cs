namespace Telerik.Data.Core.Totals
{
    /// <summary>
    /// Formats the aggregate value based on its own value and some relative values such as row/column subtotals or grand totals.
    /// </summary>
    internal abstract class SingleTotalFormat : TotalFormat
    {
        /// <summary>
        /// Formats the value located at the <paramref name="groups"/> <see cref="Coordinate"/>. The current value could be retrieved from the <paramref name="results"/>.
        /// </summary>
        /// <param name="groups">The <see cref="Coordinate"/> for the formatted value.</param>
        /// <param name="results">The current results in the data grouping.</param>
        /// <param name="aggregateIndex">The index of the aggregate description we are formatting value for.</param>
        /// <returns>The formatted value.</returns>
        protected internal abstract AggregateValue FormatValue(Coordinate groups, IAggregateResultProvider results, int aggregateIndex);
    }
}