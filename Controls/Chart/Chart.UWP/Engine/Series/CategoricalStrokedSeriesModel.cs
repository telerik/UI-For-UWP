using System;
using Telerik.Core;

namespace Telerik.Charting
{
    internal class CategoricalStrokedSeriesModel : CategoricalSeriesModel
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
            RadRect zoomedRect = this.GetZoomedRect(rect);
            bool zoomed = zoomedRect != rect;

            this.renderablePoints.Clear();

            double x = double.NaN, y = double.NaN;
            AxisPlotDirection plotDirection = this.GetTypedValue<AxisPlotDirection>(AxisModel.PlotDirectionPropertyKey, AxisPlotDirection.Vertical);
            RadRect plotRect = this.GetChartArea().view.PlotAreaClip;
            DataPoint prevPoint = null;

            foreach (CategoricalDataPoint point in this.DataPoints)
            {
                if (point.categoricalPlot == null)
                {
                    continue;
                }

                if (plotDirection == AxisPlotDirection.Vertical)
                {
                    x = point.categoricalPlot.CenterX(zoomedRect);
                    if (point.numericalPlot != null)
                    {
                        y = point.numericalPlot.CenterY(zoomedRect);
                    }

                    // points are sorted along the horizontal axis and renderable points are all the points, falling within the X-range of the plot area clip
                    if (zoomed && (x >= plotRect.X && x <= plotRect.Right))
                    {
                        if (prevPoint != null && this.renderablePoints.Count == 0)
                        {
                            this.renderablePoints.Add(prevPoint);
                        }
                        this.renderablePoints.Add(point);
                    }
                }
                else
                {
                    if (point.numericalPlot != null)
                    {
                        x = point.numericalPlot.CenterX(zoomedRect);
                    }
                    y = point.categoricalPlot.CenterY(zoomedRect);

                    // points are sorted along the vertical axis and renderable points are all the points, falling within the Y-range of the plot area clip
                    if (zoomed && (y >= plotRect.Y && y <= plotRect.Bottom))
                    {
                        if (prevPoint != null && this.renderablePoints.Count == 0)
                        {
                            this.renderablePoints.Add(prevPoint);
                        }
                        this.renderablePoints.Add(point);
                    }
                }

                RadSize pointSize = point.Measure();
                point.Arrange(new RadRect(x - (pointSize.Width / 2), y - (pointSize.Height / 2), pointSize.Width, pointSize.Height), false);

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

            return zoomedRect;
        }
    }
}
