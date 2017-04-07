using System;
using Telerik.Data.Core;
using Telerik.Data.Core.Layouts;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal class DataGridCurrencyService : ServiceBase<RadDataGrid>
    {
        internal bool ensureCurrentIntoView = true;
        internal bool isSynchronizedWithCurrent;
        internal bool isCurrentInView;

        private object currentItem;
        private ItemInfo? currentItemInfo;
        private bool updatingCurrent;
        private ICollectionView itemsSourceAsCollectionView;

        public DataGridCurrencyService(RadDataGrid owner)
            : base(owner)
        {
        }

        public event EventHandler CurrentChanged;

        public event CurrentChangingEventHandler CurrentChanging;

        public object CurrentItem
        {
            get
            {
                return this.currentItem;
            }
        }

        public ItemInfo? CurrentItemInfo
        {
            get
            {
                return this.currentItemInfo;
            }
        }

        internal void OnDataViewChanged(ViewChangedEventArgs args)
        {
            if (this.currentItem == null)
            {
                return;
            }

            this.currentItemInfo = null;

            // TODO: Update the CurrentItem
            switch (args.Action)
            {
                case CollectionChange.ItemRemoved:
                    break;
            }
        }

        internal void OnGroupExpandStateChanged()
        {
            if (this.currentItem == null)
            {
                return;
            }

            this.UpdateState(false);
        }

        internal bool MoveCurrentTo(object item)
        {
            this.ChangeCurrentItem(item, true, true);
            return object.ReferenceEquals(this.currentItem, item);
        }

        internal bool MoveCurrentToNext()
        {
            var nextItem = this.Owner.Model.FindPreviousOrNextDataItem(this.currentItem, true);
            if (nextItem == null)
            {
                return false;
            }

            this.ChangeCurrentItem(nextItem, true, true);
            return object.ReferenceEquals(this.currentItem, nextItem.Value.Item);
        }

        internal bool MoveCurrentPrevious()
        {
            var prevItem = this.Owner.Model.FindPreviousOrNextDataItem(this.currentItem, false);
            if (prevItem == null)
            {
                return false;
            }

            this.ChangeCurrentItem(prevItem, true, true);
            return object.ReferenceEquals(this.currentItem, prevItem.Value.Item);
        }

        internal bool MoveCurrentToFirst()
        {
            var firstItem = this.Owner.Model.FindFirstDataItemInView();
            if (firstItem == null)
            {
                return false;
            }

            this.ChangeCurrentItem(firstItem, true, true);
            return object.ReferenceEquals(this.currentItem, firstItem.Value.Item);
        }

        internal bool MoveCurrentToLast()
        {
            ItemInfo? lastItem = this.Owner.Model.FindLastDataItemInView();
            if (lastItem == null)
            {
                return false;
            }

            this.ChangeCurrentItem(lastItem, true, true);
            return object.ReferenceEquals(this.currentItem, lastItem.Value.Item);
        }

        internal void OnSelectedItemChanged(object newItem)
        {
            if (this.isSynchronizedWithCurrent)
            {
                this.ChangeCurrentItem(newItem, false, true);
            }
        }

        internal void OnItemsSourceChanged(object newSource)
        {
            if (this.itemsSourceAsCollectionView != null)
            {
                this.itemsSourceAsCollectionView.CurrentChanged -= this.OnItemsSourceCurrentChanged;
            }

            this.itemsSourceAsCollectionView = newSource as ICollectionView;
            this.UpdateIsSynchronizedWithCurrent(this.isSynchronizedWithCurrent);

            if (this.itemsSourceAsCollectionView != null)
            {
                this.itemsSourceAsCollectionView.CurrentChanged += this.OnItemsSourceCurrentChanged;
                this.ChangeCurrentItem(this.itemsSourceAsCollectionView.CurrentItem, false, false);
            }
            else
            {
                this.ChangeCurrentItem(null, false, false);
            }
        }

        internal void OnDataBindingComplete(bool scrollToCurrent)
        {
            this.currentItemInfo = null;
            this.UpdateState(scrollToCurrent);
        }

        internal bool ChangeCurrentItem(object newCurrentItem, bool cancelable, bool scrollToCurrent)
        {
            if (this.updatingCurrent)
            {
                return false;
            }

            if (object.ReferenceEquals(this.currentItem, newCurrentItem))
            {
                return true;
            }

            var info = this.Owner.Model.FindItemInfo(newCurrentItem);
            return this.ChangeCurrentItem(info, cancelable, scrollToCurrent);
        }

        internal bool ChangeCurrentItem(ItemInfo? info, bool cancelable, bool scrollToCurrent)
        {
            if (this.updatingCurrent)
            {
                return false;
            }

            var newCurrent = info == null ? null : info.Value.Item;
            if (object.ReferenceEquals(this.currentItem, newCurrent))
            {
                return true;
            }

            return this.ChangeCurrentCore(newCurrent, info, cancelable, scrollToCurrent);
        }

        internal bool RefreshCurrentItem(bool scrollToCurrent)
        {
            var info = this.Owner.Model.FindItemInfo(this.currentItem);
            return this.ChangeCurrentCore(this.currentItem, info, false, scrollToCurrent);
        }

        internal void UpdateIsSynchronizedWithCurrent(bool synchronize)
        {
            this.isSynchronizedWithCurrent = synchronize;
            if (this.isSynchronizedWithCurrent)
            {
                this.ChangeCurrentItem(this.Owner.SelectedItem, false, false);
            }
        }

        private bool ChangeCurrentCore(object newCurrent, ItemInfo? info, bool cancelable, bool scrollToCurrent)
        {
            // Raise CurrentChanging first
            bool cancel = this.PreviewCancelCurrentChanging(cancelable);
            if (cancel)
            {
                // the change is canceled
                return false;
            }

            var oldCurrent = this.currentItem;

            this.updatingCurrent = true;

            this.currentItem = newCurrent;
            this.currentItemInfo = info;
            this.Owner.ChangePropertyInternally(RadDataGrid.CurrentItemProperty, this.currentItem);

            if (this.isSynchronizedWithCurrent)
            {
                this.Owner.SelectedItem = this.currentItem;
                if (this.itemsSourceAsCollectionView != null)
                {
                    this.itemsSourceAsCollectionView.MoveCurrentTo(this.currentItem);
                }
            }

            this.UpdateState(scrollToCurrent);

            if (!object.ReferenceEquals(oldCurrent, this.currentItem))
            {
                this.OnCurrentChanged(EventArgs.Empty);
            }

            this.updatingCurrent = false;

            return true;
        }

        private void OnItemsSourceCurrentChanged(object sender, object e)
        {
        }

        private bool PreviewCancelCurrentChanging(bool cancelable)
        {
            var eh = this.CurrentChanging;
            if (eh == null)
            {
                return false;
            }

            var args = new CurrentChangingEventArgs(cancelable);
            eh(this.Owner, args);

            return args.Cancel;
        }

        private void OnCurrentChanged(EventArgs args)
        {
            var eh = this.CurrentChanged;
            if (eh != null)
            {
                eh(this.Owner, args);
            }
        }

        private void UpdateState(bool scrollToCurrent)
        {
            if (this.currentItem == null)
            {
                this.isCurrentInView = false;
                this.Owner.visualStateService.UpdateCurrentDecoration(-1);

                return;
            }

            this.Owner.updateService.RegisterUpdate(new DelegateUpdate<UpdateFlags>(() =>
            {
                if (this.currentItemInfo == null)
                {
                    this.currentItemInfo = this.Owner.Model.FindItemInfo(this.currentItem);
                }

                if (this.currentItemInfo == null)
                {
                    this.isCurrentInView = false;
                    this.Owner.visualStateService.UpdateCurrentDecoration(-1);
                }
                else
                {
                    this.Owner.Model.UpdateEditRow(this.currentItemInfo.Value.LayoutInfo.Line);
                    this.isCurrentInView = true;

                    if (scrollToCurrent)
                    {
                        this.ScrollToCurrent();
                    }
                    else
                    {
                        this.Owner.visualStateService.UpdateCurrentDecoration(this.currentItemInfo.Value.Slot);
                    }
                }
            }));
        }

        private void ScrollToCurrent()
        {
            if (this.isCurrentInView && this.ensureCurrentIntoView && this.currentItem != null)
            {
                this.Owner.ScrollIndexIntoView(
                    this.currentItemInfo.Value.Slot,
                    () =>
                    {
                        if (this.currentItemInfo != null)
                        {
                            this.Owner.visualStateService.UpdateCurrentDecoration(this.currentItemInfo.Value.Slot);
                        }
                        else
                        {
                            this.Owner.visualStateService.UpdateCurrentDecoration(-1);
                        }
                    });
            }
        }
    }
}