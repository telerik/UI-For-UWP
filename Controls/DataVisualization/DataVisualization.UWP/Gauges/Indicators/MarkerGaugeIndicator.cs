using System;
using Telerik.UI.Automation.Peers;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.DataVisualization
{
    /// <summary>
    /// This class represents a marker indicator that is
    /// usually placed somewhere near the ticks of the owner range.
    /// </summary>
    [TemplatePart(Name = "PART_Canvas", Type = typeof(Canvas))]
    [TemplatePart(Name = "PART_VisualElement", Type = typeof(ContentPresenter))]
    [TemplatePart(Name = "PART_VisualElementRotation", Type = typeof(RotateTransform))]
    public class MarkerGaugeIndicator : GaugeIndicator
    {
        /// <summary>
        /// Identifies the <see cref="ContentTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register(nameof(ContentTemplate), typeof(DataTemplate), typeof(MarkerGaugeIndicator), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Content"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(nameof(Content), typeof(object), typeof(MarkerGaugeIndicator), new PropertyMetadata(null, OnContentPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="IsRotated"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsRotatedProperty =
            DependencyProperty.Register(nameof(IsRotated), typeof(bool), typeof(MarkerGaugeIndicator), new PropertyMetadata(true, OnIsRotatedPropertyChanged));

        private const string VisualElementPartName = "PART_VisualElement";
        private const string VisualElementRotationPartName = "PART_VisualElementRotation";
        private const string CanvasPartName = "PART_Canvas";

        private ContentPresenter visualElement;
        private RotateTransform visualElementRotation;
        private Canvas canvas;
        private double initialRotationAngle;
        private bool isHorizontal = true;
        private bool isUserContent;

        private double minValue;
        private double maxAngle;
        private double minAngle;

        /// <summary>
        /// Initializes a new instance of the MarkerGaugeIndicator class.
        /// </summary>
        public MarkerGaugeIndicator()
        {
            this.DefaultStyleKey = typeof(MarkerGaugeIndicator);
        }

        /// <summary>
        /// Gets or sets the template for the marker.
        /// </summary>
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
        /// Gets or sets the template for the marker.
        /// </summary>
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
        /// Gets or sets a value indicating whether the marker will rotate in place as well as around the center
        /// of a radial range or whether it will rotate automatically in a linear range when its orientation is Vertical.
        /// </summary>
        public bool IsRotated
        {
            get
            {
                return (bool)this.GetValue(IsRotatedProperty);
            }

            set
            {
                this.SetValue(IsRotatedProperty, value);
            }
        }

        internal ContentPresenter VisualElement
        {
            get
            {
                return this.visualElement;
            }
        }

        /// <summary>
        /// A virtual method that is called when the value of this indicator changes.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        /// <param name="oldValue">The old value.</param>
        internal override void OnValueChanged(double newValue, double oldValue)
        {
            base.OnValueChanged(newValue, oldValue);

            this.UpdateVisualContent(newValue);
        }

        /// <summary>
        /// A virtual method that is called when the start value of this indicator changes.
        /// </summary>
        /// <param name="newStartValue">The new start value.</param>
        /// <param name="oldStartValue">The old start value.</param>
        internal override void OnStartValueChanged(double newStartValue, double oldStartValue)
        {
            base.OnStartValueChanged(newStartValue, oldStartValue);

            this.Value = newStartValue;
        }

        /// <summary>
        /// This is a virtual method that resets the state of the indicator.
        /// The parent range is responsible to (indirectly) call this method when
        /// a property of importance changes.
        /// <param name="availableSize">A size which can be used by the update logic.</param>
        /// </summary>
        /// <remarks>
        /// The linear range for example triggers the UpdateOverride() method
        /// of its indicators when its Orientation property changes.
        /// </remarks>
        internal override void UpdateOverride(Size availableSize)
        {
            base.UpdateOverride(availableSize);

            this.PrivateUpdate();

            this.UpdateIsRotated();
            this.ArrangeMarker(availableSize, this.ActualValue);
            this.UpdateRotationInLinearRange();
        }

        /// <summary>
        /// Initializes the template parts of the indicator (see the TemplatePart attributes for more information).
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            base.ApplyTemplateCore();

            this.visualElement = this.GetTemplatePartField<ContentPresenter>(VisualElementPartName);
            bool applied = this.visualElement != null;

            this.visualElementRotation = this.GetTemplatePartField<RotateTransform>(VisualElementRotationPartName);
            applied = applied && this.visualElementRotation != null;

            this.canvas = this.GetTemplatePartField<Canvas>(CanvasPartName);
            applied = applied && this.canvas != null;

            return applied;
        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate"/> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            this.initialRotationAngle = this.visualElementRotation.Angle;
            this.UpdateVisualContent(this.Value);
        }

        /// <summary>
        /// Called in the measure layout pass to determine the desired size.
        /// </summary>
        /// <param name="availableSize">The available size that was given by the layout system.</param>
        /// <returns>Returns the desired size of the indicator.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            Size baseSize = base.MeasureOverride(availableSize);
            this.visualElement.Measure(this.Owner.LastMeasureSize);

            return baseSize;
        }

        /// <summary>
        /// Called in the arrange pass of the layout system.
        /// </summary>
        /// <param name="finalSize">The final size that was given by the layout system.</param>
        /// <returns>The final size of the panel.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            this.ArrangeMarker(finalSize, this.ActualValue);
            return base.ArrangeOverride(finalSize);
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new MarkerGaugeIndicatorAutomationPeer(this);
        }

        private static void OnIsRotatedPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            MarkerGaugeIndicator indicator = sender as MarkerGaugeIndicator;
            if (!indicator.IsTemplateApplied)
            {
                return;
            }

            if (!(bool)args.NewValue)
            {
                indicator.visualElementRotation.Angle = indicator.initialRotationAngle;
            }
            indicator.UpdateRotationInLinearRange();

            var peer = FrameworkElementAutomationPeer.CreatePeerForElement(indicator) as MarkerGaugeIndicatorAutomationPeer;
            if (peer != null)
            {
                peer.RaiseToggleStatePropertyChangedEvent((bool)args.OldValue, (bool)args.NewValue);
            }
        }

        private static void OnContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MarkerGaugeIndicator indicator = d as MarkerGaugeIndicator;
            if (indicator.IsInternalPropertyChange)
            {
                return;
            }

            indicator.isUserContent = e.NewValue != null;
        }

        private static Point MultiplyPointByScalar(Point point, double scalar)
        {
            return new Point(point.X * scalar, point.Y * scalar);
        }

        private static Point AddPoints(Point left, Point right)
        {
            return new Point(left.X + right.X, left.Y + right.Y);
        }

        private void UpdateVisualContent(double value)
        {
            if (!this.isUserContent)
            {
                this.ChangePropertyInternally(ContentProperty, value);
            }
            this.ArrangeMarker(this.DesiredSize, value);
        }

        private void ArrangeMarker(Size desiredSize, double newValue)
        {
            if (this.Owner == null)
            {
                return;
            }

            double radius = Math.Min(desiredSize.Width, desiredSize.Height) / 2;

            switch (this.Owner.OwnerGauge.GaugeType)
            {
                case GaugeType.Linear:
                    double distance = RadGauge.MapLogicalToPhysicalValue(newValue - this.minValue, this.isHorizontal ? desiredSize.Width : desiredSize.Height, this.MinMaxValueDifference);
                    this.ArrangeLinearly(distance, this.CreateStartPoint(desiredSize), this.CreateDirection());
                    break;
                case GaugeType.Radial:
                    double angleInDegrees = RadGauge.MapLogicalToPhysicalValue(newValue - this.Owner.OwnerGauge.MinValue, this.maxAngle - this.minAngle, this.MinMaxValueDifference);
                    double currentAngle = this.minAngle + angleInDegrees;

                    if (this.IsRotated)
                    {
                        this.visualElementRotation.Angle = this.initialRotationAngle + currentAngle;
                    }
                    else
                    {
                        this.visualElementRotation.Angle = this.initialRotationAngle;
                    }

                    double angleInRadians = RadialGaugePanel.ConvertDegreesToRadians(currentAngle);
                    Point center = new Point((desiredSize.Width / 2) - (this.visualElement.DesiredSize.Width / 2), (desiredSize.Height / 2) - (this.visualElement.DesiredSize.Height / 2));
                    this.ArrangeRadially(angleInRadians, center, radius);
                    break;
            }
        }

        private void PrivateUpdate()
        {
            if (this.Owner != null && this.Owner.OwnerGauge is RadRadialGauge)
            {
                var gauge = (RadRadialGauge)this.Owner.OwnerGauge;
                this.maxAngle = gauge.MaxAngle;
                this.minAngle = gauge.MinAngle;
            }

            this.isHorizontal = RadLinearGauge.GetOrientation(this.Owner) == Orientation.Horizontal;
            this.minValue = this.Owner.OwnerGauge.MinValue;
        }

        private void UpdateIsRotated()
        {
            if (this.Owner.OwnerGauge.GaugeType != GaugeType.Linear)
            {
                return;
            }

            object isMarkerRotated = this.ReadLocalValue(MarkerGaugeIndicator.IsRotatedProperty);
            if (isMarkerRotated == DependencyProperty.UnsetValue)
            {
                this.IsRotated = false;
            }
        }

        private void UpdateRotationInLinearRange()
        {
            if (this.Owner.OwnerGauge.GaugeType != GaugeType.Linear || !this.IsRotated)
            {
                return;
            }

            if (this.isHorizontal)
            {
                this.visualElementRotation.Angle = 0;
            }
            else
            {
                this.visualElementRotation.Angle = 90;
            }
        }

        private Point CreateStartPoint(Size rangeSize)
        {
            double halfVisualElementWidth = this.visualElement.DesiredSize.Width / 2;
            double halfVisualElementHeight = this.visualElement.DesiredSize.Height / 2;

            if (this.isHorizontal)
            {
                return new Point(-halfVisualElementWidth, ((rangeSize.Height / 2) - halfVisualElementHeight) + RadLinearGauge.GetIndicatorOffset(this));
            }

            return new Point(((rangeSize.Width / 2) - halfVisualElementWidth) + RadLinearGauge.GetIndicatorOffset(this), rangeSize.Height - halfVisualElementHeight);
        }

        private Point CreateDirection()
        {
            if (this.isHorizontal)
            {
                return new Point(1, 0);
            }

            return new Point(0, -1);
        }

        private void ArrangeLinearly(double distance, Point startPoint, Point direction)
        {
            Point location = AddPoints(startPoint, MultiplyPointByScalar(direction, distance));
            Canvas.SetLeft(this.visualElement, location.X);
            Canvas.SetTop(this.visualElement, location.Y);
        }

        private void ArrangeRadially(double angle, Point center, double radius)
        {
            Point location = RadialGaugePanel.CreateRotatedPoint(angle, radius, center, RadRadialGauge.GetIndicatorRadiusScale(this));
            Canvas.SetLeft(this.visualElement, location.X);
            Canvas.SetTop(this.visualElement, location.Y);
        }
    }
}
