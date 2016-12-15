using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data.ListView.Commands
{
    /// <summary>
    /// The context that is passed as a parameter to the <see cref="ItemSwipingCommand"/>.
    /// </summary>
    public class ItemSwipingContext : ItemSwipeContextBase
    {
        public ItemSwipingContext(object dataItem, RadListViewItem container, double dragDelta) : base(dataItem, container, dragDelta)
        {
        }
    }
}
