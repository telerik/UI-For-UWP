using System;
using System.Diagnostics.CodeAnalysis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Telerik.Core
{
    /// <summary>
    /// This animation animates an element only on its Y axis.
    /// </summary>
    public class RadScaleYAnimation : RadAnimation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RadScaleYAnimation"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "These virtual calls do not rely on uninitialized base state.")]
        public RadScaleYAnimation()
        {
            this.StartScaleY = 0.5;
            this.EndScaleY = 1;
            this.Duration = new Duration(TimeSpan.FromSeconds(.2d));
        }

        /// <summary>
        /// Gets or sets the start scale on the Y axis.
        /// </summary>
        public double? StartScaleY
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the end scale on the Y axis.
        /// </summary>
        public double? EndScaleY
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a scale factor for X property of the scale transform.
        /// </summary>
        [Obsolete("Do not use. This property will be removed in Q3 2012.")]
        public double ScaleX
        {
            get;
            set;
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
            ScaleTransform transform = target.GetScaleTransform();
            transform.ClearValue(ScaleTransform.ScaleYProperty);
        }

        /// <summary>
        /// Creates a new instance of this animation that is the reverse of this instance.
        /// </summary>
        /// <returns>A new instance of this animation that is the reverse of this instance.</returns>
        public override RadAnimation CreateOpposite()
        {
            RadScaleYAnimation opposite = base.CreateOpposite() as RadScaleYAnimation;
            double? tmp = opposite.StartScaleY;
            opposite.StartScaleY = opposite.EndScaleY;
            opposite.EndScaleY = tmp;

            return opposite;
        }

        /// <summary>
        /// Sets the initial animation values to the provided target element.
        /// </summary>
        public override void ApplyInitialValues(UIElement target)
        {
            base.ApplyInitialValues(target);

            AnimationContext context = new AnimationContext(target);
            if (this.StartScaleY.HasValue)
            {
                context.InitializeScaleY(this.StartScaleY.Value);
            }
        }

        /// <inheritdoc/>
        protected internal override void ApplyAnimationValues(PlayAnimationInfo info)
        {
            if (info == null)
            {
                return;
            }

            double? scaleY;

            if (this.GetAutoReverse())
            {
                scaleY = this.StartScaleY;
            }
            else
            {
                scaleY = this.EndScaleY;
            }

            ScaleTransform transform = info.Target.GetScaleTransform();
            transform.ScaleY = scaleY.HasValue ? scaleY.Value : transform.ScaleY;
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

            double fromY = this.StartScaleY.HasValue ? this.StartScaleY.Value : transform.ScaleY;
            double toY = this.EndScaleY.HasValue ? this.EndScaleY.Value : transform.ScaleY;

            double duration = this.Duration.TimeSpan.TotalSeconds;

            context.ScaleY(0, fromY, duration, toY);

            base.UpdateAnimationOverride(context);
        }
    }
}