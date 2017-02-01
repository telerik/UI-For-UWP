using System;
using Telerik.Core;

namespace Telerik.Charting
{
    internal class PolarAxisLayoutStrategy : AxisModelLayoutStrategy
    {
        internal double StartAngle
        {
            get
            {
                if (this.owner == null)
                {
                    return 0;
                }

                ChartAreaModel chartArea = this.owner.GetChartArea();
                if (chartArea == null)
                {
                    return 0;
                }

                return (chartArea as PolarChartAreaModel).NormalizeAngle(0);
            }
        }

        internal override AxisLastLabelVisibility DefaultLastLabelVisibility
        {
            get
            {
                return AxisLastLabelVisibility.Visible;
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
            double thickness = this.owner.TickThickness;
            double startAngle = this.StartAngle;
            double radius = availableRect.Width / 2;
            RadPoint center = availableRect.Center;

            double length = this.owner.MajorTickLength;
            RadPoint labelAxisCenter = RadMath.GetArcPoint(startAngle + 90, center, length * 2);

            double angleInRad = (360 - startAngle) * RadMath.DegToRadFactor;
            double sin = Math.Sin(angleInRad);
            double cos = Math.Cos(angleInRad);

            foreach (AxisTickModel tick in this.owner.ticks)
            {
                double tickRadius = (double)tick.normalizedValue * radius;
                double tickLength = this.owner.GetTickLength(tick);
                RadPoint tickPosition = RadMath.GetArcPoint(startAngle, center, tickRadius);
                tick.Arrange(new RadRect(tickPosition.X, tickPosition.Y, thickness, tickLength));

                if (tick.associatedLabel == null)
                {
                    continue;
                }

                RadSize desiredSize = tick.associatedLabel.desiredSize;
                double halfWidth = desiredSize.Width / 2;
                double halfHeight = desiredSize.Height / 2;

                RadPoint labelPosition = RadMath.GetArcPoint(startAngle, labelAxisCenter, tickRadius);
                RadRect bounds = new RadRect(labelPosition.X - halfWidth, labelPosition.Y - halfHeight, desiredSize.Width, desiredSize.Height);

                bounds.X += sin * halfWidth;
                bounds.Y += cos * halfHeight;

                tick.associatedLabel.Arrange(bounds);
            }
        }

        internal override ValueRange<decimal> GetVisibleRange(RadSize availableSize)
        {
            return new ValueRange<decimal>(0, 1);
        }

        internal override RadThickness GetDesiredMargin(RadSize availableSize)
        {
            return RadThickness.Empty;
        }

        internal override RadSize GetDesiredSize(RadSize availableSize)
        {
            return RadSize.Empty;
        }

        protected override void ArrangeLabelMultiline(AxisLabelModel label, RadRect rect)
        {
        }

        protected override void ArrangeLabelNone(AxisLabelModel label, RadRect rect)
        {
        }
    }
}
