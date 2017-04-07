using System;
using System.Collections.Generic;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Used for advanced group filtering based on group's siblings.
    /// Can filters the groups based on count, average values, sorted values etc.
    /// </summary>
    internal abstract class SiblingGroupsFilter : GroupFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SiblingGroupsFilter" /> class.
        /// </summary>
        protected SiblingGroupsFilter()
        {
        }

        /// <summary>
        /// Filters the groups within a parent group. Can filter based on count, average values or sorted values.
        /// </summary>
        /// <param name="groups">A read only list of all siblings.</param>
        /// <param name="results">The current aggregate results.</param>
        /// <param name="axis">Identifies if the groups are in <see cref="DataAxis.Rows"/> or <see cref="DataAxis.Columns"/>.</param>
        /// <param name="level">The level of the groups.</param>
        /// <returns>A <see cref="ICollection{IGroup}"/> implementation that is used to filter the groups.</returns>
        protected internal abstract ICollection<IGroup> Filter(IReadOnlyList<object> groups, IAggregateResultProvider results, DataAxis axis, int level);
    }
}