using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Telerik.Core;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Map
{
    /// <summary>
    /// A specific <see cref="ChoroplethRangeDistribution"/> implementation that uses logarithmic range distribution.
    /// </summary>
    public class LogarithmicRangeDistribution : ChoroplethRangeDistribution
    {
        /// <summary>
        /// Identifies the <see cref="LogarithmBase"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LogarithmBaseProperty =
            DependencyProperty.Register(nameof(LogarithmBase), typeof(double), typeof(LogarithmicRangeDistribution), new PropertyMetadata(10d, OnLogarithmBasePropertyChanged));

        private const double MinimumReciprocialNormedIntervalWidth = 10000000000;
        private readonly Collection<ColorRange> ranges;
        private double logBaseCache = 10d;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogarithmicRangeDistribution" /> class.
        /// </summary>
        public LogarithmicRangeDistribution()
        {
            this.ranges = new Collection<ColorRange>();
        }

        /// <summary>
        /// Gets or sets the base of the Logarithm function used to transform shape attributes. Defaults to 10d.
        /// </summary>
        public double LogarithmBase
        {
            get
            {
                return this.logBaseCache;
            }
            set
            {
                this.SetValue(LogarithmBaseProperty, value);
            }
        }

        // calculates (b^i - 1)/(b^n - 1)
        // 1 < b
        internal double CalculateRatio(int index, RatioCalculationContext context)
        {
            var count = context.RangeCount;
            var maxPower = context.MaxPower;
            var logBase = this.logBaseCache;

            if (count > maxPower)
            {
                if (count - index > maxPower)
                {
                    //// 1/inf
                    return 0;
                }
                else
                {
                    if (context.BaseToPowerOfCount == 0)
                    {
                        //// b^(i-n)
                        context.BaseToPowerOfCount = Math.Pow(logBase, index - count);
                    }
                    else
                    {
                        //// b^(i-n)
                        context.BaseToPowerOfCount *= logBase;
                    }

                    return context.BaseToPowerOfCount;
                }
            }
            else
            {
                context.BaseToPowerOfCurrentIndex *= logBase;

                if (context.BaseToPowerOfCount == 0)
                {
                    context.BaseToPowerOfCount = Math.Pow(logBase, count);
                }

                return (context.BaseToPowerOfCurrentIndex - 1) / (context.BaseToPowerOfCount - 1);
            }
        }

        // calculates (1 - b^i)/(1 - b^n)
        // 0 < b < 1
        // *in this case i = n-i -> reversed order of the intervals
        internal double CalculateRatioForLogBaseBetweenZeroAndOne(int index, RatioCalculationContext context)
        {
            var count = context.RangeCount;
            var maxPower = context.MaxPower;
            var logBase = this.logBaseCache;

            if (count > maxPower)
            {
                if (index > maxPower)
                {
                    //// 1/1
                    return 1;
                }
                else
                {
                    if (context.BaseToPowerOfCurrentIndex == 0)
                    {
                        //// 1-b^i
                        context.BaseToPowerOfCurrentIndex = Math.Pow(logBase, index);
                    }
                    else
                    {
                        ////b^i
                        context.BaseToPowerOfCurrentIndex *= logBase;
                    }

                    return 1 - context.BaseToPowerOfCurrentIndex;
                }
            }
            else
            {
                context.BaseToPowerOfCurrentIndex *= logBase;

                if (context.BaseToPowerOfCount == 0)
                {
                    context.BaseToPowerOfCount = Math.Pow(logBase, count);
                }

                return (1 - context.BaseToPowerOfCurrentIndex) / (1 - context.BaseToPowerOfCount);
            }
        }

        /// <summary>
        /// Implements logic for splitting the interval defined by the <paramref name="valueRange" /> into smaller intervals (ranges) using logarithmic distribution.
        /// The base of the logarithm is the <see cref="LogarithmBase"/> property.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The <paramref name="valueRange"/> is mapped to a made-up interval that is separated into <paramref name="count"/> sub-ranges as follows:
        /// b^0, b^1, ... , b^<paramref name="count"/>.
        /// The actual boundary (upper) value of a range with index i is defined from the following equality:
        /// </para>
        /// <para>
        /// actualBoundaryValue = (max - min) * ratio + min
        /// </para>
        /// <para>
        /// where
        /// </para>
        /// <para>
        /// ratio = (base^i - 1)/(base^count - 1)
        /// </para>
        /// <para>
        /// and min and max are <see cref="T:ValueRange.Minimum"/> and <see cref="T:ValueRange.Maximum"/> of the <paramref name="valueRange" />.
        /// </para>
        /// <para>
        /// This ratio is sensitive to the power values. The <see cref="CalculateMaxPowerForRangeLengthPrecision"/> method returns a value that defines the maximum power x, defined by the following inequality:
        /// </para>
        /// <para>
        /// b^x &lt;= 10^10
        /// </para>
        /// <para>
        /// This power defines the smallest possible width of a range.
        /// </para>
        /// </remarks>
        /// <param name="valueRange">The full range that will be divided.</param>
        /// <param name="count">The count of the generated ranges.</param>
        protected internal override IEnumerable<ColorRange> BuildRanges(ValueRange<double> valueRange, int count)
        {
            this.ranges.Clear();

            double delta = valueRange.maximum - valueRange.minimum;
            if (delta <= 0)
            {
                yield break;
            }

            double startValue = valueRange.minimum;
            double max = 0d;

            if (this.logBaseCache == 1)
            {
                var step = delta / count;

                foreach (var colorRange in LinearRangeDistribution.BuildLinearRanges(valueRange, count, step))
                {
                    yield return colorRange;
                }
            }
            else
            {
                var context = new RatioCalculationContext()
                {
                    MaxPower = this.CalculateMaxPowerForRangeLengthPrecision(),
                    RangeCount = count
                };

                for (int i = 1; i <= count; i++)
                {
                    if (i == count)
                    {
                        max = valueRange.maximum;
                    }
                    else
                    {
                        var ratio = this.logBaseCache > 1 ? this.CalculateRatio(i, context) : this.CalculateRatioForLogBaseBetweenZeroAndOne(i, context);
                        max = valueRange.minimum + delta * ratio;
                    }

                    var range = new ColorRange()
                    {
                        Min = startValue,
                        Max = max,
                        Index = i - 1
                    };

                    this.ranges.Add(range);

                    startValue = max;

                    yield return range;
                }
            }
        }

        /// <summary>
        /// Gets the index of the numeric range that contains the <paramref name="value" />.
        /// </summary>
        /// <param name="value">The value.</param>
        protected internal override int GetRangeIndexForValue(double value)
        {
            foreach (var range in this.ranges)
            {
                if (value <= range.Max)
                {
                    return range.Index;
                }
            }

            return -1;
        }

        /// <summary>
        /// <para>
        /// Returns a number that defines the maximum value of x, defined by the following inequality:
        /// </para>
        /// <para>
        /// b^x &lt;= 10^10
        /// </para>
        /// <para>
        /// Here b is defined by the <see cref="LogarithmBase"/> property. This power defines the smallest possible width of an interval when the ranges  are build by the <see cref="BuildRanges"/> method.
        /// </para>
        /// </summary>
        protected internal virtual double CalculateMaxPowerForRangeLengthPrecision()
        {
            // if power goes beyond this value, base^x > 10^10
            if (this.logBaseCache == 1)
            {
                return MinimumReciprocialNormedIntervalWidth;
            }
            else
            {
                return this.logBaseCache > 1 ? 10 / Math.Log10(this.logBaseCache) : -10 / Math.Log10(this.logBaseCache);
            }
        }

        private static void OnLogarithmBasePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var logBase = (double)e.NewValue;
            var distribution = d as LogarithmicRangeDistribution;

            if (logBase <= 0)
            {
                distribution.LogarithmBase = 1;
                return;
            }

            distribution.logBaseCache = logBase;
        }

        internal class RatioCalculationContext
        {
            public RatioCalculationContext()
            {
                this.BaseToPowerOfCount = 0;
                this.BaseToPowerOfCurrentIndex = 1;
            }

            public double BaseToPowerOfCount { get; set; }

            public double BaseToPowerOfCurrentIndex { get; set; }

            public double MaxPower { get; set; }

            public int RangeCount { get; set; }
        }
    }
}