using System;
using Telerik.Charting;
using Telerik.Core;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents an elliptical axis.
    /// </summary>
    public abstract class RadialAxis : Axis
    {
        /// <summary>
        /// Identifies the <see cref="SweepDirection"/> property.
        /// </summary>
        public static readonly DependencyProperty SweepDirectionProperty =
            DependencyProperty.Register(nameof(SweepDirection), typeof(SweepDirection), typeof(RadialAxis), new PropertyMetadata(SweepDirection.Counterclockwise, OnSweepDirectionChanged));
        private Ellipse ellipse;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadialAxis"/> class.
        /// </summary>
        protected RadialAxis()
        {
            this.DefaultStyleKey = typeof(RadialAxis);
            this.ellipse = new Ellipse();
        }

        /// <summary>
        /// Gets or sets a value that specifies in which direction the axis will plot its data.
        /// </summary>
        public SweepDirection SweepDirection
        {
            get
            {
                return this.model.IsInverse ? SweepDirection.Clockwise : SweepDirection.Counterclockwise;
            }
            set
            {
                this.SetValue(SweepDirectionProperty, value);
            }
        }

        /// <summary>
        /// Gets the visual that represents the stroke of the axis.
        /// </summary>
        internal override Shape StrokeVisual
        {
            get
            {
                return this.ellipse;
            }
        }

        internal override void UpdateUICore(ChartLayoutContext context)
        {
            base.UpdateUICore(context);

            this.UpdateEllipse();
        }

        internal override void OnPlotOriginChanged(ChartLayoutContext context)
        {
            base.OnPlotOriginChanged(context);

            this.UpdateEllipse();
        }

        internal override RadRect GetLayoutSlot(Node node, ChartLayoutContext context)
        {
            return node.layoutSlot;
        }

        internal override void TransformTick(AxisTickModel tick, FrameworkElement visual)
        {
            double angle = (double)tick.normalizedValue * 360;
            double startAngle = (this.chart.chartArea as PolarChartAreaModel).StartAngle;
            visual.RenderTransformOrigin = new Point(0.5, 0);
            visual.RenderTransform = new RotateTransform() { Angle = this.model.IsInverse ? -90 + startAngle - angle : -90 - startAngle - angle };
        }

        /// <inheritdoc/>
        protected override void UnapplyTemplateCore()
        {
            base.UnapplyTemplateCore();

            if (this.renderSurface != null)
            {
                this.renderSurface.Children.Remove(this.ellipse);
            }
        }

        /// <summary>
        /// Adds the axis ellipse shape to the visual tree.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            if (applied)
            {
                this.renderSurface.Children.Add(this.ellipse);
            }

            return applied;
        }

        private static void OnSweepDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Axis presenter = d as Axis;
            presenter.model.IsInverse = (SweepDirection)e.NewValue == SweepDirection.Clockwise;
        }

        private void UpdateEllipse()
        {
            // update stroke thickness
            this.ellipse.StrokeThickness = this.model.LineThickness;

            RadRect plotArea = this.chart.chartArea.plotArea.layoutSlot;

            this.ellipse.Width = plotArea.Width + 0.5;
            this.ellipse.Height = plotArea.Height + 0.5;

            Canvas.SetLeft(this.ellipse, plotArea.X - 0.5);
            Canvas.SetTop(this.ellipse, plotArea.Y - 0.5);
        }
    }
}