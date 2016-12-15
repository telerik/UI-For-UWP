namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Defines the different asynchronous balance modes.
    /// </summary>
    public enum AsyncBalanceMode
    {
        /// <summary>
        /// Performs a standard async balance operation. In this mode a visual item is realized
        /// at once by the UI virtualization logic after which the UI thread is offloaded.
        /// </summary>
        Standard,

        /// <summary>
        /// The viewport of the <see cref="RadDataBoundListBox"/> control is filled with visual containers synchronously
        /// after which an async balance for the rest of the items is performed.
        /// </summary>
        FillViewportFirst
    }
}
