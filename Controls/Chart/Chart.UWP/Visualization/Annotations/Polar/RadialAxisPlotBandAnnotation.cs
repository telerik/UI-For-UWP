using Telerik.Charting;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Allows you highlight the area between two values drawn by the <see cref="RadPolarChart.RadialAxis"/>
    /// in the <see cref="RadPolarChart"/>.
    /// </summary>
    public class RadialAxisPlotBandAnnotation : PolarStrokedAnnotation
    {
        /// <summary>
        /// Identifies the <see cref="From"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register(nameof(From), typeof(object), typeof(RadialAxisPlotBandAnnotation), new PropertyMetadata(null, OnFromPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="To"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register(nameof(To), typeof(object), typeof(RadialAxisPlotBandAnnotation), new PropertyMetadata(null, OnToPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="Fill"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register(nameof(Fill), typeof(Brush), typeof(RadialAxisPlotBandAnnotation), new PropertyMetadata(null, OnFillPropertyChanged));

        private Path presenter;
        private RadialPlotBandAnnotationModel model;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadialAxisPlotBandAnnotation" /> class.
        /// </summary>
        public RadialAxisPlotBandAnnotation()
        {
            this.DefaultStyleKey = typeof(RadialAxisPlotBandAnnotation);

            this.model = new RadialPlotBandAnnotationModel();
            this.presenter = new Path();
        }

        /// <summary>
        /// Gets or sets from.
        /// </summary>
        /// <value>From.</value>
        public object From
        {
            get
            {
                return this.GetValue(FromProperty);
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
        public object To
        {
            get
            {
                return this.GetValue(ToProperty);
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

            if (newAxis != null && newAxis.type == AxisType.Second)
            {
                this.model.Axis = newAxis.model;
            }
            else if (oldAxis != null && oldAxis.type == AxisType.Second)
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
            RadPolarVector vector1 = this.model.polarVector1;
            RadPolarVector vector2 = this.model.polarVector2;
            bool isLargeArc;

            if (vector1.Angle > vector2.Angle)
            {
                isLargeArc = vector1.Angle - vector2.Angle > 180d;
            }
            else
            {
                isLargeArc = (vector1.Angle + 360) - vector2.Angle > 180d;
            }
            if (this.model.Axis.IsInverse)
            {
                this.presenter.Data = this.BuildPolarStripe(vector1, vector2, !isLargeArc);
            }
            else
            {
                this.presenter.Data = this.BuildPolarStripe(vector2, vector1, isLargeArc);
            }
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

        private static void OnFromPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RadialAxisPlotBandAnnotation annotation = sender as RadialAxisPlotBandAnnotation;
            annotation.model.From = annotation.From;
        }

        private static void OnToPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RadialAxisPlotBandAnnotation annotation = sender as RadialAxisPlotBandAnnotation;
            annotation.model.To = annotation.To;
        }

        private static void OnFillPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RadialAxisPlotBandAnnotation annotation = sender as RadialAxisPlotBandAnnotation;
            annotation.presenter.Fill = e.NewValue as Brush;
        }

        private PathGeometry BuildPolarStripe(RadPolarVector vector, RadPolarVector nextVector, bool isLargeArc)
        {
            double radius = this.model.radius;
            Point center = new Point(vector.Center.X, vector.Center.Y);
            PathGeometry geometry = new PathGeometry();
            PathFigure figure = new PathFigure();
            figure.IsClosed = true;
            figure.IsFilled = true;
            figure.StartPoint = center;

            // first line
            LineSegment line1 = new LineSegment();
            line1.Point = new Point(vector.Point.X, vector.Point.Y);
            figure.Segments.Add(line1);

            // arc
            ArcSegment arc = new ArcSegment();
            arc.SweepDirection = SweepDirection.Clockwise;
            arc.Size = new Size(radius, radius);
            arc.IsLargeArc = isLargeArc;
            arc.Point = new Point(nextVector.Point.X, nextVector.Point.Y);
            figure.Segments.Add(arc);

            // second line
            var line2 = new LineSegment();
            line2.Point = center;
            figure.Segments.Add(line2);

            geometry.Figures.Add(figure);

            return geometry;
        }
    }
}