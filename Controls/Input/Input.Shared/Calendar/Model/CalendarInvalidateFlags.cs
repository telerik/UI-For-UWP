using System;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    [Flags]
    internal enum CalendarInvalidateFlags
    {
        None = 0,
        InvalidateContent = 1,
        InvalidateDecoration = InvalidateContent << 1,
        All = InvalidateContent | InvalidateDecoration
    }
}
