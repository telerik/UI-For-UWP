using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Data
{
    public partial class RadDataBoundListBoxItem
    {
        private bool manipulationStartedHandled = false;
        private Point startPoint;

        internal virtual void OnItemManipulationStarted(ManipulationStartedRoutedEventArgs e)
        {
            RadDataBoundListBox typedOwner = this.Owner as RadDataBoundListBox;
            if (typedOwner != null)
            {
                typedOwner.OnItemManipulationStarted(this, e.Container, e.Position);
            }
        }

        /// <summary>
        /// Do not use.
        /// </summary>
        /// <param name="e">The parameter is not used.</param>
        protected override void OnHolding(HoldingRoutedEventArgs e)
        {
            base.OnHolding(e);

            if (this.typedOwner != null)
            {
                // TODO: Check availability in each version because of the "Do not use" comment
                if (this.ItemState == ItemState.Realized)
                {
                    this.typedOwner.HideCheckBoxesPressIndicator();
                }

                this.typedOwner.OnItemHold(this, e);
            }
        }

        /// <inheritdoc/>
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);

            if (this.typedOwner != null && e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
            {
                this.startPoint = e.GetCurrentPoint(this).Position;

                this.typedOwner.holdTimer.Tick -= this.HoldTimer_Tick;
                this.typedOwner.holdTimer.Tick += this.HoldTimer_Tick;
                this.typedOwner.holdTimer.Start();
            }
        }

        /// <inheritdoc/>
        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            base.OnPointerMoved(e);

            var currentPoint = e.GetCurrentPoint(this).Position;

            var xDistance = this.startPoint.X - currentPoint.X;
            var yDistance = this.startPoint.Y - currentPoint.Y;
            var distance = Math.Sqrt(xDistance * xDistance + yDistance * yDistance);

            if (this.typedOwner != null && this.typedOwner.holdTimer.IsEnabled && distance > 20)
            {
                this.typedOwner.holdTimer.Tick -= this.HoldTimer_Tick;
                this.typedOwner.holdTimer.Stop();
            }
        }

        /// <inheritdoc/>
        protected override void OnPointerCanceled(PointerRoutedEventArgs e)
        {
            base.OnPointerCanceled(e);

            if (this.typedOwner != null)
            {
                this.typedOwner.holdTimer.Stop();
            }
        }

        /// <inheritdoc/>
        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            base.OnPointerReleased(e);

            if (this.typedOwner != null)
            {
                this.typedOwner.holdTimer.Stop();
            }
        }

        /// <summary>
        /// Called when the <see cref="E:System.Windows.Controls.Control.ManipulationStarted"/> event occurs. This member overrides <see cref="M:System.Windows.UIElement.OnManipulationStarted(System.Object,System.Windows.Input.ManipulationStartedEventArgs)"/>.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnManipulationStarted(ManipulationStartedRoutedEventArgs e)
        {
            base.OnManipulationStarted(e);

            if (this.ItemState != ItemState.Realized)
            {
                return;
            }

            this.OnItemManipulationStarted(e);
            this.UpdateCheckBoxVisualState("Pressed");
            this.manipulationStartedHandled = e.Handled;
        }

        /// <summary>
        /// Called when the <see cref="E:System.Windows.Controls.Control.ManipulationDelta"/> event occurs. This member overrides <see cref="M:System.Windows.UIElement.OnManipulationDelta(System.Object,System.Windows.Input.ManipulationDeltaEventArgs)"/>.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnManipulationDelta(ManipulationDeltaRoutedEventArgs e)
        {
            base.OnManipulationDelta(e);

            if (this.ItemState != ItemState.Realized)
            {
                return;
            }

            this.UpdateCheckBoxVisualState("Normal");

            if (this.typedOwner != null)
            {
                this.typedOwner.HideCheckBoxesPressIndicator();
            }
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.Tap" /> event
        /// occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            base.OnTapped(e);

            if (this.ItemState != ItemState.Realized)
            {
                return;
            }

            if (e.Handled || this.manipulationStartedHandled)
            {
                this.manipulationStartedHandled = false;
                return;
            }

            this.OnTap(e.OriginalSource as UIElement, e.OriginalSource as UIElement, e.GetPosition(e.OriginalSource as UIElement));
            this.UpdateCheckBoxVisualState("Normal");

            AutomationPeer itemPeer = FrameworkElementAutomationPeer.FromElement(this);
            if (itemPeer != null)
            {
                itemPeer.RaiseAutomationEvent(AutomationEvents.AutomationFocusChanged);
            }
        }

        /// <summary>
        /// Performs the core Tap implementation. Currently the owner is asked to handle the action.
        /// </summary>
        protected virtual void OnTap(UIElement container, UIElement originalSource, Point hitPoint)
        {
            RadDataBoundListBox owner = this.Owner as RadDataBoundListBox;
            if (owner != null)
            {
                owner.OnItemTap(this, container, originalSource, hitPoint);
            }
        }

        private void HoldTimer_Tick(object sender, object e)
        {
            if (this.typedOwner != null)
            {
                this.typedOwner.holdTimer.Tick -= this.HoldTimer_Tick;
                this.typedOwner.holdTimer.Stop();

                if (this.ItemState == ItemState.Realized)
                {
                    this.typedOwner.HideCheckBoxesPressIndicator();
                }

                this.typedOwner.OnItemMouseHold(this);
            }
        }
    }
}