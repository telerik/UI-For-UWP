using System.Collections.Generic;
using Telerik.Core;
using Telerik.Geospatial;

namespace Telerik.UI.Xaml.Controls.Map
{
    /// <summary>
    /// A specific <see cref="ChoroplethRangeDistribution"/> implementation that uses linear range distribution.
    /// </summary>
    public class LinearRangeDistribution : ChoroplethRangeDistribution
    {
        private double step;
        private int rangeCount;
        private ValueRange<double> actualRange;

        internal static IEnumerable<ColorRange> BuildLinearRanges(ValueRange<double> actualRange, int count, double step)
        {
            double startValue = actualRange.minimum;

            for (int i = 0; i < count; i++)
            {
                var range = new ColorRange()
                {
                    Min = startValue,
                    Max = startValue + step,
                    Index = i
                };

                if (i == count - 1)
                {
                    range.Max = actualRange.maximum;
                }

                startValue += step;

                yield return range;
            }
        }

        /// <summary>
        /// Divides the interval defined by the <paramref name="valueRange" /> into smaller intervals (ranges) using linear distribution.
        /// </summary>
        /// <param name="valueRange">The full range that will be divided.</param>
        /// <param name="count">The count of the generated ranges.</param>
        protected internal override IEnumerable<ColorRange> BuildRanges(ValueRange<double> valueRange, int count)
        {
            double delta = valueRange.maximum - valueRange.minimum;
            if (delta <= 0)
            {
                yield break;
            }

            this.actualRange = valueRange;
            this.rangeCount = count;
            this.step = delta / count;

            foreach (var colorRange in BuildLinearRanges(this.actualRange, this.rangeCount, this.step))
            {
                yield return colorRange;
            }
        }

        /// <summary>
        /// Gets the index of the numeric range that contains the <paramref name="value" />.
        /// </summary>
        /// <param name="value">The value.</param>
        protected internal override int GetRangeIndexForValue(double value)
        {
            int index = (int)((value - this.actualRange.minimum) / this.step);
            if (index < 0 || index >= this.rangeCount)
            {
                index = this.rangeCount - 1;
            }

            return index;
        }
    }
}
