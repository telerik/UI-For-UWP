using Telerik.Core;

namespace Telerik.Charting
{
    internal class RadialPlotBandAnnotationModel : PlotBandAnnotationModel
    {
        internal RadPolarVector polarVector1, polarVector2;
        internal double radius;

        protected override RadRect ArrangeCore(RadRect rect)
        {
            this.radius = rect.Width / 2;
            RadPoint center = rect.Center;

            AxisPlotInfo anglePlot1 = this.firstPlotInfo;
            double angle1 = 0d;

            NumericalAxisPlotInfo numericalAnglePlot1 = anglePlot1 as NumericalAxisPlotInfo;
            if (numericalAnglePlot1 != null)
            {
                angle1 = numericalAnglePlot1.ConvertToAngle();
            }
            else
            {
                CategoricalAxisPlotInfo categoricalAnglePlot1 = anglePlot1 as CategoricalAxisPlotInfo;
                if (categoricalAnglePlot1 != null)
                {
                    angle1 = categoricalAnglePlot1.ConvertToAngle(this.GetChartArea<PolarChartAreaModel>());
                }
            }

            RadPoint arcPoint1 = RadMath.GetArcPoint(angle1, center, this.radius);
            this.polarVector1 = new RadPolarVector() { Point = arcPoint1, Angle = angle1, Center = center };

            AxisPlotInfo anglePlot2 = this.secondPlotInfo;
            double angle2 = 0d;

            NumericalAxisPlotInfo numericalAnglePlot2 = anglePlot2 as NumericalAxisPlotInfo;
            if (numericalAnglePlot2 != null)
            {
                angle2 = numericalAnglePlot2.ConvertToAngle();
            }
            else
            {
                CategoricalAxisPlotInfo categoricalAnglePlot2 = anglePlot2 as CategoricalAxisPlotInfo;
                if (categoricalAnglePlot2 != null)
                {
                    angle2 = categoricalAnglePlot2.ConvertToAngle(this.GetChartArea<PolarChartAreaModel>());
                }
            }

            RadPoint arcPoint2 = RadMath.GetArcPoint(angle2, center, this.radius);
            this.polarVector2 = new RadPolarVector() { Point = arcPoint2, Angle = angle2, Center = center };

            return rect;
        }
    }
}
