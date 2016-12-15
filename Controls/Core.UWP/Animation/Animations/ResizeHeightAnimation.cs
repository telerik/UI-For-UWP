using Windows.UI.Xaml;

namespace Telerik.Core
{
    /// <summary>
    /// This class represents a resize animation that animates the FrameworkElement.Height property.
    /// </summary>
    public class RadResizeHeightAnimation : RadAnimation
    {
        /// <summary>
        /// Gets or sets the initial height. If no value is applied current element height is used.
        /// </summary>
        public double? StartHeight { get; set; }

        /// <summary>
        /// Gets or sets the final height of the animation target. If no value is applied current element height is used.
        /// </summary>
        public double? EndHeight { get; set; }

        /// <summary>
        /// Gets or sets a the width that will be applied to the animation target during this animation.
        /// </summary>
        public double? Width { get; set; }

        /// <summary>
        /// Creates a new instance of this animation that is the reverse of this instance.
        /// </summary>
        /// <returns>A new instance of this animation that is the reverse of this instance.</returns>
        public override RadAnimation CreateOpposite()
        {
            RadResizeHeightAnimation opposite = base.CreateOpposite() as RadResizeHeightAnimation;
            double? tmp = opposite.StartHeight;
            opposite.StartHeight = opposite.EndHeight;
            opposite.EndHeight = tmp;

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

        /// <inheritdoc/>
        public override void ApplyInitialValues(UIElement target)
        {
            base.ApplyInitialValues(target);
            AnimationContext context = new AnimationContext(target);

            if (this.StartHeight.HasValue)
            {
                context.InitializeHeight(this.StartHeight.Value);
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
            double height = this.GetAutoReverse() ? GetHeight(target, this.StartHeight) : GetHeight(target, this.EndHeight);

            target.SetValue(FrameworkElement.HeightProperty, height);
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
            double startHeight = GetHeight(context.Target, this.StartHeight);
            double endHeight = GetHeight(context.Target, this.EndHeight);

            context.EnsureDefaultTransforms();

            if (this.Width.HasValue)
            {
                (context.Target as FrameworkElement).Width = this.Width.Value;
            }

            context.Height(0, startHeight, duration, endHeight);
            base.UpdateAnimationOverride(context);
        }

        private static double GetHeight(UIElement target, double? height)
        {
            if (height.HasValue)
            {
                return height.Value;
            }

            return (double)target.GetValue(FrameworkElement.ActualHeightProperty);
        }
    }
}