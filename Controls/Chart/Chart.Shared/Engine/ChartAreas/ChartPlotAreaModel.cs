using System;
using Telerik.Core;

namespace Telerik.Charting
{
    /// <summary>
    /// Base class for plot areas in different charts.
    /// </summary>
    internal class ChartPlotAreaModel : Element
    {
        internal ElementCollection<ChartSeriesModel> series;
        internal int dataPointCount;

        internal ChartPlotAreaModel()
        {
            this.series = new ElementCollection<ChartSeriesModel>(this);
        }

        public ElementCollection<ChartSeriesModel> Series
        {
            get
            {
                return this.series;
            }
        }

        internal override ModifyChildrenResult CanAddChild(Node child)
        {
            if (child is ChartSeriesModel)
            {
                return ModifyChildrenResult.Accept;
            }

            return base.CanAddChild(child);
        }

        internal override void OnChildInserted(int index, Node child)
        {
            base.OnChildInserted(index, child);

            if (this.root != null)
            {
                this.GetChartArea().Invalidate(ChartAreaInvalidateFlags.All);
            }
        }

        internal override void OnChildRemoved(int index, Node child)
        {
            base.OnChildRemoved(index, child);

            if (this.root != null)
            {
                this.GetChartArea().Invalidate(ChartAreaInvalidateFlags.All);
            }
        }

        internal override RadRect ArrangeOverride(RadRect rect)
        {
            // NOTE: When calculating the renderable points of each series, we will need the plot area clip, which depends on the layout slot of the plot area
            this.layoutSlot = rect;

            // track the total count of all data points so that we can display NoData message if appropriate
            this.dataPointCount = 0;

            // arrange series and indicators
            foreach (ChartSeriesModel seriesModel in this.series)
            {
                if (seriesModel.presenter.IsVisible)
                {
                    this.dataPointCount += seriesModel.DataPointsInternal.Count;
                    seriesModel.Arrange(rect);
                }
            }

            return rect;
        }
    }
}
