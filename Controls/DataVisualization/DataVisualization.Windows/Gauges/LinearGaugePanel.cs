using System;
using System.Diagnostics.CodeAnalysis;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.DataVisualization
{
    /// <summary>
    /// Represents a linear range that arranges its ticks
    /// and labels in a straight line and defines attached
    /// properties for the contained indicators.
    /// </summary>
    public class LinearGaugePanel : GaugePanel
    {
        private RotateTransform verticalTickTransform = new RotateTransform() { Angle = -90 };

        private double availableExtent;
        private double halfAvailableExtent;

        private double halfAvailableWidth;
        private double halfAvailableHeight;

        private bool isHorizontal = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="LinearGaugePanel"/> class.
        /// </summary>
        public LinearGaugePanel()
        {
        }

        internal bool IsHorizontal
        {
            get
            {
                return this.isHorizontal;
            }
        }

        /// <summary>
        /// Defines the arrange logic for the ticks.
        /// </summary>
        /// <param name="finalSize">The size in which the ticks can be arranged.</param>
        internal override void ArrangeTicksOverride(Size finalSize)
        {
            int tickCount = this.TickCount;

            for (int i = 0; i < tickCount; ++i)
            {
                ContentPresenter tick = this.GetTick(i);
                this.ArrangeTick(tick, (double)tick.GetValue(RadGauge.TickValueProperty));
            }
        }

        /// <summary>
        /// Defines the arrange logic for the labels.
        /// </summary>
        /// <param name="finalSize">The size in which the labels can be arranged.</param>
        internal override void ArrangeLabelsOverride(Size finalSize)
        {
            int labelCount = this.LabelCount;
            double offset = (this.OwnerGauge as RadLinearGauge).LabelOffset;

            for (int i = 0; i < labelCount; ++i)
            {
                ContentPresenter label = this.GetLabel(i);
                this.ArrangeLabel(label, (double)label.GetValue(RadGauge.TickValueProperty), offset);
            }
        }

        /// <summary>
        /// A virtual method that is called when the LabelOffset property changes.
        /// </summary>
        /// <param name="newLabelOffset">The new LabelOffset.</param>
        /// <param name="oldLabelOffset">The old LabelOffset.</param>
        internal virtual void OnLabelOffsetChanged(double newLabelOffset, double oldLabelOffset)
        {
            this.ArrangeLabelsOverride(new Size(this.ActualWidth, this.ActualHeight));
        }

        /// <summary>
        /// A virtual method that is called when the Orientation property changes.
        /// </summary>
        /// <param name="newOrientation">The new Orientation.</param>
        /// <param name="oldOrientation">The old Orientation.</param>
        internal virtual void OnOrientationChanged(Orientation newOrientation, Orientation oldOrientation)
        {
            this.isHorizontal = newOrientation == Orientation.Horizontal;

            foreach (UIElement element in this.Indicators)
            {
                GaugeIndicator indicator = element as GaugeIndicator;
                if (indicator == null)
                {
                    continue;
                }

                RadLinearGauge.SetOrientation(indicator, newOrientation);
            }

            this.InvalidateMeasure();
        }

        /// <summary>
        /// Called in the arrange pass of the layout system.
        /// </summary>
        /// <param name="finalSize">The final size that was given by the layout system.</param>
        /// <returns>The final size of the panel.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            this.isHorizontal = RadLinearGauge.GetOrientation(this.OwnerGauge) == Orientation.Horizontal;

            this.availableExtent = this.isHorizontal ? finalSize.Width : finalSize.Height;
            this.halfAvailableExtent = this.availableExtent / 2;

            this.halfAvailableWidth = finalSize.Width / 2;
            this.halfAvailableHeight = finalSize.Height / 2;

            return base.ArrangeOverride(finalSize);
        }

        private void ArrangeTick(ContentPresenter tick, double value)
        {
            double halfTickHeight = tick.DesiredSize.Height / 2;
            double halfTickWidth = tick.DesiredSize.Width / 2;

            double minMaxDiff = Math.Abs(this.OwnerGauge.MaxValue - this.OwnerGauge.MinValue);
            double distance = RadGauge.MapLogicalToPhysicalValue(value, this.availableExtent, minMaxDiff);

            if (this.OwnerGauge.MaxValue < this.OwnerGauge.MinValue)
            {
                distance = this.availableExtent - distance;
            }

            double left = 0;
            double top = 0;

            if (this.isHorizontal)
            {
                tick.RenderTransform = null;
                left = distance - halfTickWidth;
                top = this.halfAvailableHeight - halfTickHeight;
            }
            else
            {
                tick.RenderTransform = this.verticalTickTransform;
                tick.RenderTransformOrigin = new Point(0.5, 0.5);
                left = this.halfAvailableWidth - halfTickWidth;
                top = this.availableExtent - distance - halfTickHeight;
            }

            tick.Arrange(new Rect(new Point(left, top), tick.DesiredSize));
        }

        private void ArrangeLabel(ContentPresenter label, double value, double offset)
        {
            double halfLabelHeight = label.DesiredSize.Height / 2;
            double halfLabelWidth = label.DesiredSize.Width / 2;

            bool flipLayout = this.OwnerGauge.MaxValue < this.OwnerGauge.MinValue;

            double minMaxDiff = Math.Abs(this.OwnerGauge.MaxValue - this.OwnerGauge.MinValue);
            double valueDiff = flipLayout ? value - this.OwnerGauge.MaxValue : value - this.OwnerGauge.MinValue;

            double tmp = this.isHorizontal ? halfLabelWidth : halfLabelHeight;
            double half = flipLayout ? -tmp : tmp;
            double distanceFromLayoutRectEdge = RadGauge.MapLogicalToPhysicalValue(valueDiff, this.availableExtent, minMaxDiff) - half;

            if (flipLayout)
            {
                distanceFromLayoutRectEdge = this.availableExtent - distanceFromLayoutRectEdge;
            }

            double left = 0;
            double top = 0;

            if (this.isHorizontal)
            {
                left = distanceFromLayoutRectEdge;
                top = (this.halfAvailableHeight - halfLabelHeight) + offset;
            }
            else
            {
                left = (this.halfAvailableWidth - halfLabelWidth) + offset;
                top = this.availableExtent - distanceFromLayoutRectEdge - label.DesiredSize.Height;
            }

            label.Arrange(new Rect(new Point(left, top), label.DesiredSize));
        }
    }
}
