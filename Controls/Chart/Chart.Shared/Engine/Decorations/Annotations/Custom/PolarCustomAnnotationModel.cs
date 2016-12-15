using Telerik.Core;

namespace Telerik.Charting
{
    internal class PolarCustomAnnotationModel : CustomAnnotationModel
    {
        internal double angle;

        protected override RadRect ArrangeCore(RadRect rect)
        {
            double radius = rect.Width / 2;
            RadPoint center = rect.Center;
            NumericalAxisPlotInfo polarPlot = this.firstPlotInfo as NumericalAxisPlotInfo;
            double pointRadius = polarPlot.NormalizedValue * radius;
            AxisPlotInfo anglePlot = this.secondPlotInfo;
            this.angle = 0d;

            NumericalAxisPlotInfo numericalAnglePlot = anglePlot as NumericalAxisPlotInfo;
            if (numericalAnglePlot != null)
            {
                this.angle = numericalAnglePlot.ConvertToAngle();
            }
            else
            {
                CategoricalAxisPlotInfo categoricalAnglePlot = anglePlot as CategoricalAxisPlotInfo;
                if (categoricalAnglePlot != null)
                {
                    this.angle = categoricalAnglePlot.ConvertToAngle(this.GetChartArea<PolarChartAreaModel>());
                }
            }

            RadPoint arcPosition = RadMath.GetArcPoint(this.angle, center, pointRadius);
            RadSize desiredSize = this.Measure();

            return new RadRect(arcPosition.X, arcPosition.Y, desiredSize.Width, desiredSize.Height);
        }
    }
}
