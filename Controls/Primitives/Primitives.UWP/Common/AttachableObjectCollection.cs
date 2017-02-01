using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Represents a collection of <see cref="AttachableObject{TOwner}" /> instances.
    /// </summary>
    /// <typeparam name="TOwner">Specifies the type of the owner of the items.</typeparam>
    /// <typeparam name="TObject">Specifies the type of the attachable objects.</typeparam>
    public class AttachableObjectCollection<TOwner, TObject> : ObservableCollection<TObject> 
        where TObject : AttachableObject<TOwner> 
        where TOwner : class
    {
        private TOwner owner;

        /// <summary>
        /// Initializes a new instance of the <see cref="AttachableObjectCollection{TOwner, TObject}" /> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        internal AttachableObjectCollection(TOwner owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Removes the item.
        /// </summary>
        /// <param name="index">The index.</param>
        protected override void RemoveItem(int index)
        {
            var item = this[index];
            item.Owner = null;

            base.RemoveItem(index);
        }

        /// <summary>
        /// Sets the item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        protected override void SetItem(int index, TObject item)
        {
            var oldItem = this[index];
            oldItem.Owner = null;

            base.SetItem(index, item);

            item.Owner = this.owner;
        }

        /// <summary>
        /// Inserts the item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        protected override void InsertItem(int index, TObject item)
        {
            base.InsertItem(index, item);

            item.Owner = this.owner;
        }

        /// <summary>
        /// Clears the items.
        /// </summary>
        protected override void ClearItems()
        {
            foreach (var item in this)
            {
                item.Owner = null;
            }

            base.ClearItems();
        }
    }
}
