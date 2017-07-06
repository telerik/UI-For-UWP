using System;
using System.Reflection;
using Telerik.UI.Xaml.Controls.Chart;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace SDKExamples.UWP.Chart
{
    public class SeriesAnimationsHelper
    {
        public static readonly DependencyProperty AllowSlideAnimationOnStartProperty =
            DependencyProperty.RegisterAttached("AllowSlideAnimationOnStart", typeof(bool), typeof(SeriesAnimationsHelper), new PropertyMetadata(false, OnAllowSlideAnimationOnStartChanged));

        public static bool GetAllowSlideAnimationOnStart(DependencyObject obj)
        {
            return (bool)obj.GetValue(AllowSlideAnimationOnStartProperty);
        }

        public static void SetAllowSlideAnimationOnStart(DependencyObject obj, bool value)
        {
            obj.SetValue(AllowSlideAnimationOnStartProperty, value);
        }

        private static void OnAllowSlideAnimationOnStartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var series = d as PointTemplateSeries;
            if (series != null && e.NewValue != e.OldValue)
            {
                if ((bool)e.NewValue)
                {
                    series.Loaded += OnSeriesLoaded;
                }
                else
                {
                    series.Loaded -= OnSeriesLoaded;
                }
            }
        }

        private static void OnSeriesLoaded(object sender, RoutedEventArgs e)
        {
            var renderSurface = sender.GetType().GetProperty("RenderSurface", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(sender) as Canvas;
            if (renderSurface != null && renderSurface.Clip != null)
            {
                TriggerSlideInAnimation(renderSurface);
            }
        }

        private static void TriggerSlideInAnimation(Canvas renderSurface, int duration = 500)
        {
            var slideInStoryboard = new Storyboard();
            var currentClipRect = renderSurface.Clip.Rect;

            renderSurface.Clip.Rect = new Windows.Foundation.Rect(currentClipRect.X, currentClipRect.Y, 0, currentClipRect.Height);

            ObjectAnimationUsingKeyFrames rectAnimation = new ObjectAnimationUsingKeyFrames();
            rectAnimation.Duration = TimeSpan.FromMilliseconds(duration);
            double widthCoeff = currentClipRect.Width / duration;
            double calculatedWidth = 0;
            for (double i = 0; i <= duration; i += widthCoeff)
            {
                calculatedWidth = i * widthCoeff;
                rectAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame()
                {
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(i)),
                    Value = new RectangleGeometry() { Rect = new Windows.Foundation.Rect(currentClipRect.X, currentClipRect.Y, calculatedWidth, currentClipRect.Height) }
                });
            }

            if (calculatedWidth < currentClipRect.Width)
            {
                rectAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame()
                {
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(duration)),
                    Value = new RectangleGeometry() { Rect = new Windows.Foundation.Rect(currentClipRect.X, currentClipRect.Y, currentClipRect.Width, currentClipRect.Height) }
                });
            }

            Storyboard.SetTargetProperty(rectAnimation, "(UIElement.Clip)");
            Storyboard.SetTarget(rectAnimation, renderSurface);

            slideInStoryboard.Children.Add(rectAnimation);
            slideInStoryboard.Begin();
        }
    }
}
