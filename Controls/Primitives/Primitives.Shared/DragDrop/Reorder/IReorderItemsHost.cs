using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Primitives.DragDrop.Reorder
{
    internal interface IReorderItemsHost
    {
        IReorderItem ElementAt(int index);

        void OnItemsReordered(IReorderItem sourceItem, IReorderItem destinationItem);

        void CommitReorderOperation(int sourceIndex, int destinationIndex);
    }
}
