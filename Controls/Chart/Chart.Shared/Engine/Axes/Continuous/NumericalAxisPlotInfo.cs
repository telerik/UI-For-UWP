using Telerik.Core;

namespace Telerik.Charting
{
    internal class NumericalAxisPlotInfo : NumericalAxisPlotInfoBase
    {
        public double NormalizedValue;

        public static NumericalAxisPlotInfo Create(AxisModel axis, double plotOffset, double value, double origin)
        {
            NumericalAxisPlotInfo info = new NumericalAxisPlotInfo();
            info.Axis = axis;
            info.PlotOriginOffset = plotOffset;
            info.NormalizedValue = value;
            info.NormalizedOrigin = origin;

            return info;
        }

        public override double CenterX(RadRect relativeRect)
        {
            return relativeRect.X + (this.NormalizedValue * relativeRect.Width);
        }

        public override double CenterY(RadRect relativeRect)
        {
            return relativeRect.Y + (relativeRect.Height * (1 - this.NormalizedValue));
        }

        public double ConvertToAngle()
        {
            return this.NormalizedValue * 360;
        }
    }
}
