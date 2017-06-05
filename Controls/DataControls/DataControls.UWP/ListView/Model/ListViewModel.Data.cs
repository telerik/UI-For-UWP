using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Telerik.Data.Core;
using Telerik.Data.Core.Layouts;
using Telerik.UI.Xaml.Controls.Data.ListView;
using Windows.Foundation.Collections;

namespace Telerik.UI.Xaml.Controls.Data
{
    internal partial class ListViewModel : IDataDescriptorsHost
    {
        private SortDescriptorCollection sortDescriptors;
        private GroupDescriptorCollection groupDescriptors;
        private FilterDescriptorCollection filterDescriptors;
        private AggregateDescriptorCollection aggregateDescriptors;

        public bool IsDataReady
        {
            get
            {
                return this.itemsSource != null && !this.isDataProviderUpdating;
            }
        }

        public SortDescriptorCollection SortDescriptors
        {
            get
            {
                return this.sortDescriptors;
            }
        }

        public GroupDescriptorCollection GroupDescriptors
        {
            get
            {
                return this.groupDescriptors;
            }
        }

        public FilterDescriptorCollection FilterDescriptors
        {
            get
            {
                return this.filterDescriptors;
            }
        }

        public AggregateDescriptorCollection AggregateDescriptors
        {
            get
            {
                return this.aggregateDescriptors;
            }
        }

        IDataProvider IDataDescriptorsHost.CurrentDataProvider
        {
            get
            {
                return this.CurrentDataProvider;
            }
        }

        IEnumerable<IDataDescriptorPeer> IDataDescriptorsHost.DescriptorPeers
        {
            get
            {
                return Enumerable.Empty<IDataDescriptorPeer>();
            }
        }

        public double BufferScale { get; internal set; }

        public void InitializeDescriptors()
        {
            this.sortDescriptors = new SortDescriptorCollection((IDataDescriptorsHost)this);
            this.sortDescriptors.CollectionChanged += this.OnSortDescriptorsCollectionChanged;

            this.groupDescriptors = new GroupDescriptorCollection((IDataDescriptorsHost)this);
            this.groupDescriptors.CollectionChanged += this.OnGroupDescriptorsCollectionChanged;

            this.filterDescriptors = new FilterDescriptorCollection((IDataDescriptorsHost)this);
            this.filterDescriptors.CollectionChanged += this.OnFilterDescriptorsCollectionChanged;

            this.aggregateDescriptors = new AggregateDescriptorCollection((IDataDescriptorsHost)this);
            this.aggregateDescriptors.CollectionChanged += this.OnAggregateDescriptorsCollectionChanged;
        }

        void IDataDescriptorsHost.OnDataDescriptorPropertyChanged(DataDescriptor descriptor)
        {
            this.dataChangeFlags |= descriptor.UpdateFlags;
            this.View.UpdateService.RegisterUpdate((int)UpdateFlags.AffectsData);
        }

        internal void ProcessPendingCollectionChange()
        {
            this.localDataProvider.DataView.ProcessPendingCollectionChange();
        }

        private void OnSortDescriptorsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.dataChangeFlags |= DataChangeFlags.Sort;
            this.View.UpdateService.RegisterUpdate((int)UpdateFlags.AffectsData);
        }

        private void OnGroupDescriptorsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.dataChangeFlags |= DataChangeFlags.Group;
            this.View.UpdateService.RegisterUpdate((int)UpdateFlags.AffectsData);
        }

        private void OnFilterDescriptorsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.dataChangeFlags |= DataChangeFlags.Filter;
            this.View.UpdateService.RegisterUpdate((int)UpdateFlags.AffectsData);
        }

        private void OnAggregateDescriptorsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.dataChangeFlags |= DataChangeFlags.Aggregate;
            this.View.UpdateService.RegisterUpdate((int)UpdateFlags.AffectsData);
        }

        private void HandleItemAdded(ViewChangingEventArgs e)
        {
            foreach (var changedItem in e.ChangedItems)
            {
                this.View.AnimatingService.ScheduleItemForAnimation(changedItem);
            }
        }

        private void OnDataProviderViewChanging(object sender, ViewChangingEventArgs e)
        {
            if (e.Action == CollectionChange.ItemRemoved)
            {
                if (((this.View.ItemAnimationMode & ItemAnimationMode.PlayOnRemove) == ItemAnimationMode.PlayOnRemove) && this.View.ItemRemovedAnimation != null)
                {
                    this.layoutController.HandleItemRemoved(e.ChangedItems);
                }
                else
                {
                    this.localDataProvider.DataView.ProcessPendingCollectionChange();
                }
            }

            if (e.Action == CollectionChange.ItemInserted && ((this.View.ItemAnimationMode & ItemAnimationMode.PlayOnAdd) == ItemAnimationMode.PlayOnAdd) && this.View.ItemAddedAnimation != null)
            {
                this.HandleItemAdded(e);
            }

            if (e.Action == CollectionChange.Reset)
            {
                if ((this.View.ItemAnimationMode & ItemAnimationMode.PlayOnSourceReset) == ItemAnimationMode.PlayOnSourceReset && this.View.ItemRemovedAnimation != null)
                {
                    this.layoutController.HandleSourceResetAnimations();
                }
                else
                {
                    this.localDataProvider.DataView.ProcessPendingCollectionChange();
                }
            }

            this.View.SelectionService.OnDataChanged(e.Action, e.ChangedItems);
        }

        private void OnDataProviderViewChanged(object sender, ViewChangedEventArgs e)
        {
            if (!this.IsDataReady)
            {
                this.View.UpdateService.RegisterUpdate((int)UpdateFlags.All);
                return;
            }

            if (e.Action == CollectionChange.Reset)
            {
                this.View.UpdateService.RegisterUpdate((int)UpdateFlags.All);
            }
            else if (e.Action == CollectionChange.ItemChanged)
            {
                this.View.UpdateService.RegisterUpdate((int)UpdateFlags.AffectsContent);
                this.dataChangeFlags |= DataChangeFlags.PropertyChanged;
            }
            else
            {
                var changes = e.Changes;
                for (int i = 0; i < changes.Count; i++)
                {
                    var change = changes[i];
                    object changedItem = change.ChangedCoordinate.RowGroup;
                    object addRemoveItem = change.AddedOrRemovedCoordinate.RowGroup;

                    // We use only RowGroupIndex for RowLayout and ColumnGroupIndex for ColumnLayout.
                    // For now only rowLayout is updated.
                    int index = change.AddRemoveRowGroupIndex;

                    AddRemoveLayoutResult result;
                    switch (e.Action)
                    {
                        case CollectionChange.ItemInserted:
                            result = this.layoutController.AddItem(changedItem, addRemoveItem, index);
                            this.View.UpdateService.RegisterUpdate((int)UpdateFlags.AffectsContent);

                            break;

                        case CollectionChange.ItemRemoved:
                            result = this.layoutController.RemoveItem(changedItem, addRemoveItem, index);

                            this.View.UpdateService.RegisterUpdate((int)UpdateFlags.AffectsContent);
                            break;

                        default:
                            System.Diagnostics.Debug.Assert(true, "Not expected action. Only Add, Remove or Reset are supported");
                            break;
                    }
                }
            }

            this.View.CurrencyService.OnRefreshData();
        }
    }
}