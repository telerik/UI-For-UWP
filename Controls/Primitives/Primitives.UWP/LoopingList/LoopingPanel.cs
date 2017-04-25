using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.UI.Xaml.Controls.Primitives.LoopingList
{
    /// <summary>
    /// Represents a light-weight virtualized stack panel with vertical orientation with endless looping among logical items.
    /// </summary>
    public class LoopingPanel : Panel
    {
        internal double visualOffset;
        private const int OffScreenItemCount = 2;

        /// <summary>
        /// Used internally to animate the vertical offset of the panel.
        /// </summary>
        private static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register(nameof(VerticalOffset), typeof(double), typeof(LoopingPanel), new PropertyMetadata(0d, OnVerticalOffsetChanged));

        private int visualCount;
        private double availableLength;
        private List<int> visualIndexChain;
        private double itemLength;
        private int logicalCount;
        private bool isCentered;
        private bool centeringDuringLayout;
        private LoopingListItemSnapPosition centeredItemSnapPosition;
        private bool isLoopingEnabled;
        private int topLogicalIndex;
        private Orientation orientation;
        private Size size;
        private double snapCorrectionOffset = 0;
        private RadLoopingList owner;
        private LoopingPanelScrollState scrollState;
        private double visibleItemsParts;
        private bool isLoaded;
        private bool animating;
        private Storyboard offsetStoryboard;
        private DoubleAnimation offsetAnimation;
        private List<LoopingListItem> items;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoopingPanel"/> class.
        /// </summary>
        public LoopingPanel()
        {
            this.SizeChanged += this.OnSizeChanged;
            this.Loaded += this.OnLoaded;
            this.Unloaded += this.OnUnloaded;

            this.isLoopingEnabled = true;
            this.visualIndexChain = new List<int>(this.visualCount);

            this.CreateOffsetAnimation();

            this.items = new List<LoopingListItem>();
        }

        /// <summary>
        /// Gets or sets the current scrolling state of the panel.
        /// </summary>
        internal LoopingPanelScrollState ScrollState
        {
            get
            {
                return this.scrollState;
            }
            set
            {
                this.scrollState = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether looping is enabled. That is to allow infinite scrolling of the current logical wheel.
        /// </summary>
        internal bool IsLoopingEnabled
        {
            get
            {
                return this.isLoopingEnabled;
            }
            set
            {
                if (this.isLoopingEnabled == value)
                {
                    return;
                }

                this.isLoopingEnabled = value;

                this.OnIsLoopingEnabledChanged();
            }
        }

        /// <summary>
        /// Gets the visual index chain. Exposed for test purposes.
        /// </summary>
        internal List<int> VisualIndexChain
        {
            get
            {
                return this.visualIndexChain;
            }
        }

        /// <summary>
        /// Gets or sets the position of the centered item relatively to the viewport's starting edge
        /// if the <see cref="IsCentered"/> property is set to true.
        /// </summary>
        internal LoopingListItemSnapPosition CenteredItemSnapPosition
        {
            get
            {
                return this.centeredItemSnapPosition;
            }
            set
            {
                if (value != this.centeredItemSnapPosition)
                {
                    this.centeredItemSnapPosition = value;
                    this.OnCenteredItemSnapPositionChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a double value that represents the
        /// offset correction applied when the selected item is snapped.
        /// </summary>
        internal double SnapOffsetCorrection
        {
            get
            {
                return this.snapCorrectionOffset;
            }
            set
            {
                if (this.snapCorrectionOffset != value)
                {
                    this.snapCorrectionOffset = value;
                    this.OnSnapOffsetMarginChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the panel will center the currently selected item according to the
        /// centering options and the layout orientation.
        /// </summary>
        internal bool IsCentered
        {
            get
            {
                return this.isCentered;
            }
            set
            {
                if (value != this.isCentered)
                {
                    this.isCentered = value;
                    this.OnIsCenteredChanged();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the panel is Loaded (present on the visual scene).
        /// </summary>
        internal bool IsLoaded
        {
            get
            {
                return this.isLoaded;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="RadLoopingList"/> instance that hosts this panel.
        /// </summary>
        internal RadLoopingList Owner
        {
            get
            {
                return this.owner;
            }
            set
            {
                this.owner = value;
                this.UpdateFromOwner();
            }
        }

        internal double AvailableLength
        {
            get
            {
                return this.availableLength;
            }
        }

        internal double ItemLength
        {
            get
            {
                return this.itemLength;
            }
        }

        /// <summary>
        /// Gets or sets the count of the logical items that form the wheel.
        /// </summary>
        internal int LogicalCount
        {
            get
            {
                return this.logicalCount;
            }
            set
            {
                this.logicalCount = value;
            }
        }

        /// <summary>
        /// Gets the count of the visual items.
        /// </summary>
        internal int VisualCount
        {
            get
            {
                return this.visualCount;
            }
        }

        /// <summary>
        /// Gets the top logical index that is currently realized (visible on the screen).
        /// </summary>
        internal int TopLogicalIndex
        {
            get
            {
                return this.topLogicalIndex;
            }
        }

        /// <summary>
        /// Gets the max offset. Exposed for testing purposes.
        /// </summary>
        internal double MaxOffset
        {
            get
            {
                return this.GetMaxOffset();
            }
        }

        /// <summary>
        /// Gets the min offset. Exposed for testing purposes.
        /// </summary>
        internal double MinOffset
        {
            get
            {
                return this.GetMinOffset();
            }
        }

        /// <summary>
        /// Gets the current vertically scrolled amount (in pixels).
        /// </summary>
        internal double VerticalOffset
        {
            get
            {
                return this.visualOffset;
            }
        }

        /// <summary>
        /// Applies the specified offset as current.
        /// </summary>
        /// <param name="offset">The desired offset.</param>
        /// <param name="duration">The duration of the animation used to apply the offset.</param>
        /// <param name="ease">The easing function that describes animation interpolation.</param>
        internal void SetVerticalOffset(double offset, Duration duration, EasingFunctionBase ease)
        {
            if (!this.IsInitialized())
            {
                return;
            }

            this.scrollState = LoopingPanelScrollState.Scrolling;
            this.AnimateVerticalOffset(duration, ease, offset);
        }

        /// <summary>
        /// Applies the specified vertical offset, translates the visual items accordingly and updates the logical indexes.
        /// </summary>
        /// <param name="newOffset">The new vertical offset.</param>
        /// <param name="force">True to update the wheel even if the new offset equals the current one, false otherwise.</param>
        internal void UpdateWheel(double newOffset, bool force)
        {
            if (!this.IsInitialized())
            {
                return;
            }

            if (!force && this.visualOffset == newOffset)
            {
                return;
            }

            this.UpdateWheelCore(newOffset, force);
        }

        internal void UpdateOrientation(Orientation newOrientation)
        {
            if (this.orientation == newOrientation)
            {
                return;
            }

            this.orientation = newOrientation;
            this.UpdateLayoutParams();
            this.Reset();
        }

        internal void UpdateFromOwner()
        {
            this.UpdateOrientation(this.owner.Orientation);
        }

        internal void Scroll(double amount)
        {
            if (!this.IsInitialized())
            {
                return;
            }

            if (this.animating)
            {
                this.StopOffsetAnimation(false);
            }

            this.scrollState = LoopingPanelScrollState.Scrolling;
            this.UpdateWheel(this.visualOffset + amount, false);
        }

        internal void EndScroll()
        {
            if (!this.IsInitialized())
            {
                return;
            }

            if (this.scrollState != LoopingPanelScrollState.Scrolling)
            {
                return;
            }

            if (this.animating)
            {
                this.StopOffsetAnimation(false);
            }

            this.OnScrollCompleted(this.scrollState);
        }

        internal void CenterItem(LoopingListItem item, bool select, bool animate)
        {
            if (select && this.owner != null)
            {
                if (!this.owner.SelectItem(item, LoopingListSelectionChangeReason.VisualItemSnappedToMiddle))
                {
                    this.ScrollToIndex(item, this.owner.SelectedIndex, this.owner.SelectedVisualIndex);
                    return;
                }
            }

            if (animate)
            {
                double offset = this.GetSnapOffset(item);
                if (offset != this.visualOffset)
                {
                    this.ScrollState = LoopingPanelScrollState.SnapScrolling;
                    this.AnimateVerticalOffset(offset);
                }
            }
            else
            {
                this.BringIntoView(item.LogicalIndex, item.VisualIndex);
            }
        }

        internal void Reset()
        {
            this.StopOffsetAnimation(false);

            this.visualIndexChain.Clear();
            this.visualCount = 0;
            this.topLogicalIndex = 0;
            this.visualOffset = 0;
            this.centeringDuringLayout = false;
            this.items.Clear();
            this.Children.Clear();

            this.InvalidateMeasure();
        }

        /// <summary>
        /// Gets the visual item that represents the specified logical index.
        /// Will return null if the index is not currently realized (visualized).
        /// </summary>
        internal LoopingListItem ItemFromLogicalIndex(int logicalIndex)
        {
            foreach (LoopingListItem item in this.items)
            {
                if (item.LogicalIndex == logicalIndex)
                {
                    return item;
                }
            }

            return null;
        }

        internal LoopingListItem ItemFromOffset(double offset)
        {
            foreach (LoopingListItem item in this.items)
            {
                var itemTop = this.orientation == Orientation.Vertical ? item.VerticalOffset : item.HorizontalOffset;
                var itemBottom = itemTop + this.itemLength;

                if (itemTop <= offset && itemBottom > offset)
                {
                    return item;
                }
            }

            return null;
        }

        internal LoopingListItem FindMiddleItem()
        {
            double middle = 0;

            switch (this.centeredItemSnapPosition)
            {
                case LoopingListItemSnapPosition.Far:
                    middle = this.availableLength - this.itemLength;
                    break;
                case LoopingListItemSnapPosition.Middle:
                    middle = this.availableLength / 2;
                    break;
            }

            return this.ItemFromOffset(middle);
        }

        /// <summary>
        /// Stops the vertical offset animation (if running).
        /// </summary>
        internal void StopOffsetAnimation(bool raiseCompleted)
        {
            LoopingPanelScrollState currentState = this.scrollState;

            this.scrollState = LoopingPanelScrollState.NotScrolling;
            this.animating = false;
            this.offsetStoryboard.Stop();

            if (raiseCompleted && currentState != LoopingPanelScrollState.NotScrolling)
            {
                this.OnScrollCompleted(currentState);
            }
        }

        /// <summary>
        /// Updates the count of the visual items, depending on the provided available length.
        /// </summary>
        internal void UpdateVisualItemCount()
        {
            if (this.logicalCount == 0 || this.availableLength == 0 || this.itemLength == 0)
            {
                this.visualCount = 0;
                return;
            }

            this.visibleItemsParts = this.availableLength / this.itemLength;
            this.visualCount = (int)(this.visibleItemsParts + OffScreenItemCount);
        }

        /// <summary>
        /// Animates the vertical offset to the specified value, starting from the current one.
        /// </summary>
        /// <param name="to">The final value of the animation.</param>
        internal void AnimateVerticalOffset(double to)
        {
            ExponentialEase easing = new ExponentialEase();
            easing.EasingMode = EasingMode.EaseInOut;
            Duration duration = new Duration(TimeSpan.FromSeconds(.5));

            this.AnimateVerticalOffset(duration, easing, to);
        }

        /// <summary>
        /// Animates the vertical offset to the specified value, starting from the current one.
        /// </summary>
        /// <param name="duration">The duration of the animation.</param>
        /// <param name="easing">The easing function that calculates animation steps.</param>
        /// <param name="to">The final value of the animation.</param>
        internal void AnimateVerticalOffset(Duration duration, EasingFunctionBase easing, double to)
        {
            if (RadControl.IsInTestMode)
            {
                // equivalen to AnimateVerticalOffset
                this.OnScrollOffsetChanged(to);
                this.owner.UpdateSelection(this.owner.SelectedIndex, this.owner.GetVisualIndex(this.owner.SelectedIndex), LoopingListSelectionChangeReason.VisualItemSnappedToMiddle);
            }
            else
            {
                this.offsetAnimation.From = this.visualOffset;
                this.offsetAnimation.To = to;
                this.offsetAnimation.Duration = duration;
                this.offsetAnimation.EasingFunction = easing;

                this.animating = true;
                this.offsetStoryboard.Completed += this.OffsetStoryboard_Completed;
                this.offsetStoryboard.Begin();
            }
        }
        
        /// <summary>
        /// Ensures that the visual item, associated with the specified logical index is currently displayed.
        /// </summary>
        internal void BringIntoView(int logicalIndex, int visualIndex)
        {
            this.BringIntoView(logicalIndex, visualIndex, false);
        }

        internal void UpdateIndexes(bool force, bool animate)
        {
            this.topLogicalIndex = this.GetFirstRealizedIndex(this.logicalCount);

            int itemCount = this.visualIndexChain.Count;
            int currentLogicalIndex = this.topLogicalIndex;

            for (int i = 0; i < itemCount; i++)
            {
                LoopingListItem item = this.items[this.visualIndexChain[i]];

                if (currentLogicalIndex == this.logicalCount)
                {
                    currentLogicalIndex = 0;
                }

                if (this.owner != null && !item.IsEmpty)
                {
                    this.owner.UpdateVisualItem(item, currentLogicalIndex, this.visualIndexChain[i], force, animate);
                }

                currentLogicalIndex++;
            }
        }

        internal void UpdateWheelCore(double newOffset, bool force)
        {
            this.visualOffset = this.ClampOffset(newOffset);

            double visualLength = this.visualCount * this.itemLength;
            double normalizedOffset = this.visualOffset % visualLength;
            int hiddenItems = (int)(Math.Abs(normalizedOffset) / this.itemLength);

            int topIndex = 0;
            double bottomOffset = 0;
            double topOffset = 0;

            if (this.visualOffset <= 0)
            {
                topIndex = hiddenItems;
                topOffset = normalizedOffset;
                bottomOffset = visualLength + topOffset;
            }
            else
            {
                topIndex = this.visualCount - hiddenItems;
                bottomOffset = normalizedOffset;

                if (normalizedOffset % this.itemLength != 0)
                {
                    topIndex -= 1;
                }

                topOffset = bottomOffset - visualLength;
            }

            this.visualIndexChain.Clear();

            this.TranslateItems(topIndex, this.visualCount, topOffset);
            this.TranslateItems(0, topIndex, bottomOffset);
            this.UpdateIndexes(force, false);
        }

        internal int FindClosestIndexDistance(int oldIndex, int newIndex, int count)
        {
            if (oldIndex < 0 || oldIndex >= count)
            {
                throw new ArgumentException(string.Format("Index lies outside the allowed values range [{0} : {1}].", 0, count - 1), "oldIndex");
            }

            if (newIndex < 0 || newIndex >= count)
            {
                throw new ArgumentException(string.Format("Index lies outside the allowed values range [{0} : {1}].", 0, count - 1), "newIndex");
            }

            var difference = newIndex - oldIndex;

            if (Math.Abs(difference) > count / 2)
            {
                difference = difference > 0 ? difference - count : difference + count;
            }

            return difference;
        }

        internal bool IsIndexInView(int logicalIndex)
        {
            foreach (LoopingListItem item in this.Children)
            {
                if (item.LogicalIndex == logicalIndex && !item.IsEmpty && !item.isHidden)
                {
                    return true;
                }
            }

            return false;
        }

        internal LoopingListItem ItemFromVisualIndex(int visualIndex)
        {
            foreach (LoopingListItem item in this.items)
            {
                if (item.VisualIndex == visualIndex)
                {
                    return item;
                }
            }

            return null;
        }

        internal int GetFirstRealizedIndex(int wheelItemsCount)
        {
            double wheelLength = wheelItemsCount * this.itemLength;

            double normalizedOffset = this.visualOffset % wheelLength;
            int hiddenItems = (int)(Math.Abs(normalizedOffset) / this.itemLength);

            int index;

            if (normalizedOffset <= 0)
            {
                index = hiddenItems;
            }
            else
            {
                index = wheelItemsCount - hiddenItems;
                if (normalizedOffset % this.itemLength != 0)
                {
                    index -= 1;
                }
            }

            return index;
        }

        internal LoopingListItem GetFirstVisibleItem()
        {
            LoopingListItem topItem = null;
            double offset = double.MaxValue;
            foreach (LoopingListItem item in this.Children)
            {
                if (!item.IsEmpty && !item.isHidden)
                {
                    var itemOffset = this.orientation == Orientation.Horizontal ? item.HorizontalOffset : item.VerticalOffset;

                    if (itemOffset < offset)
                    {
                        offset = itemOffset;
                        topItem = item;
                    }
                }
            }

            return topItem;
        }

        /// <summary>
        /// Provides the behavior for the Measure pass of Silverlight layout. Classes can override this method to define their own Measure pass behavior.
        /// </summary>
        /// <param name="availableSize">The available size that this object can give to child objects. Infinity (<see cref="F:System.Double.PositiveInfinity"/>) can be specified as a value to indicate that the object will size to whatever content is available.</param>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of the allocated sizes for child objects; or based on other considerations, such as a fixed container size.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (double.IsPositiveInfinity(availableSize.Width))
            {
                if (this.owner != null)
                {
                    if (this.orientation != Orientation.Horizontal)
                    {
                        availableSize.Width = this.owner.ItemWidth;
                    }
                    else
                    {
                        availableSize.Width = 2 * this.itemLength;
                    }
                }
            }

            if (double.IsPositiveInfinity(availableSize.Height))
            {
                if (this.orientation != Orientation.Horizontal)
                {
                    availableSize.Height = 2 * this.itemLength;
                }
                else
                {
                    availableSize.Height = this.owner.ItemWidth;
                }
            }

            this.size = availableSize;

            this.UpdateLayoutParams();

            this.CreateVisualItems();

            foreach (LoopingListItem item in this.items)
            {
                if (!item.IsEmpty)
                {
                    item.Measure(availableSize);
                }
            }

            return availableSize;
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
            double position = 0;

            this.owner.UpdateSelectedVisualIndex();

            for (int i = 0; i < this.visualCount; i++)
            {
                LoopingListItem item = this.items[i];

                Size desired = item.DesiredSize;
                if (!item.IsEmpty)
                {
                    Rect rect;

                    if (this.orientation == Orientation.Vertical)
                    {
                        rect = new Rect(0, position, desired.Width, desired.Height);
                    }
                    else
                    {
                        rect = new Rect(position, 0, desired.Width, desired.Height);
                    }

                    item.Arrange(rect);
                    item.ArrangeRect = rect;
                }

                position += this.orientation == Orientation.Vertical ? desired.Height : desired.Width;
            }

            if (this.isCentered && !this.centeringDuringLayout)
            {
                this.centeringDuringLayout = true;
                if (this.owner != null && this.owner.SelectedVisualIndex != -1)
                {
                    this.BringIntoView(this.owner.SelectedIndex, this.owner.SelectedVisualIndex);
                }
                else
                {
                    this.CenterMiddleItem(false, false);
                }
            }
            else
            {
                this.centeringDuringLayout = false;
            }

            return finalSize;
        }

        private static void OnVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LoopingPanel panel = d as LoopingPanel;
            if (panel.animating)
            {
                panel.OnScrollOffsetChanged((double)e.NewValue);
            }
        }

        private double GetSnapOffsetWhenCentered()
        {
            switch (this.centeredItemSnapPosition)
            {
                case LoopingListItemSnapPosition.Middle:
                    return ((this.availableLength - this.itemLength) / 2) + this.snapCorrectionOffset;
                case LoopingListItemSnapPosition.Far:
                    return this.availableLength - this.itemLength + this.snapCorrectionOffset;
                case LoopingListItemSnapPosition.Near:
                    return this.snapCorrectionOffset;
            }

            return this.snapCorrectionOffset;
        }

        /// <summary>
        /// Ensures that the visual item, associated with the specified logical index is currently displayed.
        /// </summary>
        /// <param name="logicalIndex">The logical index of the item.</param>
        /// <param name="visualIndex">The visual index of the item.</param>
        /// <param name="animate">True to perform animated scrolling, false otherwise.</param>
        private void BringIntoView(int logicalIndex, int visualIndex, bool animate)
        {
            // no visual children yet
            if (this.items.Count == 0)
            {
                return;
            }

            if (logicalIndex < 0 || logicalIndex >= this.logicalCount)
            {
                return;
            }

            double offset;
            if (this.IsIndexInView(logicalIndex) && visualIndex >= 0)
            {
                offset = this.CalculateVisualOffset(visualIndex, this.visualIndexChain[0]);
            }
            else
            {
                offset = this.CalculateLogicalOffset(logicalIndex, this.topLogicalIndex);
            }

            if (animate)
            {
                this.scrollState = LoopingPanelScrollState.ScrollingToIndex;
                this.AnimateVerticalOffset(offset);
            }
            else
            {
                this.UpdateWheel(offset, false);

                this.UpdateLayout();

                // Force items to be remeasured as occasionalli there is no measure for items and they desapear.
                this.InvalidateMeasure();
            }
        }

        private double CalculateVisualOffset(int index, int topIndex)
        {
            double offset = 0;

            if (this.isCentered)
            {
                var wheelLength = this.visualCount * this.itemLength;
                var scrolleedWheels = (int)(this.visualOffset / wheelLength) * wheelLength;
                offset = scrolleedWheels - (topIndex + this.VisualIndexChain.IndexOf(index)) * this.itemLength + this.GetSnapOffsetWhenCentered();

                if (this.visualOffset > 0 && this.visualOffset % wheelLength != 0)
                {
                    offset += wheelLength;
                }
            }
            else
            {
                offset = this.visualOffset;
            }

            return offset;
        }

        private double CalculateLogicalOffset(int index, int topIndex)
        {
            double offset = 0;

            if (this.isLoopingEnabled)
            {
                var wheelLength = this.logicalCount * this.itemLength;
                var scrolleedWheels = (int)(this.visualOffset / wheelLength) * wheelLength;
                var current = this.visualOffset > 0 ? (this.logicalCount - topIndex) * this.itemLength : -topIndex * this.itemLength;
                var difference = this.FindClosestIndexDistance(topIndex, index, this.logicalCount);

                offset = scrolleedWheels + current - difference * this.itemLength;
            }
            else
            {
                offset = -index * this.itemLength;
            }

            if (this.isCentered)
            {
                offset += this.GetSnapOffsetWhenCentered();
            }

            return offset;
        }
        
        private void OffsetStoryboard_Completed(object sender, object e)
        {
            this.offsetStoryboard.Completed -= this.OffsetStoryboard_Completed;

            this.owner.UpdateSelection(this.owner.SelectedIndex, this.owner.GetVisualIndex(this.owner.SelectedIndex), LoopingListSelectionChangeReason.VisualItemSnappedToMiddle);
        }

        /// <summary>
        /// Scrolls to the desired index, starting from the provided visual item.
        /// </summary>
        /// <param name="item">The visual item to start from.</param>
        /// <param name="toLogicalIndex">The target logical index to scroll to.</param>
        /// <param name="toVisualIndex">The target visual index to scroll to.</param>
        private void ScrollToIndex(LoopingListItem item, int toLogicalIndex, int toVisualIndex)
        {
            if (!this.IsInitialized())
            {
                return;
            }

            if (this.IsIndexInView(toLogicalIndex))
            {
                if (item.VisualIndex == toVisualIndex)
                {
                    this.BringIntoView(toLogicalIndex, toVisualIndex);
                    return;
                }

                int from = item.VisualIndex;
                int indexOffset = Math.Abs(from - toVisualIndex);
                int pivotIndex = this.visualCount / 2;
                int sign = Math.Sign(from - toVisualIndex);

                if (indexOffset > pivotIndex)
                {
                    indexOffset = this.visualCount - indexOffset;
                    sign *= -1;
                }

                double offset = this.visualOffset + sign * indexOffset * this.itemLength;
                this.scrollState = LoopingPanelScrollState.ScrollingToIndex;
                this.AnimateVerticalOffset(offset);
            }
            else
            {
                int from = item.LogicalIndex;
                int indexOffset = Math.Abs(from - toLogicalIndex);
                int pivotIndex = this.logicalCount / 2;
                int sign = Math.Sign(from - toLogicalIndex);

                if (indexOffset > pivotIndex)
                {
                    indexOffset = this.logicalCount - indexOffset;
                    sign *= -1;
                }

                double offset = this.visualOffset + sign * indexOffset * this.itemLength;
                this.scrollState = LoopingPanelScrollState.ScrollingToIndex;
                this.AnimateVerticalOffset(offset);
            }
        }

        /// <summary>
        /// Notifies that a scroll operation has completed.
        /// </summary>
        /// <param name="previousState">The state the panel was in before the scrolling ended.</param>
        private void OnScrollCompleted(LoopingPanelScrollState previousState)
        {
            this.scrollState = LoopingPanelScrollState.NotScrolling;

            if (!this.isCentered)
            {
                return;
            }

            if (previousState == LoopingPanelScrollState.Scrolling ||
                previousState == LoopingPanelScrollState.ScrollingToIndex)
            {
                this.CenterMiddleItem(previousState != LoopingPanelScrollState.ScrollingToIndex, true);
            }
        }

        private void CreateVisualItems()
        {
            int oldCount = this.visualCount;

            this.UpdateVisualItemCount();
            if (this.visualCount == 0 || this.visualCount == oldCount && this.items.Count >= this.visualCount)
            {
                return;
            }

            if (this.visualCount != oldCount || this.items.Count < this.visualCount)
            {
                this.Children.Clear();
                this.visualIndexChain.Clear();
                this.items.Clear();
            }

            for (int i = 0; i < this.visualCount; i++)
            {
                LoopingListItem item = this.CreateVisualItem();
                this.Children.Add(item);
                this.visualIndexChain.Add(i);

                if (!this.isLoopingEnabled && i > this.logicalCount - 1)
                {
                    item.SetIsEmpty(true);
                }
            }

            if (this.topLogicalIndex == 0 || this.visualCount != oldCount)
            {
                this.UpdateIndexes(this.visualCount != oldCount, false);
            }
            else
            {
                this.BringIntoView(this.topLogicalIndex, this.visualIndexChain[0]);
            }
        }

        private LoopingListItem CreateVisualItem()
        {
            if (this.owner == null)
            {
                return null;
            }

            LoopingListItem item = this.owner.CreateVisualItem();

            item.Attach(this);
            this.items.Add(item);

            return item;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.owner == null)
            {
                return;
            }

            this.size = e.NewSize;
            this.UpdateLayoutParams();
            this.UpdateVisualItemCount();

            if (this.visualCount != this.Children.Count)
            {
                this.UpdateChildren(true);
            }

            if (this.isCentered && this.owner != null)
            {
                LoopingListItem itemToCenter = this.FindMiddleItem();
                if (itemToCenter != null)
                {
                    this.CenterItem(itemToCenter, true, false);
                }
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.isLoaded = true;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            this.isLoaded = false;
            this.centeringDuringLayout = false;
            this.offsetStoryboard.Completed -= this.OffsetStoryboard_Completed;
            this.StopOffsetAnimation(false);
        }

        private void OnOffsetAnimationCompleted(object sender, object args)
        {
            if (this.scrollState == LoopingPanelScrollState.NotScrolling)
            {
                return;
            }

            LoopingPanelScrollState previousState = this.scrollState;

            this.animating = false;
            this.offsetStoryboard.Stop();
            this.OnScrollCompleted(previousState);
        }

        private void CreateOffsetAnimation()
        {
            this.offsetStoryboard = new Storyboard();
            this.offsetStoryboard.Completed += this.OnOffsetAnimationCompleted;

            this.offsetAnimation = new DoubleAnimation();
            this.offsetAnimation.EnableDependentAnimation = true;
            this.offsetStoryboard.Children.Add(this.offsetAnimation);

            Storyboard.SetTarget(this.offsetStoryboard, this);
            Storyboard.SetTargetProperty(this.offsetStoryboard, "VerticalOffset");
        }

        private bool IsInitialized()
        {
            return this.visualCount > 0 && this.logicalCount > 0;
        }

        private void UpdateLayoutParams()
        {
            this.itemLength = this.orientation == Orientation.Vertical ? this.owner.ItemHeight : this.owner.ItemWidth;
            this.itemLength = this.itemLength + (this.owner.ItemSpacing * 2);
            this.availableLength = this.orientation == Orientation.Vertical ? this.size.Height : this.size.Width;
        }

        private void CenterMiddleItem(bool select, bool animate)
        {
            LoopingListItem middleItem = this.FindMiddleItem();

            if (middleItem == null)
            {
                if (this.items.Count > 0)
                {
                    middleItem = this.items[0];
                }
                else
                {
                    return;
                }
            }

            this.CenterItem(middleItem, select, animate);
        }

        private void TranslateItems(int startIndex, int endIndex, double offset)
        {
            for (int i = startIndex; i < endIndex; i++)
            {
                LoopingListItem item = this.items[i];
                if (!item.IsEmpty)
                {
                    if (this.orientation == Orientation.Vertical)
                    {
                        item.SetVerticalOffset(offset);
                    }
                    else
                    {
                        item.SetHorizontalOffset(offset);
                    }

                    if (!this.isLoopingEnabled)
                    {
                        double itemOffset = this.orientation == Orientation.Vertical ? item.VerticalOffset : item.HorizontalOffset;
                        itemOffset = Math.Round(itemOffset, 2);
                        double thisOffset = Math.Round(this.visualOffset, 2);
                        double maxOffset = Math.Round((this.logicalCount * this.itemLength) + this.visualOffset, 2);

                        if (maxOffset <= itemOffset || itemOffset - thisOffset < 0)
                        {
                            item.SetIsHidden(true);
                        }
                        else
                        {
                            item.SetIsHidden(false);
                        }
                    }
                    else
                    {
                        item.SetIsHidden(false);
                    }
                }
                this.visualIndexChain.Add(i);
            }
        }

        private void UpdateChildren(bool updateWheel)
        {
            if (this.logicalCount == 0 || this.owner == null)
            {
                return;
            }

            // still no children, waiting for the first measure pass.
            if (this.Children.Count == 0 || this.Children.Count == this.visualCount)
            {
                return;
            }

            while (this.Children.Count < this.visualCount)
            {
                this.Children.Add(this.CreateVisualItem());
            }

            while (this.Children.Count > this.visualCount)
            {
                this.Children.RemoveAt(this.Children.Count - 1);
            }

            if (updateWheel)
            {
                this.UpdateWheel(this.visualOffset, true);
            }
        }

        private double GetMaxOffset()
        {
            double logicalWheelLength = this.logicalCount * this.itemLength;
            double maxOffset = 0;

            if (!this.isCentered)
            {
                if (this.availableLength > logicalWheelLength)
                {
                    maxOffset = 0;
                }
                else
                {
                    maxOffset = this.availableLength - logicalWheelLength;
                }
            }
            else
            {
                maxOffset = -logicalWheelLength + this.itemLength + this.GetSnapOffsetWhenCentered();
            }

            return maxOffset;
        }

        private double GetMinOffset()
        {
            if (this.isCentered)
            {
                return this.GetSnapOffsetWhenCentered();
            }

            return 0;
        }

        private double ClampOffset(double newOffset)
        {
            if (this.isLoopingEnabled)
            {
                return newOffset;
            }

            double minOffset = this.GetMinOffset();
            double maxOffset = this.GetMaxOffset();

            if (newOffset > minOffset)
            {
                newOffset = minOffset;
            }
            else if (newOffset < maxOffset)
            {
                newOffset = maxOffset;
            }

            return newOffset;
        }

        private double GetSnapOffset(LoopingListItem middleItem)
        {
            double middleOffset = this.GetSnapOffsetWhenCentered();
            double itemOffset = this.orientation == Orientation.Vertical ? middleItem.VerticalOffset : middleItem.HorizontalOffset;

            return this.visualOffset - (itemOffset - middleOffset);
        }

        private void OnScrollOffsetChanged(double offset)
        {
            this.UpdateWheel(offset, false);
        }

        private void OnIsLoopingEnabledChanged()
        {
            if (this.owner == null)
            {
                return;
            }

            this.StopOffsetAnimation(false);

            if (this.items.Count == 0)
            {
                return;
            }

            if (!this.isLoopingEnabled)
            {
                this.UpdateWheel(0, false);

                for (int i = 0; i < this.items.Count; i++)
                {
                    LoopingListItem item = this.items[i];

                    if (i >= this.logicalCount)
                    {
                        item.SetIsEmpty(true);
                    }
                }

                var offset = -this.owner.SelectedIndex * this.itemLength;
                if (this.isCentered)
                {
                    offset += this.GetSnapOffsetWhenCentered();
                }

                this.UpdateWheel(offset, false);
            }
            else
            {
                foreach (LoopingListItem item in this.items)
                {
                    item.SetIsEmpty(false);
                }

                this.InvalidateMeasure();
                this.UpdateWheel(this.visualOffset, true);
            }

            this.owner.UpdateSelection(this.owner.SelectedIndex, this.owner.GetVisualIndex(this.owner.SelectedIndex), LoopingListSelectionChangeReason.Private);
        }

        private void OnSnapOffsetMarginChanged()
        {
            if (this.items.Count == 0)
            {
                return;
            }

            if (!this.isCentered)
            {
                return;
            }

            this.CenterMiddleItem(true, false);
        }

        private void OnCenteredItemSnapPositionChanged()
        {
            if (this.items.Count == 0)
            {
                return;
            }

            if (!this.isCentered)
            {
                return;
            }

            if (this.owner.SelectedIndex != -1)
            {
                this.owner.CenterCurrentItem(false);
            }
            else
            {
                this.CenterMiddleItem(true, false);
            }
        }

        private void OnIsCenteredChanged()
        {
            if (this.items.Count == 0)
            {
                return;
            }

            if (!this.isLoopingEnabled)
            {
                if (!this.isCentered)
                {
                    this.UpdateWheel(this.ClampOffset(this.visualOffset), false);
                }
                else
                {
                    this.CenterMiddleItem(true, false);
                }
            }
            else
            {
                if (this.isCentered)
                {
                    this.BringIntoView(this.owner.SelectedIndex, this.owner.SelectedVisualIndex);
                    this.CenterMiddleItem(true, false);
                }
            }
        }
    }
}