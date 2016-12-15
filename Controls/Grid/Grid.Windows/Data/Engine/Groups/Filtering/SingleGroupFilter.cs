using System;
using System.Collections.Generic;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Used to filter groups based on simple values and aggregate results.
    /// </summary>
    internal abstract class SingleGroupFilter : GroupFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingleGroupFilter" /> class.
        /// </summary>
        protected SingleGroupFilter()
        {
        }

        /// <summary>
        /// Identifies if a group should be filtered or not.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="results">Results for the current grouping. Could be used for totals lookup.</param>
        /// <param name="axis">Identifies if the <paramref name="group"/> is positioned in the <see cref="DataAxis.Rows"/> or <see cref="DataAxis.Columns"/>.</param>
        /// <returns>True if the group should be preserved, False if the group should be removed.</returns>
        internal abstract bool Filter(IGroup group, IAggregateResultProvider results, DataAxis axis);
    }
}