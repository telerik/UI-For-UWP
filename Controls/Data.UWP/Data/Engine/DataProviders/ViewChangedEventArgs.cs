using System;
using System.Collections.Generic;
using Telerik.Data.Core.Engine;
using Windows.Foundation.Collections;

namespace Telerik.Data.Core
{
    internal class ViewChangedEventArgs : EventArgs
    {
        public ViewChangedEventArgs(List<AddRemoveResult> changedGroups, CollectionChange action)
        {
            this.Changes = changedGroups;
            this.Action = action;
        }

        public List<AddRemoveResult> Changes { get; private set; }

        public CollectionChange Action { get; private set; }
    }
}