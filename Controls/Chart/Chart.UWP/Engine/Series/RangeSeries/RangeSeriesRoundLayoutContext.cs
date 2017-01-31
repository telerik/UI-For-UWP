using System.Collections.Generic;
using System.Diagnostics;
using Telerik.Core;

namespace Telerik.Charting
{
    internal class RangeSeriesRoundLayoutContext
    {
        public double PlotLine;
        public double PlotOrigin;
        public AxisPlotDirection PlotDirection;
        public RadRect PlotArea;

        public RangeSeriesRoundLayoutContext(RangeSeriesModel series)
        {
            CartesianChartAreaModel cartesianChartArea = series.GetChartArea() as CartesianChartAreaModel;
            if (cartesianChartArea == null)
            {
                Debug.Assert(false, "Invalid chart area.");
                return;
            }

            this.PlotDirection = series.GetTypedValue<AxisPlotDirection>(AxisModel.PlotDirectionPropertyKey, AxisPlotDirection.Vertical);
            this.PlotOrigin = series.GetTypedValue<double>(AxisModel.PlotOriginPropertyKey, 0d);
            this.PlotArea = cartesianChartArea.plotArea.layoutSlot;
            this.PlotArea.Width = (int)((this.PlotArea.Width * cartesianChartArea.view.ZoomWidth) + .5);
            this.PlotArea.Height = (int)((this.PlotArea.Height * cartesianChartArea.view.ZoomHeight) + .5);

            if (this.PlotDirection == AxisPlotDirection.Vertical)
            {
                if (this.PlotOrigin == 0)
                {
                    this.PlotLine = this.PlotArea.Bottom;
                }
                else if (this.PlotOrigin == 1)
                {
                    this.PlotLine = this.PlotArea.Y;
                }
                else
                {
                    double roundError = (series.SecondAxis.majorTickCount % 2) == 0 ? 0.5 : 0;
                    this.PlotLine = this.PlotArea.Bottom - (int)((this.PlotOrigin * this.PlotArea.Height) + roundError);
                }
            }
            else
            {
                if (this.PlotOrigin == 0)
                {
                    this.PlotLine = this.PlotArea.X;
                }
                else if (this.PlotOrigin == 1)
                {
                    this.PlotLine = this.PlotArea.Right;
                }
                else
                {
                    double roundError = (series.FirstAxis.majorTickCount % 2) != 0 ? 0.5 : 0;
                    this.PlotLine = this.PlotArea.X + (int)((this.PlotOrigin * this.PlotArea.Width) + roundError);
                }
            }
        }

        public void SnapPointToGridLine(RangeDataPoint point)
        {
            if (point.numericalPlot.SnapTickIndex >= 0 && point.numericalPlot.SnapTickIndex < point.numericalPlot.Axis.ticks.Count)
            {
                AxisTickModel highTick = point.numericalPlot.Axis.ticks[point.numericalPlot.SnapTickIndex];
                if (RadMath.AreClose(point.numericalPlot.NormalizedHigh, (double)highTick.normalizedValue))
                {
                    if (this.PlotDirection == AxisPlotDirection.Vertical)
                    {
                        SnapHighToVerticalGridLine(point, highTick.layoutSlot);
                    }
                    else
                    {
                        SnapHighToHorizontalGridLine(point, highTick.layoutSlot);
                    }
                }
            }

            if (point.numericalPlot.SnapBaseTickIndex >= 0 && point.numericalPlot.SnapBaseTickIndex < point.numericalPlot.Axis.ticks.Count)
            {
                AxisTickModel lowTick = point.numericalPlot.Axis.ticks[point.numericalPlot.SnapBaseTickIndex];
                if (RadMath.AreClose(point.numericalPlot.NormalizedLow, (double)lowTick.normalizedValue))
                {
                    if (this.PlotDirection == AxisPlotDirection.Vertical)
                    {
                        SnapLowToVerticalGridLine(point, lowTick.layoutSlot);
                    }
                    else
                    {
                        SnapLowToHorizontalGridLine(point, lowTick.layoutSlot);
                    }
                }
            }
        }

        internal static void SnapNormalizedValueToPreviousY(RangeDataPoint point, Dictionary<double, double> normalizedValueToY)
        {
            if (!normalizedValueToY.ContainsKey(point.numericalPlot.NormalizedLow))
            {
                normalizedValueToY[point.numericalPlot.NormalizedLow] = point.layoutSlot.Bottom;
            }

            if (!normalizedValueToY.ContainsKey(point.numericalPlot.NormalizedHigh))
            {
                normalizedValueToY[point.numericalPlot.NormalizedHigh] = point.layoutSlot.Y;
            }

            double difference = normalizedValueToY[point.numericalPlot.NormalizedLow] - point.layoutSlot.Bottom;
            point.layoutSlot.Height += difference;

            difference = normalizedValueToY[point.numericalPlot.NormalizedHigh] - point.layoutSlot.Y;
            point.layoutSlot.Y += difference;
            point.layoutSlot.Height -= difference;

            if (point.layoutSlot.Height < 0)
            {
                point.layoutSlot.Height = 0;
            }
        }

        internal static void SnapNormalizedValueToPreviousX(RangeDataPoint point, Dictionary<double, double> normalizedValueToX)
        {
            if (!normalizedValueToX.ContainsKey(point.numericalPlot.NormalizedLow))
            {
                normalizedValueToX[point.numericalPlot.NormalizedLow] = point.layoutSlot.X;
            }

            if (!normalizedValueToX.ContainsKey(point.numericalPlot.NormalizedHigh))
            {
                normalizedValueToX[point.numericalPlot.NormalizedHigh] = point.layoutSlot.Right;
            }

            double difference = normalizedValueToX[point.numericalPlot.NormalizedLow] - point.layoutSlot.X;
            point.layoutSlot.X += difference;
            point.layoutSlot.Width -= difference;

            difference = normalizedValueToX[point.numericalPlot.NormalizedHigh] - point.layoutSlot.Right;
            point.layoutSlot.Width += difference;

            if (point.layoutSlot.Width < 0)
            {
                point.layoutSlot.Width = 0;
            }
        }

        internal void SnapToAdjacentPointInHistogramScenario(RangeDataPoint point, DataPoint nextPoint)
        {
            // TODO: Fix histogram bars in scenarios with combination (multiple bar series)
            // NOTE: We intentionally overlap the bar layout slots with single pixel so the border of the visual does not get blurred.
            if (this.PlotDirection == AxisPlotDirection.Vertical)
            {
                point.layoutSlot.Width = nextPoint.layoutSlot.X - point.layoutSlot.X + 1;
            }
            else
            {
                nextPoint.layoutSlot.Height = point.layoutSlot.Y - nextPoint.layoutSlot.Y + 1;
            }
        }

        private static void SnapHighToVerticalGridLine(RangeDataPoint point, RadRect tickRect)
        {
            if (point.isEmpty)
            {
                return;
            }

            double difference;
            double gridLine = tickRect.Y + (int)(tickRect.Height / 2);

            difference = point.layoutSlot.Y - gridLine;
            point.layoutSlot.Y -= difference;
            point.layoutSlot.Height += difference;
       
            if (point.layoutSlot.Height < 0)
            {
                point.layoutSlot.Height = 0;
            }
        }
   
        private static void SnapHighToHorizontalGridLine(RangeDataPoint point, RadRect tickRect)
        {
            double difference;
            double gridLine = tickRect.X + (int)(tickRect.Width / 2);

            difference = point.layoutSlot.Right - gridLine;
            point.layoutSlot.Width -= difference - 1;

            if (point.layoutSlot.Width < 0)
            {
                point.layoutSlot.Width = 0;
            }
        }

        private static void SnapLowToVerticalGridLine(RangeDataPoint point, RadRect tickRect)
        {
            if (point.isEmpty)
            {
                return;
            }

            double difference;
            double gridLine = tickRect.Y + (int)(tickRect.Height / 2);

            difference = point.layoutSlot.Bottom - gridLine;
            point.layoutSlot.Height -= difference;

            if (point.layoutSlot.Height < 0)
            {
                point.layoutSlot.Height = 0;
            }
        }

        private static void SnapLowToHorizontalGridLine(RangeDataPoint point, RadRect tickRect)
        {
            double difference;
            double gridLine = tickRect.X + (int)(tickRect.Width / 2);

            difference = point.layoutSlot.X - gridLine;
            point.layoutSlot.X += 1 - difference;
            point.layoutSlot.Width -= 1 + difference;

            if (point.layoutSlot.Width < 0)
            {
                point.layoutSlot.Width = 0;
            }
        }
    }
}
