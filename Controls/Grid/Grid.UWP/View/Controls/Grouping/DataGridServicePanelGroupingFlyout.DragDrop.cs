using System;
using System.Linq;
using Telerik.UI.Xaml.Controls.Primitives.DragDrop;
using Telerik.UI.Xaml.Controls.Primitives.DragDrop.Reorder;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    public partial class DataGridServicePanelGroupingFlyout : IReorderItemsHost
    {
        void IReorderItemsHost.OnItemsReordered(IReorderItem sourceItem, IReorderItem destinationItem)
        {
            if (sourceItem == null || destinationItem == null)
            {
                return;
            }

            var childrenCount = this.Container.Children.Count;
            var sourceVisual = sourceItem.Visual as DataGridFlyoutGroupHeader;
            var destinationVisual = destinationItem.Visual as DataGridFlyoutGroupHeader;

            sourceVisual.BottomGlyphOpacity = sourceItem.LogicalIndex == childrenCount - 1 ? 0.0 : 1;
            destinationVisual.BottomGlyphOpacity = destinationItem.LogicalIndex == childrenCount - 1 ? 0.0 : 1;
        }
        internal override void SetupDragDropProperties(IReorderItem item, int logicalIndex)
        {
            base.SetupDragDropProperties(item, logicalIndex);
            if (this.Owner.Owner.GroupDescriptors.Count > 1)
            {
                DragDrop.SetAllowDrag(item.Visual, true);
            }
        }

        internal override void CommitReorderOperation(int sourceIndex, int destinationIndex)
        {
            this.Owner.Owner.DragBehavior.ReorderGroupDescriptor(sourceIndex, destinationIndex);
        }
    }
}