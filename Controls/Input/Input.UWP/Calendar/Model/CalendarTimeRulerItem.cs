using System;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
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
