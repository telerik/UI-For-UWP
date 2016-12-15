using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Telerik.Core
{
    /// <summary>
    /// Represents a move animation on the Y axis.
    /// </summary>
    public class RadMoveYAnimation : MoveAnimationBase
    {
        /// <summary>
        /// Gets or sets the start Y value.
        /// </summary>
        public double? StartY { get; set; }

        /// <summary>
        /// Gets or sets the end Y value.
        /// </summary>
        public double? EndY { get; set; }

        /// <summary>
        /// Removes any property modifications, applied to the specified element by this instance.
        /// </summary>
        /// <param name="target">The element which property values are to be cleared.</param>
        /// <remarks>
        /// It is assumed that the element has been previously animated by this animation.
        /// </remarks>
        public override void ClearAnimation(UIElement target)
        {
            TranslateTransform transform = target.GetTranslateTransform();
            transform.ClearValue(TranslateTransform.YProperty);
        }

        /// <inheritdoc/>
        public override void ApplyInitialValues(UIElement target)
        {
            base.ApplyInitialValues(target);

            AnimationContext initializationContext = new AnimationContext(target);

            double startY = this.Initialize(initializationContext.Target as FrameworkElement, this.StartY);
            initializationContext.InitializeMoveY(startY);
        }

        /// <inheritdoc/>
        protected internal override void ApplyAnimationValues(PlayAnimationInfo info)
        {
            if (info == null)
            {
                return;
            }

            double endY = 0;

            FrameworkElement target = info.Target as FrameworkElement;

            if (this.GetAutoReverse())
            {
                if (this.PointMode == MoveAnimationPointMode.Absolute)
                {
                    endY = GetY(target, this.StartY);
                }
                else
                {
                    endY = GetRelativeY(target, this.StartY);
                }
            }
            else
            {
                if (this.PointMode == MoveAnimationPointMode.Absolute)
                {
                    endY = GetY(target, this.EndY);
                }
                else
                {
                    endY = GetRelativeY(target, this.EndY);
                }
            }

            target.GetTranslateTransform().Y = endY;
        }

        /// <summary>
        /// Gets the value of a nullable Y value if it is not null. If it is null it returns a value that represents the Y
        /// component of the target's TranslateTransform.
        /// </summary>
        /// <param name="target">The target from which to obtain a Y value if the provided Y value is null.</param>
        /// <param name="y">A nullable Y value.</param>
        /// <returns>Returns the value of a nullable Y value if it is not null. If it is null it returns a point that represents the Y
        /// component of the target's TranslateTransform.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "y")]
        protected static double GetY(UIElement target, double? y)
        {
            if (target == null)
            {
                return double.NaN;
            }

            if (y.HasValue)
            {
                return y.Value;
            }

            return target.GetTranslateTransform().Y;
        }

        /// <summary>
        /// Gets a Y value that is relative to the height of the animation target.
        /// </summary>
        /// <param name="target">The animation target.</param>
        /// <param name="y">The Y value in relative coordinates.</param>
        /// <returns>Returns a Y value in absolute coordinates based on the target's height and the relative value.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "y")]
        protected static double GetRelativeY(FrameworkElement target, double? y)
        {
            if (target == null)
            {
                return double.NaN;
            }

            if (y.HasValue)
            {
                return target.ActualHeight * y.Value;
            }

            return target.GetTranslateTransform().Y;
        }

        /// <summary>
        /// This method must be overridden and is called when this move animation has to swap its start and end values.
        /// This is necessary when an opposite animation is created.
        /// </summary>
        protected override void SwapValues()
        {
            double? tmp = this.StartY;
            this.StartY = this.EndY;
            this.EndY = tmp;
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

            double startY = this.Initialize(context.Target as FrameworkElement, this.StartY);
            double endY = this.Initialize(context.Target as FrameworkElement, this.EndY);

            context.MoveY(this.GetMoveArguments(startY, endY));

            base.UpdateAnimationOverride(context);
        }

        private double Initialize(FrameworkElement target, double? value)
        {
            if (this.PointMode == MoveAnimationPointMode.Absolute)
            {
                return GetY(target, value);
            }
            else
            {
                return GetRelativeY(target, value);
            }
        }
    }
}