using System;
using System.Collections.Generic;
using Telerik.Charting;
using Windows.Foundation;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal class PolarSplineRenderer : PolarLineRenderer
    {
        protected internal override IEnumerable<Point> GetPoints(DataPointSegment segment)
        {
            // return the first point since spline segmentation skips it
            yield return this.renderPoints[segment.StartIndex].Center();

            IChartView view = this.model.GetChartArea().view;
            double scaleFactor = Math.Abs(view.ZoomWidth - view.ZoomHeight) / 2;

            foreach (Point point in SplineHelper.GetSplinePoints(this.renderPoints, segment, scaleFactor, this.isClosed))
            {
                yield return point;
            }
        }

        protected override void ConnectFirstLastDataPoints()
        {
            IChartView view = this.model.GetChartArea().view;
            double scaleFactor = Math.Abs(view.ZoomWidth - view.ZoomHeight) / 2;

            PathFigure figure = null;
            PolyLineSegment lineSegment = null;
            foreach (Point point in SplineHelper.GetSplinePointsConnectingAbsoluteFirstLastDataPoints(this.renderPoints, scaleFactor))
            {
                if (lineSegment == null)
                {
                    figure = new PathFigure();
                    figure.StartPoint = point;
                    lineSegment = new PolyLineSegment();

                    continue;
                }

                lineSegment.Points.Add(point);
            }

            if (lineSegment != null)
            {
                figure.Segments.Add(lineSegment);
                this.shapeGeometry.Figures.Add(figure);
            }           
        }
    }
}
