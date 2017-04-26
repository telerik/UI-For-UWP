using System;

namespace Telerik.Data.Core.Layouts
{
    internal class StackedItemsLayoutStrategy : ItemsLayoutStrategy
    {
        public int StackCount { get; set; }

        public override int CalculateAppendedSlotsCount(BaseLayout layout, int startSlot, ref int totalLines)
        {
            if (layout == null)
            {
                return startSlot;
            }

            int levels = layout.GroupLevels;
            int slotsCount = startSlot;

            bool shouldIndexItem = false;
            foreach (var item in layout.ItemsSource)
            {
                int childrenCount = layout.CountAndPopulateTables(item, slotsCount, 0, levels, null, shouldIndexItem, null, ref totalLines);
                shouldIndexItem = shouldIndexItem || childrenCount > 1;
                slotsCount += childrenCount;
            }

            if (layout.GroupLevels == 0)
            {
                slotsCount = (int)Math.Ceiling(layout.ItemsSource.Count / (double)this.StackCount);
                totalLines = slotsCount;
            }

            return slotsCount;
        }

        public override void OnItemAdded(AddRemoveLayoutResult layoutResult)
        {
            // throw new NotImplementedException();
        }

        public override void OnItemRemoved(AddRemoveLayoutResult layoutResult)
        {
            // throw new NotImplementedException();
        }
    }
}
