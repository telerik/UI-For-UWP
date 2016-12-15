using System;
using Windows.UI.Core;

namespace Telerik.UI.Xaml.Controls.Data
{
    internal class DelegateUpdate
    {
        private Action updateAction;

        public DelegateUpdate(Action action)
        {
            this.updateAction = action;
            this.Priority = CoreDispatcherPriority.Normal;
        }

        public CoreDispatcherPriority Priority { get; set; }

        internal virtual void Process()
        {
            if (this.updateAction != null)
            {
                this.updateAction();
            }
        }
    }
}
