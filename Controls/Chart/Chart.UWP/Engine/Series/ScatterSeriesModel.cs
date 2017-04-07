using System;
using Telerik.Core;

namespace Telerik.Charting
{
    /// <summary>
    /// The model that arranges scatter points on the plot area.
    /// </summary>
    internal class ScatterSeriesModel : SeriesModelWithAxes<ScatterDataPoint>
    {
        internal override ModifyChildrenResult CanAddChild(Node child)
        {
            if (child is ScatterDataPoint)
            {
                return ModifyChildrenResult.Accept;
            }

            return base.CanAddChild(child);
        }

        internal override RadRect ArrangeOverride(RadRect rect)
        {
            RadRect zoomedRect = this.GetZoomedRect(rect);
            bool zoomed = zoomedRect != rect;

            this.renderablePoints.Clear();

            RadRect plotRect = this.GetChartArea().view.PlotAreaClip;
            DataPoint prevPoint = null;

            double x = double.NaN, y = double.NaN;

            foreach (ScatterDataPoint point in this.DataPoints)
            {
                if (point.xPlot != null)
                {
                    x = point.xPlot.CenterX(zoomedRect); 
                }

                if (point.yPlot != null)
                {
                    y = point.yPlot.CenterY(zoomedRect);
                }

                RadSize pointSize = point.Measure();
                point.Arrange(new RadRect(x - (pointSize.Width / 2), y - (pointSize.Height / 2), pointSize.Width, pointSize.Height), false);

                // we assume that the below condition is enough to consider a Point "Renderable"
                if (zoomed && (x >= plotRect.X && x <= plotRect.Right))
                {
                    if (prevPoint != null && this.renderablePoints.Count == 0)
                    {
                        this.renderablePoints.Add(prevPoint);
                    }
                    this.renderablePoints.Add(point);
                }

                prevPoint = point;
                x = double.NaN;
                y = double.NaN;
            }

            // add the point after the last renderable point
            if (this.renderablePoints.Count > 0)
            {
                int lastIndex = this.renderablePoints[this.renderablePoints.Count - 1].Index;
                if (lastIndex < this.DataPoints.Count - 1)
                {
                    this.renderablePoints.Add(this.DataPoints[lastIndex + 1]);
                }
            }
            else if (zoomed && this.DataPoints.Count > 0)
            {
                // This tells the renderer to use the Renderable points rather than the DataPoints themselves.
                this.renderablePoints.Add(this.DataPoints[0]);
            }

            return zoomedRect;
        }
    }
}