namespace Telerik.UI.Xaml.Controls.Data.ListView
{
    /// <summary>
    /// Defines the pull to refresh mode that will be used.
    /// </summary>
    public enum PullToRefreshScrollMode
    {
        /// <summary>
        /// Standard pull to refresh - drag both content and indicator.
        /// </summary>
        ContentAndIndicator,

        /// <summary>
        /// Pull to refresh is performed with partially dragging the content and then dragging the indicator only.
        /// </summary>
        IndicatorOnly
    }
}
