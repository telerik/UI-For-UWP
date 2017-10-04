using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.UI.Xaml.Controls.Data.Common
{
    internal class PullToRefreshGestureRecognizer : IDisposable
    {
        private static TimeSpan defaultAnimationDuration = new TimeSpan(0, 0, 0, 0, 500);

        private bool animate = true;

        private FrameworkElement rootPanel;
        private IPullToRefreshListener listener;

        private SwipeEaseFunction swipeFunction;

        private bool attachedToRendering;
        private double thresholdToStartPointerTracking = 10;
        private bool thresholdToStartPointerTrackininEnabled = false;
        private bool isIntermidiateChange = false;

        private double rootOffset;
        private Pointer capturedPointer;
        private double startLocation;

        private Storyboard runningAnimation;

        private bool pointerPressed = false;

        private double initialChildOffset = 0;

        public PullToRefreshGestureRecognizer(IPullToRefreshListener listener)
        {
            this.listener = listener;

            this.Attach(listener.ScrollViewer.Content as FrameworkElement);

            this.SwipeTheshold = 50;
        }

        public bool IsEnabled { get; set; }

        public double SwipeTheshold { get; set; }

        public bool IsPullToRefreshCancelled { get; set; }

        public Orientation Orientation
        {
            get
            {
                return this.listener == null ? Orientation.Vertical : this.listener.Orientation;
            }
        }

        internal bool RefreshRequested
        {
            get;
            private set;
        }

        private bool IsHorizontal
        {
            get
            {
                return this.Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal;
            }
        }

        public void Dispose()
        {
            this.Reset();

            this.Detach();
        }

        internal void UpdateInitialChildOffset()
        {
            if (this.initialChildOffset == 0 && this.listener.CompressedChildToTranslate != this.listener.MainElementToTranslate)
            {
                this.initialChildOffset = this.IsHorizontal ? Canvas.GetLeft(this.listener.MainElementToTranslate) : Canvas.GetTop(this.listener.MainElementToTranslate);
            }
        }

        internal void RequrestRefreshState()
        {
            if (!this.RefreshRequested && this.listener.MainElementToTranslate != null)
            {
                var currentThreshold = this.SwipeTheshold - this.initialChildOffset;

                if (this.Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal)
                {
                    if (Canvas.GetLeft(this.listener.MainElementToTranslate) < currentThreshold)
                    {
                        Canvas.SetLeft(this.listener.MainElementToTranslate, currentThreshold);
                    }
                }
                else
                {
                    if (Canvas.GetTop(this.listener.MainElementToTranslate) < currentThreshold)
                    {
                        Canvas.SetTop(this.listener.MainElementToTranslate, currentThreshold);
                    }
                }

                this.RefreshRequested = true;

                this.listener.OnRefreshRequested();
            }
        }

        internal void ResetPullToRefresh()
        {
            this.Reset();
        }

        private void StopTrackingPointer()
        {
        }

        private void scrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            this.isIntermidiateChange = e.IsIntermediate;
        }

        private void CompositionTarget_Rendering(object sender, object e)
        {
            var scrollViewerInStartPosition = !this.IsHorizontal && this.listener.ScrollViewer.VerticalOffset == 0 ||
this.IsHorizontal && this.listener.ScrollViewer.HorizontalOffset == 0;

            if (this.rootPanel == null ||
                this.listener.CompressedChildToTranslate == null ||
                this.listener.ScrollViewer == null ||
                !scrollViewerInStartPosition ||
                !this.IsEnabled)
            {
                this.Reset(false);
                return;
            }

            var transform = this.rootPanel.TransformToVisual(this.listener.ScrollViewer);
            var point = transform.TransformPoint(new Point(0, 0));
            var offset = this.Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal ? point.X : point.Y;

            if (this.isIntermidiateChange)
            {
                return;
            }

            if (this.thresholdToStartPointerTracking < offset && this.pointerPressed)
            {
                this.thresholdToStartPointerTrackininEnabled = true;
                this.rootOffset = offset;

                this.DetachFromRendering();

                if (this.Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal)
                {
                    Canvas.SetLeft(this.listener.CompressedChildToTranslate, offset);
                }
                else
                {
                    Canvas.SetTop(this.listener.CompressedChildToTranslate, offset);
                }

                this.rootPanel.CancelDirectManipulations();
            }
        }

        private void Attach(FrameworkElement rootPanel)
        {
            if (rootPanel == null || this.listener == null)
            {
                return;
            }

            this.listener.ScrollViewer.ViewChanged += this.scrollViewer_ViewChanged;

            this.rootPanel = rootPanel;

            this.rootPanel.PointerPressed += this.ManipulationPanel_PointerPressed;
            this.rootPanel.PointerMoved += this.ManipulationPanel_PointerMoved;
            this.rootPanel.PointerReleased += this.ManipulationPanel_PointerReleased;
        }

        private void Detach()
        {
            if (this.rootPanel == null || this.listener == null)
            {
                return;
            }

            this.listener.ScrollViewer.ViewChanged -= this.scrollViewer_ViewChanged;

            this.rootPanel.PointerPressed -= this.ManipulationPanel_PointerPressed;
            this.rootPanel.PointerMoved -= this.ManipulationPanel_PointerMoved;
            this.rootPanel.PointerReleased -= this.ManipulationPanel_PointerReleased;
        }

        private void Reset(bool cancelManipulations = true)
        {
            this.DetachFromRendering();
            if (this.listener.CompressedChildToTranslate != null && this.listener.MainElementToTranslate != null)
            {
                if (this.animate)
                {
                    this.ResetWithAnimation();
                }
                else
                {
                    if (this.listener.CompressedChildToTranslate != this.listener.MainElementToTranslate)
                    {
                        Canvas.SetLeft(this.listener.CompressedChildToTranslate, 0);
                        Canvas.SetTop(this.listener.CompressedChildToTranslate, 0);
                    }

                    if (this.IsHorizontal)
                    {
                        Canvas.SetLeft(this.listener.MainElementToTranslate, this.initialChildOffset);
                    }
                    else
                    {
                        Canvas.SetTop(this.listener.MainElementToTranslate, this.initialChildOffset);
                    }

                    if (this.capturedPointer != null)
                    {
                        this.rootPanel.ReleasePointerCaptures();
                        this.capturedPointer = null;
                    }

                    this.thresholdToStartPointerTrackininEnabled = false;

                    this.pointerPressed = false;

                    this.listener.OnEnded();

                    this.RefreshRequested = false;
                }

                if (this.listener.ScrollViewer != null && cancelManipulations)
                {
                    this.listener.ScrollViewer.CancelDirectManipulations();
                }
            }
        }

        private void ResetWithAnimation()
        {
            var transform = this.rootPanel.TransformToVisual(this.listener.ScrollViewer);
            var point = transform.TransformPoint(new Point(0, 0));

            double offset = 0;
            double mainOffsetChild = 0;

            if (this.Orientation == Windows.UI.Xaml.Controls.Orientation.Vertical)
            {
                offset = Canvas.GetTop(this.listener.CompressedChildToTranslate) + point.Y;
                mainOffsetChild = Canvas.GetTop(this.listener.MainElementToTranslate) + point.Y;
            }
            else
            {
                offset = Canvas.GetLeft(this.listener.CompressedChildToTranslate) + point.X;
                mainOffsetChild = Canvas.GetLeft(this.listener.MainElementToTranslate) + point.X;
            }

            Action completed = () =>
                {
                    this.initialChildOffset = 0;

                    if (this.capturedPointer != null)
                    {
                        this.rootPanel.ReleasePointerCaptures();
                        this.capturedPointer = null;
                    }

                    this.pointerPressed = false;
                    this.thresholdToStartPointerTrackininEnabled = false;

                    this.RefreshRequested = false;
                    this.listener.OnEnded();
                };

            if (this.listener.CompressedChildToTranslate != this.listener.MainElementToTranslate)
            {
                this.AnimatePanel(this.listener.CompressedChildToTranslate, offset, 0, null);
            }

            this.AnimatePanel(this.listener.MainElementToTranslate, mainOffsetChild, this.initialChildOffset, completed, true);
        }

        private void AnimatePanel(FrameworkElement element, double from, double to, Action completeAction, bool mainAnimation = false)
        {
            Storyboard b = new Storyboard();

            if (this.runningAnimation != null && mainAnimation)
            {
                this.runningAnimation.Stop();
                this.runningAnimation = b;
            }

            var topAnimation = new DoubleAnimation();
            topAnimation.Duration = defaultAnimationDuration;
            topAnimation.EasingFunction = new QuinticEase() { EasingMode = EasingMode.EaseOut };
            topAnimation.From = from;
            topAnimation.To = to;
            Storyboard.SetTarget(topAnimation, element);

            if (this.Orientation == Windows.UI.Xaml.Controls.Orientation.Vertical)
            {
                Storyboard.SetTargetProperty(topAnimation, "(Canvas.Top)");
            }
            else
            {
                Storyboard.SetTargetProperty(topAnimation, "(Canvas.Left)");
            }

            b.Children.Add(topAnimation);

            if (completeAction != null)
            {
                b.Completed += (s, e) => completeAction();
            }

            b.Begin();
        }

        private void AttachToRendering()
        {
            if (!this.attachedToRendering)
            {
                this.attachedToRendering = true;
                CompositionTarget.Rendering += this.CompositionTarget_Rendering;

                this.swipeFunction = new SwipeEaseFunction(this.SwipeTheshold);
            }
        }

        private void DetachFromRendering()
        {
            this.attachedToRendering = false;
            CompositionTarget.Rendering -= this.CompositionTarget_Rendering;
        }

        private void ManipulationPanel_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var scrollViewerInStartPosition = !this.IsHorizontal && this.listener.ScrollViewer.VerticalOffset == 0 ||
                this.IsHorizontal && this.listener.ScrollViewer.HorizontalOffset == 0;

            if (scrollViewerInStartPosition &&
                this.IsEnabled &&
                !this.RefreshRequested)
            {
                this.UpdateInitialChildOffset();

                this.pointerPressed = true;
                this.AttachToRendering();
                this.listener.OnStarted();
            }
        }

        private void ManipulationPanel_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (!this.IsEnabled || !this.pointerPressed)
            {
                return;
            }
            
            if (this.RefreshRequested && !this.IsPullToRefreshCancelled)
            {
                var offset = this.Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal ? Canvas.GetLeft(this.listener.CompressedChildToTranslate) : Canvas.GetTop(this.listener.CompressedChildToTranslate);

                var mainElementOffset = this.Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal ? Canvas.GetLeft(this.listener.MainElementToTranslate) : Canvas.GetTop(this.listener.MainElementToTranslate);

                this.AnimatePanel(this.listener.MainElementToTranslate, mainElementOffset, this.SwipeTheshold + this.initialChildOffset, null, true);
            }
            else
            {
                this.Reset();
            }

            if (this.IsPullToRefreshCancelled)
            {
                this.IsPullToRefreshCancelled = false;
            }
           
            this.pointerPressed = false;
        }

        private void ManipulationPanel_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (!this.IsEnabled || this.IsPullToRefreshCancelled)
            {
                return;
            }

            var scrollViewerInStartPosition = !this.IsHorizontal && this.listener.ScrollViewer.VerticalOffset == 0 ||
    this.IsHorizontal && this.listener.ScrollViewer.HorizontalOffset == 0;

            if (scrollViewerInStartPosition &&
                this.pointerPressed &&
                this.thresholdToStartPointerTrackininEnabled)
            {
                this.rootPanel.CancelDirectManipulations();

                if (this.capturedPointer == null)
                {
                    var pointerPoint = e.GetCurrentPoint(this.rootPanel).Position;
                    this.startLocation = this.Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal ? pointerPoint.X : pointerPoint.Y;

                    this.startLocation -= this.rootOffset;

                    this.capturedPointer = e.Pointer;
                    this.rootPanel.CapturePointer(this.capturedPointer);
                }

                var curentPoint = e.GetCurrentPoint(this.rootPanel).Position;
                var offset = this.Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal ? curentPoint.X - this.startLocation : curentPoint.Y - this.startLocation;

                var acceleratedOffset = offset + this.initialChildOffset;

                this.listener.OnOffsetChanged(acceleratedOffset);

                if (this.Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal)
                {
                    Canvas.SetLeft(this.listener.MainElementToTranslate, acceleratedOffset);
                }
                else
                {
                    Canvas.SetTop(this.listener.MainElementToTranslate, acceleratedOffset);
                }

                if (this.listener.MainElementToTranslate != this.listener.CompressedChildToTranslate)
                {
                    var slowedOffset = Math.Log(offset, 1.5) + 10;

                    if (this.Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal)
                    {
                        Canvas.SetLeft(this.listener.CompressedChildToTranslate, slowedOffset);
                    }
                    else
                    {
                        Canvas.SetTop(this.listener.CompressedChildToTranslate, slowedOffset);
                    }
                }

                if (offset > this.SwipeTheshold * 2)
                {
                    this.RequrestRefreshState();
                }
            }
        }
    }
}
