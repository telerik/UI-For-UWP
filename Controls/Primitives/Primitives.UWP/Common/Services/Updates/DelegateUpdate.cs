using System;
using Windows.UI.Core;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    internal class DelegateUpdate<T> : Update<T>
    {
        private Action updateAction;

        public DelegateUpdate(Action action)
        {
            this.updateAction = action;
            this.Priority = CoreDispatcherPriority.Normal;
        }

        internal override void Process()
        {
            base.Process();

            if (this.updateAction != null)
            {
                this.updateAction();
            }
        }
    }
}
