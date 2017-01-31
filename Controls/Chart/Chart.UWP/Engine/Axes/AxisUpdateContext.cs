using System;
using System.Collections.Generic;

namespace Telerik.Charting
{
    internal class AxisUpdateContext
    {
        public bool IsStacked;
        public bool IsStacked100;
        public double MaximumStackSum;
        public double MinimumStackSum;
        public IList<ChartSeriesModel> Series;
        public List<CombinedSeries> CombinedSeries;
        public List<ChartSeriesModel> NonCombinedSeries;

        public AxisUpdateContext(AxisModel axis, IList<ChartSeriesModel> series, IEnumerable<ChartSeriesCombineStrategy> seriesCombineStrategies)
        {
            this.Series = series;
            if (axis.SupportsCombinedPlot)
            {
                return;
            }

            this.CombinedSeries = new List<CombinedSeries>();
            this.NonCombinedSeries = new List<ChartSeriesModel>();
            this.MinimumStackSum = double.PositiveInfinity;
            this.MaximumStackSum = double.NegativeInfinity;

            foreach (ChartSeriesCombineStrategy combineStrategy in seriesCombineStrategies)
            {
                if (!combineStrategy.StackValueAxes.Contains(axis))
                {
                    continue;
                }

                // extract only relevant combined and non combined series.
                foreach (var seriesModel in series)
                {
                    bool isCombined = false;
                    foreach (var combinedSeries in combineStrategy.CombinedSeries)
                    {
                        if (combinedSeries.Series.Contains(seriesModel))
                        {
                            this.CombinedSeries.Add(combinedSeries);
                            this.IsStacked |= combinedSeries.CombineMode == ChartSeriesCombineMode.Stack;
                            this.IsStacked100 |= combinedSeries.CombineMode == ChartSeriesCombineMode.Stack100;
                            isCombined = true;
                        }
                    }
                    if (!isCombined && !this.NonCombinedSeries.Contains(seriesModel))
                    {
                        this.NonCombinedSeries.Add(seriesModel);
                    }
                }

                if (combineStrategy.MinimumStackSums.ContainsKey(axis))
                {
                    this.MinimumStackSum = Math.Min(combineStrategy.MinimumStackSums[axis], this.MinimumStackSum);
                    this.MaximumStackSum = Math.Max(combineStrategy.MaximumStackSums[axis], this.MaximumStackSum);
                }
            }
        }
    }
}
