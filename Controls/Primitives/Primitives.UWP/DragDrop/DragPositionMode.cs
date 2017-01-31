using System;

namespace Telerik.UI.Xaml.Controls.Primitives.DragDrop
{
    [Flags]
    internal enum DragPositionMode
    {
        Free = 1,
        RailXForward = 2,
        RailXBackwards = 4,
        RailX = RailXForward | RailXBackwards,
        RailYForward = 8,
        RailYBackwards = 16,
        RailY = RailYForward | RailYBackwards,
    }
}
