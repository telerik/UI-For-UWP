using System;
using Telerik.Core;

namespace Telerik.Charting
{
    internal class CartesianCustomLineAnnotationModel : CartesianFromToAnnotationModel
    {
        internal RadLine line;

        protected override RadRect ArrangeCore(RadRect rect)
        {
            IChartView view = this.GetChartArea().view;

            RadRect plotAreaVirtualSize = new RadRect(rect.X, rect.Y, rect.Width * view.ZoomWidth, rect.Height * view.ZoomHeight);
            
            double panOffsetX = view.PlotOriginX * rect.Width;
            double panOffsetY = view.PlotOriginY * rect.Height;

            RadPoint point1 = new RadPoint(panOffsetX + this.horizontalFromPlotInfo.CenterX(plotAreaVirtualSize), panOffsetY + this.verticalFromPlotInfo.CenterY(plotAreaVirtualSize));
            RadPoint point2 = new RadPoint(panOffsetX + this.horizontalToPlotInfo.CenterX(plotAreaVirtualSize), panOffsetY + this.verticalToPlotInfo.CenterY(plotAreaVirtualSize));

            this.line = new RadLine(point1, point2);
            this.originalLayoutSlot = new RadRect(new RadPoint(this.line.X1, this.line.Y1), new RadPoint(this.line.X2, this.line.Y2));
            this.line = AnnotationHelper.ClipLine(this.line, rect, 2 * this.StrokeThickness, this.DashPatternLength * this.StrokeThickness);
            this.line = RadLine.Round(this.line);

            return new RadRect(new RadPoint(this.line.X1, this.line.Y1), new RadPoint(this.line.X2, this.line.Y2));
        }
    }
}