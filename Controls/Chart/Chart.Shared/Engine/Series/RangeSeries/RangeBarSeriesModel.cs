using System.Collections.Generic;
using Telerik.Core;

namespace Telerik.Charting
{
    internal class RangeBarSeriesModel : RangeSeriesModel, ISupportCombineMode
    {
        internal static readonly int CombineModePropertyKey = PropertyKeys.Register(typeof(RangeBarSeriesModel), "CombineMode", ChartAreaInvalidateFlags.All);

        /// <summary>
        /// Gets or sets the <see cref="ChartSeriesCombineMode"/> value that specifies whether this instance should be combined with other instances of same type.
        /// </summary>
        public ChartSeriesCombineMode CombineMode
        {
            get
            {
                return this.GetTypedValue<ChartSeriesCombineMode>(CombineModePropertyKey, ChartSeriesCombineMode.Cluster);
            }
            set
            {
                this.SetValue(CombineModePropertyKey, value);
            }
        }

        // <summary>
        // RangeBarSeries do not support stacking and this property is disregarded. 
        // </summary>
        public object StackGroupKey { get; set; }

        internal override AxisPlotMode DefaultPlotMode
        {
            get { return AxisPlotMode.BetweenTicks; }
        }
        protected override bool ShouldRoundLayout
        {
            get { return true; }
        }

        internal override CombinedSeriesPlotStrategy GetCombinedPlotStrategy()
        {
            return new CombinedBarSeriesPlotStrategy();
        }

        internal override CombinedSeriesRoundLayoutStrategy GetCombinedRoundLayoutStrategy()
        {
            return new CombinedRangeBarSeriesRoundLayoutStrategy();
        }

        internal override void ApplyLayoutRounding()
        {
            RangeSeriesRoundLayoutContext info = new RangeSeriesRoundLayoutContext(this);

            double gapLength = CategoricalAxisModel.DefaultGapLength;
            ISupportGapLength axisModel = this.firstAxis as ISupportGapLength;
            if (axisModel == null)
            {
                axisModel = this.secondAxis as ISupportGapLength;
            }

            if (axisModel != null)
            {
                gapLength = axisModel.GapLength;
            }

            int count = this.DataPointsInternal.Count;

            AxisPlotDirection plotDirection = this.GetTypedValue<AxisPlotDirection>(AxisModel.PlotDirectionPropertyKey, AxisPlotDirection.Vertical);
            Dictionary<double, double> normalizedValueToY = new Dictionary<double, double>();
            Dictionary<double, double> normalizedValueToX = new Dictionary<double, double>();

            foreach (RangeDataPoint point in this.DataPointsInternal)
            {
                if (point.isEmpty)
                {
                    continue;
                }

                info.SnapPointToGridLine(point);

                // Handles specific scenario where range bar items from non-combined series have the same high/low value (floating point number) i.e. 
                // the presenters should be rendered on the same horizontal/vertical pixel row/column.
                if (plotDirection == AxisPlotDirection.Vertical)
                {
                    RangeSeriesRoundLayoutContext.SnapNormalizedValueToPreviousY(point, normalizedValueToY);
                }
                else
                {
                    RangeSeriesRoundLayoutContext.SnapNormalizedValueToPreviousX(point, normalizedValueToX);
                }

                if (gapLength == 0 && point.CollectionIndex < count - 1)
                {
                    DataPoint nextPoint = this.DataPointsInternal[point.CollectionIndex + 1];
                    info.SnapToAdjacentPointInHistogramScenario(point, nextPoint);
                }
            }
        }
    }
}
