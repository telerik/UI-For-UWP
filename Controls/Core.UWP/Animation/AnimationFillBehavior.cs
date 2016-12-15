namespace Telerik.Core
{
    /// <summary>
    /// Defines the fill behavior for a <see cref="RadAnimation"/> instance.
    /// </summary>
    public enum AnimationFillBehavior
    {
        /// <summary>
        /// The behavior is inherited either from a parent <see cref="RadAnimationGroup"/> or the associated Storyboard.
        /// </summary>
        Inherit = 0,

        /// <summary>
        /// Same as <see cref="T:System.Windows.Media.Animation.FillBehavior.HoldEnd"/>.
        /// </summary>
        HoldEnd,

        /// <summary>
        /// Same as <see cref="T:System.Windows.Media.Animation.FillBehavior.Stop"/>.
        /// </summary>
        Stop,
    }
}