using System;
using System.Globalization;
using Windows.UI.Core;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    internal class Update<T>
    {
        public T Flags;
        public object Context;
        public object Sender;
        public CoreDispatcherPriority Priority = CoreDispatcherPriority.Low;
        public bool RequiresValidMeasure;
        public int ScheduleCount; // This is used to prevent potential endless loop upon waiting for valid measure.

        public virtual int UpdateFlagsIndex
        {
            get
            {
                return Convert.ToInt32(this.Flags, CultureInfo.InvariantCulture);
            }
        }

        internal virtual void Process()
        {
        }
    }
}
