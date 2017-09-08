using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.UI.Xaml.Controls.Data
{
    public partial class RadVirtualizingDataControl
    {
        /// <summary>
        /// Identifies the <see cref="HorizontalScrollBarVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty =
            DependencyProperty.Register(nameof(HorizontalScrollBarVisibility), typeof(ScrollBarVisibility), typeof(RadVirtualizingDataControl), new PropertyMetadata(ScrollBarVisibility.Auto, OnHorizontalScrollBarVisibilityChanged));

        /// <summary>
        /// Identifies the <see cref="VerticalScrollBarVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty =
            DependencyProperty.Register(nameof(VerticalScrollBarVisibility), typeof(ScrollBarVisibility), typeof(RadVirtualizingDataControl), new PropertyMetadata(ScrollBarVisibility.Auto, OnVerticalScrollBarVisibilityChanged));

        internal ScrollBarVisibility verticalScrollBarVisibilityCache = ScrollBarVisibility.Auto;
        internal ScrollBarVisibility horizontalScrollBarVisibilityCache = ScrollBarVisibility.Auto;

        internal double previousScrollOffset = 0;

        private static readonly TimeSpan ScrollOpacityAnimationDuration = TimeSpan.FromMilliseconds(300);
        private static readonly TimeSpan EmptyTimeSpan = TimeSpan.FromMilliseconds(0);

        private DoubleAnimation opacityAnimation;

        /// <summary>
        /// Gets or sets the visibility of the horizontal scroll bar.
        /// </summary>
        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get
            {
                return (ScrollBarVisibility)this.GetValue(HorizontalScrollBarVisibilityProperty);
            }
            set
            {
                this.SetValue(HorizontalScrollBarVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the visibility of the vertical scroll bar.
        /// </summary>
        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get
            {
                return (ScrollBarVisibility)this.GetValue(VerticalScrollBarVisibilityProperty);
            }
            set
            {
                this.SetValue(VerticalScrollBarVisibilityProperty, value);
            }
        }

        internal void ScrollToVerticalOffset(double offset)
        {
            if (!this.IsOperational())
            {
                return;
            }

            if (this.scrollContentPresenter != null)
            {
                this.scrollContentPresenter.SetVerticalOffset(offset);
            }
            else
            {
                this.manipulationContainer.ChangeView(null, offset, null);
            }
        }

        /// <summary>
        /// Calculates the zero-based index where the collection change
        /// occurred in the collection that holds all realized items.
        /// The calculation here is made by subtracting the index of the
        /// first realized item in the data source from the index where
        /// the change occurred - again in the data source.
        /// </summary>
        internal virtual int GetItemRealizedIndexFromListSourceIndex(int changeIndex, int firstItemIndex)
        {
            return changeIndex - firstItemIndex;
        }

        /// <summary>
        /// Gets the index of the data item bound to the the first realized container in the data source.
        /// </summary>
        internal virtual int GetFirstItemCacheIndex()
        {
            return this.firstItemCache != null ? this.firstItemCache.associatedDataItem.Index : -1;
        }

        /// <summary>
        /// Gets the index of the data item bound to the last realized container in the data source.
        /// </summary>
        internal virtual int GetLastItemCacheIndex()
        {
            RadVirtualizingDataControlItem lastItem = this.lastItemCache;

            return lastItem != null ? lastItem.associatedDataItem.Index : -1;
        }

        /// <summary>
        /// Gets a boolean value that determines whether the last realized container is bound to the last
        /// data item from the list source.
        /// </summary>
        internal virtual bool IsLastItemLastInListSource()
        {
            return this.lastItemCache.associatedDataItem == this.listSource.GetLastItem();
        }

        /// <summary>
        /// Gets a boolean value that determines whether the first realized container is bound to the first
        /// data item from the list source.
        /// </summary>
        internal virtual bool IsFirstItemFirstInListSource()
        {
            return this.firstItemCache.associatedDataItem == this.listSource.GetFirstItem();
        }

        /// <summary>
        /// Called when the vertical scroll offset has been changed either by flicking or panning.
        /// </summary>
        internal virtual void OnScrollOffsetChanged(bool balanceImmediately)
        {
            this.scrollUpdateService.ProcessUpdatesQueue();

            if (this.waitingForBalance && !balanceImmediately)
            {
                return;
            }
            
            this.InvalidateMeasure();
        }

        /// <summary>
        /// Called when the <see cref="ScrollViewer.VerticalScrollBarVisibility"/> attached property changes
        /// on this instance of <see cref="RadVirtualizingDataControl"/>.
        /// </summary>
        protected virtual void OnVerticalScrollBarVisibilityChanged(DependencyPropertyChangedEventArgs args)
        {
            this.verticalScrollBarVisibilityCache = (ScrollBarVisibility)args.NewValue;

            if (this.virtualizationStrategy != null && this.IsProperlyTemplated)
            {
                this.virtualizationStrategy.SynchOwnerScrollViewerForOrientation();
            }
        }

        /// <summary>
        /// Called when the <see cref="ScrollViewer.HorizontalScrollBarVisibility"/> attached property changes
        /// on this instance of <see cref="RadVirtualizingDataControl"/>.
        /// </summary>
        protected virtual void OnHorizontalScrollBarVisibilityChanged(DependencyPropertyChangedEventArgs args)
        {
            this.horizontalScrollBarVisibilityCache = (ScrollBarVisibility)args.NewValue;

            if (this.virtualizationStrategy != null && this.IsProperlyTemplated)
            {
                this.virtualizationStrategy.SynchOwnerScrollViewerForOrientation();
            }
        }

        private static void OnVerticalScrollBarVisibilityChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as RadVirtualizingDataControl).OnVerticalScrollBarVisibilityChanged(args);
        }

        private static void OnHorizontalScrollBarVisibilityChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as RadVirtualizingDataControl).OnHorizontalScrollBarVisibilityChanged(args);
        }
    }
}