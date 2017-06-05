namespace Telerik.UI.Xaml.Controls.Data.ListView.Commands
{
    /// <summary>
    /// The context that is passed as a parameter to the <see cref="ItemReorderCompleteCommand"/>.
    /// </summary>
    public class ItemReorderCompleteContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemReorderCompleteContext"/> class.
        /// </summary>
        public ItemReorderCompleteContext(object dataItem, object destinationItem, RadListViewItem container)
        {
            this.Item = dataItem;
            this.DestinationItem = destinationItem;
            this.Container = container;
        }

        /// <summary>
        /// Gets or sets the data item that corresponds to the <see cref="RadListViewItem"/> that is being interacted with.
        /// </summary>
        public object Item { get; set; }

        /// <summary>
        /// Gets or sets the data item that corresponds to the location where the dragged item has been released.
        /// </summary>
        public object DestinationItem { get; set; }

        internal RadListViewItem Container { get; set; }
    }
}