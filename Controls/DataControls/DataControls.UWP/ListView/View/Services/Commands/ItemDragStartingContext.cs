namespace Telerik.UI.Xaml.Controls.Data.ListView.Commands
{
    /// <summary>
    /// Represents the context that is passed as a parameter of a <see cref="ItemDragStartingCommand"/>.
    /// </summary>
    public class ItemDragStartingContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemDragStartingContext"/> class.
        /// </summary>
        public ItemDragStartingContext(object dataItem, DragAction itemAction)
        {
            this.Item = dataItem;
            this.Action = itemAction;
            this.FinalDragOffset = 40;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemDragStartingContext"/> class.
        /// </summary>
        public ItemDragStartingContext(object dataItem, DragAction itemAction, RadListViewItem container)
            : this(dataItem, itemAction)
        {
            this.Container = container;
        }

        /// <summary>
        /// Gets or sets the item that is being dragged.
        /// </summary>
        public object Item { get; set; }

        /// <summary>
        /// Gets or sets the drag action type.
        /// </summary>
        public DragAction Action { get; set; }

        /// <summary>
        /// Gets or sets the maximum drag offset that item will set to after when the operation finish successfully.
        /// </summary>
        /// <value>
        /// The maximum drag offset. Can be negative value.
        /// </value>
        public double FinalDragOffset { get; set; }

        internal RadListViewItem Container { get; set; }
    }
}