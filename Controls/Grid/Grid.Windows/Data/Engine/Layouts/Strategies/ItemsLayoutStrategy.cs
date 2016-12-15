using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.Data.Core.Layouts
{
    internal class ItemsLayoutStrategy : LayoutStrategyBase
    {
        private const int DeafultExecutionOrder = 0;

        private Range<bool> slotsToBuild;

        public ItemsLayoutStrategy()
            : base(DeafultExecutionOrder)
        {
        }

        public override IList<ItemInfo> BuildItemInfos(BaseLayout layout, int visibleLine, int slot)
        {
            var compactLayout = layout as CompactLayout;

            if (slot >= 0 && slot < layout.TotalLineCount && visibleLine < layout.VisibleLineCount && compactLayout != null &&
                slot < this.slotsToBuild.UpperBound && slot >= this.slotsToBuild.LowerBound)
            {
                return compactLayout.GetItemInfosAtSlot(visibleLine, slot);
            }

            return new List<ItemInfo>();
        }

        public override int CalculateAppendedSlotsCount(BaseLayout layout, int startSlot)
        {
            var compactLayout = layout as CompactLayout;

            if (compactLayout == null)
            {
                this.slotsToBuild = new Range<bool>(startSlot, startSlot, true);
                return startSlot;
            }

            int levels = compactLayout.GroupLevels;
            int slotsCount = startSlot;

            bool shouldIndexItem = false;
            foreach (var item in compactLayout.ItemsSource)
            {
                int childrenCount = compactLayout.CountAndPopulateTables(item, slotsCount, 0, levels, null, shouldIndexItem, null);
                shouldIndexItem = shouldIndexItem || childrenCount > 1;
                slotsCount += childrenCount;
            }

            if (compactLayout.GroupLevels == 0)
            {
                slotsCount = compactLayout.ItemsSource.Count;
            }

            this.slotsToBuild = new Range<bool>(startSlot, slotsCount, true);

            return slotsCount;
        }

        public override void OnItemAdded(AddRemoveLayoutResult layoutResult)
        {
            var start = Math.Min(this.slotsToBuild.LowerBound, layoutResult.StartSlot);
            this.slotsToBuild = new Range<bool>(start, this.slotsToBuild.UpperBound + layoutResult.SlotsCount, true);
        }

        public override void OnItemRemoved(AddRemoveLayoutResult layoutResult)
        {
        }
    }
}
