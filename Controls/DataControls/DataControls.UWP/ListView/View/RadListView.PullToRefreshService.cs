using System;
using Telerik.UI.Xaml.Controls.Data.Common;
using Telerik.UI.Xaml.Controls.Data.ListView;
using Telerik.UI.Xaml.Controls.Data.ListView.Commands;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data
{
    public partial class RadListView : IPullToRefreshListener
    {
        /// <summary>
        /// Identifies the <see cref="IsPullToRefreshEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPullToRefreshEnabledProperty =
            DependencyProperty.Register(nameof(IsPullToRefreshEnabled), typeof(bool), typeof(RadListView), new PropertyMetadata(false, OnIsPullToRefreshEnabledChanged));

        /// <summary>
        /// Identifies the <see cref="IsPullToRefreshActive"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPullToRefreshActiveProperty =
            DependencyProperty.Register(nameof(IsPullToRefreshActive), typeof(bool), typeof(RadListView), new PropertyMetadata(false, OnIsPullToRefreshActiveChanged));

        /// <summary>
        /// Identifies the <see cref="PullToRefreshScrollMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PullToRefreshScrollModeProperty =
            DependencyProperty.Register(nameof(PullToRefreshScrollMode), typeof(PullToRefreshScrollMode), typeof(RadListView), new PropertyMetadata(PullToRefreshScrollMode.ContentAndIndicator, OnPullToRefreshScrollModeChanged));

        private const string RefreshingState = "Refreshing";
        private const string ReadyState = "Ready";
        private const string NormalState = "Normal";

        private PullToRefreshGestureRecognizer gestureRecognizer;
        private double refreshThreshold;

        /// <summary>
        /// Occurs when Pull to refresh is initiated.
        /// </summary>
        public event EventHandler RefreshRequested;

        /// <summary>
        /// Gets or sets a value indicating whether the pull to refresh is enabled.
        /// </summary>
        public bool IsPullToRefreshEnabled
        {
            get { return (bool)GetValue(IsPullToRefreshEnabledProperty); }
            set { SetValue(IsPullToRefreshEnabledProperty, value); }
        }

        /// <summary>
        /// Gets or sets the scroll mode of pull to refresh indicator.
        /// </summary>
        public PullToRefreshScrollMode PullToRefreshScrollMode
        {
            get { return (PullToRefreshScrollMode)GetValue(PullToRefreshScrollModeProperty); }
            set { SetValue(PullToRefreshScrollModeProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the is pull to refresh active.
        /// </summary>
        /// <value>
        /// <c>true</c> if the pull to refresh is active; otherwise, <c>false</c>.
        /// </value>
        public bool IsPullToRefreshActive
        {
            get
            {
                return (bool)GetValue(IsPullToRefreshActiveProperty);
            }
            set
            {
                SetValue(IsPullToRefreshActiveProperty, value);
            }
        }

        ScrollViewer IPullToRefreshListener.ScrollViewer
        {
            get { return this.scrollViewer; }
        }

        FrameworkElement IPullToRefreshListener.CompressedChildToTranslate
        {
            get { return this.childrenPanel; }
        }

        FrameworkElement IPullToRefreshListener.MainElementToTranslate
        {
            get { return this.PullToRefreshScrollMode == PullToRefreshScrollMode.ContentAndIndicator ? (FrameworkElement)this.childrenPanel : this.pullToRefreshIndicator; }
        }

        Orientation IPullToRefreshListener.Orientation
        {
            get
            {
                return this.Orientation;
            }
        }

        void IPullToRefreshListener.OnStarted()
        {
        }

        void IPullToRefreshListener.OnEnded()
        {
            if (this.IsTemplateApplied)
            {
                this.pullToRefreshIndicator.CurrentPullOffset = 0;
                this.pullToRefreshIndicator.GoToState(true, NormalState);
            }
        }

        void IPullToRefreshListener.OnRefreshRequested()
        {
            this.IsPullToRefreshActive = true;
            this.pullToRefreshIndicator.GoToState(true, RefreshingState);
        }

        void IPullToRefreshListener.OnOffsetChanged(double offset)
        {
            if (this.IsTemplateApplied)
            {
                this.pullToRefreshIndicator.CurrentPullOffset = offset;
            }
        }

        internal void RequestRefresh()
        {
            if (!this.commandService.ExecuteCommand(CommandId.RefreshRequested, null))
            {
                this.gestureRecognizer.IsPullToRefreshCancelled = true;
                this.IsPullToRefreshActive = false;
            }
        }

        internal void OnRefreshRequested(object parameter)
        {
            this.gestureRecognizer.RequrestRefreshState();
            this.CleanupSwipedItem();
            var handler = this.RefreshRequested;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        internal void PrepareRefresh()
        {
            if (this.IsTemplateApplied)
            {
                this.pullToRefreshIndicator.GoToState(true, ReadyState);
            }
        }

        internal void ResetRefresh()
        {
            if (this.IsTemplateApplied)
            {
                if (this.gestureRecognizer != null)
                {
                    this.gestureRecognizer.ResetPullToRefresh();
                }
            }
        }

        internal void PositionPullToRefreshIndicator()
        {
            if (this.IsTemplateApplied)
            {
                this.pullToRefreshIndicator.SetDisplayMode(this.PullToRefreshScrollMode);

                this.pullToRefreshIndicator.SetOrientation(this.Orientation);

                if (!this.IsPullToRefreshActive)
                {
                    if (this.Orientation == Orientation.Vertical)
                    {
                        Canvas.SetTop(this.pullToRefreshIndicator, -this.pullToRefreshIndicator.DesiredSize.Height);
                    }

                    this.pullToRefreshIndicator.CurrentPullOffset = 0;
                }
            }
        }

        internal void InitializePullToRefresh()
        {
            if (this.IsTemplateApplied)
            {
                this.gestureRecognizer = new PullToRefreshGestureRecognizer(this);
                this.gestureRecognizer.IsEnabled = this.IsPullToRefreshEnabled;

                this.pullToRefreshIndicator.SetDisplayMode(this.PullToRefreshScrollMode);
            }
        }

        private static void OnIsPullToRefreshEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listView = d as RadListView;
            if (listView.IsTemplateApplied)
            {
                listView.pullToRefreshIndicator.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;

                if (listView.gestureRecognizer != null)
                {
                    listView.gestureRecognizer.IsEnabled = (bool)e.NewValue;
                }
            }
        }

        private static void OnPullToRefreshScrollModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var list = d as RadListView;

            if (list.gestureRecognizer != null)
            {
                list.UpdatePullToRefreshSize();
                list.gestureRecognizer.Dispose();
            }

            list.InitializePullToRefresh();

            if (list.gestureRecognizer != null)
            {
                list.gestureRecognizer.SwipeTheshold = list.refreshThreshold;
            }
        }

        private static void OnIsPullToRefreshActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listView = d as RadListView;

            if (listView.IsTemplateApplied)
            {
                if ((bool)e.NewValue)
                {
                    listView.RequestRefresh();
                }
                else
                {
                    listView.ResetRefresh();
                }
            }
        }

        private void UpdatePullToRefreshSize()
        {
            if (this.Orientation == Orientation.Vertical)
            {
                Canvas.SetLeft(this.pullToRefreshIndicator, 0);
                this.pullToRefreshIndicator.Width = this.scrollViewer.DesiredSize.Width;
            }
            else
            {
                this.pullToRefreshIndicator.Width = this.scrollViewer.DesiredSize.Height;
                Canvas.SetTop(this.pullToRefreshIndicator, this.pullToRefreshIndicator.Width);
            }

            if (this.gestureRecognizer != null)
            {
                this.gestureRecognizer.SwipeTheshold = this.pullToRefreshIndicator.DesiredSize.Height;
                this.gestureRecognizer.UpdateInitialChildOffset();
            }
        }

        private void GestureRecognizer_PullToRefreshRequested(object sender, EventArgs e)
        {
            this.IsPullToRefreshActive = true;
        }

        private void PullToRefreshIndicator_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.gestureRecognizer != null)
            {
                this.gestureRecognizer.SwipeTheshold = e.NewSize.Height;
            }

            this.refreshThreshold = e.NewSize.Height;

            if (e.PreviousSize == new Windows.Foundation.Size(0, 0))
            {
                this.UpdatePullToRefreshSize();
            }

            this.PositionPullToRefreshIndicator();
        }
    }
}
