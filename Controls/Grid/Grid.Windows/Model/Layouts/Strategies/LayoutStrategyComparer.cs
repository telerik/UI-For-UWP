using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
