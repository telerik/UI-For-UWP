using System.Collections.Generic;

namespace Telerik.Data.Core.Layouts
{
    internal abstract class LayoutStrategyBase
    {
        public LayoutStrategyBase(int order)
        {
            this.ExecutionOrder = order;
        }

        public int ExecutionOrder { get; private set; }

        public abstract IList<ItemInfo> BuildItemInfos(BaseLayout layout, int visibleLine, int slot);

        public abstract int CalculateAppendedSlotsCount(BaseLayout layout, int startSlot, ref int totalLines);

        public abstract void OnItemAdded(AddRemoveLayoutResult layoutResult);

        public abstract void OnItemRemoved(AddRemoveLayoutResult layoutResult);
    }
}
