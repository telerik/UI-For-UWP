using System.Collections.Generic;

namespace Telerik.UI.Xaml.Controls.Data
{
    internal class ScrollUpdateService
    {
        private Queue<DelegateUpdate> updatesQueue;

        public ScrollUpdateService()
        {
            this.updatesQueue = new Queue<DelegateUpdate>();
        }

        public void RegisterUpdate(DelegateUpdate update)
        {
            this.updatesQueue.Enqueue(update);
        }

        internal void ProcessUpdatesQueue()
        {
            while (this.updatesQueue.Count > 0)
            {
                this.updatesQueue.Dequeue().Process();
            }
        }
    }
}
