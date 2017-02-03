using Telerik.Charting;
using Telerik.Core;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Base class for all <see cref="ChartSeries"/> that may visualize their data points in a circle.
    /// </summary>
    public abstract partial class PolarSeries : PointTemplateSeries
    {
        /// <summary>
        /// Identifies the <see cref="ValueBinding"/> property.
        /// </summary>
        public static readonly DependencyProperty ValueBindingProperty =
            DependencyProperty.Register(nameof(ValueBinding), typeof(DataPointBinding), typeof(PolarSeries), new PropertyMetadata(null, PolarSeries.OnValueBindingChanged));

        private IPlotAreaElementModelWithAxes axisModel;

        /// <summary>
        /// Gets or sets the binding that will be used to fill the Value member of the contained data points.
        /// </summary>
        public DataPointBinding ValueBinding
        {
            get
            {
                return this.GetValue(ValueBindingProperty) as DataPointBinding;
            }
            set
            {
                this.SetValue(ValueBindingProperty, value);
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
                return new RadSize(10, 10);
            }
        }

        private IPlotAreaElementModelWithAxes AxisModel
        {
            get
            {
                if (this.axisModel == null)
                {
                    this.axisModel = this.Model as IPlotAreaElementModelWithAxes;
                }
                return this.axisModel;
            }
        }

        internal override FrameworkElement CreateDefaultDataPointVisual(DataPoint point)
        {
            // TODO: Points cannot be set in Style due to a known problem in SL. Think of a property of the series itself that allows for user customization.
            Path visual = new Path();
            visual.Data = new EllipseGeometry() { Center = new Point(5, 5), RadiusX = 5, RadiusY = 5 };

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

            this.UpdateLegendItem(visual, point);
        }

        internal override void UpdateLegendItem(FrameworkElement visual, DataPoint dataPoint)
        {
            if (visual != null)
            {
                this.UpdateLegendItemProperties((Brush)visual.GetValue(Path.FillProperty), (Brush)visual.GetValue(Path.StrokeProperty));
            }
        }

        internal override void UpdateClip(ChartLayoutContext context)
        {
            this.chart.ApplyPlotAreaClip(this.renderSurface, this.ClipToPlotArea);
        }

        /// <summary>
        /// Occurs when one of the axes of the owning <see cref="RadPolarChart"/> has been changed.
        /// </summary>
        /// <param name="oldAxis">The old axis.</param>
        /// <param name="newAxis">The new axis.</param>
        internal void ChartAxisChanged(Axis oldAxis, Axis newAxis)
        {
            if (oldAxis != null)
            {
                this.AxisModel.DetachAxis(oldAxis.model);
            }
            if (newAxis != null)
            {
                this.AxisModel.AttachAxis(newAxis.model, newAxis.type);
            }
        }

        /// <summary>
        /// Called when <seealso cref="PolarSeries.ValueBinding"/> has changed.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnValueBindingChanged(DataPointBinding oldValue, DataPointBinding newValue)
        {
        }

        /// <summary>
        /// Occurs when the presenter has been successfully attached to its owning <see cref="RadChartBase" /> instance.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            RadPolarChart chart = this.chart as RadPolarChart;
            if (chart.PolarAxis != null)
            {
                this.AxisModel.AttachAxis(chart.PolarAxis.model, AxisType.First);
            }
            if (chart.RadialAxis != null)
            {
                this.AxisModel.AttachAxis(chart.RadialAxis.model, AxisType.Second);
            }
        }

        /// <summary>
        /// Occurs when the presenter has been successfully detached from its owning <see cref="RadChartBase" /> instance.
        /// </summary>
        protected override void OnDetached(RadChartBase oldChart)
        {
            base.OnDetached(oldChart);

            if (oldChart != null)
            {
                RadPolarChart chart = oldChart as RadPolarChart;
                this.AxisModel.DetachAxis(chart.PolarAxis.model);
                this.AxisModel.DetachAxis(chart.RadialAxis.model);
            }

            // TODO: Raise exception as needed.
        }

        private static void OnValueBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PolarSeries presenter = d as PolarSeries;
            presenter.OnValueBindingChanged(e.OldValue as DataPointBinding, e.NewValue as DataPointBinding);
        }
    }
}
