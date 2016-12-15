using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Telerik.Data.Core.Totals
{
    /// <summary>
    /// Formats the aggregate values based on the values for its siblings identified by <see cref="Axis"/> and <see cref="Level"/>.
    /// </summary>
    internal abstract class SiblingTotalsFormat : TotalFormat
    {
        /// <summary>
        /// Gets or sets the axis for which siblings are compared.
        /// </summary>
        public DataAxis Axis { get; set; }

        /// <summary>
        /// Gets or sets the level at which siblings are compared.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Gets a read only collection of the <see cref="TotalValue"/>s for all siblings at the <see cref="Level"/> and <see cref="Axis"/>. Based on the <see cref="TotalValue.Value"/>s the <see cref="TotalValue.FormattedValue"/> should be set.
        /// </summary>
        /// <param name="valueFormatters">A read only list of the <see cref="TotalValue"/>s for all siblings at the <see cref="Level"/> and <see cref="Axis"/>.</param>
        /// <param name="results">The <see cref="IAggregateResultProvider"/> with the current data grouping results.</param>
        protected internal abstract void FormatTotals(IReadOnlyList<TotalValue> valueFormatters, IAggregateResultProvider results);

        /// <summary>
        /// Gets the type of the variation for the groups deeper than the <see cref="Level"/>.
        /// </summary>
        /// <returns>The <see cref="RunningTotalSubGroupVariation"/> type.</returns>
        protected internal virtual RunningTotalSubGroupVariation SubVariation()
        {
            return RunningTotalSubGroupVariation.ParentAndSelfNames;
        }

        /// <inheritdoc />
        protected override void CloneCore(Cloneable source)
        {
            SiblingTotalsFormat runningTotals = source as SiblingTotalsFormat;
            if (runningTotals != null)
            {
                this.Axis = runningTotals.Axis;
                this.Level = runningTotals.Level;
            }
        }
    }
}