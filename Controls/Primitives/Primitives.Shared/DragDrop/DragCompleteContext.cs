using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Primitives.DragDrop
{
    internal class DragCompleteContext : DragContext
    {
        public DragCompleteContext(object data, IDragDropOperation owner, bool dragSuccessful)
            : base(data, owner)
        {
            this.DragSuccessful = dragSuccessful;
        }

        public bool DragSuccessful { get; private set; }

        public object Destination { get; internal set; }
    }
}
