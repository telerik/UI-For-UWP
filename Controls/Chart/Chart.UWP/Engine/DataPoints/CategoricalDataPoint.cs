using System;
using System.Globalization;
using Telerik.Core;

namespace Telerik.Charting
{
    /// <summary>
    /// Defines a data point that has a single value and category associated with it.
    /// </summary>
    /// <seealso cref="Telerik.UI.Xaml.Controls.Chart.CategoricalAxis"/>
    /// <seealso cref="Telerik.UI.Xaml.Controls.Chart.CategoricalRadialAxis"/>
    /// <seealso cref="Telerik.UI.Xaml.Controls.Chart.BarSeries"/>
    /// <seealso cref="Telerik.UI.Xaml.Controls.Chart.LineSeries"/>
    /// <seealso cref="Telerik.UI.Xaml.Controls.Chart.SplineSeries"/>
    /// <seealso cref="Telerik.UI.Xaml.Controls.Chart.AreaSeries"/>
    /// <seealso cref="Telerik.UI.Xaml.Controls.Chart.SplineAreaSeries"/>
    public class CategoricalDataPoint : CategoricalDataPointBase
    {
        internal static readonly int ValuePropertyKey = PropertyKeys.Register(typeof(CategoricalDataPoint), "Value", ChartAreaInvalidateFlags.All);

        private const string DefaultToolTipFormatString = "Value: {0}{2}Category: {1}";

        private double value = double.NaN;

        /// <summary>
        /// Gets or sets the core value associated with the point.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "value", Justification = "Invalid, since value is special operator")]
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
            // update local value first and then call base to raise the PropertyChanged event (if needed)
            if (e.Key == ValuePropertyKey)
            {
                this.value = (double)e.NewValue;
                this.isEmpty = DataPoint.CheckIsEmpty(this.value);

                // check whether the default label is used
                if (this.label == null)
                {
                    this.RaisePropertyChanged(null, DataPoint.LabelPropertyKey);
                }
            }

            base.OnPropertyChanged(e);
        }

        internal override void SetValueFromAxis(AxisModel axis, object newValue)
        {
            if (axis is NumericalAxisModel)
            {
                this.numericalPlot = newValue as NumericalAxisPlotInfo;
                if (this.numericalPlot != null)
                {
                    this.isInNumericalRange = true;
                    this.isPositive = this.numericalPlot.NormalizedValue >= this.numericalPlot.NormalizedOrigin;
                    //// inverse axis with negative point value is equivalent to regular axis with positive point value
                    if (this.numericalPlot.Axis.IsInverse && !RadMath.AreClose(this.numericalPlot.NormalizedValue, this.numericalPlot.NormalizedOrigin))
                    {
                        this.isPositive ^= true;
                    }
                }
                else
                {
                    this.isInNumericalRange = false;
                    this.isPositive = false;
                }
            }
            else if (axis is CategoricalAxisModel || axis is DateTimeContinuousAxisModel)
            {
                this.categoricalPlot = newValue as CategoricalAxisPlotInfo;
                this.isInCategoricalRange = true;
            }
        }

        internal override object GetValueForAxis(AxisModel axis)
        {
            if (axis is NumericalAxisModel)
            {
                return this.value;
            }

            return this.Category;
        }

        internal override object GetTooltipValue()
        {
            if (this.categoricalPlot == null || this.numericalPlot == null)
            {
                return null;
            }

            var categoryKey = this.categoricalPlot.CategoryKey;
            return string.Format(CultureInfo.CurrentUICulture, DefaultToolTipFormatString, this.Value, categoryKey, Environment.NewLine);
        }

        internal override object GetDefaultLabel()
        {
            return this.value;
        }
    }
}