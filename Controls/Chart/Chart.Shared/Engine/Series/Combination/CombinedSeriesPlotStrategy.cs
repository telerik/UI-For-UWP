using System;
using System.Diagnostics;

namespace Telerik.Charting
{
    internal abstract class CombinedSeriesPlotStrategy
    {
        public abstract void Plot(CombinedSeries series, int combinedSeriesCount);
    }
}
