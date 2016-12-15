using System;
using System.Linq;
using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Primitives.DragDrop;
using Telerik.UI.Xaml.Controls.Primitives.DragDrop.Reorder;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Represents the UI on the left edge of a <see cref="RadDataGrid"/> instance. Used to access the Grouping Flyout.
    /// </summary>
    public partial class DataGridColumnsFlyout : IDragDropElement 
    {
        internal override void SetupDragDropProperties(IReorderItem item, int logicalIndex)
        {
            base.SetupDragDropProperties(item, logicalIndex);
            if (this.Owner.Owner.Columns.Count > 1)
            {
                DragDrop.SetAllowDrag(item.Visual, true);
            }
        }

        internal override void CommitReorderOperation(int sourceIndex, int destinationIndex)
        {
            this.Owner.Owner.DragBehavior.ReorderColumn(sourceIndex, destinationIndex);
        }

        internal override bool CanStartDrag(DragDropTrigger trigger, object initializeContext)
        {
            return this.Owner.Owner.DragBehavior != null && this.Owner.Owner.DragBehavior.CanStartDragInFlyout(this.DataContext as DataGridColumn);
        }
    }
}
