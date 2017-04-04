using Telerik.Charting;
using Telerik.Core;
using Telerik.UI.Automation.Peers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Allows you highlight the area between two values drawn by the <see cref="RadPolarChart.PolarAxis"/>
    /// in the <see cref="RadPolarChart"/>.
    /// </summary>
    public class PolarAxisPlotBandAnnotation : PolarStrokedAnnotation
    {
        /// <summary>
        /// Identifies the <see cref="From"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register(nameof(From), typeof(double), typeof(PolarAxisPlotBandAnnotation), new PropertyMetadata(double.NaN, OnFromPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="To"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register(nameof(To), typeof(double), typeof(PolarAxisPlotBandAnnotation), new PropertyMetadata(double.NaN, OnToPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="Fill"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register(nameof(Fill), typeof(Brush), typeof(PolarAxisPlotBandAnnotation), new PropertyMetadata(null, OnFillPropertyChanged));

        private Path presenter;
        private PolarPlotBandAnnotationModel model;

        /// <summary>
        /// Initializes a new instance of the <see cref="PolarAxisPlotBandAnnotation" /> class.
        /// </summary>
        public PolarAxisPlotBandAnnotation()
        {
            this.DefaultStyleKey = typeof(PolarAxisPlotBandAnnotation);

            this.model = new PolarPlotBandAnnotationModel();
            this.presenter = new Path();
        }

        /// <summary>
        /// Gets or sets from.
        /// </summary>
        /// <value>From.</value>
        public double From
        {
            get
            {
                return (double)this.GetValue(FromProperty);
            }
            set
            {
                this.SetValue(FromProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets to.
        /// </summary>
        /// <value>To.</value>
        public double To
        {
            get
            {
                return (double)this.GetValue(ToProperty);
            }
            set
            {
                this.SetValue(ToProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Brush that specifies how the shape's interior is painted.
        /// </summary>
        /// <value>The fill.</value>
        public Brush Fill
        {
            get
            {
                return (Brush)this.GetValue(FillProperty);
            }
            set
            {
                this.SetValue(FillProperty, value);
            }
        }

        internal override ChartAnnotationModel Model
        {
            get
            {
                return this.model;
            }
        }

        /// <summary>
        /// Gets the presenter.
        /// </summary>
        /// <value>The presenter.</value>
        protected override Shape Presenter
        {
            get
            {
                return this.presenter;
            }
        }

        internal override void ChartAxisChanged(Axis oldAxis, Axis newAxis)
        {
            base.ChartAxisChanged(oldAxis, newAxis);

            if (newAxis != null && newAxis.type == AxisType.First)
            {
                this.model.Axis = newAxis.model;
            }
            else if (oldAxis != null && oldAxis.type == AxisType.First)
            {
                this.model.Axis = null;
            }
        }

        internal override void SetPresenterVisibility(Visibility annotationVisibility)
        {
            if (this.presenter != null)
            {
                this.presenter.Visibility = annotationVisibility;
            }
        }

        internal override Visibility GetPresenterVisibility()
        {
            if (this.presenter != null)
            {
                return this.presenter.Visibility;
            }
            else
            {
                return this.Visibility;
            }
        }

        internal override void UpdateVisibility()
        {
            if (this.model.firstPlotInfo != null && this.model.secondPlotInfo != null)
            {
                this.SetPresenterVisibility(Visibility.Visible);
            }
            else
            {
                this.SetPresenterVisibility(Visibility.Collapsed);
            }
        }

        internal override void UpdatePresenters()
        {
            this.presenter.Data = BuildRadialStripe(this.model.circle1, this.model.circle2);
        }

        /// <inheritdoc/>
        protected override void UnapplyTemplateCore()
        {
            base.UnapplyTemplateCore();

            if (this.renderSurface != null)
            {
                this.renderSurface.Children.Remove(this.presenter);
            }
        }

        /// <summary>
        /// Initializes the render surface template part.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            if (applied)
            {
                this.renderSurface.Children.Add(this.presenter);
            }

            return applied;
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new PolarAxisPlotBandAnnotationAutomationPeer(this);
        }

        private static void OnFromPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PolarAxisPlotBandAnnotation annotation = sender as PolarAxisPlotBandAnnotation;
            annotation.model.From = annotation.From;
        }

        private static void OnToPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PolarAxisPlotBandAnnotation annotation = sender as PolarAxisPlotBandAnnotation;
            annotation.model.To = annotation.To;
        }

        private static void OnFillPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PolarAxisPlotBandAnnotation annotation = sender as PolarAxisPlotBandAnnotation;
            annotation.presenter.Fill = e.NewValue as Brush;
        }

        private static Geometry BuildRadialStripe(RadCircle circle, RadCircle previousCircle)
        {
            DoughnutSegmentData segment = new DoughnutSegmentData()
            {
                Center = new RadPoint(circle.Center.X + 0.5, circle.Center.Y + 0.5),
                Radius1 = circle.Radius + 0.5,
                Radius2 = previousCircle.Radius - 0.5,
                StartAngle = 0,
                SweepAngle = 359.99 // 360 does not render properly
            };

            if (segment.Radius1 < 0)
            {
                segment.Radius1 = 0;
            }
            if (segment.Radius2 < 0)
            {
                segment.Radius2 = 0;
            }

            return DoughnutSegmentRenderer.RenderArc(segment);
        }
    }
}