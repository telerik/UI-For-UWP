using Telerik.Core;

namespace Telerik.Charting
{
    internal abstract class AxisPlotInfo
    {
        public AxisModel Axis;
        public int SnapTickIndex = -1;

        public virtual double CenterX(RadRect relativeRect)
        {
            return 0d;
        }

        public virtual double CenterY(RadRect relativeRect)
        {
            return 0d;
        }
    }
}
