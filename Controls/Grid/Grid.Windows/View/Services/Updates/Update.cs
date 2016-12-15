using System;
using Windows.UI.Core;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal class Update
    {
        public UpdateFlags Flags;
        public object Context;
        public object Sender;
        public CoreDispatcherPriority Priority = CoreDispatcherPriority.Low;
        public bool RequiresValidMeasure;
        public int ScheduleCount; // This is used to prevent potential endless loop upon waiting for valid measure.

        internal virtual void Process()
        {
        }
    }
}
