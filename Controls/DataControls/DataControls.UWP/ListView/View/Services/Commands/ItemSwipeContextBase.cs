namespace Telerik.UI.Xaml.Controls.Data.ListView.Commands
{
    /// <summary>
    /// Represents an ItemSwipeContextBase class.
    /// </summary>
    public class ItemSwipeContextBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemSwipeContextBase"/> class.
        /// </summary>
        public ItemSwipeContextBase(object dataItem, RadListViewItem container, double dragDelta)
        {
            this.Item = dataItem;
            this.Container = container;
            this.DragDelta = dragDelta;
        }

        /// <summary>
        /// Gets or sets the data item that corresponds to the <see cref="RadListViewItem"/> that is being interacted with.
        /// </summary>
        public object Item { get; set; }

        /// <summary>
        /// Gets the distance the item has been dragged.
        /// </summary>
        public double DragDelta { get; private set; }

        internal RadListViewItem Container { get; set; }
    }
}
