using System;
using System.Collections;
using System.Collections.Generic;

namespace Telerik.Core
{
    /// <summary>
    /// Represents a collection of a <see cref="Node"/> instances.
    /// </summary>
    public class NodeCollection : IEnumerable<Node>
    {
        private List<Node> list;
        private Element owner;
        private bool suspendIndexShift;

        internal NodeCollection(Element owner)
        {
            this.owner = owner;
            this.list = new List<Node>();
        }

        /// <summary>
        /// Gets the <see cref="Element"/> instance that owns this collection.
        /// </summary>
        public Element Owner
        {
            get
            {
                return this.owner;
            }
        }

        /// <summary>
        /// Gets the count of all the items.
        /// </summary>
        public int Count
        {
            get
            {
                return this.list.Count;
            }
        }

        internal Node this[int index]
        {
            get
            {
                return this.list[index];
            }
        }

        /// <summary>
        /// Gets the <see cref="IEnumerator"/> instance that allows for traversing all the items.
        /// </summary>
        public IEnumerator<Node> GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (Node node in this.list)
            {
                yield return node;
            }
        }

        internal void Add(Node node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            this.InsertCore(this.list.Count, node);
        }

        internal void Remove(Node node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            int index = this.IndexOf(node);

            if (index == -1)
            {
                throw new ArgumentException("The provided node does exist in this collection.");
            }

            this.RemoveCore(index);
        }

        internal void RemoveAt(int index)
        {
            if (index < 0 || index >= this.list.Count)
            {
                throw new IndexOutOfRangeException(nameof(index));
            }

            this.RemoveCore(index);
        }

        internal void Clear()
        {
            this.suspendIndexShift = true;

            int count = this.list.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                this.RemoveCore(i);
            }

            this.suspendIndexShift = false;
        }

        internal void Insert(int index, Node node)
        {
            if (index < 0 || index > this.list.Count)
            {
                throw new IndexOutOfRangeException();
            }

            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            this.InsertCore(index, node);
        }

        /// <summary>
        /// Gets the index of the specified node within the collection.
        /// </summary>
        internal int IndexOf(Node node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (node.parent == this.owner)
            {
                return node.Index;
            }

            return -1;
        }

        /// <summary>
        /// Determines whether the specified node is contained within the collection.
        /// </summary>
        internal bool Contains(Node node)
        {
            return node != null && node.parent == this.owner;
        }

        internal virtual void InsertCore(int index, Node node)
        {
            if (!this.VerifyAddChild(node))
            {
                return;
            }

            this.list.Insert(index, node);
            node.Index = index;

            this.ShiftNodesIndexes(index, 1);

            this.owner.OnChildInserted(index, node);
        }

        internal virtual void RemoveCore(int index)
        {
            Node node = this.list[index];

            if (!this.VerifyRemoveChild(node))
            {
                return;
            }

            this.list.RemoveAt(index);
            node.Index = -1;
            node.CollectionIndex = -1;
            this.ShiftNodesIndexes(index - 1, -1);

            this.owner.OnChildRemoved(index, node);
        }

        private bool VerifyAddChild(Node node)
        {
            ModifyChildrenResult result = this.owner.CanAddChild(node);
            if (result == ModifyChildrenResult.Cancel)
            {
                return false;
            }

            if (result == ModifyChildrenResult.Refuse)
            {
                throw new ArgumentException("Specified node is not accepted by the element");
            }

            if (node.parent != null)
            {
                throw new InvalidOperationException("ChildNode is already parented by a ChartElement instance.");
            }

            return true;
        }
        
        private bool VerifyRemoveChild(Node node)
        {
            ModifyChildrenResult result = this.owner.CanRemoveChild(node);

            if (result == ModifyChildrenResult.Cancel)
            {
                return false;
            }

            if (result == ModifyChildrenResult.Refuse)
            {
                throw new ArgumentException("Specified node may not be removed from the element");
            }

            return true;
        }

        private void ShiftNodesIndexes(int index, int offset)
        {
            if (this.suspendIndexShift)
            {
                return;
            }

            int count = this.list.Count;

            for (int i = index + 1; i < count; i++)
            {
                this.list[i].Index += offset;
            }
        }
    }
}
