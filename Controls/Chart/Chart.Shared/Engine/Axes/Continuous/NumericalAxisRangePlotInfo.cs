namespace Telerik.Charting
{
    internal class NumericalAxisRangePlotInfo : NumericalAxisPlotInfoBase
    {
        public double NormalizedHigh;
        public double NormalizedLow;

        public int SnapBaseTickIndex = -1;

        internal static NumericalAxisRangePlotInfo Create(AxisModel axis, double plotOriginOffset, double normalizedHigh, double normalizedLow, double normalizedOrigin)
        {
            NumericalAxisRangePlotInfo info = new NumericalAxisRangePlotInfo();
            info.Axis = axis;
            info.PlotOriginOffset = plotOriginOffset;
            info.NormalizedHigh = normalizedHigh;
            info.NormalizedLow = normalizedLow;
            info.NormalizedOrigin = normalizedOrigin;

            return info;
        }
    }
}
