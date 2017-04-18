using Telerik.Charting;
using Telerik.Core;
using Telerik.UI.Automation.Peers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Allows you to place am ellipse at a specific place in your <see cref="RadCartesianChart"/>. This annotation
    /// uses the <see cref="RadPolarChart.PolarAxis"/>.
    /// </summary>
    public class PolarAxisGridLineAnnotation : PolarStrokedAnnotation
    {
        /// <summary>
        /// Identifies the <see cref="Value"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(double), typeof(PolarAxisGridLineAnnotation), new PropertyMetadata(double.NaN, OnValuePropertyChanged));

        private Ellipse presenter;
        private PolarGridLineAnnotationModel model;

        /// <summary>
        /// Initializes a new instance of the <see cref="PolarAxisGridLineAnnotation" /> class.
        /// </summary>
        public PolarAxisGridLineAnnotation()
        {
            this.DefaultStyleKey = typeof(PolarAxisGridLineAnnotation);

            this.model = new PolarGridLineAnnotationModel();
            this.presenter = new Ellipse();
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        public double Value
        {
            get
            {
                return (double)this.GetValue(ValueProperty);
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

        internal override void UpdatePresenters()
        {
            double stroke = this.StrokeThickness;
            RadRect bounds = this.model.polarLine.Bounds;
            Canvas.SetLeft(this.presenter, bounds.X - (stroke / 2));
            Canvas.SetTop(this.presenter, bounds.Y - (stroke / 2));
            this.presenter.Width = bounds.Width + stroke;
            this.presenter.Height = bounds.Height + stroke;
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
            return new PolarAxisGridLineAnnotationAutomationPeer(this);
        }

        private static void OnValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PolarAxisGridLineAnnotation annotation = sender as PolarAxisGridLineAnnotation;
            annotation.model.Value = annotation.Value;

            if (AutomationPeer.ListenerExists(AutomationEvents.PropertyChanged))
            {
                var peer = FrameworkElementAutomationPeer.FromElement(annotation) as PolarAxisGridLineAnnotationAutomationPeer;
                if (peer != null)
                {
                    peer.RaiseValueChangedAutomationEvent(e.OldValue != null ? e.OldValue.ToString() : string.Empty, e.NewValue != null ? e.NewValue.ToString() : string.Empty);
                }
            }
        }
    }
}