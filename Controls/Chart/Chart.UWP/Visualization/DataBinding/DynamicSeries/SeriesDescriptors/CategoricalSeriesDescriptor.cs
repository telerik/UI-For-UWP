using System;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a concrete <see cref="ChartSeriesDescriptor"/> that may be used to create all the categorical chart series variations.
    /// </summary>
    public class CategoricalSeriesDescriptor : CategoricalSeriesDescriptorBase
    {
        /// <summary>
        /// Identifies the <see cref="ValuePath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValuePathProperty =
            DependencyProperty.Register(nameof(ValuePath), typeof(string), typeof(CategoricalSeriesDescriptor), new PropertyMetadata(null));

        /// <summary>
        /// Gets the default type of series that are to be created if no TypePath and no Style properties are specified.
        /// </summary>
        public override Type DefaultType
        {
            get
            {
                return typeof(BarSeries);
            }
        }

        /// <summary>
        /// Gets or sets the name of the property that points to the Value value of the data point view model.
        /// </summary>
        public string ValuePath
        {
            get
            {
                return this.GetValue(ValuePathProperty) as string;
            }
            set
            {
                this.SetValue(ValuePathProperty, value);
            }
        }

        /// <summary>
        /// Core entry point for creating the <see cref="ChartSeries" /> type defined by this descriptor. Allows inheritors to provide custom implementation.
        /// </summary>
        /// <param name="context">The context (this is the raw data collection or the data view model) for which a <see cref="ChartSeries" /> needs to be created.</param>
        /// <exception cref="System.InvalidOperationException">The base implementation fails to create a valid <see cref="CategoricalSeries"/> instance.</exception>
        protected override ChartSeries CreateInstanceCore(object context)
        {
            CategoricalSeriesBase series = base.CreateInstanceCore(context) as CategoricalSeriesBase;
            if (series == null)
            {
                throw new InvalidOperationException("Expected Categorical series instance.");
            }

            string valuePath = this.ValuePath;
            if (!string.IsNullOrEmpty(valuePath))
            {
                series.ValueBinding = new PropertyNameDataPointBinding(valuePath);
            }

            string categoryPath = this.CategoryPath;
            if (!string.IsNullOrEmpty(categoryPath))
            {
                series.CategoryBinding = new PropertyNameDataPointBinding(categoryPath);
            }

            return series;
        }
    }
}
