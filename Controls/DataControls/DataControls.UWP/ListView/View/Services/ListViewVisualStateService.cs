using System.Linq;
using Telerik.Core;
using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Xaml.Controls.Data
{
    internal class ListViewVisualStateService : ServiceBase<RadListView>
    {
        private DataProviderStatus dataStatus;
        private WeakReferenceList<IDataStatusListener> registeredDataLoadingListeners;

        internal ListViewVisualStateService(RadListView owner) : base(owner)
        {
            this.registeredDataLoadingListeners = new WeakReferenceList<IDataStatusListener>();
        }

        internal void UpdateDataLoadingStatus(DataProviderStatus status)
        {
            this.dataStatus = status;

            foreach (var listener in this.registeredDataLoadingListeners)
            {
                listener.OnDataStatusChanged(status);
            }
        }

        internal void RegisterDataLoadingListener(IDataStatusListener listener)
        {
            if (!this.registeredDataLoadingListeners.Contains(listener))
            {
                this.registeredDataLoadingListeners.Add(listener);
            }

            listener.OnDataStatusChanged(this.dataStatus);
        }

        internal void UnregisterDataLoadingListener(IDataStatusListener listener)
        {
            this.registeredDataLoadingListeners.Remove(listener);
            listener.OnDataStatusChanged(DataProviderStatus.Uninitialized);
        }
    }
}