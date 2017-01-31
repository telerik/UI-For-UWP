using System.Collections.Generic;
using System.Diagnostics;

namespace Telerik.Charting
{
    internal class CombinedRangeBarSeriesRoundLayoutStrategy : CombinedSeriesRoundLayoutStrategy
    {
        public override void ApplyLayoutRounding(ChartAreaModel chart, CombinedSeries series)
        {
            RangeBarSeriesModel rangeSeriesModel = series.Series[0] as RangeBarSeriesModel;
            if (rangeSeriesModel == null)
            {
                Debug.Assert(false, "Invalid combined series.");
                return;
            }

            RangeSeriesRoundLayoutContext info = new RangeSeriesRoundLayoutContext(rangeSeriesModel);
            if (info.PlotDirection == AxisPlotDirection.Vertical)
            {
                ApplyLayoutRoundingVertical(series, info);
            }
            else
            {
                ApplyLayoutRoundingHorizontal(series, info);
            }
        }
            
        private static void ApplyLayoutRoundingHorizontal(CombinedSeries series, RangeSeriesRoundLayoutContext roundLayoutContext)
        {
            double previousStackTop = -1;
            RangeDataPoint firstPoint;

            Dictionary<double, double> normalizedValueToX = new Dictionary<double, double>();

            foreach (CombineGroup group in series.Groups)
            {
                foreach (CombineStack stack in group.Stacks)
                {
                    firstPoint = stack.Points[0] as RangeDataPoint;

                    if (!firstPoint.isEmpty)
                    {
                        roundLayoutContext.SnapPointToGridLine(firstPoint);

                        // Handles visual glitches that might occur between clustered range bars.
                        RangeSeriesRoundLayoutContext.SnapNormalizedValueToPreviousX(firstPoint, normalizedValueToX);

                        SnapBottomToPreviousTop(firstPoint, previousStackTop);
                        previousStackTop = firstPoint.layoutSlot.Y;
                    }
                    else
                    {
                        previousStackTop = -1;
                    }
                }
                previousStackTop = -1;
            }
        }

        private static void ApplyLayoutRoundingVertical(CombinedSeries series, RangeSeriesRoundLayoutContext roundLayoutContext)
        {
            double previousStackRight = -1;
            RangeDataPoint firstPoint;

            Dictionary<double, double> normalizedValueToY = new Dictionary<double, double>();

            foreach (CombineGroup group in series.Groups)
            {
                foreach (CombineStack stack in group.Stacks)
                {
                    firstPoint = stack.Points[0] as RangeDataPoint;
                    if (!firstPoint.isEmpty)
                    {
                        roundLayoutContext.SnapPointToGridLine(firstPoint);

                        // Handles visual glitches that might occur between clustered range bars.
                        RangeSeriesRoundLayoutContext.SnapNormalizedValueToPreviousY(firstPoint, normalizedValueToY);

                        SnapLeftToPreviousRight(firstPoint, previousStackRight);
                        previousStackRight = firstPoint.layoutSlot.Right;
                    }
                    else
                    {
                        previousStackRight = -1;
                    }
                }
                previousStackRight = -1;
            }
        }

        private static void SnapBottomToPreviousTop(RangeDataPoint firstPoint, double previousStackTop)
        {
            if (previousStackTop != -1)
            {
                double difference = previousStackTop - firstPoint.layoutSlot.Bottom;
                firstPoint.layoutSlot.Height += difference;
            }
        }

        private static void SnapLeftToPreviousRight(RangeDataPoint firstPoint, double previousStackRight)
        {
            if (previousStackRight != -1)
            {
                double difference = previousStackRight - firstPoint.layoutSlot.X;
                firstPoint.layoutSlot.X += difference;
                firstPoint.layoutSlot.Width -= difference;
            }
        }
    }
}
