using System;

namespace Telerik.UI.Xaml.Controls.Input
{
    [Flags]
    public enum SegmentVisualState
    {
        Normal = 1,
        PointerOver = 2,
        Pressed = 4,
        Selected = 8,
        Disabled = 16,
        SelectedPointerOver = Selected | PointerOver,
        SelectedPressed = Selected | Pressed,
        SelectedDisabled = Selected | Disabled
    }
}