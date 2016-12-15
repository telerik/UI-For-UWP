using System;
using System.Collections.Generic;
using Telerik.Core;

namespace Telerik.Charting
{
    internal class RangeSeriesModel : SeriesModelWithAxes<RangeDataPoint>
    {
        internal override AxisPlotMode DefaultPlotMode
        {
            get
            {
                return AxisPlotMode.OnTicks;
            }
        }
        protected virtual bool ShouldRoundLayout
        {
            get { return false; }
        }
            
        internal override RadRect ArrangeOverride(RadRect plotAreaRect)
        {
            LinearAxisModel numericalAxis = this.firstAxis as LinearAxisModel;
            if (numericalAxis == null)
            {
                numericalAxis = this.secondAxis as LinearAxisModel;
            }

            if (numericalAxis != null && numericalAxis.IsInverse)
            {
                throw new NotSupportedException("Range series do not support inverse numerical axis. Set the IsInverse property of the numerical axis to false.");
            }

            RadRect zoomedRect = this.GetZoomedRect(plotAreaRect);
            bool zoomed = zoomedRect != plotAreaRect;

            this.renderablePoints.Clear();

            double normalizedLow = double.NaN, normalizedHigh = double.NaN;
            double x = double.NaN, y = double.NaN;
            double width = 0, height = 0;
            AxisPlotDirection plotDirection = this.GetTypedValue<AxisPlotDirection>(AxisModel.PlotDirectionPropertyKey, AxisPlotDirection.Vertical);
            RadRect plotRect = this.GetChartArea().view.PlotAreaClip;
            DataPoint prevPoint = null;

            foreach (RangeDataPoint point in this.DataPoints)
            {
                if (point.categoricalPlot == null)
                {
                    continue;
                }

                if (plotDirection == AxisPlotDirection.Vertical)
                {
                    if (point.numericalPlot != null)
                    {
                        normalizedLow = Math.Min(point.numericalPlot.NormalizedLow, point.numericalPlot.NormalizedHigh);
                        normalizedHigh = Math.Max(point.numericalPlot.NormalizedLow, point.numericalPlot.NormalizedHigh);

                        y = plotAreaRect.Y + ((1 - normalizedHigh) * zoomedRect.Height);
                        height = (normalizedHigh - normalizedLow) * zoomedRect.Height;
                    }

                    width = point.categoricalPlot.Length * zoomedRect.Width;
                    x = zoomedRect.X + ((point.categoricalPlot.Position - point.categoricalPlot.Length / 2) * zoomedRect.Width);

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
                        normalizedLow = Math.Min(point.numericalPlot.NormalizedLow, point.numericalPlot.NormalizedHigh);
                        normalizedHigh = Math.Max(point.numericalPlot.NormalizedLow, point.numericalPlot.NormalizedHigh);

                        x = plotAreaRect.X + (normalizedLow * zoomedRect.Width);
                        width = (normalizedHigh - normalizedLow) * zoomedRect.Width;
                    }

                    height = point.categoricalPlot.Length * zoomedRect.Height;
                    y = plotAreaRect.Bottom - (((point.categoricalPlot.Position - point.categoricalPlot.Length / 2) * zoomedRect.Height) + height);

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
                point.Arrange(new RadRect(x, y, width, height), this.ShouldRoundLayout);

                prevPoint = point;
                normalizedLow = double.NaN;
                normalizedHigh = double.NaN;
                x = double.NaN;
                y = double.NaN;
                width = 0;
                height = 0;
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

        internal override ModifyChildrenResult CanAddChild(Node child)
        {
            if (child is RangeDataPoint)
            {
                return ModifyChildrenResult.Accept;
            }

            return base.CanAddChild(child);
        }
    }
}