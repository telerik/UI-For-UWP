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
    /// Allows you to place a straight line at a specific place in your <see cref="RadCartesianChart"/>. 
    /// </summary>
    public class CartesianGridLineAnnotation : CartesianStrokedAnnotation
    {
        /// <summary>
        /// Identifies the <see cref="Axis"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AxisProperty =
            DependencyProperty.Register(nameof(Axis), typeof(CartesianAxis), typeof(CartesianGridLineAnnotation), new PropertyMetadata(null, OnAxisPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="Value"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(object), typeof(CartesianGridLineAnnotation), new PropertyMetadata(null, OnValuePropertyChanged));

        private Line presenter;
        private CartesianGridLineAnnotationModel model;

        /// <summary>
        /// Initializes a new instance of the <see cref="CartesianGridLineAnnotation" /> class.
        /// </summary>
        public CartesianGridLineAnnotation()
        {
            this.DefaultStyleKey = typeof(CartesianGridLineAnnotation);

            this.model = new CartesianGridLineAnnotationModel();

            this.presenter = new Line();
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

        internal override void UpdatePresenters()
        {
            base.UpdatePresenters();

            double offset = this.StrokeThickness % 2 == 0 ? 0 : 0.5;
            if (this.Axis.type == AxisType.First)
            {
                this.presenter.X1 = this.model.line.X1 + offset;
                this.presenter.Y1 = this.model.line.Y1;
                this.presenter.X2 = this.model.line.X2 + offset;
                this.presenter.Y2 = this.model.line.Y2;
            }
            else
            {
                this.presenter.X1 = this.model.line.X1;
                this.presenter.Y1 = this.model.line.Y1 + offset;
                this.presenter.X2 = this.model.line.X2;
                this.presenter.Y2 = this.model.line.Y2 + offset;
            }
        }

        internal override void UpdateVisibility()
        {
            if (this.model.plotInfo != null)
            {
                base.UpdateVisibility();
            }
            else
            {
                this.SetPresenterVisibility(Visibility.Collapsed);
            }
        }

        internal override ChartAnnotationLabelDefinition CreateDefaultLabelDefinition()
        {
            if (this.Axis.type == AxisType.First)
            {
                return new ChartAnnotationLabelDefinition()
                {
                    Location = ChartAnnotationLabelLocation.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalOffset = 4
                };
            }
            else
            {
                return new ChartAnnotationLabelDefinition()
                {
                    Location = ChartAnnotationLabelLocation.Top,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    HorizontalOffset = -4
                };
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

        /// <inheritdoc/>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new CartesianGridLineAnnotationAutomationPeer(this);
        }

        private static void OnAxisPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CartesianGridLineAnnotation annotation = sender as CartesianGridLineAnnotation;
            if (annotation.Axis != null)
            {
                annotation.model.Axis = annotation.Axis.model;
            }
            else
            {
                annotation.model.Axis = null;
            }
        }

        private static void OnValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CartesianGridLineAnnotation annotation = sender as CartesianGridLineAnnotation;
            annotation.model.Value = annotation.Value;

            if (AutomationPeer.ListenerExists(AutomationEvents.PropertyChanged))
            {
                var peer = FrameworkElementAutomationPeer.FromElement(annotation) as CartesianGridLineAnnotationAutomationPeer;
                if (peer != null)
                {
                    peer.RaiseValueChangedAutomationEvent(e.OldValue != null ? e.OldValue.ToString() : string.Empty, e.NewValue != null ? e.NewValue.ToString() : string.Empty);
                }
            }
        }
    }
}