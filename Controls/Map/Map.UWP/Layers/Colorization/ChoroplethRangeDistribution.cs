using System.Collections.Generic;
using Telerik.Core;
using Telerik.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Xaml.Controls.Map
{
    /// <summary>
    /// Encapsulates the logic behind range distribution of shape attributes, used by a <see cref="ChoroplethColorizer"/> instance.
    /// </summary>
    public abstract class ChoroplethRangeDistribution : RadDependencyObject
    {
        /// <summary>
        /// Implements logic for dividing the interval defined by the <paramref name="valueRange"/> into smaller intervals (ranges).
        /// </summary>
        /// <param name="valueRange">The full range that will be divided.</param>
        /// <param name="count">The count of the generated ranges.</param>
        protected internal abstract IEnumerable<ColorRange> BuildRanges(ValueRange<double> valueRange, int count);

        /// <summary>
        /// Gets the index of the numeric range that contains the <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        protected internal abstract int GetRangeIndexForValue(double value);
    }
}