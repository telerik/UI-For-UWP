using System;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    public class DisplayModeChangedEventArgs : EventArgs
    {
        public DisplayModeChangedEventArgs(CalendarDisplayMode newDisplayMode)
        {
            this.NewDisplayMode = newDisplayMode;
        }

        public CalendarDisplayMode NewDisplayMode { get; set; }
    }
}
