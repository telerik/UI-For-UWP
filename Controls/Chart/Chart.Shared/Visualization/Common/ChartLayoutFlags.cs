using System;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal enum ChartLayoutFlags
    {
        None = 0,
        Size = 1,
        Zoom = Size << 1,
        Pan = Zoom << 1,
        LayoutInvalid = Size | Zoom,
        All = Size | Pan | Zoom
    }
}
