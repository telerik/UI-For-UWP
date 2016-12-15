using System;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Primitives.DragDrop
{
    internal interface IDragDropOperation
    {
        IDragDropElement Source { get; }

        Point RelativeStartPosition { get; set; }

        Point GetDragVisualPosition(UIElement targetElement);

        Rect GetDragVisualBounds(UIElement targetElement);

        void EndDrag();
    }
}