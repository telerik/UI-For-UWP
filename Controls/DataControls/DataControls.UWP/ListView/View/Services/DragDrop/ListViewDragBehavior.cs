using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Data.ListView
{
    internal class ListViewDragBehavior : AttachableObject<RadListView>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListViewDragBehavior" /> class.
        /// </summary>
        public ListViewDragBehavior()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListViewDragBehavior" /> class.
        /// </summary>
        /// <param name="owner">Behavior owner.</param>
        internal ListViewDragBehavior(RadListView owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Determines whether drag operation can star with the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Whether drag operation can start.</returns>
        public virtual bool CanStartDrag(RadListViewItem item)
        {
            if (item == null || this.Owner == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the drag visual for specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public virtual FrameworkElement GetDragVisual(RadListViewItem item)
        {
            return item;
        }

        /// <summary>
        /// Called when drag drop operation completed.
        /// </summary>
        /// <param name="item">The source item being dragged.</param>
        /// <param name="dragSuccessful">Determines whether current drag operation completed successfully.</param>
        public virtual void OnDragDropCompleted(RadListViewItem item, bool dragSuccessful)
        {
        }

        internal void OnDragStarted(RadListViewItem item)
        {
        }

        internal FrameworkElement GetReorderVisual(RadListViewItem radListViewItem)
        {
            return radListViewItem.dragVisual;
        }

        internal void OnReorderStarted(object dataItem)
        {
        }

        internal bool CanReorder(object sourceData, object destinationData)
        {
            return true;
        }
    }
}
