using Telerik.Core;
namespace Telerik.Charting
{
    internal static class OhlcSeriesRoundLayoutContext
    {
        public static void SnapPointToGrid(OhlcDataPoint point)
        {
            if (point.numericalPlot == null)
            {
                return;
            }

            SnapToHighGridLine(point);
            SnapToLowGridLine(point);

            SnapToOpenGridLine(point);
            SnapToCloseGridLine(point);
        }

        private static void SnapToHighGridLine(OhlcDataPoint point)
        {
            if (point.numericalPlot.SnapTickIndex < 0 ||
                point.numericalPlot.SnapTickIndex >= point.numericalPlot.Axis.ticks.Count)
            {
                return;
            }

            var topTick = point.numericalPlot.Axis.ticks[point.numericalPlot.SnapTickIndex];
            if (!RadMath.AreClose(point.numericalPlot.NormalizedHigh, (double)topTick.normalizedValue))
            {
                return;
            }

            var tickRect = topTick.layoutSlot;
            double gridLine = tickRect.Y + (int)(tickRect.Height / 2);

            double difference = point.layoutSlot.Y - gridLine;
            point.layoutSlot.Y -= difference;
            point.layoutSlot.Height += difference;

            if (point.layoutSlot.Height < 0)
            {
                point.layoutSlot.Height = 0;
            }
        }

        private static void SnapToLowGridLine(OhlcDataPoint point)
        {
            if (point.numericalPlot.SnapBaseTickIndex == -1 ||
                point.numericalPlot.SnapBaseTickIndex >= point.numericalPlot.Axis.ticks.Count)
            {
                return;
            }

            var baseTick = point.numericalPlot.Axis.ticks[point.numericalPlot.SnapBaseTickIndex];
            if (!RadMath.AreClose(point.numericalPlot.NormalizedLow, (double)baseTick.normalizedValue))
            {
                return;
            }

            var tickRect = baseTick.layoutSlot;
            double gridLine = tickRect.Y + (int)(tickRect.Height / 2);

            double difference = gridLine - point.layoutSlot.Bottom;
            point.layoutSlot.Height += difference;

            if (point.layoutSlot.Height < 0)
            {
                point.layoutSlot.Height = 0;
            }
        }

        private static void SnapToOpenGridLine(OhlcDataPoint point)
        {
            if (point.numericalPlot.SnapOpenTickIndex == -1 ||
                point.numericalPlot.SnapOpenTickIndex >= point.numericalPlot.Axis.ticks.Count)
            {
                return;
            }

            var openTick = point.numericalPlot.Axis.ticks[point.numericalPlot.SnapOpenTickIndex];
            if (!RadMath.AreClose(point.numericalPlot.NormalizedOpen, (double)openTick.normalizedValue))
            {
                return;
            }

            var tickRect = openTick.layoutSlot;
            double gridLine = tickRect.Y + (int)(tickRect.Height / 2);
            point.numericalPlot.PhysicalOpen = gridLine - point.layoutSlot.Y;
        }

        private static void SnapToCloseGridLine(OhlcDataPoint point)
        {
            if (point.numericalPlot.SnapCloseTickIndex == -1 ||
                point.numericalPlot.SnapCloseTickIndex >= point.numericalPlot.Axis.ticks.Count)
            {
                return;
            }

            var closeTick = point.numericalPlot.Axis.ticks[point.numericalPlot.SnapCloseTickIndex];
            if (!RadMath.AreClose(point.numericalPlot.NormalizedClose, (double)closeTick.normalizedValue))
            {
                return;
            }

            var tickRect = closeTick.layoutSlot;
            double gridLine = tickRect.Y + (int)(tickRect.Height / 2);
            point.numericalPlot.PhysicalClose = gridLine - point.layoutSlot.Y;
        }
    }
}
