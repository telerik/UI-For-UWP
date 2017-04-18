using System;
using Telerik.Core;
using Telerik.Data.Core.Layouts;
using Telerik.UI.Xaml.Controls.Data.ContainerGeneration;
using Telerik.UI.Xaml.Controls.Data.ListView;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Data
{
    internal interface ICurrencyService
    {
        ListViewCurrencyService CurrencyService { get; }
    }
    internal class ListViewCurrencyService : ServiceBase<RadListView>, ISupportCurrentItem
    {
        internal bool ensureCurrentIntoView = true;
        internal bool isSynchronizedWithCurrent;
        internal bool isCurrentInView;

        private object currentItem;
        private ItemInfo? currentItemInfo;
        private bool updatingCurrent;
        private bool shouldRefreshCurrentItem;

        public ListViewCurrencyService(RadListView owner)
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

        public bool IsCurrentItemInView
        {
            get
            {
                return this.isCurrentInView;
            }
        }

        internal bool MoveCurrentTo(object item)
        {
            this.ChangeCurrentItem(item, true, true);
            return object.ReferenceEquals(this.currentItem, item);
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
            this.UpdateIsSynchronizedWithCurrent(this.isSynchronizedWithCurrent);
            this.ChangeCurrentItem(null, false, false);
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

        internal void OnRefreshData()
        {
            this.shouldRefreshCurrentItem = true;
        }

        internal bool RefreshCurrentItem(bool scrollToCurrent)
        {
            var info = this.Owner.Model.FindItemInfo(this.currentItem);
            if (info != null)
            {
                return this.ChangeCurrentCore(this.currentItem, info, false, scrollToCurrent);
            }
            else
            {
                 return this.ChangeCurrentCore(null, info, false, scrollToCurrent);
            }           
        }

        internal void UpdateIsSynchronizedWithCurrent(bool synchronize)
        {
            this.isSynchronizedWithCurrent = synchronize;
            if (this.isSynchronizedWithCurrent)
            {
                this.ChangeCurrentItem(this.Owner.SelectedItem, false, false);
            }
        }

        internal void UpdateCurrentDecoration(int slotIndex, int id)
        {
            GeneratedItemModel model = null;

            if (slotIndex >= 0)
            {       
                model = this.Owner.Model.GetDisplayedElement(slotIndex, id);
            }

            if (this.Owner.currencyLayerCache != null)
            {
                var slot = model != null ? model.LayoutSlot : RadRect.Empty;
                this.Owner.currencyLayerCache.UpdateCurrencyDecoration(slot);
            }
        }

        internal void ArrangeVisual()
        {
            GeneratedItemModel model = null;
            if (this.shouldRefreshCurrentItem)
            {
                 this.RefreshCurrentItem(false);
                 this.shouldRefreshCurrentItem = false;
            }        

            if (this.currentItemInfo != null)
            {
                model = this.Owner.Model.GetDisplayedElement(this.CurrentItemInfo.Value.Slot, this.CurrentItemInfo.Value.Id);
            }
         
            if (this.Owner.currencyLayerCache != null)
            {
               var slot = model != null ? model.LayoutSlot : RadRect.Empty;
               this.Owner.currencyLayerCache.UpdateCurrencyDecoration(slot);
            }
        }

        /// <inheritdoc />
        public bool MoveCurrentToNext()
        {
            return this.MoveCurrentToNextOrPrev(true);
        }

        /// <inheritdoc />
        public bool MoveCurrentToPrevious()
        {
            return this.MoveCurrentToNextOrPrev(false);
        }

        /// <inheritdoc />
        public bool MoveCurrentToLast()
        {
            return this.MoveCurrentToLastOrFirst(true);
        }

        /// <inheritdoc />
        public bool MoveCurrentToFirst()
        {
            return this.MoveCurrentToLastOrFirst(false);
        }
        private bool MoveCurrentToNextOrPrev(bool isNext)
        {
            int index;
            if (this.CurrentItemInfo == null)
            {
                // Current is set the item after (before) the focused one like in MS ListView.
                index = this.Owner.currentLogicalIndex;
            }
            else
            {
                index = this.CurrentItemInfo.Value.Slot;
            }

            int increment = isNext ? 1 : -1;
            ItemInfo? nextInfo = this.Owner.Model.FindDataItemFromIndex(index + increment, null);
            if (nextInfo.HasValue)
            {
                return this.MoveCurrentTo(nextInfo.Value.Item);
            }

            return false;
        }

        private bool MoveCurrentToLastOrFirst(bool isLast)
        {
            int index = isLast ? this.Owner.Model.ItemsCount - 1 : 0;
            ItemInfo? nextInfo = this.Owner.Model.FindDataItemFromIndex(index, null);
            if (nextInfo.HasValue)
            {
                return this.MoveCurrentTo(nextInfo.Value.Item);
            }

            return false;
        }

        private bool ChangeCurrentCore(object newCurrent, ItemInfo? info, bool cancelable, bool scrollToCurrent)
        {
            // Raise CurrentChanging first
            bool cancel = this.PreviewCancelCurrentChanging(cancelable);
         
            if (cancel || this.Owner.animationSurvice.IsAnimating(info))
            {
                // the change is canceled
                return false;
            }

            var oldCurrent = this.currentItem;

            this.updatingCurrent = true;

            this.currentItem = newCurrent;
            this.currentItemInfo = info;
            this.Owner.ChangePropertyInternally(RadListView.CurrentItemProperty, this.currentItem);

            if (this.isSynchronizedWithCurrent)
            {
                this.Owner.SelectedItem = this.currentItem;
            }

            this.UpdateState(scrollToCurrent);

            if (!object.ReferenceEquals(oldCurrent, this.currentItem))
            {
                this.OnCurrentChanged(EventArgs.Empty);
            }

            this.updatingCurrent = false;

            return true;
        }

        private void UpdateState(bool scrollToCurrent)
        {
            if (this.currentItem == null)
            {
                this.isCurrentInView = false;
                this.UpdateCurrentDecoration(-1, -1);

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
                    this.UpdateCurrentDecoration(-1, -1);
                }
                else
                {
                    this.isCurrentInView = true;

                    if (scrollToCurrent)
                    {
                        this.ScrollToCurrent();
                    }
                    else
                    {
                        this.UpdateCurrentDecoration(this.currentItemInfo.Value.Slot, this.currentItemInfo.Value.Id);
                    }
                }
            }));
        }

        private void ScrollToCurrent()
        {
            if (this.isCurrentInView && this.ensureCurrentIntoView && this.currentItem != null)
            {
                this.Owner.ScrollItemIntoView(
                    this.currentItem,
                    () =>
                    {
                        if (this.currentItemInfo != null)
                        {
                            this.UpdateCurrentDecoration(this.currentItemInfo.Value.Slot, this.currentItemInfo.Value.Id);
                        }
                        else
                        {
                            this.UpdateCurrentDecoration(-1, -1);
                        }
                    });
            }
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
            if (this.Owner.currencyLayerCache.CurrencyVisual is IAnimated)
            {
                if ((this.Owner.currencyLayerCache.CurrencyVisual as IAnimated).IsAnimating)
                {
                    this.Owner.animationSurvice.StopAnimation(this.Owner.currencyLayerCache.CurrencyVisual);
                }
            }
            var eh = this.CurrentChanged;
            if (eh != null)
            {
                eh(this.Owner, args);
            }
        }

        bool ISupportCurrentItem.MoveCurrentTo(object item)
        {
            throw new NotImplementedException();
        }
    }
}
