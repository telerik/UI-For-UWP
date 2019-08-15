using System;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    /// <summary>
    /// Event arguments for RadCalendar's DisplayDateChanged event.
    /// </summary>
    public class DisplayDateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayDateChangedEventArgs"/> class.
        /// </summary>
        public DisplayDateChangedEventArgs(DateTime oldValue, DateTime newValue)
        {
            this.OldDisplayDate = oldValue;
            this.NewDisplayDate = newValue;
        }

        /// <summary>
        /// Gets the old DisplayDate value of the Calendar.
        /// </summary>
        public DateTime OldDisplayDate { get; private set; }

        /// <summary>
        /// Gets the new DisplayDate value of the Calendar.
        /// </summary>
        public DateTime NewDisplayDate { get; private set; }
    }
}
