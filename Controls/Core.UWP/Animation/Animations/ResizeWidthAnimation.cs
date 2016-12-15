using Windows.UI.Xaml;

namespace Telerik.Core
{
    /// <summary>
    /// This class represents a resize animation that animates the FrameworkElement.Width property.
    /// </summary>
    public class RadResizeWidthAnimation : RadAnimation
    {
        /// <summary>
        /// Gets or sets the initial width. If no value is applied current element width is used.
        /// </summary>
        public double? StartWidth { get; set; }

        /// <summary>
        /// Gets or sets the final width of the animation target. If no value is applied current element width is used.
        /// </summary>
        public double? EndWidth { get; set; }

        /// <summary>
        /// Gets or sets a the height that will be applied to the animation target during this animation.
        /// </summary>
        public double? Height { get; set; }

        /// <summary>
        /// Creates a new instance of this animation that is the reverse of this instance.
        /// </summary>
        /// <returns>A new instance of this animation that is the reverse of this instance.</returns>
        public override RadAnimation CreateOpposite()
        {
            RadResizeWidthAnimation opposite = base.CreateOpposite() as RadResizeWidthAnimation;
            double? tmp = opposite.StartWidth;
            opposite.StartWidth = opposite.EndWidth;
            opposite.EndWidth = tmp;

            return opposite;
        }

        /// <summary>
        /// Removes any property modifications, applied to the specified element by this instance.
        /// </summary>
        /// <param name="target">The element which property values are to be cleared.</param>
        /// <remarks>
        /// It is assumed that the element has been previously animated by this animation.
        /// </remarks>
        public override void ClearAnimation(UIElement target)
        {
            target.ClearValue(FrameworkElement.WidthProperty);
        }

        /// <summary>
        /// Sets the initial animation values to the provided target element.
        /// </summary>
        public override void ApplyInitialValues(UIElement target)
        {
            base.ApplyInitialValues(target);
            AnimationContext context = new AnimationContext(target);

            if (this.StartWidth.HasValue)
            {
                context.InitializeWidth(this.StartWidth.Value);
            }
        }

        /// <inheritdoc/>
        protected internal override void ApplyAnimationValues(PlayAnimationInfo info)
        {
            if (info == null)
            {
                return;
            }

            UIElement target = info.Target;
            double width = this.GetAutoReverse() ? GetWidth(target, this.StartWidth) : GetWidth(target, this.EndWidth);

            target.SetValue(FrameworkElement.WidthProperty, width);
        }

        /// <summary>
        /// Core update routine.
        /// </summary>
        /// <param name="context">The context that holds information about the animation.</param>
        protected override void UpdateAnimationOverride(AnimationContext context)
        {
            if (context == null)
            {
                return;
            }

            double duration = this.Duration.TimeSpan.TotalSeconds;
            double startWidth = GetWidth(context.Target, this.StartWidth);
            double endWidth = GetWidth(context.Target, this.EndWidth);

            context.EnsureDefaultTransforms();

            context.Width(0, startWidth, duration, endWidth);

            if (this.Height.HasValue)
            {
                (context.Target as FrameworkElement).Height = this.Height.Value;
            }

            base.UpdateAnimationOverride(context);
        }

        private static double GetWidth(UIElement target, double? width)
        {
            if (width.HasValue)
            {
                return width.Value;
            }

            return (double)target.GetValue(FrameworkElement.ActualWidthProperty);
        }
    }
}