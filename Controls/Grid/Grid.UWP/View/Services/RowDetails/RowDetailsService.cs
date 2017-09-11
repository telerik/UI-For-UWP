using System.Collections.Generic;
using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Telerik.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal class RowDetailsService : ServiceBase<RadDataGrid>
    {
        internal DataGridRowDetailsControl DetailsPresenter;

        public RowDetailsService(RadDataGrid owner)
            : base(owner)
        {
            this.ExpandedItems = new HashSet<object>();

            this.DetailsPresenter = new DataGridRowDetailsControl();
        }

        internal HashSet<object> ExpandedItems { get; private set; }

        public void CollapseDetailsForItem(object item)
        {
            this.ExpandedItems.Remove(item);
            this.Owner.updateService.RegisterUpdate((int)UpdateFlags.AffectsContent);
        }

        public void ExpandDetailsForItem(object item)
        {
            if (this.Owner.RowDetailsDisplayMode == DataGridRowDetailsMode.Single)
            {
                this.ExpandedItems.Clear();
                this.ExpandedItems.Add(item);
                this.Owner.updateService.RegisterUpdate((int)UpdateFlags.AffectsContent);
            }
        }

        public bool HasExpandedRowDetails(object item)
        {
            return this.ExpandedItems.Contains(item);
        }

        public void Init()
        {
            var contentLayer = this.Owner.GetContentLayerForColumn(null);
            if (contentLayer != null)
            {
                contentLayer.AddVisualChild(this.DetailsPresenter);
            }
        }

        public void UpdateItems()
        {
            switch (this.Owner.RowDetailsDisplayMode)
            {
                case DataGridRowDetailsMode.None:
                    this.ExpandedItems.Clear();
                    break;

                case DataGridRowDetailsMode.Single:
                    break;

                default:
                    break;
            }

            this.Owner.updateService.RegisterUpdate((int)UpdateFlags.AffectsContent);
        }
    }
}