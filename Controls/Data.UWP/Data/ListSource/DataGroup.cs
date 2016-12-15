using System;
using System.Collections;

namespace Telerik.Core.Data
{
    /// <summary>
    /// Represents a group of items, created upon a grouping data operation.
    /// </summary>
    public class DataGroup : IEnumerable
    {
        private IEnumerable items;
        private object key;
        private bool hasChildGroups;
        private string formatString;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGroup"/> class.
        /// </summary>
        /// <param name="key">The key of the group.</param>
        /// <param name="items">The child items of the group.</param>
        public DataGroup(object key, IEnumerable items)
        {
            this.key = key;
            this.InitItems(items);
        }

        /// <summary>
        /// Gets the string used to format the display text for this group.
        /// </summary>
        public string FormatString
        {
            get
            {
                return this.formatString;
            }
            internal set
            {
                this.formatString = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this group has child groups.
        /// </summary>
        public bool HasChildGroups
        {
            get
            {
                return this.hasChildGroups;
            }
        }

        /// <summary>
        /// Gets the key value for this group.
        /// </summary>
        public object Key
        {
            get
            {
                return this.key;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="IEnumerable"/> representation of all the child items.
        /// </summary>
        public IEnumerable Items
        {
            get
            {
                return this.items;
            }
            protected internal set
            {
                this.InitItems(value);
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            DataGroup that = obj as DataGroup;
            return that != null && object.Equals(this.key, that.key);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return this.key.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.formatString))
            {
                return this.key.ToString();
            }

            return string.Format(this.formatString, this.key);
        }

        /// <summary>
        /// Gets the <see cref="IEnumerator"/> instance that allows iterating through all child items.
        /// </summary>
        public IEnumerator GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        private void InitItems(IEnumerable items)
        {
            this.items = items;
            this.hasChildGroups = false;

            IEnumerator en = items.GetEnumerator();
            if (en.MoveNext())
            {
                this.hasChildGroups = en.Current is DataGroup;
            }
        }
    }
}