using Telerik.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal partial class PieSegment
    {
        private LegendItem legendItem = new LegendItem();
        private bool visibleInLegend = true;

        internal PieSeries ParentSeries
        {
            get;
            set;
        }

        internal LegendItem LegendItem
        {
            get
            {
                return this.legendItem;
            }
        }

        internal void UpdateLegendItem()
        {
            this.legendItem.Fill = this.Path.Fill;
            this.legendItem.Stroke = this.Path.Stroke;
        }

        internal void UpdateLegendTitle(DataPointBinding legendTitleBinding)
        {
            object data = null;

            if (legendTitleBinding != null && this.point.dataItem != null)
            {
                data = legendTitleBinding.GetValue(this.point.dataItem);
            }

            if (data != null)
            {
                this.legendItem.Title = data.ToString();
            }

            if (this.legendItem.Title == null || data == null)
            {
                this.legendItem.Title = this.point.Label.ToString();
            }
        }

        internal void UpdateVisibleInLegend(DataPointBinding visibleInLegendBinding)
        {
            var oldVisibleInLegend = this.visibleInLegend;
            if (visibleInLegendBinding != null && this.point.dataItem != null)
            {
                this.visibleInLegend = (bool)visibleInLegendBinding.GetValue(this.point.dataItem);
            }

            if (oldVisibleInLegend != this.visibleInLegend && this.ParentSeries != null)
            {
                if (this.visibleInLegend)
                {
                    // TODO: Consider better solution for the scenario with runtime rebind of the control (pie segment reuse could be incorrectly interpreted as property change here).
                    // i.e. the data point at index i in the original source did not display in the legend, but the data point at the same index in the new source should be --
                    // as both data points are associated with the same segment -- we will end up with duplicates in the LegendInfos collection unless we add this check condition.
                    if (!this.ParentSeries.LegendInfos.Contains(this.legendItem))
                    {
                        this.ParentSeries.LegendInfos.Add(this.legendItem);
                    }
                }
                else
                {
                    this.ParentSeries.LegendInfos.Remove(this.legendItem);
                }
            }
        }
    }
}
