using System;

namespace Telerik.Charting
{
    internal abstract class CombinedSeriesRoundLayoutStrategy
    {
        public abstract void ApplyLayoutRounding(ChartAreaModel chart, CombinedSeries series);
    }
}
