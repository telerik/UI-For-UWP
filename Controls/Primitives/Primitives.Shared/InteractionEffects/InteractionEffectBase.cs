using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Represents a base class for all interaction effects that can be applied to an element.
    /// </summary>
    public abstract class InteractionEffectBase : DependencyObject
    {
        /// <summary>
        /// Cancels the current interaction effect.
        /// </summary>
        public virtual void CancelEffect()
        {
        }

        /// <summary>
        /// Called when manipulation has been started on an element subscribed for an interaction effect.
        /// </summary>
        internal void OnPointerDown(FrameworkElement targetElement, PointerRoutedEventArgs args)
        {
            if (this.CanStartEffect(targetElement, args))
            {
                this.OnStartEffect(targetElement, args);
            }
        }

        /// <summary>
        /// Determines whether an effect can be started on the specified target element.
        /// </summary>
        /// <param name="targetElement">The target element.</param>
        /// <param name="args">The <see cref="PointerRoutedEventArgs"/> instance containing the event data.</param>
        /// <returns>
        ///     <c>true</c> if an effect can be started on the specified target element; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool CanStartEffect(FrameworkElement targetElement, PointerRoutedEventArgs args)
        {
            return true;
        }

        /// <summary>
        /// Called when the interaction effect is started.
        /// </summary>
        /// <param name="targetElement">The target element.</param>
        /// <param name="args">The <see cref="PointerRoutedEventArgs"/> instance containing the event data.</param>
        protected abstract void OnStartEffect(FrameworkElement targetElement, PointerRoutedEventArgs args);
    }
}
