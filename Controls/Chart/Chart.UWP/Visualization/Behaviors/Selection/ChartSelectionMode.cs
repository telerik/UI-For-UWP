using System;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Specifies the possible modes used by a <see cref="ChartSelectionBehavior"/> to update the current selection within a <see cref="RadChartBase"/> instance.
    /// </summary>
    public enum ChartSelectionMode
    {
        /// <summary>
        /// No selection is performed.
        /// </summary>
        None,

        /// <summary>
        /// One DataPoint/ChartSeries may be selected at a time.
        /// </summary>
        Single,

        /// <summary>
        /// Multiple DataPoint/ChartSeries may be selected at a time.
        /// </summary>
        Multiple
    }
}
