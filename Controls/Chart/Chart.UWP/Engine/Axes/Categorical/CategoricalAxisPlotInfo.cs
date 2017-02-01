using Telerik.Core;

namespace Telerik.Charting
{
    internal class CategoricalAxisPlotInfo : AxisPlotInfo
    {
        public double Position;
        public double Length;
        public double RangePosition; // position of the axis range, containing the slot
        public object CategoryKey;

        public static CategoricalAxisPlotInfo Create(AxisModel axis, double value)
        {
            CategoricalAxisPlotInfo info = new CategoricalAxisPlotInfo();
            info.Axis = axis;
            info.RangePosition = value;

            return info;
        }

        public override double CenterX(RadRect relativeRect)
        {
            return relativeRect.X + (this.Position * relativeRect.Width);
        }

        public override double CenterY(RadRect relativeRect)
        {
            return relativeRect.Bottom - (this.Position * relativeRect.Height);
        }

        public double ConvertToAngle(PolarChartAreaModel chartArea)
        {
            double angle = this.RangePosition * 360;
            angle = chartArea.NormalizeAngle(angle);

            return angle;
        }
    }
}