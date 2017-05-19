using System;
using System.Diagnostics.CodeAnalysis;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace Telerik.Core
{
    /// <summary>
    /// Provides a pre-defined move and fade animation group.
    /// </summary>
    public class RadMoveAndFadeAnimation : RadAnimationGroup
    {
        private RadMoveAnimation move;
        private RadFadeAnimation fade;

        /// <summary>
        /// Initializes a new instance of the RadMoveAndFadeAnimation class.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")] // These virtual calls do not rely on uninitialized base state.
        public RadMoveAndFadeAnimation()
        {
            this.move = this.CreateMoveAnimation();
            this.fade = this.CreateFadeAnimation();

            this.Children.Add(this.move);
            this.Children.Add(this.fade);
        }

        /// <summary>
        /// Gets the start position for the move animation.
        /// If not set, the current element TranslateTransform is used if it exists, otherwise the empty point (0, 0) is used.
        /// </summary>
        public RadFadeAnimation FadeAnimation
        {
            get
            {
                return this.fade;
            }
        }

        /// <summary>
        /// Gets the end position for the move animation.
        /// If not set, the current element TranslateTransform is used if it exists, otherwise the empty point (0, 0) is used.
        /// </summary>
        public RadMoveAnimation MoveAnimation
        {
            get
            {
                return this.move;
            }
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
            this.MoveAnimation.ClearAnimation(target);
            this.FadeAnimation.ClearAnimation(target);
            base.ClearAnimation(target);
        }

        /// <summary>
        /// Called in the constructor to create a default move animation.
        /// </summary>
        /// <returns>A new instance of RadMoveAnimation.</returns>
        protected virtual RadMoveAnimation CreateMoveAnimation()
        {
            return new RadMoveAnimation()
            {
                StartPoint = new Point(0, 300)
            };
        }

        /// <summary>
        /// Called in the constructor to create a default fade animation.
        /// </summary>
        /// <returns>A new instance of RadFadeAnimation.</returns>
        protected virtual RadFadeAnimation CreateFadeAnimation()
        {
            return new RadFadeAnimation()
            {
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };
        }
    }
}