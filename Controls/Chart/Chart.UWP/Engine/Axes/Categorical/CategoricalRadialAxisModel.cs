using System.Collections.Generic;
using Telerik.Core;

namespace Telerik.Charting
{
    internal class CategoricalRadialAxisModel : CategoricalAxisModel, IRadialAxis
    {
        internal RadialAxisLayoutStrategy radialLayoutStrategy;

        public bool IsLargeArc
        {
            get
            {
                return this.categories.Count <= 1;
            }
        }

        internal override AxisPlotMode ActualPlotMode
        {
            get
            {
                return AxisPlotMode.OnTicks;
            }
        }

        internal override void UpdateActualPlotMode(IList<ChartSeriesModel> seriesModels)
        {
            this.actualPlotMode = AxisPlotMode.OnTicks;
        }

        internal override IEnumerable<AxisTickModel> GenerateTicks(ValueRange<decimal> currentVisibleRange)
        {
            int categoryCount = this.categories.Count;
            if (categoryCount == 0)
            {
                yield break;
            }

            int tickInterval = this.GetMajorTickInterval();
            int emptyTickCount = 0;

            int tickCount = categoryCount;
            decimal tickStep = tickCount == 1 ? 1 : 1m / tickCount;
            decimal normalizedTickStep = tickStep * 360;

            decimal startTick = 0;
            decimal endTick = 1 - tickStep;
            decimal currentTick = startTick;
            decimal value = 0;

            int virtualIndex = (int)(startTick / tickStep);
            while (currentTick < endTick || RadMath.AreClose((double)currentTick, (double)endTick))
            {
                if (emptyTickCount == 0)
                {
                    AxisTickModel tick = new MajorTickModel();
                    tick.normalizedValue = this.IsInverse ? 1 - currentTick : currentTick;
                    tick.value = value;
                    tick.virtualIndex = virtualIndex;

                    emptyTickCount = tickInterval - 1;

                    yield return tick;
                }
                else
                {
                    emptyTickCount--;
                }

                currentTick += tickStep;
                value += normalizedTickStep;
                virtualIndex++;
            }
        }

        internal override object GetLabelContent(AxisTickModel tick)
        {
            if (tick.virtualIndex < this.categories.Count)
            {
                return this.categories[tick.virtualIndex].Key;
            }

            return null;
        }

        internal override AxisModelLayoutStrategy CreateLayoutStrategy()
        {
            this.radialLayoutStrategy = new RadialAxisLayoutStrategy();
            return this.radialLayoutStrategy;
        }

        internal override double CalculateRelativePosition(double coordinate, RadRect axisVirtualSize, double stepOffset = 0)
        {
            // Circumference restricted in range [0,1].
            return (coordinate / 360d + stepOffset) % 1;
        }

        protected override double CalculateRelativeStep(int count)
        {
            return 1d / count;
        }
    }
}