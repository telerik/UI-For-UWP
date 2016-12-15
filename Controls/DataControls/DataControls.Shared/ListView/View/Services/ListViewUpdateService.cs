using Telerik.UI.Xaml.Controls.Data.ListView;
using Telerik.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Xaml.Controls.Data
{
    internal class ListViewUpdateService : UpdateServiceBase<ListView.UpdateFlags>
    {
        internal ListViewUpdateService(RadControl owner, bool shouldExecuteSynchronously) : base(owner, shouldExecuteSynchronously)
        {
        }

        public override bool HasPendingUpdates
        {
            get
            {
                return this.PendingUpdateFlags != ListView.UpdateFlags.None;
            }
        }

        internal RadListView View
        {
            get
            {
                return this.Owner as RadListView;
            }
        }

        protected override bool TryRefreshData(int flags)
        {
            if (((UpdateFlags)flags & UpdateFlags.AffectsData) == UpdateFlags.AffectsData && this.View.Model.ItemsSource != null)
            {
                this.View.Model.RefreshData();

                // The RefreshData call will schedule a new update of the UI
                return true;
            }
            return false;
        }

        protected override void ProcessUpdateFlags(int flags)
        {
            var enumFlags = (UpdateFlags)flags;

            if (enumFlags == UpdateFlags.None)
            {
                return;
            }

            this.View.Model.OnUpdate((ListView.UpdateFlags)flags);

            if (!this.TryRefreshData(flags))
            {
                this.View.RebuildUI();
            }
        }

        protected override void ResetPendingFlags()
        {
            this.pendingUpdateFlags = (int)UpdateFlags.None;
        }
    }
}