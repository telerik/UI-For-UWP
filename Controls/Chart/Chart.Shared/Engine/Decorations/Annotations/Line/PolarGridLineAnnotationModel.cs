using Telerik.Core;

namespace Telerik.Charting
{
    internal class PolarGridLineAnnotationModel : GridLineAnnotationModel
    {
        internal RadCircle polarLine;

        protected override RadRect ArrangeCore(RadRect rect)
        {
            double radius = rect.Width / 2;
            RadPoint center = rect.Center;
            NumericalAxisPlotInfo polarPlot = this.plotInfo as NumericalAxisPlotInfo;
            double pointRadius = polarPlot.NormalizedValue * radius;
            this.polarLine = new RadCircle(center, pointRadius);

            return this.polarLine.Bounds;
        }
    }
}
