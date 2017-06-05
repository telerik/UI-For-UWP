using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.UI.Xaml.Controls.Data.DataBoundListBox
{
    /// <summary>
    /// Represents a control that allows the user to reorder items in <see cref="RadDataBoundListBox"/>.
    /// </summary>
    public class ItemReorderControl : RadControl
    {
        /// <summary>
        /// Identifies the <see cref="ShiftUpButtonStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShiftUpButtonStyleProperty =
            DependencyProperty.Register(nameof(ShiftUpButtonStyle), typeof(Style), typeof(ItemReorderControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ShiftDownButtonStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShiftDownButtonStyleProperty =
            DependencyProperty.Register(nameof(ShiftDownButtonStyle), typeof(Style), typeof(ItemReorderControl), new PropertyMetadata(null));

        internal RadDataBoundListBox owner;
        internal RadVirtualizingDataControlItem targetItem;
        internal RadVirtualizingDataControlItem swappedItem;
        internal Storyboard scrollBoard;
        internal DoubleAnimation scrollAnimation;
        internal Storyboard navBarBoard;
        internal DoubleAnimation navBarAnimation;

        private static readonly DependencyProperty ScrollOffsetPrivateProperty =
         DependencyProperty.Register("ScrollOffsetPrivate", typeof(double), typeof(ItemReorderControl), new PropertyMetadata(null, OnScrollOffsetPrivateChanged));

        private Button moveUpButton;
        private Button moveDownButton;
        private Storyboard sb;
        private DoubleAnimation da1;
        private DoubleAnimation da2;
        private bool animating;
        private Border layoutRoot;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemReorderControl" /> class.
        /// </summary>
        public ItemReorderControl()
        {
            this.DefaultStyleKey = typeof(ItemReorderControl);
        }

        /// <summary>
        /// Gets or sets an instance of the <see cref="Style"/> class
        /// that defines the visual appearance of the button that shifts items down in the source collection.
        /// </summary>
        public Style ShiftDownButtonStyle
        {
            get
            {
                return this.GetValue(ShiftDownButtonStyleProperty) as Style;
            }
            set
            {
                this.SetValue(ShiftDownButtonStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets an instance of the <see cref="Style"/> class
        /// that defines the visual appearance of the button that shifts items up in the source collection.
        /// </summary>
        public Style ShiftUpButtonStyle
        {
            get
            {
                return this.GetValue(ShiftUpButtonStyleProperty) as Style;
            }
            set
            {
                this.SetValue(ShiftUpButtonStyleProperty, value);
            }
        }

        internal void EvaluateButtonsAvailability()
        {
            if (this.IsTemplateApplied)
            {
                this.moveDownButton.IsEnabled = this.owner.listSource.CanShiftItemIndexDown(this.targetItem.associatedDataItem);
                this.moveUpButton.IsEnabled = this.owner.listSource.CanShiftItemIndexUp(this.targetItem.associatedDataItem);
            }
        }

        internal void ShiftTargetItemDown(bool animate)
        {
            if (!this.owner.listSource.CanShiftItemIndexDown(this.targetItem.associatedDataItem) || this.animating)
            {
                return;
            }

            this.swappedItem = this.targetItem.previous;
            double swappedItemOffset = this.targetItem.CurrentOffset +
                (this.owner.virtualizationStrategy.GetItemLength(this.targetItem) - this.owner.virtualizationStrategy.GetItemLength(this.swappedItem));

            if (animate)
            {
                this.AnimateItems(this.swappedItem, swappedItemOffset, this.targetItem, this.swappedItem.CurrentOffset);
            }
            else
            {
                this.owner.virtualizationStrategy.SetItemOffset(this.targetItem, this.swappedItem.CurrentOffset);
                this.owner.virtualizationStrategy.SetItemOffset(this.swappedItem, swappedItemOffset);
            }

            this.swappedItem.next = this.targetItem.next;

            if (this.targetItem.next != null)
            {
                this.targetItem.next.previous = this.swappedItem;
            }

            this.targetItem.next = this.swappedItem;

            if (this.swappedItem.previous != null)
            {
                this.swappedItem.previous.next = this.targetItem;
            }

            this.targetItem.previous = this.swappedItem.previous;
            this.swappedItem.previous = this.targetItem;

            int itemIndex = this.owner.realizedItems.IndexOf(this.targetItem);
            this.owner.realizedItems.RemoveAt(itemIndex);
            this.owner.realizedItems.Insert(itemIndex - 1, this.targetItem);
            this.owner.firstItemCache = this.owner.realizedItems[0];
            this.owner.lastItemCache = this.owner.realizedItems[this.owner.realizedItems.Count - 1];

            this.owner.listSource.ShiftItemIndexDown(this.targetItem.associatedDataItem);

            this.EvaluateButtonsAvailability();
        }

        internal void ShiftTargetItemUp(bool animate)
        {
            if (!this.owner.listSource.CanShiftItemIndexUp(this.targetItem.associatedDataItem) || this.animating)
            {
                return;
            }

            this.swappedItem = this.targetItem.next;

            double nextItemOffset = this.swappedItem.CurrentOffset +
                (this.owner.virtualizationStrategy.GetItemLength(this.swappedItem) - this.owner.virtualizationStrategy.GetItemLength(this.targetItem));

            RadVirtualizingDataControlItem item1 = this.swappedItem;
            RadVirtualizingDataControlItem item2 = this.targetItem;

            if (animate)
            {
                this.AnimateItems(item1, this.targetItem.CurrentOffset, item2, nextItemOffset);
            }
            else
            {
                this.owner.virtualizationStrategy.SetItemOffset(this.swappedItem, this.targetItem.CurrentOffset);
                this.owner.virtualizationStrategy.SetItemOffset(this.targetItem, nextItemOffset);
            }

            this.targetItem.next = this.swappedItem.next;

            if (this.swappedItem.next != null)
            {
                this.swappedItem.next.previous = this.targetItem;
            }

            this.swappedItem.next = this.targetItem;

            if (this.targetItem.previous != null)
            {
                this.targetItem.previous.next = this.swappedItem;
            }

            this.swappedItem.previous = this.targetItem.previous;
            this.targetItem.previous = this.swappedItem;

            int itemIndex = this.owner.realizedItems.IndexOf(this.targetItem);
            this.owner.realizedItems.RemoveAt(itemIndex);
            this.owner.realizedItems.Insert(itemIndex + 1, this.targetItem);
            this.owner.firstItemCache = this.owner.realizedItems[0];
            this.owner.lastItemCache = this.owner.realizedItems[this.owner.realizedItems.Count - 1];

            this.owner.listSource.ShiftItemIndexUp(this.targetItem.associatedDataItem);

            this.EvaluateButtonsAvailability();
        }

        /// <summary>
        /// Occurs when a System.Windows.FrameworkElement has been constructed and added to the object tree.
        /// </summary>
        protected override void LoadCore()
        {
            base.LoadCore();

            if (this.owner.virtualizationStrategy.LayoutOrientation == Orientation.Vertical)
            {
                VisualStateManager.GoToState(this, "Vertical", false);
                Canvas.SetLeft(this, (this.owner.virtualizationStrategy.ViewportExtent - this.DesiredSize.Width) / 2);
            }
            else
            {
                VisualStateManager.GoToState(this, "Horizontal", false);
                Canvas.SetTop(this, (this.owner.virtualizationStrategy.ViewportExtent - this.DesiredSize.Height) / 2);
            }
        }

        /// <inheritdoc/>
        protected override bool ApplyTemplateCore()
        {
            base.ApplyTemplateCore();

            this.layoutRoot = this.GetTemplateChild("PART_LayoutRoot") as Border;
            this.moveUpButton = this.GetTemplateChild("PART_ButtonMoveUp") as Button;
            this.moveDownButton = this.GetTemplateChild("PART_ButtonMoveDown") as Button;

            return this.moveUpButton != null &&
                   this.moveDownButton != null;
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application
        /// code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate" />.
        /// In simplest terms, this means the method is called just before a UI element displays
        /// in an application. For more information, see Remarks.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.moveUpButton.Tapped += this.OnMoveUpButton_Tap;
            this.moveDownButton.Tapped += this.OnMoveDownButton_Tap;

            this.PrepareAnimations();

            this.EvaluateButtonsAvailability();
        }

        private static void OnScrollOffsetPrivateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as ItemReorderControl).OnScrollOffsetPrivateChanged(args);
        }

        private void OnScrollOffsetPrivateChanged(DependencyPropertyChangedEventArgs args)
        {
            this.owner.virtualizationStrategy.ScrollToOffset((double)args.NewValue, null);
        }

        private void PrepareAnimations()
        {
            this.navBarBoard = new Storyboard();
            this.navBarAnimation = new DoubleAnimation();
            this.navBarAnimation.EnableDependentAnimation = true;
            this.navBarBoard.Children.Add(this.navBarAnimation);
            this.navBarAnimation.Duration = TimeSpan.FromMilliseconds(300);
            this.navBarAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };

            this.scrollBoard = new Storyboard();
            this.scrollAnimation = new DoubleAnimation();
            this.scrollAnimation.EasingFunction = new CubicEase();
            this.scrollAnimation.Duration = TimeSpan.FromMilliseconds(500);
            this.scrollAnimation.EnableDependentAnimation = true;
            Storyboard.SetTarget(this.scrollAnimation, this);
            Storyboard.SetTargetProperty(this.scrollAnimation, "ScrollOffsetPrivate");
            this.scrollBoard.Children.Add(this.scrollAnimation);

            this.sb = new Storyboard();

            this.da1 = new DoubleAnimation();
            this.da1.Duration = TimeSpan.FromMilliseconds(300);
            this.da1.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            this.da1.EnableDependentAnimation = true;
            this.sb.Children.Add(this.da1);

            this.da2 = new DoubleAnimation();
            this.da2.Duration = TimeSpan.FromMilliseconds(300);
            this.da2.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            this.da2.EnableDependentAnimation = true;
            this.sb.Children.Add(this.da2);

            this.sb.Completed += this.ReorderAnimation_Completed;
            this.scrollBoard.Completed += this.OnScrollAnimation_Completed;
        }

        private void OnMoveDownButton_Tap(object sender, TappedRoutedEventArgs e)
        {
            this.ShiftTargetItemDown(true);

            this.owner.OnItemReorderMoveDownButtonTap();
        }

        private void OnMoveUpButton_Tap(object sender, TappedRoutedEventArgs e)
        {
            this.ShiftTargetItemUp(true);

            this.owner.OnItemReorderMoveUpButtonTap();
        }

        private void AnimateItems(RadVirtualizingDataControlItem item1, double offset1, RadVirtualizingDataControlItem item2, double offset2)
        {
            this.AnimateScrollOffsetToFocusItem(item2.next == item1 || item2.next == null);

            if (this.owner.virtualizationStrategy.LayoutOrientation == Orientation.Vertical)
            {
                Storyboard.SetTargetProperty(this.da1, "(Canvas.Top)");
                Storyboard.SetTarget(this.da1, item1);
                Storyboard.SetTargetProperty(this.da2, "(Canvas.Top)");
                Storyboard.SetTarget(this.da2, item2);
            }
            else
            {
                Storyboard.SetTargetProperty(this.da1, "(Canvas.Left)");
                Storyboard.SetTarget(this.da1, item1);

                Storyboard.SetTargetProperty(this.da2, "(Canvas.Left)");
                Storyboard.SetTarget(this.da2, item2);
            }

            this.da1.From = item1.CurrentOffset;
            this.da1.To = offset1;
            this.da2.From = item2.CurrentOffset;
            this.da2.To = offset2;

            this.sb.Begin();
            this.animating = true;
        }

        private double GetOffsetRelativeToTargetItem()
        {
            if (!ElementTreeHelper.IsElementRendered(this) || !ElementTreeHelper.IsElementRendered(this.targetItem))
            {
                return 0;
            }

            Point offset = this.TransformToVisual(this.targetItem).TransformPoint(new Point(0, 0));

            if (this.owner.virtualizationStrategy.LayoutOrientation == Orientation.Vertical)
            {
                return offset.Y + ((this.DesiredSize.Height - this.targetItem.height) / 2);
            }
            else
            {
                return offset.X + ((this.DesiredSize.Width - this.targetItem.width) / 2);
            }
        }

        private void ReorderAnimation_Completed(object sender, object e)
        {
            this.animating = false;
            double item1Offset = this.owner.virtualizationStrategy.GetElementCanvasOffset(this.swappedItem);
            double item2Offset = this.owner.virtualizationStrategy.GetElementCanvasOffset(this.targetItem);

            this.sb.Stop();

            this.owner.virtualizationStrategy.SetItemOffset(this.swappedItem, item1Offset);
            this.owner.virtualizationStrategy.SetItemOffset(this.targetItem, item2Offset);

            this.owner.BalanceVisualSpace();
        }

        private void AnimateScrollOffsetToFocusItem(bool up)
        {
            double itemRelativeOffset = this.owner.virtualizationStrategy.GetItemRelativeOffset(this.targetItem);
            double itemLength = this.owner.virtualizationStrategy.GetItemLength(this.targetItem);
            double swappedItemLength = this.owner.virtualizationStrategy.GetItemLength(this.swappedItem);

            double itemNewOffset = itemRelativeOffset + (up ? swappedItemLength : -swappedItemLength);

            if (!up && (itemNewOffset < 0))
            {
                double toOffset = this.owner.virtualizationStrategy.ScrollOffset;
                this.scrollAnimation.From = toOffset;
                toOffset -= -itemNewOffset;

                this.scrollAnimation.To = Math.Max(0, toOffset);

                this.scrollBoard.Begin();
            }
            else if (up && (itemNewOffset + itemLength > this.owner.virtualizationStrategy.ViewportLength))
            {
                double toOffset = this.owner.virtualizationStrategy.ScrollOffset;
                this.scrollAnimation.From = toOffset;

                toOffset += (itemNewOffset + itemLength) - this.owner.virtualizationStrategy.ViewportLength;

                this.scrollAnimation.To = toOffset;

                this.scrollBoard.Begin();
            }
        }

        private void OnScrollAnimation_Completed(object sender, object e)
        {
            this.owner.BalanceVisualSpace();
        }
    }
}
