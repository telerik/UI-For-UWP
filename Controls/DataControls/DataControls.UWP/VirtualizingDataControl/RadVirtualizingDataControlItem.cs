using Telerik.Core.Data;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Represents the visual container that us used in the <see cref="RadVirtualizingDataControl"/>'s virtualization mechanism.
    /// </summary>
    public class RadVirtualizingDataControlItem : RadContentControl
    {
        // fields are internal for performance sake as they are accessed multiple times during balance
        internal double verticalOffsetCache = -1;

        internal double horizontalOffsetCache = -1;
        internal double height = 0;
        internal double width = 0;
        internal bool scheduledForBatchAnimation = false;

        internal IDataSourceItem associatedDataItem;
        internal RadVirtualizingDataControlItem previous;
        internal RadVirtualizingDataControlItem next;
        internal WrapVirtualizationStrategy.WrapRow wrapRow;

        internal object mostRecentContentCache;

        private RadVirtualizingDataControl owner;
        private ItemState currentItemState;

        private Orientation scrollingOrientation;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadVirtualizingDataControlItem" /> class.
        /// </summary>
        public RadVirtualizingDataControlItem()
        {
            this.DefaultStyleKey = typeof(RadVirtualizingDataControlItem);
        }

        /// <summary>
        /// Gets an implementation of the <see cref="IDataSourceItem"/> interface that wraps
        /// the data source item currently associated with this visual container.
        /// </summary>
        /// <value>The associated data item.</value>
        public IDataSourceItem AssociatedDataItem
        {
            get
            {
                return this.associatedDataItem;
            }
        }

        internal RadVirtualizingDataControl Owner
        {
            get
            {
                return this.owner;
            }
        }

        internal double CurrentOffset
        {
            get
            {
                return this.scrollingOrientation == Orientation.Horizontal ? this.horizontalOffsetCache : this.verticalOffsetCache;
            }
        }

        /// <summary>
        /// Gets the virtualization state of the item.
        /// </summary>
        internal ItemState ItemState
        {
            get
            {
                return this.currentItemState;
            }
        }

        internal virtual void PerformSpecialItemAction(object itemAction, object argument, ref object result)
        {
        }

        internal void UpdateItemState(ItemState newState)
        {
            this.currentItemState = newState;
        }

        internal virtual void BindToDataItem(IDataSourceItem item)
        {
            this.associatedDataItem = item;
            this.SetValue(FrameworkElement.DataContextProperty, item.Value);
        }

        internal virtual void ResetDataItemBinding()
        {
            this.associatedDataItem = null;
        }

        internal virtual void Attach(RadVirtualizingDataControl owner)
        {
            this.owner = owner;
            this.OnAttached();
        }

        internal virtual void Detach()
        {
            this.OnDetaching();
            this.owner = null;
            this.OnDetached();
        }

        internal void SetVerticalOffset(double offset)
        {
            Canvas.SetTop(this, offset);
            this.verticalOffsetCache = offset;
        }

        internal void SetScrollDirection(Orientation orientation)
        {
            this.scrollingOrientation = orientation;
        }

        internal void SetHorizontalOffset(double offset)
        {
            Canvas.SetLeft(this, offset);
            this.horizontalOffsetCache = offset;
        }

        internal void InvalidateVerticalOffset()
        {
            this.verticalOffsetCache = Canvas.GetTop(this);
        }

        internal void InvalidateHorizontalOffset()
        {
            this.horizontalOffsetCache = Canvas.GetLeft(this);
        }

        internal void InvalidateCachedSize()
        {
            this.InvalidateCachedSize(this.DesiredSize);
        }

        internal virtual void InvalidateCachedSize(Size newSize)
        {
            Size oldSize = new Size(this.width, this.height);
            this.width = newSize.Width;
            this.height = newSize.Height;

            if (this.currentItemState == ItemState.Realized && this.owner.virtualizationStrategy.IsItemSizeChangeValid(oldSize, newSize))
            {
                this.owner.OnContainerSizeChanged(this, newSize, oldSize);
            }
        }

        /// <summary>
        /// Called when the value of the <see cref="P:System.Windows.Controls.ContentControl.Content"/> property changes.
        /// </summary>
        /// <param name="oldContent">The old value of the <see cref="P:System.Windows.Controls.ContentControl.Content"/> property.</param>
        /// <param name="newContent">The new value of the <see cref="P:System.Windows.Controls.ContentControl.Content"/> property.</param>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            this.mostRecentContentCache = newContent;
        }

        /// <summary>
        /// Provides the behavior for the Arrange pass of Silverlight layout. Classes can override this method to define their own Arrange pass behavior.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
        /// <returns>
        /// The actual size that is used after the element is arranged in layout.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.currentItemState != ItemState.Realized)
            {
                return base.ArrangeOverride(finalSize);
            }

            Size newSize = finalSize;
            Size prevSize = new Size(this.width, this.height);
            bool shouldFireSizeChanged = this.owner.virtualizationStrategy.IsItemSizeChangeValid(prevSize, newSize);
            this.height = newSize.Height;
            this.width = newSize.Width;

            if (this.owner != null && shouldFireSizeChanged)
            {
                this.owner.OnContainerSizeChanged(this, newSize, prevSize);
            }

            return base.ArrangeOverride(finalSize);
        }

        /// <summary>
        /// Occurs when the item is successfully attached to a <see cref="RadVirtualizingDataControl"/> instance.
        /// </summary>
        protected virtual void OnAttached()
        {
        }

        /// <summary>
        /// Occurs when the item is about to be detached from its owning <see cref="RadVirtualizingDataControl"/> instance.
        /// </summary>
        protected virtual void OnDetaching()
        {
        }

        /// <summary>
        /// Occurs when the item has been detached from its owning <see cref="RadVirtualizingDataControl"/> instance.
        /// </summary>
        protected virtual void OnDetached()
        {
        }
    }
}
