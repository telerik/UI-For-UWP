using Telerik.Charting;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Allows you to place a straight line at a specific place in your <see cref="RadCartesianChart"/>. This annotation
    /// uses the <see cref="RadPolarChart.RadialAxis"/>.
    /// </summary>
    public class RadialAxisGridLineAnnotation : PolarStrokedAnnotation
    {
        /// <summary>
        /// Identifies the <see cref="Value"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(object), typeof(RadialAxisGridLineAnnotation), new PropertyMetadata(null, OnValuePropertyChanged));

        private Line presenter;
        private RadialGridLineAnnotationModel model;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadialAxisGridLineAnnotation" /> class.
        /// </summary>
        public RadialAxisGridLineAnnotation()
        {
            this.DefaultStyleKey = typeof(RadialAxisGridLineAnnotation);

            this.model = new RadialGridLineAnnotationModel();
            this.presenter = new Line();
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        public object Value
        {
            get
            {
                return this.GetValue(ValueProperty);
            }
            set
            {
                this.SetValue(ValueProperty, value);
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
            if (this.model.plotInfo != null)
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
            this.presenter.X1 = this.model.radialLine.Center.X;
            this.presenter.Y1 = this.model.radialLine.Center.Y;
            this.presenter.X2 = this.model.radialLine.Point.X;
            this.presenter.Y2 = this.model.radialLine.Point.Y;
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

        private static void OnValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RadialAxisGridLineAnnotation annotation = sender as RadialAxisGridLineAnnotation;
            annotation.model.Value = annotation.Value;
        }
    }
}