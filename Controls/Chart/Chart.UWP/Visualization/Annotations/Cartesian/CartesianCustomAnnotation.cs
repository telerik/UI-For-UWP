using Telerik.Charting;
using Telerik.Core;
using Telerik.UI.Automation.Peers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Allows you to place a custom object at a specific place in your <see cref="RadCartesianChart"/>. 
    /// </summary>
    public class CartesianCustomAnnotation : CartesianChartAnnotation
    {
        /// <summary>
        /// Identifies the <see cref="HorizontalAxis"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalAxisProperty =
            DependencyProperty.Register(nameof(HorizontalAxis), typeof(CartesianAxis), typeof(CartesianCustomAnnotation), new PropertyMetadata(null, OnHorizontalAxisPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="VerticalAxis"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalAxisProperty =
            DependencyProperty.Register(nameof(VerticalAxis), typeof(CartesianAxis), typeof(CartesianCustomAnnotation), new PropertyMetadata(null, OnVerticalAxisPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="HorizontalValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalValueProperty =
            DependencyProperty.Register(nameof(HorizontalValue), typeof(object), typeof(CartesianCustomAnnotation), new PropertyMetadata(null, OnHorizontalValuePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="VerticalValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalValueProperty =
            DependencyProperty.Register(nameof(VerticalValue), typeof(object), typeof(CartesianCustomAnnotation), new PropertyMetadata(null, OnVerticalValuePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="Content"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(nameof(Content), typeof(object), typeof(CartesianCustomAnnotation), new PropertyMetadata(null, OnContentPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="ContentTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register(nameof(ContentTemplate), typeof(DataTemplate), typeof(CartesianCustomAnnotation), new PropertyMetadata(null, OnContentTemplatePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="HorizontalOffset"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register(nameof(HorizontalOffset), typeof(double), typeof(CartesianCustomAnnotation), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the <see cref="VerticalOffset"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register(nameof(VerticalOffset), typeof(double), typeof(CartesianCustomAnnotation), new PropertyMetadata(0d));

        private CartesianCustomAnnotationModel model;
        private ContentPresenter presenter;

        /// <summary>
        /// Initializes a new instance of the <see cref="CartesianCustomAnnotation" /> class.
        /// </summary>
        public CartesianCustomAnnotation()
        {
            this.DefaultStyleKey = typeof(CartesianCustomAnnotation);

            this.model = new CartesianCustomAnnotationModel();
            this.PreparePresenter();
        }

        /// <summary>
        /// Gets or sets the horizontal axis.
        /// </summary>
        /// <value>The horizontal axis.</value>
        public CartesianAxis HorizontalAxis
        {
            get
            {
                return (CartesianAxis)this.GetValue(HorizontalAxisProperty);
            }
            set
            {
                this.SetValue(HorizontalAxisProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the vertical axis.
        /// </summary>
        /// <value>The vertical axis.</value>
        public CartesianAxis VerticalAxis
        {
            get
            {
                return (CartesianAxis)this.GetValue(VerticalAxisProperty);
            }
            set
            {
                this.SetValue(VerticalAxisProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the horizontal value.
        /// </summary>
        /// <value>The horizontal value.</value>
        public object HorizontalValue
        {
            get
            {
                return this.GetValue(HorizontalValueProperty);
            }
            set
            {
                this.SetValue(HorizontalValueProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the vertical value.
        /// </summary>
        /// <value>The vertical value.</value>
        public object VerticalValue
        {
            get
            {
                return this.GetValue(VerticalValueProperty);
            }
            set
            {
                this.SetValue(VerticalValueProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public object Content
        {
            get
            {
                return this.GetValue(ContentProperty);
            }
            set
            {
                this.SetValue(ContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the content template.
        /// </summary>
        /// <value>
        /// The content template.
        /// </value>
        public DataTemplate ContentTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(ContentTemplateProperty);
            }
            set
            {
                this.SetValue(ContentTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the horizontal offset.
        /// </summary>
        /// <value>The horizontal offset.</value>
        public double HorizontalOffset
        {
            get
            {
                return (double)this.GetValue(HorizontalOffsetProperty);
            }
            set
            {
                this.SetValue(HorizontalOffsetProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the vertical offset.
        /// </summary>
        /// <value>The vertical offset.</value>
        public double VerticalOffset
        {
            get
            {
                return (double)this.GetValue(VerticalOffsetProperty);
            }
            set
            {
                this.SetValue(VerticalOffsetProperty, value);
            }
        }

        internal override ChartAnnotationModel Model
        {
            get
            {
                return this.model;
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
                base.UpdateVisibility();
            }
            else
            {
                this.SetPresenterVisibility(Visibility.Collapsed);
            }
        }

        internal override void UpdatePresenters()
        {
            RadRect slot = this.GetPresenterSlot();

            this.ArrangeUIElement(this.presenter, slot);
        }

        internal override void ChartAxisChanged(CartesianAxis oldAxis, CartesianAxis newAxis)
        {
            base.ChartAxisChanged(oldAxis, newAxis);
            if (this.HorizontalAxis == null)
            {
                if (oldAxis != null && oldAxis.type == AxisType.First)
                {
                    this.model.DetachAxis(oldAxis.model);
                }
                if (newAxis != null && newAxis.type == AxisType.First)
                {
                    this.model.AttachAxis(newAxis.model, AxisType.First);
                }
            }
            if (this.VerticalAxis == null)
            {
                if (oldAxis != null && oldAxis.type == AxisType.Second)
                {
                    this.model.DetachAxis(oldAxis.model);
                }
                if (newAxis != null && newAxis.type == AxisType.Second)
                {
                    this.model.AttachAxis(newAxis.model, AxisType.Second);
                }
            }
        }

        /// <summary>
        /// Core entry point for calculating the size of a node's content.
        /// </summary>      
        protected internal override RadSize MeasureNodeOverride(Node node, object content)
        {
            this.presenter.ClearValue(FrameworkElement.WidthProperty);
            this.presenter.ClearValue(FrameworkElement.HeightProperty);

            this.presenter.Measure(RadChartBase.InfinitySize);

            RadSize size = new RadSize((int)(this.presenter.DesiredSize.Width + .5), (int)(this.presenter.DesiredSize.Height + .5));

            return size;
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

        /// <summary>
        /// Occurs when the presenter has been successfully attached to its owning <see cref="RadChartBase" /> instance.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            RadCartesianChart cartesianChart = this.chart as RadCartesianChart;

            if (this.HorizontalAxis == null && cartesianChart.HorizontalAxis != null)
            {
                this.model.FirstAxis = cartesianChart.HorizontalAxis.model;
            }
            if (this.VerticalAxis == null && cartesianChart.VerticalAxis != null)
            {
                this.model.SecondAxis = cartesianChart.VerticalAxis.model;
            }
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new CartesianCustomAnnotationAutomationPeer(this);
        }

        private static void OnHorizontalAxisPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CartesianCustomAnnotation annotation = sender as CartesianCustomAnnotation;
            if (annotation.HorizontalAxis != null)
            {
                annotation.model.FirstAxis = annotation.HorizontalAxis.model;
            }
            else
            {
                annotation.model.FirstAxis = null;
            }
        }

        private static void OnVerticalAxisPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CartesianCustomAnnotation annotation = sender as CartesianCustomAnnotation;
            if (annotation.VerticalAxis != null)
            {
                annotation.model.SecondAxis = annotation.VerticalAxis.model;
            }
            else
            {
                annotation.model.SecondAxis = null;
            }
        }

        private static void OnHorizontalValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CartesianCustomAnnotation annotation = sender as CartesianCustomAnnotation;
            annotation.model.FirstValue = annotation.HorizontalValue;
        }

        private static void OnVerticalValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CartesianCustomAnnotation annotation = sender as CartesianCustomAnnotation;
            annotation.model.SecondValue = annotation.VerticalValue;
        }

        private static void OnContentPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CartesianCustomAnnotation annotation = sender as CartesianCustomAnnotation;
            annotation.InvalidateContentTemplate();
        }

        private static void OnContentTemplatePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CartesianCustomAnnotation annotation = sender as CartesianCustomAnnotation;
            annotation.InvalidateContentTemplate();
        }

        private void InvalidateContentTemplate()
        {
            this.model.desiredSize = RadSize.Invalid;

            this.InvalidateCore();
        }

        private RadRect GetPresenterSlot()
        {
            RadPoint pointPosition = this.model.layoutSlot.Location;
            double width = this.model.layoutSlot.Width;
            double height = this.model.layoutSlot.Height;
            double horizontalOffset = this.HorizontalOffset;
            double verticalOffset = this.VerticalOffset;
            HorizontalAlignment horizontalAlignment = this.HorizontalAlignment;
            VerticalAlignment verticalAlignment = this.VerticalAlignment;

            double x = pointPosition.X;
            switch (horizontalAlignment)
            {
                case HorizontalAlignment.Center:
                    {
                        x -= width / 2;
                        x += horizontalOffset;
                    }
                    break;
                case HorizontalAlignment.Left:
                    {
                        x -= width;
                        x += horizontalOffset;
                    }
                    break;
                case HorizontalAlignment.Right:
                    {
                        x += horizontalOffset;
                    }
                    break;
            }

            double y = pointPosition.Y;
            switch (verticalAlignment)
            {
                case VerticalAlignment.Center:
                    {
                        y -= height / 2;
                        y += verticalOffset;
                    }
                    break;
                case VerticalAlignment.Bottom:
                    {
                        y += verticalOffset;
                    }
                    break;
                case VerticalAlignment.Top:
                    {
                        y -= height;
                        y += verticalOffset;
                    }
                    break;
            }

            return new RadRect(x, y, width, height);
        }

        private void PreparePresenter()
        {
            this.presenter = new ContentPresenter();
            this.presenter.SetBinding(ContentPresenter.ContentProperty, new Binding() { Path = new PropertyPath("Content"), Source = this });
            this.presenter.SetBinding(ContentPresenter.ContentTemplateProperty, new Binding() { Path = new PropertyPath("ContentTemplate"), Source = this });
        }
    }
}