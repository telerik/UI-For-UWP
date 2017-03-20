using System;
using System.Collections;
using System.ComponentModel;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Data.ListView.Commands;
using Telerik.UI.Xaml.Controls.Data.ListView.View.Controls;
using Telerik.UI.Xaml.Controls.Primitives;
using Telerik.UI.Xaml.Controls.Primitives.DragDrop;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Data.ListView
{
    /// <summary>
    /// Represents a visual item that is used in the <see cref="RadListView"/> control.
    /// </summary>
    [TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Unselected", GroupName = "SelectionStates")]
    [TemplateVisualState(Name = "Selected", GroupName = "SelectionStates")]
    [TemplatePart(Name = "PART_ReorderHandle", Type = typeof(FrameworkElement))]
    public partial class RadListViewItem : RadContentControl, IArrangeChild
    {
        /// <summary>
        /// Identifies the <see cref="IsSelected"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(RadListViewItem), new PropertyMetadata(false, OnIsSelectedChanged));

        /// <summary>
        /// Identifies the <see cref="Orientation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(RadListViewItem), new PropertyMetadata(Orientation.Vertical));

        private FrameworkElement reorderHandle;
        private bool needUpdate = true;

        internal bool isDraggedForAction = false;
        internal RadListViewItem dragVisual;
        internal Size lastDesiredSize = Size.Empty;
        internal Rect arrangeRect;

        // TODO: add weakrefernce list for the images and measure/arrange item when the image is loaded/failed.
        private bool isTemplateApplied;
        private double dragX;
        private double dragY;
        private bool isDragContent;
        private Border firstHandle;
        private Border secondHandle;

        private bool isSelectedCache;
        private Orientation orientationCache = Orientation.Vertical;

        /// <summary>
        /// Exposed for testing purposes.
        /// </summary>
        internal FrameworkElement ReodredHandle
        {
            get { return this.reorderHandle; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RadListViewItem" /> class.
        /// </summary>
        public RadListViewItem()
        {
            this.DefaultStyleKey = typeof(RadListViewItem);
            this.SizeChanged += this.RadListViewItem_SizeChanged;
        }

        /// <summary>
        /// Gets a value indicating whether item action is enabled.
        /// </summary>
        public bool IsActionOnSwipeEnabled
        {
            get
            {
                return this.ListView.IsActionOnSwipeEnabled;
            }
        }

        public ListViewItemSwipeDirection SwipeDirection
        {
            get
            {
                return this.ListView.ItemSwipeDirection;
            }
        }

        /// <summary>
        /// Gets or sets the orientation of the control.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Orientation Orientation
        {
            get
            {
                return this.orientationCache;
            }
            set
            {
                if (this.orientationCache != value)
                {
                    this.SetValue(OrientationProperty, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the item is selected.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return this.isSelectedCache;
            }
            set
            {
                if (this.isSelectedCache != value)
                {
                    this.SetValue(IsSelectedProperty, value);
                }
            }
        }

        Rect IArrangeChild.LayoutSlot
        {
            get
            {
                return this.arrangeRect;
            }
        }


        internal RadListView ListView { get; set; }

        internal IListView Owner
        {
            get
            {
                return this.ListView as IListView;
            }
        }

        internal void InitializeDragHandles()
        {
            this.firstHandle = this.GetTemplateChild("PART_FirstHandle") as Border;
            if (this.firstHandle != null)
            {
                this.firstHandle.ManipulationMode = ManipulationModes.None;
                DragDrop.SetAllowDrag(this.firstHandle, true);              
            }

            this.secondHandle = this.GetTemplateChild("PART_SecondHandle") as Border;
            if (this.secondHandle != null)
            {
                this.secondHandle.ManipulationMode = ManipulationModes.None;
                DragDrop.SetAllowDrag(this.secondHandle, true);
            }

            this.UpdateSwipeHandlesVisibility();
        }

        internal void PrepareDragVisual(DragAction action)
        {
            var owner = this.ListView;
            var dragContent = owner.DragBehavior.GetDragVisual(this);


            this.dragVisual = this.ListView.GetContainerForItem();

            this.dragVisual.Orientation = this.Orientation;
            this.dragVisual.Width = this.ActualWidth;
            this.dragVisual.Height = this.ActualHeight;
            this.dragVisual.ListView = this.ListView;
            this.dragVisual.isDragContent = true;
            this.dragVisual.IsSelected = this.IsSelected;
            this.ListView.PrepareContainerForItem(this.dragVisual, this.DataContext);

            this.Opacity = 0;
            if (action == DragAction.ItemAction)
            {
                this.PrepareActionContent();
            }
        }

        internal void OnDragComplete(ItemSwipeActionCompleteContext context)
        {
            var dragPos = DragDrop.GetDragPositionMode(this);

            if (dragPos.HasFlag(DragPositionMode.RailXForward) || dragPos.HasFlag(DragPositionMode.RailXBackwards))
            {
                Canvas.SetLeft(this, context.FinalDragOffset);
            }
            else
            {
                Canvas.SetTop(this, context.FinalDragOffset);
            }
        }

        internal void SwipeActionContentControl_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var offset = this.ListView.Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal ? this.dragY : this.dragX;

            this.ListView.OnItemActionControlTap(this, offset);
        }

        internal void ResetDragPosition()
        {
            if (this.ListView.Orientation == Orientation.Horizontal)
            {
                Canvas.SetTop(this, this.arrangeRect.Y);
            }
            else
            {
                Canvas.SetLeft(this, this.arrangeRect.X);
            }
        }

        internal void EndDragOperation()
        {
            var operation = DragDrop.GetRunningOperation(this);
            if (operation != null)
            {
                operation.EndDrag();
            }
        }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize)
        {
            var size = base.MeasureOverride(availableSize);
            if (this.lastDesiredSize != size)
            {
                needUpdate = true;
            }

            this.lastDesiredSize = size;
            return size;
        }

        /// <inheritdoc/>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
        }

        /// <inheritdoc/>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.isDragContent)
            {
                return base.ArrangeOverride(finalSize);
            }

            var width = Math.Max(0, this.arrangeRect.Width - this.Margin.Left - this.Margin.Right);
            var height = Math.Max(0, this.arrangeRect.Height - this.Margin.Top - this.Margin.Bottom);
            var resultSize = new Size(Math.Max(width, finalSize.Width), Math.Max(height, finalSize.Height));

            var size = base.ArrangeOverride(resultSize);

            if (!this.ListView.contentPanel.IsInArrange)
            {
                this.ListView.contentPanel.InvalidateArrange();

                return new Size(width, height);
            }

            return resultSize;
        }

        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.isTemplateApplied = true;

            this.reorderHandle = this.GetTemplateChild("PART_ReorderHandle") as FrameworkElement;
            this.isTemplateApplied = this.isTemplateApplied && this.reorderHandle != null;

            if (this.isTemplateApplied)
            {
                this.reorderHandle.PointerPressed += OnReorderHandlePointerPressed;
            }


            this.InitializeDragHandles();
            this.ChangeVisualState();
        }

        /// <inheritdoc/>
        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            base.OnPointerExited(e);
            this.ChangeVisualState(true);
        }

        /// <inheritdoc/>
        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            base.OnPointerEntered(e);
            this.ChangeVisualState(true);
        }

        /// <inheritdoc/>
        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            base.OnTapped(e);
            if (this.ListView != null)
            {
                this.ListView.OnItemTap(this, e.GetPosition(this));
            }
        }

        /// <inheritdoc/>
        protected override void OnHolding(HoldingRoutedEventArgs e)
        {
            if (this.isDragContent)
            {
                return;
            }

            base.OnHolding(e);

            if (this.ListView != null && e.HoldingState == Windows.UI.Input.HoldingState.Started && this.ListView.ReorderMode == ListViewReorderMode.Default)
            {
                this.ListView.OnItemHold(this, e);
            }
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            object currentItem = this.ListView.CurrentItem;

            // Makes the focus moves like in MS ListView on Tab and Shift + Tab
            // The negative side effect is small glitch - focus jumps from the first / last item to the current.
            if (currentItem != this.Content)
            {
                this.ListView.FocusCurrentContainer();
            }
            else
            {
                this.ListView.ScrollItemIntoView(currentItem);
            }
        }

        /// <inheritdoc/>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadListViewItemAutomationPeer(this);
        }

        /// <inheritdoc/>
        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            base.OnKeyDown(e);

            bool success = false;
            this.ListView.currentLogicalIndex = this.logicalIndex;

            switch (e.Key)
            {
                case VirtualKey.Right:
                    {
                        if (this.Owner.Orientation == Orientation.Horizontal)
                        {
                            success = this.Owner.CurrencyService.MoveCurrentToNext();
                        }
                    }
                    break;
                case VirtualKey.Down:
                    {
                        if (this.Owner.Orientation == Orientation.Vertical)
                        {
                            success = this.Owner.CurrencyService.MoveCurrentToNext();
                        }
                    }
                    break;
                case VirtualKey.Left:
                    {
                        if (this.Owner.Orientation == Orientation.Horizontal)
                        {
                            success = this.Owner.CurrencyService.MoveCurrentToPrevious();
                        }
                    }
                    break;
                case VirtualKey.Up:
                    {
                        if (this.Owner.Orientation == Orientation.Vertical)
                        {
                            success = this.Owner.CurrencyService.MoveCurrentToPrevious();
                        }
                    }
                    break;
                case VirtualKey.Home:
                    {
                        success = this.Owner.CurrencyService.MoveCurrentToFirst();
                    }
                    break;
                case VirtualKey.End:
                    {
                        success = this.Owner.CurrencyService.MoveCurrentToLast();
                    }
                    break;
                case VirtualKey.Space:
                case VirtualKey.Enter:
                    {
                        if (this.ListView != null)
                        {
                            this.ListView.OnItemTap(this, new Point());
                        }
                        e.Handled = true;
                    }
                    break;
            }

            if (success)
            {
                this.ListView.FocusCurrentContainer();
                e.Handled = true;
            }
        }

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadListViewItem item = d as RadListViewItem;
            item.isSelectedCache = (bool)e.NewValue;
            item.ChangeVisualState(true);
        }       

        private void OnReorderHandlePointerPressed(object sender, PointerRoutedEventArgs e)
        {
            this.ListView.OnItemReorderHandlePressed(this, e, sender);
        }

        private void UpdateActionContentClipping(double offset)
        {
            double x = 0;
            double y = 0;
            double width = this.ListView.swipeActionContentControl.ActualWidth;
            double height = this.ListView.swipeActionContentControl.ActualHeight;

            var dragMode = DragDrop.GetDragPositionMode(this);

            if (offset > 0)
            {
                if (this.ListView.Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal)
                {
                    if(dragMode.HasFlag(DragPositionMode.RailYForward))
                    {
                        height = Math.Max(offset, 0);
                    }
                    else
                    {
                        height = 0;
                    }
                }
                else
                {
                    if(dragMode.HasFlag(DragPositionMode.RailXForward))
                    {
                        width = Math.Max(offset, 0);
                    } 
                    else
                    {
                        width = 0;
                    }
                }
            }
            else
            {
                if (this.ListView.Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal)
                {
                    if(dragMode.HasFlag(DragPositionMode.RailYBackwards))
                    {
                        y = Math.Max(0, height + offset);
                    }
                    else
                    {
                        y = height;
                    }
                }
                else
                {
                    if (dragMode.HasFlag(DragPositionMode.RailXBackwards))
                    {
                        x = Math.Max(0, width + offset);
                    }
                    else
                    {
                        x = width;
                    }
                }
            }

            this.ListView.swipeActionContentControl.Clip = new Windows.UI.Xaml.Media.RectangleGeometry() { Rect = new Rect(x, y, width, height) };
        }

        private void RadListViewItem_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            var aritmetics = new DoubleArithmetics(1);

            if (!aritmetics.AreClose(e.PreviousSize, e.NewSize) && !this.isDragContent)
            {
                this.Owner.UpdateService.RegisterUpdate(new DelegateUpdate<UpdateFlags>(() => this.ListView.contentPanel.InvalidateMeasure()));
                this.needUpdate = false;
            }
        }

        protected virtual void ChangeVisualState(bool useTransitions)
        {
            if (!this.IsEnabled)
            {
                this.GoToState(useTransitions, "Disabled");
            }
            else
            {
                this.GoToState(useTransitions, "Normal");
            }
            if (this.IsSelected)
            {
                this.GoToState(useTransitions, "Selected");
            }
            else
            {
                this.GoToState(useTransitions, "Unselected");
            }
            if (this.IsHandleEnabled)
            {
                this.GoToState(useTransitions, "ReorderEnabled");
            }
            else
            {
                this.GoToState(useTransitions, "ReorderDisabled");
            }
        }

        protected void GoToState(bool useTransitions, params string[] stateNames)
        {
            if (stateNames != null)
            {
                foreach (string str in stateNames)
                {
                    if (VisualStateManager.GoToState(this, str, useTransitions))
                    {
                        return;
                    }
                }
            }
        }

        private void ChangeVisualState()
        {
            if (this.isTemplateApplied)
            {
                this.ChangeVisualState(true);
            }
        }

        private void PrepareActionContent()
        {
            DragPositionMode dragMode = DragDrop.GetDragPositionMode(this);
            ContentControl swipeActionContentControl = this.ListView.swipeActionContentControl;
            swipeActionContentControl.DataContext = this.DataContext;

            if (dragMode.HasFlag(DragPositionMode.RailXForward) || dragMode.HasFlag(DragPositionMode.RailXBackwards))
            {
                swipeActionContentControl.Width = this.ActualWidth;
                swipeActionContentControl.Height = this.ActualHeight;

                Canvas.SetLeft(swipeActionContentControl, 0);
                Canvas.SetTop(swipeActionContentControl, Canvas.GetTop(this));
            }
            else
            {
                swipeActionContentControl.Width = this.ActualWidth;
                swipeActionContentControl.Height = this.ActualHeight;

                Canvas.SetLeft(swipeActionContentControl, Canvas.GetLeft(this));
                Canvas.SetTop(swipeActionContentControl, 0);
            }

            swipeActionContentControl.Margin = this.Margin;

            swipeActionContentControl.Visibility = Visibility.Visible;

            swipeActionContentControl.Tapped += this.SwipeActionContentControl_Tapped;

            this.ListView.isActionContentDisplayed = true;
        }

        private void ClearActionContent()
        {
            this.ListView.ResetActionContent();
        }

        private bool ShouldReorder(Point position, ReorderItemsDragOperation data)
        {
            if (this.reorderCoordinator == null)
            {
                return false;
            }

            var sourceElement = this.reorderCoordinator.Host.ElementAt(data.CurrentSourceReorderIndex);

            var startPosition = this.ListView.Orientation == Orientation.Horizontal ? position.X : position.Y;
            var itemLength = this.ListView.Orientation == Orientation.Horizontal ? this.ActualWidth : this.ActualHeight;

            bool draggingFromStart = startPosition >= itemLength / 2 + DragInitializer.DefaultStartTreshold && startPosition <= itemLength && this.logicalIndex > data.CurrentSourceReorderIndex;
            bool draggingFromEnd = startPosition <= itemLength / 2 - DragInitializer.DefaultStartTreshold && startPosition >= 0 && this.logicalIndex < data.CurrentSourceReorderIndex;

            bool canReorderColumn = sourceElement != null;

            return canReorderColumn && (draggingFromStart || draggingFromEnd);
        }

        private object GetDestinationDataItem(ReorderItemsDragOperation data)
        {
            IEnumerable enumerableSource = this.ListView.ItemsSource as IEnumerable;
            IEnumerator enumerator = enumerableSource.GetEnumerator();
            int i = 0;
            while (i++ <= data.CurrentSourceReorderIndex)
            {
                enumerator.MoveNext();
            }
            object destinationDataItem = enumerator.Current;
            return destinationDataItem;
        }

        private void UpdateSwipeHandlesVisibility()
        {
            if (this.firstHandle != null)
            {
                this.firstHandle.Visibility = (this.IsActionOnSwipeEnabled && (this.SwipeDirection == ListViewItemSwipeDirection.All || this.SwipeDirection == ListViewItemSwipeDirection.Forward)) ? Visibility.Visible : Visibility.Collapsed;
            }
            if (this.secondHandle != null)
            {
                this.secondHandle.Visibility = (this.IsActionOnSwipeEnabled && (this.SwipeDirection == ListViewItemSwipeDirection.All || this.SwipeDirection == ListViewItemSwipeDirection.Backwards)) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        // This is needed because sometimes the contentpanel is not notified when the listviewitem has changed its size(Downsize animations).
        bool IArrangeChild.TryInvalidateOwner()
        {
            bool isInvalidated = false;
            if (this.ListView.LayoutDefinition.GetType() == typeof(StackLayoutDefinition))
            {
                if (needUpdate)
                {
                    if (this.ListView.Orientation == Windows.UI.Xaml.Controls.Orientation.Vertical)
                    {
                        if (!ListViewModel.DoubleArithmetics.AreClose(arrangeRect.Height, this.lastDesiredSize.Height) && arrangeRect.Height > this.lastDesiredSize.Height)
                        {
                            this.ListView.contentPanel.InvalidateMeasure();
                            isInvalidated = true;
                        }
                    }
                    else
                    {
                        if (!ListViewModel.DoubleArithmetics.AreClose(arrangeRect.Width, this.lastDesiredSize.Width) && arrangeRect.Width > this.lastDesiredSize.Width)
                        {
                            this.ListView.contentPanel.InvalidateMeasure();
                            isInvalidated = true;
                        }
                    }
                    needUpdate = false;
                }

            }

            return isInvalidated;
        }
    }
}