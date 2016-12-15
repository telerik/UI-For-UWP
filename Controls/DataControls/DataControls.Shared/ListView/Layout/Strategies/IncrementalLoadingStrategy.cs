using System.Collections.Generic;
using Telerik.UI.Xaml.Controls.Data;

namespace Telerik.Data.Core.Layouts
{
    internal class IncrementalLoadingStrategy : PlaceholderStrategy
    {
        private static int DeafultExecutionOrder = SlotStrategyExecutionOrders.IncrementalLoadingOrder;

        public IncrementalLoadingStrategy() : base(DeafultExecutionOrder, PlaceholderInfoType.IncrementalLoading)
        {
        }

        public override IList<ItemInfo> BuildItemInfos(BaseLayout layout, int visibleLine, int slot)
        {
            if (layout != null && visibleLine == layout.VisibleLineCount - 1)
            {
                return this.GetPlaceholderSlot(layout, visibleLine);
            }

            return new List<ItemInfo>();
        }
    }
}
