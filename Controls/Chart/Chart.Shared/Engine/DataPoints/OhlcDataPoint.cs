using System;
using System.Globalization;
using Telerik.Core;

namespace Telerik.Charting
{
    /// <summary>
    /// Defines a data point that represents a data point that can be plotted on an open-high-low-close chart.
    /// </summary>
    /// <seealso cref="Telerik.UI.Xaml.Controls.Chart.OhlcSeries"/>
    public class OhlcDataPoint : CategoricalDataPointBase
    {
        internal static readonly int HighPropertyKey = PropertyKeys.Register(typeof(OhlcDataPoint), "High", ChartAreaInvalidateFlags.All);
        internal static readonly int LowPropertyKey = PropertyKeys.Register(typeof(OhlcDataPoint), "Low", ChartAreaInvalidateFlags.All);
        internal static readonly int OpenPropertyKey = PropertyKeys.Register(typeof(OhlcDataPoint), "Open", ChartAreaInvalidateFlags.All);
        internal static readonly int ClosePropertyKey = PropertyKeys.Register(typeof(OhlcDataPoint), "Close", ChartAreaInvalidateFlags.All);

        internal new NumericalAxisOhlcPlotInfo numericalPlot;

        private double high;
        private double low;
        private double open;
        private double close;

        /// <summary>
        /// Initializes a new instance of the <see cref="OhlcDataPoint" /> class.
        /// </summary>
        public OhlcDataPoint()
        {
            this.isEmpty = false;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is rising (Bullish).
        /// </summary>
        public bool IsRising
        {
            get
            {
                return this.open < this.close;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is falling (Bearish).
        /// </summary>
        public bool IsFalling
        {
            get
            {
                return this.open > this.close;
            }
        }

        /// <summary>
        /// Gets or sets the high associated with the point.
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
        /// Gets or sets the low associated with the point.
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

        /// <summary>
        /// Gets or sets the open associated with the point.
        /// </summary>
        public double Open
        {
            get
            {
                return this.open;
            }
            set
            {
                this.SetValue(OpenPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the close associated with the point.
        /// </summary>
        public double Close
        {
            get
            {
                return this.close;
            }
            set
            {
                this.SetValue(ClosePropertyKey, value);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Method is internal")]
        internal override void OnPropertyChanged(RadPropertyEventArgs e)
        {
            // update local value first and then call base to raise the PropertyChanged event (if needed)
            if (e.Key == HighPropertyKey)
            {
                this.high = (double)e.NewValue;
            }
            else if (e.Key == LowPropertyKey)
            {
                this.low = (double)e.NewValue;
            }
            else if (e.Key == OpenPropertyKey)
            {
                this.open = (double)e.NewValue;
            }
            else if (e.Key == ClosePropertyKey)
            {
                this.close = (double)e.NewValue;
            }

            base.OnPropertyChanged(e);
        }

        internal override object GetValueForAxis(AxisModel axis)
        {
            if (axis is NumericalAxisModel)
            {
                return new Ohlc(this.high, this.low, this.open, this.close);
            }

            return this.Category;
        }

        internal override object GetTooltipValue()
        {
            string format = "High: {0}{4}Low: {1}{4}Open: {2}{4}Close: {3}";
            return string.Format(CultureInfo.CurrentUICulture, format, this.high, this.low, this.open, this.close, Environment.NewLine);
        }

        internal override void SetValueFromAxis(AxisModel axis, object value)
        {
            // ChartSeries labels rely on isPositive to flip alignment, so isPositive is set to true by default
            this.isPositive = true;

            if (axis is NumericalAxisModel)
            {
                this.numericalPlot = value as NumericalAxisOhlcPlotInfo;
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
