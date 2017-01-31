using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Primitives.DragDrop.Reorder;
using Windows.Foundation;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    internal class ReorderItemsDragOperation
    {
        private int initialSourceIndex;
        private IReorderItem item;

        public ReorderItemsDragOperation(IReorderItem item)
        {
            this.initialSourceIndex = item.LogicalIndex;
            this.item = item;
        }

        public int CurrentSourceReorderIndex
        {
            get
            {
                return this.item.LogicalIndex;
            }
            set
            {
                this.item.LogicalIndex = value;
            }
        }
        public int InitialSourceIndex
        {
            get
            {
                return this.initialSourceIndex;
            }
        }

        public Rect LastReorderPositon { get; set; }
    }
}
