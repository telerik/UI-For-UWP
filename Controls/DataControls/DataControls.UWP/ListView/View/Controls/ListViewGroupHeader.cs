using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Data.ListView.Primitives
{
    /// <summary>
    /// Allows a user to view a header and expand that header to see further details, or to collapse a section up to a header.
    /// </summary>
    public class ListViewGroupHeader : RadContentControl, IArrangeChild
    {
        /// <summary>
        /// Identifies the IsExpanded dependency property.
        /// </summary>
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register(nameof(IsExpanded), typeof(bool), typeof(ListViewGroupHeader), new PropertyMetadata(true, OnIsExpandedPropertyChanged));

        /// <inheritdoc/>
        public Rect arrangeRect;
        
        internal bool OwnerArranging;

        internal Size ArrangeSize;

        private Size lastDesiredSize = Size.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListViewGroupHeader"/> class.
        /// </summary>
        public ListViewGroupHeader()
        {
            this.DefaultStyleKey = typeof(ListViewGroupHeader);
            this.SizeChanged += this.ListViewGroupHeader_SizeChanged;
        }

        internal ListViewGroupHeader(bool isFrozen)
            : this()
        {
            this.IsFrozen = isFrozen;
        }

        /// <summary>
        /// Gets a value indicating whether the details are expanded or collapsed.
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                return (bool)this.GetValue(IsExpandedProperty);
            }
            internal set
            {
                this.SetValue(IsExpandedProperty, value);
            }
        }

        Rect IArrangeChild.LayoutSlot
        {
            get
            {
                return this.arrangeRect;
            }
        }

        internal bool IsFrozen { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="RadListView"/> owner.
        /// </summary>
        protected internal RadListView Owner { get; set; }

        bool IArrangeChild.TryInvalidateOwner()
        {
            return false;
        }

        /// <summary>
        /// Called before the Tapped event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            base.OnTapped(e);

            if (e == null)
            {
                return;
            }

            if (!e.Handled && this.Owner != null)
            {
                this.Owner.OnGroupHeaderTap(this);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Called before the DoubleTapped event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnDoubleTapped(DoubleTappedRoutedEventArgs e)
        {
            base.OnDoubleTapped(e);

            if (e == null)
            {
                return;
            }

            if (!e.Handled && this.Owner != null)
            {
                this.Owner.OnGroupHeaderTap(this);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Builds the current visual state for this instance.
        /// </summary>
        protected override string ComposeVisualStateName()
        {
            if (this.IsExpanded)
            {
                return "Expanded";
            }

            return "Collapsed";
        }

        /// <inheritdoc/>
        protected override Size ArrangeOverride(Size finalSize)
        {
            var width = Math.Max(0, this.arrangeRect.Width - this.Margin.Left - this.Margin.Right);
            var height = Math.Max(0, this.arrangeRect.Height - this.Margin.Top - this.Margin.Bottom);
            var resultSize = new Size(Math.Max(width, finalSize.Width), Math.Max(height, finalSize.Height));

            var size = base.ArrangeOverride(resultSize);

            if (!this.Owner.contentPanel.IsInArrange)
            {
                this.Owner.contentPanel.InvalidateArrange();
                return new Size(width, height);
            }

            return resultSize;
        }

        private static void OnIsExpandedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ListViewGroupHeader groupHeader = d as ListViewGroupHeader;
            var context = groupHeader.DataContext as GroupHeaderContext;
            if (context != null)
            {
                context.IsExpanded = (bool)e.NewValue;
            }

            groupHeader.UpdateVisualState(groupHeader.IsTemplateApplied);
            if (groupHeader.Owner != null)
            {
                groupHeader.Owner.OnGroupIsExpandedChanged();
            }
        }

        private void ListViewGroupHeader_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!ListViewModel.DoubleArithmetics.AreClose(e.PreviousSize, e.NewSize))
            {
                (this.Owner as IListView).UpdateService.RegisterUpdate((int)UpdateFlags.AffectsContent);
            }
        }
    }
}