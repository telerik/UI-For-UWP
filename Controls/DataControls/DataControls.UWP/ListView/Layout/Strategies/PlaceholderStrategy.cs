using System.Collections.Generic;

namespace Telerik.Data.Core.Layouts
{
    internal class PlaceholderStrategy : LayoutStrategyBase
    {
        public PlaceholderStrategy(int executionOrder) : this(executionOrder, PlaceholderInfoType.None)
        {
        }

        public PlaceholderStrategy(int executionOrder, PlaceholderInfoType type) : base(executionOrder)
        {
            this.Placeholder = new PlaceholderInfo(type);
        }

        public PlaceholderInfo Placeholder { get; private set; }

        public override IList<ItemInfo> BuildItemInfos(BaseLayout layout, int visibleLine, int slot)
        {
            return new List<ItemInfo>();
        }

        public override int CalculateAppendedSlotsCount(BaseLayout layout, int startSlot, ref int totalLines)
        {
            return 1;
        }

        public override void OnItemAdded(AddRemoveLayoutResult layoutResult)
        {
        }

        public override void OnItemRemoved(AddRemoveLayoutResult layoutResult)
        {
        }

        protected virtual IList<ItemInfo> GetPlaceholderSlot(BaseLayout layout, int visibleLine, bool isDisplayed = true)
        {
            var itemInfo = new ItemInfo();

            itemInfo.IsDisplayed = isDisplayed;

            itemInfo.Item = this.Placeholder;
            itemInfo.Level = 0;
            itemInfo.Id = layout.TotalSlotCount - 1;
            itemInfo.Slot = itemInfo.Id;

            itemInfo.IsCollapsible = false;
            itemInfo.IsCollapsed = false;
            itemInfo.ItemType = BaseLayout.GetItemType(itemInfo.Item);
            itemInfo.IsSummaryVisible = false;

            return new List<ItemInfo> { itemInfo };
        }
    }
}