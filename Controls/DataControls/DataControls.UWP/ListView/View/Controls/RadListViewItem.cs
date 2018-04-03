using System;
using System.Collections;
using System.ComponentModel;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Data.ListView.Commands;
using Telerik.UI.Xaml.Controls.Data.ListView.View.Controls;
using Telerik.UI.Xaml.Controls.Primitives;
using Telerik.UI.Xaml.Controls.Primitives.DragDrop;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
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

        /// <summary>
        /// Identifies the <see cref="DisabledStateOpacity"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DisabledStateOpacityProperty =
            DependencyProperty.Register(nameof(DisabledStateOpacity), typeof(double), typeof(RadListViewItem), new PropertyMetadata(0.5));

        internal bool isDraggedForAction = false;
        internal RadListViewItem dragVisual;
        internal Size lastDesiredSize = Size.Empty;
        internal Rect arrangeRect;
        private FrameworkElement reorderHandle;
        private bool needUpdate = true;

        // TODO: add weakrefernce list for the images and measure/arrange item when the image is loaded/failed.
        private bool isTemplateApplied;

        private double dragX;
        private double dragY;
        private bool isReordering;
        private Border firstHandle;
        private Border secondHandle;

        private bool isSelectedCache;
        private Orientation orientationCache = Orientation.Vertical;
        private RadListView listView;

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

        /// <summary>
        /// Gets the swipe direction of the item.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the value of the opacity used when the ListView is disabled.
        /// </summary>
        /// <value>
        /// The value of the opacity used when the ListView is disabled.
        /// </value>
        public double DisabledStateOpacity
        {
            get
            {
                return (double)GetValue(DisabledStateOpacityProperty);
            }
            set
            {
                this.SetValue(DisabledStateOpacityProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the item is currently reordering.
        /// </summary>
        public bool IsReordering
        {
            get
            {
                return this.isReordering;
            }
        }

        Rect IArrangeChild.LayoutSlot
        {
            get
            {
                return this.arrangeRect;
            }
        }

        internal RadListView ListView
        {
            get
            {
                return this.listView;
            }
            set
            {
                if (this.listView != value)
                {
                    this.listView = value;
                    this.BindToListViewProperties();
                }
            }
        }

        internal IListView Owner
        {
            get
            {
                return this.ListView as IListView;
            }
        }

        /// <inheritdoc/>
        internal FrameworkElement ReodredHandle
        {
            get { return this.reorderHandle; }
        }

        // This is needed because sometimes the contentpanel is not notified when the listviewitem has changed its size(Downsize animations).
        bool IArrangeChild.TryInvalidateOwner()
        {
            bool isInvalidated = false;
            if (this.ListView.LayoutDefinition.GetType() == typeof(StackLayoutDefinition))
            {
                if (this.needUpdate)
                {
                    if (this.ListView.Orientation == Windows.UI.Xaml.Controls.Orientation.Vertical)
                    {
                        if (!ListViewModel.DoubleArithmetics.AreClose(this.arrangeRect.Height, this.lastDesiredSize.Height) && this.arrangeRect.Height > this.lastDesiredSize.Height)
                        {
                            this.ListView.contentPanel.InvalidateMeasure();
                            isInvalidated = true;
                        }
                    }
                    else
                    {
                        if (!ListViewModel.DoubleArithmetics.AreClose(this.arrangeRect.Width, this.lastDesiredSize.Width) && this.arrangeRect.Width > this.lastDesiredSize.Width)
                        {
                            this.ListView.contentPanel.InvalidateMeasure();
                            isInvalidated = true;
                        }
                    }

                    this.needUpdate = false;
                }
            }

            return isInvalidated;
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
            this.dragVisual.isReordering = true;
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

        /// <summary>
        /// Prepares the swipe drag handles of the item.
        /// </summary>
        protected internal void PrepareSwipeDragHandles()
        {
            if (this.IsActionOnSwipeEnabled)
            {
                switch (this.SwipeDirection)
                {
                    case ListViewItemSwipeDirection.All:
                        this.PrepareFirstSwipeHandle(true);
                        this.PrepareSecondSwipeHandle(true);
                        break;
                    case ListViewItemSwipeDirection.Forward:
                        this.PrepareFirstSwipeHandle(true);
                        this.PrepareSecondSwipeHandle(false);
                        break;
                    case ListViewItemSwipeDirection.Backwards:
                        this.PrepareFirstSwipeHandle(false);
                        this.PrepareSecondSwipeHandle(true);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                this.PrepareFirstSwipeHandle(false);
                this.PrepareSecondSwipeHandle(false);
            }
        }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize)
        {
            var size = base.MeasureOverride(availableSize);
            if (this.lastDesiredSize != size)
            {
                this.needUpdate = true;
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
            if (this.isReordering)
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

            this.PrepareReorderHandle();

            this.firstHandle = this.GetTemplateChild("PART_FirstHandle") as Border;
            this.secondHandle = this.GetTemplateChild("PART_SecondHandle") as Border;

            this.PrepareSwipeDragHandles();
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
        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            base.OnPointerMoved(e);

            Pointer pointer = e.Pointer;
            PointerPoint pointerPoint = e.GetCurrentPoint(this);
            if (!this.isReordering && pointerPoint.Properties.IsLeftButtonPressed && pointer.PointerDeviceType == PointerDeviceType.Mouse
                && RadListViewItem.CanCapturePointer(this, pointer))
            {
                var source = e.OriginalSource;
                if (source != this.firstHandle && source != this.secondHandle)
                {
                    this.listView.OnItemReorderHandlePressed(this, e, DragDropTrigger.MouseDrag, null);
                }
            }
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
            if (this.isReordering)
            {
                return;
            }

            base.OnHolding(e);

            if (this.ListView != null && e.HoldingState == Windows.UI.Input.HoldingState.Started)
            {
                this.ListView.OnItemHold(this, e);
            }
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        private static bool CanCapturePointer(RadListViewItem listViewItem, Pointer pointer)
        {
            if (listViewItem.CapturePointer(pointer))
            {
                listViewItem.ReleasePointerCapture(pointer);
                return true;
            }

            return false;
        }
        private void BindToListViewProperties()
        {
            if (this.ListView == null)
            {
                return;
            }

            Binding binding = new Binding();
            binding.Path = new PropertyPath(nameof(this.DisabledStateOpacity));
            binding.Source = this.ListView;

            this.SetBinding(RadListViewItem.DisabledStateOpacityProperty, binding);
        }

        private void PrepareFirstSwipeHandle(bool isVisible)
        {
            if (isVisible)
            {
                if (this.firstHandle != null)
                {
                    this.firstHandle.ManipulationMode = ManipulationModes.None;
                    DragDrop.SetAllowDrag(this.firstHandle, true);
                    this.firstHandle.Visibility = Visibility.Visible;
                }
            }
            else
            {
                if (this.firstHandle != null)
                {
                    this.firstHandle.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void PrepareSecondSwipeHandle(bool isVisible)
        {
            if (isVisible)
            {
                if (this.secondHandle != null)
                {
                    this.secondHandle.ManipulationMode = ManipulationModes.None;
                    DragDrop.SetAllowDrag(this.secondHandle, true);
                    this.secondHandle.Visibility = Visibility.Visible;
                }
            }
            else
            {
                if (this.secondHandle != null)
                {
                    this.secondHandle.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void PrepareReorderHandle()
        {
            if (this.IsHandleEnabled)
            {
                if (this.reorderHandle == null)
                {
                    this.reorderHandle = this.GetTemplateChild("PART_ReorderHandle") as FrameworkElement;
                }

                if (this.reorderHandle != null)
                {
                    this.reorderHandle.PointerPressed += this.OnReorderHandlePointerPressed;
                }
            }
            else
            {
                if (this.reorderHandle != null)
                {
                    this.reorderHandle.PointerPressed -= this.OnReorderHandlePointerPressed;
                }
            }
        }

        private void OnReorderHandlePointerPressed(object sender, PointerRoutedEventArgs e)
        {
            this.ListView.OnItemReorderHandlePressed(this, e, DragDropTrigger.Drag, sender);
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
                    if (dragMode.HasFlag(DragPositionMode.RailYForward))
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
                    if (dragMode.HasFlag(DragPositionMode.RailXForward))
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
                    if (dragMode.HasFlag(DragPositionMode.RailYBackwards))
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

            if (!aritmetics.AreClose(e.PreviousSize, e.NewSize) && !this.isReordering)
            {
                this.Owner.UpdateService.RegisterUpdate(new DelegateUpdate<UpdateFlags>(() => this.ListView.contentPanel.InvalidateMeasure()));
                this.needUpdate = false;
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

        private object GetDestinationDataItem(int index)
        {
            int actualIndex = index;
            if (this.listView.GroupDescriptors.Count == 0)
            {
                actualIndex = this.listView.Model.layoutController.strategy.GetElementFlatIndex(actualIndex);
            }

            var info = this.listView.Model.FindDataItemFromIndex(actualIndex);

            object item = null;
            if (info.HasValue)
            {
                item = info.Value.Item;
            }

            return item;
        }
    }
}