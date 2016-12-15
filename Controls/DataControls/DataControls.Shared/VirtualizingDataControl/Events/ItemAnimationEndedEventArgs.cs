using System;
using Telerik.Core;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Holds information about a single item animation in <see cref="RadDataBoundListBox"/> that
    /// has ended, as well as the amount of animations that are still playing.
    /// </summary>
    public class ItemAnimationEndedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the count of all animations of the same <see cref="RadAnimation"/> 
        /// instance that are still playing.
        /// </summary>
        public int RemainingAnimationsCount { get; internal set; }

        /// <summary>
        /// Gets the <see cref="RadAnimation"/> instance that has ended.
        /// </summary>
        public RadAnimation Animation { get; internal set; }
    }
}
