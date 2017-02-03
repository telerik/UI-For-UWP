using System;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a concrete <see cref="ChartSeriesDescriptor"/> that may be used to create all the scatter chart series variations.
    /// </summary>
    public class ScatterSeriesDescriptor : ChartSeriesDescriptor
    {
        /// <summary>
        /// Identifies the <see cref="XValuePath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty XValuePathProperty =
            DependencyProperty.Register(nameof(XValuePath), typeof(string), typeof(ScatterSeriesDescriptor), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="YValuePath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty YValuePathProperty =
            DependencyProperty.Register(nameof(YValuePath), typeof(string), typeof(ScatterSeriesDescriptor), new PropertyMetadata(null));

        /// <summary>
        /// Gets the default type of series that are to be created if no TypePath and no Style properties are specified.
        /// </summary>
        public override Type DefaultType
        {
            get
            {
                return typeof(ScatterPointSeries);
            }
        }

        /// <summary>
        /// Gets or sets the name of the property that points to the XValue value of the data point view model.
        /// </summary>
        public string XValuePath
        {
            get
            {
                return this.GetValue(XValuePathProperty) as string;
            }
            set
            {
                this.SetValue(XValuePathProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the name of the property that points to the YValue value of the data point view model.
        /// </summary>
        public string YValuePath
        {
            get
            {
                return this.GetValue(YValuePathProperty) as string;
            }
            set
            {
                this.SetValue(YValuePathProperty, value);
            }
        }

        /// <summary>
        /// Core entry point for creating the <see cref="ChartSeries" /> type defined by this descriptor. Allows inheritors to provide custom implementation.
        /// </summary>
        /// <param name="context">The context (this is the raw data collection or the data view model) for which a <see cref="ChartSeries" /> needs to be created.</param>
        /// <exception cref="System.InvalidOperationException">The base implementation fails to create a valid <see cref="ScatterPointSeries"/> instance.</exception>
        protected override ChartSeries CreateInstanceCore(object context)
        {
            ScatterPointSeries series = base.CreateInstanceCore(context) as ScatterPointSeries;
            if (series == null)
            {
                throw new InvalidOperationException("Expected Scatter series instance.");
            }

            string xValuePath = this.XValuePath;
            if (!string.IsNullOrEmpty(xValuePath))
            {
                series.XValueBinding = new PropertyNameDataPointBinding(xValuePath);
            }

            string yValuePath = this.YValuePath;
            if (!string.IsNullOrEmpty(yValuePath))
            {
                series.YValueBinding = new PropertyNameDataPointBinding(yValuePath);
            }

            return series;
        }
    }
}
