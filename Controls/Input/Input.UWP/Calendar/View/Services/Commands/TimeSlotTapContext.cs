using System;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    /// <summary>
    /// A class that represents the tapped slot.
    /// </summary>
    public class TimeSlotTapContext
    {
        private DateTime startTime;
        private DateTime endTime;
        private DateTime exactStartTime;
        private DateTime exactEndTime;
        private bool isReadOnly;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeSlotTapContext"/> class.
        /// </summary>
        public TimeSlotTapContext(DateTime startTime, DateTime endTime, DateTime exactStartTime, DateTime exactEndTime, bool isReadOnly)
        {
            this.startTime = startTime;
            this.endTime = endTime;
            this.exactStartTime = exactStartTime;
            this.exactEndTime = exactEndTime;
            this.isReadOnly = isReadOnly;
        }

        /// <summary>
        /// Gets the start <see cref="DateTime"/> of the tapped slot.
        /// </summary>
        public DateTime StartTime
        {
            get
            {
                return this.startTime;
            }
        }

        /// <summary>
        /// Gets the end <see cref="DateTime"/> of the tapped slot.
        /// </summary>
        public DateTime EndTime
        {
            get
            {
                return this.endTime;
            }
        }

        /// <summary>
        /// Gets the exact start <see cref="DateTime"/> of the tapped slot.
        /// </summary>
        public DateTime ExactStartTime
        {
            get
            {
                return this.exactStartTime;
            }
        }

        /// <summary>
        /// Gets the exact end <see cref="DateTime"/> of the tapped slot.
        /// </summary>
        public DateTime ExactEndTime
        {
            get
            {
                return this.exactEndTime;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the tapped slot is read only.
        /// </summary>
        /// <value>
        /// True if this slot is read only; otherwise, False.
        /// </value>
        public bool IsReadOnly
        {
            get
            {
                return this.isReadOnly;
            }
        }
    }
}
