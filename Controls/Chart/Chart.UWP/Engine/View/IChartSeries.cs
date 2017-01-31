using System;

namespace Telerik.Charting
{
    /// <summary>
    /// Identifies a special <see cref="IChartElementPresenter"/> that visualizes <see cref="DataPoint"/> instances.
    /// </summary>
    public interface IChartSeries : IChartElementPresenter
    {
        /// <summary>
        /// Occurs when a <see cref="DataPoint"/> owned by the series has its IsSelected property changed.
        /// </summary>
        void OnDataPointIsSelectedChanged(DataPoint point);
    }
}
