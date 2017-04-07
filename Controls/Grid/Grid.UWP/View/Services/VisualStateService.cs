using System.Linq;
using Telerik.Core;
using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal class VisualStateService : ServiceBase<RadDataGrid>
    {
        private DataProviderStatus dataStatus;
        private WeakReferenceList<IDataStatusListener> registeredDataLoadingListeners;

        internal VisualStateService(RadDataGrid owner) : base(owner)
        {
            this.registeredDataLoadingListeners = new WeakReferenceList<IDataStatusListener>();
        }

        internal void UpdateHoverDecoration(GridCellModel hoveredNode)
        {
            RadRect slot;

            switch (this.Owner.SelectionUnit)
            {
                case DataGridSelectionUnit.Row:
                    var row = hoveredNode == null ? null : hoveredNode.parent;
                    slot = row != null ? row.LayoutSlot : RadRect.Empty;
                    this.Owner.visualStateLayerCache.UpdateHoverDecoration(slot);
                    this.Owner.frozenVisualStateLayerCache.UpdateHoverDecoration(slot);
                    break;
                case DataGridSelectionUnit.Cell:
                    slot = hoveredNode != null ? hoveredNode.LayoutSlot : RadRect.Empty;
                    this.Owner.visualStateLayerCache.UpdateHoverDecoration(slot);
                    this.Owner.frozenVisualStateLayerCache.UpdateHoverDecoration(slot);
                    break;
                default:
                    break;
            }
        }

        internal void UpdateCurrentDecoration(int slotIndex)
        {
            Node node = null;

            if (slotIndex >= 0)
            {
                node = this.Owner.Model.RowPool.GetDisplayedElement(slotIndex);
            }

            if (this.Owner.visualStateLayerCache != null)
            {
                var slot = node != null ? node.LayoutSlot : RadRect.Empty;
                this.Owner.visualStateLayerCache.UpdateCurrencyDecoration(slot);
                this.Owner.frozenVisualStateLayerCache.UpdateCurrencyDecoration(slot);
            }
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