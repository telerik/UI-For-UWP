using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.Core
{
    /// <summary>
    /// This is the base class of the move X and move Y animations.
    /// </summary>
    public abstract class MoveAnimationBase : RadAnimation
    {
        /// <summary>
        /// Initializes a new instance of the MoveAnimationBase class.
        /// </summary>
        protected MoveAnimationBase()
        {
            this.MiddlePointsAxis = new DoubleKeyFrameCollection();
        }

        /// <summary>
        /// Gets or sets a value that determines how the move animation's start and end points will be interpreted.
        /// Absolute means that they will be interpreted as pixels and Relative means that they will interpreted as
        /// points in the [-1, 1] range.
        /// </summary>
        public MoveAnimationPointMode PointMode { get; set; }

        /// <summary>
        /// Gets or sets the middle points.
        /// </summary>
        /// <value>The middle points.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public DoubleKeyFrameCollection MiddlePointsAxis { get; set; }

        /// <inheritdoc/>
        public override void ApplyInitialValues(UIElement target)
        {
            base.ApplyInitialValues(target);
            target.EnsureDefaultTransforms();
        }

        /// <summary>
        /// Creates a new instance of this animation that is the reverse of this instance.
        /// </summary>
        /// <returns>A new instance of this animation that is the reverse of this instance.</returns>
        public override RadAnimation CreateOpposite()
        {
            MoveAnimationBase reverse = base.CreateOpposite() as MoveAnimationBase;
            reverse.SwapValues();

            return reverse;
        }

        /// <summary>
        /// This method must be overridden and is called when this move animation has to swap its start and end values.
        /// This is necessary when an opposite animation is created.
        /// </summary>
        protected abstract void SwapValues();

        /// <summary>
        /// Gets an array of interleaved duration/value pairs for this move animation's keyframes.
        /// </summary>
        /// <param name="startValue">The value of the first frame.</param>
        /// <param name="endValue">The value of the last frame.</param>
        /// <returns>Returns an array of interleaved duration/value pairs for this move animation's keyframes.</returns>
        protected double[] GetMoveArguments(double startValue, double endValue)
        {
            DoubleKeyFrameCollection middleValues = this.MiddlePointsAxis;
            double duration = this.Duration.TimeSpan.TotalSeconds;

            double[] result = new double[(this.MiddlePointsAxis.Count * 2) + 4]; // +4 for end and start arguments ...

            result[0] = 0;
            result[1] = startValue;
            for (int i = 0; i < middleValues.Count; i++)
            {
                int arrayIndex = 2 + (i * 2);
                result[arrayIndex] = middleValues[i].KeyTime.TimeSpan.TotalSeconds;
                result[arrayIndex + 1] = middleValues[i].Value;
            }

            result[(middleValues.Count * 2) + 2] = duration;
            result[(middleValues.Count * 2) + 3] = endValue;

            return result;
        }
    }
}