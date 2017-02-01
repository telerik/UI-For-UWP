using System;
using System.Collections.Generic;
using Telerik.Core;

namespace Telerik.Charting
{
    /// <summary>
    /// Base non-generic class used for non-generic declarations.
    /// </summary>
    internal abstract class ChartSeriesModel : Element
    {
        internal static readonly int DataPointsModifiedMessageKey = Message.Register();

        internal bool canModifyDataPoints;
        internal bool isDataBound;
        internal List<DataPoint> renderablePoints = new List<DataPoint>(32);

        internal abstract IList<DataPoint> DataPointsInternal { get; }

        /// <summary>
        /// Gets the default <see cref="AxisPlotMode"/> for this series.
        /// </summary>
        internal virtual AxisPlotMode DefaultPlotMode
        {
            get
            {
                return AxisPlotMode.OnTicks;
            }
        }

        internal static AxisPlotMode SelectPlotMode(IList<ChartSeriesModel> series)
        {
            bool isAnyBetweenTicks = false;
            for (int i = 0; i < series.Count; i++)
            {
                var currentSeries = series[i];
                if (currentSeries.DefaultPlotMode == AxisPlotMode.OnTicksPadded)
                {
                    return AxisPlotMode.OnTicksPadded;
                }
                isAnyBetweenTicks |= currentSeries.DefaultPlotMode == AxisPlotMode.BetweenTicks;
            }

            if (isAnyBetweenTicks)
            {
                return AxisPlotMode.BetweenTicks;
            }

            return AxisPlotMode.OnTicks;
        }

        internal virtual bool GetIsPlotInverse(AxisPlotDirection plotDirection)
        {
            return false;
        }

        /// <summary>
        /// Gets the strategy that will be used when series of this type are combined - for example Stacked - on the plot area.
        /// </summary>
        internal virtual CombinedSeriesPlotStrategy GetCombinedPlotStrategy()
        {
            return null;
        }

        /// <summary>
        /// Gets the strategy that will apply layout rounding for combined series of this type.
        /// </summary>
        internal virtual CombinedSeriesRoundLayoutStrategy GetCombinedRoundLayoutStrategy()
        {
            return null;
        }

        internal override void OnChildInserted(int index, Node child)
        {
            base.OnChildInserted(index, child);

            this.OnDataPointsModified();
        }

        internal override void OnChildRemoved(int index, Node child)
        {
            base.OnChildRemoved(index, child);

            this.OnDataPointsModified();
        }

        internal virtual void OnDataPointsModified()
        {
            if (this.isDataBound && !this.canModifyDataPoints)
            {
                throw new InvalidOperationException("Cannot modify the data points that belong to a databound ChartSeries instance.");
            }

            if (this.invalidateScheduled || !this.IsTreeLoaded)
            {
                return;
            }

            this.Invalidate();

            Message message = new Message(DataPointsModifiedMessageKey, null, MessageDispatchMode.Bubble);
            this.DispatchMessage(message);
        }

        internal virtual RadRect GetZoomedRect(RadRect proposed)
        {
            var chartArea = this.GetChartArea();

            proposed.Width *= chartArea.view.ZoomWidth;
            proposed.Height *= chartArea.view.ZoomHeight;

            return proposed;
        }
    }
}