using System.Diagnostics.CodeAnalysis;

namespace Telerik.Core
{
    /// <summary>
    /// Represents pre-defined animation group that moves and scales an element.
    /// </summary>
    public class RadScaleAndMoveAnimation : RadAnimationGroup
    {
        private RadMoveAnimation move;
        private RadScaleAnimation scale;

        /// <summary>
        /// Initializes a new instance of the RadScaleAndMoveAnimation class.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "These virtual calls do not rely on uninitialized base state.")]
        public RadScaleAndMoveAnimation()
        {
            this.move = this.CreateMoveAnimation();
            this.scale = this.CreateScaleAnimation();

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
    }
}
