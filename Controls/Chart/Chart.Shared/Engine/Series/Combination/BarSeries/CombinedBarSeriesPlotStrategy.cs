using System;

namespace Telerik.Charting
{
    internal class CombinedBarSeriesPlotStrategy : CombinedSeriesPlotStrategy
    {
        public override void Plot(CombinedSeries series, int combinedSeriesCount)
        {
            double groupPosition;
            double groupLength;
            double groupStartPosition;
            double stackPosition;
            double stackLength;
            double stackStartPosition;

            foreach (CombineGroup group in series.Groups)
            {
                CategoricalDataPointBase firstPoint = group.Stacks[0].Points[0] as CategoricalDataPointBase;
                CategoricalAxisPlotInfo plotInfo = firstPoint.categoricalPlot;
                if (plotInfo == null)
                {
                    continue;
                }

                groupLength = plotInfo.Length / combinedSeriesCount;
                stackLength = groupLength / group.Stacks.Count;

                groupStartPosition = plotInfo.Position - plotInfo.Length / 2;
                groupPosition = groupStartPosition + groupLength / 2 + (series.CombineIndex * groupLength);

                stackStartPosition = groupPosition - groupLength / 2;
                stackPosition = stackStartPosition + stackLength / 2;

                foreach (CombineStack stack in group.Stacks)
                {
                    foreach (CategoricalDataPointBase point in stack.Points)
                    {
                        plotInfo = point.categoricalPlot;
                        if (plotInfo != null)
                        {
                            plotInfo.Position = stackPosition;
                            plotInfo.Length = stackLength;
                        }
                    }

                    stackPosition += stackLength;
                }
            }
        }
    }
}
