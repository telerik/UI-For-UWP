using Telerik.Core;

namespace Telerik.Charting
{
    internal class CartesianGridLineAnnotationModel : GridLineAnnotationModel
    {
        internal RadLine line;

        protected override RadRect ArrangeCore(RadRect rect)
        {
            IChartView view = this.GetChartArea().view;

            RadRect plotAreaVirtualSize = new RadRect(rect.X, rect.Y, rect.Width * view.ZoomWidth, rect.Height * view.ZoomHeight);
            RadPoint point1, point2;

            double panOffsetX = view.PlotOriginX * rect.Width;
            double panOffsetY = view.PlotOriginY * rect.Height;

            if (this.axis.type == AxisType.First)
            {
                point1 = new RadPoint(panOffsetX + this.plotInfo.CenterX(plotAreaVirtualSize), plotAreaVirtualSize.Y + panOffsetY);
                point2 = new RadPoint(point1.X, plotAreaVirtualSize.Bottom + panOffsetY);
            }
            else
            {
                point1 = new RadPoint(plotAreaVirtualSize.X + panOffsetX, panOffsetY + this.plotInfo.CenterY(plotAreaVirtualSize));
                point2 = new RadPoint(plotAreaVirtualSize.Right + panOffsetX, point1.Y);
            }

            this.line = new RadLine(point1, point2);
            this.originalLayoutSlot = new RadRect(new RadPoint(this.line.X1, this.line.Y1), new RadPoint(this.line.X2, this.line.Y2));
            this.line = AnnotationHelper.ClipGridLine(this.line, rect, this.StrokeThickness * 2, this.DashPatternLength * this.StrokeThickness);
            this.line = RadLine.Round(this.line);

            return new RadRect(new RadPoint(this.line.X1, this.line.Y1), new RadPoint(this.line.X2, this.line.Y2));
        }
    }
}