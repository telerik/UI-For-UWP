using System.Diagnostics.CodeAnalysis;

namespace Telerik.Core
{
    /// <summary>
    /// Provides a pre-defined move, scale and fade animation.
    /// </summary>
    public class RadScaleMoveAndFadeAnimation : RadAnimationGroup
    {
        private RadMoveAnimation move;
        private RadScaleAnimation scale;
        private RadFadeAnimation fade;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadScaleMoveAndFadeAnimation"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "These virtual calls do not rely on uninitialized base state.")]
        public RadScaleMoveAndFadeAnimation()
        {
            this.move = this.CreateMoveAnimation();
            this.scale = this.CreateScaleAnimation();
            this.fade = this.CreateFadeAnimation();

            this.Children.Add(this.fade);
            this.Children.Add(this.move);
            this.Children.Add(this.scale);
        }

        /// <summary>
        /// Gets the scale animation in this group.
        /// </summary>
        public RadScaleAnimation ScaleAnimation
        {
            get
            {
                return this.scale;
            }
        }

        /// <summary>
        /// Gets the move animation in this group.
        /// </summary>
        public RadMoveAnimation MoveAnimation
        {
            get
            {
                return this.move;
            }
        }

        /// <summary>
        /// Gets the fade animation in this group.
        /// </summary>
        public RadFadeAnimation FadeAnimation
        {
            get
            {
                return this.fade;
            }
        }

        /// <summary>
        /// Called in the constructor to create the default move animation.
        /// </summary>
        /// <returns>Returns a new instance of RadMoveAnimation.</returns>
        protected virtual RadMoveAnimation CreateMoveAnimation()
        {
            RadMoveAnimation result = new RadMoveAnimation();

            return result;
        }

        /// <summary>
        /// Called in the constructor to create the default scale animation.
        /// </summary>
        /// <returns>Returns a new instance of RadScaleAnimation.</returns>
        protected virtual RadScaleAnimation CreateScaleAnimation()
        {
            RadScaleAnimation result = new RadScaleAnimation();

            return result;
        }

        /// <summary>
        /// Called in the constructor to create the default fade animation.
        /// </summary>
        /// <returns>Returns a new instance of RadFadeAnimation.</returns>
        protected virtual RadFadeAnimation CreateFadeAnimation()
        {
            RadFadeAnimation result = new RadFadeAnimation();

            return result;
        }
    }
}
