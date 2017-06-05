using System.Collections.ObjectModel;

namespace Telerik.UI.Xaml.Controls.Map
{
    /// <summary>
    /// Represents a strongly-typed collection of <see cref="MapLayer"/> instances.
    /// </summary>
    public class MapLayerCollection : Collection<MapLayer>
    {
        private RadMap map;

        internal MapLayerCollection(RadMap control)
        {
            this.map = control;
        }

        /// <summary>
        /// Retrieves the <see cref="MapLayer"/> instance from the specified Id.
        /// </summary>
        public MapLayer FindLayerById(int id)
        {
            foreach (var layer in this)
            {
                if (layer.Id == id)
                {
                    return layer;
                }
            }

            return null;
        }

        /// <summary>
        /// Inserts an element into the <see cref="T:System.Collections.ObjectModel.Collection`1" /> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
        /// <param name="item">The object to insert. The value can be null for reference types.</param>
        protected override void InsertItem(int index, MapLayer item)
        {
            base.InsertItem(index, item);

            this.map.OnPresenterAdded(item);
        }

        /// <summary>
        /// Removes the element at the specified index of the collection.
        /// </summary>
        protected override void RemoveItem(int index)
        {
            MapLayer presenter = this[index];

            base.RemoveItem(index);

            this.map.OnPresenterRemoved(presenter);
        }

        /// <summary>
        /// Removes all elements from the <see cref="T:System.Collections.ObjectModel.Collection`1"/>.
        /// </summary>
        protected override void ClearItems()
        {
            MapLayer[] presenters = new MapLayer[this.Items.Count];
            this.Items.CopyTo(presenters, 0);

            base.ClearItems();

            foreach (MapLayer presenter in presenters)
            {
                this.map.OnPresenterRemoved(presenter);
            }
        }

        /// <summary>
        /// Replaces the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to replace.</param>
        /// <param name="newPresenter">The new value for the element at the specified index. The value can be null for reference types.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than zero.
        /// -or-
        /// <paramref name="index"/> is greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count"/>.
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "1#", Justification = "Argument name is changed according the scope of the collection.")]
        protected override void SetItem(int index, MapLayer newPresenter)
        {
            MapLayer oldPresenter = this[index];

            base.SetItem(index, newPresenter);

            this.map.OnPresenterRemoved(oldPresenter);
            this.map.OnPresenterAdded(newPresenter);
        }
    }
}
