using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Grid.Model;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Represents a strongly typed collection of <see cref="DataGridColumn"/> objects.
    /// </summary>
    public sealed class DataGridColumnCollection : ObservableCollection<DataGridColumn>
    {
        private GridModel owner;

        private byte suspendLevel;

        internal DataGridColumnCollection(GridModel owner)
        {
            this.owner = owner;
        }

        internal bool IsSuspended
        {
            get
            {
                return this.suspendLevel > 0;
            }
        }

        /// <summary>
        /// Gets the <see cref="DataGridColumn"/> instance which <see cref="DataGridColumn.Name"/> value matches the provided one.
        /// </summary>
        /// <param name="name">The name of the column to search for.</param>
        public DataGridColumn this[string name]
        {
            get
            {
                foreach (var column in this)
                {
                    if (column.Name == name)
                    {
                        return column;
                    }
                }

                return null;
            }
        }

        internal void SuspendNotifications()
        {
            this.suspendLevel++;
        }

        internal void ResumeNotifications()
        {
            if (this.suspendLevel > 0)
            {
                this.suspendLevel--;
            }

            if (this.suspendLevel == 0)
            {
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        /// <summary>
        /// Sets the item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        protected override void SetItem(int index, DataGridColumn item)
        {
            var oldItem = this[index];
            this.DetachColumn(oldItem);
            this.AttachColumn(item);

            base.SetItem(index, item);
        }

        /// <summary>
        /// Inserts the item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        protected override void InsertItem(int index, DataGridColumn item)
        {
            this.AttachColumn(item);

            base.InsertItem(index, item);

            if (index != this.Count)
            {
                this.UpdateCollectionIndices();
            }
            else
            {
                item.CollectionIndex = index;
            }
        }

        /// <summary>
        /// Removes the item.
        /// </summary>
        /// <param name="index">The index.</param>
        protected override void RemoveItem(int index)
        {
            var column = this[index];
            this.DetachColumn(column);
            column.CollectionIndex = -1;

            base.RemoveItem(index);

            if (index != this.Count - 1)
            {
                this.UpdateCollectionIndices();
            }
        }

        /// <inheritdoc/>
        protected override void MoveItem(int oldIndex, int newIndex)
        {
            base.MoveItem(oldIndex, newIndex);

            if (Math.Abs(oldIndex - newIndex) > 1)
            {
                this.UpdateCollectionIndices();
            }
            else
            {
                this[oldIndex].CollectionIndex = oldIndex;
                this[newIndex].CollectionIndex = newIndex;
            }
        }

        /// <summary>
        /// Clears the items.
        /// </summary>
        protected override void ClearItems()
        {
            this.ClearColumns();

            base.ClearItems();
        }

        /// <summary>Raises the <see cref="E:System.Collections.ObjectModel.ObservableCollection`1.CollectionChanged"/>
        /// event with the provided arguments.</summary>
        /// <param name="e">Arguments of the event being raised.</param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!this.IsSuspended)
            {
                this.owner.OnColumnsCollectionChanged(e);
                base.OnCollectionChanged(e);
            }
        }

        private void UpdateCollectionIndices()
        {
            foreach (var item in this)
            {
                item.CollectionIndex = this.IndexOf(item);
            }
        }

        private void ClearColumns()
        {
            foreach (DataDescriptor descriptor in this.owner.ForEachDataDescriptor())
            {
                descriptor.DescriptorPeer = null;
            }
        }

        private void DetachColumn(DataGridColumn column)
        {
            foreach (DataDescriptor descriptor in this.owner.ForEachDataDescriptor())
            {
                if (descriptor.DescriptorPeer == column)
                {
                    descriptor.DescriptorPeer = null;
                }
            }

            column.Detach();
        }

        private void AttachColumn(DataGridColumn column)
        {
            column.Attach(this.owner);

            foreach (DataDescriptor descriptor in this.owner.ForEachDataDescriptor())
            {
                if (descriptor.IsAssociatedPeer(column))
                {
                    descriptor.UpdateAssociatedPeer(column);
                }
            }
        }
    }
}