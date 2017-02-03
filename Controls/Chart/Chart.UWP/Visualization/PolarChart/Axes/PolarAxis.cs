using Telerik.Charting;
using Telerik.Core;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents the Polar (Radius) axis within a <see cref="RadPolarChart"/> instance.
    /// </summary>
    public class PolarAxis : LineAxis
    {
        /// <summary>
        /// Identifies the <see cref="MajorStep"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MajorStepProperty =
            DependencyProperty.Register(nameof(MajorStep), typeof(double), typeof(PolarAxis), new PropertyMetadata(0d, OnMajorStepChanged));

        /// <summary>
        /// Identifies the <see cref="Minimum"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(nameof(Minimum), typeof(double), typeof(PolarAxis), new PropertyMetadata(double.NegativeInfinity, OnMinimumChanged));

        /// <summary>
        /// Identifies the <see cref="Maximum"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(nameof(Maximum), typeof(double), typeof(PolarAxis), new PropertyMetadata(double.PositiveInfinity, OnMaximumChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="PolarAxis"/> class.
        /// </summary>
        public PolarAxis()
        {
            this.DefaultStyleKey = typeof(PolarAxis);
        }

        /// <summary>
        /// Gets or sets the major step between each axis tick.
        /// By default the axis itself will calculate the major step, depending on the plotted data points.
        /// </summary>
        /// <remarks> You can reset this property by setting it to 0 to restore the default behavior.</remarks>
        public double MajorStep
        {
            get
            {
                return (this.model as NumericalAxisModel).MajorStep;
            }
            set
            {
                this.SetValue(MajorStepProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the user-defined minimum of the axis. 
        /// By default the axis itself will calculate the minimum, depending on the minimum of the plotted data points.
        /// </summary>
        /// <remarks> You can reset this property by setting it to double.NegativeInfinity to restore the default behavior.</remarks>
        public double Minimum
        {
            get
            {
                return (this.model as NumericalAxisModel).Minimum;
            }
            set
            {
                this.SetValue(MinimumProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the user-defined maximum of the axis. 
        /// By default the axis itself will calculate the maximum, depending on the maximum of the plotted data points.
        /// </summary>
        /// <remarks> You can reset this property by setting it to double.PositiveInfinity to restore the default behavior.</remarks>
        public double Maximum
        {
            get
            {
                return (this.model as NumericalAxisModel).Maximum;
            }
            set
            {
                this.SetValue(MaximumProperty, value);
            }
        }

        /// <summary>
        /// Gets the actual range used by the axis to plot data points.
        /// </summary>
        public ValueRange<double> ActualRange
        {
            get
            {
                return (this.model as NumericalAxisModel).actualRange;
            }
        }

        internal override AxisModel CreateModel()
        {
            return new PolarAxisModel();
        }

        internal override RadRect GetLayoutSlot(Node node, ChartLayoutContext context)
        {
            return node.layoutSlot;
        }

        internal override void TransformTick(AxisTickModel tick, FrameworkElement visual)
        {
            PolarChartAreaModel chartAreaModel = this.chart.chartArea as PolarChartAreaModel;
            double startAngle = chartAreaModel.StartAngle;
            visual.RenderTransformOrigin = new Point(0.5, 0);
            double rotationAngle = chartAreaModel.AngleAxis.IsInverse ? startAngle : 360 - startAngle;
            visual.RenderTransform = new RotateTransform() { Angle = rotationAngle };
        }

        internal override void UpdateAxisLine(ChartLayoutContext context)
        {
            PolarChartAreaModel chartArea = this.chart.chartArea as PolarChartAreaModel;
            double angle = chartArea.NormalizeAngle(0);
            RadPoint center = chartArea.plotArea.layoutSlot.Center;
            RadPoint point = RadMath.GetArcPoint(angle, center, chartArea.plotArea.layoutSlot.Width / 2);

            double antiAliasOffset = this.model.LineThickness % 2 == 1 ? 0.5 : 0;

            this.line.X1 = center.X;
            this.line.Y1 = center.Y - antiAliasOffset;
            this.line.X2 = point.X;
            this.line.Y2 = point.Y - antiAliasOffset;

            base.UpdateAxisLine(context);
        }

        private static void OnMajorStepChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            PolarAxis presenter = target as PolarAxis;
            (presenter.model as NumericalAxisModel).MajorStep = (double)args.NewValue;
        }

        private static void OnMinimumChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            PolarAxis presenter = target as PolarAxis;
            (presenter.model as NumericalAxisModel).Minimum = (double)args.NewValue;
        }

        private static void OnMaximumChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            PolarAxis presenter = target as PolarAxis;
            (presenter.model as NumericalAxisModel).Maximum = (double)args.NewValue;
        }
    }
}