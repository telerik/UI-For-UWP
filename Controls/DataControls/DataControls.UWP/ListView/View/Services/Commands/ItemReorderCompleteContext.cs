using Telerik.Data.Core;

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
        public ItemReorderCompleteContext(object dataItem, object destinationItem, ItemReorderPlacement placement)
            : this(dataItem, null, destinationItem, null, placement)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemReorderCompleteContext"/> class.
        /// </summary>
        public ItemReorderCompleteContext(object dataItem, IDataGroup dataGroup, object destinationItem, IDataGroup destinationGroup, ItemReorderPlacement placement)
        {
            this.Item = dataItem;
            this.Group = dataGroup;
            this.DestinationItem = destinationItem;
            this.DestinationGroup = destinationGroup;
            this.Placement = placement;
        }

        /// <summary>
        /// Gets or sets the data item that corresponds to the <see cref="RadListViewItem"/> that is being interacted with.
        /// </summary>
        public object Item { get; set; }

        /// <summary>
        /// Gets or sets the data group that corresponds to the <see cref="RadListViewItem"/> that is being interacted with.
        /// </summary>
        public IDataGroup Group { get; set; }

        /// <summary>
        /// Gets or sets the data item that corresponds to the location where the dragged item has been released.
        /// </summary>
        public object DestinationItem { get; set; }

        /// <summary>
        /// Gets or sets the data group that corresponds to the location where the dragged item has been released.
        /// </summary>
        public IDataGroup DestinationGroup { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the dragged item should be placed before or after the destination item.
        /// </summary>
        public ItemReorderPlacement Placement { get; set; }
    }
}