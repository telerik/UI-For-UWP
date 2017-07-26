using System.Collections.Generic;
using System.Linq;
using Telerik.Charting;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a chart range bar series.
    /// </summary>
    public class RangeBarSeries : RangeSeriesBase
    {
        /// <summary>
        /// Identifies the <see cref="CombineMode"/> property.
        /// </summary>
        public static readonly DependencyProperty CombineModeProperty =
            DependencyProperty.Register(nameof(CombineMode), typeof(ChartSeriesCombineMode), typeof(RangeBarSeries), new PropertyMetadata(ChartSeriesCombineMode.Cluster, OnCombineModePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="PaletteMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PaletteModeProperty =
            DependencyProperty.Register(nameof(PaletteMode), typeof(SeriesPaletteMode), typeof(RangeBarSeries), new PropertyMetadata(SeriesPaletteMode.Series, OnPaletteModeChanged));

        /// <summary>
        /// Identifies the <see cref="LegendTitleBinding"/> property.
        /// </summary>
        public static readonly DependencyProperty LegendTitleBindingProperty =
            DependencyProperty.Register(nameof(LegendTitleBinding), typeof(DataPointBinding), typeof(RangeBarSeries), new PropertyMetadata(null, OnLegendTitleBindingChanged));

        private SeriesPaletteMode paletteModeCache = SeriesPaletteMode.Series;
        private Dictionary<DataPoint, LegendItem> legendItems;

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeBarSeries" /> class.
        /// </summary>
        public RangeBarSeries()
        {
            this.DefaultStyleKey = typeof(RangeBarSeries);
        }

        /// <summary>
        /// Gets or sets the combination mode to be used when data points are plotted.
        /// </summary>
        public ChartSeriesCombineMode CombineMode
        {
            get
            {
                return (this.model as RangeBarSeriesModel).CombineMode;
            }
            set
            {
                this.SetValue(CombineModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the target type of the chart palette, specified to the owning Chart instance. Defaults to PaletteTargetType.Series.
        /// </summary>
        public SeriesPaletteMode PaletteMode
        {
            get
            {
                return this.paletteModeCache;
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

        internal override string Family
        {
            get
            {
                return ChartPalette.BarFamily;
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

        internal override RangeSeriesModel CreateModel()
        {
            return new RangeBarSeriesModel();
        }

        internal override FrameworkElement CreateDefaultDataPointVisual(DataPoint point)
        {
            return new Border();
        }

        internal override bool IsDefaultVisual(FrameworkElement visual)
        {
            return visual is Border;
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
                visual.SetValue(Border.BackgroundProperty, paletteFill);
            }
            else
            {
                visual.ClearValue(Border.BackgroundProperty);
            }

            if (paletteStroke != null)
            {
                visual.SetValue(Border.BorderBrushProperty, paletteStroke);
            }
            else
            {
                visual.ClearValue(Border.BorderBrushProperty);
            }
        }

        internal override void ApplyPaletteToContainerVisual(SpriteVisual visual, DataPoint point)
        {
            int index = this.paletteModeCache == SeriesPaletteMode.Series ? this.ActualPaletteIndex : point.CollectionIndex;
            if (index >= 0)
            {
                SolidColorBrush paletteFill = this.chart.GetPaletteBrush(index, PaletteVisualPart.Fill, this.Family, point.isSelected) as SolidColorBrush;

                if (paletteFill != null)
                {
                    this.chart.ContainerVisualsFactory.SetCompositionColorBrush(visual, paletteFill, true);
                }
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

                    RangeBarSeries.UpdateLegendItemProperties(item, (Brush)visual.GetValue(Border.BackgroundProperty), (Brush)visual.GetValue(Border.BorderBrushProperty));
                }
                else
                {
                    item = this.LegendItems.FirstOrDefault();
                    this.UpdateLegendItemProperties((Brush)visual.GetValue(Border.BackgroundProperty), (Brush)visual.GetValue(Border.BorderBrushProperty));
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

        private static void OnLegendTitleBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var series = (RangeBarSeries)d;

            if (series.paletteModeCache == SeriesPaletteMode.DataPoint)
            {
                foreach (var pair in series.legendItems)
                {
                    series.UpdateLegendTitle(pair.Key, pair.Value);
                }
            }
        }

        private static void OnPaletteModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue.Equals(e.NewValue))
            {
                return;
            }

            RangeBarSeries series = d as RangeBarSeries;
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
        private static void OnCombineModePropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            if (args.OldValue.Equals(args.NewValue))
            {
                return;
            }

            var series = target as RangeBarSeries;

            var combineMode = (ChartSeriesCombineMode)args.NewValue;
            if (CoerceCombineMode(ref combineMode))
            {
                series.CombineMode = combineMode;
                return;
            }

            (series.model as RangeBarSeriesModel).CombineMode = combineMode;
        }

        private static bool CoerceCombineMode(ref ChartSeriesCombineMode combineMode)
        {
            if (combineMode == ChartSeriesCombineMode.None || combineMode == ChartSeriesCombineMode.Cluster)
            {
                return false;
            }
            else
            {
                combineMode = ChartSeriesCombineMode.Cluster;
                return true;
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
    }
}
