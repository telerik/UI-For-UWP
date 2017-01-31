using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Primitives.DragDrop
{
    internal interface IDragSurface
    {
        FrameworkElement RootElement { get; }

        DragVisualContext CreateDragContext();

        Rect PositionDragHost(DragVisualContext context, Point dragPoint, Point relativeStartPosition);

        void CompleteDrag(DragVisualContext context, bool dragSuccessful);
    }
}
