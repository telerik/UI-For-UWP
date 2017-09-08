using System.Collections.Generic;

namespace Telerik.Data.Core.Layouts
{
    internal class LayoutStrategyComparer : IComparer<LayoutStrategyBase>
    {
        public int Compare(LayoutStrategyBase x, LayoutStrategyBase y)
        {
            if (x == null && y == null)
            {
                return 0;
            }

            if (x == null)
            {
                return -1;
            }

            if (y == null)
            {
                return 1;
            }

            return x.ExecutionOrder.CompareTo(y.ExecutionOrder);
        }
    }
}
