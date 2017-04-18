using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Charting;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a chart series that plot their points using rectangular shapes, named "Bars".
    /// The series support default visuals - <see cref="Border"/> instances.
    /// </summary>
    public class BarSeries : CategoricalSeries
    {
        /// <summary>
        /// Identifies the <see cref="PaletteMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PaletteModeProperty =
            DependencyProperty.Register(nameof(PaletteMode), typeof(SeriesPaletteMode), typeof(BarSeries), new PropertyMetadata(SeriesPaletteMode.Series, OnPaletteModeChanged));

        /// <summary>
        /// Identifies the <see cref="LegendTitleBinding"/> property.
        /// </summary>
        public static readonly DependencyProperty LegendTitleBindingProperty =
            DependencyProperty.Register(nameof(LegendTitleBinding), typeof(DataPointBinding), typeof(BarSeries), new PropertyMetadata(null, OnLegendTitleBindingChanged));

        private BarSeriesModel model;
        private SeriesPaletteMode paletteModeCache = SeriesPaletteMode.Series;
        private Dictionary<DataPoint, LegendItem> legendItems;

        /// <summary>
        /// Initializes a new instance of the <see cref="BarSeries"/> class.
        /// </summary>
        public BarSeries()
        {
            this.DefaultStyleKey = typeof(BarSeries);
            this.model = new BarSeriesModel();
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

        /// <summary>
        /// Gets the human-readable family of this instance. For example Area is the family for Area, SplineArea and PolarArea series.
        /// </summary>
        internal override string Family
        {
            get
            {
                return ChartPalette.BarFamily;
            }
        }

        internal override FrameworkElement SeriesVisual
        {
            get
            {
                return this.RenderSurface;
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

                    BarSeries.UpdateLegendItemProperties(item, (Brush)visual.GetValue(Border.BackgroundProperty), (Brush)visual.GetValue(Border.BorderBrushProperty));
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
            return new Border();
        }

        internal override bool IsDefaultVisual(FrameworkElement visual)
        {
            return visual is Border;
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new BarSeriesAutomationPeer(this);
        }

        private static void OnPaletteModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue.Equals(e.NewValue))
            {
                return;
            }

            BarSeries series = d as BarSeries;
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
            var series = (BarSeries)d;

            if (series.paletteModeCache == SeriesPaletteMode.DataPoint)
            {
                foreach (var pair in series.legendItems)
                {
                    series.UpdateLegendTitle(pair.Key, pair.Value);
                }
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
