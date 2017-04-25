using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Defines a collection of objects of type <see cref="DataDescriptor"/>.
    /// </summary>
    /// <typeparam name="T">The type of the T.</typeparam>
    public abstract class DataDescriptorCollection<T> : ObservableCollection<T> where T : DataDescriptor
    {
        private IList descriptionCollection; // this is the DataEngine collection that needs to be synchronized

        private IDataDescriptorsHost host;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataDescriptorCollection{T}" /> class.
        /// </summary>
        internal DataDescriptorCollection(IDataDescriptorsHost host)
        {
            this.host = host;
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

        internal void RemoveDescriptor(T descriptor)
        {
            descriptor.PropertyChanged -= this.OnItemPropertyChanged;
            descriptor.Detach();
            this.RemoveDescription(descriptor);
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
            this.host.OnDataDescriptorPropertyChanged(sender as DataDescriptor);
        }

        private void AddDescriptor(T descriptor, int index)
        {
            descriptor.PropertyChanged += this.OnItemPropertyChanged;
            descriptor.Attach(this.host);
            this.AddDescription(descriptor, index);
        }

        private void RemoveDescription(T oldItem)
        {
            if (this.descriptionCollection == null || oldItem.EngineDescription == null)
            {
                return;
            }

            foreach (DescriptionBase description in this.descriptionCollection)
            {
                if (oldItem.EngineDescription.Equals(description))
                {
                    this.descriptionCollection.Remove(description);
                    break;
                }
            }
        }

        private void AddDescription(T newItem, int index)
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

            var dataProvider = this.host.CurrentDataProvider;

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