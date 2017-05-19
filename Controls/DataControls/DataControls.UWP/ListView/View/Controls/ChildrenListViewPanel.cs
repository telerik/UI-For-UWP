using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data.ListView
{
    /// <summary>
    /// The panel that holds the elements in the <see cref="RadListView"/> control.
    /// </summary>
    public partial class ChildrenListViewPanel : Canvas
    {
        /// <inheritdoc/>
        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (var item in this.Children)
            {
                var arrangeRect = new Windows.Foundation.Rect(0, 0, item.DesiredSize.Width, item.DesiredSize.Height);

                var child = item as IArrangeChild;
                if (child != null)
                {
                    child.TryInvalidateOwner();
                    arrangeRect = child.LayoutSlot;
                }

                item.Arrange(arrangeRect);
            }

            return finalSize;
        }
    }
}
