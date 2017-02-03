using System;
using Telerik.Core;
using Telerik.UI.Xaml.Controls.Data.DataBoundListBox;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.UI.Xaml.Controls.Data
{
    public partial class RadDataBoundListBox
    {
        /// <summary>
        /// Identifies the <see cref="PullToRefreshIndicatorStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PullToRefreshIndicatorStyleProperty =
            DependencyProperty.Register(nameof(PullToRefreshIndicatorStyle), typeof(Style), typeof(RadDataBoundListBox), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ShowPullToRefreshWhenNoData"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowPullToRefreshWhenNoDataProperty =
            DependencyProperty.Register(nameof(ShowPullToRefreshWhenNoData), typeof(bool), typeof(RadDataBoundListBox), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="IsPullToRefreshEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPullToRefreshEnabledProperty =
            DependencyProperty.Register(nameof(IsPullToRefreshEnabled), typeof(bool), typeof(RadDataBoundListBox), new PropertyMetadata(false, OnIsPullToRefreshActiveChanged));

        internal PullToRefreshIndicatorControl pullToRefreshIndicator;
        internal bool isIndicatorVisible;
        internal bool refreshRequested = false;
        internal bool animating;

        private bool isItemsPanelTranslated = false;
        private Storyboard translateStorybord;
        private DoubleAnimation translateAnimation;
        private Pointer capturedPointer;
        private Point startPoint;
        private bool pointerPressed;
        private double startScrollOffset = -1;

        /// <summary>
        /// Gets or sets a value indicating whether the Pull to Refresh indicator is
        /// visible when there is no data to display in the <see cref="RadDataBoundListBox"/> instance.
        /// </summary>
        public bool ShowPullToRefreshWhenNoData
        {
            get
            {
                return (bool)this.GetValue(ShowPullToRefreshWhenNoDataProperty);
            }
            set
            {
                this.SetValue(ShowPullToRefreshWhenNoDataProperty, value);
            }
        }

        /// <summary>
        /// Stops the loading indicator in the pull-to-refresh element and hides the element by updating
        /// the last refresh time if required. By default the DateTime.Now value is used but by using the UpdateRefreshTime
        /// method a different DateTime value can be provided.
        /// </summary>
        /// <param name="animate">Indicates whether the pull to refresh indicator will be hidden
        /// by animating it.</param>
        /// <param name="updateRefreshLabel">Indicates whether the refresh label is updated.</param>
        public void StopPullToRefreshLoading(bool animate, bool updateRefreshLabel)
        {
            if (this.refreshRequested)
            {
                this.refreshRequested = false;

                if (this.virtualizationStrategy.GetElementCanvasOffset(this.itemsPanel) != 0)
                {
                    if (this.animating && this.translateStorybord != null)
                    {
                        this.translateStorybord.SeekAlignedToLastTick(this.translateAnimation.Duration.TimeSpan);
                    }

                    this.virtualizationStrategy.SetElementCanvasOffset(this.itemsPanel, 0);
                    this.virtualizationStrategy.ScrollToOffset(this.virtualizationStrategy.ScrollOffset, null);
                    this.isItemsPanelTranslated = false;
                }

                this.pullToRefreshIndicator.StopLoading();
                if (updateRefreshLabel)
                {
                    this.pullToRefreshIndicator.UpdateRefreshTime(DateTime.Now);
                }
            }
        }

        /// <summary>
        /// Stops the loading indicator in the pull-to-refresh element and hides the element by updating
        /// the last refresh time. By default the DateTime.Now value is used but a custom value can be provided
        /// as an optional parameter.
        /// </summary>
        public void StopPullToRefreshLoading(bool updateRefreshLabel)
        {
            this.StopPullToRefreshLoading(true, updateRefreshLabel);
        }

        internal override void OnItemRemovedAnimationEnded(RadAnimation animation, SingleItemAnimationContext context)
        {
            base.OnItemRemovedAnimationEnded(animation, context);

            if (this.realizedItems.Count > 0 && this.listSource.Count > 0)
            {
                SingleItemAnimationContext[] animatedItems = this.scheduledRemoveAnimations.ToArray();
                foreach (SingleItemAnimationContext ctxt in animatedItems)
                {
                    if (ctxt.RealizedIndex == 0)
                    {
                        return;
                    }
                }

                this.PositionPullToRefreshIndicator();
            }
        }

        internal void HandlePullToRefreshItemStateChanged(object item, ItemState state)
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                return;
            }

            if (!this.isPullToRefreshEnabledCache)
            {
                return;
            }

            bool showIndicatorWhenNoData = this.ShowPullToRefreshWhenNoData;

            if (this.realizedItems.Count == 0 && !showIndicatorWhenNoData)
            {
                if (state == ItemState.Recycled && !showIndicatorWhenNoData)
                {
                    this.StopPullToRefreshLoading(false);
                    this.HidePullToRefreshIndicator();
                }
                return;
            }

            if (state == ItemState.Realized)
            {
                if (this.listSource.Count == 0 && !showIndicatorWhenNoData)
                {
                    return;
                }

                if ((!this.hasHeader && (this.listSource.GetFirstItem() != null && this.listSource.GetFirstItem().Value == item)) ||
                    (item == RadDataBoundListBox.ListHeaderIndicator.Value) || this.listSource.Count == 0 && showIndicatorWhenNoData)
                {
                    this.ShowPullToRefreshIndicator();

                    if (this.refreshRequested)
                    {
                        this.virtualizationStrategy.CheckTopScrollableBounds();
                        if (!this.pullToRefreshIndicator.IsLoading)
                        {
                            this.pullToRefreshIndicator.StartLoading();
                        }

                        if (!this.isItemsPanelTranslated)
                        {
                            this.isItemsPanelTranslated = this.TryTranslateItemsPanelWithPullToRefreshHeight();
                        }
                    }
                }
            }

            if (state == ItemState.Recycled)
            {
                if (this.listSource.Count > 0)
                {
                    if ((!this.hasHeader && this.listSource.GetFirstItem().Value == item) || item == RadDataBoundListBox.ListHeaderIndicator.Value)
                    {
                        this.HidePullToRefreshIndicator();
                    }
                }
                else if (showIndicatorWhenNoData)
                {
                    this.ShowPullToRefreshIndicator();
                }
            }
        }

        internal void HidePullToRefreshIndicator()
        {
            this.isIndicatorVisible = false;
            if (!this.IsProperlyTemplated)
            {
                return;
            }

            this.pullToRefreshIndicator.Visibility = Visibility.Collapsed;
        }

        internal void ShowPullToRefreshIndicator()
        {
            this.isIndicatorVisible = true;
            if (!this.IsProperlyTemplated)
            {
                return;
            }

            this.pullToRefreshIndicator.Visibility = Visibility.Visible;
            this.pullToRefreshIndicator.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            this.PositionPullToRefreshIndicator();
        }

        internal void PositionPullToRefreshIndicator()
        {
            bool showIndicator = this.ShowPullToRefreshWhenNoData;
            if (this.listSource.Count > 0 || showIndicator)
            {
                if (this.virtualizationStrategy.LayoutOrientation == Orientation.Vertical)
                {
                    this.pullToRefreshIndicator.Width = this.availableWidth;
                    if (!showIndicator)
                    {
                        Canvas.SetTop(this.pullToRefreshIndicator, this.virtualizationStrategy.GetElementCanvasOffset(this.firstItemCache) - this.pullToRefreshIndicator.DesiredSize.Height);
                    }
                    else
                    {
                        Canvas.SetTop(this.pullToRefreshIndicator, -this.pullToRefreshIndicator.DesiredSize.Height);
                    }

                    Canvas.SetLeft(this.pullToRefreshIndicator, 0);
                    VisualStateManager.GoToState(this.pullToRefreshIndicator, "Vertical", false);
                }
                else
                {
                    this.pullToRefreshIndicator.Width = this.availableHeight;

                    if (!showIndicator)
                    {
                        Canvas.SetLeft(this.pullToRefreshIndicator, this.virtualizationStrategy.GetElementCanvasOffset(this.firstItemCache) - (this.availableHeight + this.pullToRefreshIndicator.DesiredSize.Height));
                    }
                    else
                    {
                        Canvas.SetLeft(this.pullToRefreshIndicator, -(this.availableHeight + this.pullToRefreshIndicator.DesiredSize.Height));
                    }

                    Canvas.SetTop(this.pullToRefreshIndicator, 0);
                    VisualStateManager.GoToState(this.pullToRefreshIndicator, "Horizontal", false);
                }
            }
        }

        internal void TranslateItemsPanel(double offset)
        {
            this.translateStorybord = new Storyboard();
            this.translateStorybord.Completed += this.OnItemsPanelTranslateStoryboard_Completed;
            this.translateAnimation = new DoubleAnimation();
            this.translateAnimation.EnableDependentAnimation = false;
            this.translateStorybord.Children.Add(this.translateAnimation);

            Storyboard.SetTarget(this.translateAnimation, this.itemsPanel);

            if (this.virtualizationStrategy.LayoutOrientation == Orientation.Vertical)
            {
                Storyboard.SetTargetProperty(this.translateAnimation, "(Canvas.Top)");
            }
            else
            {
                Storyboard.SetTargetProperty(this.translateAnimation, "(Canvas.Left)");
            }

            this.translateAnimation.To = offset;
            this.translateAnimation.Duration = TimeSpan.FromMilliseconds(550);
            this.translateAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            this.translateStorybord.Children.Clear();
            this.translateStorybord.Children.Add(this.translateAnimation);
            this.translateStorybord.Begin();
            this.animating = true;
        }

        internal override bool CanBalance(BalanceOperationType operationType)
        {
            if (operationType != BalanceOperationType.TopBoundsCheck)
            {
                return base.CanBalance(operationType);
            }

            if ((this.refreshRequested && this.isIndicatorVisible) ||
                (this.animating && this.isIndicatorVisible))
            {
                return false;
            }

            return base.CanBalance(operationType);
        }

        /// <summary>
        /// Fires the <see cref="RefreshRequested"/> event.
        /// </summary>
        protected virtual void OnRefreshRequested()
        {
            this.refreshRequested = true;
            this.isItemsPanelTranslated = false;
            this.pullToRefreshIndicator.StartLoading();

            if (this.RefreshRequested != null)
            {
                this.RefreshRequested(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Called when the virtualization state of a given data item is changed.
        /// </summary>
        /// <param name="item">The data item that has an updated state.</param>
        /// <param name="state">The new state.</param>
        protected override void OnItemStateChanged(object item, ItemState state)
        {
            base.OnItemStateChanged(item, state);

            this.HandleItemReorderItemStateChanged(item, state);
            this.HandlePullToRefreshItemStateChanged(item, state);
        }

        private static void OnIsPullToRefreshActiveChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as RadDataBoundListBox).OnIsPullToRefreshActiveChanged(args);
        }

        private void CompositionTarget_Rendering(object sender, object e)
        {
            UIElement scrollViewerPresenter = this.manipulationContainer.Content as UIElement;
            Point offset = scrollViewerPresenter.TransformToVisual(this.manipulationContainer).TransformPoint(new Point());

            var startThreshold = 10;

            if (this.virtualizationStrategy.LayoutOrientation == Orientation.Vertical)
            {
                if (this.refreshRequested && offset.Y <= startThreshold && this.IsPullToRefreshEnabled)
                {
                    this.isItemsPanelTranslated = this.TryTranslateItemsPanelWithPullToRefreshHeight();
                }
                else if (!this.refreshRequested && offset.Y > 0 && this.IsPullToRefreshEnabled)
                {
                    scrollViewerPresenter.CancelDirectManipulations();
                }
            }
            else
            {
                if (this.refreshRequested && offset.X <= startThreshold && this.IsPullToRefreshEnabled)
                {
                    this.isItemsPanelTranslated = this.TryTranslateItemsPanelWithPullToRefreshHeight();
                }
                else if (!this.refreshRequested && offset.X > 0 && this.IsPullToRefreshEnabled)
                {
                    scrollViewerPresenter.CancelDirectManipulations();
                }
            }
        }

        private void ScrollableContent_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (this.IsPullToRefreshEnabled)
            {
                this.startPoint = e.GetCurrentPoint(this.scrollableContent).Position;
                this.pointerPressed = true;
                this.startScrollOffset = this.virtualizationStrategy.LayoutOrientation == Orientation.Vertical ? this.manipulationContainer.VerticalOffset : this.manipulationContainer.HorizontalOffset;
            }
        }

        private void ScrollViewerPresenter_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (!this.IsPullToRefreshEnabled)
            {
                return;
            }

            if (!this.refreshRequested && this.startScrollOffset == 0 && this.pointerPressed)
            {
                var offsetPoint = e.GetCurrentPoint(this.scrollableContent).Position;
                var offset = this.virtualizationStrategy.LayoutOrientation == Orientation.Vertical ? offsetPoint.Y : offsetPoint.X;
                var startLocation = this.virtualizationStrategy.LayoutOrientation == Orientation.Vertical ? this.startPoint.Y : this.startPoint.X;

                this.scrollableContent.CancelDirectManipulations();

                if (this.capturedPointer == null)
                {
                    this.capturedPointer = e.Pointer;
                    this.scrollableContent.CapturePointer(this.capturedPointer);
                }

                this.virtualizationStrategy.SetElementCanvasOffset(this.itemsPanel, offset - startLocation);

                if (offset - startLocation > this.PullToRefreshScrollOffsetThreshold)
                {
                    if (this.capturedPointer != null)
                    {
                        this.scrollableContent.ReleasePointerCaptures();
                        this.capturedPointer = null;
                    }

                    this.pointerPressed = false;

                    this.OnRefreshRequested();
                }
            }
        }

        private void ScrollableContent_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (!this.IsPullToRefreshEnabled)
            {
                return;
            }

            if (this.capturedPointer != null)
            {
                this.scrollableContent.ReleasePointerCaptures();
                this.capturedPointer = null;
            }

            this.pointerPressed = false;
            this.startScrollOffset = -1;

            if (!this.refreshRequested)
            {
                this.TranslateItemsPanel(0);
            }
        }

        private void OnIsPullToRefreshActiveChanged(DependencyPropertyChangedEventArgs args)
        {
            this.isPullToRefreshEnabledCache = (bool)args.NewValue;

            if (this.isPullToRefreshEnabledCache)
            {
                if (this.realizedItems.Count > 0 &&
                    this.IsFirstItemFirstInListSource() || this.ShowPullToRefreshWhenNoData)
                {
                    this.ShowPullToRefreshIndicator();
                }
            }
            else
            {
                if (this.refreshRequested)
                {
                    this.StopPullToRefreshLoading(false);
                }
                this.HidePullToRefreshIndicator();
            }
        }

        private void OnItemsPanelTranslateStoryboard_Completed(object sender, object e)
        {
            this.animating = false;

            if ((this.manipulationContainer.VerticalOffset == 0 && this.virtualizationStrategy.LayoutOrientation == Orientation.Vertical ||
                 this.manipulationContainer.HorizontalOffset == 0 && this.virtualizationStrategy.LayoutOrientation == Orientation.Horizontal) &&
                !this.isCompositionTargetRenderListening)
            {
                CompositionTarget.Rendering += this.CompositionTarget_Rendering;
                this.isCompositionTargetRenderListening = true;
            }
        }

        private bool TryTranslateItemsPanelWithPullToRefreshHeight()
        {
            var currentOffset = this.virtualizationStrategy.GetElementCanvasOffset(this.itemsPanel);
            if (RadMath.AreClose(currentOffset, 0))
            {
                var length = this.virtualizationStrategy.LayoutOrientation == Orientation.Vertical ? this.pullToRefreshIndicator.DesiredSize.Height : this.pullToRefreshIndicator.DesiredSize.Width;

                this.TranslateItemsPanel(length);

                return true;
            }

            return false;
        }
    }
}