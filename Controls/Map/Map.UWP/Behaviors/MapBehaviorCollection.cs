using System;
using System.Collections.ObjectModel;

namespace Telerik.UI.Xaml.Controls.Map
{
    /// <summary>
    /// This collection contains the behaviors for RadMap.
    /// </summary>
    public class MapBehaviorCollection : Collection<MapBehavior>
    {
        private RadMap owner;

        internal MapBehaviorCollection(RadMap owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Inserts a behavior at the specified index.
        /// </summary>
        /// <param name="index">The index at which to insert the behavior.</param>
        /// <param name="item">The behavior to insert.</param>
        protected override void InsertItem(int index, MapBehavior item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (index < 0 || index > this.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            // attach the new behavior
            item.Attach(this.owner);

            base.InsertItem(index, item);
        }

        /// <summary>
        /// Removes a behavior at the specified index.
        /// </summary>
        /// <param name="index">The index at which a behavior will be removed.</param>
        protected override void RemoveItem(int index)
        {
            this.VerifyIndex(index);

            // detach the removed behavior
            this[index].Detach();

            base.RemoveItem(index);
        }

        /// <summary>
        /// Replaces a behavior at the specified index with the specified item.
        /// </summary>
        /// <param name="index">The index of the behavior to replace.</param>
        /// <param name="item">The new behavior.</param>
        protected override void SetItem(int index, MapBehavior item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            this.VerifyIndex(index);

            // detach replaced behavior
            this[index].Detach();

            // attach new behavior
            item.Attach(this.owner);

            base.SetItem(index, item);
        }

        /// <summary>
        /// Removes all behaviors from RadMap.
        /// </summary>
        protected override void ClearItems()
        {
            // detach existing behaviors first
            foreach (MapBehavior behavior in this)
            {
                behavior.Detach();
            }

            base.ClearItems();
        }

        private void VerifyIndex(int index)
        {
            if (index < 0 || index >= this.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }
    }
}
