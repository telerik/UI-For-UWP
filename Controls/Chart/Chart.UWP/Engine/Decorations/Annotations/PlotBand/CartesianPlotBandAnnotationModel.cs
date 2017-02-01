using Telerik.Core;

namespace Telerik.Charting
{
    internal class CartesianPlotBandAnnotationModel : PlotBandAnnotationModel
    {
        protected override RadRect ArrangeCore(RadRect rect)
        {
            IChartView view = this.GetChartArea().view;
            RadRect plotAreaVirtualSize = new RadRect(rect.X, rect.Y, rect.Width * view.ZoomWidth, rect.Height * view.ZoomHeight);
            RadPoint point1, point2;

            double panOffsetX = view.PlotOriginX * rect.Width;
            double panOffsetY = view.PlotOriginY * rect.Height;

            if (this.axis.type == AxisType.First)
            {
                point1 = new RadPoint(panOffsetX + this.firstPlotInfo.CenterX(plotAreaVirtualSize), plotAreaVirtualSize.Y + panOffsetY);
                point2 = new RadPoint(panOffsetX + this.secondPlotInfo.CenterX(plotAreaVirtualSize), plotAreaVirtualSize.Bottom + panOffsetY);
            }
            else
            {
                point1 = new RadPoint(plotAreaVirtualSize.X + panOffsetX, panOffsetY + this.firstPlotInfo.CenterY(plotAreaVirtualSize));
                point2 = new RadPoint(plotAreaVirtualSize.Right + panOffsetX, panOffsetY + this.secondPlotInfo.CenterY(plotAreaVirtualSize));
            }

            var arrangeRect = new RadRect(point1, point2);
            this.originalLayoutSlot = arrangeRect;
            return AnnotationHelper.ClipRectangle(arrangeRect, rect, this.StrokeThickness * 2, this.DashPatternLength * this.StrokeThickness);
        }
    }
}