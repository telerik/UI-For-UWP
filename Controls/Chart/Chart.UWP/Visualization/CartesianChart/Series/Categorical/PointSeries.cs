using System.Collections.Generic;
using System.Linq;
using Telerik.Charting;
using Telerik.Core;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a chart series that plot its points using ellipses.
    /// </summary>
    public class PointSeries : CategoricalSeriesBase
    {
        /// <summary>
        /// Identifies the <see cref="PointSize"/> property.
        /// </summary>     
        public static readonly DependencyProperty PointSizeProperty =
            DependencyProperty.Register(nameof(PointSize), typeof(Size), typeof(PointSeries), new PropertyMetadata(new Size(12, 12), OnPointSizeChanged));

        /// <summary>
        /// Identifies the <see cref="PaletteMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PaletteModeProperty =
          DependencyProperty.Register(nameof(PaletteMode), typeof(SeriesPaletteMode), typeof(PointSeries), new PropertyMetadata(SeriesPaletteMode.Series, OnPaletteModeChanged));

        /// <summary>
        /// Identifies the <see cref="LegendTitleBinding"/> property.
        /// </summary>
        public static readonly DependencyProperty LegendTitleBindingProperty =
            DependencyProperty.Register(nameof(LegendTitleBinding), typeof(DataPointBinding), typeof(PointSeries), new PropertyMetadata(null, OnLegendTitleBindingChanged));

        private Dictionary<DataPoint, LegendItem> legendItems;
        private SeriesPaletteMode paletteModeCache = SeriesPaletteMode.Series;
        private PointSeriesModel model;
        private RadSize defaultVisualSizeCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="PointSeries" /> class.
        /// </summary>
        public PointSeries()
        {
            this.DefaultStyleKey = typeof(PointSeries);
            this.model = new PointSeriesModel();
            this.defaultVisualSizeCache = new RadSize(this.PointSize.Width, this.PointSize.Height);
        }

        /// <summary>
        /// Gets or sets the <see cref="Windows.Foundation.Size(double, double)"/> of the points. This property will be ignored if <see cref="PointTemplateSeries.PointTemplate"/> property is set.
        /// </summary>
        public Size PointSize
        {
            get
            {
                return (Size)this.GetValue(PointSizeProperty);
            }
            set
            {
                this.SetValue(PointSizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the target type of the chart palette, specified to the owning Chart instance. Defaults to PaletteTargetType.Series.
        /// </summary>
        public SeriesPaletteMode PaletteMode
        {
            get
            {
                return (SeriesPaletteMode)this.GetValue(PaletteModeProperty);
            }
            set
            {
                this.SetValue(PaletteModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the binding that will be used by any attached legend to display legend item title.
        /// </summary>
        public DataPointBinding LegendTitleBinding
        {
            get
            {
                return this.GetValue(LegendTitleBindingProperty) as DataPointBinding;
            }
            set
            {
                this.SetValue(LegendTitleBindingProperty, value);
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

        internal override Charting.ChartSeriesModel Model
        {
            get
            {
                return this.model;
            }
        }

        internal override RadSize DefaultVisualSize
        {
            get
            {
                return this.defaultVisualSizeCache;
            }
        }

        internal override bool SupportsDefaultVisuals
        {
            get
            {
                return true;
            }
        }
        internal override IEnumerable<LegendItem> LegendItems
        {
            get
            {
                if (this.paletteModeCache == SeriesPaletteMode.Series)
                {
                    return base.LegendItems;
                }

                if (this.legendItems == null)
                {
                    this.legendItems = new Dictionary<DataPoint, LegendItem>();
                    this.RemoveOldLegendItems();
                }

                return this.legendItems.Values;
            }
        }

        internal override FrameworkElement SeriesVisual
        {
            get
            {
                return this.renderSurface;
            }
        }

        internal override int GetPaletteIndexForPoint(DataPoint point)
        {
            return this.paletteModeCache == SeriesPaletteMode.Series ? this.ActualPaletteIndex : point.CollectionIndex;
        }

        internal override void ApplyPaletteToDefaultVisual(FrameworkElement visual, DataPoint point)
        {
            int index = this.paletteModeCache == SeriesPaletteMode.Series ? this.ActualPaletteIndex : point.CollectionIndex;

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

        internal override void UpdateLegendItems()
        {
            if (this.paletteModeCache == SeriesPaletteMode.Series)
            {
                base.UpdateLegendItems();
            }
            else
            {
                var oldLegendItems = new Dictionary<DataPoint, LegendItem>(this.legendItems);

                foreach (FrameworkElement visual in this.RealizedDefaultVisualElements)
                {
                    if (!this.IsDefaultVisual(visual) || visual.Visibility == Visibility.Collapsed /* recycled item */)
                    {
                        continue;
                    }

                    DataPoint point = visual.Tag as DataPoint;

                    this.UpdateLegendItem(visual, point);

                    oldLegendItems.Remove(point);
                }

                foreach (var item in oldLegendItems)
                {
                    // TODO refactor this.
                    this.Chart.LegendInfosInternal.Remove(item.Value);
                    this.legendItems.Remove(item.Key);
                }

                // TODO: optimize this.
                this.RemoveOldLegendItems();
            }
        }

        internal override void SetDynamicLegendTitle(string titlePath, string extractedValue)
        {
            if (this.paletteModeCache == SeriesPaletteMode.Series)
            {
                base.SetDynamicLegendTitle(titlePath, extractedValue);
            }
            else
            {
                this.LegendTitleBinding = new PropertyNameDataPointBinding(titlePath);
            }
        }

        internal override void UpdateLegendItem(FrameworkElement visual, DataPoint dataPoint)
        {
            if (visual != null && this.IsVisibleInLegend)
            {
                LegendItem item;
                bool titleUpdateNeeded = false;

                if (this.paletteModeCache == SeriesPaletteMode.DataPoint)
                {
                    if (!this.legendItems.TryGetValue(dataPoint, out item))
                    {
                        item = new LegendItem();
                        titleUpdateNeeded = true;
                        this.legendItems.Add(dataPoint, item);

                        // TODO refactor this.
                        this.Chart.LegendInfosInternal.Add(item);
                    }

                    PointSeries.UpdateLegendItemProperties(item, (Brush)visual.GetValue(Path.FillProperty), (Brush)visual.GetValue(Path.StrokeProperty));
                }
                else
                {
                    item = this.LegendItems.FirstOrDefault();
                    this.UpdateLegendItemProperties((Brush)visual.GetValue(Path.FillProperty), (Brush)visual.GetValue(Path.StrokeProperty));
                }

                if (item != null && titleUpdateNeeded)
                {
                    if (this.LegendTitleBinding != null)
                    {
                        this.UpdateLegendTitle(dataPoint, item);
                    }
                    else if (this.LegendTitle != null)
                    {
                        item.Title = this.LegendTitle;
                    }
                }
            }
        }

        internal override void OnLegendTitleChanged(string oldValue, string newValue)
        {
            if (this.LegendTitleBinding == null)
            {
                foreach (var item in this.LegendItems)
                {
                    item.Title = newValue;
                }
            }
        }

        internal override FrameworkElement CreateDefaultDataPointVisual(DataPoint point)
        {
            Path visual = new Path();
            visual.Data = new EllipseGeometry() { Center = new Point(this.PointSize.Width / 2, this.PointSize.Width / 2), RadiusX = this.PointSize.Height / 2 - 1, RadiusY = this.PointSize.Height / 2 - 1 };

            return visual;
        }

        internal override bool IsDefaultVisual(FrameworkElement visual)
        {
            return visual is Path;
        }

        private static void OnPaletteModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue.Equals(e.NewValue))
            {
                return;
            }

            PointSeries series = d as PointSeries;
            series.paletteModeCache = (SeriesPaletteMode)e.NewValue;

            if (series.paletteModeCache == SeriesPaletteMode.Series)
            {
                foreach (var item in series.legendItems)
                {
                    series.Chart.LegendInfosInternal.Remove(item.Value);
                }
                series.legendItems = null;
            }
            else
            {
                series.legendItems = new Dictionary<DataPoint, LegendItem>();
            }

            series.InvalidatePalette();
        }

        private static void OnLegendTitleBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var series = (PointSeries)d;

            if (series.paletteModeCache == SeriesPaletteMode.DataPoint)
            {
                foreach (var pair in series.legendItems)
                {
                    series.UpdateLegendTitle(pair.Key, pair.Value);
                }
            }
        }
        private static void OnPointSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PointSeries presenter = d as PointSeries;
            presenter.defaultVisualSizeCache.Width = presenter.PointSize.Width;
            presenter.defaultVisualSizeCache.Height = presenter.PointSize.Height;
            presenter.realizedDataPointPresenters.Clear();
            if (presenter.renderSurface != null)
            {
                presenter.renderSurface.Children.Clear();
            }

            foreach (var dataPoint in presenter.DataPoints)
            {
                dataPoint.desiredSize = RadSize.Invalid;
            }

            presenter.InvalidateCore();
        }

        private void UpdateLegendTitle(DataPoint dataPoint, LegendItem legendItem)
        {
            object data = null;
            if (this.LegendTitleBinding != null)
            {
                data = this.LegendTitleBinding.GetValue(dataPoint.dataItem);
            }

            if (data != null)
            {
                legendItem.Title = data.ToString();
            }

            if (legendItem.Title == null || data == null)
            {
                legendItem.Title = dataPoint.Label.ToString();
            }
        }

        private void RemoveOldLegendItems()
        {
            if (this.Chart == null || this.Chart.LegendInfosInternal == null)
            {
                return;
            }
            foreach (var item in base.LegendItems)
            {
                this.Chart.LegendInfosInternal.Remove(item);
            }
        }
    }
}
