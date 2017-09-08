using System;
using System.Collections.Generic;
using Telerik.Charting;
using Windows.Foundation;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal class SplineRenderer : LineRenderer
    {
        protected internal override IEnumerable<Point> GetPoints(DataPointSegment segment)
        {
            // return the first point since spline segmentation skips it
            yield return this.renderPoints[segment.StartIndex].Center();

            IChartView view = this.model.GetChartArea().view;
            double scaleFactor = Math.Abs(view.ZoomWidth - view.ZoomHeight) / 2;

            foreach (Point point in SplineHelper.GetSplinePoints(this.renderPoints, segment, scaleFactor))
            {
                yield return point;
            }
        }
    }
}
