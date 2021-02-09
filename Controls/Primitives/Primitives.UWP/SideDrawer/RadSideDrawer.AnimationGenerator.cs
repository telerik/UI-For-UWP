using System;
using System.Collections.Generic;
using System.Text;
using Telerik.UI.Xaml.Controls.Primitives.SideDrawer.Commands;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    public partial class RadSideDrawer
    {
        private DoubleAnimation sideBarXAnimation;
        private DoubleAnimation sideBarYAnimation;
        private DoubleAnimation mainContentXAnimation;
        private DoubleAnimation mainContentYAnimation;
        private DoubleAnimation mainContentOpacityAnimation;
        private DoubleAnimation sideBarClipAnimation;
        private DoubleAnimation slideoutSeekedAnimation;

        private DoubleAnimation sideBarXAnimationReverse;
        private DoubleAnimation sideBarYAnimationReverse;
        private DoubleAnimation mainContentXAnimationReverse;
        private DoubleAnimation mainContentYAnimationReverse;
        private DoubleAnimation mainContentOpacityAnimationReverse;
        private DoubleAnimation sideBarClipAnimationReverse;
        private DoubleAnimation slideoutSeekedAnimationReverse;

        private Storyboard mainContentStoryboard;
        private Storyboard sideBarStoryboard;
        private Storyboard mainContentStoryboardReverse;
        private Storyboard sideBarStoryboardReverse;

        internal AnimationContext GetAnimations(bool shouldPrepareDrawer = true)
        {
            this.CreateDoubleAnimations();

            if (shouldPrepareDrawer)
            {
                this.PrepareSideBar();
            }

            switch (this.DrawerTransition)
            {
                case DrawerTransition.Push:
                    return this.GetPushAnimations();

                case DrawerTransition.Reveal:
                    return this.GetRevealAnimations();

                case DrawerTransition.ReverseSlideOut:
                    return this.GetReverseSlideOutAnimations();

                case DrawerTransition.ScaleDownPusher:
                    return this.GetScaleDownPusherAnimations();

                case DrawerTransition.ScaleUp:
                    return this.GetScaleUpAnimations();

                case DrawerTransition.SlideInOnTop:
                    return this.GetSlideInOnTopAnimations();

                case DrawerTransition.SlideAlong:
                    return this.GetSlideAlongAnimations();

                default:
                    return new AnimationContext() { DrawerLocation = this.DrawerLocation, DrawerTransition = this.DrawerTransition };
            }
        }

        private AnimationContext GetPushAnimations()
        {
            switch (this.DrawerLocation)
            {
                case DrawerLocation.Left:

                    this.sideBarXAnimation.From = -this.drawer.Width;
                    this.sideBarXAnimation.To = 0;
                    this.sideBarStoryboard.Children.Add(this.sideBarXAnimation);

                    this.sideBarXAnimationReverse.From = 0;
                    this.sideBarXAnimationReverse.To = -this.drawer.Width;
                    this.sideBarStoryboardReverse.Children.Add(this.sideBarXAnimationReverse);

                    this.mainContentXAnimation.From = 0;
                    this.mainContentXAnimation.To = this.drawer.Width;
                    this.mainContentStoryboard.Children.Add(this.mainContentXAnimation);

                    this.mainContentXAnimationReverse.From = this.drawer.Width;
                    this.mainContentXAnimationReverse.To = 0;
                    this.mainContentStoryboardReverse.Children.Add(this.mainContentXAnimationReverse);

                    break;
                case DrawerLocation.Right:

                    this.sideBarXAnimation.From = this.mainContent.ActualWidth;
                    this.sideBarXAnimation.To = this.mainContent.ActualWidth - this.drawer.Width;
                    this.sideBarStoryboard.Children.Add(this.sideBarXAnimation);

                    this.sideBarXAnimationReverse.From = this.mainContent.ActualWidth - this.drawer.Width;
                    this.sideBarXAnimationReverse.To = this.mainContent.ActualWidth;
                    this.sideBarStoryboardReverse.Children.Add(this.sideBarXAnimationReverse);

                    this.mainContentXAnimation.From = 0;
                    this.mainContentXAnimation.To = -this.drawer.Width;
                    this.mainContentStoryboard.Children.Add(this.mainContentXAnimation);

                    this.mainContentXAnimationReverse.From = -this.drawer.Width;
                    this.mainContentXAnimationReverse.To = 0;
                    this.mainContentStoryboardReverse.Children.Add(this.mainContentXAnimationReverse);

                    break;
                case DrawerLocation.Top:

                    this.sideBarYAnimation.From = -this.drawer.Height;
                    this.sideBarYAnimation.To = 0;
                    this.sideBarStoryboard.Children.Add(this.sideBarYAnimation);

                    this.sideBarYAnimationReverse.From = 0;
                    this.sideBarYAnimationReverse.To = -this.drawer.Height;
                    this.sideBarStoryboardReverse.Children.Add(this.sideBarYAnimationReverse);

                    this.mainContentYAnimation.From = 0;
                    this.mainContentYAnimation.To = this.drawer.Height;
                    this.mainContentStoryboard.Children.Add(this.mainContentYAnimation);

                    this.mainContentYAnimationReverse.From = this.drawer.Height;
                    this.mainContentYAnimationReverse.To = 0;
                    this.mainContentStoryboardReverse.Children.Add(this.mainContentYAnimationReverse);

                    break;
                case DrawerLocation.Bottom:

                    this.sideBarYAnimation.From = this.mainContent.ActualHeight;
                    this.sideBarYAnimation.To = this.mainContent.ActualHeight - this.drawer.Height;
                    this.sideBarStoryboard.Children.Add(this.sideBarYAnimation);

                    this.sideBarYAnimationReverse.From = this.mainContent.ActualHeight - this.drawer.Height;
                    this.sideBarYAnimationReverse.To = this.mainContent.ActualHeight;
                    this.sideBarStoryboardReverse.Children.Add(this.sideBarYAnimationReverse);

                    this.mainContentYAnimation.From = 0;
                    this.mainContentYAnimation.To = -this.drawer.Height;
                    this.mainContentStoryboard.Children.Add(this.mainContentYAnimation);

                    this.mainContentYAnimationReverse.From = -this.drawer.Height;
                    this.mainContentYAnimationReverse.To = 0;
                    this.mainContentStoryboardReverse.Children.Add(this.mainContentYAnimationReverse);

                    break;
            }

            this.mainContentOpacityAnimation.From = 1;
            this.mainContentOpacityAnimation.To = this.DrawerTransitionFadeOpacity;
            this.mainContentStoryboard.Children.Add(this.mainContentOpacityAnimation);

            this.mainContentOpacityAnimationReverse.From = this.DrawerTransitionFadeOpacity;
            this.mainContentOpacityAnimationReverse.To = 1;
            this.mainContentStoryboardReverse.Children.Add(this.mainContentOpacityAnimationReverse);

            return new AnimationContext()
            {
                MainContentStoryBoard = this.mainContentStoryboard,
                MainContentStoryBoardReverse = this.mainContentStoryboardReverse,
                DrawerStoryBoardReverse = this.sideBarStoryboardReverse,
                DrawerStoryBoard = this.sideBarStoryboard,
                DrawerLocation = this.DrawerLocation,
                DrawerTransition = this.DrawerTransition
            };
        }

        private AnimationContext GetRevealAnimations()
        {
            switch (this.DrawerLocation)
            {
                case DrawerLocation.Left:

                    this.mainContentXAnimation.From = 0;
                    this.mainContentXAnimation.To = this.drawer.Width;
                    this.mainContentStoryboard.Children.Add(this.mainContentXAnimation);

                    this.mainContentXAnimationReverse.From = this.drawer.Width;
                    this.mainContentXAnimationReverse.To = 0;
                    this.mainContentStoryboardReverse.Children.Add(this.mainContentXAnimationReverse);

                    this.sideBarClipAnimation.From = 0;
                    this.sideBarClipAnimation.To = this.drawer.Width;

                    this.sideBarClipAnimationReverse.From = this.drawer.Width;
                    this.sideBarClipAnimationReverse.To = 0;

                    break;
                case DrawerLocation.Right:

                    this.mainContentXAnimation.From = 0;
                    this.mainContentXAnimation.To = -this.drawer.Width;
                    this.mainContentStoryboard.Children.Add(this.mainContentXAnimation);

                    this.mainContentXAnimationReverse.From = -this.drawer.Width;
                    this.mainContentXAnimationReverse.To = 0;
                    this.mainContentStoryboardReverse.Children.Add(this.mainContentXAnimationReverse);

                    this.sideBarClipAnimation.From = this.drawer.Width;
                    this.sideBarClipAnimation.To = 0;

                    this.sideBarClipAnimationReverse.From = 0;
                    this.sideBarClipAnimationReverse.To = this.drawer.Width;

                    break;
                case DrawerLocation.Top:

                    this.mainContentYAnimation.From = 0;
                    this.mainContentYAnimation.To = this.drawer.Height;
                    this.mainContentStoryboard.Children.Add(this.mainContentYAnimation);

                    this.mainContentYAnimationReverse.From = this.drawer.Height;
                    this.mainContentYAnimationReverse.To = 0;
                    this.mainContentStoryboardReverse.Children.Add(this.mainContentYAnimationReverse);

                    this.sideBarClipAnimation.From = 0;
                    this.sideBarClipAnimation.To = this.drawer.Height;

                    this.sideBarClipAnimationReverse.From = this.drawer.Height;
                    this.sideBarClipAnimationReverse.To = 0;

                    break;
                case DrawerLocation.Bottom:

                    this.mainContentYAnimation.From = 0;
                    this.mainContentYAnimation.To = -this.drawer.Height;
                    this.mainContentStoryboard.Children.Add(this.mainContentYAnimation);

                    this.mainContentYAnimationReverse.From = -this.drawer.Height;
                    this.mainContentYAnimationReverse.To = 0;
                    this.mainContentStoryboardReverse.Children.Add(this.mainContentYAnimationReverse);

                    this.sideBarClipAnimation.From = this.drawer.Height;
                    this.sideBarClipAnimation.To = 0;

                    this.sideBarClipAnimationReverse.From = 0;
                    this.sideBarClipAnimationReverse.To = this.drawer.Height;

                    break;
            }

            this.sideBarStoryboard.Children.Add(this.sideBarClipAnimation);
            this.sideBarStoryboardReverse.Children.Add(this.sideBarClipAnimationReverse);

            this.mainContentOpacityAnimation.From = 1;
            this.mainContentOpacityAnimation.To = this.DrawerTransitionFadeOpacity;
            this.mainContentStoryboard.Children.Add(this.mainContentOpacityAnimation);

            this.mainContentOpacityAnimationReverse.From = this.DrawerTransitionFadeOpacity;
            this.mainContentOpacityAnimationReverse.To = 1;
            this.mainContentStoryboardReverse.Children.Add(this.mainContentOpacityAnimationReverse);

            return new AnimationContext()
            {
                MainContentStoryBoard = this.mainContentStoryboard,
                MainContentStoryBoardReverse = this.mainContentStoryboardReverse,
                DrawerStoryBoardReverse = this.sideBarStoryboardReverse,
                DrawerStoryBoard = this.sideBarStoryboard,
                DrawerLocation = this.DrawerLocation,
                DrawerTransition = this.DrawerTransition
            };
        }

        private AnimationContext GetReverseSlideOutAnimations()
        {
            switch (this.DrawerLocation)
            {
                case DrawerLocation.Left:

                    this.mainContentXAnimation.From = 0;
                    this.mainContentXAnimation.To = this.drawer.Width;
                    this.mainContentStoryboard.Children.Add(this.mainContentXAnimation);

                    this.mainContentXAnimationReverse.From = this.drawer.Width;
                    this.mainContentXAnimationReverse.To = 0;
                    this.mainContentStoryboardReverse.Children.Add(this.mainContentXAnimationReverse);

                    this.sideBarXAnimation.From = this.drawer.Width;
                    this.sideBarXAnimation.To = 0;
                    this.sideBarStoryboard.Children.Add(this.sideBarXAnimation);

                    this.sideBarXAnimationReverse.From = 0;
                    this.sideBarXAnimationReverse.To = this.drawer.Width;
                    this.sideBarStoryboardReverse.Children.Add(this.sideBarXAnimationReverse);

                    this.sideBarClipAnimation.From = 0;
                    this.sideBarClipAnimation.To = this.drawer.Width;

                    this.sideBarClipAnimationReverse.From = this.drawer.Width;
                    this.sideBarClipAnimationReverse.To = 0;

                    break;
                case DrawerLocation.Right:

                    this.mainContentXAnimation.From = 0;
                    this.mainContentXAnimation.To = -this.drawer.Width;
                    this.mainContentStoryboard.Children.Add(this.mainContentXAnimation);

                    this.mainContentXAnimationReverse.From = -this.drawer.Width;
                    this.mainContentXAnimationReverse.To = 0;
                    this.mainContentStoryboardReverse.Children.Add(this.mainContentXAnimationReverse);

                    this.sideBarXAnimation.From = this.mainContent.ActualWidth - 2 * this.drawer.Width;
                    this.sideBarXAnimation.To = this.mainContent.ActualWidth - this.drawer.Width;
                    this.sideBarStoryboard.Children.Add(this.sideBarXAnimation);

                    this.sideBarXAnimationReverse.From = this.mainContent.ActualWidth - this.drawer.Width;
                    this.sideBarXAnimationReverse.To = this.mainContent.ActualWidth - 2 * this.drawer.Width;
                    this.sideBarStoryboardReverse.Children.Add(this.sideBarXAnimationReverse);

                    this.sideBarClipAnimation.From = this.drawer.Width;
                    this.sideBarClipAnimation.To = 0;

                    this.sideBarClipAnimationReverse.From = 0;
                    this.sideBarClipAnimationReverse.To = this.drawer.Width;

                    break;
                case DrawerLocation.Top:

                    this.mainContentYAnimation.From = 0;
                    this.mainContentYAnimation.To = this.drawer.Height;
                    this.mainContentStoryboard.Children.Add(this.mainContentYAnimation);

                    this.mainContentYAnimationReverse.From = this.drawer.Height;
                    this.mainContentYAnimationReverse.To = 0;
                    this.mainContentStoryboardReverse.Children.Add(this.mainContentYAnimationReverse);

                    this.sideBarYAnimation.From = this.drawer.Height;
                    this.sideBarYAnimation.To = 0;
                    this.sideBarStoryboard.Children.Add(this.sideBarYAnimation);

                    this.sideBarYAnimationReverse.From = 0;
                    this.sideBarYAnimationReverse.To = this.drawer.Height;
                    this.sideBarStoryboardReverse.Children.Add(this.sideBarYAnimationReverse);

                    this.sideBarClipAnimation.From = 0;
                    this.sideBarClipAnimation.To = this.drawer.Height;

                    this.sideBarClipAnimationReverse.From = this.drawer.Height;
                    this.sideBarClipAnimationReverse.To = 0;

                    break;
                case DrawerLocation.Bottom:

                    this.mainContentYAnimation.From = 0;
                    this.mainContentYAnimation.To = -this.drawer.Height;
                    this.mainContentStoryboard.Children.Add(this.mainContentYAnimation);

                    this.mainContentYAnimationReverse.From = -this.drawer.Height;
                    this.mainContentYAnimationReverse.To = 0;
                    this.mainContentStoryboardReverse.Children.Add(this.mainContentYAnimationReverse);

                    this.sideBarYAnimation.From = this.mainContent.ActualHeight - 2 * this.drawer.Height;
                    this.sideBarYAnimation.To = this.mainContent.ActualHeight - this.drawer.Height;
                    this.sideBarStoryboard.Children.Add(this.sideBarYAnimation);

                    this.sideBarYAnimationReverse.From = this.mainContent.ActualHeight - this.drawer.Height;
                    this.sideBarYAnimationReverse.To = this.mainContent.ActualHeight - 2 * this.drawer.Height;
                    this.sideBarStoryboardReverse.Children.Add(this.sideBarYAnimationReverse);

                    this.sideBarClipAnimation.From = this.drawer.Height;
                    this.sideBarClipAnimation.To = 0;

                    this.sideBarClipAnimationReverse.From = 0;
                    this.sideBarClipAnimationReverse.To = this.drawer.Height;

                    break;
            }

            this.sideBarClipAnimation.BeginTime = TimeSpan.FromMilliseconds(this.AnimationDuration.TimeSpan.Milliseconds / 2);
            this.sideBarClipAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(this.AnimationDuration.TimeSpan.Milliseconds / 2));
            this.sideBarStoryboard.Children.Add(this.sideBarClipAnimation);

            this.sideBarClipAnimationReverse.BeginTime = TimeSpan.FromMilliseconds(0);
            this.sideBarClipAnimationReverse.Duration = new Duration(TimeSpan.FromMilliseconds(this.AnimationDuration.TimeSpan.Milliseconds / 2));
            this.sideBarStoryboardReverse.Children.Add(this.sideBarClipAnimationReverse);

            this.slideoutSeekedAnimation.From = 0;
            this.slideoutSeekedAnimation.To = 1;
            this.slideoutSeekedAnimation.BeginTime = TimeSpan.FromMilliseconds(0);
            this.sideBarStoryboard.Children.Add(this.slideoutSeekedAnimation);

            this.slideoutSeekedAnimationReverse.From = 0;
            this.slideoutSeekedAnimationReverse.To = 1;
            this.slideoutSeekedAnimationReverse.BeginTime = TimeSpan.FromMilliseconds(this.AnimationDuration.TimeSpan.Milliseconds / 2);
            this.sideBarStoryboardReverse.Children.Add(this.slideoutSeekedAnimationReverse);

            this.mainContentOpacityAnimation.From = 1;
            this.mainContentOpacityAnimation.To = this.DrawerTransitionFadeOpacity;
            this.mainContentStoryboard.Children.Add(this.mainContentOpacityAnimation);

            this.mainContentOpacityAnimationReverse.From = this.DrawerTransitionFadeOpacity;
            this.mainContentOpacityAnimationReverse.To = 1;
            this.mainContentStoryboardReverse.Children.Add(this.mainContentOpacityAnimationReverse);

            return new AnimationContext()
            {
                MainContentStoryBoard = this.mainContentStoryboard,
                MainContentStoryBoardReverse = this.mainContentStoryboardReverse,
                DrawerStoryBoardReverse = this.sideBarStoryboardReverse,
                DrawerStoryBoard = this.sideBarStoryboard,
                DrawerLocation = this.DrawerLocation,
                DrawerTransition = this.DrawerTransition
            };
        }

        private AnimationContext GetScaleDownPusherAnimations()
        {
            switch (this.DrawerLocation)
            {
                case DrawerLocation.Left:

                    this.sideBarXAnimation.From = -this.drawer.Width;
                    this.sideBarXAnimation.To = 0;
                    this.sideBarStoryboard.Children.Add(this.sideBarXAnimation);

                    this.sideBarXAnimationReverse.From = 0;
                    this.sideBarXAnimationReverse.To = -this.drawer.Width;
                    this.sideBarStoryboardReverse.Children.Add(this.sideBarXAnimationReverse);

                    break;
                case DrawerLocation.Right:

                    this.sideBarXAnimation.From = this.mainContent.ActualWidth;
                    this.sideBarXAnimation.To = this.mainContent.ActualWidth - this.drawer.Width;
                    this.sideBarStoryboard.Children.Add(this.sideBarXAnimation);

                    this.sideBarXAnimationReverse.From = this.mainContent.ActualWidth - this.drawer.Width;
                    this.sideBarXAnimationReverse.To = this.mainContent.ActualWidth;
                    this.sideBarStoryboardReverse.Children.Add(this.sideBarXAnimationReverse);

                    break;
                case DrawerLocation.Top:

                    this.sideBarYAnimation.From = -this.drawer.Height;
                    this.sideBarYAnimation.To = 0;
                    this.sideBarStoryboard.Children.Add(this.sideBarYAnimation);

                    this.sideBarYAnimationReverse.From = 0;
                    this.sideBarYAnimationReverse.To = -this.drawer.Height;
                    this.sideBarStoryboardReverse.Children.Add(this.sideBarYAnimationReverse);

                    break;
                case DrawerLocation.Bottom:

                    this.sideBarYAnimation.From = this.mainContent.ActualHeight;
                    this.sideBarYAnimation.To = this.mainContent.ActualHeight - this.drawer.Height;
                    this.sideBarStoryboard.Children.Add(this.sideBarYAnimation);

                    this.sideBarYAnimationReverse.From = this.mainContent.ActualHeight - this.drawer.Height;
                    this.sideBarYAnimationReverse.To = this.mainContent.ActualHeight;
                    this.sideBarStoryboardReverse.Children.Add(this.sideBarYAnimationReverse);

                    break;
            }

            this.mainContent.RenderTransform = new ScaleTransform() { CenterX = this.mainContent.ActualWidth / 2, CenterY = this.mainContent.ActualHeight / 2 };

            var xAnim = new DoubleAnimation();
            var yAnim = new DoubleAnimation();

            xAnim.Duration = this.AnimationDuration;
            yAnim.Duration = this.AnimationDuration;

            xAnim.To = 0.75;
            yAnim.To = 0.75;
            xAnim.From = 1;
            yAnim.From = 1;

            var xAnimReverse = new DoubleAnimation();
            var yAnimReverse = new DoubleAnimation();

            xAnimReverse.Duration = this.AnimationDuration;
            yAnimReverse.Duration = this.AnimationDuration;

            xAnimReverse.To = 1;
            yAnimReverse.To = 1;
            xAnimReverse.From = 0.75;
            yAnimReverse.From = 0.75;

            this.mainContentStoryboard.Children.Add(xAnim);
            this.mainContentStoryboard.Children.Add(yAnim);

            this.mainContentStoryboardReverse.Children.Add(xAnimReverse);
            this.mainContentStoryboardReverse.Children.Add(yAnimReverse);

            Storyboard.SetTarget(xAnim, this.mainContent);
            Storyboard.SetTarget(yAnim, this.mainContent);

            Storyboard.SetTarget(xAnimReverse, this.mainContent);
            Storyboard.SetTarget(yAnimReverse, this.mainContent);

            Storyboard.SetTargetProperty(xAnim, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");
            Storyboard.SetTargetProperty(yAnim, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");

            Storyboard.SetTargetProperty(xAnimReverse, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");
            Storyboard.SetTargetProperty(yAnimReverse, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");

            this.mainContentOpacityAnimation.From = 1;
            this.mainContentOpacityAnimation.To = this.DrawerTransitionFadeOpacity;
            this.mainContentStoryboard.Children.Add(this.mainContentOpacityAnimation);

            this.mainContentOpacityAnimationReverse.From = this.DrawerTransitionFadeOpacity;
            this.mainContentOpacityAnimationReverse.To = 1;
            this.mainContentStoryboardReverse.Children.Add(this.mainContentOpacityAnimationReverse);

            return new AnimationContext()
            {
                MainContentStoryBoard = this.mainContentStoryboard,
                MainContentStoryBoardReverse = this.mainContentStoryboardReverse,
                DrawerStoryBoardReverse = this.sideBarStoryboardReverse,
                DrawerStoryBoard = this.sideBarStoryboard,
                DrawerLocation = this.DrawerLocation,
                DrawerTransition = this.DrawerTransition
            };
        }

        private AnimationContext GetScaleUpAnimations()
        {
            this.drawer.Clip = new Windows.UI.Xaml.Media.RectangleGeometry() { Rect = new Windows.Foundation.Rect(0, 0, 0, 0) };

            this.drawer.RenderTransform = new ScaleTransform() { CenterX = this.drawer.Width / 2, CenterY = this.drawer.Height / 2 };

            var xAnim1 = new DoubleAnimation();
            var yAnim1 = new DoubleAnimation();

            var xAnim1Reverse = new DoubleAnimation();
            var yAnim1Reverse = new DoubleAnimation();

            xAnim1.Duration = this.AnimationDuration;
            yAnim1.Duration = this.AnimationDuration;
            xAnim1.To = 1;
            yAnim1.To = 1;
            xAnim1.From = 0.80;
            yAnim1.From = 0.80;

            xAnim1Reverse.Duration = this.AnimationDuration;
            yAnim1Reverse.Duration = this.AnimationDuration;
            xAnim1Reverse.To = 0.80;
            yAnim1Reverse.To = 0.80;
            xAnim1Reverse.From = 1;
            yAnim1Reverse.From = 1;

            Storyboard.SetTarget(xAnim1, this.drawer);
            Storyboard.SetTarget(yAnim1, this.drawer);

            Storyboard.SetTargetProperty(xAnim1, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");
            Storyboard.SetTargetProperty(yAnim1, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");

            Storyboard.SetTarget(xAnim1Reverse, this.drawer);
            Storyboard.SetTarget(yAnim1Reverse, this.drawer);

            Storyboard.SetTargetProperty(xAnim1Reverse, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");
            Storyboard.SetTargetProperty(yAnim1Reverse, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");

            switch (this.DrawerLocation)
            {
                case DrawerLocation.Left:

                    this.mainContentXAnimation.From = 0;
                    this.mainContentXAnimation.To = this.drawer.Width;
                    this.mainContentStoryboard.Children.Add(this.mainContentXAnimation);

                    this.mainContentXAnimationReverse.From = this.drawer.Width;
                    this.mainContentXAnimationReverse.To = 0;
                    this.mainContentStoryboardReverse.Children.Add(this.mainContentXAnimationReverse);

                    this.sideBarClipAnimation.From = 0;
                    this.sideBarClipAnimation.To = this.drawer.Width;

                    this.sideBarClipAnimationReverse.From = this.drawer.Width;
                    this.sideBarClipAnimationReverse.To = 0;

                    this.sideBarStoryboard.Children.Add(yAnim1);
                    this.sideBarStoryboardReverse.Children.Add(yAnim1Reverse);

                    break;
                case DrawerLocation.Right:

                    this.mainContentXAnimation.From = 0;
                    this.mainContentXAnimation.To = -this.drawer.Width;
                    this.mainContentStoryboard.Children.Add(this.mainContentXAnimation);

                    this.mainContentXAnimationReverse.From = -this.drawer.Width;
                    this.mainContentXAnimationReverse.To = 0;
                    this.mainContentStoryboardReverse.Children.Add(this.mainContentXAnimationReverse);

                    this.sideBarClipAnimation.From = this.drawer.Width;
                    this.sideBarClipAnimation.To = 0;

                    this.sideBarClipAnimationReverse.From = 0;
                    this.sideBarClipAnimationReverse.To = this.drawer.Width;

                    this.sideBarStoryboard.Children.Add(yAnim1);
                    this.sideBarStoryboardReverse.Children.Add(yAnim1Reverse);

                    break;
                case DrawerLocation.Top:

                    this.mainContentYAnimation.From = 0;
                    this.mainContentYAnimation.To = this.drawer.Height;
                    this.mainContentStoryboard.Children.Add(this.mainContentYAnimation);

                    this.mainContentYAnimationReverse.From = this.drawer.Height;
                    this.mainContentYAnimationReverse.To = 0;
                    this.mainContentStoryboardReverse.Children.Add(this.mainContentYAnimationReverse);

                    this.sideBarClipAnimation.From = 0;
                    this.sideBarClipAnimation.To = this.drawer.Height;

                    this.sideBarClipAnimationReverse.From = this.drawer.Height;
                    this.sideBarClipAnimationReverse.To = 0;

                    this.sideBarStoryboard.Children.Add(xAnim1);
                    this.sideBarStoryboardReverse.Children.Add(xAnim1Reverse);

                    break;
                case DrawerLocation.Bottom:

                    this.mainContentYAnimation.From = 0;
                    this.mainContentYAnimation.To = -this.drawer.Height;
                    this.mainContentStoryboard.Children.Add(this.mainContentYAnimation);

                    this.mainContentYAnimationReverse.From = -this.drawer.Height;
                    this.mainContentYAnimationReverse.To = 0;
                    this.mainContentStoryboardReverse.Children.Add(this.mainContentYAnimationReverse);

                    this.sideBarClipAnimation.From = this.drawer.Height;
                    this.sideBarClipAnimation.To = 0;

                    this.sideBarClipAnimationReverse.From = 0;
                    this.sideBarClipAnimationReverse.To = this.drawer.Height;

                    this.sideBarStoryboard.Children.Add(xAnim1);
                    this.sideBarStoryboardReverse.Children.Add(xAnim1Reverse);

                    break;
            }

            this.mainContentOpacityAnimation.From = 1;
            this.mainContentOpacityAnimation.To = this.DrawerTransitionFadeOpacity;
            this.mainContentStoryboard.Children.Add(this.mainContentOpacityAnimation);

            this.mainContentOpacityAnimationReverse.From = this.DrawerTransitionFadeOpacity;
            this.mainContentOpacityAnimationReverse.To = 1;
            this.mainContentStoryboardReverse.Children.Add(this.mainContentOpacityAnimationReverse);

            this.sideBarStoryboard.Children.Add(this.sideBarClipAnimation);
            this.sideBarStoryboardReverse.Children.Add(this.sideBarClipAnimationReverse);

            return new AnimationContext()
            {
                MainContentStoryBoard = this.mainContentStoryboard,
                MainContentStoryBoardReverse = this.mainContentStoryboardReverse,
                DrawerStoryBoardReverse = this.sideBarStoryboardReverse,
                DrawerStoryBoard = this.sideBarStoryboard,
                DrawerLocation = this.DrawerLocation,
                DrawerTransition = this.DrawerTransition
            };
        }

        private AnimationContext GetSlideInOnTopAnimations()
        {
            switch (this.DrawerLocation)
            {
                case DrawerLocation.Left:

                    this.sideBarXAnimation.From = -this.drawer.Width;
                    this.sideBarXAnimation.To = 0;
                    this.sideBarStoryboard.Children.Add(this.sideBarXAnimation);

                    this.sideBarXAnimationReverse.From = 0;
                    this.sideBarXAnimationReverse.To = -this.drawer.Width;
                    this.sideBarStoryboardReverse.Children.Add(this.sideBarXAnimationReverse);

                    break;
                case DrawerLocation.Right:

                    this.sideBarXAnimation.From = this.mainContent.ActualWidth;
                    this.sideBarXAnimation.To = this.mainContent.ActualWidth - this.drawer.Width;
                    this.sideBarStoryboard.Children.Add(this.sideBarXAnimation);

                    this.sideBarXAnimationReverse.From = this.mainContent.ActualWidth - this.drawer.Width;
                    this.sideBarXAnimationReverse.To = this.mainContent.ActualWidth;
                    this.sideBarStoryboardReverse.Children.Add(this.sideBarXAnimationReverse);

                    break;
                case DrawerLocation.Top:

                    this.sideBarYAnimation.From = -this.drawer.Height;
                    this.sideBarYAnimation.To = 0;
                    this.sideBarStoryboard.Children.Add(this.sideBarYAnimation);

                    this.sideBarYAnimationReverse.From = 0;
                    this.sideBarYAnimationReverse.To = -this.drawer.Height;
                    this.sideBarStoryboardReverse.Children.Add(this.sideBarYAnimationReverse);

                    break;
                case DrawerLocation.Bottom:

                    this.sideBarYAnimation.From = this.mainContent.ActualHeight;
                    this.sideBarYAnimation.To = this.mainContent.ActualHeight - this.drawer.Height;
                    this.sideBarStoryboard.Children.Add(this.sideBarYAnimation);

                    this.sideBarYAnimationReverse.From = this.mainContent.ActualHeight - this.drawer.Height;
                    this.sideBarYAnimationReverse.To = this.mainContent.ActualHeight;
                    this.sideBarStoryboardReverse.Children.Add(this.sideBarYAnimationReverse);

                    break;
            }

            this.mainContentOpacityAnimation.From = 1;
            this.mainContentOpacityAnimation.To = this.DrawerTransitionFadeOpacity;
            this.mainContentStoryboard.Children.Add(this.mainContentOpacityAnimation);

            this.mainContentOpacityAnimationReverse.From = this.DrawerTransitionFadeOpacity;
            this.mainContentOpacityAnimationReverse.To = 1;
            this.mainContentStoryboardReverse.Children.Add(this.mainContentOpacityAnimationReverse);

            return new AnimationContext()
            {
                MainContentStoryBoard = this.mainContentStoryboard,
                MainContentStoryBoardReverse = this.mainContentStoryboardReverse,
                DrawerStoryBoardReverse = this.sideBarStoryboardReverse,
                DrawerStoryBoard = this.sideBarStoryboard,
                DrawerLocation = this.DrawerLocation,
                DrawerTransition = this.DrawerTransition
            };
        }

        private AnimationContext GetSlideAlongAnimations()
        {
            switch (this.DrawerLocation)
            {
                case DrawerLocation.Left:
                    this.mainContentXAnimation.From = 0;
                    this.mainContentXAnimation.To = this.drawer.Width;
                    this.mainContentStoryboard.Children.Add(this.mainContentXAnimation);

                    this.mainContentXAnimationReverse.From = this.drawer.Width;
                    this.mainContentXAnimationReverse.To = 0;
                    this.mainContentStoryboardReverse.Children.Add(this.mainContentXAnimationReverse);

                    this.sideBarXAnimation.From = -this.drawer.Width / 2;
                    this.sideBarXAnimation.To = 0;
                    this.sideBarStoryboard.Children.Add(this.sideBarXAnimation);

                    this.sideBarXAnimationReverse.From = 0;
                    this.sideBarXAnimationReverse.To = -this.drawer.Width / 2;
                    this.sideBarStoryboardReverse.Children.Add(this.sideBarXAnimationReverse);

                    this.sideBarClipAnimation.From = 0;
                    this.sideBarClipAnimation.To = this.drawer.Width;
                    this.sideBarStoryboard.Children.Add(this.sideBarClipAnimation);

                    this.sideBarClipAnimationReverse.From = this.drawer.Width;
                    this.sideBarClipAnimationReverse.To = 0;
                    this.sideBarStoryboardReverse.Children.Add(this.sideBarClipAnimationReverse);
                    break;
                case DrawerLocation.Right:
                    this.mainContentXAnimation.From = 0;
                    this.mainContentXAnimation.To = -this.drawer.Width;
                    this.mainContentStoryboard.Children.Add(this.mainContentXAnimation);

                    this.mainContentXAnimationReverse.From = -this.drawer.Width;
                    this.mainContentXAnimationReverse.To = 0;
                    this.mainContentStoryboardReverse.Children.Add(this.mainContentXAnimationReverse);

                    this.sideBarXAnimation.From = this.mainContent.ActualWidth - this.drawer.Width / 2;
                    this.sideBarXAnimation.To = this.mainContent.ActualWidth - this.drawer.Width;
                    this.sideBarStoryboard.Children.Add(this.sideBarXAnimation);

                    this.sideBarXAnimationReverse.From = this.mainContent.ActualWidth - this.drawer.Width;
                    this.sideBarXAnimationReverse.To = this.mainContent.ActualWidth - this.drawer.Width / 2;
                    this.sideBarStoryboardReverse.Children.Add(this.sideBarXAnimationReverse);

                    this.sideBarClipAnimation.From = this.drawer.Width / 2;
                    this.sideBarClipAnimation.To = 0;
                    this.sideBarStoryboard.Children.Add(this.sideBarClipAnimation);

                    this.sideBarClipAnimationReverse.From = 0;
                    this.sideBarClipAnimationReverse.To = this.drawer.Width / 2;
                    this.sideBarStoryboardReverse.Children.Add(this.sideBarClipAnimationReverse);

                    break;
                case DrawerLocation.Top:
                    this.mainContentYAnimation.From = 0;
                    this.mainContentYAnimation.To = this.drawer.Height;
                    this.mainContentStoryboard.Children.Add(this.mainContentYAnimation);

                    this.mainContentYAnimationReverse.From = this.drawer.Height;
                    this.mainContentYAnimationReverse.To = 0;
                    this.mainContentStoryboardReverse.Children.Add(this.mainContentYAnimationReverse);

                    this.sideBarYAnimation.From = -this.drawer.Height / 2;
                    this.sideBarYAnimation.To = 0;
                    this.sideBarStoryboard.Children.Add(this.sideBarYAnimation);

                    this.sideBarYAnimationReverse.From = 0;
                    this.sideBarYAnimationReverse.To = -this.drawer.Height / 2;
                    this.sideBarStoryboardReverse.Children.Add(this.sideBarYAnimationReverse);

                    this.sideBarClipAnimation.From = 0;
                    this.sideBarClipAnimation.To = this.drawer.Height;
                    this.sideBarStoryboard.Children.Add(this.sideBarClipAnimation);

                    this.sideBarClipAnimationReverse.From = this.drawer.Height;
                    this.sideBarClipAnimationReverse.To = 0;
                    this.sideBarStoryboardReverse.Children.Add(this.sideBarClipAnimationReverse);
                    break;
                case DrawerLocation.Bottom:
                    this.mainContentYAnimation.From = 0;
                    this.mainContentYAnimation.To = -this.drawer.Height;
                    this.mainContentStoryboard.Children.Add(this.mainContentYAnimation);

                    this.mainContentYAnimationReverse.From = -this.drawer.Height;
                    this.mainContentYAnimationReverse.To = 0;
                    this.mainContentStoryboardReverse.Children.Add(this.mainContentYAnimationReverse);

                    this.sideBarYAnimation.From = this.mainContent.ActualHeight - this.drawer.Height / 2;
                    this.sideBarYAnimation.To = this.mainContent.ActualHeight - this.drawer.Height;
                    this.sideBarStoryboard.Children.Add(this.sideBarYAnimation);

                    this.sideBarYAnimationReverse.From = this.mainContent.ActualHeight - this.drawer.Height;
                    this.sideBarYAnimationReverse.To = this.mainContent.ActualHeight - this.drawer.Height / 2;
                    this.sideBarStoryboardReverse.Children.Add(this.sideBarYAnimationReverse);

                    this.sideBarClipAnimation.From = this.drawer.Height / 2;
                    this.sideBarClipAnimation.To = 0;
                    this.sideBarStoryboard.Children.Add(this.sideBarClipAnimation);

                    this.sideBarClipAnimationReverse.From = 0;
                    this.sideBarClipAnimationReverse.To = this.drawer.Height / 2;
                    this.sideBarStoryboardReverse.Children.Add(this.sideBarClipAnimationReverse);
                    break;
            }

            this.mainContentOpacityAnimation.From = 1;
            this.mainContentOpacityAnimation.To = this.DrawerTransitionFadeOpacity;
            this.mainContentStoryboard.Children.Add(this.mainContentOpacityAnimation);

            this.mainContentOpacityAnimationReverse.From = this.DrawerTransitionFadeOpacity;
            this.mainContentOpacityAnimationReverse.To = 1;
            this.mainContentStoryboardReverse.Children.Add(this.mainContentOpacityAnimationReverse);

            return new AnimationContext()
            {
                MainContentStoryBoard = this.mainContentStoryboard,
                MainContentStoryBoardReverse = this.mainContentStoryboardReverse,
                DrawerStoryBoardReverse = this.sideBarStoryboardReverse,
                DrawerStoryBoard = this.sideBarStoryboard,
                DrawerLocation = this.DrawerLocation,
                DrawerTransition = this.DrawerTransition
            };
        }

        private void PrepareSideBar()
        {
            this.drawer.Clip = null;
            this.drawer.RenderTransform = null;
            var zeroRect = new Rect(0, 0, 0, 0);

            switch (this.DrawerLocation)
            {
                case DrawerLocation.Left:
                    Canvas.SetTop(this.drawer, 0);
                    switch (this.DrawerTransition)
                    {
                        case DrawerTransition.Reveal:
                            Canvas.SetLeft(this.drawer, 0);
                            this.drawer.Clip = new RectangleGeometry() { Rect = zeroRect };
                            break;
                        case DrawerTransition.ScaleUp:
                            Canvas.SetLeft(this.drawer, 0);
                            this.drawer.Clip = new RectangleGeometry() { Rect = zeroRect };
                            break;
                        case DrawerTransition.ReverseSlideOut:
                            this.drawer.Clip = new RectangleGeometry() { Rect = zeroRect };
                            break;
                        case DrawerTransition.SlideAlong:
                            Canvas.SetLeft(this.drawer, this.mainContent.ActualWidth - this.drawer.Width / 2);
                            this.drawer.Clip = new RectangleGeometry() { Rect = zeroRect };
                            break;
                        default:
                            Canvas.SetLeft(this.drawer, -this.drawer.Width);
                            break;
                    }
                    break;
                case DrawerLocation.Right:
                    Canvas.SetTop(this.drawer, 0);

                    switch (this.DrawerTransition)
                    {
                        case DrawerTransition.Reveal:
                            Canvas.SetLeft(this.drawer, this.mainContent.ActualWidth - this.drawer.Width);
                            this.drawer.Clip = new RectangleGeometry() { Rect = zeroRect };
                            break;
                        case DrawerTransition.ScaleUp:
                            Canvas.SetLeft(this.drawer, this.mainContent.ActualWidth - this.drawer.Width);
                            this.drawer.Clip = new RectangleGeometry() { Rect = zeroRect };
                            break;
                        case DrawerTransition.ReverseSlideOut:
                            this.drawer.Clip = new RectangleGeometry() { Rect = zeroRect };
                            break;
                        case DrawerTransition.SlideAlong:
                            Canvas.SetLeft(this.drawer, this.mainContent.ActualWidth - this.drawer.Width / 2);
                            this.drawer.Clip = new RectangleGeometry() { Rect = zeroRect };
                            break;
                        default:
                            Canvas.SetLeft(this.drawer, -this.drawer.Width);
                            break;
                    }
                    break;
                case DrawerLocation.Top:
                    Canvas.SetLeft(this.drawer, 0);

                    switch (this.DrawerTransition)
                    {
                        case DrawerTransition.Reveal:
                            Canvas.SetTop(this.drawer, 0);
                            this.drawer.Clip = new RectangleGeometry() { Rect = zeroRect };
                            break;
                        case DrawerTransition.ScaleUp:
                            Canvas.SetTop(this.drawer, 0);
                            this.drawer.Clip = new RectangleGeometry() { Rect = zeroRect };
                            break;
                        case DrawerTransition.ReverseSlideOut:
                            this.drawer.Clip = new RectangleGeometry() { Rect = zeroRect };
                            break;
                        case DrawerTransition.SlideAlong:
                            this.drawer.Clip = new RectangleGeometry() { Rect = zeroRect };
                            break;
                        default:
                            Canvas.SetTop(this.drawer, -this.drawer.Height);
                            break;
                    }
                    break;
                case DrawerLocation.Bottom:
                    Canvas.SetLeft(this.drawer, 0);

                    switch (this.DrawerTransition)
                    {
                        case DrawerTransition.Reveal:
                            Canvas.SetTop(this.drawer, this.mainContent.ActualHeight - this.drawer.Height);
                            this.drawer.Clip = new RectangleGeometry() { Rect = zeroRect };
                            break;
                        case DrawerTransition.ScaleUp:
                            Canvas.SetTop(this.drawer, this.mainContent.ActualHeight - this.drawer.Height);
                            this.drawer.Clip = new RectangleGeometry() { Rect = zeroRect };
                            break;
                        case DrawerTransition.ReverseSlideOut:
                            this.drawer.Clip = new RectangleGeometry() { Rect = zeroRect };
                            break;
                        case DrawerTransition.SlideAlong:
                            this.drawer.Clip = new RectangleGeometry() { Rect = zeroRect };
                            break;
                        default:
                            Canvas.SetTop(this.drawer, -this.drawer.Height);
                            break;
                    }

                    break;
            }
        }

        private void CreateDoubleAnimations()
        {
            this.mainContentStoryboard = new Storyboard();
            this.sideBarStoryboard = new Storyboard();
            this.mainContentStoryboardReverse = new Storyboard();
            this.sideBarStoryboardReverse = new Storyboard();

            this.sideBarXAnimation = new DoubleAnimation();
            this.sideBarXAnimation.Duration = this.AnimationDuration;
            Storyboard.SetTargetProperty(this.sideBarXAnimation, "(Canvas.Left)");
            Storyboard.SetTarget(this.sideBarXAnimation, this.drawer);

            this.sideBarXAnimationReverse = new DoubleAnimation();
            this.sideBarXAnimationReverse.Duration = this.AnimationDuration;
            Storyboard.SetTargetProperty(this.sideBarXAnimationReverse, "(Canvas.Left)");
            Storyboard.SetTarget(this.sideBarXAnimationReverse, this.drawer);

            this.mainContentXAnimation = new DoubleAnimation();
            this.mainContentXAnimation.Duration = this.AnimationDuration;
            Storyboard.SetTargetProperty(this.mainContentXAnimation, "(Canvas.Left)");
            Storyboard.SetTarget(this.mainContentXAnimation, this.mainContent);

            this.mainContentXAnimationReverse = new DoubleAnimation();
            this.mainContentXAnimationReverse.Duration = this.AnimationDuration;
            Storyboard.SetTargetProperty(this.mainContentXAnimationReverse, "(Canvas.Left)");
            Storyboard.SetTarget(this.mainContentXAnimationReverse, this.mainContent);

            this.mainContentYAnimation = new DoubleAnimation();
            this.mainContentYAnimation.Duration = this.AnimationDuration;
            Storyboard.SetTargetProperty(this.mainContentYAnimation, "(Canvas.Top)");
            Storyboard.SetTarget(this.mainContentYAnimation, this.mainContent);

            this.mainContentYAnimationReverse = new DoubleAnimation();
            this.mainContentYAnimationReverse.Duration = this.AnimationDuration;
            Storyboard.SetTargetProperty(this.mainContentYAnimationReverse, "(Canvas.Top)");
            Storyboard.SetTarget(this.mainContentYAnimationReverse, this.mainContent);

            this.mainContentOpacityAnimation = new DoubleAnimation();
            this.mainContentOpacityAnimation.Duration = this.AnimationDuration;
            Storyboard.SetTargetProperty(this.mainContentOpacityAnimation, "Opacity");
            Storyboard.SetTarget(this.mainContentOpacityAnimation, this.mainContent);

            this.mainContentOpacityAnimationReverse = new DoubleAnimation();
            this.mainContentOpacityAnimationReverse.Duration = this.AnimationDuration;
            Storyboard.SetTargetProperty(this.mainContentOpacityAnimationReverse, "Opacity");
            Storyboard.SetTarget(this.mainContentOpacityAnimationReverse, this.mainContent);

            this.sideBarYAnimation = new DoubleAnimation();
            this.sideBarYAnimation.Duration = this.AnimationDuration;
            Storyboard.SetTargetProperty(this.sideBarYAnimation, "(Canvas.Top)");
            Storyboard.SetTarget(this.sideBarYAnimation, this.drawer);

            this.sideBarYAnimationReverse = new DoubleAnimation();
            this.sideBarYAnimationReverse.Duration = this.AnimationDuration;
            Storyboard.SetTargetProperty(this.sideBarYAnimationReverse, "(Canvas.Top)");
            Storyboard.SetTarget(this.sideBarYAnimationReverse, this.drawer);

            this.sideBarClipAnimation = new DoubleAnimation() { EnableDependentAnimation = true };
            this.sideBarClipAnimation.Duration = this.AnimationDuration;
            Storyboard.SetTargetProperty(this.sideBarClipAnimation, "DrawerClip");
            Storyboard.SetTarget(this.sideBarClipAnimation, this);

            this.sideBarClipAnimationReverse = new DoubleAnimation() { EnableDependentAnimation = true };
            this.sideBarClipAnimationReverse.Duration = this.AnimationDuration;
            Storyboard.SetTargetProperty(this.sideBarClipAnimationReverse, "DrawerClip");
            Storyboard.SetTarget(this.sideBarClipAnimationReverse, this);

            this.slideoutSeekedAnimation = new DoubleAnimation() { EnableDependentAnimation = true };
            this.slideoutSeekedAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(this.AnimationDuration.TimeSpan.Milliseconds / 2));
            Storyboard.SetTargetProperty(this.slideoutSeekedAnimation, "SlideoutSeekedAnimationClip");
            Storyboard.SetTarget(this.slideoutSeekedAnimation, this);

            this.slideoutSeekedAnimationReverse = new DoubleAnimation() { EnableDependentAnimation = true };
            this.slideoutSeekedAnimationReverse.Duration = new Duration(TimeSpan.FromMilliseconds(this.AnimationDuration.TimeSpan.Milliseconds / 2));
            Storyboard.SetTargetProperty(this.slideoutSeekedAnimationReverse, "SlideoutSeekedAnimationClip");
            Storyboard.SetTarget(this.slideoutSeekedAnimationReverse, this);
        }
    }
}
