using System;
using System.Collections.Generic;
using System.Text;
using Telerik.UI.Xaml.Controls.Data;

namespace Telerik.Data.Core.Layouts
{
    internal class HeaderStrategy : PlaceholderStrategy
    {
        private static int DefaultExecutionOrder = SlotStrategyExecutionOrders.IncrementalLoadingOrder;

        public PlaceholderInfo Placeholder { get; private set; }

        public HeaderStrategy()
            : this(DefaultExecutionOrder, PlaceholderInfoType.Header)
        {
        }

        public HeaderStrategy(int executionOrder, PlaceholderInfoType type)
            : base(DefaultExecutionOrder, type)
        {
            this.Placeholder = new PlaceholderInfo(type);
        }

        public override IList<ItemInfo> BuildItemInfos(BaseLayout layout, int visibleLine, int slot)
        {
            if (visibleLine == 0 && layout != null)
            {
                return this.GetPlaceholderSlot(layout, visibleLine);
            }

            return new List<ItemInfo>();
        }

        public override int CalculateAppendedSlotsCount(BaseLayout layout, int startSlot, ref int totalLines)
        {
            return 1;
        }


        private IList<ItemInfo> GetPlaceholderSlot(BaseLayout layout, int visibleLine)
        {
            var itemInfo = new ItemInfo();

            itemInfo.IsDisplayed = true;

            itemInfo.Item = this.Placeholder;
            itemInfo.Level = 0;
            itemInfo.Id = 0;
            itemInfo.Slot = itemInfo.Id;

            itemInfo.IsCollapsible = false;
            itemInfo.IsCollapsed = false;
            itemInfo.ItemType = BaseLayout.GetItemType(itemInfo.Item);
            itemInfo.IsSummaryVisible = false;

            LayoutInfo info = new LayoutInfo();
            //  info.Indent = this.GetIndent(itemInfo, parentGroupInfo);
            info.Indent = 0;
            info.Line = visibleLine;
            info.ChildLine = visibleLine;
            info.LineSpan = 1;
            info.LevelSpan = 1;
            info.SpansThroughCells = true;
            info.Level = 0;

            itemInfo.LayoutInfo = info;

            return new List<ItemInfo> { itemInfo };
        }
    }
}
