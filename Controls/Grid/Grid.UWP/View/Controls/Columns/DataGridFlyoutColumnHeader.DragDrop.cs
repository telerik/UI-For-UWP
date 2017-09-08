using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Primitives.Common;
using Telerik.UI.Xaml.Controls.Primitives.DragDrop;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    public partial class DataGridFlyoutColumnHeader 
    {
        internal override bool CanStartDrag(DragDropTrigger trigger, object initializeContext = null)
        {
            return this.ParentGrid.DragBehavior != null && this.ParentGrid.DragBehavior.CanStartReorder(this.DataContext as DataGridColumn);
        }

        internal override bool CanReorder(object sourceColumn, object column)
        {
            return this.ParentGrid.DragBehavior.CanReorder(sourceColumn as DataGridColumn, column as DataGridColumn);
        }
    }
}
