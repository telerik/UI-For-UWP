using System.Collections.Generic;
using System.Linq;
using Telerik.Charting;
using Telerik.Core;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a chart series which can visualize <see cref="ScatterDataPoint"/> instances.
    /// </summary>
    [ContentProperty(Name = "DataPoints")]
    public class ScatterPointSeries : CartesianSeries
    {
        /// <summary>
        /// Identifies the <see cref="XValueBinding"/> property.
        /// </summary>
        public static readonly DependencyProperty XValueBindingProperty =
            DependencyProperty.Register(nameof(XValueBinding), typeof(DataPointBinding), typeof(ScatterPointSeries), new PropertyMetadata(null, OnXValueBindingChanged));

        /// <summary>
        /// Identifies the <see cref="YValueBinding"/> property.
        /// </summary>
        public static readonly DependencyProperty YValueBindingProperty =
            DependencyProperty.Register(nameof(YValueBinding), typeof(DataPointBinding), typeof(ScatterPointSeries), new PropertyMetadata(null, OnYValueBindingChanged));

        private ScatterSeriesModel model;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScatterPointSeries"/> class.
        /// </summary>
        public ScatterPointSeries()
        {
            this.DefaultStyleKey = typeof(ScatterPointSeries);
            this.model = new ScatterSeriesModel();
        }

        /// <summary>
        /// Gets the collection of data points associated with the series.
        /// </summary>
        public DataPointCollection<ScatterDataPoint> DataPoints
        {
            get
            {
                return this.model.DataPoints;
            }
        }

        /// <summary>
        /// Gets or sets the binding that will be used to fill the <see cref="ScatterDataPoint.XValue"/> member of the contained data points.
        /// </summary>
        public DataPointBinding XValueBinding
        {
            get
            {
                return this.GetValue(XValueBindingProperty) as DataPointBinding;
            }
            set
            {
                this.SetValue(XValueBindingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the binding that will be used to fill the <see cref="ScatterDataPoint.YValue"/> member of the contained data points.
        /// </summary>
        public DataPointBinding YValueBinding
        {
            get
            {
                return this.GetValue(YValueBindingProperty) as DataPointBinding;
            }
            set
            {
                this.SetValue(YValueBindingProperty, value);
            }
        }

        /// <summary>
        /// Gets the human-readable family of this instance. For example Area is the family for Area, SplineArea and PolarArea series.
        /// </summary>
        internal override string Family
        {
            get
            {
                return ChartPalette.PointFamily;
            }
        }

        internal override ChartSeriesModel Model
        {
            get
            {
                return this.model;
            }
        }

        internal override bool SupportsDefaultVisuals
        {
            get
            {
                return true;
            }
        }

        internal override RadSize DefaultVisualSize
        {
            get
            {
                return new RadSize(12, 12);
            }
        }

        internal override FrameworkElement SeriesVisual
        {
            get
            {
                return this.renderSurface;
            }
        }

        /// <summary>
        /// Creates the concrete data source for this instance.
        /// </summary>
        internal override ChartSeriesDataSource CreateDataSourceInstance()
        {
            return new ScatterSeriesDataSource();
        }

        internal override FrameworkElement CreateDefaultDataPointVisual(DataPoint point)
        {
            // TODO: Points cannot be set in Style due to a known problem in SL. Think of a property of the series itself that allows for user customization.
            Path visual = new Path();
            visual.Data = new EllipseGeometry() { Center = new Point(6, 6), RadiusX = 5, RadiusY = 5 };

            return visual;
        }

        internal override bool IsDefaultVisual(FrameworkElement visual)
        {
            return visual is Path;
        }

        internal override void ApplyPaletteToDefaultVisual(FrameworkElement visual, DataPoint point)
        {
            int index = this.ActualPaletteIndex;

            Brush paletteFill = this.chart.GetPaletteBrush(index, PaletteVisualPart.Fill, this.Family, point.isSelected);
            Brush paletteStroke = this.chart.GetPaletteBrush(index, PaletteVisualPart.Stroke, this.Family, point.isSelected);

            if (paletteFill != null)
            {
                visual.SetValue(Path.FillProperty, paletteFill);
            }
            else
            {
                visual.ClearValue(Path.FillProperty);
            }

            if (paletteStroke != null)
            {
                visual.SetValue(Path.StrokeProperty, paletteStroke);
            }
            else
            {
                visual.ClearValue(Path.StrokeProperty);
            }
        }

        internal override void UpdateLegendItem(FrameworkElement visual, DataPoint dataPoint)
        {
            if (visual != null && this.IsVisibleInLegend)
            {
                var item = this.LegendItems.FirstOrDefault();

                if (item != null)
                {
                    this.UpdateLegendItemProperties((Brush)visual.GetValue(Path.FillProperty), (Brush)visual.GetValue(Path.StrokeProperty));
                    if (this.LegendTitle != null)
                    {
                        item.Title = this.LegendTitle;
                    }
                }
            }
        }

        /// <inheritdoc/>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ScatterPointSeriesAutomationPeer(this);
        }

        private static void OnXValueBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScatterPointSeries presenter = d as ScatterPointSeries;
            (presenter.dataSource as ScatterSeriesDataSource).XValueBinding = e.NewValue as DataPointBinding;
        }

        private static void OnYValueBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScatterPointSeries presenter = d as ScatterPointSeries;
            (presenter.dataSource as ScatterSeriesDataSource).YValueBinding = e.NewValue as DataPointBinding;
        }
    }
}