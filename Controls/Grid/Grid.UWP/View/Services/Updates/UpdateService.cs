using Telerik.Core;
using Telerik.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Serves as a sync service for all the internal updates that originate within the grid.
    /// </summary>
    internal class UpdateService : UpdateServiceBase<UpdateFlags>
    {
        public UpdateService(RadControl owner, bool shouldExecuteSynchronously) : base(owner, shouldExecuteSynchronously)
        {
        }

        public override bool HasPendingUpdates
        {
            get
            {
                return this.PendingUpdateFlags != UpdateFlags.None;
            }
        }

        public void RegisterUpdate(UpdateFlags flags)
        {
            this.RegisterUpdate((int)flags);
        }

        protected override void ResetPendingFlags()
        {
            this.pendingUpdateFlags = (int)UpdateFlags.None;
        }

        protected override bool TryRefreshData(int flags)
        {
            var enumFlags = (UpdateFlags)flags;

            var grid = this.Owner as RadDataGrid;

            if ((enumFlags & UpdateFlags.AffectsData) == UpdateFlags.AffectsData && grid.Model.ItemsSource != null)
            {
                grid.Model.RefreshData();

                // The RefreshData call will schedule a new update of the UI
                return true;
            }

            return false;
        }

        protected override void ProcessUpdateFlags(int flags)
        {
            var enumFlags = (UpdateFlags)flags;

            var grid = this.Owner as RadDataGrid;

            if (enumFlags == UpdateFlags.None)
            {
                return;
            }

            if (enumFlags == UpdateFlags.AffectsColumnHeader)
            {
                grid.Model.ColumnPool.Update(enumFlags);
                return;
            }

            grid.Model.ColumnPool.Update(enumFlags);
            grid.Model.RowPool.Update(enumFlags);
            grid.Model.CellsController.Update(enumFlags);
            grid.Model.DecorationsController.Update(enumFlags);

            if (!this.TryRefreshData(flags))
            {
                if ((enumFlags & UpdateFlags.AffectsScrollPosition) == UpdateFlags.AffectsScrollPosition)
                {
                    grid.Model.GridView.SetScrollPosition(RadPoint.Empty, false, true);
                }

                grid.Model.GridView.RebuildUI();
            }
        }
    }
}