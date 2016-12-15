using System;
using System.Diagnostics.CodeAnalysis;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.DataVisualization
{
    /// <summary>
    /// Represents a radial range which arranges its ticks and labels
    /// in a circle and defines attached properties for the contained
    /// indicators.
    /// </summary>
    public class RadialGaugePanel : GaugePanel
    {
        private SweepDirection sweepDir;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadialGaugePanel"/> class.
        /// </summary>
        public RadialGaugePanel()
        {
            this.sweepDir = SweepDirection.Clockwise;
        }

        /// <summary>
        /// Creates a point rotated angle radians around the center. The point will
        /// be radius * radiusScale away from the center.
        /// </summary>
        /// <param name="angle">Determines how much the resulting point will be rotated.</param>
        /// <param name="radius">The radius of the circle over which to rotate the point.</param>
        /// <param name="center">The center of rotation.</param>
        /// <param name="radiusScale">A scale factor that the radius will be multiplied by.</param>
        /// <returns>Returns a point rotated according to the parameters.</returns>
        public static Point CreateRotatedPoint(double angle, double radius, Point center, double radiusScale)
        {
            Point result = new Point();
            double scaledRadius = radius * radiusScale;
            result.X = center.X + (-Math.Cos(angle) * scaledRadius);
            result.Y = center.Y + (-Math.Sin(angle) * scaledRadius);

            return result;
        }

        /// <summary>
        /// Converts the provided value in degrees to radians.
        /// </summary>
        /// <param name="angleInDegrees">The angle in degrees to convert.</param>
        /// <returns>Returns the provided value in degrees converted to radians.</returns>
        internal static double ConvertDegreesToRadians(double angleInDegrees)
        {
            return angleInDegrees * Math.PI / 180;
        }

        /// <summary>
        /// Calculates and angle in degrees based on a value, a value range, an angle range and a sweep direction.
        /// </summary>
        /// <param name="context">A context that contains the value to be converted to an angle.</param>
        /// <returns>Returns an angle in radians that corresponds to the provided value in the context.</returns>
        internal static double CalculateAngle(CalculateAngleContext context)
        {
            double angleDeg = RadGauge.MapLogicalToPhysicalValue(context.value - context.startValue, context.gaugePhysicalLength, context.gaugeLogicalLength);

            var angle = RadialGaugePanel.ConvertDegreesToRadians(angleDeg);
            return RadialGaugePanel.ConvertDegreesToRadians(context.minAngle) + angle;
        }

        /// <summary>
        /// Defines the arrange logic for the ticks.
        /// </summary>
        /// <param name="finalSize">The size in which the ticks can be arranged.</param>
        internal override void ArrangeTicksOverride(Size finalSize)
        {
            int tickCount = this.TickCount;
            if (tickCount == 0 || this.OwnerGauge.TickStep == 0)
            {
                return;
            }

            var gauge = this.OwnerGauge as RadRadialGauge;

            double tickRadiusScale = gauge.TickRadiusScale;
            double minValue = this.OwnerGauge.MinValue;
            double maxValue = this.OwnerGauge.MaxValue;
            double tickStep = this.OwnerGauge.TickStep;
            double minAngle = gauge.MinAngle;
            double maxAngle = gauge.MaxAngle;
            
            int tickCounter = 0;
            for (double tickValue = 0; tickCounter < tickCount; tickValue += tickStep, tickCounter++)
            {
                double angle = RadGauge.MapLogicalToPhysicalValue(tickValue, maxAngle - minAngle, maxValue - minValue);
                this.ArrangeTick(this.GetTick(tickCounter), finalSize, angle, tickRadiusScale);
            }
        }

        /// <summary>
        /// Defines the arrange logic for the labels.
        /// </summary>
        /// <param name="finalSize">The size in which the labels can be arranged.</param>
        internal override void ArrangeLabelsOverride(Size finalSize)
        {
            if (this.LabelCount == 0 || this.OwnerGauge.LabelStep == 0)
            {
                return;
            }

            var gauge = this.OwnerGauge as RadRadialGauge;

            double labelRadiusScale = gauge.LabelRadiusScale;
            double minValue = this.OwnerGauge.MinValue;
            double maxValue = this.OwnerGauge.MaxValue;
            double labelStep = this.OwnerGauge.LabelStep;
            double minAngle = gauge.MinAngle;
            double maxAngle = gauge.MaxAngle;

            int j = 0;
            for (double i = minValue; i <= maxValue; i += labelStep, j++)
            {
                double angle = RadGauge.MapLogicalToPhysicalValue(i - minValue, maxAngle - minAngle, maxValue - minValue);
                this.ArrangeLabel(this.GetLabel(j), finalSize, angle, labelRadiusScale);
            }
        }

        /// <summary>
        /// A virtual method that is called for each indicator in the arrange layout pass.
        /// </summary>
        /// <param name="indicator">The indicator to arrange.</param>
        /// <param name="finalSize">The size in which to arrange the indicator.</param>
        /// <remarks>
        /// The rect in which the indicator will be arranged should be retrieved from
        /// the indicator itself via the GetArrangeRect() method.
        /// </remarks>
        internal override void ArrangeIndicator(GaugeIndicator indicator, Size finalSize)
        {
            Rect indicatorRect = indicator.GetArrangeRect(finalSize);
            indicator.Arrange(indicatorRect);

            Size indicatorSize = new Size(indicatorRect.Width, indicatorRect.Height);
            Point indicatorOffset = GetIndicatorOffset(indicatorSize, finalSize);

            Canvas.SetLeft(indicator, indicatorOffset.X);
            Canvas.SetTop(indicator, indicatorOffset.Y);
        }

        /// <summary>
        /// A virtual method that is called when the TickRadiusScale property changes.
        /// </summary>
        /// <param name="newTickRadiusScale">The new TickRadiusScale.</param>
        /// <param name="oldTickRadiusScale">The old TickRadiusScale.</param>
        internal virtual void OnTickRadiusScaleChanged(double newTickRadiusScale, double oldTickRadiusScale)
        {
            this.ArrangeTicksOverride(new Size(this.ActualWidth, this.ActualHeight));
        }

        /// <summary>
        /// A virtual method that is called when the LabelRadiusScale property changes.
        /// </summary>
        /// <param name="newLabelRadiusScale">The new LabelRadiusScale.</param>
        /// <param name="oldLabelRadiusScale">The old LabelRadiusScale.</param>
        internal virtual void OnLabelRadiusScaleChanged(double newLabelRadiusScale, double oldLabelRadiusScale)
        {
            this.ArrangeLabelsOverride(new Size(this.ActualWidth, this.ActualHeight));
        }

        private static Point GetIndicatorOffset(Size indicatorSise, Size rangeSize)
        {
            double halfRangeWidth = rangeSize.Width / 2;
            double halfRangeHeight = rangeSize.Height / 2;

            double halfIndicatorWidth = indicatorSise.Width / 2;
            double halfIndicatorHeight = indicatorSise.Height / 2;

            double offsetX = halfRangeWidth - halfIndicatorWidth;
            double offsetY = halfRangeHeight - halfIndicatorHeight;

            return new Point(offsetX, offsetY);
        }

        private void ArrangeLabel(ContentPresenter label, Size finalSize, double currentTickAngle, double radiusScale)
        {
            Size desiredSize = label.DesiredSize;
            double radius = Math.Min(finalSize.Width, finalSize.Height) / 2;
            Point center = GetIndicatorOffset(desiredSize, finalSize);

            var gauge = this.OwnerGauge as RadRadialGauge;

            double angle = 0;
            if (this.sweepDir == SweepDirection.Clockwise)
            {
                angle = RadialGaugePanel.ConvertDegreesToRadians(gauge.MinAngle) + RadialGaugePanel.ConvertDegreesToRadians(currentTickAngle);
            }
            else
            {
                angle = RadialGaugePanel.ConvertDegreesToRadians(gauge.MaxAngle) - RadialGaugePanel.ConvertDegreesToRadians(currentTickAngle);
            }

            Point labelPosition = CreateRotatedPoint(angle, radius, center, radiusScale);
            if ((TickType)label.GetValue(RadGauge.TickTypeProperty) == TickType.Label)
            {
                labelPosition.X -= desiredSize.Width * Math.Cos(angle) / 3;
            }

            label.Arrange(new Rect(labelPosition, desiredSize));
        }

        private void ArrangeTick(ContentPresenter tick, Size finalSize, double currentTickAngle, double radiusScale)
        {
            this.ArrangeLabel(tick, finalSize, currentTickAngle, radiusScale);

            double angle = 0;
            if (this.sweepDir == SweepDirection.Clockwise)
            {
                angle = ((RadRadialGauge)this.OwnerGauge).MinAngle + currentTickAngle;
            }
            else
            {
                angle = ((RadRadialGauge)this.OwnerGauge).MaxAngle - currentTickAngle;
            }

            tick.RenderTransformOrigin = new Point(0.5, 0.5);
            tick.RenderTransform = new RotateTransform() { Angle = angle };
        }

        private void UpdateTicksAndLabels()
        {
            Size finalSize = new Size(this.ActualWidth, this.ActualHeight);
            this.ArrangeTicksOverride(finalSize);
            this.ArrangeLabelsOverride(finalSize);
        }
    }
}
