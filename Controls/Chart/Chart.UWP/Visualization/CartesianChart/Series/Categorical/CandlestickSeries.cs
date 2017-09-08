using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Charting;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a chart series that plot their points using financial "Candlestick" shapes.
    /// </summary>
    public class CandlestickSeries : OhlcSeriesBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CandlestickSeries"/> class.
        /// </summary>
        public CandlestickSeries()
        {
            this.DefaultStyleKey = typeof(CandlestickSeries);
        }
        
        internal override string Family
        {
            get
            {
                return ChartPalette.CandlestickFamily;
            }
        }

        internal override IEnumerable<LegendItem> LegendItems
        {
            get
            {
                return Enumerable.Empty<LegendItem>();
            }
        }
        
        internal override void ApplyPaletteToDefaultVisual(FrameworkElement visual, DataPoint point)
        {
            int index = this.ActualPaletteIndex;

            Brush paletteFill = this.chart.GetPaletteBrush(index, PaletteVisualPart.Fill, this.Family, point.isSelected);
            Brush paletteSpecialFill = this.chart.GetPaletteBrush(index, PaletteVisualPart.SpecialFill, this.Family, point.isSelected);
            if (paletteSpecialFill == null)
            {
                paletteSpecialFill = paletteFill;
            }

            if (paletteFill != null)
            {
                visual.SetValue(Candlestick.UpFillProperty, paletteFill);
            }
            else
            {
                visual.ClearValue(Candlestick.UpFillProperty);
            }

            if (paletteSpecialFill != null)
            {
                visual.SetValue(Candlestick.DownFillProperty, paletteSpecialFill);
            }
            else
            {
                visual.ClearValue(Candlestick.DownFillProperty);
            }

            // base class will call UpdateElementAppearance for the shape
            base.ApplyPaletteToDefaultVisual(visual, point);
        }

        internal override void ApplyPaletteToContainerVisual(SpriteVisual visual, DataPoint point)
        {
            int index = this.ActualPaletteIndex;
            var ohlcDataPoint = point as OhlcDataPoint;
            Brush paletteFill = this.chart.GetPaletteBrush(index, PaletteVisualPart.Fill, this.Family, point.isSelected);
            Brush paletteSpecialFill = this.chart.GetPaletteBrush(index, PaletteVisualPart.SpecialFill, this.Family, point.isSelected);
            Brush stroke = this.chart.GetPaletteBrush(index, PaletteVisualPart.Stroke, this.Family, point.isSelected);

            if (paletteFill != null && stroke != null && ohlcDataPoint != null)
            {
                for (int i = 0; i < visual.Children.Count; i++)
                {
                    var childVisual = visual.Children.ElementAt(i) as SpriteVisual;
                    if (childVisual != null)
                    {
                        if (i + 1 != visual.Children.Count)
                        {
                            this.chart.ContainerVisualsFactory.SetCompositionColorBrush(childVisual, stroke, true);
                        }
                        else
                        {
                            if (ohlcDataPoint.IsFalling && paletteSpecialFill != null)
                            {
                                this.chart.ContainerVisualsFactory.SetCompositionColorBrush(childVisual, paletteSpecialFill, true);
                            }
                            else
                            {
                                this.chart.ContainerVisualsFactory.SetCompositionColorBrush(childVisual, paletteFill, true);
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var child in visual.Children)
                {
                    var childVisual = child as SpriteVisual;
                    if (childVisual != null)
                    {
                        this.chart.ContainerVisualsFactory.SetCompositionColorBrush(childVisual, null, true);
                    }
                }
            }
        }

        internal override FrameworkElement CreateDefaultDataPointVisual(DataPoint point)
        {
            return new Candlestick();
        }

        internal override void UpdateLegendItem(FrameworkElement visual, DataPoint dataPoint)
        {
            // Implement this when the legend start to makes sense for this series.
        }

        /// <inheritdoc/>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new CandlestickSeriesAutomationPeer(this);
        }
    }
}
