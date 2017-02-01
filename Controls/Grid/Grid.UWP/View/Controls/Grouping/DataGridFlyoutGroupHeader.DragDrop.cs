using System;
using System.Linq;
using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Primitives.DragDrop;
using Telerik.UI.Xaml.Controls.Primitives.DragDrop.Reorder;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    public partial class DataGridFlyoutGroupHeader 
    {
        internal override bool CanStartDrag(DragDropTrigger trigger, object initializeContext = null)
        {
            return this.ParentGrid.DragBehavior != null && this.ParentGrid.DragBehavior.CanStartReorder(this.DataContext as GroupDescriptorBase);
        }

        internal override bool CanReorder(object sourceDescriptor, object descriptor)
        {
            return this.ParentGrid.DragBehavior.CanReorder(sourceDescriptor as GroupDescriptorBase, descriptor as GroupDescriptorBase);
        }
    }
}