using System.Diagnostics.CodeAnalysis;
using Windows.Foundation;

namespace Telerik.Core
{
    /// <summary>
    /// Provides a pre-defined move, scale and rotate animation.
    /// </summary>
    public class RadScaleMoveAndRotateAnimation : RadAnimationGroup
    {
        private RadScaleAnimation scale;
        private RadMoveAnimation move;
        private RadPlaneProjectionAnimation rotate;

        /// <summary>
        /// Initializes a new instance of the RadScaleMoveAndRotateAnimation class.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Call stack reviewed.")]
        public RadScaleMoveAndRotateAnimation()
        {
            this.scale = this.CreateScaleAnimation();
            this.move = this.CreateMoveAnimation();
            this.rotate = this.CreateRotateAnimation();

            this.Children.Add(this.scale);
            this.Children.Add(this.move);
            this.Children.Add(this.rotate);
        }

        /// <summary>
        /// Gets the scale animation for this group.
        /// </summary>
        public RadScaleAnimation ScaleAnimation
        {
            get
            {
                return this.scale;
            }
        }

        /// <summary>
        /// Gets the move animation for this group.
        /// </summary>
        public RadMoveAnimation MoveAnimation
        {
            get
            {
                return this.move;
            }
        }

        /// <summary>
        /// Gets the rotate animation for this group.
        /// </summary>
        public RadPlaneProjectionAnimation RotateAnimation
        {
            get
            {
                return this.rotate;
            }
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
        /// Called in the constructor to create the default move animation.
        /// </summary>
        /// <returns>Returns a new instance of RadMoveAnimation.</returns>
        protected virtual RadMoveAnimation CreateMoveAnimation()
        {
            RadMoveAnimation result = new RadMoveAnimation();
            result.StartPoint = new Point(0, 300);

            return result;
        }

        /// <summary>
        /// Called in the constructor to create the default rotate animation.
        /// </summary>
        /// <returns>Returns a new instance of RadPerspectiveAnimation.</returns>
        protected virtual RadPlaneProjectionAnimation CreateRotateAnimation()
        {
            RadPlaneProjectionAnimation result = new RadPlaneProjectionAnimation();
            result.StartAngleY = 90;
            result.EndAngleY = 360;

            return result;
        }
    }
}
