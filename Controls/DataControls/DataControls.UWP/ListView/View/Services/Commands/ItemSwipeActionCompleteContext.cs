using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data.ListView.Commands
{
    /// <summary>
    /// The context that is passed as a parameter to the <see cref="ItemSwipeActionCompleteCommand"/>.
    /// </summary>
    public class ItemSwipeActionCompleteContext : ItemSwipeContextBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemSwipeActionCompleteContext"/> class.
        /// </summary>
        public ItemSwipeActionCompleteContext(object dataItem, RadListViewItem container, double dragDelta) : base(dataItem, container, dragDelta)
        {
            this.FinalDragOffset = this.GetOffset(container.ListView.Orientation, dragDelta, container.ListView.ItemSwipeOffset);
        }

        /// <summary>
        /// Gets or sets the final drag offset that the item will be positioned when the operation complete.
        /// </summary>
        public double FinalDragOffset { get; set; }

        private double GetOffset(Orientation orientation, double dragDelta, Thickness thickness)
        {
            if (orientation == Orientation.Vertical)
            {
                return dragDelta >= 0 ? thickness.Left : -thickness.Right;
            }
            else
            {
                return dragDelta >= 0 ? thickness.Top : -thickness.Bottom;
            }
        }
    }
}
