using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.Core
{
    /// <summary>
    /// Slides the target element from a start point to an end point.
    /// </summary>
    public class RadSlideAnimation : RadMoveAnimation
    {
        private static readonly DependencyProperty ClipStartYProperty =
            DependencyProperty.RegisterAttached("ClipStartY", typeof(double), typeof(RadSlideAnimation), new PropertyMetadata(null, OnClipChanged));

        private static readonly DependencyProperty ClipStartXProperty =
            DependencyProperty.RegisterAttached("ClipStartX", typeof(double), typeof(RadSlideAnimation), new PropertyMetadata(null, OnClipChanged));

        private bool reverseClipAnimation = false;

        /// <summary>
        /// Initializes a new instance of the RadSlideAnimation class.
        /// </summary>
        public RadSlideAnimation()
        {
            this.Easing = null;
        }

        /// <summary>
        /// Creates a new instance of this animation that is the reverse of this instance.
        /// </summary>
        /// <returns>A new instance of this animation that is the reverse of this instance.</returns>
        public override RadAnimation CreateOpposite()
        {
            RadSlideAnimation opposite = base.CreateOpposite() as RadSlideAnimation;
            opposite.reverseClipAnimation = !opposite.reverseClipAnimation;
            return opposite;
        }

        internal override EasingFunctionBase ComposeEasingFunction()
        {
            return null;
        }

        /// <summary>
        /// Core update routine.
        /// </summary>
        /// <param name="context">An AnimationContext that provides internal animation information.</param>
        protected override void UpdateAnimationOverride(AnimationContext context)
        {
            base.UpdateAnimationOverride(context);

            if (context == null)
            {
                return;
            }

            Point from = GetPoint(context.Target, this.StartPoint);
            Point to = GetPoint(context.Target, this.EndPoint);
            double duration = this.Duration.TimeSpan.TotalSeconds;

            RectangleGeometry geom = new RectangleGeometry();
            context.Target.Clip = geom;

            if (this.reverseClipAnimation)
            {
                context.SingleProperty("ClipStartXProperty", 0, 0, duration, -to.X);
                context.SingleProperty("ClipStartYProperty", 0, 0, duration, -to.Y);
            }
            else
            {
                context.SingleProperty("ClipStartXProperty", 0, -from.X, duration, 0);
                context.SingleProperty("ClipStartYProperty", 0, -from.Y, duration, 0);
            }

            SetAnimationCompleteCallback(context);
        }

        private static void SetAnimationCompleteCallback(AnimationContext context)
        {
            EventHandler<object> completeHandler = null;
            completeHandler += (sender, e) =>
            {
                context.Target.ClearValue(UIElement.ClipProperty);
                context.Storyboard.Completed -= completeHandler;
            };
            context.Storyboard.Completed += completeHandler;
        }

        private static void OnClipChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = d as UIElement;

            RectangleGeometry clip = d.GetValue(UIElement.ClipProperty) as RectangleGeometry;
            if (clip == null)
            {
                return;
            }

            double newVal = (double)e.NewValue;

            Point location = new Point(clip.Rect.X, clip.Rect.Y);

            if (e.Property == RadSlideAnimation.ClipStartYProperty)
            {
                location.Y = newVal;
            }
            else
            {
                location.X = newVal;
            }

            clip.Rect = new Rect(location, element.RenderSize);
        }
    }
}