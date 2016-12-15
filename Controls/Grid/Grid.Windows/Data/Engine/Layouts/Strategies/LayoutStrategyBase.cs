using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public abstract int CalculateAppendedSlotsCount(BaseLayout layout, int startSlot);

        public abstract void OnItemAdded(AddRemoveLayoutResult layoutResult);

        public abstract void OnItemRemoved(AddRemoveLayoutResult layoutResult);  
    }
}
