namespace Telerik.Core.Data
{
    internal partial class RadListSource
    {
        /// <summary>
        /// Gets the first item in the collection.
        /// </summary>
        public IDataSourceItem GetFirstItem()
        {
            if (this.items.Count > 0)
            {
                return this.items[0];
            }

            return null;
        }

        /// <summary>
        /// Gets the item that is right after the specified one.
        /// </summary>
        public IDataSourceItem GetItemAfter(IDataSourceItem item)
        {
            if (item == null)
            {
                return null;
            }

            return item.Next;
        }

        /// <summary>
        /// Gets the item that is just before the specified one.
        /// </summary>
        public IDataSourceItem GetItemBefore(IDataSourceItem item)
        {
            if (item == null)
            {
                return null;
            }

            return item.Previous;
        }

        /// <summary>
        /// Gets the last item in the collection.
        /// </summary>
        public virtual IDataSourceItem GetLastItem()
        {
            if (this.items.Count > 0)
            {
                return this.items[this.items.Count - 1];
            }

            return null;
        }

        /// <summary>
        /// Gets the item at the specified index.
        /// </summary>
        public virtual IDataSourceItem GetItemAt(int index)
        {
            if (index >= 0 && index < this.items.Count)
            {
                return this.items[index];
            }

            return null;
        }

        /// <summary>
        /// Retrieves the index of the specified item within the collection.
        /// </summary>
        public int IndexOf(IDataSourceItem item)
        {
            if (item == null)
            {
                return -1;
            }

            return item.Index;
        }
    }
}