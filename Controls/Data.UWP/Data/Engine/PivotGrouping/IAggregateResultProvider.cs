namespace Telerik.Data.Core
{
    /// <summary>
    /// This interface provides access to the <see cref="IGroup"/>s and intermediate <see cref="AggregateValue"/>s accumulated during a data grouping process.
    /// </summary>
    internal interface IAggregateResultProvider
    {
        /// <summary>
        /// Gets a coordinate with the GrandTotal root <see cref="IGroup"/>s.
        /// </summary>
        Coordinate Root { get; }

        /// <summary>
        /// Gets the <see cref="AggregateValue"/> for the <see cref="AggregateDescriptionBase"/> at index <paramref name="aggregateIndex"/> for the row and column <see cref="IGroup"/>s defined by <paramref name="groups"/>.
        /// </summary>
        /// <param name="aggregateIndex">The index of the <see cref="AggregateDescriptionBase"/> for which an <see cref="AggregateValue"/> should be retrieved.</param>
        /// <param name="groups">A <see cref="Coordinate"/> of the <see cref="IGroup"/>s we want to retrieve value for.</param>
        AggregateValue GetAggregateResult(int aggregateIndex, Coordinate groups);
    }
}