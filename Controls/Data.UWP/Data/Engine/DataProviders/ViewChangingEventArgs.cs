using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;

namespace Telerik.Data.Core
{
    internal class ViewChangingEventArgs : EventArgs
    {
        public ViewChangingEventArgs(IList changedItems, CollectionChange action)
        {
            this.ChangedItems = changedItems;
            this.Action = action;
        }

        public IList ChangedItems { get; private set; }
        public CollectionChange Action { get; private set; }
    }
}
