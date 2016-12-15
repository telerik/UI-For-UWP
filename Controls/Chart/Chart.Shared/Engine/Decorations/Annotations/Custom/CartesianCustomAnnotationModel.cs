using Telerik.Core;

namespace Telerik.Charting
{
    internal class CartesianCustomAnnotationModel : CustomAnnotationModel
    {
        protected override RadRect ArrangeCore(RadRect rect)
        {
            IChartView view = this.GetChartArea().view;
            RadRect plotAreaVirtualSize = new RadRect(rect.X, rect.Y, rect.Width * view.ZoomWidth, rect.Height * view.ZoomHeight);

            double panOffsetX = view.PlotOriginX * rect.Width;
            double panOffsetY = view.PlotOriginY * rect.Height;

            RadPoint centerPoint = new RadPoint(panOffsetX + this.firstPlotInfo.CenterX(plotAreaVirtualSize), panOffsetY + this.secondPlotInfo.CenterY(plotAreaVirtualSize));
            RadSize desiredSize = this.Measure();

            return new RadRect(centerPoint.X, centerPoint.Y, desiredSize.Width, desiredSize.Height);
        }
    }
}
