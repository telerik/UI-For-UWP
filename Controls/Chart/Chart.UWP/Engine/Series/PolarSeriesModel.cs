using System;
using Telerik.Core;

namespace Telerik.Charting
{
    internal class PolarSeriesModel : SeriesModelWithAxes<PolarDataPoint>
    {
        internal override ModifyChildrenResult CanAddChild(Node child)
        {
            if (child is PolarDataPoint)
            {
                return ModifyChildrenResult.Accept;
            }

            return base.CanAddChild(child);
        }

        internal override RadRect ArrangeOverride(RadRect rect)
        {
            double radius = rect.Width / 2;
            RadPoint arcPosition;
            RadPoint center = rect.Center;

            foreach (PolarDataPoint point in this.DataPoints)
            {
                if (point.valuePlot == null || point.anglePlot == null)
                {
                    continue;
                }

                double pointRadius = point.valuePlot.NormalizedValue * radius;
                double angle = point.anglePlot.ConvertToAngle();
                arcPosition = RadMath.GetArcPoint(angle, center, pointRadius);

                RadSize pointSize = point.Measure();
                point.Arrange(new RadRect(arcPosition.X - (pointSize.Width / 2), arcPosition.Y - (pointSize.Height / 2), pointSize.Width, pointSize.Height), false);
            }

            return rect;
        }
    }
}
