using System;

namespace Telerik.Data.Core.Layouts
{
    internal class ExpandCollapseEventArgs : EventArgs
    {
        internal ExpandCollapseEventArgs(object item, int layoutSlot, int slotsCount)
        {
            this.Item = item;
            this.StartSlot = layoutSlot;
            this.SlotsCount = slotsCount;
        }

        public object Item { get; private set; }
        public int StartSlot { get; private set; }
        public int SlotsCount { get; private set; }
    }
}