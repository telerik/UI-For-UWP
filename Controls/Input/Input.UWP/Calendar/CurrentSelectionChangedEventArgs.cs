using System;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    /// <summary>
    /// Event arguments for RadCalendar's SelectionChanged event.
    /// </summary>
    public class CurrentSelectionChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the newly selected DataTime value.
        /// </summary>
        public DateTime? NewSelection { get; set; }
    }
}
