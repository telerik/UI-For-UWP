using System.Collections.ObjectModel;

namespace Telerik.UI.Xaml.Controls.Data.ListView
{
    internal class SelectedItemCollection : ObservableCollection<object>
    {
        private int suspendLevel;

        public bool AllowMultipleSelect { get; set; }
        public bool AllowSelect { get; set; }

        private bool IsSuspended
        {
            get
            {
                return this.suspendLevel > 0;
            }
        }

        internal void SuspendUpdate()
        {
            this.suspendLevel++;
        }

        internal void ResumeUpdate()
        {
            if (this.suspendLevel > 0)
            {
                this.suspendLevel--;
            }
        }

        protected override void InsertItem(int index, object item)
        {
            if (this.CanInsertItem(item))
            {
                base.InsertItem(index, item);
            }
        }

        protected override void RemoveItem(int index)
        {
            if (!this.IsSuspended)
            {
                base.RemoveItem(index);
            }
        }

        protected override void SetItem(int index, object item)
        {
            if (!this.IsSuspended)
            {
                base.SetItem(index, item);
            }
        }

        protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (!this.IsSuspended)
            {
                this.SuspendUpdate();
                base.OnCollectionChanged(e);
                this.ResumeUpdate();
            }
        }

        protected override void ClearItems()
        {
            if (!this.IsSuspended)
            {
                base.ClearItems();
            }
        }

        private bool CanInsertItem(object item)
        {
            return this.suspendLevel == 0 && this.AllowSelect && ((!this.AllowMultipleSelect && this.Count == 0) || this.AllowMultipleSelect);
        }
    }
}
