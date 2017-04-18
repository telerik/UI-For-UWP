using System;
using Telerik.Core;

namespace Telerik.Charting
{
    internal class VerticalAxisLayoutStrategy : AxisModelLayoutStrategy
    {
        internal override AxisLastLabelVisibility DefaultLastLabelVisibility
        {
            get
            {
                return AxisLastLabelVisibility.Visible;
            }
        }

        internal override double GetZoom()
        {
            return this.owner.GetChartArea().view.ZoomHeight;
        }

        internal override void ApplyLayoutRounding()
        {
            // fit first and last ticks within axis layout slot
            AxisTickModel firstTick = this.owner.FirstTick;
            AxisTickModel lastTick = this.owner.LastTick;

            double thickness = this.owner.TickThickness;
            double thicknessOffset = (int)(thickness / 2);

            // ensure that the first and last ticks are within axis' layout slot
            if (firstTick != null && RadMath.IsZero(firstTick.normalizedValue))
            {
                double zoomHeight = this.owner.layoutSlot.Height * this.owner.GetChartArea().view.ZoomHeight;
                firstTick.layoutSlot.Y = this.owner.layoutSlot.Y + zoomHeight - thicknessOffset;

                // remove one additional pixel at bottom (rendering along the Y-axis goes from top to bottom)
                firstTick.layoutSlot.Y--;
            }
            if (lastTick != null && RadMath.IsOne(lastTick.normalizedValue))
            {
                lastTick.layoutSlot.Y = this.owner.layoutSlot.Y - thicknessOffset;
            }
        }

        internal override void UpdateTicksVisibility(RadRect clipRect)
        {
            AxisPlotMode plotMode = this.owner.ActualPlotMode;

            foreach (AxisTickModel tick in this.owner.ticks)
            {
                bool visible = tick.layoutSlot.Center.Y >= clipRect.Y && tick.layoutSlot.Center.Y <= clipRect.Bottom;
                tick.isVisible = visible;
                if (tick.associatedLabel != null)
                {
                    if (plotMode == AxisPlotMode.OnTicks)
                    {
                        tick.associatedLabel.isVisible = visible;
                    }
                    else
                    {
                        tick.associatedLabel.isVisible = tick.associatedLabel.layoutSlot.Y >= clipRect.Y && tick.associatedLabel.layoutSlot.Bottom <= clipRect.Bottom;
                    }
                }
            }
        }

        internal override void Arrange(RadRect availableRect)
        {
            // arrange title
            double titleLeft;
            if (this.owner.HorizontalLocation == AxisHorizontalLocation.Left)
            {
                titleLeft = availableRect.X;
            }
            else
            {
                titleLeft = availableRect.Right - this.owner.title.desiredSize.Height;
            }
            this.owner.title.Arrange(new RadRect(
                titleLeft,
                availableRect.Y + ((availableRect.Height - this.owner.title.desiredSize.Height) / 2),
                this.owner.title.desiredSize.Width,
                this.owner.title.desiredSize.Height));

            // scale by the zoom factor
            availableRect.Height *= this.owner.GetChartArea().view.ZoomHeight;

            // arrange ticks
            double thickness = this.owner.TickThickness;
            double thicknessOffset = (int)(thickness / 2);
            foreach (AxisTickModel tick in this.owner.ticks)
            {
                double y;
                if (tick.normalizedValue == 0)
                {
                    y = availableRect.Bottom - thicknessOffset;
                }
                else if (tick.normalizedValue == 1)
                {
                    y = availableRect.Y;
                }
                else
                {
                    y = availableRect.Bottom - ((double)tick.normalizedValue * availableRect.Height) - thicknessOffset;
                }

                double x;
                double width = this.owner.GetTickLength(tick);
                if (this.owner.HorizontalLocation == AxisHorizontalLocation.Left)
                {
                    x = availableRect.Right - width;
                }
                else
                {
                    x = availableRect.X;
                }
                tick.Arrange(new RadRect(x, y, width, thickness));
            }

            // arrange labels
            RadRect labelRect;
            double majorTickLength = this.owner.MajorTickLength;

            foreach (AxisLabelModel label in this.owner.labels)
            {
                double center = (double)label.normalizedPosition * availableRect.Height;
                double x;
                if (this.owner.HorizontalLocation == AxisHorizontalLocation.Left)
                {
                    x = availableRect.Right - majorTickLength - label.desiredSize.Width;
                }
                else
                {
                    x = availableRect.X + majorTickLength;
                }
                labelRect = new RadRect(
                    x,
                    availableRect.Bottom - center - (label.desiredSize.Height / 2),
                    label.desiredSize.Width,
                    label.desiredSize.Height);
                label.Arrange(labelRect);
            }
        }

        internal override ValueRange<decimal> GetVisibleRange(RadSize availableSize)
        {
            decimal zoomFactor = (decimal)this.owner.GetChartArea().view.ZoomHeight;
            if (zoomFactor == 1)
            {
                return new ValueRange<decimal>(0, 1);
            }

            decimal viewportOffsetBottom = zoomFactor - 1 + (decimal)this.owner.GetChartArea().view.PlotOriginY;
            decimal visibleLength = 1 / zoomFactor;
            decimal offsetY = viewportOffsetBottom / zoomFactor;
            return new ValueRange<decimal>(offsetY, offsetY + visibleLength);
        }

        internal override RadThickness GetDesiredMargin(RadSize availableSize)
        {
            RadThickness margin = new RadThickness();
            if (this.maxLabelHeight == 0 || this.owner.LastLabelVisibility != AxisLastLabelVisibility.Visible)
            {
                return margin;
            }

            double labelOffset = 0;
            if (this.owner.ActualPlotMode == AxisPlotMode.OnTicks)
            {
                labelOffset = (int)(this.maxLabelHeight / 2);
                margin.Top = labelOffset;
            }

            return margin;
        }

        internal override RadSize GetDesiredSize(RadSize availableSize)
        {
            RadSize size = new RadSize(this.owner.LineThickness, 0);
            size.Width += this.owner.MajorTickLength;

            this.maxLabelHeight = 0;
            this.maxLabelWidth = 0;
            foreach (AxisLabelModel label in this.owner.labels)
            {
                if (!label.isVisible)
                {
                    continue;
                }

                if (label.desiredSize.Width > this.maxLabelWidth)
                {
                    this.maxLabelWidth = label.desiredSize.Width;
                }
                if (label.desiredSize.Height > this.maxLabelHeight)
                {
                    this.maxLabelHeight = label.desiredSize.Height;
                }
            }

            size.Width += this.maxLabelWidth;

            // We are adding the height of the title since we will rotate it.
            size.Width += this.owner.title.desiredSize.Height;

            return size;
        }

        protected override void ArrangeLabelMultiline(AxisLabelModel label, RadRect rect)
        {
        }

        protected override void ArrangeLabelNone(AxisLabelModel label, RadRect rect)
        {
        }
    }
}
