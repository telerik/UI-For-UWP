using System;
using System.Globalization;
using Telerik.Core;

namespace Telerik.Charting
{
    /// <summary>
    /// Class that represents a High-Low data point.
    /// </summary>
    public class RangeDataPoint : CategoricalDataPointBase
    {
        internal static readonly int HighPropertyKey = PropertyKeys.Register(typeof(RangeDataPoint), "High", ChartAreaInvalidateFlags.All);
        internal static readonly int LowPropertyKey = PropertyKeys.Register(typeof(RangeDataPoint), "Low", ChartAreaInvalidateFlags.All);

        internal new NumericalAxisRangePlotInfo numericalPlot;

        private double high;
        private double low;

        /// <summary>
        /// Gets or sets the high value associated with the point.
        /// </summary>
        public double High
        {
            get
            {
                return this.high;
            }
            set
            {
                this.SetValue(HighPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the low value associated with the point.
        /// </summary>
        public double Low
        {
            get
            {
                return this.low;
            }
            set
            {
                this.SetValue(LowPropertyKey, value);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        internal override void OnPropertyChanged(RadPropertyEventArgs e)
        {
            if (e.Key == HighPropertyKey)
            {
                this.high = (double)e.NewValue;
                this.isEmpty = false;
            }
            else if (e.Key == LowPropertyKey)
            {
                this.low = (double)e.NewValue;
                this.isEmpty = false;
            }

            base.OnPropertyChanged(e);
        }

        internal override object GetValueForAxis(AxisModel axis)
        {
            if (axis is NumericalAxisModel)
            {
                return new Range(this.low, this.high);
            }

            return this.Category;
        }

        internal override object GetTooltipValue()
        {
            string format = "Low: {0}{3}High: {1}{3}Category: {2}";

            return string.Format(CultureInfo.CurrentUICulture, format, this.low, this.high, this.categoricalPlot.CategoryKey, Environment.NewLine);
        }

        internal override void SetValueFromAxis(AxisModel axis, object value)
        {
            // ChartSeries labels rely on isPositive to flip alignment, so isPositive is set to true by default
            this.isPositive = true;

            if (axis is NumericalAxisModel)
            {
                this.numericalPlot = value as NumericalAxisRangePlotInfo;
                this.isInNumericalRange = true;
            }
            else if (axis is CategoricalAxisModel || axis is DateTimeContinuousAxisModel)
            {
                this.categoricalPlot = value as CategoricalAxisPlotInfo;
                this.isInCategoricalRange = true;
            }
        }
    }
}
