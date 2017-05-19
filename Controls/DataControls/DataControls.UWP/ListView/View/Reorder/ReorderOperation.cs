using Telerik.UI.Xaml.Controls.Primitives.DragDrop.Reorder;
using Windows.Foundation;

namespace Telerik.UI.Xaml.Controls.Data.ListView.View.Controls
{
    internal class ReorderItemsDragOperation
    {
        private int initialSourceIndex;
        private IReorderItem item;
        private object data;

        public ReorderItemsDragOperation(IReorderItem item, object data)
        {
            this.initialSourceIndex = item.LogicalIndex;
            this.item = item;
            this.data = data;
        }

        public object Data
        {
            get
            {
                return this.data;
            }
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

        public Rect LastReorderPositon
        {
            get;
            set;
        }

        internal IReorderItem Item
        {
            get
            {
                return this.item;
            }
        }
    }
}