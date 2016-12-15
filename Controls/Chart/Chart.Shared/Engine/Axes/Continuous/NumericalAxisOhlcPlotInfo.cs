namespace Telerik.Charting
{
    internal class NumericalAxisOhlcPlotInfo : NumericalAxisPlotInfoBase
    {
        public double NormalizedHigh;
        public double NormalizedLow;
        public double NormalizedOpen;
        public double NormalizedClose;
        public double RelativeOpen;
        public double RelativeClose;

        public double PhysicalOpen = -1;
        public double PhysicalClose = -1;

        public int SnapBaseTickIndex = -1;
        public int SnapOpenTickIndex = -1;
        public int SnapCloseTickIndex = -1;

        internal static NumericalAxisOhlcPlotInfo Create(AxisModel axis, double plotOffset, double high, double low, double open, double close, double origin)
        {
            NumericalAxisOhlcPlotInfo info = new NumericalAxisOhlcPlotInfo();
            info.Axis = axis;
            info.PlotOriginOffset = plotOffset;
            info.NormalizedHigh = high;
            info.NormalizedLow = low;
            info.NormalizedOpen = open;
            info.NormalizedClose = close;
            info.NormalizedOrigin = origin;

            if (high == low)
            {
                info.RelativeOpen = 0;
                info.RelativeClose = 0;
            }
            else
            {
                info.RelativeOpen = (open - low) / (high - low);
                info.RelativeClose = (close - low) / (high - low);
            }

            return info;
        }
    }
}
