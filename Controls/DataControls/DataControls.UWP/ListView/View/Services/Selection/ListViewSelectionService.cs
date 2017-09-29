using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Data.ListView.Commands;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.Foundation.Collections;

namespace Telerik.UI.Xaml.Controls.Data.ListView
{
    internal interface ISelectionService
    {
        ListViewSelectionService SelectionService { get; }
    }

    internal class ListViewSelectionService : ServiceBase<RadListView>
    {
        internal HashSet<object> selectedItemsSet;
        private SelectedItemCollection selectedItems;

        internal ListViewSelectionService(RadListView owner) : base(owner)
        {
            if (this.Owner == null)
            {
                throw new ArgumentNullException("Selection service cannot operate without owner");
            }

            this.selectedItems = new SelectedItemCollection();
            this.selectedItems.AllowMultipleSelect = this.Owner.SelectionMode == DataControlsSelectionMode.Multiple || this.Owner.SelectionMode == DataControlsSelectionMode.MultipleWithCheckBoxes;
            this.selectedItems.AllowSelect = this.Owner.SelectionMode != DataControlsSelectionMode.None;

            this.selectedItems.CollectionChanged += this.OnSelectedItemsCollectionChanged;
            this.selectedItemsSet = new HashSet<object>();
        }

        internal event EventHandler<ListViewSelectionChangedEventArgs> SelectionChanged;

        public SelectedItemCollection SelectedItems
        {
            get
            {
                return this.selectedItems;
            }
        }

        internal DataControlsSelectionMode SelectionMode
        {
            get
            {
                return this.Owner.SelectionMode;
            }
        }

        internal void SelectItem(object item, bool select, bool uiSelect)
        {
            if (!this.CanSelectDataItem(item))
            {
                return;
            }

            switch (this.Owner.SelectionMode)
            {
                case DataControlsSelectionMode.Single:
                    this.SelectSingleItem(item, select);
                    break;

                case DataControlsSelectionMode.Multiple:
                    this.SelectMultipleItems(item, select, uiSelect);
                    break;

                case DataControlsSelectionMode.MultipleWithCheckBoxes:
                    this.SelectMultipleItems(item, select, uiSelect);
                    this.Owner.itemCheckBoxService.SelectItem(item, select);
                    break;

                default:
                    break;
            }

            this.OnSelectionChanged();
        }

        internal void SelectAll()
        {
            if (this.SelectionMode != DataControlsSelectionMode.Multiple && this.SelectionMode != DataControlsSelectionMode.MultipleWithCheckBoxes)
            {
                return;
            }

            this.ClearSelection();
            this.SelectAllItems();
        }

        internal void ClearSelection()
        {
            this.selectedItemsSet.Clear();

            if (this.selectedItems.Count > 0)
            {
                this.selectedItems.Clear();
            }

            this.OnSelectionChanged();
        }

        internal bool IsSelected(object item)
        {
            return this.selectedItemsSet.Contains(item);
        }

        internal void OnSelectionModeChanged(DataControlsSelectionMode listViewSelectionMode)
        {
            if (listViewSelectionMode != DataControlsSelectionMode.Multiple && listViewSelectionMode != DataControlsSelectionMode.MultipleWithCheckBoxes)
            {
                this.ClearSelection();
            }

            this.selectedItems.AllowMultipleSelect = listViewSelectionMode == DataControlsSelectionMode.Multiple || listViewSelectionMode == DataControlsSelectionMode.MultipleWithCheckBoxes;
            this.selectedItems.AllowSelect = listViewSelectionMode != DataControlsSelectionMode.None;
        }

        internal void OnSelectionChanged()
        {
            (this.Owner as IListView).UpdateService.RegisterUpdate((int)UpdateFlags.AffectsDecorations);
        }

        internal void OnSelectedItemChanged(object oldValue, object newValue)
        {
            if (newValue == null && this.selectedItems.Count > 0)
            {
                this.ClearSelection();
            }
            else if (!this.CanSelectDataItem(newValue))
            {
                this.Owner.ChangePropertyInternally(RadListView.SelectedItemProperty, oldValue);
            }
            else
            {
                this.ClearSelection();
                this.SelectItem(newValue, true, false);
            }
        }

        internal void Select(ItemTapContext tapContext)
        {
            // TODO refactor to not use visual.
            if (this.Owner.SelectionMode != DataControlsSelectionMode.MultipleWithCheckBoxes)
            {
                this.SelectItem(tapContext.Item, !tapContext.Container.IsSelected, true);
            }
        }

        internal void OnDataChanged(CollectionChange action, IList changedItems)
        {
            switch (action)
            {
                case CollectionChange.Reset:
                    this.ClearSelection();
                    break;

                case CollectionChange.ItemRemoved:
                    foreach (var removedItem in changedItems)
                    {
                        if (this.SelectedItems.Contains(removedItem))
                        {
                            this.SelectedItems.Remove(removedItem);
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        private void SelectAllItems()
        {
            var newSelectedItems = this.selectedItemsSet.ToArray();

            // TODO: schedule update if data is not ready.
            //      throw new NotImplementedException("data view?");

            // TODO: foreach (var item in this.Owner.GetDataView())
            foreach (var item in this.Owner.Model.CurrentDataProvider.ItemsSource as IEnumerable)
            {
                if (item is IDataGroup)
                {
                    continue;
                }

                this.SelectItem(item, true, false);
            }

            this.OnSelectedItemsChanged(newSelectedItems, this.selectedItemsSet.Select(c => c));

            this.OnSelectionChanged();
        }

        private void SelectSingleItem(object item, bool select)
        {
            this.SelectSingleItemCore(item, select);

            if (select)
            {
                this.selectedItemsSet.Add(item);
            }
        }

        private void SelectSingleItemCore(object item, bool select)
        {
            if (select)
            {
                this.selectedItemsSet.Clear();
                this.selectedItemsSet.Add(item);

                if (this.selectedItems.Count == 1)
                {
                    this.selectedItems[0] = item;
                }
                else
                {
                    this.selectedItems.Add(item);
                }
            }
            else if (this.selectedItems.Count == 1 && this.selectedItemsSet.Contains(item))
            {
                this.selectedItemsSet.Clear();
                this.selectedItems.Remove(item);
            }
        }

        private void SelectMultipleItems(object item, bool select, bool toggleSelection)
        {
            if (!select || (toggleSelection && this.selectedItemsSet.Contains(item)))
            {
                this.selectedItemsSet.Remove(item);
                this.selectedItems.Remove(item);
            }
            else if (select)
            {
                this.selectedItemsSet.Add(item);
                this.selectedItems.Add(item);
            }
        }

        private bool CanSelectDataItem(object item)
        {
            return item != null && this.Owner.SelectionMode != DataControlsSelectionMode.None;
        }

        private void OnSelectedItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                    this.UpdateItemsSet(e.OldItems, e.NewItems);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    this.Refresh();
                    break;

                case NotifyCollectionChangedAction.Move:
                default:
                    break;
            }
        }

        private void UpdateItemsSet(IList removedItems, IList addedItems)
        {
            IEnumerable<object> itemsToRemove = removedItems == null ? Enumerable.Empty<object>() : removedItems.OfType<object>();
            IEnumerable<object> itemsToAdd = addedItems == null ? Enumerable.Empty<object>() : addedItems.OfType<object>();

            foreach (var item in itemsToRemove)
            {
                this.SelectItem(item, false, false);
            }

            foreach (var item in itemsToAdd)
            {
                this.SelectItem(item, true, false);
            }

            this.OnSelectedItemsChanged(itemsToRemove, itemsToAdd);
        }

        private void Refresh()
        {
            IEnumerable<object> removedItems = null;
            IEnumerable<object> addedItems = null;

            removedItems = this.selectedItemsSet.ToArray();

            this.selectedItemsSet.Clear();
            foreach (var item in this.SelectedItems)
            {
                this.selectedItemsSet.Add(item);
            }

            // Select different Instance of enumerator to ensure that users does not have access to the private instance of set.
            addedItems = this.selectedItemsSet.Select(c => c);

            this.OnSelectedItemsChanged(removedItems, addedItems);
            this.OnSelectionChanged();
        }

        private void OnSelectedItemsChanged(IEnumerable<object> removedItems, IEnumerable<object> addedItems)
        {
            this.UpdateSelectedItem();

            if (this.SelectionChanged != null)
            {
                this.SelectionChanged(this.Owner, new ListViewSelectionChangedEventArgs(removedItems, addedItems));
            }
        }

        private void UpdateSelectedItem()
        {
            if (this.SelectedItems.Count > 0)
            {
                if (this.Owner.SelectedItem == null || !this.selectedItemsSet.Contains(this.Owner.SelectedItem))
                {
                    this.Owner.ChangePropertyInternally(RadListView.SelectedItemProperty, this.SelectedItems.First());
                }
            }
            else
            {
                this.Owner.ChangePropertyInternally(RadListView.SelectedItemProperty, null);
            }
        }
    }
}