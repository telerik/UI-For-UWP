using System;
using System.Diagnostics;

namespace Telerik.Charting
{
    internal class CombinedBarSeriesRoundLayoutStrategy : CombinedSeriesRoundLayoutStrategy
    {
        public override void ApplyLayoutRounding(ChartAreaModel chart, CombinedSeries series)
        {
            CategoricalSeriesModel categoricalSeries = series.Series[0] as CategoricalSeriesModel;
            if (categoricalSeries == null)
            {
                Debug.Assert(false, "Invalid combined series.");
                return;
            }

            CategoricalSeriesRoundLayoutContext info = new CategoricalSeriesRoundLayoutContext(categoricalSeries);
            if (info.PlotDirection == AxisPlotDirection.Vertical)
            {
                ApplyLayoutRoundingVertical(series, info);
            }
            else
            {
                ApplyLayoutRoundingHorizontal(series, info);
            }
        }

        private static void ApplyLayoutRoundingHorizontal(CombinedSeries series, CategoricalSeriesRoundLayoutContext info)
        {
            double previousStackTop = -1;
            CategoricalDataPoint firstPoint;
            CategoricalDataPoint point;
            CategoricalDataPoint previousPositivePoint = null;
            CategoricalDataPoint previousNegativePoint = null;

            foreach (CombineGroup group in series.Groups)
            {
                foreach (CombineStack stack in group.Stacks)
                {
                    // first point is on the plot line
                    firstPoint = stack.Points[0] as CategoricalDataPoint;
                    if (previousStackTop != -1)
                    {
                        firstPoint.layoutSlot.Y = previousStackTop - firstPoint.layoutSlot.Height;
                    }
                    info.SnapPointToPlotLine(firstPoint);
                    info.SnapPointToGridLine(firstPoint);

                    int count = stack.Points.Count;
                    previousPositivePoint = firstPoint;
                    previousNegativePoint = firstPoint;

                    for (int i = 1; i < count; i++)
                    {
                        point = stack.Points[i] as CategoricalDataPoint;

                        if (point.numericalPlot == null || double.IsNaN(point.layoutSlot.Center.X) || double.IsNaN(point.layoutSlot.Center.Y))
                        {
                            continue;
                        }

                        if (previousStackTop != -1)
                        {
                            point.layoutSlot.Y = previousStackTop - point.layoutSlot.Height;
                        }

                        //// inverse axis with negative point is equivalent to regular axis with positive point
                        if (point.isPositive ^ point.numericalPlot.Axis.IsInverse)
                        {
                            point.layoutSlot.X = previousPositivePoint.layoutSlot.Right;
                            previousPositivePoint = point;
                        }
                        else
                        {
                            point.layoutSlot.X = previousNegativePoint.layoutSlot.X - point.layoutSlot.Width;
                            previousNegativePoint = point;
                        }
                        info.SnapPointToGridLine(point);
                    }

                    previousStackTop = firstPoint.isEmpty ? -1 : firstPoint.layoutSlot.Y;
                }

                previousStackTop = -1;
            }
        }

        private static void ApplyLayoutRoundingVertical(CombinedSeries series, CategoricalSeriesRoundLayoutContext info)
        {
            double previousStackRight = -1;
            CategoricalDataPoint firstPoint;
            CategoricalDataPoint point;
            CategoricalDataPoint previousPositivePoint = null;
            CategoricalDataPoint previousNegativePoint = null;

            foreach (CombineGroup group in series.Groups)
            {
                foreach (CombineStack stack in group.Stacks)
                {
                    // first point is on the plot line
                    firstPoint = stack.Points[0] as CategoricalDataPoint;
                    if (previousStackRight != -1)
                    {
                        firstPoint.layoutSlot.X = previousStackRight;
                    }
                    info.SnapPointToPlotLine(firstPoint);
                    info.SnapPointToGridLine(firstPoint);

                    int count = stack.Points.Count;
                    previousPositivePoint = firstPoint;
                    previousNegativePoint = firstPoint;

                    for (int i = 1; i < count; i++)
                    {
                        point = stack.Points[i] as CategoricalDataPoint;

                        if (point.numericalPlot == null || double.IsNaN(point.layoutSlot.Center.X) || double.IsNaN(point.layoutSlot.Center.Y))
                        {
                            continue;
                        }

                        if (previousStackRight != -1)
                        {
                            point.layoutSlot.X = previousStackRight;
                        }

                        //// inverse axis with negative point is equivalent to regular axis with positive point
                        if (point.isPositive ^ point.numericalPlot.Axis.IsInverse)
                        {
                            point.layoutSlot.Y = previousPositivePoint.layoutSlot.Y - point.layoutSlot.Height;
                            previousPositivePoint = point;
                        }
                        else
                        {
                            point.layoutSlot.Y = previousNegativePoint.layoutSlot.Bottom;
                            previousNegativePoint = point;
                        }
                        info.SnapPointToGridLine(point);
                    }

                    previousStackRight = firstPoint.isEmpty ? -1 : firstPoint.layoutSlot.Right;
                }

                previousStackRight = -1;
            }
        }
    }
}