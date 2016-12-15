namespace Telerik.Data.Core.Layouts
{
    internal struct AddRemoveLayoutResult
    {
        internal AddRemoveLayoutResult(int layoutSlot, int slotsCount)
            : this()
        {
            this.StartSlot = layoutSlot;
            this.SlotsCount = slotsCount;
        }

        public int StartSlot { get; private set; }
        public int SlotsCount { get; private set; }
    }
}