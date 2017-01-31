using System;
using System.ComponentModel;
using System.Globalization;
using Telerik.Core;

namespace Telerik.Charting
{
    /// <summary>
    /// Defines a data point that has two values interpreted as X and Y coordinates in a cartesian coordinate system.
    /// </summary>
    /// <seealso cref="Telerik.UI.Xaml.Controls.Chart.ScatterPointSeries"/>
    /// <seealso cref="Telerik.UI.Xaml.Controls.Chart.RadCartesianChart"/>
    public class ScatterDataPoint : DataPoint
    {
        internal const string DefaultLabelFormatString = "({0},{1})";
        internal const string DefaultToolTipFormatString = "XValue: {0}{2}YValue: {1}";

        internal static readonly int XValuePropertyKey = PropertyKeys.Register(typeof(ScatterDataPoint), "XValue", ChartAreaInvalidateFlags.All);
        internal static readonly int YValuePropertyKey = PropertyKeys.Register(typeof(ScatterDataPoint), "YValue", ChartAreaInvalidateFlags.All);

        internal NumericalAxisPlotInfo xPlot;
        internal NumericalAxisPlotInfo yPlot;

        private double xValue;
        private double yValue = double.NaN;

        /// <summary>
        /// Gets or sets the value that is provided for the X-axis of the cartesian chart.
        /// </summary>
        public double XValue
        {
            get
            {
                return this.xValue;
            }
            set
            {
                this.SetValue(XValuePropertyKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that is provided for the X-axis of the cartesian chart.
        /// </summary>
        public double YValue
        {
            get
            {
                return this.yValue;
            }
            set
            {
                this.SetValue(YValuePropertyKey, value);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Method is internal")]
        internal override void OnPropertyChanged(RadPropertyEventArgs e)
        {
            // update local values first and then call base to raise the PropertyChanged event (if needed)
            if (e.Key == XValuePropertyKey)
            {
                this.xValue = (double)e.NewValue;
            }
            else if (e.Key == YValuePropertyKey)
            {
                this.yValue = (double)e.NewValue;
                this.isEmpty = DataPoint.CheckIsEmpty(this.yValue);
            }

            base.OnPropertyChanged(e);
        }

        internal override object GetValueForAxis(AxisModel axis)
        {
            if (axis.type == AxisType.First)
            {
                return this.xValue;
            }

            return this.yValue;
        }

        internal override void SetValueFromAxis(AxisModel axis, object value)
        {
            NumericalAxisPlotInfo plot = value as NumericalAxisPlotInfo;
            if (axis.type == AxisType.First)
            {
                this.xPlot = plot;
            }
            else
            {
                this.yPlot = plot;
            }
        }

        internal override object GetTooltipValue()
        {
            if (this.xPlot == null || this.yPlot == null)
            {
                return null;
            }

            return string.Format(CultureInfo.CurrentUICulture, DefaultToolTipFormatString, this.xValue, this.yValue, Environment.NewLine);
        }

        internal override object GetDefaultLabel()
        {
            if (this.xPlot == null || this.yPlot == null)
            {
                return null;
            }

            return string.Format(CultureInfo.CurrentUICulture, DefaultLabelFormatString, this.xValue, this.yValue);
        }
    }
}