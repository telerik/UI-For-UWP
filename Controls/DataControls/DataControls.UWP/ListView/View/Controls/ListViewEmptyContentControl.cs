using Windows.Foundation;

namespace Telerik.UI.Xaml.Controls.Data.ListView.Primitives
{
    /// <summary>
    /// Represents a ListViewEmptyContentControl control.
    /// </summary>
    public class ListViewEmptyContentControl : RadContentControl, IArrangeChild
    {
        private Rect layoutSlot;

        /// <summary>
        /// Gets the layout slot of the <see cref="ListViewEmptyContentControl"/>.
        /// </summary>
        public Rect LayoutSlot
        {
            get
            {
                return this.layoutSlot;
            }
        }

        /// <summary>
        /// Tries to invalidate the owner.
        /// </summary>
        public bool TryInvalidateOwner()
        {
            return false;
        }

        /// <inheritdoc/>
        protected override Size ArrangeOverride(Size finalSize)
        {
            this.layoutSlot = new Rect(new Point(0, 0), finalSize);
            return base.ArrangeOverride(finalSize);
        }
    }
}
