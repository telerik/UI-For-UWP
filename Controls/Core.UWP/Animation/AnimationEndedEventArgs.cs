using System.ComponentModel;

namespace Telerik.Core
{
    /// <summary>
    /// Contains information about the <see cref="RadAnimation.Ended"/> event.
    /// </summary>
    public class AnimationEndedEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationEndedEventArgs" /> class.
        /// </summary>
        /// <param name="target">The target.</param>
        internal AnimationEndedEventArgs(PlayAnimationInfo target)
        {
            this.AnimationInfo = target;
        }

        /// <summary>
        /// Gets the <see cref="Windows.UI.Xaml.UIElement"/> that was animated by the animation
        /// for which the <see cref="RadAnimation.Ended"/> event fires.
        /// </summary>
        public PlayAnimationInfo AnimationInfo { get; private set; }
    }
}