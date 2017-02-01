using System;
using Windows.UI.Core;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal class DelegateUpdate : Update
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
