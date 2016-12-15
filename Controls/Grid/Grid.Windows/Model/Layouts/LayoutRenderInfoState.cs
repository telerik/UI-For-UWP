using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Grid;

namespace Telerik.Data.Core.Layouts
{
    internal class LayoutRenderInfoState : IRenderInfoState
    {
        private IndexToValueTable<bool> collapsedSlots;

        public LayoutRenderInfoState(IndexToValueTable<bool> collapsedSlots)
        {
            this.collapsedSlots = collapsedSlots;
        }

        public double? GetValueAt(int index)
        {
            if (this.collapsedSlots.GetValueAt(index))
            {
                return 0;
            }
            return null;
        }
    }
}
