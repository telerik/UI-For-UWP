using System;
using System.Linq;
using Telerik.UI.Xaml.Controls.Data.ListView;
using Telerik.UI.Xaml.Controls.Primitives.DragDrop.Reorder;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data
{
    internal class ListViewReorderItemsCoordinator : ReorderItemsCoordinator
    {
        public ListViewReorderItemsCoordinator(IReorderItemsHost host)
            : base(host)
        {
        }

        protected override void UpdatePositionAndIndices(IReorderItem source, IReorderItem destination)
        {
            var listView = (RadListView)this.Host;

            if (listView.GroupDescriptors.Count == 0 || listView.LayoutDefinition is StackLayoutDefinition)
            {
                base.UpdatePositionAndIndices(source, destination);
                return;
            }

            var sourceIndex = source.LogicalIndex;
            var destinationIndex = destination.LogicalIndex;
            var childrenPanel = listView.childrenPanel;
            var reorderItems = childrenPanel.Children.OfType<IReorderItem>()
                .OrderBy(reorderItem => reorderItem.LogicalIndex).ToArray();
            var firstItem = reorderItems[0];
            var layoutItem = (IArrangeChild)firstItem;
            var layoutSlot = layoutItem.LayoutSlot;
            var arrangePosition = firstItem.ArrangePosition;

            for (int logicalIndex = 0; logicalIndex < reorderItems.Length; logicalIndex++)
            {
                IReorderItem reorderItem;

                if (logicalIndex == destinationIndex)
                {
                    reorderItem = reorderItems[sourceIndex];
                }
                else if (logicalIndex >= sourceIndex && logicalIndex < destinationIndex)
                {
                    reorderItem = reorderItems[logicalIndex + 1];
                }
                else if (logicalIndex > destinationIndex && logicalIndex <= sourceIndex)
                {
                    reorderItem = reorderItems[logicalIndex - 1];
                }
                else
                {
                    reorderItem = reorderItems[logicalIndex];
                }

                reorderItem.LogicalIndex = logicalIndex;

                var actualSize = reorderItem.ActualSize;

                if (listView.Orientation == Orientation.Vertical)
                {
                    if (arrangePosition.X + actualSize.Width > childrenPanel.ActualWidth)
                    {
                        arrangePosition.X = 0;
                        arrangePosition.Y += layoutSlot.Height;
                    }
                }
                else
                {
                    if (arrangePosition.Y + actualSize.Height > childrenPanel.ActualHeight)
                    {
                        arrangePosition.Y = 0;
                        arrangePosition.X += layoutSlot.Width;
                    }
                }

                if (arrangePosition != reorderItem.ArrangePosition)
                {
                    this.AnimateItem(source, reorderItem, arrangePosition);
                }

                if (listView.Orientation == Orientation.Vertical)
                {
                    arrangePosition.X += actualSize.Width;
                }
                else
                {
                    arrangePosition.Y += actualSize.Height;
                }

                layoutItem = (IArrangeChild)reorderItem;
                layoutSlot = layoutItem.LayoutSlot;
            }
        }
    }
}
