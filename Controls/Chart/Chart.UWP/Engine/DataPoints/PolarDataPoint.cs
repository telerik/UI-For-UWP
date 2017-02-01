using System;
using System.Globalization;
using Telerik.Core;

namespace Telerik.Charting
{
    /// <summary>
    /// Represents a data point that can be plotted on polar charts.
    /// </summary>
    /// <seealso cref="Telerik.UI.Xaml.Controls.Chart.RadPolarChart"/>
    /// <seealso cref="Telerik.UI.Xaml.Controls.Chart.PolarAxis"/>
    /// <seealso cref="Telerik.UI.Xaml.Controls.Chart.PolarLineSeries"/>
    /// <seealso cref="Telerik.UI.Xaml.Controls.Chart.PolarSplineSeries"/>
    /// <seealso cref="Telerik.UI.Xaml.Controls.Chart.PolarAreaSeries"/>
    /// <seealso cref="Telerik.UI.Xaml.Controls.Chart.PolarSplineAreaSeries"/>
    public class PolarDataPoint : DataPoint
    {
        internal static readonly int AnglePropertyKey = PropertyKeys.Register(typeof(PolarDataPoint), "Angle", ChartAreaInvalidateFlags.All);
        internal static readonly int ValuePropertyKey = PropertyKeys.Register(typeof(PolarDataPoint), "Value", ChartAreaInvalidateFlags.All);

        internal NumericalAxisPlotInfo anglePlot;
        internal NumericalAxisPlotInfo valuePlot;

        private const string DefaultToolTipFormatString = "Value: {0}{2}Angle: {1}";

        private double angle;
        private double value = double.NaN;

        /// <summary>
        /// Gets or sets the angle coordinate of the data point.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "value", Justification = "Value is reserved operator.")]
        public double Angle
        {
            get
            {
                return this.angle;
            }
            set
            {
                this.SetValue(AnglePropertyKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the distance of the data point from the center of the coordinate system (radius).
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "value", Justification = "Value is reserved operator.")]
        public double Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.SetValue(ValuePropertyKey, value);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Method is internal")]
        internal override void OnPropertyChanged(RadPropertyEventArgs e)
        {
            if (e.Key == AnglePropertyKey)
            {
                this.angle = (double)e.NewValue;
            }
            else if (e.Key == ValuePropertyKey)
            {
                this.value = (double)e.NewValue;
                this.isEmpty = DataPoint.CheckIsEmpty(this.value);
            }

            base.OnPropertyChanged(e);
        }

        internal override object GetValueForAxis(AxisModel axis)
        {
            if (axis.type == AxisType.First)
            {
                return this.value;
            }

            if (this.value >= 0)
            {
                return this.angle;
            }

            // consider negative values
            return this.angle + 180;
        }

        internal override object GetTooltipValue()
        {
            if (this.valuePlot == null || this.anglePlot == null)
            {
                return null;
            }

            return string.Format(CultureInfo.CurrentUICulture, DefaultToolTipFormatString, this.Value, this.Angle, Environment.NewLine);
        }

        internal override object GetDefaultLabel()
        {
            return this.value;
        }

        internal override void SetValueFromAxis(AxisModel axis, object newValue)
        {
            NumericalAxisPlotInfo plotInfo = newValue as NumericalAxisPlotInfo;

            if (axis.type == AxisType.First)
            {
                this.valuePlot = plotInfo;
            }
            else
            {
                this.anglePlot = plotInfo;
            }
        }
    }
}
