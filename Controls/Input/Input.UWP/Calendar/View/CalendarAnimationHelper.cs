using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    internal static class CalendarAnimationHelper
    {
        private static TimeSpan animationDuration = TimeSpan.FromMilliseconds(200);

        internal static Storyboard CreateMoveToPreviousViewStoryboard(Panel container)
        {
            Storyboard storyboard = new Storyboard();

            DoubleAnimationUsingKeyFrames animation = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTarget(animation, container);
            Storyboard.SetTargetProperty(animation, "(Canvas.Top)");

            animation.KeyFrames.Add(new EasingDoubleKeyFrame()
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero),
                Value = -container.ActualHeight
            });
            animation.KeyFrames.Add(new EasingDoubleKeyFrame()
            {
                KeyTime = KeyTime.FromTimeSpan(animationDuration),
                Value = 0,
                EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseOut }
            });

            storyboard.Children.Add(animation);

            return storyboard;
        }

        internal static Storyboard CreateMoveToNextViewStoryboard(Panel container)
        {
            Storyboard storyboard = new Storyboard();

            DoubleAnimationUsingKeyFrames animation = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTarget(animation, container);
            Storyboard.SetTargetProperty(animation, "(Canvas.Top)");

            animation.KeyFrames.Add(new EasingDoubleKeyFrame()
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero),
                Value = container.ActualHeight
            });
            animation.KeyFrames.Add(new EasingDoubleKeyFrame()
            {
                KeyTime = KeyTime.FromTimeSpan(animationDuration),
                Value = 0,
                EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseOut }
            });

            storyboard.Children.Add(animation);

            return storyboard;
        }

        internal static Storyboard CreateMoveToUpperOrLowerViewStoryboard(Panel container)
        {
            Storyboard storyboard = new Storyboard();

            DoubleAnimationUsingKeyFrames scaleXAnimation = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTarget(scaleXAnimation, container);
            Storyboard.SetTargetProperty(scaleXAnimation, "(UIElement.RenderTransform).(ScaleTransform.ScaleX)");

            scaleXAnimation.KeyFrames.Add(new EasingDoubleKeyFrame()
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero),
                Value = 0
            });
            scaleXAnimation.KeyFrames.Add(new EasingDoubleKeyFrame()
            {
                KeyTime = KeyTime.FromTimeSpan(animationDuration),
                Value = 1,
                EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseOut }
            });

            DoubleAnimationUsingKeyFrames scaleYAnimation = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTarget(scaleYAnimation, container);
            Storyboard.SetTargetProperty(scaleYAnimation, "(UIElement.RenderTransform).(ScaleTransform.ScaleY)");

            scaleYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame()
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero),
                Value = 0
            });
            scaleYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame()
            {
                KeyTime = KeyTime.FromTimeSpan(animationDuration),
                Value = 1,
                EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseOut }
            });

            DoubleAnimationUsingKeyFrames opacityAnimation = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTarget(opacityAnimation, container);
            Storyboard.SetTargetProperty(opacityAnimation, "(UIElement.Opacity)");

            opacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame()
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero),
                Value = 0
            });
            opacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame()
            {
                KeyTime = KeyTime.FromTimeSpan(animationDuration),
                Value = 1,
                EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseOut }
            });

            storyboard.Children.Add(scaleXAnimation);
            storyboard.Children.Add(scaleYAnimation);
            storyboard.Children.Add(opacityAnimation);

            return storyboard;
        }
    }
}
