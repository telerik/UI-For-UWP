using System.Collections.Generic;

namespace Telerik.Data.Core
{
    /// <summary>
    /// This interface provides access to the <see cref="IGroup"/>s and <see cref="AggregateValue"/>s accumulated during a data grouping process.
    /// </summary>
    internal interface IDataResults : IAggregateResultProvider
    {
        // TODO:
        // These GroupDescriptions here should have been with read only properties...
        // That was one of the reasons at the beginning all descriptions had defined protected methods that were used for grouping.
        // The original description's public values could have been copied in a read only mode here or these could have been read only wrappers over the original descriptions.
        // Further more a 'ResultGroupDescription' could have had an 'AllKeys' property rather than the current 'GetUniqueKeys(axis, index)' method.

        /// <summary>
        /// A read-only collection of the <see cref="GroupDescription"/>s used to generate the <see cref="IAggregateResultProvider.Root"/>'s <see cref="Coordinate.RowGroup"/> <see cref="IGroup"/>s tree.
        /// </summary>
        IReadOnlyList<GroupDescription> RowGroupDescriptions { get; }

        /// <summary>
        /// A read-only collection of the <see cref="GroupDescription"/>s used to generate the <see cref="IAggregateResultProvider.Root"/>'s <see cref="Coordinate.ColumnGroup"/> <see cref="IGroup"/>s tree.
        /// </summary>
        IReadOnlyList<GroupDescription> ColumnGroupDescriptions { get; }

        /// <summary>
        /// A read-only collection of the <see cref="IAggregateDescription"/> used to generate the the available <see cref="AggregateValue"/>s for the <see cref="GetAggregateResult"/>.
        /// </summary>
        IReadOnlyList<IAggregateDescription> AggregateDescriptions { get; }

        /// <summary>
        /// A read-only collection of the <see cref="FilterDescription"/> used to filter the items.
        /// </summary>
        IReadOnlyList<FilterDescription> FilterDescriptions { get; }

        /// <summary>
        /// Gets the AggregateValue for the <see cref="AggregateDescriptionBase"/> at index <paramref name="aggregateIndex"/> for the <paramref name="row"/> and <paramref name="column"/> <see cref="IGroup"/>s.
        /// </summary>
        /// <param name="aggregateIndex">The index of the <see cref="AggregateDescriptionBase"/> for which a value is retrieved.</param>
        /// <param name="row">An <see cref="IGroup"/> from the <see cref="IAggregateResultProvider.Root"/>'s <see cref="Coordinate.RowGroup"/> tree.</param>
        /// <param name="column">An <see cref="IGroup"/> from the <see cref="IAggregateResultProvider.Root"/>'s <see cref="Coordinate.ColumnGroup"/> tree.</param>
        /// <returns>Null or <see cref="AggregateValue"/> if it is available.</returns>
        AggregateValue GetAggregateResult(int aggregateIndex, IGroup row, IGroup column);

        /// <summary>
        /// Returns the unique keys generated for the GroupDescription located at <see ref="axis" /> at index <see ref="index" />.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="index">The GroupDescription index.</param>
        /// <returns>The unique keys.</returns>
        IEnumerable<object> GetUniqueKeys(DataAxis axis, int index);

        /// <summary>
        /// Returns the unique items generated for FilterDescription located at index <paramref name="filterIndex"/>.
        /// </summary>
        /// <param name="filterIndex">The FilterDescription index.</param>
        /// <returns>The unique items.</returns>
        IEnumerable<object> GetUniqueFilterItems(int filterIndex);
    }
}
