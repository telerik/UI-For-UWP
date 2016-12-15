using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.Data.Core.Layouts
{
    internal class PlaceholderStrategy : LayoutStrategyBase
    {
        private const int DeafultExecutionOrder = 1;

        private static PlaceholderInfo placeholderInfo = new PlaceholderInfo(PlaceholderInfoType.None);

        public PlaceholderStrategy()
            : base(DeafultExecutionOrder)
        {
        }

        public override IList<ItemInfo> BuildItemInfos(BaseLayout layout, int visibleLine, int slot)
        {
            var compactLayout = layout as CompactLayout;
            if (visibleLine == layout.VisibleLineCount - 1 && compactLayout != null)
            {
                return PlaceholderStrategy.GetPlaceholderSlot(compactLayout, visibleLine);
            }

            return new List<ItemInfo>();
        }

        public override int CalculateAppendedSlotsCount(BaseLayout layout, int startSlot)
        {
            return 1;
        }

        public override void OnItemAdded(AddRemoveLayoutResult layoutResult)
        {
        }

        public override void OnItemRemoved(AddRemoveLayoutResult layoutResult)
        {
        }

        private static IList<ItemInfo> GetPlaceholderSlot(CompactLayout layout, int visibleLine)
        {
            var itemInfo = new ItemInfo();

            itemInfo.IsDisplayed = true;

            itemInfo.Item = placeholderInfo;
            itemInfo.Level = 0;
            itemInfo.Id = layout.TotalLineCount - 1;
            itemInfo.Slot = itemInfo.Id;

            itemInfo.IsCollapsible = false;
            itemInfo.IsCollapsed = false;
            itemInfo.ItemType = BaseLayout.GetItemType(itemInfo.Item);
            itemInfo.IsSummaryVisible = false;

            itemInfo.LayoutInfo = layout.GenerateLayoutInfo(itemInfo, null, visibleLine, visibleLine);

            return new List<ItemInfo> { itemInfo };
        }
    }
}
