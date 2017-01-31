using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal class SelectedItemCollection : ObservableCollection<object>
    {
        private int suspendLevel;
        private bool allowMultipleSelect;

        public bool AllowMultipleSelect
        {
            get
            {
                return this.allowMultipleSelect;
            }
            set
            {
                this.allowMultipleSelect = value;
            }
        }

        internal DataGridSelectionUnit SelectionUnit { get; set; }

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
                base.OnCollectionChanged(e);
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
            return this.suspendLevel == 0 &&
                ((!this.AllowMultipleSelect && this.Count == 0) || this.AllowMultipleSelect) &&
                (this.SelectionUnit == DataGridSelectionUnit.Row ||
                (this.SelectionUnit == DataGridSelectionUnit.Cell && item is DataGridCellInfo));
        }
    }
}
