using System;
using Telerik.Core;

namespace Telerik.Charting
{
    internal class RadarSeriesModel : CategoricalSeriesModel
    {
        internal override AxisPlotMode DefaultPlotMode
        {
            get
            {
                return AxisPlotMode.OnTicks;
            }
        }

        internal override RadRect ArrangeOverride(RadRect rect)
        {
            double radius = rect.Width / 2;
            RadPoint arcPosition;
            RadPoint center = rect.Center;

            foreach (CategoricalDataPoint point in this.DataPoints)
            {
                if (point.numericalPlot == null || point.categoricalPlot == null)
                {
                    continue;
                }

                double pointRadius = point.numericalPlot.NormalizedValue * radius;
                double angle = point.categoricalPlot.ConvertToAngle(this.GetChartArea() as PolarChartAreaModel);
                arcPosition = RadMath.GetArcPoint(angle, center, pointRadius);

                RadSize pointSize = point.Measure();
                point.Arrange(new RadRect(arcPosition.X - (pointSize.Width / 2), arcPosition.Y - (pointSize.Height / 2), pointSize.Width, pointSize.Height), false);
            }

            return rect;
        }
    }
}
