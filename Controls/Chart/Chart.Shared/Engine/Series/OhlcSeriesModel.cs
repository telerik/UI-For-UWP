using System;
using Telerik.Core;

namespace Telerik.Charting
{
    internal class OhlcSeriesModel : SeriesModelWithAxes<OhlcDataPoint>
    {
        internal override AxisPlotMode DefaultPlotMode
        {
            get
            {
                return AxisPlotMode.OnTicksPadded;
            }
        }

        internal override void ApplyLayoutRounding()
        {
            foreach (OhlcDataPoint point in this.DataPointsInternal)
            {
                OhlcSeriesRoundLayoutContext.SnapPointToGrid(point);
            }
        }

        internal override RadRect ArrangeOverride(RadRect plotAreaRect)
        {
            AxisPlotDirection plotDirection = this.GetTypedValue<AxisPlotDirection>(AxisModel.PlotDirectionPropertyKey, AxisPlotDirection.Vertical);
            if (plotDirection == AxisPlotDirection.Horizontal)
            {
                throw new NotSupportedException("Horizontal OHLC series are not supported.");
            }

            LinearAxisModel axisModel = this.firstAxis as LinearAxisModel;
            if (axisModel == null)
            {
                axisModel = this.secondAxis as LinearAxisModel;
            }
            if (axisModel != null && axisModel.IsInverse)
            {
                throw new NotSupportedException("Ohlc and Candlestick series do not support inverse numerical axis. Set the IsInverse property of the numerical axis to false.");
            }

            plotAreaRect = this.GetZoomedRect(plotAreaRect);

            double x = double.NaN, y = double.NaN;
            double width = 0, height = 0;

            foreach (OhlcDataPoint point in this.DataPoints)
            {
                if (point.categoricalPlot == null) 
                {
                    continue;
                }

                x = plotAreaRect.X + ((point.categoricalPlot.Position - point.categoricalPlot.Length / 2) * plotAreaRect.Width);
                width = point.categoricalPlot.Length * plotAreaRect.Width;

                if (point.numericalPlot != null)
                {
                    height = Math.Abs(point.numericalPlot.NormalizedHigh - point.numericalPlot.NormalizedLow) * plotAreaRect.Height;
                    y = plotAreaRect.Y + ((1 - point.numericalPlot.NormalizedHigh) * plotAreaRect.Height);
                
                    // NOTE: We need to calculate pixel values here so we can later round the inner open/close elements.
                    point.numericalPlot.PhysicalOpen = (int)((1 - point.numericalPlot.RelativeOpen) * height);
                    point.numericalPlot.PhysicalClose = (int)((1 - point.numericalPlot.RelativeClose) * height);
                }

                point.Arrange(new RadRect(x, y, width, height));

                x = double.NaN;
                y = double.NaN;
                width = 0;
                height = 0;
            }
         
            return plotAreaRect;
        }

        internal override ModifyChildrenResult CanAddChild(Node child)
        {
            if (child is OhlcDataPoint)
            {
                return ModifyChildrenResult.Accept;
            }

            return base.CanAddChild(child);
        }
    }
}