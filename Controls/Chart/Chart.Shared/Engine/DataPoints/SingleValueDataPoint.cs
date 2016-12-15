using System;
using System.ComponentModel;
using System.Globalization;
using Telerik.Core;

namespace Telerik.Charting
{
    /// <summary>
    /// Base class for <see cref="DataPoint"/> that has a single <see cref="Value"/> property, that can be used when the point is visualized in a chart.
    /// </summary>
    public abstract class SingleValueDataPoint : DataPoint
    {
        internal static readonly int ValuePropertyKey = PropertyKeys.Register(typeof(SingleValueDataPoint), "Value", ChartAreaInvalidateFlags.All);

        private const string DefaultToolTipFormatString = "Value: {0}";

        private double value = double.NaN;

        /// <summary>
        /// Gets or sets the value associated with the point.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "value", Justification = "Value is reserved oeprator word.")]
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

        internal override object GetValueForAxis(AxisModel axis)
        {
            if (axis is IContinuousAxisModel)
            {
                return this.value;
            }

            return null;
        }

        internal override object GetTooltipValue()
        {
            return string.Format(CultureInfo.CurrentUICulture, DefaultToolTipFormatString, this.value);
        }

        internal override object GetDefaultLabel()
        {
            return this.value;
        }
    }
}
