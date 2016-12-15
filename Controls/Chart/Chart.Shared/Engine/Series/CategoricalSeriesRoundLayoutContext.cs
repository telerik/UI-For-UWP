using System;
using System.Diagnostics;
using Telerik.Core;

namespace Telerik.Charting
{
    internal class CategoricalSeriesRoundLayoutContext
    {
        public double PlotLine;
        public double PlotOrigin;
        public AxisPlotDirection PlotDirection;
        public RadRect PlotArea;

        public CategoricalSeriesRoundLayoutContext(CategoricalSeriesModel series)
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

        public void SnapPointToPlotLine(CategoricalDataPoint point)
        {
            if (point.numericalPlot == null)
            {
                return;
            }

            if (this.PlotDirection == AxisPlotDirection.Vertical)
            {
                // positive point with regular axis is equivalent to negative point with inverse axis
                if (point.isPositive ^ point.numericalPlot.Axis.IsInverse)
                {
                    point.layoutSlot.Y = this.PlotLine - point.layoutSlot.Height;
                    if (this.PlotOrigin > 0)
                    {
                        point.layoutSlot.Y++;
                    }
                }
                else
                {
                    point.layoutSlot.Y = this.PlotLine;
                }
            }
            else
            {
                // positive point with regular axis is equivalent to negative point with inverse axis
                if (point.isPositive ^ point.numericalPlot.Axis.IsInverse)
                {
                    point.layoutSlot.X = this.PlotLine;
                }
                else
                {
                    point.layoutSlot.X = this.PlotLine - point.layoutSlot.Width;
                    if (this.PlotOrigin < 1)
                    {
                        point.layoutSlot.X++;
                    }
                }
            }
        }

        public void SnapPointToGridLine(CategoricalDataPoint point)
        {
            if (point.numericalPlot == null)
            {
                return;
            }

            if (point.numericalPlot.SnapTickIndex < 0 ||
                point.numericalPlot.SnapTickIndex >= point.numericalPlot.Axis.ticks.Count)
            {
                return;
            }

            AxisTickModel tick = point.numericalPlot.Axis.ticks[point.numericalPlot.SnapTickIndex];
            if (!RadMath.AreClose(point.numericalPlot.NormalizedValue, (double)tick.normalizedValue))
            {
                return;
            }

            if (this.PlotDirection == AxisPlotDirection.Vertical)
            {
                CategoricalSeriesRoundLayoutContext.SnapToGridLineVertical(point, tick.layoutSlot);
            }
            else
            {
                CategoricalSeriesRoundLayoutContext.SnapToGridLineHorizontal(point, tick.layoutSlot);
            }
        }

        internal void SnapToAdjacentPointInHistogramScenario(CategoricalDataPoint point, DataPoint nextPoint)
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

        private static void SnapToGridLineHorizontal(CategoricalDataPoint point, RadRect tickRect)
        {
            double difference;
            double gridLine = tickRect.X + (int)(tickRect.Width / 2);

            // positive point with regular axis is equivalent to negative point with inverse axis
            if (point.isPositive ^ point.numericalPlot.Axis.IsInverse)
            {
                difference = point.layoutSlot.Right - gridLine;
                point.layoutSlot.Width -= difference - 1;
            }
            else
            {
                difference = gridLine - point.layoutSlot.X;
                point.layoutSlot.X += difference;
                point.layoutSlot.Width -= difference;
            }

            if (point.layoutSlot.Width < 0)
            {
                point.layoutSlot.Width = 0;
            }
        }

        private static void SnapToGridLineVertical(CategoricalDataPoint point, RadRect tickRect)
        {
            double difference;
            double gridLine = tickRect.Y + (int)(tickRect.Height / 2);

            // positive point with regular axis is equivalent to negative point with inverse axis
            if (point.isPositive ^ point.numericalPlot.Axis.IsInverse)
            {
                difference = point.layoutSlot.Y - gridLine;
                point.layoutSlot.Y -= difference;
                point.layoutSlot.Height += difference;
            }
            else
            {
                difference = gridLine - point.layoutSlot.Bottom;
                point.layoutSlot.Height += difference + 1;
            }

            if (point.layoutSlot.Height < 0)
            {
                point.layoutSlot.Height = 0;
            }
        }
    }
}