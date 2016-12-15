using System;
using Telerik.Core;

namespace Telerik.Charting
{
    internal class HorizontalAxisLayoutStrategy : AxisModelLayoutStrategy
    {
        private double labelTop;
        private double totalLabelHeight;

        private delegate void LabelArranger(AxisLabelModel label, RadRect rect);

        internal override AxisLastLabelVisibility DefaultLastLabelVisibility
        {
            get
            {
                // NOTE: Visible default value adds performance hit (pan causes PlotArea recalculation as last label size changes). 
                return AxisLastLabelVisibility.Clip;
            }
        }

        internal override double GetZoom()
        {
            return this.owner.GetChartArea().view.ZoomWidth;
        }

        internal override void ApplyLayoutRounding()
        {
            // fit first and last ticks within axis layout slot
            AxisTickModel firstTick = this.owner.FirstTick;
            AxisTickModel lastTick = this.owner.LastTick;

            double thickness = this.owner.TickThickness;
            double thicknessOffset = (int)(thickness / 2);

            if (firstTick != null && RadMath.IsZero(firstTick.normalizedValue))
            {
                firstTick.layoutSlot.X = this.owner.layoutSlot.X - thicknessOffset;
            }
            if (lastTick != null && RadMath.IsOne(lastTick.normalizedValue))
            {
                double zoomWidth = this.owner.layoutSlot.Width * this.owner.GetChartArea().view.ZoomWidth;
                lastTick.layoutSlot.X = this.owner.layoutSlot.X + zoomWidth - thicknessOffset;

                // remove one additional pixel on the right (rendering along the X-axis goes from left to right)
                lastTick.layoutSlot.X--;
            }
        }

        internal override void UpdateTicksVisibility(RadRect clipRect)
        {
            AxisPlotMode plotMode = this.owner.ActualPlotMode;

            foreach (AxisTickModel tick in this.owner.ticks)
            {
                bool visible = tick.layoutSlot.Center.X >= clipRect.X && tick.layoutSlot.Center.X <= clipRect.Right;
                tick.isVisible = visible;
                if (tick.associatedLabel != null && tick.associatedLabel.isVisible)
                {
                    if (plotMode == AxisPlotMode.OnTicks)
                    {
                        tick.associatedLabel.isVisible = visible;
                    }
                    else if (!visible)
                    {
                        tick.associatedLabel.isVisible = tick.associatedLabel.layoutSlot.X >= clipRect.X && tick.associatedLabel.layoutSlot.Right <= clipRect.Right;
                    }
                }
            }
        }

        internal override void Arrange(RadRect availableRect)
        {
            // arrange title
            double titleTop = 0d;
            if (this.owner.VerticalLocation == AxisVerticalLocation.Bottom)
            {
                titleTop = availableRect.Bottom - this.owner.title.desiredSize.Height;
            }
            else
            {
                titleTop = availableRect.Y;
                availableRect.Y += this.owner.title.desiredSize.Height; 
            }

            this.owner.title.Arrange(new RadRect(
                availableRect.X + ((availableRect.Width - this.owner.title.desiredSize.Width) / 2),
                titleTop,
                this.owner.title.desiredSize.Width,
                this.owner.title.desiredSize.Height));

            // scale by the zoom factor
            availableRect.Width *= this.owner.GetChartArea().view.ZoomWidth;

            // arrange ticks
            double x;
            double y;
            if (this.owner.VerticalLocation == AxisVerticalLocation.Bottom)
            {
                y = availableRect.Y;
            }
            else
            {
                y = availableRect.Y + this.totalLabelHeight + this.owner.LineThickness;
            }
            double thickness = this.owner.TickThickness;
            double thicknessOffset = (int)(thickness / 2);

            foreach (AxisTickModel tick in this.owner.ticks)
            {
                if (tick.normalizedValue == 0)
                {
                    x = availableRect.X - thicknessOffset;
                }
                else if (tick.normalizedValue == 1)
                {
                    x = availableRect.X + availableRect.Width;
                }
                else
                {
                    x = availableRect.X + ((double)tick.normalizedValue * availableRect.Width) - thicknessOffset;
                }

                tick.Arrange(new RadRect(x, y, thickness, this.owner.GetTickLength(tick)));
            }

            // arrange labels
            if (this.owner.VerticalLocation == AxisVerticalLocation.Bottom)
            {
                this.labelTop = availableRect.Y + this.owner.MajorTickLength;
            }
            else
            {
                this.labelTop = availableRect.Y;
            }

            LabelArranger labelFitStrategy = this.ArrangeLabelNone;
            if (this.owner.labelFitMode == AxisLabelFitMode.MultiLine)
            {
                labelFitStrategy = this.ArrangeLabelMultiline;
            }

            foreach (AxisLabelModel label in this.owner.labels)
            {
                labelFitStrategy(label, availableRect);
            }
        }

        internal override ValueRange<decimal> GetVisibleRange(RadSize availableSize)
        {
            decimal zoomFactor = (decimal)this.owner.GetChartArea().view.ZoomWidth;
            if (zoomFactor == 1)
            {
                return new ValueRange<decimal>(0, 1);
            }

            decimal visibleLength = 1 / zoomFactor;
            decimal offsetX = (decimal)(-this.owner.GetChartArea().view.PlotOriginX) / zoomFactor;
            return new ValueRange<decimal>(offsetX, offsetX + visibleLength);
        }

        internal override RadThickness GetDesiredMargin(RadSize availableSize)
        {
            RadThickness margin = new RadThickness();
            if (this.maxLabelWidth == 0 || this.owner.LastLabelVisibility != AxisLastLabelVisibility.Visible)
            {
                return margin;
            }

            // TODO: Is this assumption good enough as it is theoretically possible that
            // the horizontal reach of any inner label be wider than the reach of the last label.
            AxisLabelModel lastLabel = this.owner.labels[this.owner.labels.Count - 1];
            double lastLabelHReach = (int)(lastLabel.desiredSize.Width / 2);

            if (this.owner.ActualPlotMode == AxisPlotMode.OnTicks)
            {
                margin.Right = lastLabelHReach;
            }
            else
            {
                double slotWidth = (int)(availableSize.Width / this.owner.majorTickCount);
                margin.Right = Math.Max(0, lastLabelHReach - (slotWidth / 2));
            }

            return margin;
        }

        internal override RadSize GetDesiredSize(RadSize availableSize)
        {
            RadSize size = new RadSize(0, this.owner.LineThickness);
            size.Height += this.owner.MajorTickLength;

            this.maxLabelWidth = 0;
            this.maxLabelHeight = 0;
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

            this.UpdateTotalLabelHeight(availableSize);

            size.Height += this.totalLabelHeight;
            size.Height += this.owner.title.desiredSize.Height;

            return size;
        }

        protected override void ArrangeLabelMultiline(AxisLabelModel label, RadRect rect)
        {
            double center = (double)label.normalizedPosition * rect.Width;
            double stackShift = (label.CollectionIndex % this.totalLabelWidthToAvailableWidth) * this.shouldFitLabelsMultiLine;

            RadRect labelRect = new RadRect(rect.X + center - (label.desiredSize.Width / 2), this.labelTop + (stackShift * label.desiredSize.Height), label.desiredSize.Width, label.desiredSize.Height);
            label.Arrange(labelRect);
        }

        protected override void ArrangeLabelNone(AxisLabelModel label, RadRect rect)
        {
            double center = (double)label.normalizedPosition * rect.Width;

            RadRect labelRect = new RadRect(rect.X + center - (label.untransformedDesiredSize.Width / 2), this.labelTop, label.untransformedDesiredSize.Width, label.untransformedDesiredSize.Height);
            label.Arrange(labelRect);
        }

        private void UpdateTotalLabelHeight(RadSize availableSize)
        {
            // we asume that all labels have almost same width and we take the maximum of all
            // TODO: This is not always true, we need a more extended algorithm which will build lines dynamically
            double totalLabelWidth = this.owner.labels.Count * this.maxLabelWidth;
            this.totalLabelWidthToAvailableWidth = (int)(totalLabelWidth / availableSize.Width) + 1;
            if (this.totalLabelWidthToAvailableWidth > this.owner.labels.Count)
            {
                this.totalLabelWidthToAvailableWidth = this.owner.labels.Count;
            }

            this.shouldFitLabelsMultiLine = this.totalLabelWidthToAvailableWidth > 1 ? 1 : 0;

            this.totalLabelHeight = this.maxLabelHeight;
            if (this.owner.LabelFitMode == AxisLabelFitMode.MultiLine)
            {
                this.totalLabelHeight *= this.totalLabelWidthToAvailableWidth;
            }
        }
    }
}
