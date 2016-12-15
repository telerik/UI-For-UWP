using System;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Event arguments for RadChart's DataContextNeeded event.
    /// </summary>
    public class TooltipContextNeededEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the TooltipContextNeededEventArgs class.
        /// </summary>
        /// <param name="defaultContext">The default context which will be used if no new context is provided.</param>
        public TooltipContextNeededEventArgs(ChartDataContext defaultContext)
        {
            this.DefaultContext = defaultContext;
        }

        /// <summary>
        /// Gets the default data context that is provided by RadChart.
        /// </summary>
        public ChartDataContext DefaultContext { get; private set; }

        /// <summary>
        /// Gets or sets user defined object that will be used as the chart data context if
        /// it is not null. The user can create a custom context based on the information
        /// in the default context.
        /// </summary>
        public object Context { get; set; }
    }
}
