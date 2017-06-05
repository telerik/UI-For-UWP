using System.Collections.Generic;

namespace Telerik.Data.Core.Layouts
{
    internal class ItemsLayoutStrategy : LayoutStrategyBase
    {
        private const int DefaultExecutionOrder = 0;

        public ItemsLayoutStrategy()
            : base(DefaultExecutionOrder)
        {
        }

        public override IList<ItemInfo> BuildItemInfos(BaseLayout layout, int visibleLine, int slot)
        {
            if (slot >= 0 && slot < layout.TotalSlotCount && visibleLine < layout.VisibleLineCount)
            {
                return layout.GetItemInfosAtSlot(visibleLine, slot);
            }

            return new List<ItemInfo>();
        }

        public override int CalculateAppendedSlotsCount(BaseLayout layout, int startSlot, ref int totalLines)
        {
            if (layout == null || layout.ItemsSource == null)
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
                slotsCount = layout.ItemsSource.Count;
                totalLines = layout.CalculateFlatRowCount();
            }

            return slotsCount;
        }

        public override void OnItemAdded(AddRemoveLayoutResult layoutResult)
        {
        }

        public override void OnItemRemoved(AddRemoveLayoutResult layoutResult)
        {
        }
    }
}
