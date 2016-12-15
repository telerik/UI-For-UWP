using System;
using System.Linq;
using Telerik.Core;

namespace Telerik.Charting
{
    internal class CartesianMarkedZoneAnnotationModel : CartesianFromToAnnotationModel
    {
        protected override RadRect ArrangeCore(RadRect rect)
        {
            IChartView view = this.GetChartArea().view;

            RadRect plotAreaVirtualSize = new RadRect(rect.X, rect.Y, rect.Width * view.ZoomWidth, rect.Height * view.ZoomHeight);
            
            double panOffsetX = view.PlotOriginX * rect.Width;
            double panOffsetY = view.PlotOriginY * rect.Height;

            RadPoint point1 = new RadPoint(panOffsetX + this.horizontalFromPlotInfo.CenterX(plotAreaVirtualSize), panOffsetY + this.verticalFromPlotInfo.CenterY(plotAreaVirtualSize));
            RadPoint point2 = new RadPoint(panOffsetX + this.horizontalToPlotInfo.CenterX(plotAreaVirtualSize), panOffsetY + this.verticalToPlotInfo.CenterY(plotAreaVirtualSize));
                        
            var arrangeRect = new RadRect(point1, point2);
            this.originalLayoutSlot = arrangeRect;
            return AnnotationHelper.ClipRectangle(arrangeRect, rect, this.StrokeThickness * 2, this.DashPatternLength * this.StrokeThickness);
        }
    }
}