namespace Telerik.Data.Core.Layouts
{
    internal class LayoutRenderInfoState : IRenderInfoState
    {
        private IndexToValueTable<bool> collapsedSlots;

        public LayoutRenderInfoState(IndexToValueTable<bool> collapsedSlots)
        {
            this.collapsedSlots = collapsedSlots;
        }

        public double? GetValueAt(int index)
        {
            if (this.collapsedSlots.GetValueAt(index))
            {
                return 0;
            }
            return null;
        }
    }
}
