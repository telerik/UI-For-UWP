using Telerik.Core;

namespace Telerik.Charting
{
    internal class RadialGridLineAnnotationModel : GridLineAnnotationModel
    {
        internal RadPolarVector radialLine;

        protected override RadRect ArrangeCore(RadRect rect)
        {
            double radius = rect.Width / 2;
            RadPoint center = rect.Center;
            AxisPlotInfo anglePlot = this.plotInfo;
            double angle = 0d;

            NumericalAxisPlotInfo numericalAnglePlot = anglePlot as NumericalAxisPlotInfo;
            if (numericalAnglePlot != null)
            {
                angle = numericalAnglePlot.ConvertToAngle();
            }
            else
            {
                CategoricalAxisPlotInfo categoricalAnglePlot = anglePlot as CategoricalAxisPlotInfo;
                if (categoricalAnglePlot != null)
                {
                    angle = categoricalAnglePlot.ConvertToAngle(this.GetChartArea<PolarChartAreaModel>());
                }
            }

            RadPoint arcPoint = RadMath.GetArcPoint(angle, center, radius);
            this.radialLine = new RadPolarVector() { Point = arcPoint, Angle = angle, Center = center };

            return new RadRect(center, arcPoint);
        }
    }
}
