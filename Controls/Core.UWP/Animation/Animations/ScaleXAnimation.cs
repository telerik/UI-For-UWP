using System;
using System.Diagnostics.CodeAnalysis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Telerik.Core
{
    /// <summary>
    /// This animation animates an element only on its X axis.
    /// </summary>
    public class RadScaleXAnimation : RadAnimation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RadScaleXAnimation"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "These virtual calls do not rely on uninitialized base state.")]
        public RadScaleXAnimation()
        {
            this.StartScaleX = 0.5;
            this.EndScaleX = 1;
            this.Duration = new Duration(TimeSpan.FromSeconds(.2d));
        }

        /// <summary>
        /// Gets or sets the start scale on the X axis.
        /// </summary>
        public double? StartScaleX { get; set; }

        /// <summary>
        /// Gets or sets the end scale on the X axis.
        /// </summary>
        public double? EndScaleX { get; set; }

        /// <summary>
        /// Gets or sets a scale factor for Y property of the scale transform.
        /// </summary>
        [Obsolete("Do not use. This property will be removed in Q3 2012.")]
        public double ScaleY { get; set; }

        /// <summary>
        /// Removes any property modifications, applied to the specified element by this instance.
        /// </summary>
        /// <param name="target">The element which property values are to be cleared.</param>
        /// <remarks>
        /// It is assumed that the element has been previously animated by this animation.
        /// </remarks>
        public override void ClearAnimation(UIElement target)
        {
            ScaleTransform transform = target.GetScaleTransform();
            transform.ClearValue(ScaleTransform.ScaleXProperty);
        }

        /// <summary>
        /// Creates a new instance of this animation that is the reverse of this instance.
        /// </summary>
        /// <returns>A new instance of this animation that is the reverse of this instance.</returns>
        public override RadAnimation CreateOpposite()
        {
            RadScaleXAnimation opposite = base.CreateOpposite() as RadScaleXAnimation;
            double? tmp = opposite.StartScaleX;
            opposite.StartScaleX = opposite.EndScaleX;
            opposite.EndScaleX = tmp;

            return opposite;
        }

        /// <summary>
        /// Sets the initial animation values to the provided target element.
        /// </summary>
        public override void ApplyInitialValues(UIElement target)
        {
            base.ApplyInitialValues(target);

            AnimationContext context = new AnimationContext(target);
            if (this.StartScaleX.HasValue)
            {
                context.InitializeScaleX(this.StartScaleX.Value);
            }
        }

        /// <inheritdoc/>
        protected internal override void ApplyAnimationValues(PlayAnimationInfo info)
        {
            if (info == null)
            {
                return;
            }

            double? scaleX;

            if (this.GetAutoReverse())
            {
                scaleX = this.StartScaleX;
            }
            else
            {
                scaleX = this.EndScaleX;
            }

            ScaleTransform transform = info.Target.GetScaleTransform();
            transform.ScaleX = scaleX.HasValue ? scaleX.Value : transform.ScaleX;
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

            context.EnsureDefaultTransforms();
            ScaleTransform transform = context.Target.GetScaleTransform();

            double fromX = this.StartScaleX.HasValue ? this.StartScaleX.Value : transform.ScaleX;
            double toX = this.EndScaleX.HasValue ? this.EndScaleX.Value : transform.ScaleX;

            double duration = this.Duration.TimeSpan.TotalSeconds;

            context.ScaleX(0, fromX, duration, toX);

            base.UpdateAnimationOverride(context);
        }
    }
}