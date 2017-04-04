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
    /// Allows you to place a custom object at a specific place in your <see cref="RadPolarChart"/>. 
    /// </summary>
    public class PolarCustomAnnotation : PolarChartAnnotation
    {
        /// <summary>
        /// Identifies the <see cref="RadialValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RadialValueProperty = DependencyProperty.Register(nameof(RadialValue), typeof(object), typeof(PolarCustomAnnotation), new PropertyMetadata(null, OnRadialValuePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="PolarValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PolarValueProperty = DependencyProperty.Register(nameof(PolarValue), typeof(double), typeof(PolarCustomAnnotation), new PropertyMetadata(double.NaN, OnPolarValuePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="Content"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(nameof(Content), typeof(object), typeof(PolarCustomAnnotation), new PropertyMetadata(null, OnContentPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="ContentTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentTemplateProperty = DependencyProperty.Register(nameof(ContentTemplate), typeof(DataTemplate), typeof(PolarCustomAnnotation), new PropertyMetadata(null, OnContentTemplatePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="HorizontalOffset"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalOffsetProperty = DependencyProperty.Register(nameof(HorizontalOffset), typeof(double), typeof(PolarCustomAnnotation), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the <see cref="VerticalOffset"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalOffsetProperty = DependencyProperty.Register(nameof(VerticalOffset), typeof(double), typeof(PolarCustomAnnotation), new PropertyMetadata(0d));

        private PolarCustomAnnotationModel model;
        private ContentPresenter presenter;

        /// <summary>
        /// Initializes a new instance of the <see cref="PolarCustomAnnotation" /> class.
        /// </summary>
        public PolarCustomAnnotation()
        {
            this.DefaultStyleKey = typeof(PolarCustomAnnotation);

            this.model = new PolarCustomAnnotationModel();
            this.PreparePresenter();
        }

        /// <summary>
        /// Gets or sets the radial value.
        /// </summary>
        /// <value>The radial value.</value>
        public object RadialValue
        {
            get
            {
                return this.GetValue(RadialValueProperty);
            }
            set
            {
                this.SetValue(RadialValueProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the polar value.
        /// </summary>
        /// <value>The polar value.</value>
        public double PolarValue
        {
            get
            {
                return (double)this.GetValue(PolarValueProperty);
            }
            set
            {
                this.SetValue(PolarValueProperty, value);
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

        internal override void ChartAxisChanged(Axis oldAxis, Axis newAxis)
        {
            base.ChartAxisChanged(oldAxis, newAxis);

            if (newAxis != null)
            {
                if (newAxis.type == AxisType.First)
                {
                    this.model.FirstAxis = newAxis.model;
                }
                else
                {
                    this.model.SecondAxis = newAxis.model;
                }
            }
            else
            {
                if (oldAxis.type == AxisType.First)
                {
                    this.model.FirstAxis = null;
                }
                else
                {
                    this.model.SecondAxis = null;
                }
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
            RadRect slot = this.GetPresenterSlot();

            this.ArrangeUIElement(this.presenter, slot);
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

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new PolarCustomAnnotationAutomationPeer(this);
        }

        private static void OnRadialValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PolarCustomAnnotation annotation = sender as PolarCustomAnnotation;
            annotation.model.SecondValue = annotation.RadialValue;
        }

        private static void OnPolarValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PolarCustomAnnotation annotation = sender as PolarCustomAnnotation;
            annotation.model.FirstValue = annotation.PolarValue;
        }

        private static void OnContentPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PolarCustomAnnotation annotation = sender as PolarCustomAnnotation;
            annotation.InvalidateContentTemplate();
        }

        private static void OnContentTemplatePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PolarCustomAnnotation annotation = sender as PolarCustomAnnotation;
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