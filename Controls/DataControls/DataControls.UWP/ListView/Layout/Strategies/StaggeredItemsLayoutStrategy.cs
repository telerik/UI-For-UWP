namespace Telerik.Data.Core.Layouts
{
    internal class StaggeredItemsLayoutStrategy : StackedItemsLayoutStrategy
    {
        public override int CalculateAppendedSlotsCount(BaseLayout layout, int startSlot, ref int totalLines)
        {
            int slot = startSlot;
            if (layout.ItemsSource != null)
            {
                slot = layout.ItemsSource.Count;
            }

            totalLines = slot;
            return slot;
        }
    }
}
