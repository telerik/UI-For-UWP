using System;
using System.Reflection;
using Telerik.UI.Xaml.Controls.Chart;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace SDKExamples.UWP.Chart
{
    public class ChartAnimationsHelper
    {
        private static Storyboard slideInStoryboard;
        private static DoubleAnimation slideInAnimation;

        public static readonly DependencyProperty AllowSlideAnimationOnStartProperty =
            DependencyProperty.RegisterAttached("AllowSlideAnimationOnStart", typeof(bool), typeof(ChartAnimationsHelper), new PropertyMetadata(false, OnAllowSlideAnimationOnStartChanged));

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
            var chart = d as RadChartBase;
            if (chart != null && e.NewValue != e.OldValue)
            {
                if ((bool)e.NewValue)
                {
                    chart.Loaded += OnChartLoaded;
                }
                else
                {
                    chart.Loaded -= OnChartLoaded;
                }
            }
        }

        private static void OnChartLoaded(object sender, RoutedEventArgs e)
        {
            var renderSurface = sender.GetType().GetProperty("RenderSurface", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(sender) as Canvas;
            if (renderSurface != null)
            {
                PrepareSlideInAnimation(renderSurface);
                slideInStoryboard.Begin();
            }
        }

        private static void PrepareSlideInAnimation(Canvas renderSurface)
        {
            slideInAnimation = new DoubleAnimation();
            slideInStoryboard = new Storyboard();

            slideInAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(3000));
            slideInAnimation.From = 1;
            slideInAnimation.To = 0;
            Storyboard.SetTargetProperty(slideInAnimation, "Opacity");
            Storyboard.SetTarget(slideInAnimation, renderSurface);
            slideInStoryboard.Children.Add(slideInAnimation);
        }
    }
}
