using System;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    /// <summary>
    /// A class that represents the time area that build the timer ruler part of the day view.
    /// </summary>
    public class CalendarTimeRulerItem : CalendarNode
    {
        /// <summary>
        /// Gets the start time of the item - from where the slots starts.
        /// </summary>
        public TimeSpan StartTime
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the end time of the item - where the slots ends and where the next one starts.
        /// </summary>
        public TimeSpan EndTime
        {
            get;
            internal set;
        }
    }
}
