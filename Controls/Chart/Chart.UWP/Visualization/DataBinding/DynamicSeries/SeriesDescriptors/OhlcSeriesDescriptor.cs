using System;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a concrete <see cref="ChartSeriesDescriptor"/> that may be used to create financial series - <see cref="CandlestickSeries"/> and <see cref="OhlcSeries"/>.
    /// </summary>
    public class OhlcSeriesDescriptor : CategoricalSeriesDescriptorBase
    {
        /// <summary>
        /// Identifies the <see cref="OpenPath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OpenPathProperty =
            DependencyProperty.Register(nameof(OpenPath), typeof(string), typeof(OhlcSeriesDescriptor), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="HighPath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HighPathProperty =
            DependencyProperty.Register(nameof(HighPath), typeof(string), typeof(OhlcSeriesDescriptor), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="LowPath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LowPathProperty =
            DependencyProperty.Register(nameof(LowPath), typeof(string), typeof(OhlcSeriesDescriptor), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ClosePath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ClosePathProperty =
            DependencyProperty.Register(nameof(ClosePath), typeof(string), typeof(OhlcSeriesDescriptor), new PropertyMetadata(null));

        /// <summary>
        /// Gets the default type of series that are to be created if no Style is specified.
        /// </summary>
        public override Type DefaultType
        {
            get
            {
                return typeof(CandlestickSeries);
            }
        }

        /// <summary>
        /// Gets or sets the name of the property that points to the Open value of the data point view model.
        /// </summary>
        public string OpenPath
        {
            get
            {
                return this.GetValue(OpenPathProperty) as string;
            }
            set
            {
                this.SetValue(OpenPathProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the name of the property that points to the High value of the data point view model.
        /// </summary>
        public string HighPath
        {
            get
            {
                return this.GetValue(HighPathProperty) as string;
            }
            set
            {
                this.SetValue(HighPathProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the name of the property that points to the Low value of the data point view model.
        /// </summary>
        public string LowPath
        {
            get
            {
                return this.GetValue(LowPathProperty) as string;
            }
            set
            {
                this.SetValue(LowPathProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the name of the property that points to the Close value of the data point view model.
        /// </summary>
        public string ClosePath
        {
            get
            {
                return this.GetValue(ClosePathProperty) as string;
            }
            set
            {
                this.SetValue(ClosePathProperty, value);
            }
        }

        /// <summary>
        /// Core entry point for creating the <see cref="ChartSeries" /> type defined by this descriptor. Allows inheritors to provide custom implementation.
        /// </summary>
        /// <param name="context">The context (this is the raw data collection or the data view model) for which a <see cref="ChartSeries" /> needs to be created.</param>
        /// <exception cref="System.InvalidOperationException">The base implementation fails to create a valid <see cref="OhlcSeriesBase"/> instance.</exception>
        protected override ChartSeries CreateInstanceCore(object context)
        {
            OhlcSeriesBase series = base.CreateInstanceCore(context) as OhlcSeriesBase;
            if (series == null)
            {
                throw new InvalidOperationException("Expected Ohlc series instance.");
            }

            string openPath = this.OpenPath;
            if (!string.IsNullOrEmpty(openPath))
            {
                series.OpenBinding = new PropertyNameDataPointBinding(openPath);
            }

            string highPath = this.HighPath;
            if (!string.IsNullOrEmpty(highPath))
            {
                series.HighBinding = new PropertyNameDataPointBinding(highPath);
            }

            string lowPath = this.LowPath;
            if (!string.IsNullOrEmpty(lowPath))
            {
                series.LowBinding = new PropertyNameDataPointBinding(lowPath);
            }

            string closePath = this.ClosePath;
            if (!string.IsNullOrEmpty(closePath))
            {
                series.CloseBinding = new PropertyNameDataPointBinding(closePath);
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
