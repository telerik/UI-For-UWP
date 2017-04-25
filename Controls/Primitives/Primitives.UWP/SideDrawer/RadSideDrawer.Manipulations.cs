using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Telerik.UI.Xaml.Controls.Primitives.SideDrawer.Commands;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    public partial class RadSideDrawer
    {
        /// <summary>
        /// Identifies the <see cref="TouchTargetThreshold"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TouchTargetThresholdProperty =
            DependencyProperty.Register(nameof(TouchTargetThreshold), typeof(double), typeof(RadSideDrawer), new PropertyMetadata(20d));

        internal static readonly DependencyProperty ContextProperty =
            DependencyProperty.Register(nameof(Context), typeof(AnimationContext), typeof(RadSideDrawer), new PropertyMetadata(new AnimationContext(), OnContextChanged));

        private bool shouldAnimate;

        /// <summary>
        /// Gets or sets the distance from the main content's edge(depending on the DrawerLocation) in pixels which is responsive to gestures(to open/move/close the drawer).
        /// </summary>
        public double TouchTargetThreshold
        {
            get { return (double)GetValue(TouchTargetThresholdProperty); }
            set { SetValue(TouchTargetThresholdProperty, value); }
        }

        internal AnimationContext Context
        {
            get { return (AnimationContext)GetValue(ContextProperty); }
            set { SetValue(ContextProperty, value); }
        }

        /// <summary>
        /// Called before the KeyDown event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Windows.System.VirtualKey.Space || e.Key == Windows.System.VirtualKey.Enter)
            {
                this.ToggleDrawer();
                e.Handled = true;
            }
        }

        private static void OnContextChanged(DependencyObject owner, DependencyPropertyChangedEventArgs args)
        {
            var drawer = owner as RadSideDrawer;
            var oldValue = args.OldValue as AnimationContext;
            var newValue = args.NewValue as AnimationContext;

            if (oldValue != null)
            {
                if (oldValue.DrawerStoryBoard != null)
                {
                    oldValue.DrawerStoryBoard.Completed -= drawer.DrawerStoryBoard_Completed;
                }

                if (oldValue.DrawerStoryBoardReverse != null)
                {
                    oldValue.DrawerStoryBoardReverse.Completed -= drawer.DrawerStoryBoardReverse_Completed;
                }
            }

            if (newValue != null)
            {
                if (newValue.DrawerStoryBoard != null)
                {
                    newValue.DrawerStoryBoard.Completed += drawer.DrawerStoryBoard_Completed;
                }

                if (newValue.DrawerStoryBoardReverse != null)
                {
                    newValue.DrawerStoryBoardReverse.Completed += drawer.DrawerStoryBoardReverse_Completed;
                }
            }
        }

        private void DrawerStoryBoard_Completed(object sender, object e)
        {
            this.DrawerState = DrawerState.Opened;
        }

        private void DrawerStoryBoardReverse_Completed(object sender, object e)
        {
            this.DrawerState = DrawerState.Closed;
        }

        private void SubscribeToManipulationEvents()
        {
            if (this.IsOpen)
            {
                this.drawer.ManipulationStarted += this.Drawer_ManipulationStarted;
                this.drawer.ManipulationDelta += this.Drawer_ManipulationDelta;
                this.drawer.ManipulationCompleted += this.Drawer_ManipulationCompleted;
            }
            else
            {
                this.mainContent.ManipulationStarted += this.MainContent_ManipulationStarted;
                this.mainContent.ManipulationDelta += this.MainContent_ManipulationDelta;
                this.mainContent.ManipulationCompleted += this.MainContent_ManipulationCompleted;
            }

            this.swipeAreaElement.ManipulationStarted += this.MainContent_ManipulationStarted;
            this.swipeAreaElement.ManipulationDelta += this.MainContent_ManipulationDelta;
            this.swipeAreaElement.ManipulationCompleted += this.MainContent_ManipulationCompleted;
        }

        private void UnSubscribeToManipulationEvents()
        {
            if (this.IsOpen)
            {
                this.mainContent.ManipulationStarted -= this.MainContent_ManipulationStarted;
                this.mainContent.ManipulationDelta -= this.MainContent_ManipulationDelta;
                this.mainContent.ManipulationCompleted -= this.MainContent_ManipulationCompleted;
            }
            else
            {
                this.drawer.ManipulationStarted -= this.Drawer_ManipulationStarted;
                this.drawer.ManipulationDelta -= this.Drawer_ManipulationDelta;
                this.drawer.ManipulationCompleted -= this.Drawer_ManipulationCompleted;
            }

            this.swipeAreaElement.ManipulationStarted -= this.MainContent_ManipulationStarted;
            this.swipeAreaElement.ManipulationDelta -= this.MainContent_ManipulationDelta;
            this.swipeAreaElement.ManipulationCompleted -= this.MainContent_ManipulationCompleted;
        }

        private void Drawer_ManipulationCompleted(object sender, Windows.UI.Xaml.Input.ManipulationCompletedRoutedEventArgs e)
        {
            if (this.shouldAnimate)
            {
                double cumulativeTranslation = 0;
                double animationDistance = 0;
                switch (this.DrawerLocation)
                {
                    case DrawerLocation.Left:
                        cumulativeTranslation = -e.Cumulative.Translation.X;
                        animationDistance = this.drawer.Width;
                        break;
                    case DrawerLocation.Right:
                        cumulativeTranslation = e.Cumulative.Translation.X;
                        animationDistance = this.drawer.Width;
                        break;
                    case DrawerLocation.Top:
                        cumulativeTranslation = -e.Cumulative.Translation.Y;
                        animationDistance = this.drawer.Height;
                        break;
                    case DrawerLocation.Bottom:
                        cumulativeTranslation = e.Cumulative.Translation.Y;
                        animationDistance = this.drawer.Height;
                        break;
                }

                if (cumulativeTranslation > animationDistance / 3)
                {
                    this.Context.MainContentStoryBoardReverse.Resume();
                    this.Context.DrawerStoryBoardReverse.Resume();

                    this.IsOpen = false;
                }
                else
                {
                    var offset = (cumulativeTranslation / animationDistance) * this.AnimationDuration.TimeSpan.Milliseconds;
                    this.Context.MainContentStoryBoard.Begin();
                    this.Context.MainContentStoryBoard.Pause();
                    this.Context.DrawerStoryBoard.Begin();
                    this.Context.DrawerStoryBoard.Pause();

                    this.Context.MainContentStoryBoard.Seek(TimeSpan.FromMilliseconds(this.AnimationDuration.TimeSpan.Milliseconds - offset));
                    this.Context.DrawerStoryBoard.Seek(TimeSpan.FromMilliseconds(this.AnimationDuration.TimeSpan.Milliseconds - offset));

                    this.Context.MainContentStoryBoard.Resume();
                    this.Context.DrawerStoryBoard.Resume();

                    this.IsOpen = true;
                }
            }

            this.shouldAnimate = false;
        }

        private void Drawer_ManipulationDelta(object sender, Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        {
            if (this.shouldAnimate)
            {
                double cumulativeTranslation = 0;
                double animationDistance = 0;
                switch (this.DrawerLocation)
                {
                    case DrawerLocation.Left:
                        cumulativeTranslation = -e.Cumulative.Translation.X;
                        animationDistance = this.drawer.Width;
                        break;
                    case DrawerLocation.Right:
                        cumulativeTranslation = e.Cumulative.Translation.X;
                        animationDistance = this.drawer.Width;
                        break;
                    case DrawerLocation.Top:
                        cumulativeTranslation = -e.Cumulative.Translation.Y;
                        animationDistance = this.drawer.Height;
                        break;
                    case DrawerLocation.Bottom:
                        cumulativeTranslation = e.Cumulative.Translation.Y;
                        animationDistance = this.drawer.Height;
                        break;
                }

                var offset = (cumulativeTranslation / animationDistance) * this.AnimationDuration.TimeSpan.Milliseconds;

                if (offset > this.AnimationDuration.TimeSpan.Milliseconds)
                {
                    offset = this.AnimationDuration.TimeSpan.Milliseconds;
                }
                else if (offset < 0)
                {
                    offset = 0;
                }

                this.Context.MainContentStoryBoardReverse.Seek(TimeSpan.FromMilliseconds(offset));
                this.Context.DrawerStoryBoardReverse.Seek(TimeSpan.FromMilliseconds(offset));

                if (offset == this.AnimationDuration.TimeSpan.Milliseconds && e.IsInertial)
                {
                    this.IsOpen = false;
                    this.shouldAnimate = false;
                }
                else if (offset == 0 && e.IsInertial)
                {
                    this.IsOpen = true;
                    this.shouldAnimate = false;
                }
            }
        }

        private void Drawer_ManipulationStarted(object sender, Windows.UI.Xaml.Input.ManipulationStartedRoutedEventArgs e)
        {
            this.DrawerState = Primitives.DrawerState.Moving;
            this.shouldAnimate = true;
            this.Context.MainContentStoryBoardReverse.Begin();
            this.Context.MainContentStoryBoardReverse.Pause();
            this.Context.DrawerStoryBoardReverse.Begin();
            this.Context.DrawerStoryBoardReverse.Pause();
        }

        private void MainContent_ManipulationCompleted(object sender, Windows.UI.Xaml.Input.ManipulationCompletedRoutedEventArgs e)
        {
            if (this.shouldAnimate)
            {
                double cumulativeTranslation = 0;
                double animationDistance = 0;
                switch (this.DrawerLocation)
                {
                    case DrawerLocation.Left:
                        cumulativeTranslation = e.Cumulative.Translation.X;
                        animationDistance = this.drawer.Width;
                        break;
                    case DrawerLocation.Right:
                        cumulativeTranslation = -e.Cumulative.Translation.X;
                        animationDistance = this.drawer.Width;
                        break;
                    case DrawerLocation.Top:
                        cumulativeTranslation = e.Cumulative.Translation.Y;
                        animationDistance = this.drawer.Height;
                        break;
                    case DrawerLocation.Bottom:
                        cumulativeTranslation = -e.Cumulative.Translation.Y;
                        animationDistance = this.drawer.Height;
                        break;
                }

                var offset = (cumulativeTranslation / animationDistance) * this.AnimationDuration.TimeSpan.Milliseconds;

                if (cumulativeTranslation > animationDistance / 3)
                {
                    this.Context.MainContentStoryBoard.Resume();
                    this.Context.DrawerStoryBoard.Resume();

                    this.IsOpen = true;
                }
                else
                {
                    this.Context.MainContentStoryBoardReverse.Begin();
                    this.Context.MainContentStoryBoardReverse.Pause();
                    this.Context.DrawerStoryBoardReverse.Begin();
                    this.Context.DrawerStoryBoardReverse.Pause();

                    this.Context.MainContentStoryBoardReverse.Seek(TimeSpan.FromMilliseconds(this.AnimationDuration.TimeSpan.Milliseconds - offset));
                    this.Context.DrawerStoryBoardReverse.Seek(TimeSpan.FromMilliseconds(this.AnimationDuration.TimeSpan.Milliseconds - offset));

                    this.Context.MainContentStoryBoardReverse.Resume();
                    this.Context.DrawerStoryBoardReverse.Resume();

                    this.IsOpen = false;
                }

                this.shouldAnimate = false;
            }
        }

        private void MainContent_ManipulationStarted(object sender, Windows.UI.Xaml.Input.ManipulationStartedRoutedEventArgs e)
        {
            var owner = sender as FrameworkElement;
            bool isInArea = false;
            switch (this.DrawerLocation)
            {
                case DrawerLocation.Left:
                    isInArea = e.Position.X < this.TouchTargetThreshold;
                    break;
                case DrawerLocation.Right:
                    isInArea = e.Position.X > owner.ActualWidth - this.TouchTargetThreshold;
                    break;
                case DrawerLocation.Top:
                    isInArea = e.Position.Y < this.TouchTargetThreshold;
                    break;
                case DrawerLocation.Bottom:
                    isInArea = e.Position.Y > owner.ActualHeight - this.TouchTargetThreshold;
                    break;
            }

            if (isInArea)
            {
                this.DrawerState = Primitives.DrawerState.Moving;
                this.Context.MainContentStoryBoard.Begin();
                this.Context.MainContentStoryBoard.Pause();
                this.Context.DrawerStoryBoard.Begin();
                this.Context.DrawerStoryBoard.Pause();

                this.shouldAnimate = true;
            }
        }

        private void MainContent_ManipulationDelta(object sender, Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        {
            if (this.shouldAnimate)
            {
                double cumulativeTranslation = 0;
                double animationDistance = 0;
                switch (this.DrawerLocation)
                {
                    case DrawerLocation.Left:
                        cumulativeTranslation = e.Cumulative.Translation.X;
                        animationDistance = this.drawer.Width;
                        break;
                    case DrawerLocation.Right:
                        cumulativeTranslation = -e.Cumulative.Translation.X;
                        animationDistance = this.drawer.Width;
                        break;
                    case DrawerLocation.Top:
                        cumulativeTranslation = e.Cumulative.Translation.Y;
                        animationDistance = this.drawer.Height;
                        break;
                    case DrawerLocation.Bottom:
                        cumulativeTranslation = -e.Cumulative.Translation.Y;
                        animationDistance = this.drawer.Height;
                        break;
                }

                var offset = (cumulativeTranslation / animationDistance) * this.AnimationDuration.TimeSpan.Milliseconds;

                if (offset > this.AnimationDuration.TimeSpan.Milliseconds)
                {
                    offset = this.AnimationDuration.TimeSpan.Milliseconds;
                }
                else if (offset < 0)
                {
                    offset = 0;
                    if (DrawerState != Primitives.DrawerState.Closed)
                    {
                        this.DrawerState = Primitives.DrawerState.Closed;
                    }
                }

                if (offset < this.AnimationDuration.TimeSpan.Milliseconds && offset > 0)
                {
                    if (this.DrawerState != Primitives.DrawerState.Moving)
                    {
                        this.Context.MainContentStoryBoard.Begin();
                        this.Context.DrawerStoryBoard.Begin();
                        this.Context.MainContentStoryBoard.Pause();
                        this.Context.DrawerStoryBoard.Pause();
                        this.DrawerState = Primitives.DrawerState.Moving;
                    }
                }

                this.Context.MainContentStoryBoard.Seek(TimeSpan.FromMilliseconds(offset));
                this.Context.DrawerStoryBoard.Seek(TimeSpan.FromMilliseconds(offset));

                if (offset == this.AnimationDuration.TimeSpan.Milliseconds && e.IsInertial)
                {
                    this.IsOpen = true;
                    this.shouldAnimate = false;
                }
                else if (offset == 0 && e.IsInertial)
                {
                    this.IsOpen = false;
                    this.shouldAnimate = false;
                }
            }
        }
    }
}
