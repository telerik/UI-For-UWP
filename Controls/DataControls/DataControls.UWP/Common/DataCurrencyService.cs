using System;
using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Data
{
    internal abstract class DataCurrencyService : ServiceBase<RadControl>
    {
        // internal bool isSynchronizedWithCurrent;
        internal bool ensureCurrentIntoView = true;

        private object currentItem;
        private bool updatingCurrent;
        private ICollectionView itemsSourceAsCollectionView;

        public DataCurrencyService(RadControl owner)
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

        internal virtual void OnDataViewChanged(ViewChangedEventArgs args)
        {
            if (this.currentItem == null)
            {
                return;
            }
        }

        internal void OnGroupExpandStateChanged()
        {
            if (this.currentItem == null)
            {
                return;
            }

            this.UpdateOwnerState(false);
        }

        internal bool MoveCurrentTo(object item)
        {
            this.ChangeCurrentItem(item, true, true);
            return object.ReferenceEquals(this.currentItem, item);
        }

        internal virtual bool MoveCurrentToNext()
        {
            var nextItem = this.FindPreviousOrNextItem(this.currentItem, true);

            if (nextItem == null)
            {
                return false;
            }

            this.ChangeCurrentItem(nextItem, true, true);

            return object.ReferenceEquals(this.currentItem, nextItem);
        }

        internal bool MoveCurrentPrevious()
        {
            var prevItem = this.FindPreviousOrNextItem(this.currentItem, false);

            if (prevItem == null)
            {
                return false;
            }

            this.ChangeCurrentItem(prevItem, true, true);

            return object.ReferenceEquals(this.currentItem, prevItem);
        }

        internal bool MoveCurrentToFirst()
        {
            var firstItem = this.FindFirstItem();
            if (firstItem == null)
            {
                return false;
            }

            this.ChangeCurrentItem(firstItem, true, true);

            return object.ReferenceEquals(this.currentItem, firstItem);
        }

        internal bool MoveCurrentToLast()
        {
            var lastItem = this.FindLastItem();
            if (lastItem == null)
            {
                return false;
            }

            this.ChangeCurrentItem(lastItem, true, true);

            return object.ReferenceEquals(this.currentItem, lastItem);
        }

        internal void OnSelectedItemChanged(object newItem)
        {
        }

        internal void OnItemsSourceChanged(object newSource)
        {
            if (this.itemsSourceAsCollectionView != null)
            {
                this.itemsSourceAsCollectionView.CurrentChanged -= this.OnItemsSourceCurrentChanged;
            }

            this.itemsSourceAsCollectionView = newSource as ICollectionView;

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

        internal virtual void OnDataBindingComplete(bool scrollToCurrent)
        {
            this.UpdateOwnerState(scrollToCurrent);
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

            return this.ChangeCurrentCore(newCurrentItem, cancelable, scrollToCurrent);
        }

        internal virtual bool RefreshCurrentItem(bool scrollToCurrent)
        {
            return this.ChangeCurrentCore(this.currentItem, false, scrollToCurrent);
        }

        protected abstract object FindPreviousOrNextItem(object currentItem, bool next);

        protected abstract object FindFirstItem();

        protected abstract object FindLastItem();

        protected bool ChangeCurrentCore(object newCurrent, bool cancelable, bool scrollToCurrent)
        {
            // Raise CurrentChanging first
            bool cancel = this.PreviewCancelCurrentChanging(cancelable);
            if (cancel)
            {
                // the change is canceled
                return false;
            }

            return this.ChangeCurrentCoreOverride(newCurrent, scrollToCurrent);
        }

        protected virtual bool ChangeCurrentCoreOverride(object newCurrent, bool scrollToCurrent)
        {
            this.updatingCurrent = true;

            var oldCurrent = this.currentItem;
            this.currentItem = newCurrent;

            this.UpdateOwnerState(scrollToCurrent);

            if (!object.ReferenceEquals(oldCurrent, this.currentItem))
            {
                this.OnCurrentChanged(EventArgs.Empty);
            }

            this.updatingCurrent = false;

            return true;
        }

        protected abstract void UpdateOwnerState(bool scrollToCurrent);

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
    }
}