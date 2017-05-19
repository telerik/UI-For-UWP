using Telerik.Charting;
using Telerik.UI.Automation.Peers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Allows you highlight the area between two values drawn by the <see cref="CartesianPlotBandAnnotation.Axis"/>
    /// in the <see cref="RadCartesianChart"/>.
    /// </summary>
    public class CartesianPlotBandAnnotation : CartesianStrokedAnnotation
    {
        /// <summary>
        /// Identifies the <see cref="Axis"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AxisProperty =
            DependencyProperty.Register(nameof(Axis), typeof(CartesianAxis), typeof(CartesianPlotBandAnnotation), new PropertyMetadata(null, OnAxisPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="From"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register(nameof(From), typeof(object), typeof(CartesianPlotBandAnnotation), new PropertyMetadata(null, OnFromPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="To"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register(nameof(To), typeof(object), typeof(CartesianPlotBandAnnotation), new PropertyMetadata(null, OnToPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="Fill"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register(nameof(Fill), typeof(Brush), typeof(CartesianPlotBandAnnotation), new PropertyMetadata(null, OnFillPropertyChanged));

        private Rectangle presenter;
        private CartesianPlotBandAnnotationModel model;

        /// <summary>
        /// Initializes a new instance of the <see cref="CartesianPlotBandAnnotation" /> class.
        /// </summary>
        public CartesianPlotBandAnnotation()
        {
            this.DefaultStyleKey = typeof(CartesianPlotBandAnnotation);

            this.model = new CartesianPlotBandAnnotationModel();
            this.presenter = new Rectangle();
        }

        /// <summary>
        /// Gets or sets the axis.
        /// </summary>
        /// <value>The axis.</value>
        public CartesianAxis Axis
        {
            get
            {
                return (CartesianAxis)this.GetValue(AxisProperty);
            }
            set
            {
                this.SetValue(AxisProperty, value);
            }
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

        /// <summary>
        /// Gets a value indicating whether the stroke goes inwards by the full <see cref="Shape.StrokeThickness"/>.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is stroke inset; otherwise, <c>false</c>.
        /// </value>
        protected override bool IsStrokeInset
        {
            get
            {
                return true;
            }
        }

        internal override void SetPresenterVisibility(Visibility annotationVisibility)
        {
            if (this.presenter != null)
            {
                this.presenter.Visibility = annotationVisibility;
            }

            if (this.labelPresenter != null)
            {
                this.labelPresenter.Visibility = annotationVisibility;
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
                base.UpdateVisibility();
            }
            else
            {
                this.SetPresenterVisibility(Visibility.Collapsed);
            }
        }

        internal override void UpdatePresenters()
        {
            base.UpdatePresenters();

            Canvas.SetLeft(this.presenter, this.model.layoutSlot.X);
            Canvas.SetTop(this.presenter, this.model.layoutSlot.Y);
            this.presenter.Width = this.model.layoutSlot.Width;
            this.presenter.Height = this.model.layoutSlot.Height;
        }

        internal override ChartAnnotationLabelDefinition CreateDefaultLabelDefinition()
        {
            return new ChartAnnotationLabelDefinition()
            {
                Location = ChartAnnotationLabelLocation.Inside,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalOffset = 2
            };
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

        /// <inheritdoc/>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new CartesianPlotBandAnnotationAutomationPeer(this);
        }

        private static void OnAxisPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CartesianPlotBandAnnotation annotation = sender as CartesianPlotBandAnnotation;
            if (annotation.Axis != null)
            {
                annotation.model.Axis = annotation.Axis.model;
            }
            else
            {
                annotation.model.Axis = null;
            }
        }

        private static void OnFromPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CartesianPlotBandAnnotation annotation = sender as CartesianPlotBandAnnotation;
            annotation.model.From = annotation.From;
        }

        private static void OnToPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CartesianPlotBandAnnotation annotation = sender as CartesianPlotBandAnnotation;
            annotation.model.To = annotation.To;
        }

        private static void OnFillPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CartesianPlotBandAnnotation annotation = sender as CartesianPlotBandAnnotation;
            annotation.presenter.Fill = e.NewValue as Brush;
        }
    }
}