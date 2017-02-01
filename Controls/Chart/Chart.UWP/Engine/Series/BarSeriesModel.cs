using System;
using Telerik.Core;

namespace Telerik.Charting
{
    internal class BarSeriesModel : CategoricalSeriesModel
    {
        internal override CombinedSeriesPlotStrategy GetCombinedPlotStrategy()
        {
            return new CombinedBarSeriesPlotStrategy();
        }

        internal override CombinedSeriesRoundLayoutStrategy GetCombinedRoundLayoutStrategy()
        {
            return new CombinedBarSeriesRoundLayoutStrategy();
        }

        internal override void ApplyLayoutRounding()
        {
            CategoricalSeriesRoundLayoutContext info = new CategoricalSeriesRoundLayoutContext(this);

            double gapLength = CategoricalAxisModel.DefaultGapLength;
            ISupportGapLength axisModel = this.firstAxis as ISupportGapLength;
            if (axisModel == null)
            {
                axisModel = this.secondAxis as ISupportGapLength;
            }

            if (axisModel != null)
            {
                gapLength = axisModel.GapLength;
            }

            int count = this.DataPointsInternal.Count;
            foreach (CategoricalDataPoint point in this.DataPointsInternal)
            {
                info.SnapPointToPlotLine(point);
                info.SnapPointToGridLine(point);

                if (gapLength == 0 && point.CollectionIndex < count - 1)
                {
                    DataPoint nextPoint = this.DataPointsInternal[point.CollectionIndex + 1];
                    info.SnapToAdjacentPointInHistogramScenario(point, nextPoint);
                }
            }
        }

        internal override RadRect ArrangeOverride(RadRect plotAreaRect)
        {
            plotAreaRect = this.GetZoomedRect(plotAreaRect);

            double x = double.NaN, y = double.NaN;
            double width = 0, height = 0;
            AxisPlotDirection plotDirection = this.GetTypedValue<AxisPlotDirection>(AxisModel.PlotDirectionPropertyKey, AxisPlotDirection.Vertical);

            foreach (CategoricalDataPoint point in this.DataPoints)
            {
                if (point.categoricalPlot == null)
                {
                    continue;
                }

                if (plotDirection == AxisPlotDirection.Vertical)
                {
                    // vertical bars
                    x = plotAreaRect.X + ((point.categoricalPlot.Position - point.categoricalPlot.Length / 2) * plotAreaRect.Width);
                    width = point.categoricalPlot.Length * plotAreaRect.Width;
                    if (point.numericalPlot != null)
                    { 
                        height = Math.Abs(point.numericalPlot.NormalizedValue - point.numericalPlot.PlotOriginOffset) * plotAreaRect.Height;
                        y = plotAreaRect.Y + ((1 - Math.Max(point.numericalPlot.NormalizedValue, point.numericalPlot.PlotOriginOffset)) * plotAreaRect.Height);
                    }
                }
                else
                {
                    // horizontal bars
                    if (point.numericalPlot != null)
                    {
                        x = plotAreaRect.X + (Math.Min(point.numericalPlot.NormalizedValue, point.numericalPlot.PlotOriginOffset) * plotAreaRect.Width);
                        width = Math.Abs(point.numericalPlot.NormalizedValue - point.numericalPlot.PlotOriginOffset) * plotAreaRect.Width;
                    }
                    height = point.categoricalPlot.Length * plotAreaRect.Height;
                    y = plotAreaRect.Bottom - (((point.categoricalPlot.Position - point.categoricalPlot.Length / 2) * plotAreaRect.Height) + height);
                }

                point.Arrange(new RadRect(x, y, width, height));
                x = double.NaN;
                y = double.NaN;
                width = 0;
                height = 0;
            }

            return plotAreaRect;
        }
    }
}