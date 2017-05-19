namespace Telerik.UI.Xaml.Controls.Data.ListView.Commands
{
    /// <summary>
    /// The context that is passed as a parameter to the <see cref="ItemSwipingCommand"/>.
    /// </summary>
    public class ItemSwipingContext : ItemSwipeContextBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemSwipingContext"/> class.
        /// </summary>
        public ItemSwipingContext(object dataItem, RadListViewItem container, double dragDelta) : base(dataItem, container, dragDelta)
        {
        }
    }
}
