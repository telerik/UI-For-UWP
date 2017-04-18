using System;
using Telerik.Charting;
using Telerik.UI.Automation.Peers;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a chart series which visualize <see cref="ScatterDataPoint"/> instances by an area shape.
    /// </summary>
    public class ScatterAreaSeries : ScatterLineSeries, IFilledSeries
    {
        /// <summary>
        /// Identifies the <see cref="Fill"/> property.
        /// </summary>
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register(nameof(Fill), typeof(Brush), typeof(ScatterAreaSeries), new PropertyMetadata(null, OnFillChanged));

        internal AreaRenderer areaRenderer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScatterAreaSeries"/> class.
        /// </summary>
        public ScatterAreaSeries()
        {
            this.DefaultStyleKey = typeof(ScatterAreaSeries);
            this.areaRenderer = this.renderer as AreaRenderer;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Fill"/> property has been set locally.
        /// </summary>
        bool IFilledSeries.IsFillSetLocally
        {
            get
            {
                return this.ReadLocalValue(FillProperty) is Brush;
            }
        }

        /// <summary>
        /// Gets or sets the mode that defines how the area is stroked.
        /// </summary>
        public AreaSeriesStrokeMode StrokeMode
        {
            get
            {
                return this.areaRenderer.strokeMode;
            }
            set
            {
                if (this.areaRenderer.strokeMode == value)
                {
                    return;
                }

                this.areaRenderer.strokeMode = value;
                this.InvalidateCore();
            }
        }

        /// <summary>
        /// Gets or sets the style used to draw the <see cref="Windows.UI.Xaml.Shapes.Polyline"/> shape.
        /// </summary>
        public Brush Fill
        {
            get
            {
                return this.areaRenderer.areaShape.Fill;
            }
            set
            {
                this.SetValue(FillProperty, value);
            }
        }

        /// <summary>
        /// Gets the human-readable family of this instance. For example Area is the family for Area, SplineArea and PolarArea series.
        /// </summary>
        internal override string Family
        {
            get
            {
                return ChartPalette.AreaFamily;
            }
        }

        internal override FrameworkElement SeriesVisual
        {
            get
            {
                return this.areaRenderer.areaShape;
            }
        }

        internal override LineRenderer CreateRenderer()
        {
            return new AreaRenderer();
        }

        /// <inheritdoc/>
        protected override void UnapplyTemplateCore()
        {
            base.UnapplyTemplateCore();

            if (this.renderSurface != null)
            {
                this.renderSurface.Children.Remove(this.areaRenderer.areaShape);
            }
        }

        /// <summary>
        /// Adds the polyline shape to the visual tree.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            if (applied)
            {
                this.renderSurface.Children.Add(this.areaRenderer.areaShape);

                // area is below the stroke
                Canvas.SetZIndex(this.areaRenderer.areaShape, -1);
            }

            return applied;
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ScatterAreaSeriesAutomationPeer(this);
        }

        private static void OnFillChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScatterAreaSeries series = d as ScatterAreaSeries;
            series.areaRenderer.areaShape.Fill = e.NewValue as Brush;

            if (series.isPaletteApplied)
            {
                series.UpdatePalette(true);
            }
        }
    }
}
