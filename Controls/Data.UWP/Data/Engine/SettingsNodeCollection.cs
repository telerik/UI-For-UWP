using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Telerik.Data.Core
{
    /// <summary>
    /// A Collection of <see cref="SettingsNode"/> items. Tunnels events from the items to the <see cref="Parent"/>.
    /// </summary>
    /// <typeparam name="T">A class that inherits the <see cref="SettingsNode"/>.</typeparam>
    internal class SettingsNodeCollection<T> : Collection<T>, IList<T>, IList, IEnumerable<T>, IEnumerable
        where T : SettingsNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsNodeCollection{T}"/> class.
        /// </summary>
        /// <param name="parent">The parent <see cref="SettingsNode"/>.</param>
        public SettingsNodeCollection(SettingsNode parent)
        {
            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }

            this.Parent = parent;
        }

        /// <summary>
        /// Gets the parent <see cref="SettingsNode"/>.
        /// </summary>
        public SettingsNode Parent { get; private set; }

        internal void CloneItemsFrom(SettingsNodeCollection<T> original)
        {
            this.Clear();
            foreach (var item in original)
            {
                this.Add((T)item.Clone());
            }
        }

        /// <inheritdoc />
        protected override void SetItem(int index, T item)
        {
            this.Parent.RemoveSettingsChild(this[index]);
            this.Parent.AddSettingsChild(item);
            base.SetItem(index, item);
        }

        /// <inheritdoc />
        protected override void RemoveItem(int index)
        {
            this.Parent.RemoveSettingsChild(this[index]);
            base.RemoveItem(index);
        }

        /// <inheritdoc />
        protected override void InsertItem(int index, T item)
        {
            this.Parent.AddSettingsChild(item);
            base.InsertItem(index, item);
        }

        /// <inheritdoc />
        protected override void ClearItems()
        {
            foreach (var item in this)
            {
                this.Parent.RemoveSettingsChild(item);
            }

            base.ClearItems();
        }

        /// <summary>
        /// Notifies the Parent <see cref="SettingsNode"/> for a change.
        /// </summary>
        /// <param name="settingsEventArgs">The <see cref="SettingsChangedEventArgs" /> that contains the event data.</param>
        protected void NotifyChange(SettingsChangedEventArgs settingsEventArgs)
        {
            this.Parent.NotifyChange(settingsEventArgs);
        }
    }
}
