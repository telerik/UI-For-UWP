using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Grid.Model;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Represents a collection of <see cref="DataDescriptor"/> objects.
    /// </summary>
    public abstract class DataDescriptorCollection<T> : ObservableCollection<T> where T : DataDescriptor
    {
        private GridModel owner;
        private IList descriptionCollection; // this is the DataEngine collection that needs to be synchronized

        /// <summary>
        /// Initializes a new instance of the <see cref="DataDescriptorCollection{T}" /> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        internal DataDescriptorCollection(GridModel owner)
        {
            this.owner = owner;
        }

        internal IList DescriptionCollection
        {
            get
            {
                return this.descriptionCollection;
            }
            set
            {
                if (this.descriptionCollection != null) 
                {
                    this.descriptionCollection.Clear();
                }
                this.descriptionCollection = value;
                this.AddDescritions();
            }
        }

        /// <summary>
        /// Sets the item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        protected override void SetItem(int index, T item)
        {
            var oldItem = this[index];

            this.RemoveDescriptor(oldItem);
            this.AddDescriptor(item, index);

            base.SetItem(index, item);
        }

        /// <summary>
        /// Inserts the item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        protected override void InsertItem(int index, T item)
        {
            this.AddDescriptor(item, index);
            base.InsertItem(index, item);
        }

        /// <summary>
        /// Removes the item.
        /// </summary>
        /// <param name="index">The index.</param>
        protected override void RemoveItem(int index)
        {
            var item = this[index];
            this.RemoveDescriptor(item);

            base.RemoveItem(index);
        }

        /// <summary>
        /// Clears the items.
        /// </summary>
        protected override void ClearItems()
        {
            if (this.Count == 0)
            {
                return;
            }

            foreach (var descriptor in this)
            {
                this.RemoveDescriptor(descriptor);
            }

            this.ClearDescriptions();

            base.ClearItems();
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.owner.OnDataDescriptorPropertyChanged(sender as DataDescriptor);
        }

        private void RemoveDescriptor(DataDescriptor descriptor)
        {
            descriptor.PropertyChanged -= this.OnItemPropertyChanged;
            descriptor.Detach();
            this.RemoveDescription(descriptor);
        }

        private void AddDescriptor(DataDescriptor descriptor, int index)
        {
            descriptor.PropertyChanged += this.OnItemPropertyChanged;
            descriptor.Attach(this.owner);
            this.AddDescription(descriptor, index);
        }

        private void RemoveDescription(DataDescriptor oldItem)
        {
            if (this.descriptionCollection == null)
            {
                return;
            }

            foreach (DescriptionBase description in this.descriptionCollection)
            {
                if (oldItem.Equals(description))
                {
                    this.descriptionCollection.Remove(description);
                    break;
                }
            }
        }

        private void AddDescription(DataDescriptor newItem, int index)
        {
            if (this.descriptionCollection == null)
            {
                return;
            }

            if (index >= 0)
            {
                this.descriptionCollection.Insert(index, newItem.EngineDescription);
            }
            else
            {
                this.descriptionCollection.Add(newItem.EngineDescription);
            }
        }

        private void AddDescritions()
        {
            foreach (var descriptor in this)
            {
                this.AddDescription(descriptor, -1);
            }
        }

        private void ResetDescriptions()
        {
            if (this.descriptionCollection == null)
            {
                return;
            }

            var dataProvider = this.owner.CurrentDataProvider;

            IDisposable deferRefresh = null;
            if (dataProvider != null)
            {
                deferRefresh = dataProvider.DeferRefresh();
            }

            this.descriptionCollection.Clear();

            foreach (var descriptor in this)
            {
                this.descriptionCollection.Add(descriptor.EngineDescription);
            }

            if (deferRefresh != null)
            {
                deferRefresh.Dispose();
            }
        }

        private void ClearDescriptions()
        {
            if (this.descriptionCollection == null)
            {
                return;
            }

            this.descriptionCollection.Clear();
        }
    }
}
