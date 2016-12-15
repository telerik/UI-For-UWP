using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Telerik.Data.Core
{
    internal class Group : IGroup, IEquatable<Group>
    {
        public const string GrandTotalName = "Grand Total";
        private static readonly int RandomHashCode = new object().GetHashCode();

        private List<object> itemsList;
        private Dictionary<object, Group> groupsByName;
        private int? hash;
        private string caption;

        internal Group(object name)
        {
            this.Name = name ?? NullValue.Instance;
            this.caption = System.Convert.ToString(this.Name, System.Globalization.CultureInfo.InvariantCulture);
            this.itemsList = new List<object>();
        }

        public bool IsBottomLevel
        {
            get
            {
                // NOTE: Not bullet proof but will suffice for now.
                return this.groupsByName == null || this.groupsByName.Count == 0;
            }
        }

        public GroupType Type
        {
            get
            {
                return GroupType.Subheading;
            }
        }

        public IGroup Parent
        {
            get
            {
                return this.InternalParent;
            }
        }

        public object Name
        {
            get;
            private set;
        }

        public IReadOnlyList<object> Items
        {
            get
            {
                return this.itemsList;
            }
        }

        public bool HasItems
        {
            get
            {
                return this.itemsList.Count > 0;
            }
        }

        public bool HasParent
        {
            get
            {
                return this.Parent != null;
            }
        }

        public int Level
        {
            get
            {
                int level = 0;
                var parent = this.Parent;
                while (parent != null)
                {
                    level++;
                    parent = parent.Parent;
                }

                return level;
            }
        }

        internal int Hash
        {
            get
            {
                if (!this.hash.HasValue)
                {
                    this.hash = ((this.Parent != null ? this.Parent.GetHashCode() : RandomHashCode) * 104743) + (this.Name.GetHashCode() * 104759);
                }

                return this.hash.Value;
            }
        }

        internal Group InternalParent
        {
            get;
            set;
        }

        private Dictionary<object, Group> GroupsByName
        {
            get
            {
                if (this.groupsByName == null)
                {
                    this.groupsByName = new Dictionary<object, Group>();
                }

                return this.groupsByName;
            }
        }

        public override int GetHashCode()
        {
            return this.Hash;
        }

        public override bool Equals(object obj)
        {
            Group other = obj as Group;
            if (other != null)
            {
                return this.Equals(other);
            }

            return false;
        }

        public bool Equals(Group other)
        {
            if (this == other)
            {
                return true;
            }

            if (other == null)
            {
                return false;
            }

            if (!object.Equals(this.Name, other.Name))
            {
                return false;
            }

            if (this.Parent == null)
            {
                return other.Parent == null;
            }

            return this.Parent.Equals(other.Parent);
        }

        public override string ToString()
        {
            return this.caption;
        }

        /// <summary>
        /// This method sets the source or root group.
        /// </summary>
        /// <param name="items">The items list.</param>
        internal void SetItems(List<object> items)
        {
            this.itemsList = items;
        }

        /// <summary>
        /// Used by Tests only. This method sets the source or root group.
        /// </summary>
        /// <param name="groups">The groups list.</param>
        internal void SetGroups(List<Group> groups)
        {
            foreach (var group in groups)
            {
                this.AddGroup(group);
            }
        }

        internal void SortSubGroups(IComparer<object> comparer)
        {
            if (this.HasItems)
            {
                this.itemsList.Sort(comparer);
            }
        }

        /// <summary>
        /// This method should be used only after TryGetGroup returns false.
        /// </summary>
        /// <param name="groupName">The groupName.</param>
        /// <param name="groupFactory">The factory that will create new groups.</param>
        /// <param name="sortComparer">The groupName comparer.</param>
        /// <returns>Returns the index of the newly added group.</returns>
        internal int AddGroupByName(object groupName, IGroupFactory groupFactory, IComparer<object> sortComparer)
        {
            if (groupName == null)
            {
                groupName = NullValue.Instance;
            }

            System.Diagnostics.Debug.Assert(!this.GroupsByName.ContainsKey(groupName), "This method should not be called for existing groupName!");

            Group group;
            if (!this.GroupsByName.TryGetValue(groupName, out group))
            {
                group = groupFactory.CreateGroup(groupName);
                group.InternalParent = this;
                this.GroupsByName.Add(groupName, group);
                return this.InsertItem(-1, group, sortComparer);
            }
            else
            {
                return this.itemsList.IndexOf(group);
            }
        }

        /// <summary>
        /// This method is used in unit tests only.
        /// </summary>
        /// <param name="group">The group that should be added.</param>
        internal void AddGroup(Group group)
        {
            if (group.Parent != null)
            {
                throw new InvalidOperationException("Group is already a child of another Group.");
            }

            group.InternalParent = this;
            this.GroupsByName.Add(group.Name, group);
            this.InsertItem(-1, group, null);
        }

        internal Group CreateGroupByName(object groupName, IGroupFactory groupFactor)
        {
            if (groupName == null)
            {
                groupName = NullValue.Instance;
            }

            Group group;
            if (!this.GroupsByName.TryGetValue(groupName, out group))
            {
                group = groupFactor.CreateGroup(groupName);
                group.InternalParent = this;
                this.GroupsByName.Add(groupName, group);
                this.InsertItem(-1, group, null);
            }

            return group;
        }

        internal bool TryGetGroup(object groupNameKey, out Group group)
        {
            if (this.groupsByName != null)
            {
                if (groupNameKey == null)
                {
                    groupNameKey = NullValue.Instance;
                }

                return this.groupsByName.TryGetValue(groupNameKey, out group);
            }

            group = null;
            return false;
        }

        internal int InsertItem(int index, object item, IComparer<object> sortComparer)
        {
            int adjustedIndex = index;
            if (sortComparer != null)
            {
                adjustedIndex = this.GetItemIndex(item, sortComparer, false);
            }
            else if (!this.HasItems || index < 0 || index > this.itemsList.Count)
            {
                adjustedIndex = this.itemsList.Count;
            }

            this.itemsList.Insert(adjustedIndex, item);
            return adjustedIndex;
        }

        internal int RemoveItem(int index, object item, IComparer<object> sortComparer)
        {
            if (this.HasItems)
            {
                // TODO: RaiseCollectionChanged here
                int itemIndex = index;
                if (sortComparer != null)
                {
                    itemIndex = this.GetItemIndex(item, sortComparer, true);
                }
                else if (index < 0 || index >= this.itemsList.Count)
                {
                    itemIndex = this.itemsList.IndexOf(item);
                }

                if (itemIndex >= 0)
                {
                    this.RemoveAt(itemIndex);
                    return itemIndex;
                }
            }

            return -1;
        }

        /// <summary>
        /// Returns the index of the item in this group.
        /// NOTE that sortComparer should be used only when item property on which was sorted has NOT changed.
        /// </summary>
        /// <param name="item">The item which we are searching.</param>
        /// <param name="sortComparer">The itemComparer. Should be used only when item property which was sorted hasn't changed.</param>
        /// <returns>The index of the item in this group or negative number if not found.</returns>
        internal int IndexOf(object item, IComparer<object> sortComparer)
        {
            if (sortComparer != null)
            {
                int index = this.itemsList.BinarySearch(item, sortComparer);
                if (index >= 0)
                {
                    return this.FindItemByReference(item, sortComparer, index);
                }
            }
            else
            {
                return this.itemsList.IndexOf(item);
            }

            return -1;
        }

        private void AddItem(object item)
        {
            // TODO: RaiseCollectionChanged here
            this.itemsList.Add(item);
        }

        private void RemoveGroup(Group group)
        {
            if (group.Parent != this)
            {
                throw new InvalidOperationException("Group is not a child this Group.");
            }

            group.InternalParent = null;
            this.GroupsByName.Remove(group.Name);
        }

        private int GetItemIndex(object item, IComparer<object> sortComparer, bool remove)
        {
            int adjustedIndex = this.itemsList.BinarySearch(item, sortComparer);
            if (remove)
            {
                if (adjustedIndex >= 0)
                {
                    adjustedIndex = this.FindItemByReference(item, sortComparer, adjustedIndex);
                }
                else
                {
                    adjustedIndex = this.itemsList.IndexOf(item);
                }
            }
            else if (adjustedIndex < 0)
            {
                adjustedIndex = ~adjustedIndex;
            }

            return adjustedIndex;
        }

        private int FindItemByReference(object item, IComparer<object> sortComparer, int firstMatch)
        {
            // collision detection in case we have more than one item with the same key
            bool found = false;
            int comparison;
            object currentItem;

            // we have found the first match already, traverse the items list back and forward to verify that the item at the found index reference equals with the removed one
            // search back
            for (int i = firstMatch; i >= 0; i--)
            {
                currentItem = this.itemsList[i];
                comparison = sortComparer.Compare(currentItem, item);
                if (comparison != 0)
                {
                    break;
                }

                if (object.ReferenceEquals(item, currentItem))
                {
                    found = true;
                    firstMatch = i;
                    break;
                }
            }

            if (!found)
            {
                // search forward
                for (int i = firstMatch + 1; i < this.itemsList.Count; i++)
                {
                    currentItem = this.itemsList[i];
                    comparison = sortComparer.Compare(currentItem, item);
                    if (comparison != 0)
                    {
                        break;
                    }

                    if (object.ReferenceEquals(item, currentItem))
                    {
                        firstMatch = i;
                        found = true;
                        break;
                    }
                }
            }

            if (!found)
            {
                Debug.Assert(false, "Failed to find the removed item.");
                return -1;
            }

            return firstMatch;
        }

        private void RemoveAt(int index)
        {
            Group group = this.itemsList[index] as Group;
            if (group != null)
            {
                this.RemoveGroup(group);
            }

            this.itemsList.RemoveAt(index);
        }
    }
}