using System;
using Telerik.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Represents an attachable object, that handles Input events from the owning control instance.
    /// </summary>
    /// <typeparam name="T">The type of the object this behavior can be attached to. Minimum requirement is the <see cref="RadControl"/> type.</typeparam>
    public abstract class InputBehavior<T> : AttachableObject<T> where T : RadControl
    {
        internal InputBehavior(T owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Handles the <see cref="E:{T}.PointerEntered"/> event of the owning instance.
        /// </summary>
        protected internal virtual void OnPointerEntered(PointerRoutedEventArgs args)
        {
        }

        /// <summary>
        /// Handles the <see cref="E:{T}.PointerMoved"/> event of the owning instance.
        /// </summary>
        protected internal virtual void OnPointerMoved(PointerRoutedEventArgs args)
        {
        }

        /// <summary>
        /// Handles the <see cref="E:{T}.PointerExited"/> event of the owning instance.
        /// </summary>
        protected internal virtual void OnPointerExited(PointerRoutedEventArgs args)
        {
        }

        /// <summary>
        /// Handles the <see cref="E:{T}.PointerPressed"/> event of the owning instance.
        /// </summary>
        protected internal virtual void OnPointerPressed(PointerRoutedEventArgs args)
        {
        }

        /// <summary>
        /// Handles the <see cref="E:{T}.HoldCompleted"/> event of the owning instance.
        /// </summary>
        protected internal virtual void OnHoldCompleted(HoldingRoutedEventArgs args)
        {
        }

        /// <summary>
        /// Handles the <see cref="E:{T}.HoldStarted"/> event of the owning instance.
        /// </summary>
        protected internal virtual void OnHoldStarted(HoldingRoutedEventArgs args)
        {
        }

        /// <summary>
        /// Handles the <see cref="E:{T}.Tapped"/> event of the owning instance.
        /// </summary>
        protected internal virtual void OnTapped(TappedRoutedEventArgs args)
        {
        }

        /// <summary>
        /// Handles the <see cref="E:{T}.DoubleTapped"/> event of the owning instance.
        /// </summary>
        protected internal virtual void OnDoubleTapped(DoubleTappedRoutedEventArgs args)
        {
        }

        /// <summary>
        /// Handles the <see cref="E:{T}.ManipulationStarted"/> event of the owning instance.
        /// </summary>
        protected internal virtual void OnManipulationStarted(ManipulationStartedRoutedEventArgs args)
        {
        }

        /// <summary>
        /// Handles the <see cref="E:{T}.ManipulationDelta"/> event of the owning instance.
        /// </summary>
        protected internal virtual void OnManipulationDelta(ManipulationDeltaRoutedEventArgs args)
        {
        }

        /// <summary>
        /// Handles the <see cref="E:{T}.ManipulationCompleted"/> event of the owning instance.
        /// </summary>
        protected internal virtual void OnManipulationCompleted(ManipulationCompletedRoutedEventArgs args)
        {
        }

        /// <summary>
        /// Handles the <see cref="E:{T}.PointerWheelChanged"/> event of the owning instance.
        /// </summary>
        protected internal virtual void OnPointerWheelChanged(PointerRoutedEventArgs args)
        {
        }

        /// <summary>
        /// Handles the <see cref="E:{T}.PointerReleased"/> event of the owning instance.
        /// </summary>
        protected internal virtual void OnPointerReleased(PointerRoutedEventArgs args)
        {
        }

        /// <summary>
        /// Called when the owning instance is loaded in the visual tree.
        /// </summary>
        protected internal virtual void OnLoaded()
        {
        }

        /// <summary>
        /// Called when the owning instance is unloaded from the visual tree.
        /// </summary>
        protected internal virtual void OnUnloaded()
        {
        }
    }
}
