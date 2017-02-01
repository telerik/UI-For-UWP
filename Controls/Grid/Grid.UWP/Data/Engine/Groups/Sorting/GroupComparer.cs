using System.ComponentModel;

namespace Telerik.Data.Core
{
    /// <summary>
    /// A base class for <see cref="IGroup"/> comparers.
    /// </summary>
    internal abstract class GroupComparer : SettingsNode
    {
        /// <summary>
        /// Compares two <see cref="IGroup"/>s based on the current aggregate results.
        /// </summary>
        /// <param name="results">The current aggregate results.</param>
        /// <param name="left">The first <see cref="IGroup"/> to compare.</param>
        /// <param name="right">The second <see cref="IGroup"/> to compare.</param>
        /// <param name="axis">Identifies if the groups are in <see cref="DataAxis.Rows"/> or <see cref="DataAxis.Columns"/>.</param>
        /// <returns>
        /// A signed integer that indicates the relative values of x and y, as shown in the following table.
        /// <para>Value Meaning Less than zero x is less than y.</para>
        /// <para>Zero x equals y.</para>
        /// <para>Greater than zero x is greater than y.</para>
        /// </returns>
        public abstract int CompareGroups(IAggregateResultProvider results, IGroup left, IGroup right, DataAxis axis);
    }
}