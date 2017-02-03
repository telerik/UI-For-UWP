using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents an annotation which shape may be stroked (outlined).
    /// </summary>
    public abstract class PolarStrokedAnnotation : PolarChartAnnotation, IStrokedAnnotation
    {
        /// <summary>
        /// Identifies the <see cref="Stroke"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register(nameof(Stroke), typeof(Brush), typeof(PolarStrokedAnnotation), new PropertyMetadata(null, OnStrokePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="StrokeThickness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register(nameof(StrokeThickness), typeof(double), typeof(PolarStrokedAnnotation), new PropertyMetadata(0d, OnStrokeThicknessPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="StrokeDashArray"/> property.
        /// </summary>
        public static readonly DependencyProperty StrokeDashArrayProperty =
            DependencyProperty.Register(nameof(StrokeDashArray), typeof(DoubleCollection), typeof(PolarStrokedAnnotation), new PropertyMetadata(null, OnStrokeDashArrayChanged));

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> that specifies how the shape outline is painted.
        /// </summary>
        /// <value>The line stroke.</value>
        public Brush Stroke
        {
            get
            {
                return (Brush)this.GetValue(StrokeProperty);
            }
            set
            {
                this.SetValue(StrokeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of the shape stroke outline.
        /// </summary>
        /// <value>The line stroke thickness.</value>
        public double StrokeThickness
        {
            get
            {
                return (double)this.GetValue(StrokeThicknessProperty);
            }
            set
            {
                this.SetValue(StrokeThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a collection of <see cref="T:System.Double" /> values that indicates the pattern of dashes and gaps that is used to outline stroked series.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public DoubleCollection StrokeDashArray
        {
            get
            {
                return (DoubleCollection)this.GetValue(StrokeDashArrayProperty);
            }
            set
            {
                this.SetValue(StrokeDashArrayProperty, value);
            }
        }

        /// <summary>
        /// Gets the presenter.
        /// </summary>
        /// <value>The presenter.</value>
        protected abstract Shape Presenter
        {
            get;
        }

        private static void OnStrokePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PolarStrokedAnnotation annotation = sender as PolarStrokedAnnotation;
            annotation.Presenter.Stroke = e.NewValue as Brush;
        }

        private static void OnStrokeThicknessPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PolarStrokedAnnotation annotation = sender as PolarStrokedAnnotation;
            annotation.Presenter.StrokeThickness = (double)e.NewValue;
        }

        private static void OnStrokeDashArrayChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PolarStrokedAnnotation annotation = sender as PolarStrokedAnnotation;
            annotation.Presenter.StrokeDashArray = (e.NewValue as DoubleCollection).Clone();
        }
    }
}
