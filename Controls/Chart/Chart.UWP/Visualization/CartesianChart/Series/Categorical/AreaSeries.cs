using System.Diagnostics.CodeAnalysis;
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
    /// Represents a chart series that are visualize like an area figure in the cartesian space.
    /// </summary>
    public class AreaSeries : CategoricalStrokedSeries, IFilledSeries
    {
        /// <summary>
        /// Identifies the <see cref="Fill"/> property.
        /// </summary>
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register(nameof(Fill), typeof(Brush), typeof(AreaSeries), new PropertyMetadata(null, OnFillChanged));

        internal AreaRenderer areaRenderer;

        /// <summary>
        /// Initializes a new instance of the <see cref="AreaSeries"/> class.
        /// </summary>
        public AreaSeries()
        {
            this.DefaultStyleKey = typeof(AreaSeries);
            this.areaRenderer = this.renderer as AreaRenderer;
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
        /// Gets a value indicating whether the <see cref="Fill"/> property has been set locally.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        bool IFilledSeries.IsFillSetLocally
        {
            get
            {
                return this.ReadLocalValue(FillProperty) is Brush;
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

        internal override void UpdateUICore(ChartLayoutContext context)
        {
            if (this.Visibility == Visibility.Collapsed)
            {
                return;
            }

            base.UpdateUICore(context);

            if (this.model.CombineMode == ChartSeriesCombineMode.Stack ||
                this.model.CombineMode == ChartSeriesCombineMode.Stack100)
            {
                // pass our reference to next stacked series
                this.chart.StackedSeriesContext.PreviousStackedArea = this.areaRenderer.topSurfacePoints;
            }
        }

        internal override void UpdateLegendItem(FrameworkElement visual, DataPoint dataPoint)
        {
            this.UpdateLegendItemProperties(this.areaRenderer.areaShape.Fill, this.areaRenderer.strokeShape.Stroke);
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

        /// <inheritdoc/>
        protected override Point[] SelectRectPoints(ref Rect touchRect)
        {
            if (touchRect.Width == 0)
            {
                return new Point[] { new Point(touchRect.Left, touchRect.Top) };
            }
            else
            {
                return new Point[] 
                {
                    new Point(touchRect.Left, touchRect.Bottom),
                    new Point(touchRect.Right, touchRect.Bottom),
                    new Point(touchRect.Right, touchRect.Top),
                    new Point(touchRect.Left, touchRect.Top),
                    new Point((touchRect.Left + touchRect.Right) / 2, (touchRect.Top + touchRect.Bottom) / 2)
                };
            }
        }

        /// <inheritdoc/>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new AreaSeriesAutomationPeer(this);
        }

        private static void OnFillChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AreaSeries series = d as AreaSeries;
            series.areaRenderer.areaShape.Fill = e.NewValue as Brush;

            if (series.isPaletteApplied)
            {
                series.UpdatePalette(true);
            }
        }
    }
}
