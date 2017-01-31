using System;
using Telerik.Core;

namespace Telerik.Charting
{
    internal class RadialAxisLayoutStrategy : AxisModelLayoutStrategy
    {
        internal RadThickness margins;

        internal override AxisLastLabelVisibility DefaultLastLabelVisibility
        {
            get
            {
                return AxisLastLabelVisibility.Clip;
            }
        }

        internal override double GetZoom()
        {
            return 1;
        }

        internal override void ApplyLayoutRounding()
        {
        }

        internal override void UpdateTicksVisibility(RadRect clipRect)
        {
        }

        internal override void Arrange(RadRect availableRect)
        {
            var polarArea = this.owner.GetChartArea<PolarChartAreaModel>();

            RadRect tickRect = this.GetTicksArrangeRect(availableRect);
            RadRect labelRect = this.GetLabelsArrangeRect(tickRect);

            RadPoint tickCenter = tickRect.Center;
            double tickRadius = tickRect.Width / 2;

            RadPoint labelCenter = labelRect.Center;
            double labelRadius = labelRect.Width / 2;

            double tickThickness = this.owner.TickThickness;

            foreach (AxisTickModel tick in this.owner.ticks)
            {
                if (!tick.isVisible)
                {
                    continue;
                }

                double angle = polarArea.NormalizeAngle((double)tick.value);
                RadPoint tickPosition = RadMath.GetArcPoint(angle, tickCenter, tickRadius);
                tick.Arrange(new RadRect(tickPosition.X, tickPosition.Y, tickThickness, this.owner.GetTickLength(tick)));

                if (tick.associatedLabel == null || !tick.associatedLabel.isVisible)
                {
                    continue;
                }

                RadSize desiredSize = tick.associatedLabel.desiredSize;
                RadPoint labelPosition = RadMath.GetArcPoint(angle, labelCenter, labelRadius);
                labelPosition.X += desiredSize.Width * Math.Cos(angle * RadMath.DegToRadFactor) / 2;
                labelPosition.Y += desiredSize.Height * Math.Sin(angle * RadMath.DegToRadFactor) / 2;
                tick.associatedLabel.Arrange(new RadRect(labelPosition.X - (desiredSize.Width / 2), labelPosition.Y - (desiredSize.Height / 2), desiredSize.Width, desiredSize.Height));
            }
        }

        internal override ValueRange<decimal> GetVisibleRange(RadSize availableSize)
        {
            return new ValueRange<decimal>(0, 1);
        }

        internal override RadThickness GetDesiredMargin(RadSize availableSize)
        {
            this.margins = new RadThickness();
            this.UpdateLabels(availableSize);
            this.margins.Left = Math.Max(this.margins.Left, this.margins.Right);
            this.margins.Right = Math.Max(this.margins.Left, this.margins.Right);
            this.margins.Top = Math.Max(this.margins.Top, this.margins.Bottom);
            this.margins.Bottom = Math.Max(this.margins.Top, this.margins.Bottom);
            return this.margins;
        }

        internal override RadSize GetDesiredSize(RadSize availableSize)
        {
            return new RadSize(availableSize.Width, availableSize.Height);
        }

        protected override void ArrangeLabelMultiline(AxisLabelModel label, RadRect rect)
        {
            throw new NotImplementedException();
        }

        protected override void ArrangeLabelNone(AxisLabelModel label, RadRect rect)
        {
        }

        private RadRect GetTicksArrangeRect(RadRect availableRect)
        {
            RadRect tickRect = availableRect;
            tickRect.X += this.margins.Left;
            tickRect.Y += this.margins.Top;
            tickRect.Width -= this.margins.Left + this.margins.Right;
            tickRect.Height -= this.margins.Top + this.margins.Bottom;

            return tickRect;
        }

        private RadRect GetLabelsArrangeRect(RadRect tickRect)
        {
            double offset = this.owner.LineThickness + this.owner.MajorTickLength;

            RadRect labelRect = tickRect;
            labelRect.X -= offset;
            labelRect.Y -= offset;
            labelRect.Width += 2 * offset;
            labelRect.Height += 2 * offset;

            return labelRect;
        }

        private void UpdateLabels(RadSize availableSize)
        {
            RadRect availableRect = new RadRect(availableSize.Width, availableSize.Height);
            RadRect ellipseRect = RadRect.ToSquare(availableRect, false);
            ellipseRect = RadRect.CenterRect(ellipseRect, availableRect);
            double radius = ellipseRect.Width / 2;
            RadPoint center = ellipseRect.Center;
            RadPoint arcPosition;

            foreach (AxisLabelModel label in this.owner.labels)
            {
                if (!label.isVisible)
                {
                    continue;
                }

                double angle = this.owner.IsInverse ? 360 - (double)label.normalizedPosition * 360 : (double)label.normalizedPosition * 360;
                arcPosition = RadMath.GetArcPoint(angle, center, radius);

                this.UpdateMargins(ellipseRect, label.desiredSize, arcPosition);
            }

            double offset = this.owner.LineThickness + this.owner.MajorTickLength;
            this.margins.Left += offset;
            this.margins.Top += offset;
            this.margins.Right += offset;
            this.margins.Bottom += offset;
        }

        private void UpdateMargins(RadRect availableRect, RadSize labelSize, RadPoint arcPosition)
        {
            double left = arcPosition.X - labelSize.Width;
            if (left < availableRect.X)
            {
                this.margins.Left = Math.Max(this.margins.Left, availableRect.X - left);
            }

            double top = arcPosition.Y - labelSize.Height;
            if (top < availableRect.Y)
            {
                this.margins.Top = Math.Max(this.margins.Top, availableRect.Y - top);
            }

            double right = arcPosition.X + labelSize.Width;
            if (right > availableRect.Right)
            {
                this.margins.Right = Math.Max(this.margins.Right, right - availableRect.Right);
            }

            double bottom = arcPosition.Y + labelSize.Height;
            if (bottom > availableRect.Bottom)
            {
                this.margins.Bottom = Math.Max(this.margins.Bottom, bottom - availableRect.Bottom);
            }
        }
    }
}