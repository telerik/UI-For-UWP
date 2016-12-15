using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Represents the possible tilt interaction states.
    /// </summary>
    internal enum TiltInteractionState
    {
        /// <summary>
        /// The element is currently being tilted.
        /// </summary>
        Tilting,

        /// <summary>
        /// A tilt effect reset procedure is started.
        /// </summary>
        EndingTilting,

        /// <summary>
        /// A tilt effect reset procedure is finished.
        /// </summary>
        EndedTilting
    }

    /// <summary>
    /// Represents a context describing a tilted element.
    /// </summary>
    internal class TiltInteractionInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TiltInteractionInfo"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="resetAnimation">The reset animation.</param>
        internal TiltInteractionInfo(FrameworkElement target, Storyboard resetAnimation)
        {
            this.RootVisual = Window.Current.Content as FrameworkElement;
            this.TargetElement = target;
            this.ResetAnimation = resetAnimation;
            resetAnimation.Completed += this.OnResetAnimation_Completed;
        }

        /// <summary>
        /// Gets the target element that is tilted.
        /// </summary>
        /// <value>The target element.</value>
        internal FrameworkElement TargetElement { get; private set; }

        internal FrameworkElement RootVisual { get; private set; }

        /// <summary>
        /// Gets the tilt reset animation.
        /// </summary>
        /// <value>The reset animation.</value>
        internal Storyboard ResetAnimation { get; private set; }

        /// <summary>
        /// Gets or sets the current tilt interaction state.
        /// </summary>
        /// <value>The state.</value>
        internal TiltInteractionState State { get; set; }

        internal void Clear(bool animate)
        {
            if (animate)
            {
                if (this.ResetAnimation != null)
                {
                    this.ResetAnimation.Begin();
                }
            }
            else
            {
                this.Clear();
            }
        }

        private void Clear()
        {
            if (this.ResetAnimation != null)
            {
                this.ResetAnimation.Completed -= this.OnResetAnimation_Completed;
                this.TargetElement.ClearValue(FrameworkElement.RenderTransformProperty);
                this.TargetElement.ClearValue(FrameworkElement.ProjectionProperty);
                this.ResetAnimation.Stop();
                this.ResetAnimation = null;
                this.State = TiltInteractionState.EndedTilting;
            }
        }

        private void OnResetAnimation_Completed(object sender, object args)
        {
            this.Clear();
        }
    }
}