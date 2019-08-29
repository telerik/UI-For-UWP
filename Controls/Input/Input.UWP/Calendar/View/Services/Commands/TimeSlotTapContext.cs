using System;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    public class TimeSlotTapContext
    {
        private DateTime startTime;
        private DateTime endTime;
        private DateTime exactStartTime;
        private DateTime exactEndTime;
        private bool isReadOnly;

        public TimeSlotTapContext(DateTime startTime, DateTime endTime, DateTime exactStartTime, DateTime exactEndTime, bool isReadOnly)
        {
            this.startTime = startTime;
            this.endTime = endTime;
            this.exactStartTime = exactStartTime;
            this.exactEndTime = exactEndTime;
            this.isReadOnly = isReadOnly;
        }

        public DateTime StartTime
        {
            get
            {
                return this.startTime;
            }
        }

        public DateTime EndTime
        {
            get
            {
                return this.endTime;
            }
        }

        public DateTime ExactStartTime
        {
            get
            {
                return this.exactStartTime;
            }
        }

        public DateTime ExactEndTime
        {
            get
            {
                return this.exactEndTime;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return this.isReadOnly;
            }
        }
    }
}
