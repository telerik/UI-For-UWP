using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Telerik.Core
{
    /// <summary>
    /// Represents a move animation on the X axis.
    /// </summary>
    public class RadMoveXAnimation : MoveAnimationBase
    {
        /// <summary>
        /// Gets or sets the start X value.
        /// </summary>
        public double? StartX { get; set; }

        /// <summary>
        /// Gets or sets the end X value.
        /// </summary>
        public double? EndX { get; set; }

        /// <summary>
        /// Removes any property modifications, applied to the specified element by this instance.
        /// </summary>
        /// <param name="target">The element which property values are to be cleared.</param>
        /// <remarks>
        /// It is assumed that the element has been previously animated by this animation.
        /// </remarks>
        public override void ClearAnimation(UIElement target)
        {
            target.GetTranslateTransform().ClearValue(TranslateTransform.XProperty);
        }

        /// <inheritdoc/>
        public override void ApplyInitialValues(UIElement target)
        {
            base.ApplyInitialValues(target);

            AnimationContext initializationContext = new AnimationContext(target);

            double startX = this.Initialize(initializationContext.Target as FrameworkElement, this.StartX);
            initializationContext.InitializeMoveX(startX);
        }

        /// <inheritdoc/>
        protected internal override void ApplyAnimationValues(PlayAnimationInfo info)
        {
            if (info == null)
            {
                return;
            }

            double endX = 0;

            FrameworkElement target = info.Target as FrameworkElement;

            if (this.GetAutoReverse())
            {
                if (this.PointMode == MoveAnimationPointMode.Absolute)
                {
                    endX = GetX(target, this.StartX);
                }
                else
                {
                    endX = GetRelativeX(target, this.StartX);
                }
            }
            else
            {
                if (this.PointMode == MoveAnimationPointMode.Absolute)
                {
                    endX = GetX(target, this.EndX);
                }
                else
                {
                    endX = GetRelativeX(target, this.EndX);
                }
            }

            target.GetTranslateTransform().X = endX;
        }

        /// <summary>
        /// Gets the value of a nullable X value if it is not null. If it is null it returns a point that represents the X
        /// component of the target's TranslateTransform.
        /// </summary>
        /// <param name="target">The target from which to obtain a point if the provided point is null.</param>
        /// <param name="x">A nullable X coordinate.</param>
        /// <returns>Returns the value of a nullable X value if it is not null. If it is null it returns a value that represents the X
        /// component of the target's TranslateTransform.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "x")]
        protected static double GetX(UIElement target, double? x)
        {
            if (target == null)
            {
                return double.NaN;
            }

            if (x.HasValue)
            {
                return x.Value;
            }

            return target.GetTranslateTransform().X;
        }

        /// <summary>
        /// Gets an X value that is relative to the size of the animation target.
        /// </summary>
        /// <param name="target">The animation target.</param>
        /// <param name="x">The X value in relative coordinates.</param>
        /// <returns>Returns an X value in absolute coordinates based on the target's size and the relative argument.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "x")]
        protected static double GetRelativeX(FrameworkElement target, double? x)
        {
            if (target == null)
            {
                return double.NaN;
            }

            if (x.HasValue)
            {
                return target.ActualWidth * x.Value;
            }

            return target.GetTranslateTransform().X;
        }

        /// <summary>
        /// This method must be overridden and is called when this move animation has to swap its start and end values.
        /// This is necessary when an opposite animation is created.
        /// </summary>
        protected override void SwapValues()
        {
            double? tmp = this.StartX;
            this.StartX = this.EndX;
            this.EndX = tmp;
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

            FrameworkElement target = context.Target as FrameworkElement;

            double startX = this.Initialize(target, this.StartX);
            double endX = this.Initialize(target, this.EndX);

            context.MoveX(this.GetMoveArguments(startX, endX));

            base.UpdateAnimationOverride(context);
        }

        private double Initialize(FrameworkElement target, double? value)
        {
            if (this.PointMode == MoveAnimationPointMode.Absolute)
            {
                return GetX(target, value);
            }
            else
            {
                return GetRelativeX(target, value);
            }
        }
    }
}