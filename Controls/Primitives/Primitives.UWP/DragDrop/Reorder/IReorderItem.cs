using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Primitives.DragDrop.Reorder
{
    internal interface IReorderItem
    {
        DependencyObject Visual { get; }
        int LogicalIndex { get; set; }
        Point ArrangePosition { get; set; }
        Size ActualSize { get; }
        ReorderItemsCoordinator Coordinator { get; set; }
    }
}
