using System.Collections.ObjectModel;

namespace Telerik.UI.Xaml.Controls.DataVisualization
{
    /// <summary>
    /// This collection is used insider RadBulletGraph and contains the additional comparative measures.
    /// </summary>
    /// <typeparam name="T">The type of the measures contained in the collection.</typeparam>
    public class AdditionalMeasuresCollection<T> : ObservableCollection<T> where T : BulletGraphMeasureBase
    {
        private RadBulletGraph owner;

        internal AdditionalMeasuresCollection(RadBulletGraph owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Inserts an item into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert.</param>
        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);

            this.owner.InsertMeasure(index, item);
        }

        /// <summary>
        /// Removes the item at the specified index from the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        protected override void RemoveItem(int index)
        {
            this.owner.RemoveMeasure(this[index]);
            base.RemoveItem(index);
        }

        /// <summary>
        /// Replaces the item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to replace.</param>
        /// <param name="item">The new value for the item at the specified index.</param>
        /// <exception cref="System.InvalidOperationException">
        /// The method is being called in a System.Collections.ObjectModel.ObservableCollection.PropertyChanged
        /// or System.Collections.ObjectModel.ObservableCollection.CollectionChanged
        /// event handler.
        /// </exception>
        protected override void SetItem(int index, T item)
        {
            this.owner.SetMeasure(item, this[index]);
            base.SetItem(index, item);
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// The method is being called in a System.Collections.ObjectModel.ObservableCollection.PropertyChanged
        /// or System.Collections.ObjectModel.ObservableCollection.CollectionChanged
        /// event handler.
        /// </exception>
        protected override void ClearItems()
        {
            foreach (T measure in this)
            {
                this.owner.RemoveMeasure(measure);
            }
            base.ClearItems();
        }
    }
}
