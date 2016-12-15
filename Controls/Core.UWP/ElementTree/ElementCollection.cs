using System;
using System.Collections.ObjectModel;

namespace Telerik.Core
{
    /// <summary>
    /// Represents a typed collection of <see cref="Node"/> instances. For example an element instance may aggregate two or more typed collections of different nodes.
    /// </summary>
    /// <typeparam name="T">Must be a <see cref="Node"/>.</typeparam>
    public class ElementCollection<T> : Collection<T> where T : Node
    {
        private Element owner;

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementCollection&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public ElementCollection(Element owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Gets a value indicating whether the collection should cache the index of each node added.
        /// This will improve performance if many IndexOf calls will be performed upon this collection.
        /// </summary>
        protected virtual bool ShouldCacheIndexes
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Inserts the item at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);

            this.owner.children.Add(item);

            if (this.ShouldCacheIndexes)
            {
                item.CollectionIndex = index;
                this.ShiftNodesIndexes(index, 1);
            }
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        protected override void ClearItems()
        {
            // start from the end of the collection -> better performance since indexes in the owner children's collection will not be shifted
            for (int i = this.Count - 1; i >= 0; i--)
            {
                this.owner.children.RemoveAt(this.Items[i].Index);
            }

            base.ClearItems();
        }

        /// <summary>
        /// Removes the item at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        protected override void RemoveItem(int index)
        {
            Node node = this[index];

            base.RemoveItem(index);

            this.owner.children.Remove(node);

            if (this.ShouldCacheIndexes)
            {
                this.ShiftNodesIndexes(index - 1, -1);
            }
        }

        private void ShiftNodesIndexes(int index, int offset)
        {
            int count = this.Count;

            for (int i = index + 1; i < count; i++)
            {
                this[i].CollectionIndex += offset;
            }
        }
    }
}