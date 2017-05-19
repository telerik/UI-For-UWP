using System;
using Telerik.UI.Automation.Peers;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.DataVisualization
{
    /// <summary>
    /// Represents a line indicator with a specific color and thickness.
    /// </summary>
    ////[TemplatePart(Name = BarPartName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_Bar", Type = typeof(Rectangle))]
    public class LinearBarGaugeIndicator : BarGaugeIndicator
    {
        private const string BarPartName = "PART_Bar";

        private FrameworkElement bar;
        private bool isHorizontal = true;
        private double barLocation = 0;

        /// <summary>
        /// Initializes a new instance of the LinearBarGaugeIndicator class.
        /// </summary>
        public LinearBarGaugeIndicator()
        {
            this.DefaultStyleKey = typeof(LinearBarGaugeIndicator);
        }

        /// <summary>
        /// This method defines how a particular indicator will
        /// arrange itself in the parent range.
        /// </summary>
        /// <param name="finalSize">The size in which the indicator should arrange itself.</param>
        internal override Rect GetArrangeRect(Size finalSize)
        {
            this.UpdateBarLocation(this.StartValue);

            return new Rect(new Point(), this.DesiredSize);
        }

        /// <summary>
        /// A virtual method that is called when the Owner of this indicator changes.
        /// </summary>
        /// <param name="newOwner">The new Owner.</param>
        /// <param name="oldOwner">The old Owner.</param>
        internal override void OnOwnerChanged(GaugePanel newOwner, GaugePanel oldOwner)
        {
            base.OnOwnerChanged(newOwner, oldOwner);

            Orientation orientation = RadLinearGauge.GetOrientation(newOwner);
            this.isHorizontal = orientation == Orientation.Horizontal;
            RadLinearGauge.SetOrientation(this, orientation);
        }

        /// <summary>
        /// A virtual method that is called when the thickness of this indicator changes.
        /// </summary>
        /// <param name="newThickness">The new thickness.</param>
        internal override void OnThicknessChanged(double newThickness)
        {
            base.OnThicknessChanged(newThickness);

            this.UpdateBarThickness(newThickness);
        }

        /// <summary>
        /// A virtual method that is called when the value of this indicator changes.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        /// <param name="oldValue">The old value.</param>
        internal override void OnValueChanged(double newValue, double oldValue)
        {
            base.OnValueChanged(newValue, oldValue);

            if (!this.IsTemplateApplied)
            {
                return;
            }

            this.SetLocation(this.ArrangeRectangle(newValue, new Size(this.Owner.ActualWidth, this.Owner.ActualHeight)));
        }

        /// <summary>
        /// A virtual method that is called when the start value of this indicator changes.
        /// </summary>
        /// <param name="newStartValue">The new start value.</param>
        /// <param name="oldStartValue">The old start value.</param>
        internal override void OnStartValueChanged(double newStartValue, double oldStartValue)
        {
            base.OnStartValueChanged(newStartValue, oldStartValue);

            this.UpdateBarLocation(newStartValue);
        }

        /// <summary>
        /// This is a virtual method that resets the state of the indicator.
        /// The parent range is responsible to (indirectly) call this method when
        /// a property of importance changes.
        /// </summary>
        /// <param name="availableSize">A size which can be used by the update logic.</param>
        /// <remarks>
        /// The linear range for example triggers the UpdateOverride() method
        /// of its indicators when its Orientation property changes.
        /// </remarks>
        internal override void UpdateOverride(Size availableSize)
        {
            base.UpdateOverride(availableSize);

            this.isHorizontal = RadLinearGauge.GetOrientation(this) == Orientation.Horizontal;

            this.UpdateBarLocation(this.StartValue);
        }

        /// <summary>
        /// Initializes the template parts of the indicator (see the TemplatePart attributes for more information).
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            base.ApplyTemplateCore();

            this.bar = this.GetTemplatePartField<FrameworkElement>(BarPartName);
            return this.bar != null;
        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate"/> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            this.UpdateBarThickness(this.Thickness);
        }

        /// <inheritdoc/>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new LinearBarGaugeIndicatorAutomationPeer(this);
        }

        private void SetLocation(Point location)
        {
            Canvas.SetLeft(this.bar, location.X);
            Canvas.SetTop(this.bar, location.Y);
        }

        private Point ArrangeRectangle(double value, Size finalSize)
        {
            Point result = new Point();

            double finalValue = Math.Abs(value - this.StartValue);
            double extent = RadGauge.MapLogicalToPhysicalValue(finalValue, this.isHorizontal ? finalSize.Width : finalSize.Height, this.MinMaxValueDifference);

            bool flip = this.Value < this.StartValue;
            double barLength = extent;

            if (this.isHorizontal)
            {
                this.bar.Width = barLength;
                this.bar.Height = this.Thickness;

                result.X = flip ? this.barLocation - extent : this.barLocation;
                result.Y = ((finalSize.Height / 2) - (this.bar.Height / 2)) + RadLinearGauge.GetIndicatorOffset(this);
            }
            else
            {
                this.bar.Height = barLength;
                this.bar.Width = this.Thickness;

                result.X = ((finalSize.Width / 2) - (this.bar.Width / 2)) + RadLinearGauge.GetIndicatorOffset(this);
                result.Y = flip ? this.barLocation : this.barLocation - extent;
            }

            return result;
        }

        private void UpdateBarLocation(double startValue)
        {
            double minValue = Math.Min(this.Owner.OwnerGauge.MinValue, this.Owner.OwnerGauge.MaxValue);
            double extent = RadGauge.MapLogicalToPhysicalValue(startValue - minValue, this.isHorizontal ? this.Owner.LastMeasureSize.Width : this.Owner.LastMeasureSize.Height, this.MinMaxValueDifference);

            if (this.isHorizontal)
            {
                this.barLocation = extent;
            }
            else
            {
                this.barLocation = this.Owner.LastMeasureSize.Height - extent;
            }

            this.SetLocation(this.ArrangeRectangle(this.ActualValue, this.Owner.LastMeasureSize));
        }

        private void UpdateBarThickness(double thickness)
        {
            if (this.isHorizontal)
            {
                this.bar.Height = thickness;
            }
            else
            {
                this.bar.Width = thickness;
            }
        }
    }
}
